using BusinessLayer.Interfaces;
using CommonLayer.RequestModels;
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
        public Task<bool> SendToGoogleChatAsync(StatusUpdateRequest request, EmployeeMasterEntity Emp)
        {
            return taskRepository.SendToGoogleChatAsync(request, Emp);
        }
        public Task<bool> SendToTelegramAsync(StatusUpdateRequest request, EmployeeMasterEntity Emp)
        {
            return taskRepository.SendToTelegramAsync(request, Emp);
        }
        public Task<bool> ScheduleTaskAsync(ScheduleTaskRequest request, EmployeeMasterEntity Emp)
        {
            return taskRepository.ScheduleTaskAsync(request, Emp);
        }
        public Task<List<ScheduledTaskEntity>> GetScheduledTasks(EmployeeMasterEntity Emp)
        {
            return taskRepository.GetScheduledTasks( Emp);
        }
        public Task<List<ScheduledTaskEntity>> GetPostedTasks(EmployeeMasterEntity Emp)
        {
            return taskRepository.GetPostedTasks(Emp);
        }
        /* public Task SendMessageToGoogleChat(string messageText)
         {
             return taskRepository.SendMessageToGoogleChat(messageText);
         }*/
        public async Task<bool> SaveWebhooksURL(WebhooksUrlRequestModel webhooks, EmployeeMasterEntity Emp)
        {
            return await taskRepository.SaveWebhooksURL(webhooks, Emp);
        }
        public async Task<bool> SaveTelegramConfig(TelegramWebhookRequest webhooks, EmployeeMasterEntity Emp)
        {
            return await taskRepository.SaveTelegramConfig(webhooks, Emp);
        }
        public async Task<ResponseModel<List<WebhooksUrlRequestModel>>> GetWebhooks(EmployeeMasterEntity emp)
        {
            return await taskRepository.GetWebhooks(emp);
        }
        public async Task<ResponseModel<List<TelegramWebhookRequest>>> GetTelegramConfig(EmployeeMasterEntity emp)
        {
            return await taskRepository.GetTelegramConfig(emp);
        }
    }   
}
