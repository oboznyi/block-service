using Npgsql;
using Dapper;
using System;
using Block.Processing.DTO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Block.Processing.Configuration;
using Block.Processing.Dao;

namespace Block.Processing.Providers
{
    public class BlockProviderPg
    {
        private readonly ILogger<BlockProviderPg> _logger;

        private readonly string _connectionString;

        public BlockProviderPg(ILogger<BlockProviderPg> logger, EnviromentConfiguration config)
        {
            _logger = logger;
            _connectionString = config.DBConnection;
        }

        public void InsertBlock(BlockInfoDao block)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                var createTableQuery = @"INSERT INTO  blockinfo(id, blocknumber, date) values(@id, @blocknumber, @date)";
                connection.Execute(createTableQuery, block);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "InsertBlock error");
                throw;
            }
        }
    }
}
