    using CommonLayer.RequestModels;
using CommonLayer.ResponseModel;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
        public async Task<bool> SendToTelegramAsync(StatusUpdateRequest request, EmployeeMasterEntity Emp)
        {
            var chatLog = new TelegramTokenLogEntity
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
                    var ispushedonTelegram = await SendToTelegramAsync(request.FormattedMessage, Emp);
                    if (ispushedonTelegram)
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
                syncContext.TelegramTokenLog.Add(chatLog);
                await syncContext.SaveChangesAsync();
            }
        }
        public async Task<ResponseModel<List<WebhooksUrlRequestModel>>> GetWebhooks(EmployeeMasterEntity emp)
        {
            try
            {
                if (emp is not null && emp.EmployeeID > 0)
                {
                    var webhooks = syncContext.Webhooks
    .Where(w => w.EmployeeID == emp.EmployeeID && w.IsActive && !w.IsDeleted)
    .Select(w => new WebhooksUrlRequestModel { Url = w.WebhooksURL, Name = w.WebhookName })
    .ToList();
                    if (webhooks.Any())
                    {
                        return new ResponseModel<List<WebhooksUrlRequestModel>>
                        {
                            Success = true,
                            Message = "Webhooks retrieved successfully",
                            Data = webhooks
                        };
                    }
                    else
                    {
                        return new ResponseModel<List<WebhooksUrlRequestModel>>
                        {
                            Success = false,
                            Message = "No active webhooks found for this employee",
                            Data = null
                        };
                    }
                }
                else
                {
                    return new ResponseModel<List<WebhooksUrlRequestModel>>
                    {
                        Success = false,
                        Message = "Invalid employee details provided",
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public async Task<ResponseModel<List<TelegramWebhookRequest>>> GetTelegramConfig(EmployeeMasterEntity emp)
        {
            try
            {
                if (emp is not null && emp.EmployeeID > 0)
                {
                    var webhooks = syncContext.TelegramToken
    .Where(w => w.EmployeeID == emp.EmployeeID && w.IsActive && !w.IsDeleted)
    .Select(w => new TelegramWebhookRequest { telegramToken = w.TokenName, channelName= w.ChannelName })
    .ToList();
                    if (webhooks.Any())
                    {
                        return new ResponseModel<List<TelegramWebhookRequest>>
                        {
                            Success = true,
                            Message = "Webhooks retrieved successfully",
                            Data = webhooks
                        };
                    }
                    else
                    {
                        return new ResponseModel<List<TelegramWebhookRequest>>
                        {
                            Success = false,
                            Message = "No active webhooks found for this employee",
                            Data = null
                        };
                    }
                }
                else
                {
                    return new ResponseModel<List<TelegramWebhookRequest>>
                    {
                        Success = false,
                        Message = "Invalid employee details provided",
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public async Task<bool> SaveWebhooksURL(WebhooksUrlRequestModel webhooks , EmployeeMasterEntity Emp)
        {
            try
            {
                // check the user is valid 
                var existingEmployee = syncContext.EmployeeMaster
                    .FirstOrDefault(e => e.UserName == Emp.UserName || e.Email == Emp.Email);
                if (existingEmployee != null)
                {
                    if (string.IsNullOrEmpty(webhooks.Url))
                    {
                        throw new ArgumentException("Webhooks URL cannot be null or empty");
                    }
                    WebhooksEntity webhooksEntity = new WebhooksEntity
                    {
                        WebhooksURL = webhooks.Url,
                        WebhookName = webhooks.Name,
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
        public async Task<bool> SaveTelegramConfig(TelegramWebhookRequest webhooks , EmployeeMasterEntity Emp)
        {
            try
            {
                // check the user is valid 
                var existingEmployee = syncContext.EmployeeMaster
                    .FirstOrDefault(e => e.UserName == Emp.UserName || e.Email == Emp.Email);
                if (existingEmployee != null)
                {
                    if (string.IsNullOrEmpty(webhooks.telegramToken))
                    {
                        throw new ArgumentException("Telegram Token cannot be null or empty");
                    }
                    TelegramTokenEntity webhooksEntity = new TelegramTokenEntity
                    {
                        TokenName = webhooks.telegramToken,
                        ChannelName = webhooks.channelName,
                        EmployeeID = Emp.EmployeeID,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false
                    };
                    syncContext.TelegramToken.Add(webhooksEntity);
                    await syncContext.SaveChangesAsync();
                    _logger.LogInformation($"Telegram Tokens saved successfully for Employee {Emp.EmployeeID}");
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
            }
            return false;
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
        /*public async Task<bool> SendMessageToGoogleChat_New(string messageText, EmployeeMasterEntity emp)
        {
            try
            {
                var Webhooks = syncContext.Webhooks.FirstOrDefault(w => Convert.ToInt32(w.EmployeeID) == (emp.EmployeeID));
                if (!string.IsNullOrEmpty(Webhooks.WebhooksURL))
                {
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
            }
            catch (Exception)
            {}
            return false;
        }*/

        // Alternative method using Cards V2 (newer format)
        public async Task<bool> SendMessageToGoogleChat_New(string messageText, EmployeeMasterEntity emp)
        {
            try
            {
                var Webhooks = syncContext.Webhooks.FirstOrDefault(w => Convert.ToInt32(w.EmployeeID) == (emp.EmployeeID));
                if (!string.IsNullOrEmpty(Webhooks.WebhooksURL))
                {
                    var client = new RestClient();
                    var request = new RestRequest(Webhooks.WebhooksURL, Method.Post);
                    request.AddHeader("Content-Type", "application/json; charset=UTF-8");

                    // Cards V2 format with custom sender
                    var message = new
                    {
                        cardsV2 = new object[]
                        {
                    new
                    {
                        cardId = "daily-update-card",
                        card = new
                        {
                            header = new
                            {
                                title = emp.Name ?? "Employee Update",
                                subtitle = $"ID: {emp.EmployeeID}",
                                imageUrl = GetEmployeeAvatarUrl(emp), // Method to get employee avatar
                                imageType = "CIRCLE"
                            },
                            sections = new object[]
                            {
                                new
                                {
                                    header = "📋 Daily Update",
                                    widgets = new object[]
                                    {
                                        new
                                        {
                                            textParagraph = new
                                            {
                                                text = $"<b>Status Update:</b><br>{messageText}"
                                            }
                                        },
                                        new
                                        {
                                            decoratedText = new
                                            {
                                                startIcon = new
                                                {
                                                    knownIcon = "CLOCK"
                                                },
                                                text = $"<b>Time:</b> {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                        }
                    };

                    request.AddJsonBody(message);

                    try
                    {
                        var response = client.Execute(request);
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
                        Console.WriteLine($"Error sending message: {ex.Message}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
            }
            return false;
        }

        // Method to get employee avatar URL
        private string GetEmployeeAvatarUrl(EmployeeMasterEntity emp)
        {
            if (!string.IsNullOrEmpty(emp.Email))
            {
                var emailHash = GetMD5Hash(emp.Email.ToLower().Trim());
                return $"https://www.gravatar.com/avatar/{emailHash}?d=identicon&s=200";
            }
            return $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(emp.Name ?? "User")}&background=0D8ABC&color=fff&size=200";
        }

        // Helper method for MD5 hash (for Gravatar)
        private string GetMD5Hash(string input)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes).ToLower();
            }
        }
        public async Task<bool> SendToTelegramAsync(string sendMessage, EmployeeMasterEntity emp)
        {
            try
            {
                var TokenDetails = syncContext.TelegramToken.FirstOrDefault(w => Convert.ToInt32(w.EmployeeID) == (emp.EmployeeID));
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                sendMessage = sendMessage.Replace("_", "\\_");
                var client = new RestClient();
                var request = new RestRequest($"https://api.telegram.org/bot{TokenDetails.TokenName}/sendMessage", Method.Post);
                request.AddQueryParameter("parse_mode", "Markdown");
                request.AddJsonBody(new
                {
                    chat_id = TokenDetails.ChannelName,
                    text = sendMessage
                });
                var response = await client.ExecuteAsync(request);

                return response.IsSuccessful;
            }
            catch (Exception ex)
            {
                // Log ex if needed
                return false;
            }
        }
    }
}
