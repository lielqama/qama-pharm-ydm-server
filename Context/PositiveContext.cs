using PharmYdm.Migrations;
using PharmYdm.Models;
using Positive.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PharmYdm.Context
{
    public class PharmYdmContext : DbContext 
    {

        public PharmYdmContext(): base("AuthContext") {

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<PharmYdmContext, Configuration>());

        }

        public DbSet<PharmYdm_Log> PharmYdm_Log { get; set; }
        public DbSet<PharmYdmQamaClient> HFDQamaClients { get; set; }
        public DbSet<BarQamaClient> BarQamaClients { get; set; }

    }
}