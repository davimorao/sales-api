CREATE TABLE Customer (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    CustomerName VARCHAR(100) NOT NULL  
);

CREATE TABLE Branch (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    BranchName VARCHAR(100) NOT NULL
);

CREATE TABLE Product (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    ProductName VARCHAR(100) NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL
);

CREATE TABLE Sale (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    SaleNumber VARCHAR(20) NOT NULL,
    SaleDate DATETIME NOT NULL,
    CustomerId BIGINT NOT NULL,
    BranchId BIGINT NOT NULL,
    TotalSaleValue DECIMAL(18,2) NOT NULL,
    SaleStatus INT NOT NULL DEFAULT 0,
    FOREIGN KEY (CustomerId) REFERENCES Customer(Id),
    FOREIGN KEY (BranchId) REFERENCES Branch(Id)
);

CREATE TABLE SaleItem (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    SaleId BIGINT NOT NULL,
    ProductId BIGINT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    Discount DECIMAL(18,2) NOT NULL,
    TotalItemValue AS ((Quantity * UnitPrice) - Discount) PERSISTED,
    FOREIGN KEY (SaleId) REFERENCES Sale(Id),
    FOREIGN KEY (ProductId) REFERENCES Product(Id)
);


-- CREATE INDEX IDX_Sale_SaleDate ON Sale(SaleDate);
-- CREATE INDEX IDX_SaleItem_ProductId ON SaleItem(ProductId);