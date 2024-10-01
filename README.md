# Sales API

This project contains a Sales API that uses a .NET 8 application with an SQL Server database and Flyway for migrations. The application provides endpoints for managing sales, customers, products, and branches. The database is initialized with sample data upon startup.

## Prerequisites

Ensure you have the following installed:

- Docker
- Docker Compose

## Getting Started

1. **Clone the repository**

   Clone this repository to your local machine:

   ```bash
   git clone https://github.com/davimorao/sales-api.git

2. **Navigate to the project directory**

   ```bash
   cd sales-api

3. **Run the application**

   ```bash
   docker-compose up --build -d

 This will do the following:

- Build and run the SQL Server container.
- Run the migrations to create and initialize the database.
- Build and run the Sales API container.

4. **Access the API documentation**

   Once the containers are up and running, you can access the API Swagger documentation in your browser:

   ```bash
   http://localhost:8080/swagger/index.html
   http://localhost:8081/swagger/index.html


## Database Structure and Initial Data
The SQL Server database is automatically created and populated with initial data upon startup. The data includes:
goo

## Table Branch
| Id                   BranchName |
|---------------------------------|
| 201                  Filial X   |
| 202                  Filial Y   |

## Table Customer
| Id                   CustomerName |
|-----------------------------------|
| 101                  Customer A   |
| 102                  Customer B   |

## Table Customer
| Id                   ProductName                                                                                          UnitPrice |
|-------------------------------------------------------------------------------------------------------------------------------------|
| 301                  Produto 1                                                                                            200.00    |
| 302                  Produto 2                                                                                            600.00    |
| 303                  Produto 3                                                                                            100.00    |

## Table Sale
| Id                   SaleNumber           SaleDate                CustomerId           BranchId             TotalSaleValue                          SaleStatus |
|----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 1                    S001                 2024-09-30 22:09:02.897 101                  201                  1000.00                                 0          |
| 2                    S002                 2024-09-30 22:09:02.907 102                  202                  500.00                                  0          |


## Table SaleItem
| Id                   SaleId               ProductId            Quantity    UnitPrice                               Discount                                TotalItemValue |
|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 1                    1                    301                  2           200.00                                  0.00                                    400.00         |
| 2                    1                    302                  1           600.00                                  0.00                                    600.00         |
| 3                    2                    303                  5           100.00                                  0.00                                    500.00         |

## Create a Sale

   Run a request with **Method: POST** and **Action: /api/Sale**
   
   ```bash
   Body:
   {
  "saleDate": "2024-10-01T00:54:46.999Z",
  "customerId": 101,
  "branchId": 201,
  "items": [
    {
      "productId": 303,
      "quantity": 5,
      "unitPrice": 100,
      "discount": 10
    },
{
      "productId": 302,
      "quantity": 5,
      "unitPrice": 100,
      "discount": 10
    },
{
      "productId": 301,
      "quantity": 5,
      "unitPrice": 100,
      "discount": 10
    }
  ]
}
