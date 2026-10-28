# 🛍️ AuraMart — Modern Lifestyle & Tech E-Commerce Web Store
> **5th Semester University Project — Web Technologies with .NET**  
> Built with **ASP.NET Core 8.0 MVC**, **Entity Framework Core (SQLite)**, **ASP.NET Core Identity**, and the **UI/UX Pro Max Design System**.

---

## 📌 Executive Summary & Why This Project Stands Out

In a review of peer student e-commerce submissions (e.g., basic furniture and food ordering catalogs), typical projects only feature raw product lists with simple forms and no real shopping pipeline.

**AuraMart** provides an enterprise-standard, end-to-end retail shopping experience:
- **Product Catalog & Multi-Facet Filters**: Browse items by category, price range, brand, customer star ratings, and sorting (price low-high, high-low, newest).
- **Interactive Cart & Promo Engine**: Real-time quantity toggles, subtotal calculation, free shipping thresholds, and promotional coupons (e.g., `SAVE20`, `STUDENT50`).
- **Simulated Payment Gateway**: Interactive checkout modal with instant **UPI QR / VPA simulator**, **Credit/Debit Card** with 1-click demo card filler, and **Cash on Delivery**.
- **Order Lifecycle & Tracking**: Visual progress stepper (`Placed` $\to$ `Processing` $\to$ `Shipped` $\to$ `Delivered`) and printable **Computer-Generated Tax Invoice** (`window.print()`).
- **Role-Based Security (RBAC)**: Distinct portals for **Store Admin** and **Registered Customers**.
- **Merchant Executive Telemetry**: Real-time sales charts with Chart.js, category inventory distribution doughnut, low-stock warnings, and 1-click CSV order export.

---

## 🔐 Pre-Seeded Evaluation Accounts

The project includes an automatic database seed (`DbInitializer.cs`) that provisions test accounts, categories, products, coupons, and orders on startup:

| Role | Email | Password | Primary Capabilities |
| :--- | :--- | :--- | :--- |
| **Store Admin** | `admin@auramart.com` | `Admin@123` | Sales telemetry, add/edit products, inventory stock manager, order fulfillment desk, CSV export |
| **Customer** | `customer@auramart.com` | `Customer@123` | Browse catalog, add to cart, apply coupons, simulated checkout gateway, order history & invoices |

---

## 🏗️ Technical Architecture & Project Structure

```
NET-web-project/
├── Controllers/
│   ├── HomeController.cs        # Hero showcase, value props, category pills
│   ├── ShopController.cs        # Multi-facet catalog, product details, star reviews
│   ├── CartController.cs        # Session/Cookie cart, quantity toggles, coupon applicator
│   ├── CheckoutController.cs    # Simulated payment gateway (UPI, Card, COD) & order placer
│   ├── OrdersController.cs      # Customer order tracking, status stepper, tax invoice
│   └── AdminController.cs       # Executive dashboard, Chart.js metrics, inventory CRUD, CSV export
├── Models/
│   ├── ApplicationUser.cs       # Customer profile, delivery address, phone
│   ├── Category.cs              # Product categories, icons, slugs
│   ├── Product.cs               # SKUs, pricing, stock quantities, star ratings, badges
│   ├── CartItem.cs              # Session and user-backed cart items
│   ├── Order.cs                 # Order reference, totals, status state machine
│   ├── OrderItem.cs             # Itemized order line items snapshot
│   ├── Review.cs                # Verified customer reviews and 1-5 star ratings
│   ├── Coupon.cs                # Promo codes (SAVE20, STUDENT50)
│   └── ViewModels/              # Strongly-typed models for dashboards & checkout
├── Data/
│   ├── ApplicationDbContext.cs  # EF Core context with relationships & unique indexes
│   └── DbInitializer.cs         # Zero-config role, category, and product seeder
├── Views/                       # Clean Razor Views with UI/UX Pro Max design tokens
├── wwwroot/
│   ├── css/site.css             # WCAG AA contrast tokens, bento cards, product cards
│   └── js/site.js               # Client validation & UI helpers
└── app.db                       # Self-contained SQLite database
```

---

## 🎨 UI/UX Pro Max Design System

Designed in accordance with **UI/UX Pro Max** standards for modern e-commerce portals:
- **Typography**: `Plus Jakarta Sans` for clean, high-conversion retail readability and `JetBrains Mono` for SKUs and invoice numbers.
- **Color Palette (WCAG AA Compliant)**:
  - `Primary (#059669)`: Emerald Green for trust, purchase actions, and prices.
  - `Accent (#ea580c)`: Warm Orange for urgency, sales badges, and CTAs.
  - `Surfaces (#ffffff / #f8fafc)`: Crisp white cards with subtle slate borders.
- **Accessibility & Polish**:
  - Minimum 44x44px touch targets.
  - No raw emojis as icons (curated SVG Bootstrap Icons only).
  - Floating toast notifications for "Added to Cart" and form feedback.
  - Responsive breakpoints for mobile, tablet, and desktop viewports.

---

## 🚀 How to Run the Application

### 1-Click Launch (Recommended)
Double-click **[`run.bat`](file:///c:/Users/saiyy/OneDrive/amtics/5th_sem/NET-web-project/run.bat)** in this folder. It starts the server and automatically opens your browser to:
👉 **`http://localhost:5220`**

### Or via Terminal
```bash
dotnet run --launch-profile http
```
Then navigate to **`http://localhost:5220`** in Chrome, Edge, or Brave.
