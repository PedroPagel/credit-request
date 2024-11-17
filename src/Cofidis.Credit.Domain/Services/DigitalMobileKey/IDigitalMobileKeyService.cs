using Cofidis.Credit.Domain.Models.DigitalMobileKey;

namespace Cofidis.Credit.Domain.Services.DigitalMobileKey
{
    public interface IDigitalMobileKeyService
    {
        Task<UserInfo> GetUserInfoByNIF(string nif);
    }

}
