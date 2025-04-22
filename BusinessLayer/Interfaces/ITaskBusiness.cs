using CommonLayer.ResponseModel;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface ITaskBusiness
    {
        public Task<ResponseModel<TaskMasterEntity>> CreateTask(TaskMasterEntity task);
    }
}
