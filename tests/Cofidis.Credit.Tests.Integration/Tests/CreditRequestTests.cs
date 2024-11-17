using Cofidis.Credit.Api.Dto;
using Cofidis.Credit.Domain.Models.Credits;
using Xunit;

namespace Cofidis.Credit.Tests.Integration.Tests
{
    public class CreditRequestTests : ServerFixture, IClassFixture<DatabaseTestFixture>
    {
        private readonly DatabaseTestFixture _fixture;

        public CreditRequestTests(DatabaseTestFixture fixture)
        {
            _fixture = fixture;
            _fixture.CleanDatabase();
            _fixture.SeedCrediRequestData();
        }

        [Theory(DisplayName = "Add Credit request")]
        [InlineData("credit-request/add-credit")]
        public async Task ProcessCreditRequest(string url)
        {
            var request = new CreditRequested()
            {
                AmountRequested = 5000,
                TermInMonths = 12,
                UserId = new("11111111-1111-1111-1111-111111111111")
            };

            // Act
            var response = await PostAsync<CreditRequestDto>(url, request);

            // Assert
            Assert.NotNull(response);
        }

        [Theory(DisplayName = "Credit request by user id")]
        [InlineData("credit-request/by-user/22222222-2222-2222-2222-222222222222")]
        public async Task GetCreditRequestsByUserId(string url)
        {
            // Act
            var response = await GetAsync<CreditRequestDto>(url);

            // Assert
            Assert.NotNull(response);
        }

        [Theory(DisplayName = "Delete Credit request")]
        [InlineData("credit-request/delete-credit/66666666-6666-6666-6666-666666666666")]
        public async Task DeleteCreditRequest(string url)
        {
            // Act
            var response = await DeleteAsync<bool>(url);

            // Assert
            Assert.True(response);
        }
    }
}
