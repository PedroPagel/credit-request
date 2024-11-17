using Cofidis.Credit.Api.Dto;
using Cofidis.Credit.Domain.Models.Users;
using Xunit;

namespace Cofidis.Credit.Tests.Integration.Tests
{
    public class UserTests : ServerFixture, IClassFixture<DatabaseTestFixture>
    {
        private readonly DatabaseTestFixture _fixture;

        public UserTests(DatabaseTestFixture fixture)
        {
            _fixture = fixture;
            _fixture.CleanDatabase();
            _fixture.SeedUserData();
        }

        [Theory(DisplayName = "Add user")]
        [InlineData("user/add-user")]
        public async Task AddUser(string url)
        {
            var request = new UserRequest()
            {
                Email = "test@gmai.com",
                FullName = "Codfidis Test",
                MonthlyIncome = 2000,
                Nif = "319666999",
                PhoneNumber = "917689653"
            };

            // Act
            var response = await PostAsync<UserDto>(url, request);

            // Assert
            Assert.NotNull(response);
        }

        [Theory(DisplayName = "Get user")]
        [InlineData("user/11111111-1111-1111-1111-111111111111")]
        public async Task GetUser(string url)
        {
            // Act
            var response = await GetAsync<UserDto>(url);

            // Assert
            Assert.NotNull(response);
        }

        [Theory(DisplayName = "Delete user")]
        [InlineData("user/delete-user/22222222-2222-2222-2222-222222222222")]
        public async Task DeleteUser(string url)
        {
            // Act
            var response = await DeleteAsync<bool>(url);

            // Assert
            Assert.True(response);
        }
    }
}
