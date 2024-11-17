using Cofidis.Credit.Domain.Entities;
using Cofidis.Credit.Domain.Repositories;
using Cofidis.Credit.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Cofidis.Credit.Infrastructure.Repositories
{
    public class CreditRequestRepository(CreditDbContext db) : Repository<CreditRequest>(db), ICreditRequestRepository
    {
        public async Task<int> GetCreditLimitByIncome(decimal monthlyIncome)
        {
            var creditLimitOutput = new SqlParameter
            {
                ParameterName = "@CreditLimit",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output
            };

            var monthlyIncomeInput = new SqlParameter
            {
                ParameterName = "@MonthlyIncome",
                Value = monthlyIncome,
                SqlDbType = System.Data.SqlDbType.Decimal,
                Precision = 18,
                Scale = 2
            };

            await Db.Database.ExecuteSqlRawAsync(
                "EXEC sp_GetCreditLimitByIncome @MonthlyIncome, @CreditLimit OUTPUT",
                monthlyIncomeInput,
                creditLimitOutput
            );

            return (int)creditLimitOutput.Value;
        }
    }
}
