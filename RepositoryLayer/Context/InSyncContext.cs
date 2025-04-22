using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
namespace RepositoryLayer.Context
{
    public class InSyncContext : DbContext
    {
        public DbSet<EmployeeMasterEntity> EmployeeMaster { get; set; }
        public DbSet<TaskMasterEntity> taskMasterEntities { get; set; }
        public InSyncContext(DbContextOptions<InSyncContext> options)
            : base(options)
        {
        }

    }
}
