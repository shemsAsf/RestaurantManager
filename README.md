# RestaurantManager

> ⚠️ **Work in Progress** - This project is actively under development and not yet production-ready.

A web-based restaurant ordering system that allows customers to browse a menu, manage a shared basket per table, and place orders directly from their phones - no app install required.

---

## What It's Meant to Become

### Customer Side
Customers scan a QR code or navigate to the app on their phone, enter their table number, and get instant access to the restaurant's menu. They can:

- Browse menu items grouped by category, with photos, descriptions and prices
- Add and remove items from a shared basket
- See live basket updates when someone else at the same table adds or removes items (via SignalR)
- Review and confirm their order, then continue ordering in additional rounds
- View all orders placed during their session

### Kitchen Side
Cooks get a dedicated real-time view of all incoming orders. Each item can be individually marked as done, giving the kitchen granular control over what has been prepared.

### Admin / Manager Side
Staff with elevated access can:

- Monitor all open tables and their current baskets and orders in real time
- Get notified instantly when a new order is placed
- Close tables at the end of a session
- Review past sessions and their full order history

---

## Tech Stack

### Backend
| Technology | Role |
|---|---|
| **ASP.NET Core MVC (.NET 10)** | Main web framework - routing, controllers, views |
| **Entity Framework Core** | ORM - maps C# models to PostgreSQL tables |
| **ASP.NET Core Identity** | Authentication and role-based access control |
| **SignalR** | Real-time WebSocket communication between phones and server |

### Database
| Technology | Role |
|---|---|
| **PostgreSQL** | Primary relational database |
| **Redis** *(planned)* | SignalR backplane for multi-instance deployments |

### Frontend
| Technology | Role |
|---|---|
| **Razor Views (.cshtml)** | Server-rendered HTML templates |
| **Tailwind CSS** | Utility-first mobile-first styling |
| **Alpine.js** | Lightweight browser reactivity (no-reload interactions) |
| **SignalR JS Client** | Real-time connection from the browser |

### Infrastructure
| Technology | Role |
|---|---|
| **Docker** | Local development and deployment containerisation |

---

## Roles & Access

| Role | Access |
|---|---|
| **Admin** | Full access - accounts, menu, all tables, history |
| **Manager** | Dashboard, open/close tables, view orders |
| **Cook** | Kitchen view - incoming orders, mark items done |
| **Waiter** | Table status overview |
| *(no account)* | Customer-facing menu, basket, and ordering |

---

## Project Structure

```
RestaurantManager/
├── Controllers/         # HTTP request handlers
├── Data/                # AppDbContext and database configuration
├── Extensions/          # Helper extensions (e.g. session resolution)
├── Migrations/          # EF Core schema migrations
├── Models/              # Database entities
├── ViewModels/          # Data shapes passed to Razor views
├── Views/               # .cshtml templates
│   ├── Basket/
│   ├── Menu/
│   ├── Order/
│   ├── Session/
│   └── Shared/          # Layout, partials
└── wwwroot/             # Static files (CSS, JS, images)
```

---

## Current Status

- [x] Project setup - .NET 10 MVC, EF Core, Identity, PostgreSQL
- [x] Table session system - enter table number, share session across phones
- [x] Menu page - categories, items, images, live quantity controls
- [x] Basket - add, remove, adjust quantities, shared per table
- [x] Order placement - price snapshot on confirm, multiple rounds supported
- [ ] SignalR real-time sync across phones at the same table
- [ ] Kitchen view for cooks
- [ ] Admin dashboard - open tables, order notifications, close table
- [ ] Past sessions and order history (admin)
- [ ] Staff account management UI
- [ ] Menu management UI (admin)
- [ ] Production deployment pipeline

---

## Getting Started (Development)

### Prerequisites
- .NET 10 SDK
- Docker Desktop
- Node.js (for Tailwind CSS build)
- Postgress (PgAdmin 4)

### Run locally

```bash
# Start the database
docker compose up db -d

# Apply migrations
dotnet ef database update

# Run the app
dotnet run --project RestaurantManager
```

### Build Tailwind CSS

```bash
npm install
npm run css:watch
```

The app will be available at `http://localhost:5129`.  
To test on a phone on the same network, use `http://<your-local-ip>:5129`.

---

## Environment Configuration

Connection strings and secrets go in `appsettings.Development.json` (gitignored). Copy this template:

```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Port=5432;Database=restaurant_dev;Username=postgres;Password=yourpassword"
  }
}
```

---

*Built with ASP.NET Core · Tailwind CSS · Alpine.js · PostgreSQL*