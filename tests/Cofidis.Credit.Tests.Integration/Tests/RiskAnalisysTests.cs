using Cofidis.Credit.Api.Dto;
using Cofidis.Credit.Domain.Models.Risks;
using Xunit;

namespace Cofidis.Credit.Tests.Integration.Tests
{
    public class RiskAnalisysTests : ServerFixture, IClassFixture<DatabaseTestFixture>
    {
        private readonly DatabaseTestFixture _fixture;

        public RiskAnalisysTests(DatabaseTestFixture fixture)
        {
            _fixture = fixture;
            _fixture.CleanDatabase();
            _fixture.SeedRiskAnalisysData();

        }

        [Theory(DisplayName = "Add risk analisys")]
        [InlineData("risk-analisys/add-risk")]
        public async Task AddRiskAnalisys(string url)
        {
            var request = new RiskAnalysisRequest()
            {
                CreditHistoryScore = 100,
                InflationRate = 2,
                OutstandingDebts = 0,
                UnemploymentRate = 1,
                UserId = new("11111111-1111-1111-1111-111111111111")
            };

            // Act
            var response = await PostAsync<RiskAnalysisDto>(url, request);

            // Assert
            Assert.NotNull(response);
        }

        [Theory(DisplayName = "Update by user id")]
        [InlineData("risk-analisys/update-risk/44444444-4444-4444-4444-444444444444")]
        public async Task UpdateRiskAnalisys(string url)
        {
            var request = new RiskAnalysisRequest()
            {
                CreditHistoryScore = 90,
                InflationRate = 2,
                OutstandingDebts = 1,
                UnemploymentRate = 1,
                UserId = new("22222222-2222-2222-2222-222222222222")
            };

            // Act
            var response = await PatchAsync<RiskAnalysisDto>(url, request);

            // Assert
            Assert.NotNull(response);
        }

        [Theory(DisplayName = "Delete risk analisys")]
        [InlineData("risk-analisys/delete-risk-analisys/44444444-4444-4444-4444-444444444444")]
        public async Task DeleteRiskAnalisys(string url)
        {
            // Act
            var response = await DeleteAsync<bool>(url);

            // Assert
            Assert.True(response);
        }
    }
}
