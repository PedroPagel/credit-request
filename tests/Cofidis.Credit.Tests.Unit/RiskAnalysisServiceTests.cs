using Cofidis.Credit.Domain.Entities;
using Cofidis.Credit.Domain.Enums;
using Cofidis.Credit.Domain.Models.Risks;
using Cofidis.Credit.Domain.Options;
using Cofidis.Credit.Domain.Repositories;
using Cofidis.Credit.Domain.Services.Credits.Requests;
using Cofidis.Credit.Domain.Services.Notificator;
using Cofidis.Credit.Domain.Services.Risks.Analysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Cofidis.Credit.Tests.Unit
{
    public class RiskAnalysisServiceTests
    {
        private readonly Mock<ILogger<RiskAnalysisService>> _loggerMock = new();
        private readonly Mock<IOptions<RiskAnalysisOptions>> _optionsMock = new();
        private readonly Mock<IRiskAnalysisRepository> _repositoryMock = new();
        private readonly Mock<ICreditRequestService> _creditRequestServiceMock = new();
        private readonly Mock<INotificator> _notificatorMock = new();

        private readonly RiskAnalysisService _service;

        public RiskAnalysisServiceTests()
        {
            var options = new RiskAnalysisOptions
            {
                HighRiskThreshold = (int)0.8m,
                MediumRiskThreshold = (int)0.5m
            };

            _optionsMock.Setup(o => o.Value).Returns(options);

            _service = new RiskAnalysisService(
                _notificatorMock.Object,
                _loggerMock.Object,
                _optionsMock.Object,
                _repositoryMock.Object,
                _creditRequestServiceMock.Object);
        }

        [Fact]
        public void GetRiskLevel_ShouldReturnHigh_WhenScoreExceedsHighThreshold()
        {
            // Arrange
            var request = new RiskAnalysisRequest
            {
                UnemploymentRate = 0.5m,
                InflationRate = 0.3m,
                CreditHistoryScore = 30,
                OutstandingDebts = 0.2m
            };

            // Act
            var riskLevel = _service.GetRiskLevel(request);

            // Assert
            Assert.Equal(RiskLevel.High, riskLevel);
        }

        [Fact]
        public void GetRiskLevel_ShouldReturnLow_WhenScoreIsBelowMediumThreshold()
        {
            // Arrange
            var request = new RiskAnalysisRequest
            {
                UnemploymentRate = 0.1m,
                InflationRate = 0.05m,
                CreditHistoryScore = 90,
                OutstandingDebts = 0.05m
            };

            // Act
            var riskLevel = _service.GetRiskLevel(request);

            // Assert
            Assert.Equal(RiskLevel.Low, riskLevel);
        }

        [Fact]
        public async Task AddRiskAnalysis_ShouldAddRiskAnalysis_WhenValidRequest()
        {
            // Arrange
            var request = new RiskAnalysisRequest
            {
                UserId = Guid.NewGuid(),
                UnemploymentRate = 0.1m,
                InflationRate = 0.05m,
                CreditHistoryScore = 85,
                OutstandingDebts = 0.02m
            };

            _repositoryMock.Setup(r => r.Add(It.IsAny<RiskAnalysis>())).ReturnsAsync(true);

            // Act
            var result = await _service.AddRiskAnalysis(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.UserId, result.UserId);
            _repositoryMock.Verify(r => r.Add(It.IsAny<RiskAnalysis>()), Times.Once);
        }

        [Fact]
        public async Task AddRiskAnalysis_ShouldReturnNull_WhenRepositoryFailsToAdd()
        {
            // Arrange
            var request = new RiskAnalysisRequest
            {
                UserId = Guid.NewGuid(),
                UnemploymentRate = 0.1m,
                InflationRate = 0.05m,
                CreditHistoryScore = 85,
                OutstandingDebts = 0.02m
            };

            _repositoryMock.Setup(r => r.Add(It.IsAny<RiskAnalysis>())).ReturnsAsync(false);

            // Act
            var result = await _service.AddRiskAnalysis(request);

            // Assert
            Assert.Null(result);
            _notificatorMock.Verify(n => n.HandleError(It.Is<Notification>(n => n.Message == "It was not possible to add the risk analisys.")), Times.Once);
        }

        [Fact]
        public async Task DeleteRiskAnalysis_ShouldReturnFalse_WhenRiskNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetById(id)).ReturnsAsync((RiskAnalysis)null);

            // Act
            var result = await _service.DeleteRiskAnalisys(id);

            // Assert
            Assert.False(result);
            _notificatorMock.Verify(n => n.HandleError(It.Is<Notification>(n => n.Message == "The risk analisys was not found with the id.")), Times.Once);
        }

        [Fact]
        public async Task UpdateRiskAnalysis_ShouldReturnNull_WhenRiskNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new RiskAnalysisRequest();
            _repositoryMock.Setup(r => r.GetById(id)).ReturnsAsync((RiskAnalysis)null);

            // Act
            var result = await _service.UpdateRiskAnalysis(id, request);

            // Assert
            Assert.Null(result);
            _notificatorMock.Verify(n => n.HandleError(It.Is<Notification>(n => n.Message == "Risk not found")), Times.Once);
        }

        [Fact]
        public async Task UpdateRiskAnalysis_ShouldUpdateRiskSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new RiskAnalysisRequest
            {
                UnemploymentRate = 0.2m,
                InflationRate = 0.1m,
                CreditHistoryScore = 75,
                OutstandingDebts = 0.3m
            };
            var existingRisk = new RiskAnalysis { Id = id, UserId = Guid.NewGuid() };

            _repositoryMock.Setup(r => r.GetById(id)).ReturnsAsync(existingRisk);
            _repositoryMock.Setup(r => r.Update(It.IsAny<RiskAnalysis>())).ReturnsAsync(true);

            // Act
            var result = await _service.UpdateRiskAnalysis(id, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.UnemploymentRate, result.UnemploymentRate);
            _repositoryMock.Verify(r => r.Update(It.IsAny<RiskAnalysis>()), Times.Once);
            _notificatorMock.Verify(n => n.HandleError(It.IsAny<Notification>()), Times.Never); 
        }

        [Fact]
        public async Task GetRiskAnalysisById_ShouldReturnNull_WhenIdIsInvalid()
        {
            // Arrange
            var id = Guid.Empty;

            // Act
            var result = await _service.GetRiskAnalisysById(id);

            // Assert
            Assert.Null(result);
            _notificatorMock.Verify(n => n.HandleError(It.Is<Notification>(n => n.Message == "Id is invalid!")), Times.Once);
        }

        [Fact]
        public async Task GetRiskAnalysisById_ShouldReturnRiskAnalysis_WhenFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedRisk = new RiskAnalysis { Id = id };
            _repositoryMock.Setup(r => r.GetById(id)).ReturnsAsync(expectedRisk);

            // Act
            var result = await _service.GetRiskAnalisysById(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedRisk.Id, result.Id);
            _notificatorMock.Verify(n => n.HandleError(It.IsAny<Notification>()), Times.Never); 
        }
    }
}