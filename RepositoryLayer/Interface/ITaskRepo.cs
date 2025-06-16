using CommonLayer.RequestModels;
using CommonLayer.ResponseModel;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Interface
{
    public interface ITaskRepo
    {
        public Task<ResponseModel<TaskMasterEntity>> CreateTask(TaskMasterEntity task);
        Task<bool> SendToGoogleChatAsync(StatusUpdateRequest request, EmployeeMasterEntity Emp);
        Task<bool> ScheduleTaskAsync(ScheduleTaskRequest request, EmployeeMasterEntity Emp);
        Task<bool> SaveWebhooksURL(WebhooksUrlRequestModel webhooks, EmployeeMasterEntity Emp);
        Task<ResponseModel<List<WebhooksUrlRequestModel>>> GetWebhooks(EmployeeMasterEntity emp);
    }
}
