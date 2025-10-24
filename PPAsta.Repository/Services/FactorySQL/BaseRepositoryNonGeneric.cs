namespace PPAsta.Repository.Services.FactorySQL
{
    public abstract class BaseRepositoryNonGeneric
    {
        protected readonly IDatabaseConnectionFactory _connectionFactory;

        protected BaseRepositoryNonGeneric(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
    }

}
