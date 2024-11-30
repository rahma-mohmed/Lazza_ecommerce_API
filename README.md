# laza-opal.vercel.app

A RESTful API for managing an e-commerce platform. This project provides endpoints for user authentication, product and brand management, cart and order processing, and more.

## Features

- **Authentication and Authorization**: User registration, login, logout, role assignment, and third-party login (Google/Facebook).
- **Product Management**: Add, update, view, and delete products with filtering by brand or searching.
- **Cart and Orders**: Add items to the cart, view items, and manage orders.
- **Favorites**: Manage favorite items.
- **Reviews**: Add and view product reviews.
- **Payments**: Basic payment processing integration.

## Technologies Used

- **Backend Framework**: ASP.NET Core
- **Database**: SQL Server
- **Authentication**: Identity, JWT
- **Documentation**: Swagger UI

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/rahma-mohmed/laza-opal.vercel.app.git

2. Install dependencies:
     ```bash
     dotnet restore

3. Update the database connection string in \`appsettings.json\`.
   
4. Run database migrations:
      ```bash
      dotnet ef database update

5. Start the application:
     ```bash
     dotnet run

## API Endpoints

### **Authentication**
- `GET /api/Auth/getAllUsers` - Retrieve all users.
- `POST /api/Auth/register` - Register a new user.
- `POST /api/Auth/login` - Log in a user.
- `POST /api/Auth/assign-role` - Assign a role to a user.
- And more...

### **Brand Management**
- `GET /api/Brand` - Retrieve all brands.
- `POST /api/Brand` - Add a new brand.
- `PUT /api/Brand/{id}` - Update a brand.
- `DELETE /api/Brand/{id}` - Delete a brand.

### **Product Management**
- `GET /api/Product` - Retrieve all products.
- `POST /api/Product` - Add a new product.
- `PUT /api/Product/{id}` - Update a product.
- `GET /api/Product/GetProductsByBrandId/{brandId}` - Retrieve products by brand.

### **Cart Management**
- `POST /api/Cart/add-item` - Add an item to the cart.
- `PUT /api/Cart/{itemId}` - Update cart item quantity.
- `DELETE /api/Cart/{id}` - Remove an item from the cart.

### **Favorites**
- `GET /api/Favorite` - View all favorite items.
- `DELETE /api/Favorite` - Remove an item from favorites.

### **Orders**
- `GET /api/Order/GetOrder` - Retrieve all orders.
- `POST /api/Order` - Create a new order.

### **Reviews**
- `POST /api/Review` - Add a review for a product.
- `GET /api/Review/{productId}` - Retrieve reviews for a product.

## Contributing

1. Fork the repository.
2. Create a new branch:
   ```bash
   git checkout -b feature-name
   ```
3. Make changes and commit:
   ```bash
   git commit -m "Description of changes"
   ```
4. Push to the branch:
   ```bash
   git push origin feature-name
   ```

---

## 🛠️ Technologies Used

This project leverages the following technologies:

- **ASP.NET Core Web API**: A framework for building robust and scalable APIs.
- **Entity Framework Core**: An Object-Relational Mapper (ORM) for seamless interaction with SQL Server databases.
- **SQL Server**: A relational database for storing essential data, including user, product, and order information.
- **JWT Authentication**: Provides secure authentication and token-based authorization for users.
- **Clean Architecture**: A design pattern that ensures clear separation of concerns, making the application easy to maintain and scale.
- **Swagger**: An API documentation tool that allows easy testing and exploration of the API endpoints. Swagger is also integrated for payment-related functionality.
- **SMTP/Email Service**: Used for sending transactional emails, including user registration confirmations and order notifications.
- **Strip Payment Integration**: Strip also helps in testing and simulating payment APIs, making it easy to verify payment flow in a sandbox environment.

---
## 📑 API Documentation

Check out the published API documentation here: [Swagger UI](https://lazza-opal-vercel-app.runasp.net/swagger/index.html)


## License

This project is licensed under the MIT License. See `LICENSE` for more details.


