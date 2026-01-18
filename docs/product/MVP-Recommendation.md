# MVP Recommendation
- WEB-02 Buyer authentication: Required to gate all shopper actions and matches OTP/password flows in specs. [Source: Phụ_Lục_01 r19 cDESCRIPTION][Source: Phụ_Lục_01 r21 cDESCRIPTION]
- WEB-09 Product catalog & search: Core discovery (categories, list, detail, search suggestions) enabling cart creation. [Source: Phụ_Lục_01 r44 cDESCRIPTION][Source: Phụ_Lục_01 r47 cDESCRIPTION]
- WEB-10 Cart operations: Allows selecting quantities and voucher input before checkout. [Source: Phụ_Lục_01 r50 cDESCRIPTION][Source: Phụ_Lục_01 r53 cDESCRIPTION]
- WEB-11 Checkout & payment: Covers delivery selection, totals, Payoo/COD, invoice capture, and order emails. [Source: Phụ_Lục_01 r56 cDESCRIPTION][Source: Phụ_Lục_01 r63 cDESCRIPTION][Source: Phụ_Lục_01 r64 cDESCRIPTION]
- PORTAL-02 Category & group management: Needed to seed catalog structure consumed by shopper flows. [Source: Phụ_Lục_01 r161 cITEM][Source: Phụ_Lục_01 r162 cDESCRIPTION]
- PORTAL-05 Order management & status mapping: Lets operations view/transition orders and manage carriers. [Source: Phụ_Lục_01 r174 cDESCRIPTION][Source: Phụ_Lục_01 r176 cDESCRIPTION]
- INTEG-01 Payoo payment integration: Required for online payments within checkout. [Source: Phụ_Lục_01 r16 cDESCRIPTION][Source: Phụ_Lục_01 r63 cDESCRIPTION]

Rationale: These seven stories deliver an end-to-end buyer journey (browse -> cart -> checkout), ensure catalog data exists, process payments, and enable ops to manage resulting orders. Remaining stories (promotions, loyalty, CRM, supplier portal, reporting) can layer on after core ordering works.
