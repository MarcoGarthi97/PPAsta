using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Repository.Services.FactorySQL
{
    public interface IRepository<T> where T : class
    {

    }

    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly IDatabaseConnectionFactory _connectionFactory;

        public BaseRepository(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
    }

}
