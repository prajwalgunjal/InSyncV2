using CommonLayer.RequestModels;
using CommonLayer.ResponseModel;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Services
{
    public class TaskRepo : ITaskRepo
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
        public async Task SendMessageToGoogleChat(string messageText)
        {
            string url = "";
            var client = new RestClient();
            var request = new RestRequest(url, Method.Post);
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");
            var message = new { text = messageText };
            request.AddJsonBody(message); // Automatically serializes to JSON
            try
            {
                // Send the request
                var response = client.Execute(request);

                // Check if the response is successful
                if (response.IsSuccessful)
                {
                    Console.WriteLine("Message sent successfully!");
                }
                else
                {
                    Console.WriteLine($"Failed to send message. Status code: {response.StatusCode}, Response: {response.Content}");
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
