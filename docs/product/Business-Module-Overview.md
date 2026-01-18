# Business Module Overview

## Modules / Epics
- Shopper Experience (Web + Mobile): Browsing, authentication, account self-service, cart/checkout, CMS/help, promotions, notifications for buyers; scope stops at consumer channel UI and relies on admin data setup. [Source: Phụ_Lục_01 r10 c#NO][Source: Phụ_Lục_01 r83 c#NO][Source: Task Breakdown r11 cA][Source: Phụ_Lục_01 r77 c#NO]
- Product Catalog & Search: Category hierarchy, product groups/variants, product detail/search and sales-off lots; boundaries exclude warehouse stocking logic. [Source: Phụ_Lục_01 r43 cITEM][Source: Task Breakdown r17 cC][Source: Task Breakdown r18 cC]
- Cart & Checkout: Cart operations, voucher application, shipping selection, payment (Payoo + COD), invoice issuance, order email. [Source: Phụ_Lục_01 r49 cITEM][Source: Phụ_Lục_01 r55 cITEM][Source: Phụ_Lục_01 r16 cDESCRIPTION]
- Promotions & Loyalty: Promotions/campaign pages, voucher wallet, loyalty points and redemption rules; admin voucher setup in portal. [Source: Task Breakdown r15 cC][Source: Phụ_Lục_01 r38 cITEM][Source: Phụ_Lục_01 r77 cITEM][Source: Phụ_Lục_01 r228 cITEM][Source: Phụ_Lục_01 r230 cITEM]
- Notifications & Deeplinks: In-app/web notifications, email notifications, role-based templates; deeplink behavior unspecified. [Source: Task Breakdown r35 cC][Source: Phụ_Lục_01 r31 cITEM][Source: Phụ_Lục_01 r81 cITEM][Source: Phụ_Lục_01 r217 cITEM]
- CMS & Help Content: CMS pages/blocks and help center articles accessible on web/app; admin content authoring. [Source: Task Breakdown r34 cC][Source: Phụ_Lục_01 r65 cITEM][Source: Phụ_Lục_01 r219 cITEM]
- Portal Operations (Internal Admin): Catalog setup, alerts, promotions, orders, invoices, warehouse, suppliers, debt, shipping, shippers, staff/permissions, CRM, reporting. [Source: Task Breakdown r19 cA][Source: Task Breakdown r20 cC][Source: Task Breakdown r33 cC][Source: Phụ_Lục_01 r155 c#NO]
- Supplier Portal: Supplier authentication, profile, and purchase order lifecycle view. [Source: Task Breakdown r32 cC][Source: Phụ_Lục_01 r231 cITEM][Source: Phụ_Lục_01 r234 cDESCRIPTION]
- Reporting & Analytics: Revenue dashboards segmented by payment, region, customer type, promotion, trend analysis. [Source: Task Breakdown r33 cC][Source: Phụ_Lục_01 r235 cITEM]
- Manual Fallback & GTM: Manual operations when external shipping not connected and Google Tag Manager instrumentation across modules. [Source: Task Breakdown r36 cC]
- Technical Foundations: React frontend, Vietnamese language support, payment/notification SDK configs. Backend framework/database unspecified. [Source: Phụ_Lục_01 r4 cITEM][Source: Phụ_Lục_01 r6 cITEM][Source: Phụ_Lục_01 r16 cDESCRIPTION][Source: Phụ_Lục_01 r13 cITEM]

## Cross-check Between Sheets
- Task Breakdown portal items map to Phụ_Lục_01 sections: e.g., Quản lý nhóm hàng/product/order/warehouse/supplier/transport align with portal items 3–10. [Source: Task Breakdown r19 cC][Source: Phụ_Lục_01 r161 cITEM][Source: Phụ_Lục_01 r173 cITEM]
- Customer-facing rows (giỏ hàng, xác nhận đơn, tích điểm, đánh giá) align with Phụ_Lục_01 sections 4–7 and 10–11 for both web and mobile. [Source: Task Breakdown r13 cC][Source: Phụ_Lục_01 r49 cITEM][Source: Phụ_Lục_01 r77 cITEM]
- Task Breakdown references to Mục 10 for staff/customer management actually correspond to portal items 11–12 in Phụ_Lục_01 (permissions, CRM); adjust numbering in implementation. [Source: Task Breakdown r30 cC][Source: Task Breakdown r31 cC][Source: Phụ_Lục_01 r206 cITEM][Source: Phụ_Lục_01 r211 cITEM]
- Phụ_Lục_01 contains extra scope not listed in Task Breakdown: content management (articles/pages), voucher configuration, supplier notification templates, and detailed reporting slices. [Source: Phụ_Lục_01 r219 cITEM][Source: Phụ_Lục_01 r228 cDESCRIPTION][Source: Phụ_Lục_01 r191 cDESCRIPTION][Source: Phụ_Lục_01 r235 cITEM]
- Task Breakdown includes Manual System Alternative & GTM and deeplink-heavy notifications that are not detailed in Phụ_Lục_01, requiring clarification. [Source: Task Breakdown r35 cC][Source: Task Breakdown r36 cD]

## Actors / Roles
- Người mua (Customer): Browses, orders, manages account/addresses/vouchers/points, reviews purchases on web/app. [Source: Task Breakdown r11 cA][Source: Phụ_Lục_01 r10 c#NO][Source: Phụ_Lục_01 r29 cITEM]
- Admin/Supper Admin: Configures portal access, creates staff accounts, oversees all modules. [Source: Phụ_Lục_01 r160 cDESCRIPTION][Source: Task Breakdown r30 cC]
- Nhân viên kho (Warehouse staff): Manages stock, locations, handles alerts on expiry/low stock. [Source: Phụ_Lục_01 r182 cDESCRIPTION][Source: Phụ_Lục_01 r183 cDESCRIPTION]
- Nhân viên vận hành đơn hàng: Views and updates orders, invoices, shipping status. [Source: Phụ_Lục_01 r173 cITEM][Source: Phụ_Lục_01 r179 cITEM]
- Shipper (in-house team): Assigned deliveries, updates pickup/delivery statuses via mobile UI. [Source: Task Breakdown r28 cC][Source: Task Breakdown r29 cC][Source: Phụ_Lục_01 r203 cDESCRIPTION]
- Đơn vị vận chuyển (External carriers): Ahamove/Express/VNPost/ViettelPost configured per warehouse and delivery method. [Source: Phụ_Lục_01 r57 cDESCRIPTION][Source: Phụ_Lục_01 r200 cDESCRIPTION]
- Nhà cung cấp (Supplier): Provides goods, receives POs, updates fulfillment via supplier portal. [Source: Task Breakdown r25 cC][Source: Phụ_Lục_01 r185 cITEM][Source: Phụ_Lục_01 r231 cITEM]
- Cổng thanh toán (Payoo) & ngân hàng: Processes online payments. [Source: Phụ_Lục_01 r16 cDESCRIPTION][Source: Phụ_Lục_01 r89 cDESCRIPTION]
- Hệ thống thông báo (FCM/Email/SMS OTP): Delivers app notifications and OTP/password flows. [Source: Phụ_Lục_01 r13 cITEM][Source: Phụ_Lục_01 r31 cITEM][Source: Phụ_Lục_01 r22 cITEM]
- Cổng hóa đơn điện tử: Receives invoice payloads after delivery for GTGT issuance. [Source: Phụ_Lục_01 r61 cDESCRIPTION][Source: Phụ_Lục_01 r133 cDESCRIPTION]

## Glossary (VN -> EN)
- "Giỏ hàng" -> Shopping cart with selectable items/quantities. [Source: Phụ_Lục_01 r49 cITEM]
- "Sổ địa chỉ" -> Customer address book (max 5 entries, default flag). [Source: Phụ_Lục_01 r37 cDESCRIPTION]
- "Voucher" -> Discount or shipping coupon applied at cart/checkout; admin-defined. [Source: Phụ_Lục_01 r38 cITEM][Source: Phụ_Lục_01 r228 cDESCRIPTION]
- "Tích điểm" -> Loyalty points earned per order (1000 VND = 1 điểm) convertible to vouchers. [Source: Phụ_Lục_01 r39 cDESCRIPTION][Source: Phụ_Lục_01 r230 cDESCRIPTION]
- "Đơn vị vận chuyển" -> Configured carrier services for deliveries. [Source: Phụ_Lục_01 r197 cITEM]
- "Hình Thức Giao Hàng" -> Delivery method (Siêu tốc/Nhanh/Tiết kiệm) chosen at checkout. [Source: Phụ_Lục_01 r57 cDESCRIPTION][Source: Phụ_Lục_01 r199 cDESCRIPTION]
- "Lô hàng" -> Inventory lot/batch with SKU, supplier, cost, quantities, location. [Source: Phụ_Lục_01 r169 cDESCRIPTION]
- "Hóa đơn điện tử (GTGT)" -> Electronic VAT invoice sent via external API after delivery. [Source: Phụ_Lục_01 r61 cDESCRIPTION][Source: Phụ_Lục_01 r179 cDESCRIPTION]
- "Trang nhà cung cấp" -> Supplier self-service portal for auth/profile/PO tracking. [Source: Phụ_Lục_01 r231 cITEM][Source: Phụ_Lục_01 r234 cDESCRIPTION]
- "Manual System Alternative" -> Manual fallback flows when shipping providers are unavailable; details TBD. [Source: Task Breakdown r36 cD]
