using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Data
{
    public class ServiceDbContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ServiceDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}