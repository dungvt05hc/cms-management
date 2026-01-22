"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import {
  getCart,
  getAddresses,
  getShippingMethods,
  checkoutPreview,
  checkoutSubmit,
  Cart,
  Address,
  ShippingMethod,
  CheckoutTotals,
  PaymentMethod,
} from "@/lib/api";

export default function CheckoutPage() {
  const router = useRouter();
  const [cart, setCart] = useState<Cart | null>(null);
  const [addresses, setAddresses] = useState<Address[]>([]);
  const [shippingMethods, setShippingMethods] = useState<ShippingMethod[]>([]);
  const [selectedAddress, setSelectedAddress] = useState<string>("");
  const [selectedShippingMethod, setSelectedShippingMethod] = useState<string>("");
  const [selectedShippingCarrier, setSelectedShippingCarrier] = useState<string>("");
  const [selectedPaymentMethod, setSelectedPaymentMethod] = useState<PaymentMethod>(PaymentMethod.COD);
  const [discountCode, setDiscountCode] = useState("");
  const [shippingCode, setShippingCode] = useState("");
  const [notes, setNotes] = useState("");
  const [totals, setTotals] = useState<CheckoutTotals | null>(null);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadData();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    if (selectedAddress && selectedShippingMethod && selectedShippingCarrier) {
      loadPreview();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selectedAddress, selectedShippingMethod, selectedShippingCarrier, discountCode, shippingCode]);

  const loadData = async () => {
    setLoading(true);
    setError(null);
    try {
      const token = localStorage.getItem("authToken");
      if (!token) {
        router.push("/account/login");
        return;
      }

      const [cartData, addressesData, shippingMethodsData] = await Promise.all([
        getCart(token),
        getAddresses(token),
        getShippingMethods(),
      ]);

      setCart(cartData);
      setAddresses(addressesData);
      setShippingMethods(shippingMethodsData);

      // Auto-select default address
      const defaultAddress = addressesData.find((a) => a.isDefault);
      if (defaultAddress) {
        setSelectedAddress(defaultAddress.id);
      }

      // Auto-select first shipping method
      if (shippingMethodsData.length > 0) {
        setSelectedShippingMethod(shippingMethodsData[0].code);
        if (shippingMethodsData[0].carriers.length > 0) {
          setSelectedShippingCarrier(shippingMethodsData[0].carriers[0].code);
        }
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load checkout data");
    } finally {
      setLoading(false);
    }
  };

  const loadPreview = async () => {
    try {
      const token = localStorage.getItem("authToken");
      if (!token) return;

      const totalsData = await checkoutPreview(
        token,
        selectedAddress,
        selectedShippingMethod,
        selectedShippingCarrier,
        discountCode || undefined,
        shippingCode || undefined
      );
      setTotals(totalsData);
    } catch (err) {
      console.error("Failed to load preview:", err);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedAddress || !selectedShippingMethod || !selectedShippingCarrier) {
      setError("Please select address and shipping method");
      return;
    }

    setSubmitting(true);
    setError(null);
    try {
      const token = localStorage.getItem("authToken");
      if (!token) {
        router.push("/account/login");
        return;
      }

      const result = await checkoutSubmit(
        token,
        selectedAddress,
        selectedShippingMethod,
        selectedShippingCarrier,
        selectedPaymentMethod,
        discountCode || undefined,
        shippingCode || undefined,
        notes || undefined
      );

      if (result.orderId) {
        // COD: Navigate to order confirmation
        router.push(`/account/orders/${result.orderId}`);
      } else if (result.paymentUrl) {
        // Payoo: Redirect to payment URL
        window.location.href = result.paymentUrl;
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to submit checkout");
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div data-testid="checkout-loading">Loading checkout...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div data-testid="checkout-error" className="text-red-600">
          {error}
        </div>
      </div>
    );
  }

  if (!cart || cart.items.filter((i) => i.selected).length === 0) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div data-testid="checkout-empty">Your cart is empty</div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8" data-testid="checkout-page">
      <h1 className="text-3xl font-bold mb-8">Checkout</h1>

      <form onSubmit={handleSubmit} className="space-y-8">
        {/* Address Selection */}
        <div data-testid="address-section">
          <h2 className="text-xl font-semibold mb-4">Shipping Address</h2>
          <div className="space-y-2">
            {addresses.map((address) => (
              <label key={address.id} className="flex items-start space-x-3 p-4 border rounded cursor-pointer hover:bg-gray-50">
                <input
                  type="radio"
                  name="address"
                  value={address.id}
                  checked={selectedAddress === address.id}
                  onChange={(e) => setSelectedAddress(e.target.value)}
                  data-testid={`address-${address.id}`}
                  className="mt-1"
                />
                <div>
                  <div className="font-medium">{address.fullName} - {address.phone}</div>
                  <div className="text-sm text-gray-600">
                    {address.addressLine}, {address.ward}, {address.district}, {address.city}
                  </div>
                </div>
              </label>
            ))}
          </div>
        </div>

        {/* Shipping Method Selection */}
        <div data-testid="shipping-section">
          <h2 className="text-xl font-semibold mb-4">Shipping Method</h2>
          <div className="space-y-4">
            {shippingMethods.map((method) => (
              <div key={method.code} className="border rounded p-4">
                <div className="font-medium mb-2">{method.name}</div>
                <div className="space-y-2">
                  {method.carriers.map((carrier) => (
                    <label
                      key={carrier.code}
                      className="flex items-center space-x-3 p-2 border rounded cursor-pointer hover:bg-gray-50"
                    >
                      <input
                        type="radio"
                        name="shipping"
                        checked={selectedShippingMethod === method.code && selectedShippingCarrier === carrier.code}
                        onChange={() => {
                          setSelectedShippingMethod(method.code);
                          setSelectedShippingCarrier(carrier.code);
                        }}
                        data-testid={`shipping-${method.code}-${carrier.code}`}
                      />
                      <div>
                        <div className="font-medium">{carrier.name}</div>
                        {carrier.description && <div className="text-sm text-gray-600">{carrier.description}</div>}
                        {carrier.supportsCOD && <span className="text-xs text-green-600">Supports COD</span>}
                      </div>
                    </label>
                  ))}
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Voucher Codes */}
        <div data-testid="voucher-section">
          <h2 className="text-xl font-semibold mb-4">Voucher Codes</h2>
          <div className="space-y-2">
            <input
              type="text"
              placeholder="Discount code"
              value={discountCode}
              onChange={(e) => setDiscountCode(e.target.value)}
              data-testid="discount-code-input"
              className="w-full p-2 border rounded"
            />
            <input
              type="text"
              placeholder="Shipping code"
              value={shippingCode}
              onChange={(e) => setShippingCode(e.target.value)}
              data-testid="shipping-code-input"
              className="w-full p-2 border rounded"
            />
          </div>
        </div>

        {/* Payment Method */}
        <div data-testid="payment-section">
          <h2 className="text-xl font-semibold mb-4">Payment Method</h2>
          <div className="space-y-2">
            <label className="flex items-center space-x-3 p-4 border rounded cursor-pointer hover:bg-gray-50">
              <input
                type="radio"
                name="payment"
                checked={selectedPaymentMethod === PaymentMethod.COD}
                onChange={() => setSelectedPaymentMethod(PaymentMethod.COD)}
                data-testid="payment-cod"
              />
              <span>Cash on Delivery (COD)</span>
            </label>
            <label className="flex items-center space-x-3 p-4 border rounded cursor-pointer hover:bg-gray-50">
              <input
                type="radio"
                name="payment"
                checked={selectedPaymentMethod === PaymentMethod.Payoo}
                onChange={() => setSelectedPaymentMethod(PaymentMethod.Payoo)}
                data-testid="payment-payoo"
              />
              <span>Payoo Online Payment</span>
            </label>
          </div>
        </div>

        {/* Notes */}
        <div data-testid="notes-section">
          <h2 className="text-xl font-semibold mb-4">Order Notes</h2>
          <textarea
            placeholder="Additional notes (optional)"
            value={notes}
            onChange={(e) => setNotes(e.target.value)}
            data-testid="notes-input"
            className="w-full p-2 border rounded"
            rows={3}
          />
        </div>

        {/* Order Summary */}
        {totals && (
          <div data-testid="order-summary" className="border rounded p-4 bg-gray-50">
            <h2 className="text-xl font-semibold mb-4">Order Summary</h2>
            <div className="space-y-2">
              <div className="flex justify-between">
                <span>Subtotal:</span>
                <span>${totals.subtotal.toFixed(2)}</span>
              </div>
              {totals.discountAmount > 0 && (
                <div className="flex justify-between text-green-600">
                  <span>Discount ({totals.discountVoucherCode}):</span>
                  <span>-${totals.discountAmount.toFixed(2)}</span>
                </div>
              )}
              <div className="flex justify-between">
                <span>Shipping:</span>
                <span>${totals.shippingFee.toFixed(2)}</span>
              </div>
              {totals.shippingDiscount > 0 && (
                <div className="flex justify-between text-green-600">
                  <span>Shipping Discount ({totals.shippingVoucherCode}):</span>
                  <span>-${totals.shippingDiscount.toFixed(2)}</span>
                </div>
              )}
              <div className="flex justify-between font-bold text-lg pt-2 border-t">
                <span>Total:</span>
                <span data-testid="total-amount">${totals.total.toFixed(2)}</span>
              </div>
            </div>
          </div>
        )}

        {/* Submit Button */}
        <button
          type="submit"
          disabled={submitting || !selectedAddress || !selectedShippingMethod || !selectedShippingCarrier}
          data-testid="submit-checkout"
          className="w-full bg-blue-600 text-white py-3 px-6 rounded font-semibold hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed"
        >
          {submitting ? "Processing..." : selectedPaymentMethod === PaymentMethod.COD ? "Place Order" : "Proceed to Payment"}
        </button>
      </form>
    </div>
  );
}
