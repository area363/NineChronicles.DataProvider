using Bencodex.Types;
using Lib9c.DevExtensions.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NineChronicles.DataProvider.DataRendering;
using NineChronicles.DataProvider.Store;
using NineChronicles.DataProvider.Store.Models;

namespace NineChronicles.DataProvider.Tools.SubCommand
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Cocona;
    using Libplanet;
    using Libplanet.Action;
    using Libplanet.Blockchain;
    using Libplanet.Blockchain.Policies;
    using Libplanet.Blocks;
    using Libplanet.RocksDBStore;
    using Libplanet.Store;
    using MySqlConnector;
    using Nekoyume.Action;
    using Nekoyume.BlockChain.Policy;
    using Nekoyume.BlockChain;
    using Nekoyume.Model.State;
    using Serilog;
    using Serilog.Events;
    using NCAction = Libplanet.Action.PolymorphicAction<Nekoyume.Action.ActionBase>;

    public class MySqlMigration
    {
        private const string AgentTable = "Agents";
        private const string AvatarTable = "Avatars";
        private const string HasTable = "HackAndSlashes";
        private string _connectionString;
        private IStore _baseStore;
        private BlockChain<NCAction> _baseChain;
        private StreamWriter _agentBulkFile;
        private StreamWriter _avatarBulkFile;
        private StreamWriter _hasBulkFile;
        private List<string> _agentList;
        private List<string> _avatarList;
        private List<string> _agentFiles;
        private List<string> _avatarFiles;
        private List<string> _hasFiles;
        private IDbContextFactory<NineChroniclesContext> _dbContextFactory;
        private MySqlStore mySqlStore;
        protected const string ConnectionStringFormat = "server=localhost;database={0};uid=root;port=3306;";
        protected NineChroniclesContext Context;
        protected ServiceCollection Services;

        [Command(Description = "Migrate action data in rocksdb store to mysql db.")]
        public void Migration(
            [Option('o', Description = "Rocksdb path to migrate.")]
            string storePath,
            [Option(
                "rocksdb-storetype",
                Description = "Store type of RocksDb (new or mono).")]
            string rocksdbStoreType,
            [Option(
                "mysql-server",
                Description = "A hostname of MySQL server.")]
            string mysqlServer,
            [Option(
                "mysql-port",
                Description = "A port of MySQL server.")]
            uint mysqlPort,
            [Option(
                "mysql-username",
                Description = "The name of MySQL user.")]
            string mysqlUsername,
            [Option(
                "mysql-password",
                Description = "The password of MySQL user.")]
            string mysqlPassword,
            [Option(
                "mysql-database",
                Description = "The name of MySQL database to use.")]
            string mysqlDatabase,
            [Option(
                "offset",
                Description = "offset of block index (no entry will migrate from the genesis block).")]
            int? offset = null,
            [Option(
                "limit",
                Description = "limit of block count (no entry will migrate to the chain tip).")]
            int? limit = null
        )
        {
            DateTimeOffset start = DateTimeOffset.UtcNow;
            var builder = new MySqlConnectionStringBuilder
            {
                Database = mysqlDatabase,
                UserID = mysqlUsername,
                Password = mysqlPassword,
                Server = mysqlServer,
                Port = mysqlPort,
                AllowLoadLocalInfile = true,
            };

            _connectionString = builder.ConnectionString;

            var connectionString = _connectionString;

            var services = new ServiceCollection();
            services.AddDbContextFactory<NineChroniclesContext>(options =>
            {
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(
                        connectionString),
                    b => b.MigrationsAssembly("NineChronicles.DataProvider.Executable"));
            });
            services.AddSingleton<MySqlStore>();
            var dbContextOptions =
                new DbContextOptionsBuilder<NineChroniclesContext>()
                    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)).Options;
            Context = new NineChroniclesContext(dbContextOptions);
            var serviceCollection = new ServiceCollection();
            IServiceProvider provider = serviceCollection.BuildServiceProvider();
            _dbContextFactory = new DbContextFactory<NineChroniclesContext>(
                provider,
                dbContextOptions,
                new DbContextFactorySource<NineChroniclesContext>());
            mySqlStore = new MySqlStore(_dbContextFactory);

            Console.WriteLine("Setting up RocksDBStore...");
            if (rocksdbStoreType == "new")
            {
                _baseStore = new RocksDBStore(
                    storePath,
                    dbConnectionCacheSize: 10000);
            }
            else
            {
                throw new CommandExitedException("Invalid rocksdb-storetype. Please enter 'new' or 'mono'", -1);
            }

            long totalLength = _baseStore.CountBlocks();

            if (totalLength == 0)
            {
                throw new CommandExitedException("Invalid rocksdb-store. Please enter a valid store path", -1);
            }

            if (!(_baseStore.GetCanonicalChainId() is Guid chainId))
            {
                Console.Error.WriteLine("There is no canonical chain: {0}", storePath);
                Environment.Exit(1);
                return;
            }

            if (!(_baseStore.IndexBlockHash(chainId, 0) is { } gHash))
            {
                Console.Error.WriteLine("There is no genesis block: {0}", storePath);
                Environment.Exit(1);
                return;
            }

            // Setup base store
            RocksDBKeyValueStore baseStateKeyValueStore = new RocksDBKeyValueStore(Path.Combine(storePath, "states"));
            TrieStateStore baseStateStore =
                new TrieStateStore(baseStateKeyValueStore);

            // Setup block policy
            IStagePolicy<NCAction> stagePolicy = new VolatileStagePolicy<NCAction>();
            LogEventLevel logLevel = LogEventLevel.Debug;
            var blockPolicySource = new BlockPolicySource(Log.Logger, logLevel);
            IBlockPolicy<NCAction> blockPolicy = blockPolicySource.GetPolicy();

            // Setup base chain & new chain
            Block<NCAction> genesis = _baseStore.GetBlock<NCAction>(gHash);
            _baseChain = new BlockChain<NCAction>(blockPolicy, stagePolicy, _baseStore, baseStateStore, genesis);

            // Prepare block hashes to append to new chain
            long height = _baseChain.Tip.Index;
            if (offset + limit > (int)height)
            {
                Console.Error.WriteLine(
                    "The sum of the offset and limit is greater than the chain tip index: {0}",
                    height);
                Environment.Exit(1);
                return;
            }

            Console.WriteLine("Start migration.");

            // files to store bulk file paths (new file created every 10000 blocks for bulk load performance)
            _agentFiles = new List<string>();
            _avatarFiles = new List<string>();
            _hasFiles = new List<string>();

            // lists to keep track of inserted addresses to minimize duplicates
            _agentList = new List<string>();
            _avatarList = new List<string>();

            CreateBulkFiles();
            try
            {
                int totalCount = limit ?? (int)_baseStore.CountBlocks();
                int remainingCount = totalCount;
                int offsetIdx = 0;

                while (remainingCount > 0)
                {
                    int interval = 100;
                    int limitInterval;
                    Task<List<ActionEvaluation>>[] taskArray;
                    if (interval < remainingCount)
                    {
                        taskArray = new Task<List<ActionEvaluation>>[interval];
                        limitInterval = interval;
                    }
                    else
                    {
                        taskArray = new Task<List<ActionEvaluation>>[remainingCount];
                        limitInterval = remainingCount;
                    }

                    foreach (var item in
                        _baseStore.IterateIndexes(_baseChain.Id, offset + offsetIdx ?? 0 + offsetIdx, limitInterval).Select((value, i) => new { i, value }))
                    {
                        var block = _baseStore.GetBlock<NCAction>(item.value);
                        var storeBlockList = new List<BlockModel> {BlockData.GetBlockInfo(block)};
                        mySqlStore.StoreBlockList(storeBlockList);
                        taskArray[item.i] = Task.Factory.StartNew(() =>
                        {
                            List<ActionEvaluation> actionEvaluations = EvaluateBlock(block);
                            Console.WriteLine($"Block progress: {block.Index}/{remainingCount}");
                            return actionEvaluations;
                        });
                    }

                    if (interval < remainingCount)
                    {
                        remainingCount -= interval;
                        offsetIdx += interval;
                    }
                    else
                    {
                        remainingCount = 0;
                        offsetIdx += remainingCount;
                    }

                    Task.WaitAll(taskArray);
                    ProcessTasks(taskArray);
                }

                FlushBulkFiles();
                DateTimeOffset postDataPrep = DateTimeOffset.Now;
                Console.WriteLine("Data Preparation Complete! Time Elapsed: {0}", postDataPrep - start);

                foreach (var path in _agentFiles)
                {
                    BulkInsert(AgentTable, path);
                }

                foreach (var path in _avatarFiles)
                {
                    BulkInsert(AvatarTable, path);
                }

                foreach (var path in _hasFiles)
                {
                    BulkInsert(HasTable, path);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            DateTimeOffset end = DateTimeOffset.UtcNow;
            Console.WriteLine("Migration Complete! Time Elapsed: {0}", end - start);
        }

        private void ProcessTasks(Task<List<ActionEvaluation>>[] taskArray)
        {
            foreach (var task in taskArray)
            {
                if (task.Result is { } data)
                {
                    foreach (var ae in data)
                    {
                        if (ae.Action is PolymorphicAction<ActionBase> action)
                        {
                            // avatarNames will be stored as "N/A" for optimzation
                            if (action.InnerAction is HackAndSlash hasAction)
                            {
                                var avatarModel = AvatarData.GetAvatarInfo(ae.OutputStates, ae.InputContext.Signer,
                                    hasAction.AvatarAddress, hasAction.RuneInfos, DateTimeOffset.Now);
                                var storeAvatarList = new List<AvatarModel> {avatarModel};
                                mySqlStore.StoreAvatarList(storeAvatarList);
                                AvatarState avatarState = ae.OutputStates.GetAvatarStateV2(hasAction.AvatarAddress);
                                bool isClear = avatarState.stageMap.ContainsKey(hasAction.StageId);
                                WriteHackAndSlash(
                                    hasAction.Id,
                                    ae.InputContext.BlockIndex,
                                    ae.InputContext.Signer,
                                    hasAction.AvatarAddress,
                                    avatarState.name,
                                    hasAction.StageId,
                                    isClear);
                            }

                            if (action.InnerAction is HackAndSlash19 hasAction19)
                            {
                                var avatarModel = AvatarData.GetAvatarInfo(ae.OutputStates, ae.InputContext.Signer,
                                    hasAction19.AvatarAddress, hasAction19.RuneInfos, DateTimeOffset.Now);
                                var storeAvatarList = new List<AvatarModel> {avatarModel};
                                mySqlStore.StoreAvatarList(storeAvatarList);
                                AvatarState avatarState = ae.OutputStates.GetAvatarStateV2(hasAction19.AvatarAddress);
                                bool isClear = avatarState.stageMap.ContainsKey(hasAction19.StageId);
                                WriteHackAndSlash(
                                    hasAction19.Id,
                                    ae.InputContext.BlockIndex,
                                    ae.InputContext.Signer,
                                    hasAction19.AvatarAddress,
                                    avatarState.name,
                                    hasAction19.StageId,
                                    isClear);
                            }

                            if (action.InnerAction is HackAndSlash0 hasAction0)
                            {
                                AvatarState avatarState = ae.OutputStates.GetAvatarStateV2(hasAction0.avatarAddress);
                                bool isClear = avatarState.stageMap.ContainsKey(hasAction0.stageId);
                                WriteHackAndSlash(
                                    hasAction0.Id,
                                    ae.InputContext.BlockIndex,
                                    ae.InputContext.Signer,
                                    hasAction0.avatarAddress,
                                    avatarState.name,
                                    hasAction0.stageId,
                                    isClear);
                            }

                            if (action.InnerAction is HackAndSlash2 hasAction2)
                            {
                                AvatarState avatarState = ae.OutputStates.GetAvatarStateV2(hasAction2.avatarAddress);
                                bool isClear = avatarState.stageMap.ContainsKey(hasAction2.stageId);
                                WriteHackAndSlash(
                                    hasAction2.Id,
                                    ae.InputContext.BlockIndex,
                                    ae.InputContext.Signer,
                                    hasAction2.avatarAddress,
                                    avatarState.name,
                                    hasAction2.stageId,
                                    isClear);
                            }

                            if (action.InnerAction is HackAndSlash3 hasAction3)
                            {
                                AvatarState avatarState = ae.OutputStates.GetAvatarStateV2(hasAction3.avatarAddress);
                                bool isClear = avatarState.stageMap.ContainsKey(hasAction3.stageId);
                                WriteHackAndSlash(
                                    hasAction3.Id,
                                    ae.InputContext.BlockIndex,
                                    ae.InputContext.Signer,
                                    hasAction3.avatarAddress,
                                    avatarState.name,
                                    hasAction3.stageId,
                                    isClear);
                            }

                            if (action.InnerAction is HackAndSlash4 hasAction4)
                            {
                                AvatarState avatarState = ae.OutputStates.GetAvatarStateV2(hasAction4.avatarAddress);
                                bool isClear = avatarState.stageMap.ContainsKey(hasAction4.stageId);
                                WriteHackAndSlash(
                                    hasAction4.Id,
                                    ae.InputContext.BlockIndex,
                                    ae.InputContext.Signer,
                                    hasAction4.avatarAddress,
                                    avatarState.name,
                                    hasAction4.stageId,
                                    isClear);
                            }

                            if (action.InnerAction is HackAndSlash5 hasAction5)
                            {
                                AvatarState avatarState = ae.OutputStates.GetAvatarStateV2(hasAction5.avatarAddress);
                                bool isClear = avatarState.stageMap.ContainsKey(hasAction5.stageId);
                                WriteHackAndSlash(
                                    hasAction5.Id,
                                    ae.InputContext.BlockIndex,
                                    ae.InputContext.Signer,
                                    hasAction5.avatarAddress,
                                    avatarState.name,
                                    hasAction5.stageId,
                                    isClear);
                            }
                        }
                    }
                }
            }
        }

        private List<ActionEvaluation> EvaluateBlock(Block<NCAction> block)
        {
            var evList = _baseChain.ExecuteActions(block, StateCompleterSet<NCAction>.Reject).ToList();
            return evList;
        }

        private void FlushBulkFiles()
        {
            _agentBulkFile.Flush();
            _agentBulkFile.Close();

            _avatarBulkFile.Flush();
            _avatarBulkFile.Close();

            _hasBulkFile.Flush();
            _hasBulkFile.Close();
        }

        private void CreateBulkFiles()
        {
            string agentFilePath = Path.GetTempFileName();
            _agentBulkFile = new StreamWriter(agentFilePath);

            string avatarFilePath = Path.GetTempFileName();
            _avatarBulkFile = new StreamWriter(avatarFilePath);

            string hasFilePath = Path.GetTempFileName();
            _hasBulkFile = new StreamWriter(hasFilePath);

            _agentFiles.Add(agentFilePath);
            _avatarFiles.Add(avatarFilePath);
            _hasFiles.Add(hasFilePath);
        }

        private void BulkInsert(
            string tableName,
            string filePath)
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            try
            {
                DateTimeOffset start = DateTimeOffset.Now;
                Console.WriteLine($"Start bulk insert to {tableName}.");
                MySqlBulkLoader loader = new MySqlBulkLoader(connection)
                {
                    TableName = tableName,
                    FileName = filePath,
                    Timeout = 0,
                    LineTerminator = "\n",
                    FieldTerminator = ";",
                    Local = true,
                    ConflictOption = MySqlBulkLoaderConflictOption.Ignore,
                };

                loader.Load();
                Console.WriteLine($"Bulk load to {tableName} complete.");
                DateTimeOffset end = DateTimeOffset.Now;
                Console.WriteLine("Time elapsed: {0}", end - start);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine($"Bulk load to {tableName} failed.");
            }
        }

        private void WriteHackAndSlash(
            Guid actionId,
            long blockIndex,
            Address agentAddress,
            Address avatarAddress,
            string avatarName,
            int stageId,
            bool isClear)
        {
            // check if address is already in _agentList
            if (!_agentList.Contains(agentAddress.ToString()))
            {
                _agentBulkFile.WriteLine(
                    $"{agentAddress.ToString()}");
                _agentList.Add(agentAddress.ToString());
            }

            // check if address is already in _avatarList
            if (!_avatarList.Contains(avatarAddress.ToString()))
            {
                _avatarBulkFile.WriteLine(
                    $"{avatarAddress.ToString()};" +
                    $"{agentAddress.ToString()};" +
                    $"{avatarName ?? "N/A"}");
                _avatarList.Add(avatarAddress.ToString());
            }

            _hasBulkFile.WriteLine(
                $"{actionId.ToString()};" +
                $"{avatarAddress.ToString()};" +
                $"{agentAddress.ToString()};" +
                $"{stageId};" +
                $"{(isClear ? 1 : 0)};" +
                $"{(stageId > 10000000 ? 1 : 0)};" +
                $"{blockIndex.ToString()}");
            Console.WriteLine("Writing HackAndSlash action in block #{0}", blockIndex);
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddDbContextFactory<NineChroniclesContext>(options =>
                    {
                        // Get configuration from appsettings or env
                        var configurationBuilder = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .AddEnvironmentVariables("NC_");
                        IConfiguration config = configurationBuilder.Build();
                        var headlessConfig = new Configuration();
                        config.Bind(headlessConfig);
                        if (headlessConfig.MySqlConnectionString != string.Empty)
                        {
                            args = new[] { headlessConfig.MySqlConnectionString };
                        }

                        if (args.Length == 1)
                        {
                            options.UseMySql(
                                args[0],
                                ServerVersion.AutoDetect(
                                    args[0]),
                                b => b.MigrationsAssembly("NineChronicles.DataProvider.Executable"));
                        }
                    });
                });
    }
}
