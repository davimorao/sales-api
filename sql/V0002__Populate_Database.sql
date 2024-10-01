-- Customer
IF NOT EXISTS (SELECT 1 FROM Customer WHERE CustomerName = 'Customer A')
    INSERT INTO Customer (CustomerName) VALUES ('Customer A');

IF NOT EXISTS (SELECT 1 FROM Customer WHERE CustomerName = 'Customer B')
    INSERT INTO Customer (CustomerName) VALUES ('Customer B');

-- Branch
IF NOT EXISTS (SELECT 1 FROM Branch WHERE BranchName = 'Filial X')
    INSERT INTO Branch (BranchName) VALUES ('Filial X');

IF NOT EXISTS (SELECT 1 FROM Branch WHERE BranchName = 'Filial Y')
    INSERT INTO Branch (BranchName) VALUES ('Filial Y');

-- Product
IF NOT EXISTS (SELECT 1 FROM Product WHERE ProductName = 'Produto 1')
    INSERT INTO Product (ProductName, UnitPrice) VALUES ('Produto 1', 200.00);

IF NOT EXISTS (SELECT 1 FROM Product WHERE ProductName = 'Produto 2')
    INSERT INTO Product (ProductName, UnitPrice) VALUES ('Produto 2', 600.00);

IF NOT EXISTS (SELECT 1 FROM Product WHERE ProductName = 'Produto 3')
    INSERT INTO Product (ProductName, UnitPrice) VALUES ('Produto 3', 100.00);

-- Sale
IF NOT EXISTS (SELECT 1 FROM Sale WHERE SaleNumber = 'S001')
    INSERT INTO Sale (SaleNumber, SaleDate, CustomerId, BranchId, TotalSaleValue, SaleStatus)
    VALUES ('S001', GETDATE(), 1, 1, 1000.00, 0);

IF NOT EXISTS (SELECT 1 FROM Sale WHERE SaleNumber = 'S002')
    INSERT INTO Sale (SaleNumber, SaleDate, CustomerId, BranchId, TotalSaleValue, SaleStatus)
    VALUES ('S002', GETDATE(), 2, 2, 500.00, 0);

-- SaleItem
IF NOT EXISTS (SELECT 1 FROM SaleItem WHERE SaleId = 1 AND ProductId = 1)
    INSERT INTO SaleItem (SaleId, ProductId, Quantity, UnitPrice, Discount)
    VALUES (1, 1, 2, 200.00, 0.00);

IF NOT EXISTS (SELECT 1 FROM SaleItem WHERE SaleId = 1 AND ProductId = 2)
    INSERT INTO SaleItem (SaleId, ProductId, Quantity, UnitPrice, Discount)
    VALUES (1, 2, 1, 600.00, 0.00);

IF NOT EXISTS (SELECT 1 FROM SaleItem WHERE SaleId = 2 AND ProductId = 3)
    INSERT INTO SaleItem (SaleId, ProductId, Quantity, UnitPrice, Discount)
    VALUES (2, 3, 5, 100.00, 0.00);
