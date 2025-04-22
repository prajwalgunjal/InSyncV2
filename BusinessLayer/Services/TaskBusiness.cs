using BusinessLayer.Interfaces;
using CommonLayer.ResponseModel;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class TaskBusiness :ITaskBusiness
    {
        private readonly ITaskRepo  taskRepository;
        public TaskBusiness(ITaskRepo taskRepository)
        {
            this.taskRepository = taskRepository;
        }
        public async Task<ResponseModel<TaskMasterEntity>> CreateTask(TaskMasterEntity task)
        {
            return await taskRepository.CreateTask(task);
        }
    }
}
