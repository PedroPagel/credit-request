using Cofidis.Credit.Domain.Entities;
using Cofidis.Credit.Domain.Enums;
using Cofidis.Credit.Domain.Models.Credits;
using Cofidis.Credit.Domain.Repositories;
using Cofidis.Credit.Domain.Services.Credits.Requests;
using Cofidis.Credit.Domain.Services.Notificator;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Cofidis.Credit.Tests.Unit
{
    public class CreditServiceTests
    {
        private readonly Mock<ICreditRequestRepository> _creditRequestRepositoryMock = new();
        private readonly Mock<IRiskAnalysisRepository> _riskAnalysisRepositoryMock = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<ILogger<CreditRequestService>> _loggerMock = new();
        private readonly Mock<INotificator> _notificatorMock = new();

        private readonly CreditRequestService _service;

        public CreditServiceTests()
        {
            _service = new CreditRequestService(
                _notificatorMock.Object,
                _loggerMock.Object,
                _riskAnalysisRepositoryMock.Object,
                _creditRequestRepositoryMock.Object,
                _userRepositoryMock.Object);
        }

        [Fact]
        public async Task DeleteCreditRequest_ShouldReturnFalse_WhenIdIsInvalid()
        {
            // Arrange
            var invalidId = Guid.Empty;

            // Act
            var result = await _service.DeleteCreditRequest(invalidId);

            // Assert
            Assert.False(result);
            _notificatorMock.Verify(n => n.HandleError(It.Is<Notification>(n => n.Message == "Id is invalid!")), Times.Once);
        }

        [Fact]
        public async Task DeleteCreditRequest_ShouldReturnTrue_WhenSuccessfullyDeleted()
        {
            // Arrange
            var validId = Guid.NewGuid();
            var credit = new CreditRequest { Id = validId };
            _creditRequestRepositoryMock.Setup(r => r.GetById(validId)).ReturnsAsync(credit);
            _creditRequestRepositoryMock.Setup(r => r.Delete(It.IsAny<Expression<Func<CreditRequest, bool>>>()))
                            .ReturnsAsync(1);


            // Act
            var result = await _service.DeleteCreditRequest(validId);

            // Assert
            Assert.True(result);
            _notificatorMock.Verify(n => n.HandleError(It.IsAny<Notification>()), Times.Never);
        }

        [Fact]
        public async Task ProcessCreditRequest_ShouldReturnNull_WhenRequestIsInvalid()
        {
            // Arrange
            var invalidRequest = new CreditRequested { AmountRequested = -1, TermInMonths = 0 };

            // Act
            var result = await _service.ProcessCreditRequest(invalidRequest);

            // Assert
            Assert.Null(result);
            _notificatorMock.Verify(n => n.HandleError(It.Is<Notification>(n => n.Message == "The requested values are incorrect")), Times.Once);
        }

        [Fact]
        public async Task ProcessCreditRequest_ShouldReturnNull_WhenUserNotFound()
        {
            // Arrange
            var request = new CreditRequested { UserId = Guid.NewGuid(), AmountRequested = 1000, TermInMonths = 12 };
            _userRepositoryMock.Setup(r => r.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync((User)null);

            // Act
            var result = await _service.ProcessCreditRequest(request);

            // Assert
            Assert.Null(result);
            _notificatorMock.Verify(n => n.HandleError(It.Is<Notification>(n => n.Message == "User not found.")), Times.Once);
        }

        [Fact]
        public async Task ProcessCreditRequest_ShouldReturnNull_WhenRiskAnalysisMissing()
        {
            // Arrange
            var request = new CreditRequested { UserId = Guid.NewGuid(), AmountRequested = 1000, TermInMonths = 12 };
            _userRepositoryMock.Setup(r => r.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(new User());
            _riskAnalysisRepositoryMock.Setup(r => r.FirstOrDefault(It.IsAny<Expression<Func<RiskAnalysis, bool>>>())).ReturnsAsync((RiskAnalysis)null);

            // Act
            var result = await _service.ProcessCreditRequest(request);

            // Assert
            Assert.Null(result);
            _notificatorMock.Verify(n => n.HandleError(It.Is<Notification>(n => n.Message == "Risk analysis is required before processing a credit request.")), Times.Once);
        }

        [Fact]
        public async Task ProcessCreditRequest_ShouldReturnNull_WhenRiskIsHigh()
        {
            // Arrange
            var request = new CreditRequested { UserId = Guid.NewGuid(), AmountRequested = 1000, TermInMonths = 12 };
            var riskAnalysis = new RiskAnalysis { RiskLevel = RiskLevel.High };

            _userRepositoryMock.Setup(r => r.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(new User());
            _riskAnalysisRepositoryMock.Setup(r => r.FirstOrDefault(It.IsAny<Expression<Func<RiskAnalysis, bool>>>())).ReturnsAsync(riskAnalysis);

            // Act
            var result = await _service.ProcessCreditRequest(request);

            // Assert
            Assert.Null(result);
            _notificatorMock.Verify(n => n.HandleError(It.Is<Notification>(n => n.Message == "Credit cannot be granted for high-risk analyses.")), Times.Once);
        }

        [Fact]
        public async Task ProcessCreditRequest_ShouldProcess_WhenAllChecksPass()
        {
            // Arrange
            var request = new CreditRequested { UserId = Guid.NewGuid(), AmountRequested = 1000, TermInMonths = 12 };
            var user = new User { Id = request.UserId, MonthlyIncome = 5000 };
            var riskAnalysis = new RiskAnalysis { RiskLevel = RiskLevel.Low };

            _userRepositoryMock.Setup(r => r.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(user);
            _riskAnalysisRepositoryMock.Setup(r => r.FirstOrDefault(It.IsAny<Expression<Func<RiskAnalysis, bool>>>())).ReturnsAsync(riskAnalysis);
            _creditRequestRepositoryMock.Setup(r => r.Add(It.IsAny<CreditRequest>())).ReturnsAsync(true);

            // Act
            var result = await _service.ProcessCreditRequest(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.AmountRequested, result.AmountRequested);
            _notificatorMock.Verify(n => n.HandleError(It.IsAny<Notification>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCredit_ShouldReturnNull_WhenCreditNotFound()
        {
            // Arrange
            var creditId = Guid.NewGuid();
            var request = new CreditRequested { AmountRequested = 1000, TermInMonths = 12 };
            _creditRequestRepositoryMock.Setup(r => r.GetById(creditId)).ReturnsAsync((CreditRequest)null);

            // Act
            var result = await _service.UpdateCredit(creditId, request);

            // Assert
            Assert.Null(result);
            _notificatorMock.Verify(n => n.HandleError(It.Is<Notification>(n => n.Message == "Credit not found")), Times.Once);
        }

        [Fact]
        public async Task UpdateCredit_ShouldUpdate_WhenAllChecksPass()
        {
            // Arrange
            var creditId = Guid.NewGuid();
            var request = new CreditRequested { UserId = Guid.NewGuid(), AmountRequested = 1000, TermInMonths = 12 };
            var credit = new CreditRequest { Id = creditId };
            var user = new User { Id = request.UserId, MonthlyIncome = 5000 };
            var riskAnalysis = new RiskAnalysis { RiskLevel = RiskLevel.Low };

            _creditRequestRepositoryMock.Setup(r => r.GetById(creditId)).ReturnsAsync(credit);
            _userRepositoryMock.Setup(r => r.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(user);
            _riskAnalysisRepositoryMock.Setup(r => r.FirstOrDefault(It.IsAny<Expression<Func<RiskAnalysis, bool>>>())).ReturnsAsync(riskAnalysis);
            _creditRequestRepositoryMock.Setup(r => r.Update(It.IsAny<CreditRequest>())).ReturnsAsync(true);

            // Act
            var result = await _service.UpdateCredit(creditId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.AmountRequested, result.AmountRequested);
            _notificatorMock.Verify(n => n.HandleError(It.IsAny<Notification>()), Times.Never);
        }
    }
}