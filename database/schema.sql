-- Truck Logistics Database Schema
-- SQL Server

CREATE DATABASE TruckLogisticsDB;
GO

USE TruckLogisticsDB;
GO

-- Carriers Table
CREATE TABLE Carriers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    MCNumber NVARCHAR(50) NOT NULL UNIQUE,
    DOTNumber NVARCHAR(50),
    ContactName NVARCHAR(200),
    ContactEmail NVARCHAR(200),
    ContactPhone NVARCHAR(50),
    Address NVARCHAR(500),
    City NVARCHAR(100),
    State NVARCHAR(50),
    ZipCode NVARCHAR(20),
    IsActive BIT NOT NULL DEFAULT 1,
    Rating DECIMAL(3,2),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Drivers Table
CREATE TABLE Drivers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CarrierId INT NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    LicenseNumber NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(50),
    Email NVARCHAR(200),
    IsAvailable BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Drivers_Carriers FOREIGN KEY (CarrierId) REFERENCES Carriers(Id)
);

-- Loads Table
CREATE TABLE Loads (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    LoadNumber NVARCHAR(50) NOT NULL UNIQUE,
    CarrierId INT,
    DriverId INT,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Available',  -- Available, Booked, InTransit, Delivered, Cancelled
    EquipmentType NVARCHAR(100) NOT NULL,              -- Flatbed, Dry Van, Reefer, etc.
    Weight DECIMAL(10,2),
    Commodity NVARCHAR(200),
    PickupAddress NVARCHAR(500) NOT NULL,
    PickupCity NVARCHAR(100) NOT NULL,
    PickupState NVARCHAR(50) NOT NULL,
    PickupZip NVARCHAR(20),
    PickupDate DATETIME2 NOT NULL,
    DeliveryAddress NVARCHAR(500) NOT NULL,
    DeliveryCity NVARCHAR(100) NOT NULL,
    DeliveryState NVARCHAR(50) NOT NULL,
    DeliveryZip NVARCHAR(20),
    DeliveryDate DATETIME2 NOT NULL,
    Miles DECIMAL(10,2),
    Rate DECIMAL(10,2),
    Notes NVARCHAR(MAX),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Loads_Carriers FOREIGN KEY (CarrierId) REFERENCES Carriers(Id),
    CONSTRAINT FK_Loads_Drivers FOREIGN KEY (DriverId) REFERENCES Drivers(Id)
);

-- Tracking Events Table
CREATE TABLE TrackingEvents (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    LoadId INT NOT NULL,
    EventType NVARCHAR(100) NOT NULL,   -- PickedUp, InTransit, Delayed, Delivered, etc.
    Location NVARCHAR(500),
    City NVARCHAR(100),
    State NVARCHAR(50),
    Latitude DECIMAL(10,6),
    Longitude DECIMAL(10,6),
    Notes NVARCHAR(MAX),
    EventTime DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_TrackingEvents_Loads FOREIGN KEY (LoadId) REFERENCES Loads(Id)
);

-- Indexes
CREATE INDEX IX_Loads_Status ON Loads(Status);
CREATE INDEX IX_Loads_PickupDate ON Loads(PickupDate);
CREATE INDEX IX_Loads_CarrierId ON Loads(CarrierId);
CREATE INDEX IX_TrackingEvents_LoadId ON TrackingEvents(LoadId);
CREATE INDEX IX_Drivers_CarrierId ON Drivers(CarrierId);

-- Sample Seed Data
INSERT INTO Carriers (Name, MCNumber, DOTNumber, ContactName, ContactEmail, ContactPhone, City, State, Rating)
VALUES
    ('Swift Transport', 'MC-123456', 'DOT-654321', 'John Smith', 'jsmith@swift.com', '555-100-0001', 'Phoenix', 'AZ', 4.5),
    ('Werner Enterprises', 'MC-234567', 'DOT-765432', 'Jane Doe', 'jdoe@werner.com', '555-100-0002', 'Omaha', 'NE', 4.7),
    ('JB Hunt Transport', 'MC-345678', 'DOT-876543', 'Bob Johnson', 'bjohnson@jbhunt.com', '555-100-0003', 'Lowell', 'AR', 4.8);

INSERT INTO Drivers (CarrierId, FirstName, LastName, LicenseNumber, Phone, Email)
VALUES
    (1, 'Mike', 'Davis', 'CDL-AZ-001', '555-200-0001', 'mdavis@swift.com'),
    (1, 'Sarah', 'Wilson', 'CDL-AZ-002', '555-200-0002', 'swilson@swift.com'),
    (2, 'Tom', 'Brown', 'CDL-NE-001', '555-200-0003', 'tbrown@werner.com'),
    (3, 'Lisa', 'Garcia', 'CDL-AR-001', '555-200-0004', 'lgarcia@jbhunt.com');

INSERT INTO Loads (LoadNumber, CarrierId, DriverId, Status, EquipmentType, Weight, Commodity, PickupAddress, PickupCity, PickupState, PickupDate, DeliveryAddress, DeliveryCity, DeliveryState, DeliveryDate, Miles, Rate)
VALUES
    ('TL-2024-001', 1, 1, 'InTransit', 'Dry Van', 42000, 'Electronics', '123 Warehouse Dr', 'Chicago', 'IL', '2024-12-01 08:00', '456 Distribution Blvd', 'Dallas', 'TX', '2024-12-03 16:00', 921, 2500.00),
    ('TL-2024-002', 2, 3, 'Booked', 'Reefer', 35000, 'Produce', '789 Cold Storage Rd', 'Los Angeles', 'CA', '2024-12-02 06:00', '321 Market St', 'Seattle', 'WA', '2024-12-04 12:00', 1135, 3200.00),
    ('TL-2024-003', NULL, NULL, 'Available', 'Flatbed', NULL, 'Steel Coils', '555 Steel Mill Ave', 'Pittsburgh', 'PA', '2024-12-05 07:00', '999 Auto Plant Rd', 'Detroit', 'MI', '2024-12-06 15:00', 289, 1100.00),
    ('TL-2024-004', 3, 4, 'Delivered', 'Dry Van', 28000, 'Clothing', '100 Fashion Blvd', 'New York', 'NY', '2024-11-28 09:00', '200 Retail Row', 'Miami', 'FL', '2024-11-30 17:00', 1278, 3800.00);
GO
