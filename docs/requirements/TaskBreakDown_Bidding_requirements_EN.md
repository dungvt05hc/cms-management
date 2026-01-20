# Requirements Summary (English Version)

> Glossary (kept as-is in several places):
> - **DVVC**: Shipping carrier / delivery partner (Don Vi Van Chuyen)
> - **COD**: Cash on Delivery
> - **GTGT**: VAT (Value Added Tax) e-invoice
> - **FCM**: Firebase Cloud Messaging

## 1. Overview
- The workbook defines a Work Breakdown Structure (WBS) and a high-level task breakdown for an e-commerce solution.
- Main components: **Buyer Website**, **Buyer Mobile App (Android/iOS)**, **Internal Management Portal (Admin/Operations)**.
- The Task Breakdown also mentions a **Shipper mobile UI**, a **Supplier page/portal**, notifications/deeplinks, and a manual fallback and Google Tag Manager (GTM).

## 2. Key integrations & external services (explicitly mentioned)
- **Firebase Cloud Messaging (FCM)**: push notifications.
- **Google Analytics** and **Google Tag Manager (GTM)** (GTM mentioned in Task Breakdown).
- **Shipping SDK** ("Fast shipping" / "Van chuyen nhanh") and shipping partners (listed in Website checkout).
- **Online payment gateway**: **Payoo** (domestic ATM cards / international cards such as Visa/MasterCard/JCB / e-wallets / bank apps).
- **E-invoice (GTGT/VAT) API** integration during checkout.
- **Customer support channels**: hotline + chat apps (Viber/Zalo/Messenger).
- **Google Maps**: navigation for shipper UI (Task Breakdown).


## 3. Buyer-facing scope (Website & Mobile App)

### 3.1 Requirements from Appendix_01 (Phu_Luc_01)

### I. ECOMMERCE WEBSITE - BUYER WEBSITE

#### 1. BUILD STRUCTURE AND CONFIGURATION
- **1.1 Build the base core structure**
- **1.2 Configure FCM**
  Firebase notifications
- **1.3 Configure Google Analytics**
- **1.4 Configure the fast shipping SDK**
- **1.5 Configure the online payment SDK**
  Payoo: supports multiple payment methods (domestic ATM cards / international cards (Visa/MasterCard/JCB...) / e-wallets / bank apps)
- **1.6 Layout, master page, static pages**

#### 2. AUTHENTICATION - USER VERIFICATION
- **2.1 Login**
  Use phone number or email / password
- **2.2 Forgot password**
  Prefer password reset by email; alternative: SMS OTP
- **2.3 Register**
  After OTP verification, customer must provide additional information (full name, email, phone number, ...) and set a password for easier future login
- **2.4 OTP confirmation**

#### 3. HOME PAGE
- **3.1 Product categories**
  UI equivalent to www.bachhoaxanh.com
- **3.2 Banner (static/slider) with links**
- **3.3 Menu**
- **3.4 Featured / promotional products**
- **3.5 Footer**

#### 4. CUSTOMER ACCOUNT
- **4.1 Account information**
  - Title (Mr./Ms.) / Full name / Phone number / Email / Nationality
  Account update: allow changing information except Phone number
  Change password: prefer using email to change password (alternative: SMS OTP)
- **4.2 My notifications**
  All notifications / Promotions / Orders / System notifications
- **4.4 My orders**
  List of orders grouped by order status:
  - Processing / Delivering / Delivered / Cancelled / Pending review / Return
  - **Order details**
    - Order code / Tracking code / Order time / Delivery time / Payment method / Shipping address / Status / Voucher / Carrier name (DVVC) / Shipper name / Shipper phone / Delivery method
    - Product list: Product name / Variant / Quantity / Unit price / Line total
    - (Subtotal - Discount + Shipping fee) * VAT => Total payable
  - **Cancel order**
    Customer can only cancel orders with status: "Processing"
  - **Confirm received (successful delivery)**
    If customer does not confirm, the order will be auto-confirmed after [n] hours
  - **Reorder an old order (delivered / cancelled)**
    Customer can reorder any order, adjust quantities, add/remove products, ...
- **4.5 Address book**
  - Add / update / delete shipping addresses
  - Maximum: 5 shipping addresses
  - Each address includes: Full name / Phone number / Address
  - Set default address (frequently used)
- **4.6 Vouchers**
  - Customer receives vouchers when purchasing;
  - or Customer Support sends them;
  - or voucher redemption via loyalty points (per rules)
- **4.7 Loyalty points**
  - Each successful order converts to points: 1,000 VND = 1 point
  - Points can be redeemed for vouchers / gifts when promotions are available
- **4.8 Reviews for purchased products**
  Review list grouped by: Pending review / Reviewed
  - Review content includes: Rating (5 stars) / Comment / Images
- **4.9 Viewed products list**
  View history sorted newest-first, up to [n] products
- **4.10 Favorite products list**
  Products marked as favorite by the customer, sorted newest-first

#### 5. PRODUCTS
- **5.1 Product categories**
  - Unlimited category hierarchy
  - Entering a category displays the corresponding product list
  - Entering a category also displays related brands
- **5.2 Product groups (product type, attributes, ...)**
  - Used to group products with shared characteristics (type)
  - A product group must be linked to a specific product category
  - Each category can have multiple product groups
- **5.3 Product list**
  - Displayed as a grid/table: [n] columns x [n] rows
  - Each item shows: Product image / Product name / Price / Buy button (add to cart) / Promotion
  - Infinite scroll: auto-load more when user reaches the end
  - Sort by price (asc/desc)
- **5.4 Product details**
  - Product name / Images / Videos / Description and details
  - Promotions
  - Variants/specifications (color / volume / weight / packaging, ...) with corresponding price
  - Shipping cost and delivery time to [address]
  - Quantity (+/-) to add to cart
  - Product comments (customer can see their own; others cannot see until Admin approval)
  - Product rating (1-5 stars) for customers who purchased
  - Suggested products (same category / similar / ...)
- **5.5 Product search**
  As the customer types a phrase, suggested results appear

#### 6. CART
- **6.1 Product list**
  - Table columns: Select checkbox / Product / Quantity (+/-) / Unit price / Line total / Delete button
- **6.2 Edit / remove products**
  - Increase/decrease quantity
  - Choose product variant/specification (weight / volume / packaging, ...)
- **6.3 For selected products**
  - Display subtotal
- **6.4 Promotions**
  - Enter or choose a voucher code
  - 2 voucher types: Discount code | Shipping code
- **6.5 CHECKOUT button**
  Place order for selected products and navigate to ORDER CONFIRMATION

#### 7. ORDER CONFIRMATION
- **7.1 Shipping address**
  Choose an existing address / or add a new one (max 5 addresses)
- **7.2 Choose delivery method**
  Customer selects delivery method (e.g., Super Fast | Fast | Economy)
  - Then selects the corresponding carrier (DVVC): ViettelPost, VNPost, Ahamove, Express
  (These configurations are pre-set by Admin)
  System returns:
  - Estimated delivery time
  - SHIPPING FEE
- **7.3 Products being purchased**
  - Product list: Product name / Variant / Quantity / Unit price / Line total
  - PRODUCT SUBTOTAL
- **7.4 Promotions**
  Customer searches/selects general vouchers or gifted vouchers and applies to the order
  - 2 voucher types: (1) Discount (2) Shipping fee discount
  - Maximum: 1 voucher per type
- **7.5 Total amount payable**
  Product subtotal
  - Discount
  + Shipping fee
  - Shipping promotion (<= shipping fee)
  = Order total
- **7.6 E-invoice (GTGT/VAT)**
  - Customer enters invoice information
  - After 10 days from successful delivery: invoice content is pushed to the E-invoice Portal API, issued automatically, and emailed to the customer
  - Save invoice info for next time
- **7.7 Notes**
  Customer leaves notes/instructions for the order
- **7.8 Payment method selection**
  1) Pay on delivery: [ ] Cash  [ ] Card swipe (configured per carrier)
  2) Online payment gateway: Domestic ATM card | International card (Visa/Master/JCB) | E-wallet | Bank app
  => Only after successful payment is the order created and set to "Processing"; if payment fails, selected products return to Cart and no order is created
- **7.9 New order email notification**
  Send a full order confirmation email to the customer and warehouse staff

#### 8. CMS PAGES - ARTICLES
- **8.1 Articles home page**
  (as per the provided mockup images)
- **8.2 Article categories**
- **8.3 Article detail page**
- **8.4 Article list**
- **8.5 Blocks (login / promotions / ...)**

#### 9. HELP
- **9.1 Help center**
  How-to articles and system lookup guides
- **9.2 Terms of service**
- **9.3 Sales policy**
- **9.4 Consulting & customer feedback/complaints**
  - Live chat and hotline 19005454
  - Buttons for other chat apps: Viber, Zalo, Messenger, ...
- **9.5 Partner contact**

#### 10. PROMOTIONS
- **10.1 Promotion programs**
  Promotion types (examples): Clearance at cost / Buy 2 get 1 free / Bundle deals, ...
- **10.2 Promotional product list**
  Clicking a promotion program shows the list of products in that program for the customer to select and purchase

#### 11. NOTIFICATIONS
- **11.1 Notification feature for website / mobile app**
  Admin configures notifications per module
- **11.2 Email notifications to customers**

### II. MOBILE APP - BUYER MOBILE APP

#### 1. BUILD STRUCTURE AND CONFIGURATION
- **1.1 Build the base core structure**
- **1.2 Configure FCM**
  Firebase notifications
- **1.3 Configure Google Analytics**
- **1.4 Configure the fast shipping SDK**
- **1.5 Configure the online payment SDK**
  Payoo: supports multiple payment methods (domestic ATM cards / international cards (Visa/MasterCard/JCB...) / e-wallets / bank apps)

#### 2. AUTHENTICATION - USER VERIFICATION
- **2.1 Login**
  Use phone number or email / password
- **2.2 Forgot password**
  Prefer password reset by email; alternative: SMS OTP
- **2.3 Register**
  After OTP verification, customer must provide additional information (full name, email, phone number, ...) and set a password for easier future login
- **2.4 OTP confirmation**

#### 3. HOME PAGE
- **3.1 Product categories**
  UI equivalent to www.bachhoaxanh.com
- **3.2 Banner (static/slider) with links**
- **3.3 Menu**
- **3.4 Featured / promotional products**
- **3.5 Footer**

#### 4. CUSTOMER ACCOUNT
- **4.1 Account information**
  - Title (Mr./Ms.) / Full name / Phone number / Email / Nationality
  Account update: allow changing information except Phone number
  Change password: prefer using email to change password (alternative: SMS OTP)
- **4.2 My notifications**
  All notifications / Promotions / Orders / System notifications
- **4.4 My orders**
  List of orders grouped by order status:
  - Processing / Delivering / Delivered / Cancelled / Pending review / Return
  - **Order details**
    - Order code / Tracking code / Order time / Delivery time / Payment method / Shipping address / Status / Voucher / Carrier name (DVVC) / Shipper name / Shipper phone / Delivery method
    - Product list: Product name / Variant / Quantity / Unit price / Line total
    - (Subtotal - Discount + Shipping fee) * VAT => Total payable
  - **Cancel order**
    Customer can only cancel orders with status: "Processing"
  - **Confirm received (successful delivery)**
    If customer does not confirm, the order will be auto-confirmed after [n] hours
  - **Reorder an old order (delivered / cancelled)**
    Customer can reorder any order, adjust quantities, add/remove products, ...
- **4.5 Address book**
  - Add / update / delete shipping addresses
  - Maximum: 5 shipping addresses
  - Each address includes: Full name / Phone number / Address
  - Set default address (frequently used)
- **4.6 Vouchers**
  - Customer receives vouchers when purchasing;
  - or Customer Support sends them;
  - or voucher redemption via loyalty points (per rules)
- **4.7 Loyalty points**
  - Each successful order converts to points: 1,000 VND = 1 point
  - Points can be redeemed for vouchers / gifts when promotions are available
- **4.8 Reviews for purchased products**
  Review list grouped by: Pending review / Reviewed
  - Review content includes: Rating (5 stars) / Comment / Images
- **4.9 Viewed products list**
  View history sorted newest-first, up to [n] products
- **4.10 Favorite products list**
  Products marked as favorite by the customer, sorted newest-first

#### 5. PRODUCTS
- **5.1 Product categories**
  - Unlimited category hierarchy
  - Entering a category displays the corresponding product list
  - Entering a category also displays related brands
- **5.2 Product groups (product type, attributes, ...)**
  - Used to group products with shared characteristics (type)
  - A product group must be linked to a specific product category
  - Each category can have multiple product groups
- **5.3 Product list**
  - Displayed as a grid/table: [n] columns x [n] rows
  - Each item shows: Product image / Product name / Price / Buy button (add to cart) / Promotion
  - Infinite scroll: auto-load more when user reaches the end
  - Sort by price (asc/desc)
- **5.4 Product details**
  - Product name / Images / Videos / Description and details
  - Promotions
  - Variants/specifications (color / volume / weight / packaging, ...) with corresponding price
  - Shipping cost and delivery time to [address]
  - Quantity (+/-) to add to cart
  - Product comments (customer can see their own; others cannot see until Admin approval)
  - Product rating (1-5 stars) for customers who purchased
  - Suggested products (same category / similar / ...)
- **5.5 Product search**
  As the customer types a phrase, suggested results appear

#### 6. CART
- **6.1 Product list**
  - Table columns: Select checkbox / Product / Quantity (+/-) / Unit price / Line total / Delete button
- **6.2 Edit / remove products**
  - Increase/decrease quantity
  - Choose product variant/specification (weight / volume / packaging, ...)
- **6.3 For selected products**
  - Display subtotal
- **6.4 Promotions**
  - Enter or choose a voucher code
  - 2 voucher types: Discount code | Shipping code
- **6.5 CHECKOUT button**
  Place order for selected products and navigate to ORDER CONFIRMATION

#### 7. ORDER CONFIRMATION
- **7.1 Shipping address**
  Choose an existing address / or add a new one (max 5 addresses)
- **7.2 Choose delivery method**
  Customer selects delivery method (e.g., Super Fast | Fast | Economy)
  - Then selects the corresponding carrier (DVVC): ???
  (These configurations are pre-set by Admin)
  System returns:
  - Estimated delivery time
  - SHIPPING FEE
- **7.3 Products being purchased**
  - Product list: Product name / Variant / Quantity / Unit price / Line total
  - PRODUCT SUBTOTAL
- **7.4 Promotions**
  Customer searches/selects general vouchers or gifted vouchers and applies to the order
  - 2 voucher types: (1) Discount (2) Shipping fee discount
  - Maximum: 1 voucher per type
- **7.5 Total amount payable**
  Product subtotal
  - Discount
  + Shipping fee
  - Shipping promotion (<= shipping fee)
  = Order total
- **7.6 E-invoice (GTGT/VAT)**
  - Customer enters invoice information
  - After 10 days from successful delivery: invoice content is pushed to the E-invoice Portal API, issued automatically, and emailed to the customer
  - Save invoice info for next time
- **7.7 Notes**
  Customer leaves notes/instructions for the order
- **7.8 Payment method selection**
  1) Pay on delivery: [ ] Cash  [ ] Card swipe (configured per carrier)
  2) Online payment gateway: Domestic ATM card | International card (Visa/Master/JCB) | E-wallet | Bank app
  => Only after successful payment is the order created and set to "Processing"; if payment fails, selected products return to Cart and no order is created
- **7.9 New order email notification**
  Send a full order confirmation email to the customer and warehouse staff

#### 8. CMS PAGES - ARTICLES
- **8.1 Articles home page**
  (as per the provided mockup images)
- **8.2 Article categories**
- **8.3 Article detail page**
- **8.4 Article list**
- **8.5 Blocks (login / promotions / ...)**

#### 9. HELP
- **9.1 Help center**
  How-to articles and system lookup guides
- **9.2 Terms of service**
- **9.3 Sales policy**
- **9.4 Consulting & customer feedback/complaints**
  - Live chat and hotline 19005454
  - Buttons for other chat apps: Viber, Zalo, Messenger, ...
- **9.5 Partner contact**

#### 10. PROMOTIONS
- **10.1 Promotion programs**
  Promotion types (examples): Clearance at cost / Buy 2 get 1 free / Bundle deals, ...
- **10.2 Promotional product list**
  Clicking a promotion program shows the list of products in that program for the customer to select and purchase

#### 11. NOTIFICATIONS
- **11.1 Notification feature for website / mobile app**
  Admin configures notifications per module
- **11.2 Email notifications to customers**

### 3.2 Noted deltas between Website vs Mobile App (only where Appendix content differs)
- Website only: **1.6 Layout, master page, static pages**
- Different content at **7.2 Choose delivery method**
  - Website: customer chooses delivery method (Super Fast | Fast | Economy) and then chooses a carrier (DVVC): ViettelPost, VNPost, Ahamove, Express
  - Mobile: customer chooses delivery method (Super Fast | Fast | Economy) and then chooses a carrier (DVVC): ???

## 4. Internal Management Portal scope (Appendix_01 / Phu_Luc_01)

### III. PORTAL - INTERNAL MANAGEMENT WEBSITE

#### 1. BUILD STRUCTURE AND CONFIGURATION
- **1.1 Build the base core structure**
- **1.2 Configure FCM**
  Firebase notifications

#### 2. AUTHENTICATION - MANAGEMENT SYSTEM ACCESS
- **2.1 Authenticate access to the management system**
  1. Registration: the Super Admin (highest privilege) creates staff accounts and assigns permissions
  2. Login: by email/phone number / password
  3. Forgot password: use email to reset password

#### 3. PRODUCT GROUP MANAGEMENT
- **3.1 Manage product categories (hierarchical) and product groups (attributes, type, packaging, ...)**
  Product groups are linked to a specific product category; one category can have multiple product groups.
  Functionalities:
  1. List all product groups
  2. Add a new product group
  3. Update a product group
  4. Delete a product group
  Example:
  - Category: RICE, FLOUR > ALL RICE TYPES
  - Product groups under category "ALL RICE TYPES":
    - Group 1: Attributes (fluffy rice, fragrant sticky rice, ...)
    - Group 2: Rice type (regular rice, brown rice, glutinous rice, ...)

#### 4. PRODUCT MANAGEMENT
- **4.1 Product list**
  Products are grouped by product category
- **4.2 Product details**
- **4.3 Basic product information**
  1. Update product details (including SKU, name, price, images, videos, information, ...)
  2. Group products by attributes / packaging (e.g., color, volume, weight, ...) with corresponding buy/sell prices
  3. Shelf life / expiry date
  4. Sales end date (last sale date)
  5. Rating: 5-star scale
- **4.4 Alert/warning system**
  * When the product reaches its sales end date:
    a. The system sends a notification to the warehouse owner/responsible staff about the product status
    b. Warehouse staff (manual) checks the product and decides whether to apply promotion or move it into expired inventory management
  * When stock quantity reaches the MIN threshold:
    a. The system alerts staff to create a purchase order to the supplier
    b. The replenishment quantity must not exceed the MAX limit
- **4.5 Promotions / discounts**
  1. Discount based on approaching sales end date
  2. Discount based on product set/bundles (manually decided by user)
  3. Promotion programs:
     - Buy n of product N, get m of product M
     - Buy n of product N, get m of the same product N
     (n: quantity; N/M: products)
     - Start time / End time
     - Apply time rules:
       + Day of month (e.g., the 5th of every month)
       + Day of week (e.g., every Friday)
       + Time of day (e.g., 11:00-13:00; 20:00-22:00)
- **4.6 Batch/lot list**
  Each product can have multiple inbound batches/lots:
  - Batch details: Batch SKU / Supplier / Purchase price / Received quantity / On-hand quantity / Warehouse location
- **4.7 Shipping configuration**
  - Shipping distance limit (e.g., max 5km)
  - Allowed delivery methods (e.g., Super Fast / Super Fast Food)
- **4.8 Moderation of product reviews**
  Staff reviews and approves if appropriate
- **4.9 Moderation of product comments**
  Staff reviews and approves if appropriate

#### 5. ORDER MANAGEMENT
- **5.1 Order list**
  Table (columns x rows) with all fields from ORDER DETAILS
  - View by time (week / month / year / custom range), pagination, export to Excel
  - Filters: Product category / Product / Order status / Warehouse / Province/City / Voucher / Customer / Carrier (DVVC) / Payment method
  - Summary metrics:
    + Total number of orders
    + Total amount paid by customers
    + Total shipping fees (collected by carrier)
    + Total COD amount (collected on behalf by carrier)
    + Total payment gateway service fees
    + Total amount customers paid via the payment gateway
- **5.2 Order details**
  - Order code / Tracking code / Order time / Delivery time / Payment method / Shipping address / Status / Voucher / Carrier name (DVVC) / Shipper name / Shipper phone
  - Product list: Product name / Variant / Quantity / Unit price / Line total
  - (Subtotal + Shipping fee - Discount) * VAT = Total payable
  - Actual shipping fee payable to the carrier
- **5.3 Order statuses**
  Customer-facing statuses:
  Processing / Delivering / Delivered / Cancelled / Return / Pending review
  Corresponding internal statuses (for staff operations):
  1) Processing = Preparing items
  2) Delivering = Packed -> Carrier picked up
  3) Delivered = Delivered -> Customer confirmed received (tap confirm) OR system auto-confirms after [n] hours
  4) Cancelled = Customer requests cancellation (with reason) -> Staff confirms cancellation
  5) Return = Customer requests return -> Staff approves -> Carrier collects returned goods -> Carrier completed return
  6) Pending review = appears after customer confirmed received
  * Cancellation reasons: pre-configured by Admin, e.g.:
    - Wrong quantity/variant
    - Delivery took too long
    - No longer needed
    - Other (with notes)
- **5.4 Links to track shipment status for carriers (DVVC)**
  Track by tracking code via links provided by the carrier API
- **5.5 Order dispatching function**
  For orders not yet assigned to a shipper, a manager can switch to another carrier (DVVC) without changing the customer-facing shipping fee
- **5.6 E-invoice (GTGT/VAT)**
  - Monthly e-invoice list
  - View invoice details
  - Export invoice list to Excel
- **5.7 Order reports / analytics**
  1) Order statistics by status: Cancelled (with reasons) / Successful / Preparing / Packed / Delivering
  2) Report customers who cancel 3 times or more

#### 6. WAREHOUSE MANAGEMENT
- **6.1 Warehouse list**
  Warehouse information includes:
  - Warehouse name / Address / Responsible person / Phone number
  - Warehouse staff accounts
- **6.2 Warehouse operations include**
  1. Stock out
  2. Stock in (from supplier) via the SUPPLIER module
  3. Cancelled items
     a. Expired items
     b. Items returned to supplier
  4. Sealed/quarantined items
     a. Items pending approval
     b. Staff checks items and then, depending on condition, updates status to cancel or accept
  5. When stock is low (MIN threshold), the system notifies the responsible person to CREATE A PURCHASE ORDER and send it to the supplier
  6. Staff cannot create a purchase order exceeding the MAX quantity
- **6.3 Warehouse location management**
  Each location is coded hierarchically: A.1.2.3.4, where:
  - Warehouse A, room 1, rack 2, level 3, slot 4
  * This code increments automatically with no fixed limit

#### 7. SUPPLIER MANAGEMENT
- **7.1 Supplier list**
  Admin manages suppliers (add / edit / delete)
- **7.2 Supplier grouping**
  Suppliers can be grouped with custom groupings
- **7.3 Supplier details**
  Required supplier information includes:
  - Supplier name / Contact person / Phone / Email / Address
  - Bank account info: Account number / Account name / Bank name
- **7.4 Purchase order history**
  List of supplier supply batches for ???, grouped by Purchase Order status:
  - Draft PO / PO sent to supplier / PO confirmed / In production / Shipped / Received / Accepted / Returned / Paid / Cancelled / Late order
  - Product / Variant / Quantity / Purchase price
  - Supply lead time / due date
- **7.5 Automatic email / app notification**
  - Send notifications related to supply orders to the supplier and tracking staff
  - Send debt reconciliation notifications to the supplier and tracking staff
- **7.6 Manual email / app notification**
  Select supplier recipients:
  - Select supplier groups
  - Select individual suppliers
  Select a template to send:
  1. Debt reconciliation notice
  2. Thank-you note (Lunar New Year, Christmas, ...)
  3. SUPPLIER SURVEY FORM (collect supplier feedback and store results in the system)
  (Email template list is pre-configured by Admin; staff selects and sends)

#### 8. RECEIVABLES / DEBT MANAGEMENT
- **8.1 For customers**
  Admin/staff can check customer receivables in the ORDER MANAGEMENT module
- **8.2 For suppliers**
  Admin/staff can check supplier debt in the SUPPLIER MANAGEMENT module
- **8.3 For shipping carriers (DVVC)**
  View receivables per carrier (by week / month / year / custom range):
  - Full order list: Order code / Tracking code / Pickup date / Delivery date / Total amount / COD / Shipping fee collected by carrier
  - Total COD the carrier owes
  - Total shipping fee the carrier will collect
- **8.4 For the online payment gateway**
  View receivables per payment gateway (by week / month / year / custom range):
  - Full order list: Order code / Payment date / Customer / Payment method / Paid amount / Payment fee
  - Total amount paid by customers
  - Total fees collected by the gateway

#### 9. SHIPPING MANAGEMENT
- **9.1 Carrier list**
  Allow enabling/disabling carriers
- **9.2 Declare/define delivery methods**
  Example delivery methods:
  1) Super Fast (Ahamove Super Fast, Express Super Fast)
  2) Fast (Ahamove Low-cost Fast, Express Fast, ViettelPost Fast)
  3) Economy (VNPost Standard, ViettelPost Economy)
  4) Super Fast Food (Ahamove Super Fast Food, Express Super Fast)
- **9.3 Configure shipping warehouses**
  - Configure carriers by Warehouse -> Destination province/city
  Example:
  - HCM warehouse to [HCM] = Ahamove, Express, VNPost, ViettelPost
  - HCM warehouse to [NHA TRANG, PHU YEN] = VNPost
  - HCM warehouse to [other provinces] = ViettelPost, VNPost
- **9.4 Configure COD collection methods**
  Configure allowed COD methods per carrier, e.g.:
  Express => [Cash, Card swipe]
  ViettelPost => [Cash]
  ...

#### 10. SHIPPER TEAM MANAGEMENT (EXPRESS)
- **10.1 Shipper list**
  - Admin manages shippers (add / delete / edit / issue passwords, ...)
  - Only Admin can change shipper statuses: Picked up / Delivered
  - Shipper details: full name / gender / phone / email
  * After login, shipper can only operate Picked up / Delivered within this module
- **10.2 Assigned orders list**
  Assigned order list and details:
  - Order code / Tracking code / Order time / Delivery time / Payment method / Shipping address / Status / Shipper / COD / Warehouse
  - Product details: Name / variant / quantity
  - Total amount to collect: if the customer paid online, then this value is 0 VND
  - Picked up button / Delivered button: for shipper operations
  - Change shipper button: for Admin operations
- **10.3 Shipper team classification/ranking**
  Shipper rating statistics based on number of orders delivered

#### 11. EMPLOYEE MANAGEMENT
- **11.1 Employee groups**
  Add / delete / edit
- **11.2 Employee list**
  List employees by group
- **11.3 Employee permissions**
  All operations (VIEW / ADD / DELETE / EDIT ...) on each module must be permissioned, for example:
  - Product management module permissions: [ ] Add [ ] Delete [ ] Edit [ ] Promotions, ...
  - Customer management module permissions: [ ] Add [ ] Delete [ ] Edit [ ] Group customers, ...
  - Warehouse module permissions: [ ] Stock in [ ] Stock out
- **11.4 Employee details**
  Full name / Gender / Employee group / Phone / Email / Contact address

#### 12. CUSTOMER MANAGEMENT
- **12.1 Customer list**
  Table (columns x rows) with columns:
  - Full name / Gender / Province/City / Area / Phone / Email / Address / Number of purchased orders / Total amount purchased
  Filters/search/sort by: Area / Province/City / Membership tier / Group / Customer name / Phone / Points
- **12.2 Customer details**
  - Profile: Title (Mr./Ms.) / Full name / Phone / Email / Nationality / Area / Province/City
  - Address book
  - Notifications received
  - Order history
  - Loyalty points
  - Voucher list
  - Product review list
  - Viewed products list
  - Favorite products list
  - Customer groups (one customer can belong to multiple groups)
- **12.3 Customer groups**
  - Add / delete / edit customer groups
  - Group details: Group name / Group description, e.g.:
    - VIP customers
    - Frequent customers
    - Potential customers
    - Family customers
    - Close friends
  * Manual operations: add customer to group / remove customer from group
  * Auto-add conditions, e.g.:
    - Customer reaches [n] points/month => auto-add to [GROUP]
    - Customer reaches [n] orders/month => auto-add to [GROUP]
- **12.4 Order feedback and complaints**
  Feedback list grouped by processing status: Pending | In progress | Completed
  For successful customers (with order number), the system allows the customer to send feedback via purchased orders.
  1. After submission, the system automatically sends an email to the customer
  2. The responsible staff contacts the customer to resolve (manual, outside the system); once done, staff records the resolution result on the order and marks it Completed
- **12.5 Manual email and app notifications**
  Select customers:
  - Select customer groups
  - Select individual customers
  Select a template to send:
  1. Promotion/voucher announcement
  2. Thank-you note (Lunar New Year, Christmas, ...)
  3. CUSTOMER SURVEY FORM (collect customer feedback and store results in the system)
  (Email template list is pre-configured by Admin; staff selects and sends)
- **12.6 Automatic email / app notifications**
  ORDERS:
  - Notify new order
  - Notify order delivering
  - Notify order delivered
  - Send thank-you message including the customer's current points
  CUSTOMER INFORMATION:
  - Notify customer updated profile
  - Notify customer requested password change
  - Notify customer changed password successfully
- **12.7 Manage customer product reviews and comments**
  Reviews grouped by: Pending approval / Approved
  Review details: comment content / rating (5 stars) / customer / order / product / approver / communication history between staff and customer

#### 13. ARTICLE CONTENT MANAGEMENT
- **13.1 Hierarchical categories**
- **13.2 Tags / keywords**
- **13.3 Article list**
- **13.4 Article details**

#### 14. STATIC PAGE CONTENT MANAGEMENT
- **14.1 Page list**
- **14.2 Page details**

#### 15. PROMOTION MANAGEMENT
- **15.1 DISCOUNT VOUCHERS**
  - Voucher list
  - Add / edit / delete
  - Voucher details:
    - Voucher code / Discount amount or % / Min and max order value / Applicable categories / Applicable products / Start time / End time
    - Apply time rules: Day of month / Day of week / Time of day
    - Allowed usage frequency: [n] times/day, [n] times/week, [n] times/month
    - Total maximum uses during voucher validity period
- **15.2 FREE SHIPPING VOUCHERS**
  - Discount amount, %
  - Max/min order value
  - Start time, end time
- **15.3 Points redemption**
  Points redemption program:
  - Redeem [n] points into a discount voucher worth [m]

#### 16. SUPPLIER PAGE / SUPPLIER PORTAL
- **16.1 Supplier authentication**
  - Registration: account is created by Admin
  - Forgot password: reset via Email / SMS OTP
  - Login: Email / Phone / Password
- **16.2 Supplier information**
  - Supplier name / Contact person / Phone / Email / Address
  - Bank account info: Account number / Account name / Bank name
  * Supplier changes require contacting Admin
  * Supplier can add 1 email address and 1 phone number to receive notifications
- **16.3 My orders**
  List of supply batches for ???, grouped by Purchase Order status:
  - Pending confirmation / Order received / In production / Shipped / Received / Accepted / Returned / Paid / Cancelled / Late order
  - Product / Variant / Quantity / Purchase price
  - Supply due date

#### 17. REVENUE REPORTING
- **17.1 Report viewing format and approach**
  - Select time range: Day / Week / Month / Year / or custom from date ... to date ...
  - Metrics displayed: Gross revenue, Net revenue, Order count, %, Chart
  - Sort descending / ascending
  Net revenue = Gross revenue - Promotions - Shipping fee payable to carriers
- **17.2 Total revenue**
  Gross revenue / Net revenue / Total promotions / Total number of orders / Total shipping fee
- **17.3 Best-selling revenue**
  - Top-selling products
  - Top-selling product groups
- **17.4 Revenue by payment type**
  Pay on delivery (Cash, Card swipe) / Domestic ATM card/Visa/Master/E-wallet / Bank app
- **17.5 Revenue by province/city (63 provinces/cities)**
  Based on Province/City of shipping address
- **17.6 Revenue by customer**
  - Member customers
  - Guest customers (first-time purchase, or purchased without registration)
  - Revenue per customer
- **17.7 Revenue by promotions**
  - Revenue statistics by issued vouchers
  - Revenue statistics by promotion programs (configured in PRODUCT MANAGEMENT)
- **17.8 Sales trends**
  - Revenue by day-of-month
  - Revenue by day-of-week
  - Revenue by time-of-day
  * Compare this month | last month | same month last year
  * Compare this week | last week | same week last year
  Use bar charts

## 5. Task Breakdown sheet - Epic summary (estimation fields are mostly TBD in source)

| Epic / User role | No. | Related function group | Web | App | Difficulty | References (Appendix_01) | Notable notes |
|---|---:|---|:---:|:---:|:---:|---|---|
| I. CUSTOMER | 1 | Add / delete / edit / update customer information | ✔ | ✔ | I | Section 4.5 |  |
| I. CUSTOMER | 2 | Customer orders | ✔ | ✔ | I | Section 4.4, Section 4.6 |  |
| I. CUSTOMER | 3 | Cart | ✔ | ✔ | I | Section 6 |  |
| I. CUSTOMER | 4 | Order confirmation | ✔ | ✔ | II | Section 7 |  |
| I. CUSTOMER | 4 | Loyalty points | ✔ | ✔ | II | Section 4.7 |  |
| I. CUSTOMER | 5 | Product reviews | ✔ | ✔ | I | Sections 4.8-4.10 |  |
| II. PRODUCTS | 1 | Products / details / search | ✔ | ✔ | II | Sections 3, 5.1-5.5 |  |
| II. PRODUCTS | 2 | Discounted/near-expiry products | ✔ | ✔ | II |  |  |
| III. PORTAL - INTERNAL MANAGEMENT ROLES | 1 | Product group management | ✔ | - | I | Section 3 |  |
| III. PORTAL - INTERNAL MANAGEMENT ROLES | 2 | Product management | ✔ | - | I | Section 4 |  |
| III. PORTAL - INTERNAL MANAGEMENT ROLES | 3 | Promotions/discounts / identify near-expiry products for sale / choose sales campaigns | ✔ | - | III | Section 15 |  |
| III. PORTAL - INTERNAL MANAGEMENT ROLES | 4 | Alert/warning system | ✔ | - | III | Section 4.4 |  |
| III. PORTAL - INTERNAL MANAGEMENT ROLES | 5 | Order management for ??? | ✔ | - | III | Section 5 |  |
| III. PORTAL - INTERNAL MANAGEMENT ROLES | 6 | Warehouse management | ✔ | - | II | Section 6 |  |
| III. PORTAL - INTERNAL MANAGEMENT ROLES | 7 | Supplier management | ✔ | - | III | Sections 4, 7 |  |
| III. PORTAL - INTERNAL MANAGEMENT ROLES | 8 | Receivables/debt management | ✔ | - | II | Section 8 |  |
| III. PORTAL - INTERNAL MANAGEMENT ROLES | 9 | Shipping management | ✔ | - | II | Section 9 |  |
| III. PORTAL - INTERNAL MANAGEMENT ROLES | 10 | Shipper team management | ✔ | - | II | Section 10 |  |
| III. PORTAL - INTERNAL MANAGEMENT ROLES | 11 | Shipper UI on mobile app | - | ✔ | I |  |  |
| III. PORTAL - INTERNAL MANAGEMENT ROLES | 12 | Employee management / permissions | ✔ | TBD | I | Section 11 |  |
| III. PORTAL - INTERNAL MANAGEMENT ROLES | 13 | Customer management | ✔ | - | III | Section 12 |  |
| III. PORTAL - INTERNAL MANAGEMENT ROLES | 14 | Supplier portal page | ✔ | ✔ | TBD | Section 16 | If ??? does not build the app part for this, subtract ??? VND; cannot reduce delivery days because it is done in parallel |
| III. PORTAL - INTERNAL MANAGEMENT ROLES | 15 | Revenue reports | ✔ | - | I | Section 17 | Currently there are 8 report types in Appendix_01 and total reports are around 15 |
| IV. CMS Pages & Help | 1 | CMS Pages & Help (rows 65-76) in Appendix_01 | ✔ | ✔ | I | Sections 8, 9 |  |
| V. NOTIFICATIONS & DEEPLINKS | 1 | All notification types for all user roles; each role has different notifications | ✔ | ✔ | III |  |  |
| VI. MANUAL FALLBACK & GOOGLE TAG MANAGER (GTM) | 1 | Manual system alternative & GTM | ✔ | - | II |  |  |

## 6. Open questions / ambiguities flagged in the source
- (I. CUSTOMER / No. 4) Loyalty points: (1) Appendix_01 Section 4.7; (2) customer membership tiers (gold/silver/bronze, ...) and downgrade rules after inactivity - the logic here is complex
- (III. PORTAL / No. 3) Promotions/discounts: (1) This is cross-function/cross-role. All promo-related features originate from ???, so this epic is only priced here; (2) Appendix_01 Section 15
- (III. PORTAL / No. 8) Receivables/debt management: Appendix_01 Section 8 - unclear whether customers can buy on credit (Tiki/bachhoaxanh typically do not allow credit purchases) => wholesale customers?
- (Appendix_01 - Mobile App - Section 7.2) contains `???`: Choose delivery method -> carrier list is TBD
- (Appendix_01 - Portal - Section 7.4) contains `???`: Purchase order history
- (Appendix_01 - Supplier page - Section 16.3) contains `???`: My orders
