-- Customer
IF NOT EXISTS (SELECT 1 FROM Customer WHERE Id = 101)
    INSERT INTO Customer (Id, CustomerName) VALUES (101, 'Customer A');

IF NOT EXISTS (SELECT 1 FROM Customer WHERE Id = 102)
    INSERT INTO Customer (Id, CustomerName) VALUES (102, 'Customer B');

-- Branch
IF NOT EXISTS (SELECT 1 FROM Branch WHERE Id = 201)
    INSERT INTO Branch (Id, BranchName) VALUES (201, 'Filial X');

IF NOT EXISTS (SELECT 1 FROM Branch WHERE Id = 202)
    INSERT INTO Branch (Id, BranchName) VALUES (202, 'Filial Y');

-- Product
IF NOT EXISTS (SELECT 1 FROM Product WHERE Id = 301)
    INSERT INTO Product (Id, ProductName, UnitPrice) VALUES (301, 'Produto 1', 200.00);

IF NOT EXISTS (SELECT 1 FROM Product WHERE Id = 302)
    INSERT INTO Product (Id, ProductName, UnitPrice) VALUES (302, 'Produto 2', 600.00);

IF NOT EXISTS (SELECT 1 FROM Product WHERE Id = 303)
    INSERT INTO Product (Id, ProductName, UnitPrice) VALUES (303, 'Produto 3', 100.00);

-- Sale
IF NOT EXISTS (SELECT 1 FROM Sale WHERE SaleNumber = 'S001')
    INSERT INTO Sale (SaleNumber, SaleDate, CustomerId, BranchId, TotalSaleValue, SaleStatus)
    VALUES ('S001', GETDATE(), 101, 201, 1000.00, 0);

IF NOT EXISTS (SELECT 1 FROM Sale WHERE SaleNumber = 'S002')
    INSERT INTO Sale (SaleNumber, SaleDate, CustomerId, BranchId, TotalSaleValue, SaleStatus)
    VALUES ('S002', GETDATE(), 102, 202, 500.00, 0);

-- SaleItem
IF NOT EXISTS (SELECT 1 FROM SaleItem WHERE SaleId = 1 AND ProductId = 301)
    INSERT INTO SaleItem (SaleId, ProductId, Quantity, UnitPrice, Discount)
    VALUES (1, 301, 2, 200.00, 0.00);

IF NOT EXISTS (SELECT 1 FROM SaleItem WHERE SaleId = 1 AND ProductId = 302)
    INSERT INTO SaleItem (SaleId, ProductId, Quantity, UnitPrice, Discount)
    VALUES (1, 302, 1, 600.00, 0.00);

IF NOT EXISTS (SELECT 1 FROM SaleItem WHERE SaleId = 2 AND ProductId = 303)
    INSERT INTO SaleItem (SaleId, ProductId, Quantity, UnitPrice, Discount)
    VALUES (2, 303, 5, 100.00, 0.00);
