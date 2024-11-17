using Cofidis.Credit.Domain.Entities;
using Cofidis.Credit.Domain.Models.Users;
using Cofidis.Credit.Domain.Repositories;
using Cofidis.Credit.Domain.Services.Credits.Requests;
using Cofidis.Credit.Domain.Services.DigitalMobileKey;
using Cofidis.Credit.Domain.Services.Notificator;
using Cofidis.Credit.Domain.Services.Risks.Analysis;
using Cofidis.Credit.Domain.Services.Users;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Cofidis.Credit.Tests.Unit
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<ILogger<UserService>> _loggerMock = new();
        private readonly Mock<INotificator> _notificatorMock = new();
        private readonly Mock<IDigitalMobileKeyService> _digitalMobileKeyServiceMock = new();
        private readonly Mock<IRiskAnalysisService> _riskAnalysisServiceMock = new();
        private readonly Mock<ICreditRequestService> _creditRequestServiceMock = new();

        private readonly UserService _service;

        public UserServiceTests()
        {
            _service = new UserService(
                _userRepositoryMock.Object,
                _loggerMock.Object,
                _notificatorMock.Object,
                _digitalMobileKeyServiceMock.Object,
                _riskAnalysisServiceMock.Object,
                _creditRequestServiceMock.Object);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnNull_WhenIdIsInvalid()
        {
            // Arrange
            var invalidId = Guid.Empty;

            // Act
            var result = await _service.GetUserById(invalidId);

            // Assert
            Assert.Null(result);
            _notificatorMock.Verify(n => n.HandleError(It.Is<Notification>(n => n.Message == "Id is invalid!")), Times.Once);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUser_WhenFound()
        {
            // Arrange
            var validId = Guid.NewGuid();
            var user = new User { Id = validId };
            _userRepositoryMock.Setup(r => r.GetById(validId)).ReturnsAsync(user);

            // Act
            var result = await _service.GetUserById(validId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            _notificatorMock.Verify(n => n.HandleError(It.IsAny<Notification>()), Times.Never);
        }

        [Fact]
        public async Task GetUserByNif_ShouldReturnNull_WhenNifIsInvalid()
        {
            // Arrange
            var invalidNif = "";

            // Act
            var result = await _service.GetUserByNif(invalidNif);

            // Assert
            Assert.Null(result);
            _notificatorMock.Verify(n => n.HandleError(It.Is<Notification>(n => n.Message == "Nif is empty!")), Times.Once);
        }

        [Fact]
        public async Task AddUser_ShouldReturnNull_WhenValidationFails()
        {
            // Arrange
            var request = new UserRequest { Nif = "" }; 
            _notificatorMock.Setup(n => n.HandleError(It.IsAny<Notification>()));

            // Act
            var result = await _service.AddUser(request);

            // Assert
            Assert.Null(result);
            _notificatorMock.Verify(n => n.HandleError(It.Is<Notification>(n => n.Message == "Nif cannot be empty")), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnFalse_WhenIdIsInvalid()
        {
            // Arrange
            var invalidId = Guid.Empty;

            // Act
            var result = await _service.DeleteUser(invalidId);

            // Assert
            Assert.False(result);
            _notificatorMock.Verify(n => n.HandleError(It.Is<Notification>(n => n.Message == "Id is invalid!")), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnFalse_WhenUserHasCredit()
        {
            // Arrange
            var validId = Guid.NewGuid();
            _creditRequestServiceMock.Setup(c => c.GetCreditRequestByUserId(validId)).ReturnsAsync(new CreditRequest());

            // Act
            var result = await _service.DeleteUser(validId);

            // Assert
            Assert.False(result);
            _notificatorMock.Verify(n => n.HandleError(It.Is<Notification>(n => n.Message == "The user has a credit request, the user will not be deleted")), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnFalse_WhenUserHasRiskAnalysis()
        {
            // Arrange
            var validId = Guid.NewGuid();
            _riskAnalysisServiceMock.Setup(r => r.GetRiskAnalisysById(validId)).ReturnsAsync(new RiskAnalysis());

            // Act
            var result = await _service.DeleteUser(validId);

            // Assert
            Assert.False(result);
            _notificatorMock.Verify(n => n.HandleError(It.Is<Notification>(n => n.Message == "The user has a risk analisys, the user will not be deleted")), Times.Once);
        }
    }
}
