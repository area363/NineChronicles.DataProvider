namespace NineChronicles.DataProvider.Store
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Options;
    using MySqlConnector;
    using NineChronicles.DataProvider.Store.Models;
    using Serilog;
    using SqlKata.Compilers;
    using SqlKata.Execution;

    public class MySqlStore
    {
        private const string AgentsDbName = "agent";
        private const string AvatarsDbName = "avatar";
        private const string HackAndSlashDbName = "hack_and_slash";

        private readonly MySqlCompiler _compiler;
        private readonly string _connectionString;

        public MySqlStore(IOptions<Configuration> option)
            : this(option.Value.MySqlConnectionString)
        {
        }

        public MySqlStore(string connectionString)
        {
            _connectionString = connectionString;
            _compiler = new MySqlCompiler();
        }

        public void StoreAvatar(
            string address,
            string agentAddress)
        {
            Insert(AvatarsDbName, new Dictionary<string, object>
            {
                ["address"] = address,
                ["agent_address"] = agentAddress,
            });
        }

        public void StoreAgent(string address)
        {
            Insert(AgentsDbName, new Dictionary<string, object>
            {
                ["address"] = address,
            });
        }

        public void StoreHackAndSlash(
            string agentAddress,
            string avatarAddress,
            int stageId,
            bool cleared,
            string blockHash)
        {
            Insert(HackAndSlashDbName, new Dictionary<string, object>
            {
                ["agent_address"] = agentAddress,
                ["avatar_address"] = avatarAddress,
                ["stage_id"] = stageId,
                ["cleared"] = cleared,
                ["block_hash"] = blockHash,
            });
        }

        public IEnumerable<HackAndSlashModel> GetHackAndSlash(
            string? agentAddress = null,
            int? limit = null)
        {
            using QueryFactory db = OpenDb();
            var query = db.Query(HackAndSlashDbName);
            if (agentAddress is { })
            {
                query = query.Where("agent_address", agentAddress);
            }

            if (limit is int limitNotNull)
            {
                query = query.Limit(limitNotNull);
            }

            return query.Get<HackAndSlashModel>();
        }

        private QueryFactory OpenDb() =>
            new QueryFactory(new MySqlConnection(_connectionString), _compiler);

        private void Insert(string tableName, IReadOnlyDictionary<string, object> data)
        {
            using QueryFactory db = OpenDb();
            try
            {
                db.Query(tableName).Insert(data);
            }
            catch (MySqlException e)
            {
                Log.Debug(e.ErrorCode.ToString());
                if (e.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                {
                    Log.Debug("Ignore DuplicateKeyEntry");
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
