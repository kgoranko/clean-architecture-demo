/*
    Clean Architecture Demo SQL Server setup.

    Create the database yourself first, then run this script inside that database.

    Example:
      CREATE DATABASE CleanArchitectureDemo;
      GO
      USE CleanArchitectureDemo;
      GO
      -- run this script
*/

SET XACT_ABORT ON;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE [name] = N'Demo')
BEGIN
    EXEC(N'CREATE SCHEMA [Demo]');
END;
GO

IF OBJECT_ID(N'[Demo].[Users]', N'U') IS NULL
BEGIN
    CREATE TABLE [Demo].[Users]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [Email] NVARCHAR(320) NOT NULL,
        [FirstName] NVARCHAR(100) NOT NULL,
        [LastName] NVARCHAR(100) NOT NULL,
        [FullName] NVARCHAR(220) NOT NULL,
        [CreatedAt] DATETIME2(7) NOT NULL,
        [IsActive] BIT NOT NULL,
        [WelcomeEmailSentAt] DATETIME2(7) NULL,
        [DeactivatedAt] DATETIME2(7) NULL,
        [DeactivationReason] NVARCHAR(500) NULL,
        CONSTRAINT [PK_Demo_Users] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [UX_Demo_Users_Email] UNIQUE ([Email]),
        CONSTRAINT [CK_Demo_Users_Email_NotEmpty] CHECK (LEN(LTRIM(RTRIM([Email]))) > 0),
        CONSTRAINT [CK_Demo_Users_FirstName_NotEmpty] CHECK (LEN(LTRIM(RTRIM([FirstName]))) > 0),
        CONSTRAINT [CK_Demo_Users_LastName_NotEmpty] CHECK (LEN(LTRIM(RTRIM([LastName]))) > 0),
        CONSTRAINT [CK_Demo_Users_FullName_NotEmpty] CHECK (LEN(LTRIM(RTRIM([FullName]))) > 0)
    );
END;
GO

IF OBJECT_ID(N'[Demo].[Orders]', N'U') IS NULL
BEGIN
    CREATE TABLE [Demo].[Orders]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [CustomerName] NVARCHAR(200) NOT NULL,
        [ProductName] NVARCHAR(200) NOT NULL,
        [Quantity] INT NOT NULL,
        [UnitPrice] DECIMAL(18, 2) NOT NULL,
        [TotalPrice] DECIMAL(18, 2) NOT NULL,
        [Status] NVARCHAR(40) NOT NULL,
        [CreatedAt] DATETIME2(7) NOT NULL,
        [ProcessedAt] DATETIME2(7) NULL,
        [CancelledAt] DATETIME2(7) NULL,
        [CancellationReason] NVARCHAR(500) NULL,
        [ShippedAt] DATETIME2(7) NULL,
        CONSTRAINT [PK_Demo_Orders] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [CK_Demo_Orders_CustomerName_NotEmpty] CHECK (LEN(LTRIM(RTRIM([CustomerName]))) > 0),
        CONSTRAINT [CK_Demo_Orders_ProductName_NotEmpty] CHECK (LEN(LTRIM(RTRIM([ProductName]))) > 0),
        CONSTRAINT [CK_Demo_Orders_Quantity_Positive] CHECK ([Quantity] > 0),
        CONSTRAINT [CK_Demo_Orders_UnitPrice_Positive] CHECK ([UnitPrice] > 0),
        CONSTRAINT [CK_Demo_Orders_TotalPrice_Positive] CHECK ([TotalPrice] > 0),
        CONSTRAINT [CK_Demo_Orders_Status] CHECK ([Status] IN (N'Pending', N'Confirmed', N'Cancelled', N'Shipped'))
    );
END;
GO

IF OBJECT_ID(N'[Demo].[ProductCatalog]', N'U') IS NULL
BEGIN
    CREATE TABLE [Demo].[ProductCatalog]
    (
        [ProductName] NVARCHAR(200) NOT NULL,
        [Price] DECIMAL(18, 2) NOT NULL,
        [StockQuantity] INT NOT NULL,
        [IsActive] BIT NOT NULL,
        CONSTRAINT [PK_Demo_ProductCatalog] PRIMARY KEY CLUSTERED ([ProductName]),
        CONSTRAINT [CK_Demo_ProductCatalog_ProductName_NotEmpty] CHECK (LEN(LTRIM(RTRIM([ProductName]))) > 0),
        CONSTRAINT [CK_Demo_ProductCatalog_Price_Positive] CHECK ([Price] > 0),
        CONSTRAINT [CK_Demo_ProductCatalog_StockQuantity_NotNegative] CHECK ([StockQuantity] >= 0)
    );
END;
GO

MERGE [Demo].[ProductCatalog] AS [Target]
USING
(
    VALUES
        (N'Laptop', 999.99, 50, CONVERT(BIT, 1)),
        (N'Phone', 599.99, 100, CONVERT(BIT, 1)),
        (N'Tablet', 399.99, 30, CONVERT(BIT, 1)),
        (N'Headphones', 149.99, 200, CONVERT(BIT, 1))
) AS [Source] ([ProductName], [Price], [StockQuantity], [IsActive])
ON [Target].[ProductName] = [Source].[ProductName]
WHEN MATCHED THEN
    UPDATE SET
        [Price] = [Source].[Price],
        [StockQuantity] = [Source].[StockQuantity],
        [IsActive] = [Source].[IsActive]
WHEN NOT MATCHED THEN
    INSERT ([ProductName], [Price], [StockQuantity], [IsActive])
    VALUES ([Source].[ProductName], [Source].[Price], [Source].[StockQuantity], [Source].[IsActive]);
GO

MERGE [Demo].[Users] AS [Target]
USING
(
    VALUES
        ('11111111-1111-1111-1111-111111111111', N'admin@company.com', N'Admin', N'User', N'Admin User', DATEADD(MONTH, -3, SYSUTCDATETIME()), CONVERT(BIT, 1), DATEADD(MONTH, -3, SYSUTCDATETIME())),
        ('22222222-2222-2222-2222-222222222222', N'john.doe@company.com', N'John', N'Doe', N'John Doe', DATEADD(MONTH, -1, SYSUTCDATETIME()), CONVERT(BIT, 1), DATEADD(MONTH, -1, SYSUTCDATETIME()))
) AS [Source] ([Id], [Email], [FirstName], [LastName], [FullName], [CreatedAt], [IsActive], [WelcomeEmailSentAt])
ON [Target].[Email] = [Source].[Email]
WHEN NOT MATCHED THEN
    INSERT ([Id], [Email], [FirstName], [LastName], [FullName], [CreatedAt], [IsActive], [WelcomeEmailSentAt])
    VALUES ([Source].[Id], [Source].[Email], [Source].[FirstName], [Source].[LastName], [Source].[FullName], [Source].[CreatedAt], [Source].[IsActive], [Source].[WelcomeEmailSentAt]);
GO

COMMIT TRANSACTION;
GO
