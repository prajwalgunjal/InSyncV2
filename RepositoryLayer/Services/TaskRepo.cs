using CommonLayer.RequestModels;
using CommonLayer.ResponseModel;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Services
{
    class TaskRepo
    {
        private InSyncContext syncContext;
        public TaskRepo(InSyncContext syncContext)
        {
            this.syncContext = syncContext;
        }
        public async Task<ResponseModel<TaskMasterEntity>> CreateTask(TaskMasterEntity task)
        {
            try
            {
                await syncContext.taskMasterEntities.AddAsync(task);
                await syncContext.SaveChangesAsync();
                return new ResponseModel<TaskMasterEntity> { Success = true, Message = "Password updated successfully.",Data= task };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
