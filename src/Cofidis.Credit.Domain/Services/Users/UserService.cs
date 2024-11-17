using Cofidis.Credit.Domain.Entities;
using Cofidis.Credit.Domain.Models.Users;
using Cofidis.Credit.Domain.Repositories;
using Cofidis.Credit.Domain.Services.Credits.Requests;
using Cofidis.Credit.Domain.Services.DigitalMobileKey;
using Cofidis.Credit.Domain.Services.Notificator;
using Cofidis.Credit.Domain.Services.Risks.Analysis;
using Cofidis.Credit.Domain.Services.Validations;
using Microsoft.Extensions.Logging;

namespace Cofidis.Credit.Domain.Services.Users
{
    public class UserService(IUserRepository userRepository, 
                             ILogger<UserService> logger, 
                             INotificator notificator, 
                             IDigitalMobileKeyService digitalMobileKeyService,
                             IRiskAnalysisService riskAnalysisService,
                             ICreditRequestService creditRequestService) : BaseService(notificator), IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ILogger<UserService> _logger = logger;
        private readonly IDigitalMobileKeyService _digitalMobileKeyService = digitalMobileKeyService;
        private readonly IRiskAnalysisService _riskAnalysisService = riskAnalysisService;
        private readonly ICreditRequestService _creditRequestService = creditRequestService;

        public async Task<User> GetUserById(Guid id)
        {
            _logger.LogInformation("GetUserByIdAsync with {id}", id);

            if (id == Guid.Empty)
            {
                NotifyError("Id is invalid!");
                return null;
            }

            var user = await _userRepository.GetById(id);

            _logger.LogInformation(user != null ? "User found for ID: {Id}" : "No user found for ID: {Id}", id);
            return user;
        }

        public async Task<User> GetUserByNif(string nif)
        {
            _logger.LogInformation("GetUserByNif with {nif}", nif);

            if (string.IsNullOrWhiteSpace(nif))
            {
                NotifyError("Nif is empty!");
                return null;
            }

            var user = await _userRepository.FirstOrDefault(user => user.Nif.Equals(nif));

            _logger.LogInformation(user == null ? "No user found for NIF: {Nif}" : "User found for NIF: {Nif}", nif);
            return user;
        }

        public async Task<User> AddUser(UserRequest request)
        {
            _logger.LogInformation("Attempting to add user with NIF: {Nif}", request.Nif);

            if (!Validate(new UserRequestValidation(), request))
                return null;

            var user = await _userRepository.FirstOrDefault(user => user.Nif.Equals(request.Nif));

            if (user != null) 
            {
                NotifyError("User not found or dont exist.");
                return null;
            }

            user = new()
            {
                Email = request.Email,
                FullName = request.FullName,
                Nif = request.Nif,
                PhoneNumber = request.PhoneNumber,
                RegistrationDate = DateTime.UtcNow,
                MonthlyIncome = request.MonthlyIncome
            };

            if (!await _userRepository.Add(user))
            {
                NotifyError("It was not possible to add the user.");
                return null;
            }

            _logger.LogInformation("User successfully added with NIF: {Nif}", request.Nif);
            return await _userRepository.FirstOrDefault(user => user.Nif.Equals(request.Nif));
        }

        public async Task<User> AddUserByDigitalKey(string nif, decimal monthlyIncome)
        {
            _logger.LogInformation("Attempting to add user by digital key with NIF: {Nif}", nif);

            if (string.IsNullOrWhiteSpace(nif))
            {
                NotifyError("Nif is empty!");
                return null;
            }

            var user = await _digitalMobileKeyService.GetUserInfoByNIF(nif);
            _logger.LogInformation("User information fetched from digital key for NIF: {Nif}", nif);

            return await AddUser(new UserRequest()
            {
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Nif = nif,
                MonthlyIncome = monthlyIncome
            });
        }

        public async Task<User> UpdateUser(Guid userId, UserRequest request)
        {
            _logger.LogInformation("Updating user with ID: {UserId}", userId);

            if (userId == Guid.Empty)
            {
                NotifyError("Id is invalid!");
                return null;
            }

            if (!Validate(new UserRequestValidation(), request))
                return null;

            var user = await _userRepository.GetById(userId);

            if (user is null)
            {
                NotifyError("User not found or dont exist.");
                return null;
            }

            user.Email = request.Email;
            user.FullName = request.FullName;
            user.Nif = request.Nif;
            user.PhoneNumber = request.PhoneNumber;
            user.RegistrationDate = DateTime.UtcNow;
            user.MonthlyIncome = request.MonthlyIncome;

            if (!await _userRepository.Update(user))
            {
                NotifyError("It was not possible to update the user.");
                return null;
            }

            _logger.LogInformation("Successfully updated user with ID: {UserId}", userId);
            return user;
        }

        public async Task<bool> DeleteUser(Guid id)
        {
            _logger.LogInformation("Deleting user with ID: {Id}", id);

            if (id == Guid.Empty)
            {
                NotifyError("Id is invalid!");
                return false;
            }

            var credit = await _creditRequestService.GetCreditRequestByUserId(id);

            if (credit is not null)
            {
                NotifyError("The user has a credit request, the user will not be deleted");
                return false;
            }

            var risk = await _riskAnalysisService.GetRiskAnalisysById(id);

            if (risk is not null)
            {
                NotifyError("The user has a risk analisys, the user will not be deleted");
                return false;
            }

            var result = await _userRepository.Delete(us => us.Id == id);

            if (result == 0)
            {
                NotifyError("It was not possible to remove the user.");
                return false;
            }

            _logger.LogInformation("Successfully deleted user with ID: {Id}", id);
            return true;
        }
    }
}
