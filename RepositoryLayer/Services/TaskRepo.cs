    using CommonLayer.RequestModels;
using CommonLayer.ResponseModel;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<TaskRepo> _logger;

        public TaskRepo(InSyncContext syncContext, ILogger<TaskRepo> _logger)
        {
            this.syncContext = syncContext;
            this._logger = _logger;
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
        public async Task<bool> SendToGoogleChatAsync(StatusUpdateRequest request, EmployeeMasterEntity Emp)
        {
            var chatLog = new GoogleChatLogEntity
            {
                MessageTemplate = request.MessageTemplate,
                FormattedMessage = request.FormattedMessage,
                TaskCount = request.Tasks?.Count ?? 0,
                EmployeeID = Emp.EmployeeID,
                IsSuccessful = false,
                CreatedDate = DateTime.Now,
                SentDate = DateTime.Now
            };

            try
            {
                // Create new tasks from the request
                var iscreated = await CreateTasksFromRequest(request, Emp);
                if (iscreated)
                {
                    // Send to Google Chat using the provided formatted message
                    var ispushedonGchat = await SendMessageToGoogleChat_New(request.FormattedMessage, Emp);
                    if (ispushedonGchat)
                    {
                        // Mark as successful
                        chatLog.IsSuccessful = true;
                        chatLog.ErrorMessage = "";

                        _logger.LogInformation($"Successfully sent status update to Google Chat for Employee {Emp.EmployeeID}");
                        return true;
                    }
                }
                chatLog.ErrorMessage = "Failed to create tasks from request";
                _logger.LogWarning($"Failed to create tasks for Employee {Emp.EmployeeID}");
                return false;
                
            }
            catch (Exception ex)
            {
                chatLog.ErrorMessage = ex.Message;
                _logger.LogError(ex, $"Failed to send status update to Google Chat for Employee {Emp.EmployeeID}");
                throw;
            }
            finally
            {
                // Always log the attempt
                syncContext.GoogleChatLog.Add(chatLog);
                await syncContext.SaveChangesAsync();
            }
        }
        public async Task<bool> SaveWebhooksURL(string url , EmployeeMasterEntity Emp)
        {
            try
            {
                // check the user is valid 
                var existingEmployee = syncContext.EmployeeMaster
                    .FirstOrDefault(e => e.UserName == Emp.UserName || e.Email == Emp.Email);
                if (existingEmployee != null)
                {
                    if (string.IsNullOrEmpty(url))
                    {
                        throw new ArgumentException("Webhooks URL cannot be null or empty");
                    }
                    WebhooksEntity webhooksEntity = new WebhooksEntity
                    {
                        WebhooksURL = url,
                        EmployeeID = Emp.EmployeeID,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false
                    };
                    syncContext.Webhooks.Add(webhooksEntity);
                    await syncContext.SaveChangesAsync();
                    _logger.LogInformation($"Webhooks URL saved successfully for Employee {Emp.EmployeeID}");
                    return true;
                }
                else
                {
                    _logger.LogWarning($"Employee not found for UserName: {Emp.UserName} or Email: {Emp.Email}");
                    throw new ArgumentException("Invalid employee details provided");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> ScheduleTaskAsync(ScheduleTaskRequest request, EmployeeMasterEntity Emp)
        {
            try
            {
                var scheduledTasks = new List<ScheduledTaskEntity>();

                foreach (var taskDto in request.ScheduledTasks)
                {
                    // Parse date and time strings
                    if (!DateTime.TryParse(taskDto.ScheduledDate, out DateTime scheduledDate))
                    {
                        throw new ArgumentException($"Invalid date format: {taskDto.ScheduledDate}. Expected format: YYYY-MM-DD");
                    }

                    if (!TimeSpan.TryParse(taskDto.ScheduledTime, out TimeSpan scheduledTime))
                    {
                        throw new ArgumentException($"Invalid time format: {taskDto.ScheduledTime}. Expected format: HH:MM");
                    }

                    var scheduledTask = new ScheduledTaskEntity
                    {
                        Title = taskDto.Title,
                        Status = taskDto.Status,
                        ClickupId = taskDto.ClickupId,
                        AdditionalNotes = taskDto.AdditionalNotes,
                        ScheduledDate = scheduledDate.Date,
                        ScheduledTime = scheduledTime,
                        Type = taskDto.Type,
                        EmployeeID = Emp.EmployeeID,
                        TaskId = null, // No existing task reference since we're creating new ones
                        IsExecuted = false,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false
                    };

                    scheduledTasks.Add(scheduledTask);
                }

                syncContext.ScheduledTask.AddRange(scheduledTasks);
                await syncContext.SaveChangesAsync();

                _logger.LogInformation($"Successfully scheduled {scheduledTasks.Count} tasks for Employee {Emp.EmployeeID}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to schedule tasks for Employee {Emp.EmployeeID}");
                throw;
            }
        }

        private async Task<bool> CreateTasksFromRequest(StatusUpdateRequest tasks, EmployeeMasterEntity Emp)
        {
            try
            {
                if (tasks.Tasks == null || !tasks.Tasks.Any()) return false;

                var taskEntities = new List<TaskEntity>();

                foreach (var taskRequest in tasks.Tasks)
                {
                    var taskEntity = new TaskEntity
                    {
                        Type = taskRequest.Type,
                        Title = taskRequest.Title,
                        Status = taskRequest.Status,
                        ClickupId = taskRequest.ClickupId,
                        AdditionalNotes = taskRequest.AdditionalNotes,
                        MessageTemplate = tasks.MessageTemplate, // Will be set from the main request
                        FormattedMessage = tasks.FormattedMessage, // Will be set from the main request
                        EmployeeID = Emp.EmployeeID,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false
                    };

                    taskEntities.Add(taskEntity);
                }

                syncContext.Task.AddRange(taskEntities);
                await syncContext.SaveChangesAsync();
                _logger.LogInformation($"Created {taskEntities.Count} new tasks for Employee {Emp.EmployeeID}");
                return true;
            }
            catch (Exception ex)
            {
                
            }
            return false;
        }
        public async Task<bool> SendMessageToGoogleChat_New(string messageText, EmployeeMasterEntity emp)
        {
            var Webhooks = syncContext.Webhooks.FirstOrDefault(w => Convert.ToInt32(w.EmployeeID) == (emp.EmployeeID));
            if (!string.IsNullOrEmpty(Webhooks.WebhooksURL))
            {
                //string url = "https://chat.googleapis.com/v1/spaces/AAAA_IKzwCI/messages?key=AIzaSyDdI0hCZtE6vySjMm-WEfRq3CPzqKqqsHI&token=UyRJ-aPa1hZieISMW9CgfPhsjonluBUz6X_RA4x_Ipo";
                var client = new RestClient();
                var request = new RestRequest(Webhooks.WebhooksURL, Method.Post);
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
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to send message. Status code: {response.StatusCode}, Response: {response.Content}");
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }
    }
}
