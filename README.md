# EijiDev Budget Pocket (Blazor + EF Core + SQLite)

A modern, responsive FinTech Web App built with Blazor Interactive Server, EF Core, and SQLite. Features a soft-clay, frosted-glass design inspired by modern FinTech Neo-Minimalism with typography powered by **Plus Jakarta Sans**, styled in Philippine Peso (PHP - ₱).

---

## Quick Start

### Requirements
- **.NET SDK 9** (or .NET 8 / 10).

```bash
# 1. Run the web app
dotnet run

# 2. Open your browser
# Navigate to: http://localhost:5080
```

The SQLite database `expenses.db` is generated and seeded automatically on first launch.

---

## Design & UI Highlights

- **Brand:** **EijiDev Budget Pocket** (PHP Financial Intelligence / PHP Financial Tracker).
- **Currency:** Full **Philippine Peso (₱ / PHP)** support across all balance indicators, charts, stat cards, and transaction records.
- **Aesthetic:** Neo-minimalist, soft-clay surfaces, frosted glass pill badges, convex metallic chrome icons, and smooth organic squircle corners.
- **Pinned Bottom Navigation Dock:** Pinned to the viewport with `position: fixed` (`bottom-4 left-1/2 -translate-x-1/2 z-50`), with `pb-24` (`padding-bottom: 6rem`) on the content container to ensure the last transaction item is never covered up while scrolling.
- **Responsive Web & Mobile:** 
  - On desktop: Full sidebar + top bar + 3 stat cards + 2-panel layout (Balance & Overviews + Analytics & Breakdown).
  - On mobile/tablet: Seamless single-column view with touch-friendly navigation via the pinned bottom floating dock.
- **Login Screen:** Dedicated login page (`/login`) matching the exact frosted neo-minimalist clay design with demo one-click access and authentication state tracking.

---

## Features (CRUD & Real-Time Sync)

| Operation | Where | Details |
|---|---|---|
| **Create** | **Add Transaction** (`/add`) | Type (Segmented toggle: Expense / Income), Amount in ₱ (PHP), Category, Date, optional Description |
| **Read** | **Dashboard** (`/`) | Total Balance (`₱2,650.32`), PHP badge, percentage indicators, Spend/Income split cards, Categories Distribution progress bars, and recent activity |
| **Search & Filter** | **Transaction History** (`/history`) | Instant search keyword filter + Category dropdown + Type dropdown |
| **Update** | **Edit Modal** | Frosted backdrop blur modal with full validation and instant UI updates |
| **Delete** | **Delete Modal** | Confirmation dialog with safe cancellation |
| **Login / Auth** | **Account Login** (`/login`) | Neo-minimalist login card with demo credentials and session state |

Toast notifications provide instant user feedback for all actions.

---

## Running Automated Tests

```bash
dotnet run --project tests/ExpenseTracker.Tests/ExpenseTracker.Tests.csproj
```
