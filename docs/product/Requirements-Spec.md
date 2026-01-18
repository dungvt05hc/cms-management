# Requirements Specification

## Tech Stack & Constraints
- Goals: Establish baseline stack and required SDKs for all channels. [Source: Phụ_Lục_01 r2 cA][Source: Phụ_Lục_01 r4 cITEM]
- Assumptions/Constraints:
  - Backend: .NET 9 ASP.NET Core Web API (apps/api). *Team decision (not in Excel); aligns with monorepo plan.*
  - Frontend: React + Vite + TypeScript (apps/web). *Team decision (not in Excel); aligns with monorepo plan.*
  - Database: Postgres, provisioned via docker-compose (infra/docker). *Team decision (not in Excel); aligns with monorepo plan.*
  - Frontend uses HTML/CSS/JavaScript with ReactJS; Vietnamese language default. [Source: Phụ_Lục_01 r4 cDESCRIPTION][Source: Phụ_Lục_01 r6 cITEM]
  - Payoo handles online payments; SDK for fast shipping and GA/FCM configured per channel. [Source: Phụ_Lục_01 r16 cDESCRIPTION][Source: Phụ_Lục_01 r13 cITEM][Source: Phụ_Lục_01 r15 cITEM]
  - Device “Attendance Machine” is listed without usage detail (clarify before integration). [Source: Phụ_Lục_01 r7 cITEM]

## Module: Authentication (Web & App)
- Goals: Secure access with OTP-backed onboarding for buyers. [Source: Phụ_Lục_01 r18 cITEM][Source: Phụ_Lục_01 r90 cITEM]
- Functional Requirements:
  1. FR-AUTH-1: Support login via phone number or email plus password. [Source: Phụ_Lục_01 r19 cDESCRIPTION][Source: Phụ_Lục_01 r91 cDESCRIPTION]
  2. FR-AUTH-2: Provide password recovery prioritizing email, with SMS OTP fallback. [Source: Phụ_Lục_01 r20 cDESCRIPTION][Source: Phụ_Lục_01 r92 cDESCRIPTION]
  3. FR-AUTH-3: Registration flow requires OTP verification then collecting name/email/phone/password. [Source: Phụ_Lục_01 r21 cDESCRIPTION][Source: Phụ_Lục_01 r93 cDESCRIPTION]
  4. FR-AUTH-4: OTP confirmation screen prior to account activation. [Source: Phụ_Lục_01 r22 cITEM][Source: Phụ_Lục_01 r94 cITEM]
- Business Rules:
  1. BR-AUTH-1: Super Admin creates staff accounts in portal; staff cannot self-register. [Source: Phụ_Lục_01 r160 cDESCRIPTION]
- Acceptance Criteria:
  - Users cannot proceed past registration without valid OTP; login fails with incorrect credentials; password reset emails include secure token; SMS OTP used when email not available. [Source: Phụ_Lục_01 r19 cDESCRIPTION][Source: Phụ_Lục_01 r20 cDESCRIPTION]

## Module: Customer Account & Self-Service
- Goals: Allow customers to manage profile, notifications, orders, addresses, vouchers, loyalty, and product interactions. [Source: Task Breakdown r11 cC][Source: Phụ_Lục_01 r29 cITEM]
- Functional Requirements:
  1. FR-ACC-1: View/update profile fields (name, email, nationality), excluding phone changes. [Source: Phụ_Lục_01 r30 cDESCRIPTION]
  2. FR-ACC-2: View notifications grouped by promotions/orders/system. [Source: Phụ_Lục_01 r31 cDESCRIPTION]
  3. FR-ACC-3: View order history grouped by status; view order detail, cancel when eligible, confirm receipt, and reorder past orders. [Source: Phụ_Lục_01 r32 cDESCRIPTION][Source: Phụ_Lục_01 r33 cDESCRIPTION][Source: Phụ_Lục_01 r34 cDESCRIPTION][Source: Phụ_Lục_01 r36 cDESCRIPTION]
  4. FR-ACC-4: Manage address book (add/update/delete, set default) with max five addresses. [Source: Phụ_Lục_01 r37 cDESCRIPTION]
  5. FR-ACC-5: View voucher wallet (received, CS gifted, or converted from points). [Source: Phụ_Lục_01 r38 cDESCRIPTION]
  6. FR-ACC-6: Display loyalty points, accumulation per order, and available conversions. [Source: Phụ_Lục_01 r39 cDESCRIPTION][Source: Phụ_Lục_01 r230 cDESCRIPTION]
  7. FR-ACC-7: Manage product reviews (5-star, comment, images) and see own pending/published reviews. [Source: Phụ_Lục_01 r40 cDESCRIPTION]
  8. FR-ACC-8: Track recently viewed products and favorites list. [Source: Phụ_Lục_01 r41 cDESCRIPTION][Source: Phụ_Lục_01 r42 cDESCRIPTION]
- Business Rules:
  1. BR-ACC-1: Address book limited to five entries; one default required. [Source: Phụ_Lục_01 r37 cDESCRIPTION]
  2. BR-ACC-2: Order cancellation allowed only in status “Đang xử lý”; auto receipt confirmation after [n] hours if customer does not confirm. [Source: Phụ_Lục_01 r34 cDESCRIPTION][Source: Phụ_Lục_01 r35 cDESCRIPTION]
  3. BR-ACC-3: Points accrue at 1 point per 1,000 VND of successful order value; conversion thresholds configurable. [Source: Phụ_Lục_01 r39 cDESCRIPTION][Source: Phụ_Lục_01 r230 cDESCRIPTION]
- Acceptance Criteria:
  - Profile updates save except phone; notifications filterable by category; address CRUD enforces max count; cancel button hidden when status not “Đang xử lý”; reorder copies items into cart for editable quantities. [Source: Phụ_Lục_01 r30 cDESCRIPTION][Source: Phụ_Lục_01 r34 cDESCRIPTION][Source: Phụ_Lục_01 r36 cDESCRIPTION]

## Module: Product Catalog & Search (Shopper)
- Goals: Enable discovery of products via categories, groups, lists, detail, and search suggestions. [Source: Task Breakdown r17 cC][Source: Phụ_Lục_01 r43 cITEM]
- Functional Requirements:
  1. FR-CAT-1: Maintain unlimited-depth category hierarchy with navigation to associated products and brands. [Source: Phụ_Lục_01 r44 cDESCRIPTION]
  2. FR-CAT-2: Define product groups per category for shared attributes (e.g., type, characteristics). [Source: Phụ_Lục_01 r45 cDESCRIPTION]
  3. FR-CAT-3: Present product lists with image/name/price/promo/CTA, infinite scroll, and price sort. [Source: Phụ_Lục_01 r46 cDESCRIPTION]
  4. FR-CAT-4: Show product detail including media, variants (quy cách) with pricing, shipping cost/time to address, comments (after admin approval), ratings, and recommendations. [Source: Phụ_Lục_01 r47 cDESCRIPTION]
  5. FR-CAT-5: Provide search box with suggestion results while typing. [Source: Phụ_Lục_01 r48 cDESCRIPTION]
  6. FR-CAT-6: Surface sale-off/expiry-aware product flags from lots/batches. [Source: Task Breakdown r18 cD][Source: Phụ_Lục_01 r169 cDESCRIPTION]
- Business Rules:
  1. BR-CAT-1: Comments and ratings appear only after admin approval. [Source: Phụ_Lục_01 r171 cDESCRIPTION][Source: Phụ_Lục_01 r172 cDESCRIPTION]
- Acceptance Criteria:
  - Category navigation drills down to products; list supports infinite scroll and price sort; detail page shows variant-specific prices and shipping estimate; search suggestions update with input. [Source: Phụ_Lục_01 r46 cDESCRIPTION][Source: Phụ_Lục_01 r47 cDESCRIPTION][Source: Phụ_Lục_01 r48 cDESCRIPTION]

## Module: Cart
- Goals: Manage selected products before checkout with clear pricing and voucher inputs. [Source: Phụ_Lục_01 r49 cITEM]
- Functional Requirements:
  1. FR-CART-1: Display cart table with selection checkbox, product info, quantity controls, price, subtotal, delete. [Source: Phụ_Lục_01 r50 cDESCRIPTION]
  2. FR-CART-2: Allow quantity adjustment and variant selection per line. [Source: Phụ_Lục_01 r51 cDESCRIPTION]
  3. FR-CART-3: Show subtotal for selected items only. [Source: Phụ_Lục_01 r52 cDESCRIPTION]
  4. FR-CART-4: Accept voucher input/selection for discount and shipping voucher types. [Source: Phụ_Lục_01 r53 cDESCRIPTION]
  5. FR-CART-5: Proceed to checkout with selected items only. [Source: Phụ_Lục_01 r54 cDESCRIPTION]
- Business Rules:
  1. BR-CART-1: Exactly two voucher types supported: discount and shipping. [Source: Phụ_Lục_01 r53 cDESCRIPTION]
- Acceptance Criteria:
  - Subtotal recalculates on quantity change; invalid vouchers show error; checkout button disabled when no items selected. [Source: Phụ_Lục_01 r52 cDESCRIPTION][Source: Phụ_Lục_01 r53 cDESCRIPTION]

## Module: Checkout & Payments
- Goals: Collect delivery, promotion, invoicing, payment choices, and confirm orders. [Source: Phụ_Lục_01 r55 cITEM]
- Functional Requirements:
  1. FR-CHK-1: Select existing address or add new (respecting address limit) for delivery. [Source: Phụ_Lục_01 r56 cDESCRIPTION][Source: Phụ_Lục_01 r37 cDESCRIPTION]
  2. FR-CHK-2: Choose delivery method and carrier with returned ETA and shipping fee. [Source: Phụ_Lục_01 r57 cDESCRIPTION]
  3. FR-CHK-3: Present order summary of items, variants, quantities, totals. [Source: Phụ_Lục_01 r58 cDESCRIPTION]
  4. FR-CHK-4: Apply vouchers (discount and shipping) with max one per type. [Source: Phụ_Lục_01 r59 cDESCRIPTION]
  5. FR-CHK-5: Calculate payable amount with product total, discounts, shipping fee, and shipping discount cap. [Source: Phụ_Lục_01 r60 cDESCRIPTION]
  6. FR-CHK-6: Capture VAT invoice data, queue e-invoice API push 10 days after delivery, and persist for reuse. [Source: Phụ_Lục_01 r61 cDESCRIPTION]
  7. FR-CHK-7: Allow customer notes for the order. [Source: Phụ_Lục_01 r62 cDESCRIPTION]
  8. FR-CHK-8: Offer COD (cash or card per carrier config) and online Payoo payment; create order only on successful payment, else restore cart. [Source: Phụ_Lục_01 r63 cDESCRIPTION][Source: Phụ_Lục_01 r16 cDESCRIPTION]
  9. FR-CHK-9: Send order confirmation email to customer and warehouse staff. [Source: Phụ_Lục_01 r64 cDESCRIPTION]
- Business Rules:
  1. BR-CHK-1: Shipping voucher discount cannot exceed shipping fee. [Source: Phụ_Lục_01 r60 cDESCRIPTION]
  2. BR-CHK-2: Payment failure returns items to cart and no order record is created. [Source: Phụ_Lục_01 r63 cDESCRIPTION]
- Acceptance Criteria:
  - Delivery selection computes ETA and fee; voucher validation enforces one per type; payable total matches formula; e-invoice requests fire after 10 days for delivered orders; payment success changes order status to “Đang xử lý”. [Source: Phụ_Lục_01 r57 cDESCRIPTION][Source: Phụ_Lục_01 r60 cDESCRIPTION][Source: Phụ_Lục_01 r61 cDESCRIPTION]

## Module: CMS & Help Content
- Goals: Serve CMS pages/blocks and help center articles on web/app. [Source: Task Breakdown r34 cC][Source: Phụ_Lục_01 r65 cITEM]
- Functional Requirements:
  1. FR-CMS-1: Render CMS home, categories, article lists, article detail, and reusable blocks. [Source: Phụ_Lục_01 r66 cITEM][Source: Phụ_Lục_01 r69 cITEM][Source: Phụ_Lục_01 r70 cITEM]
  2. FR-CMS-2: Expose Help Center with terms, sales policy, FAQ/contact options including chat/phone buttons. [Source: Phụ_Lục_01 r71 cITEM][Source: Phụ_Lục_01 r75 cDESCRIPTION]
- Acceptance Criteria:
  - CMS content displays on both web and app; help links open configured chat apps/phone numbers. [Source: Phụ_Lục_01 r65 cITEM][Source: Phụ_Lục_01 r75 cDESCRIPTION]

## Module: Promotions & Loyalty (Shopper)
- Goals: Present promotions and allow voucher/point usage. [Source: Phụ_Lục_01 r77 cITEM][Source: Phụ_Lục_01 r38 cITEM]
- Functional Requirements:
  1. FR-PROMO-1: List promotion programs (e.g., Mua 2 tặng 1, Xả kho) with detail pages. [Source: Phụ_Lục_01 r78 cDESCRIPTION]
  2. FR-PROMO-2: Show products within a selected promotion with purchase flow. [Source: Phụ_Lục_01 r79 cDESCRIPTION]
  3. FR-PROMO-3: Allow applying earned vouchers and loyalty redemptions at cart/checkout. [Source: Phụ_Lục_01 r38 cDESCRIPTION][Source: Phụ_Lục_01 r230 cDESCRIPTION]
- Acceptance Criteria:
  - Promotion list links to product lists; voucher balances visible and selectable; applied promotion reflected in totals. [Source: Phụ_Lục_01 r78 cDESCRIPTION][Source: Phụ_Lục_01 r79 cDESCRIPTION]

## Module: Notifications & Messaging
- Goals: Deliver notifications per role across channels (app, web, email). [Source: Task Breakdown r35 cC][Source: Phụ_Lục_01 r80 cITEM]
- Functional Requirements:
  1. FR-NOTI-1: Admin configures notifications per module and role (promotions, orders, system). [Source: Phụ_Lục_01 r81 cDESCRIPTION][Source: Phụ_Lục_01 r217 cDESCRIPTION]
  2. FR-NOTI-2: Send email notifications for customer orders. [Source: Phụ_Lục_01 r82 cITEM][Source: Phụ_Lục_01 r64 cDESCRIPTION]
  3. FR-NOTI-3: Support manual broadcast to selected customer/supplier groups using templates. [Source: Phụ_Lục_01 r216 cDESCRIPTION][Source: Phụ_Lục_01 r191 cDESCRIPTION]
- Business Rules:
  1. BR-NOTI-1: Templates preconfigured by admin; staff select recipients by group or individual. [Source: Phụ_Lục_01 r216 cDESCRIPTION]
- Acceptance Criteria:
  - Notifications logged with delivery channel; order emails include order summary; manual sends respect selected recipient scope. [Source: Phụ_Lục_01 r82 cITEM][Source: Phụ_Lục_01 r216 cDESCRIPTION]

## Module: Portal - Catalog & Pricing
- Goals: Let admins configure catalog, pricing, alerts, and promo settings. [Source: Task Breakdown r19 cC][Source: Phụ_Lục_01 r161 cITEM]
- Functional Requirements:
  1. FR-PCAT-1: CRUD category hierarchy and product groups. [Source: Phụ_Lục_01 r162 cDESCRIPTION]
  2. FR-PCAT-2: Manage product list/detail including SKU, media, variants with buy/sell prices, expiry/sell-by dates, rating fields. [Source: Phụ_Lục_01 r166 cDESCRIPTION][Source: Phụ_Lục_01 r164 cDESCRIPTION]
  3. FR-PCAT-3: Configure alerts for sell-by expiry and low stock min/max thresholds. [Source: Phụ_Lục_01 r167 cDESCRIPTION]
  4. FR-PCAT-4: Create promotions/discounts by expiry, product sets, schedule by date/day/hour. [Source: Phụ_Lục_01 r168 cDESCRIPTION]
  5. FR-PCAT-5: Manage product lots with supplier, cost, quantities, warehouse location. [Source: Phụ_Lục_01 r169 cDESCRIPTION]
  6. FR-PCAT-6: Set delivery distance limits and available delivery methods. [Source: Phụ_Lục_01 r170 cDESCRIPTION]
  7. FR-PCAT-7: Moderate ratings and comments before they surface. [Source: Phụ_Lục_01 r171 cDESCRIPTION][Source: Phụ_Lục_01 r172 cDESCRIPTION]
- Business Rules:
  1. BR-PCAT-1: Stock alert triggers when quantity <= MIN; purchase quantity cannot exceed MAX. [Source: Phụ_Lục_01 r167 cDESCRIPTION]
  2. BR-PCAT-2: Promotions can be scheduled by day-of-month, weekday, and time windows. [Source: Phụ_Lục_01 r168 cDESCRIPTION]
- Acceptance Criteria:
  - Alerts generated for expiry/min stock; promotion schedule enforces configured windows; lot records include location code; unapproved comments stay hidden. [Source: Phụ_Lục_01 r167 cDESCRIPTION][Source: Phụ_Lục_01 r169 cDESCRIPTION][Source: Phụ_Lục_01 r172 cDESCRIPTION]

## Module: Portal - Order Management
- Goals: Control end-to-end order lifecycle, carrier selection, invoicing, and KPIs. [Source: Task Breakdown r23 cC][Source: Phụ_Lục_01 r173 cITEM]
- Functional Requirements:
  1. FR-PORD-1: Order list with filters (category/product/status/warehouse/region/voucher/customer/carrier/payment) and pagination/export. [Source: Phụ_Lục_01 r174 cDESCRIPTION]
  2. FR-PORD-2: Order detail with full payment, shipping, voucher, product lines, shipping fee payable. [Source: Phụ_Lục_01 r175 cDESCRIPTION]
  3. FR-PORD-3: Manage status transitions mapping customer vs. internal states, including cancel/return reasons. [Source: Phụ_Lục_01 r176 cDESCRIPTION]
  4. FR-PORD-4: Track shipment via carrier-provided links. [Source: Phụ_Lục_01 r177 cITEM]
  5. FR-PORD-5: Reassign carriers before pickup without changing customer shipping fee. [Source: Phụ_Lục_01 r178 cDESCRIPTION]
  6. FR-PORD-6: Manage electronic invoices list and export. [Source: Phụ_Lục_01 r179 cDESCRIPTION]
  7. FR-PORD-7: Order status reports (cancel reasons, success, prep, packed, in transit) and repeat cancellations. [Source: Phụ_Lục_01 r180 cDESCRIPTION]
- Business Rules:
  1. BR-PORD-1: Customer statuses map to internal states as defined; cancel reasons preconfigured. [Source: Phụ_Lục_01 r176 cDESCRIPTION]
- Acceptance Criteria:
  - Filters return correct subsets; status changes enforce mapping; carrier reassignment preserves customer fee; invoice exports succeed per month. [Source: Phụ_Lục_01 r174 cDESCRIPTION][Source: Phụ_Lục_01 r178 cDESCRIPTION][Source: Phụ_Lục_01 r179 cDESCRIPTION]

## Module: Portal - Warehouse & Inventory
- Goals: Maintain warehouses, stock movements, and storage locations. [Source: Task Breakdown r24 cC][Source: Phụ_Lục_01 r181 cITEM]
- Functional Requirements:
  1. FR-PINV-1: List warehouses with name/address/owner/phone and staff accounts. [Source: Phụ_Lục_01 r182 cDESCRIPTION]
  2. FR-PINV-2: Track movements (import from suppliers, export, returns, quarantined stock) and enforce MIN/MAX ordering from alerts. [Source: Phụ_Lục_01 r183 cDESCRIPTION][Source: Phụ_Lục_01 r167 cDESCRIPTION]
  3. FR-PINV-3: Manage warehouse location codes (e.g., A.1.2.3.4) with auto-increment. [Source: Phụ_Lục_01 r184 cDESCRIPTION]
- Acceptance Criteria:
  - Movement types recorded with quantities; reorder attempts above MAX blocked; location codes unique and hierarchical. [Source: Phụ_Lục_01 r183 cDESCRIPTION][Source: Phụ_Lục_01 r184 cDESCRIPTION]

## Module: Portal - Supplier Management
- Goals: Administer supplier master data and purchase order lifecycle with messaging. [Source: Task Breakdown r25 cC][Source: Phụ_Lục_01 r185 cITEM]
- Functional Requirements:
  1. FR-PSUP-1: CRUD suppliers and groupings. [Source: Phụ_Lục_01 r186 cDESCRIPTION][Source: Phụ_Lục_01 r187 cDESCRIPTION]
  2. FR-PSUP-2: Maintain supplier details including banking info. [Source: Phụ_Lục_01 r188 cDESCRIPTION]
  3. FR-PSUP-3: Track supply order history with statuses (draft to paid/late) and product quantities/prices. [Source: Phụ_Lục_01 r189 cDESCRIPTION]
  4. FR-PSUP-4: Auto notifications to suppliers/staff for PO events and debt reconciliation. [Source: Phụ_Lục_01 r190 cDESCRIPTION]
  5. FR-PSUP-5: Manual email/app notifications using templates (thanks, debt, surveys) to groups or individuals. [Source: Phụ_Lục_01 r191 cDESCRIPTION]
- Acceptance Criteria:
  - Supplier records include bank info; PO statuses follow defined set; notifications logged and target selected recipients. [Source: Phụ_Lục_01 r188 cDESCRIPTION][Source: Phụ_Lục_01 r189 cDESCRIPTION][Source: Phụ_Lục_01 r191 cDESCRIPTION]

## Module: Portal - Debt & Reconciliation
- Goals: Provide debt visibility across customers, suppliers, carriers, and payment gateways. [Source: Task Breakdown r26 cD][Source: Phụ_Lục_01 r192 cITEM]
- Functional Requirements:
  1. FR-PDEBT-1: View customer debts within order module. [Source: Phụ_Lục_01 r193 cDESCRIPTION]
  2. FR-PDEBT-2: View supplier debts within supplier module. [Source: Phụ_Lục_01 r194 cDESCRIPTION]
  3. FR-PDEBT-3: Track carrier COD owed and shipping fees by period. [Source: Phụ_Lục_01 r195 cDESCRIPTION]
  4. FR-PDEBT-4: Track payment gateway settlements and fees by period. [Source: Phụ_Lục_01 r196 cDESCRIPTION]
- Business Rules:
  1. BR-PDEBT-1: Carrier debt view includes order list with COD and shipping fee totals. [Source: Phụ_Lục_01 r195 cDESCRIPTION]
- Acceptance Criteria:
  - Debt summaries filter by week/month/year/custom; lists exportable with required columns. [Source: Phụ_Lục_01 r195 cDESCRIPTION][Source: Phụ_Lục_01 r196 cDESCRIPTION]

## Module: Portal - Shipping & Shipper Ops
- Goals: Configure carriers/methods, warehouse routing, COD options, and manage in-house shippers. [Source: Task Breakdown r27 cC][Source: Phụ_Lục_01 r197 cITEM]
- Functional Requirements:
  1. FR-PSHIP-1: Activate/deactivate carriers list. [Source: Phụ_Lục_01 r198 cITEM]
  2. FR-PSHIP-2: Define delivery methods (Siêu tốc/Nhanh/Tiết kiệm/food) mapped to carriers. [Source: Phụ_Lục_01 r199 cDESCRIPTION]
  3. FR-PSHIP-3: Configure per-warehouse routing of carriers by destination province. [Source: Phụ_Lục_01 r200 cDESCRIPTION]
  4. FR-PSHIP-4: Configure COD payment options per carrier. [Source: Phụ_Lục_01 r201 cDESCRIPTION]
  5. FR-PSHIP-5: Manage in-house shipper roster, statuses (pickup/delivered), credentials. [Source: Phụ_Lục_01 r203 cDESCRIPTION]
  6. FR-PSHIP-6: List shipper-assigned orders with actions (pickup, delivered) and reassignment. [Source: Phụ_Lục_01 r204 cDESCRIPTION]
  7. FR-PSHIP-7: Rank shipper performance by delivered orders. [Source: Phụ_Lục_01 r205 cDESCRIPTION]
- Acceptance Criteria:
  - Carrier routing respects warehouse-destination rules; COD options limited to configured payment types; shipper UI exposes pickup/delivered toggles. [Source: Phụ_Lục_01 r200 cDESCRIPTION][Source: Phụ_Lục_01 r204 cDESCRIPTION]

## Module: Portal - Staff & Permissions
- Goals: Manage staff records, grouping, and fine-grained permissions. [Source: Task Breakdown r30 cC][Source: Phụ_Lục_01 r206 cITEM]
- Functional Requirements:
  1. FR-PSTAFF-1: CRUD staff groups. [Source: Phụ_Lục_01 r207 cITEM]
  2. FR-PSTAFF-2: Maintain staff list by group. [Source: Phụ_Lục_01 r208 cITEM]
  3. FR-PSTAFF-3: Configure permissions (view/create/delete/edit, promo, import/export) per module. [Source: Phụ_Lục_01 r209 cDESCRIPTION]
  4. FR-PSTAFF-4: Store staff profile details. [Source: Phụ_Lục_01 r210 cDESCRIPTION]
- Acceptance Criteria:
  - Permissions enforce per-module actions; staff appear under assigned groups with correct contact info. [Source: Phụ_Lục_01 r209 cDESCRIPTION][Source: Phụ_Lục_01 r210 cDESCRIPTION]

## Module: Portal - Customer CRM & Communication
- Goals: Provide CRM views, grouping, feedback handling, and outbound comms. [Source: Task Breakdown r31 cC][Source: Phụ_Lục_01 r211 cITEM]
- Functional Requirements:
  1. FR-PCRM-1: Customer list with filters (region, tier, group, spend, phone, points). [Source: Phụ_Lục_01 r212 cDESCRIPTION]
  2. FR-PCRM-2: Customer detail view with addresses, notifications, order history, points, vouchers, reviews, viewed/favorite products, groups. [Source: Phụ_Lục_01 r213 cDESCRIPTION]
  3. FR-PCRM-3: Manage customer groups (manual add/remove) with auto-add rules based on points/orders. [Source: Phụ_Lục_01 r214 cDESCRIPTION]
  4. FR-PCRM-4: Track order feedback/complaints with statuses and resolution notes. [Source: Phụ_Lục_01 r215 cDESCRIPTION]
  5. FR-PCRM-5: Manual email/notification sends to groups or individuals using templates. [Source: Phụ_Lục_01 r216 cDESCRIPTION]
  6. FR-PCRM-6: Automatic notifications for order events and password updates. [Source: Phụ_Lục_01 r217 cDESCRIPTION]
  7. FR-PCRM-7: Moderate customer product ratings/comments. [Source: Phụ_Lục_01 r218 cDESCRIPTION]
- Business Rules:
  1. BR-PCRM-1: Auto group membership based on thresholds (points per month, orders per month). [Source: Phụ_Lục_01 r214 cDESCRIPTION]
- Acceptance Criteria:
  - Filters return correct segments; auto-group rules add customers meeting thresholds; complaints move through status workflow; outbound sends log template and recipients. [Source: Phụ_Lục_01 r212 cDESCRIPTION][Source: Phụ_Lục_01 r214 cDESCRIPTION][Source: Phụ_Lục_01 r215 cDESCRIPTION]

## Module: Content Management (Portal)
- Goals: Author and manage blog/news and static pages. [Source: Phụ_Lục_01 r219 cITEM]
- Functional Requirements:
  1. FR-CONT-1: Manage article categories and tags. [Source: Phụ_Lục_01 r220 cITEM][Source: Phụ_Lục_01 r221 cITEM]
  2. FR-CONT-2: CRUD articles. [Source: Phụ_Lục_01 r222 cITEM][Source: Phụ_Lục_01 r223 cITEM]
  3. FR-CONT-3: Manage static pages list/detail. [Source: Phụ_Lục_01 r224 cITEM][Source: Phụ_Lục_01 r226 cITEM]
- Acceptance Criteria:
  - Articles tagged and categorized; static pages publish/unpublish; content surfaces to web/app CMS module. [Source: Phụ_Lục_01 r222 cITEM][Source: Phụ_Lục_01 r224 cITEM]

## Module: Portal Promotions & Loyalty Config
- Goals: Configure vouchers, shipping vouchers, and point conversion. [Source: Task Breakdown r21 cC][Source: Phụ_Lục_01 r227 cITEM]
- Functional Requirements:
  1. FR-PPROMO-1: Manage discount vouchers with code, amount/%, min/max order value, applicable groups/products, validity window, frequency caps. [Source: Phụ_Lục_01 r228 cDESCRIPTION]
  2. FR-PPROMO-2: Manage free-ship vouchers with amount/% and order value constraints. [Source: Phụ_Lục_01 r229 cDESCRIPTION]
  3. FR-PPROMO-3: Configure point-to-voucher conversion rules. [Source: Phụ_Lục_01 r230 cDESCRIPTION]
- Business Rules:
  1. BR-PPROMO-1: Voucher applicability scoped by category/product/time window and usage frequency per day/week/month plus total cap. [Source: Phụ_Lục_01 r228 cDESCRIPTION]
- Acceptance Criteria:
  - Voucher validations enforce scope and frequency; point conversion generates voucher with configured value. [Source: Phụ_Lục_01 r228 cDESCRIPTION][Source: Phụ_Lục_01 r230 cDESCRIPTION]

## Module: Supplier Portal
- Goals: Provide supplier-facing access to credentials, profile, and purchase orders. [Source: Task Breakdown r32 cC][Source: Phụ_Lục_01 r231 cITEM]
- Functional Requirements:
  1. FR-SP-1: Supplier login via admin-created account; password reset via email/SMS OTP. [Source: Phụ_Lục_01 r232 cDESCRIPTION]
  2. FR-SP-2: View supplier profile with bank info, edit via admin request; add secondary email/phone for notifications. [Source: Phụ_Lục_01 r233 cDESCRIPTION]
  3. FR-SP-3: View purchase orders with statuses, products, quantities, price, delivery deadlines. [Source: Phụ_Lục_01 r234 cDESCRIPTION]
- Acceptance Criteria:
  - Login restricted to issued accounts; profile changes require admin approval; PO list reflects status definitions. [Source: Phụ_Lục_01 r232 cDESCRIPTION][Source: Phụ_Lục_01 r234 cDESCRIPTION]

## Module: Reporting & Analytics
- Goals: Deliver revenue and sales insights across segments and time. [Source: Task Breakdown r33 cC][Source: Phụ_Lục_01 r235 cITEM]
- Functional Requirements:
  1. FR-REP-1: Allow time filters (day/week/month/year/custom) and sorting. [Source: Phụ_Lục_01 r236 cDESCRIPTION]
  2. FR-REP-2: Show total revenue (gross/net), promotion totals, shipping fees, order counts. [Source: Phụ_Lục_01 r237 cDESCRIPTION]
  3. FR-REP-3: Report best-selling products and groups. [Source: Phụ_Lục_01 r238 cDESCRIPTION]
  4. FR-REP-4: Revenue by payment method. [Source: Phụ_Lục_01 r239 cDESCRIPTION]
  5. FR-REP-5: Revenue by province. [Source: Phụ_Lục_01 r240 cDESCRIPTION]
  6. FR-REP-6: Revenue by customer type (member vs walk-in). [Source: Phụ_Lục_01 r241 cDESCRIPTION]
  7. FR-REP-7: Revenue by voucher/program. [Source: Phụ_Lục_01 r242 cDESCRIPTION]
  8. FR-REP-8: Trend analysis by day/week/hour with period comparisons and bar charts. [Source: Phụ_Lục_01 r243 cDESCRIPTION]
- Acceptance Criteria:
  - Reports respect filters and show defined metrics; charts support comparisons between current vs previous periods. [Source: Phụ_Lục_01 r236 cDESCRIPTION][Source: Phụ_Lục_01 r243 cDESCRIPTION]

## Module: Manual Fallback & GTM
- Goals: Provide manual operations when shipping integrations fail and instrument GTM/pixels. [Source: Task Breakdown r36 cD]
- Functional Requirements:
  1. FR-MAN-1: Allow manual handling of shipping steps when carriers are unavailable, keeping audit trail. [Source: Task Breakdown r36 cD]
  2. FR-MAN-2: Install GTM to manage analytics pixels across web/app. [Source: Task Breakdown r36 cD]
- Acceptance Criteria:
  - Manual mode toggles per order with activity log; GTM container ID configurable and events fired on key flows. [Source: Task Breakdown r36 cD]
