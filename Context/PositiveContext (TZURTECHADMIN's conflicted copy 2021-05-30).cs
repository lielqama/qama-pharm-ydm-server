using HFD.Migrations;
using HFD.Models;
using Positive.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HFD.Context
{
    public class HFDContext : DbContext 
    {

        public HFDContext(): base("AuthContext") {

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<HFDContext, Configuration>());

        }

        public DbSet<HFD_Log> HFD_Log { get; set; }
        public DbSet<HFDQamaClient> HFDQamaClients { get; set; }

        public System.Data.Entity.DbSet<HFD.Models.BarQamaClient> BarQamaClients { get; set; }
    }
}