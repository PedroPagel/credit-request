CREATE DATABASE CofidisCredit;
GO

USE CofidisCredit;
GO

CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(15) NOT NULL,
    Nif NVARCHAR(15) NOT NULL UNIQUE,
    MonthlyIncome DECIMAL(18, 2) NOT NULL,
    RegistrationDate DATETIME NOT NULL
);

CREATE TABLE RiskAnalyses (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    UnemploymentRate DECIMAL(5,2) NOT NULL,
    InflationRate DECIMAL(5,2) NOT NULL,
    CreditHistoryScore DECIMAL(5,2) NOT NULL,
    OutstandingDebts DECIMAL(18,2) NOT NULL,
    RiskLevel INT NOT NULL,
    AnalysisDate DATETIME NOT NULL
);

CREATE TABLE CreditRequests (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    RiskAnalysisId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    AmountRequested DECIMAL(18,2) NOT NULL,
    TermInMonths INT NOT NULL,
    ApprovedAmount DECIMAL(18,2),
    RequestDate DATETIME NOT NULL,
    CONSTRAINT FK_CreditRequests_RiskAnalyses FOREIGN KEY (RiskAnalysisId) REFERENCES RiskAnalyses(Id)
);

GO

USE CofidisCredit;
GO

CREATE PROCEDURE sp_GetCreditLimitByIncome
    @MonthlyIncome DECIMAL(18, 2),
    @CreditLimit INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @MonthlyIncome < 1000
        SET @CreditLimit = 1000;
    ELSE IF @MonthlyIncome >= 1000 AND @MonthlyIncome < 2000
        SET @CreditLimit = 2000;
    ELSE
        SET @CreditLimit = 5000;
END;
GO

-- Create the test database
CREATE DATABASE CofidisCredit_Test;
GO

USE CofidisCredit_Test;
GO

CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(15) NOT NULL,
    Nif NVARCHAR(15) NOT NULL UNIQUE,
    MonthlyIncome DECIMAL(18, 2) NOT NULL,
    RegistrationDate DATETIME NOT NULL
);

CREATE TABLE RiskAnalyses (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    UnemploymentRate DECIMAL(5,2) NOT NULL,
    InflationRate DECIMAL(5,2) NOT NULL,
    CreditHistoryScore DECIMAL(5,2) NOT NULL,
    OutstandingDebts DECIMAL(18,2) NOT NULL,
    RiskLevel INT NOT NULL,
    AnalysisDate DATETIME NOT NULL
);

CREATE TABLE CreditRequests (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    RiskAnalysisId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    AmountRequested DECIMAL(18,2) NOT NULL,
    TermInMonths INT NOT NULL,
    ApprovedAmount DECIMAL(18,2),
    RequestDate DATETIME NOT NULL,
    CONSTRAINT FK_CreditRequests_RiskAnalyses FOREIGN KEY (RiskAnalysisId) REFERENCES RiskAnalyses(Id)
);

USE CofidisCredit_Test;
GO

CREATE PROCEDURE sp_GetCreditLimitByIncome
    @MonthlyIncome DECIMAL(18, 2),
    @CreditLimit INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @MonthlyIncome < 1000
        SET @CreditLimit = 1000;
    ELSE IF @MonthlyIncome >= 1000 AND @MonthlyIncome < 2000
        SET @CreditLimit = 2000;
    ELSE
        SET @CreditLimit = 5000;
END;
GO
