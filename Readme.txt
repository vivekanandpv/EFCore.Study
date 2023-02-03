IMPORTANT

Steps required for setting up this study
1. Please create a database in localdb called SalesManagement
2. Use the following SQL script to create the required database schema
3. Install the following packages:
	Microsoft.EntityFrameworkCore
	Microsoft.EntityFrameworkCore.SqlServer


-------------
SQL Script
-------------

CREATE TABLE Customer (
	CustomerId INT PRIMARY KEY IDENTITY(1,1),
	LegalName NVARCHAR(200) NOT NULL,
	Gstin NVARCHAR(15) NOT NULL,
);

CREATE TABLE CustomerAddress(
	AddressId INT PRIMARY KEY IDENTITY(1,1),
	AddressLine1 NVARCHAR(200) NULL,
	AddressLine2 NVARCHAR(200) NULL,
	AddressLine3 NVARCHAR(200) NULL,
	City NVARCHAR(200) NULL,
	StateCode NVARCHAR(200) NULL,
	Pin INT NULL,
	CustomerId INT NOT NULL,
	FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId)
);

CREATE TABLE Invoice (
	InvoiceId INT PRIMARY KEY IDENTITY(1,1),
	InvoiceDate DATETIME2 NOT NULL,
	CustomerId INT NOT NULL,
	AddressId INT NOT NULL,
	Narration NVARCHAR(200),
	FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId)
);

CREATE TABLE Product (
	ProductId INT PRIMARY KEY IDENTITY(1,1),
	ProductName NVARCHAR(200) NOT NULL,
	Price NUMERIC(12,2) NOT NULL DEFAULT 0.0,
	TaxRate NUMERIC(4,2) NOT NULL DEFAULT 0.0,
	CONSTRAINT CHX_ProductPrice CHECK (Price >=0),
	CONSTRAINT CHX_TaxRate CHECK (TaxRate >= 0 AND TaxRate <= 99.99)
);

CREATE TABLE LineItem (
	InvoiceId INT NOT NULL,
	ProductId INT NOT NULL,
	BasePrice NUMERIC(12,2) NOT NULL,
	TaxRate NUMERIC(4,2) NOT NULL,
	Quantity NUMERIC(10,4) NOT NULL,
	TaxableAmount NUMERIC(12,2) NOT NULL,
	TaxAmount NUMERIC(12,2) NOT NULL,
	LineAmount NUMERIC(12,2) NOT NULL,
	PRIMARY KEY (InvoiceId,ProductId),
	FOREIGN KEY (InvoiceId) REFERENCES Invoice(InvoiceId),
	FOREIGN KEY (ProductId) REFERENCES Product(ProductId),
	CONSTRAINT CHX_BasePrice CHECK (BasePrice >= 0),
	CONSTRAINT CHX_LI_TaxRate CHECK (TaxRate >=0 AND TaxRate <= 99.99),
	CONSTRAINT CHX_Quantity CHECK (Quantity > 0),
	CONSTRAINT CHX_TaxableAmount CHECK (TaxableAmount >= 0),
	CONSTRAINT CHX_TaxAmount CHECK (TaxAmount >= 0),
	CONSTRAINT CHX_LineAmount CHECK (LineAmount >= 0)
);