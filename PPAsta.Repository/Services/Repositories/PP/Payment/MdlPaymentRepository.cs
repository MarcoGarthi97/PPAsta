using Dapper.Contrib.Extensions;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Models.Entities.Payment;
using PPAsta.Repository.Services.FactorySQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Dapper.Plus;

namespace PPAsta.Repository.Services.Repositories.PP.Payment
{
    public interface IMdlPaymentRepository : IForServiceCollectionExtension
    {
        Task DeletePaymentAsync(MdlPayment Payment);
        Task<IEnumerable<MdlPayment>> GetAllPaymentsAsync();
        Task InsertPaymentAsync(MdlPayment Payment);
        Task InsertPaymentsAsync(IEnumerable<MdlPayment> Payments);
        Task UpdatePaymentAsync(MdlPayment Payment);
    }

    public class MdlPaymentRepository : BaseRepository<MdlPayment>, IMdlPaymentRepository
    {
        public MdlPaymentRepository(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<IEnumerable<MdlPayment>> GetAllPaymentsAsync()
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            return await connection.GetAllAsync<MdlPayment>();
        }

        public async Task InsertPaymentAsync(MdlPayment Payment)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.InsertAsync(Payment);
        }

        public async Task InsertPaymentsAsync(IEnumerable<MdlPayment> Payments)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.BulkInsertAsync(Payments);
        }

        public async Task UpdatePaymentAsync(MdlPayment Payment)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.UpdateAsync(Payment);
        }

        public async Task DeletePaymentAsync(MdlPayment Payment)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.DeleteAsync(Payment);
        }
    }
}
