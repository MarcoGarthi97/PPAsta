using Microsoft.Extensions.Logging;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Migration.Services.Migrations;
using PPAsta.Repository.Services.Repositories.PP.Version;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PPAsta.Migration.Services.Orchestrator
{
    public interface IMigrationOrchestrator : IForServiceCollectionExtension
    {
        Task ExecuteMigrationAsync();
    }

    public class MigrationOrchestrator : IMigrationOrchestrator
    {
        private readonly ILogger<MigrationOrchestrator> _logger;
        private readonly IMdlVersionRepository _versionRepository;
        private readonly IMigration_1_0_0 _migration_1_0_0;
        private readonly IMigration_1_0_1 _migration_1_0_1;

        public MigrationOrchestrator(ILogger<MigrationOrchestrator> logger, IMdlVersionRepository versionRepository, IMigration_1_0_0 migration_1_0_0, IMigration_1_0_1 migration_1_0_1)
        {
            _logger = logger;
            _versionRepository = versionRepository;
            _migration_1_0_0 = migration_1_0_0;
            _migration_1_0_1 = migration_1_0_1;
        }

        public async Task ExecuteMigrationAsync()
        {
            try
            {
                string versionDB = await _versionRepository.GetVersionAsync();

                if (!VersionComparison(versionDB, _migration_1_0_0.GetVersion()))
                {
                    await _migration_1_0_0.ExecuteMigration_1_0_0();

                    await _versionRepository.InsertVersionAsync(_migration_1_0_0.GetVersion());
                }

                if (!VersionComparison(versionDB, _migration_1_0_1.GetVersion()))
                {
                    await _migration_1_0_1.ExecuteMigration_1_0_1();

                    await _versionRepository.UpdateVersionAsync(_migration_1_0_1.GetVersion());
                }
            }
            catch (Exception ex)
            {

            }
        }

        private bool VersionComparison(string versionDB, string versionMigration)
        {
            if (string.IsNullOrEmpty(versionDB))
            {
                return false;
            }

            string[] vDB = versionDB.Split(".");
            string[] vMigration = versionMigration.Split(".");

            if (Convert.ToInt32(vDB[0]) > Convert.ToInt32(vMigration[0]))
            {
                return true;
            }
            else if (Convert.ToInt32(vDB[0]) == Convert.ToInt32(vMigration[0]))
            {
                if (Convert.ToInt32(vDB[1]) > Convert.ToInt32(vMigration[1]))
                {
                    return true;
                }
                else if (Convert.ToInt32(vDB[1]) == Convert.ToInt32(vMigration[1]))
                {
                    if (Convert.ToInt32(vDB[2]) >= Convert.ToInt32(vMigration[2]))
                    {
                        return true;
                    }
                }
            }

            return false;

            //return Convert.ToInt32(vDB[0]) >= Convert.ToInt32(vMigration[0])
            //    || Convert.ToInt32(vDB[1]) >= Convert.ToInt32(vMigration[1])
            //    || Convert.ToInt32(vDB[2]) >= Convert.ToInt32(vMigration[2]);
        }   
    }
}
