using Microsoft.Data.SqlClient;

namespace Cofidis.Credit.Tests.Integration
{
    public class DatabaseTestFixture
    {
        private readonly string _connectionString;

        public DatabaseTestFixture()
        {
            _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=CofidisCredit_Test;Trusted_Connection=True;MultipleActiveResultSets=true";
        }

        public void CleanDatabase()
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                DELETE FROM CreditRequests;
                DELETE FROM RiskAnalyses;
                DELETE FROM Users;";

            command.ExecuteNonQuery();
        }

        public void SeedCrediRequestData()
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                -- Insert into Users
                INSERT INTO Users (Id, FullName, Email, PhoneNumber, Nif, MonthlyIncome, RegistrationDate)
                VALUES 
                ('11111111-1111-1111-1111-111111111111', 'John Doe', 'john.doe@example.com', '1234567890', '123456789', 3000.00, '2023-01-01'),
                ('22222222-2222-2222-2222-222222222222', 'Jane Smith', 'jane.smith@example.com', '0987654321', '987654321', 2500.00, '2023-02-01');

                -- Insert into RiskAnalyses
                INSERT INTO RiskAnalyses (Id, UserId, UnemploymentRate, InflationRate, CreditHistoryScore, OutstandingDebts, RiskLevel, AnalysisDate)
                VALUES 
                ('33333333-3333-3333-3333-333333333333', '11111111-1111-1111-1111-111111111111', 4.5, 2.1, 750.00, 1000.00, 1, '2023-03-01'),
                ('44444444-4444-4444-4444-444444444444', '22222222-2222-2222-2222-222222222222', 6.2, 3.0, 680.00, 1500.00, 1, '2023-03-15');

                -- Insert into CreditRequests
                INSERT INTO CreditRequests (Id, UserId, RiskAnalysisId, AmountRequested, TermInMonths, ApprovedAmount, RequestDate)
                VALUES 
                ('66666666-6666-6666-6666-666666666666', '22222222-2222-2222-2222-222222222222', '44444444-4444-4444-4444-444444444444', 3000.00, 24, 2800.00, '2023-04-15');
                ";

            command.ExecuteNonQuery();
        }

        public void SeedRiskAnalisysData()
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                -- Insert into Users
                INSERT INTO Users (Id, FullName, Email, PhoneNumber, Nif, MonthlyIncome, RegistrationDate)
                VALUES 
                ('11111111-1111-1111-1111-111111111111', 'John Doe', 'john.doe@example.com', '1234567890', '123456789', 3000.00, '2023-01-01'),
                ('22222222-2222-2222-2222-222222222222', 'Jane Smith', 'jane.smith@example.com', '0987654321', '987654321', 2500.00, '2023-02-01');

                -- Insert into RiskAnalyses
                INSERT INTO RiskAnalyses (Id, UserId, UnemploymentRate, InflationRate, CreditHistoryScore, OutstandingDebts, RiskLevel, AnalysisDate)
                VALUES 
                ('44444444-4444-4444-4444-444444444444', '22222222-2222-2222-2222-222222222222', 6.2, 3.0, 680.00, 1500.00, 1, '2023-03-15')";

            command.ExecuteNonQuery();
        }

        public void SeedUserData()
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                -- Insert into Users
                INSERT INTO Users (Id, FullName, Email, PhoneNumber, Nif, MonthlyIncome, RegistrationDate)
                VALUES 
                ('11111111-1111-1111-1111-111111111111', 'John Doe', 'john.doe@example.com', '1234567890', '123456789', 3000.00, '2023-01-01'),
                ('22222222-2222-2222-2222-222222222222', 'Jane Smith', 'jane.smith@example.com', '0987654321', '987654321', 2500.00, '2023-02-01');";

            command.ExecuteNonQuery();
        }
    }

}
