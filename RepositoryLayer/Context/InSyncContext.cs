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
        public DbSet<TaskEntity> Task { get; set; }
        public DbSet<ScheduledTaskEntity> ScheduledTask { get; set; }
        public DbSet<GoogleChatLogEntity> GoogleChatLog { get; set; }
        public DbSet<WebhooksEntity> Webhooks { get; set; }
        public InSyncContext(DbContextOptions<InSyncContext> options)
            : base(options)
        {
        }

    }
}
