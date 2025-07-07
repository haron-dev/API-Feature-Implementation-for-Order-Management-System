# Order Management System API

## Overview

This project is a .NET 8 Web API that simulates an order management system with features for discount calculation, order status tracking, and analytics.

## Features Implemented

### 1. Discounting System

- Implemented a flexible discount system that applies different promotion rules based on:
  - Customer segments (Regular, Silver, Gold, Platinum)
  - Order history (loyal customers with 10+ orders)
  - Order value (orders over $500)
- Discounts can be percentage-based or fixed amounts
- The system always applies the best discount for the customer

### 2. Order Status Tracking

- Implemented a state machine for order status transitions
- Valid transitions are enforced (e.g., Created → Processing → Shipped → Delivered)
- Each status change is recorded in a history log with timestamps and optional notes
- Prevents invalid status transitions

### 3. Order Analytics

- Created an endpoint that returns order analytics including:
  - Average order value
  - Average fulfillment time
  - Total orders and completed orders
  - Orders by status

## Technical Implementation

### Architecture

- Used a service-oriented architecture with clear separation of concerns
- Controllers handle HTTP requests and responses
- Services contain the business logic
- Models represent the domain entities

### Performance Optimization

- Implemented a decorator pattern with memory caching for discount-related operations
- Cached discount lists by segment to reduce database calls
- Set appropriate cache expiration policies

### API Documentation

- Used Swagger/OpenAPI annotations for comprehensive API documentation
- Enabled XML documentation generation for better API descriptions

### Testing

- Unit tests for the discount calculation logic
- Integration tests for the Orders API endpoints using WebApplicationFactory
- Tests cover various discount scenarios and order status transitions
- Custom test server configuration with mock repositories for isolated testing

#### Test Project Structure

- `DiscountServiceTests.cs` - Unit tests for discount calculation logic
- `OrdersControllerIntegrationTests.cs` - Integration tests for Orders API endpoints
- `CustomWebApplicationFactory.cs` - Test server configuration with mock repositories

## Assumptions

1. **Data Storage**: For simplicity, this implementation uses in-memory collections instead of a database. In a production environment, this would be replaced with a proper database.

2. **Authentication/Authorization**: The API doesn't implement authentication or authorization. In a real-world scenario, these would be essential.

3. **Validation**: Basic validation is implemented, but a production system would need more comprehensive validation.

4. **Concurrency**: The current implementation doesn't handle concurrency issues that might arise in a multi-user environment.

5. **Logging**: While status changes are tracked, a production system would need more comprehensive logging.

## How to Run

1. Clone the repository
2. Navigate to the project directory
3. Run `dotnet restore` to restore dependencies
4. Run `dotnet build` to build the project
5. Run `dotnet run` to start the API
6. Access the Swagger UI at `https://localhost:5001` or `http://localhost:5000/swagger`

## How to Test

### Automated Tests
1. Navigate to the solution root directory
2. Run `dotnet test` to execute all unit and integration tests
   - This will run tests for discount calculation logic
   - Tests for order status transitions
   - Tests for API endpoints
3. To run specific test classes:
   - `dotnet test --filter "FullyQualifiedName~DiscountServiceTests"`
   - `dotnet test --filter "FullyQualifiedName~OrdersControllerIntegrationTests"`
4. To run a specific test method:
   - `dotnet test --filter "FullyQualifiedName~OrdersControllerIntegrationTests.CreateOrder_ReturnsCreatedResponse"`

### Test Requirements
- Ensure the test project targets the same .NET version as the main project (net8.0)
- The integration tests use WebApplicationFactory to create an in-memory test server
- Mock repositories are used to isolate tests from external dependencies

### Manual Testing with Swagger UI
After starting the application, navigate to the Swagger UI to test the following endpoints:

#### Orders API
- `GET /api/orders` - Get all orders
- `GET /api/orders/{id}` - Get order by ID
- `POST /api/orders` - Create a new order
  ```json
  {
    "id": 0,
    "customerId": 1,
    "customerSegment": "Regular",
    "orderDate": "2023-07-06T00:00:00.000Z",
    "totalAmount": 100.00,
    "status": "Created",
    "items": [
      {
        "id": 0,
        "productId": 1,
        "quantity": 2,
        "unitPrice": 50.00
      }
    ]
  }
  ```
- `PUT /api/orders/{id}/status?status={status}&notes={notes}` - Update order status
  - Valid status values: Created, Processing, Shipped, Delivered, Cancelled
- `GET /api/orders/analytics` - Get order analytics data

#### Discounts API
- `GET /api/discounts` - Get all available discounts
- `GET /api/discounts/by-segment/{segment}` - Get discounts by customer segment
  - Valid segment values: Regular, Silver, Gold, Platinum

### Testing with HTTP File
You can also use the included `.http` file with REST Client in VS Code:
1. Open `OrderManagementSystem.http`
2. Click on "Send Request" above each request to execute it
3. View the response in the editor

### Testing Discount Rules
To test the discount system, create orders with different:
- Customer segments (Regular, Silver, Gold, Platinum)
- Order values (try orders over $500 to trigger value-based discounts)
- For loyal customer discounts, create a customer with 10+ orders
