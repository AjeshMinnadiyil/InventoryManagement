# Inventory Management System

An ASP.NET Core MVC web application for managing products, categories, suppliers, and stock transactions. The system provides role-based access for Admin and Staff users to manage inventory efficiently.

## Features

- Role-based authentication using ASP.NET Core Identity
- Two user roles: Admin and Staff
- Dashboard with inventory statistics and low-stock information
- Create, view, update, and delete products
- Manage product categories and suppliers
- Search and filter products
- Stock In and Stock Out transactions
- Track stock transaction history
- Role-based authorization for inventory operations
- Form validation using model validation attributes
- Entity Framework Core database migrations

## Technologies Used

- C#
- ASP.NET Core MVC
- .NET 10
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Razor Views
- HTML, CSS, and Bootstrap

## User Roles

### Admin
- View the inventory dashboard
- Manage products, categories, and suppliers
- View product information
- Perform stock transactions
- Access inventory management features

### Staff
- View products and product details
- Perform permitted stock transactions
- View stock information and transaction history
- Access features allowed by their assigned role

## Getting Started

### Prerequisites

- .NET 10 SDK
- Visual Studio 2022 or a compatible .NET IDE
- SQL Server or SQL Server LocalDB

### Installation

1. Clone the repository:

   ```bash
   git clone YOUR_GITHUB_REPOSITORY_URL
   ```

2. Open the solution in Visual Studio.

3. Update the `DefaultConnection` connection string in `appsettings.json` to match your SQL Server instance.

4. Open the Package Manager Console and select the project containing the EF Core migrations.

5. Apply the database migrations:

   ```powershell
   Update-Database
   ```

6. Run the application.

## Demo Accounts

Use the following demo accounts for local testing, provided the development seeding code has run successfully.

| Role | Email | Password |
|---|---|---|
| Admin | `admin@inventory.com` | `Admin@123` |
| Staff | `staff@inventory.com` | `Staff@12345` |

**Note:** These credentials are for development and demonstration only. Do not use them in production.

## Database

The application uses SQL Server for data storage and Entity Framework Core for database operations. Migrations are used to create and update the database schema.

## Future Improvements

- Add reports for stock movement
- Export inventory data to Excel or PDF
- Add pagination and advanced filtering
- Improve dashboard charts and analytics

## Author

**Ajesh Minnadiyil**

GitHub: [AjeshMinnadiyil](https://github.com/AjeshMinnadiyil)
