# EijiDev Budget Pocket (Blazor + EF Core + SQLite)

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


## Features (CRUD & Real-Time Sync)

| Operation | Where | Details |
|---|---|---|
| **Create** | **Add Transaction** (`/add`) | Type (Segmented toggle: Expense / Income), Amount in ₱ (PHP), Category, Date, optional Description |
| **Read** | **Dashboard** (`/`) | Total Balance (`₱2,650.32`), PHP badge, percentage indicators, Spend/Income split cards, Categories Distribution progress bars, and recent activity |
| **Search & Filter** | **Transaction History** (`/history`) | Instant search keyword filter + Category dropdown + Type dropdown |
| **Update** | **Edit Modal** | Frosted backdrop blur modal with full validation and instant UI updates |
| **Delete** | **Delete Modal** | Confirmation dialog with safe cancellation |

Toast notifications provide instant user feedback for all actions.

---
