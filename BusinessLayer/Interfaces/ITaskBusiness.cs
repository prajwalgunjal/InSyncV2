using CommonLayer.RequestModels;
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
        Task<bool> SendToGoogleChatAsync(StatusUpdateRequest request, EmployeeMasterEntity emp);
        Task<bool> ScheduleTaskAsync(ScheduleTaskRequest request, EmployeeMasterEntity employeeId);
        /*Task SendMessageToGoogleChat(string messageText);*/
        Task<bool> SaveWebhooksURL(WebhooksUrlRequestModel webhooks, EmployeeMasterEntity Emp);
        Task<ResponseModel<List<WebhooksUrlRequestModel>>> GetWebhooks(EmployeeMasterEntity emp);
    }
}
