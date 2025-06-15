using Azure.Core;
using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using CommonLayer.RequestModels;
using CommonLayer.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Entity;
using System.Security.Claims;

namespace InSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : Controller
    {
        private ITaskBusiness taskBusiness;
        private IUserBusiness iUserBusiness;
        private readonly ILogger<TaskController> _logger;
        public TaskController(ITaskBusiness taskBusiness, ILogger<TaskController> logger, IUserBusiness iUserBusiness)
        {
            this.taskBusiness = taskBusiness;
            this._logger = logger;
            this.iUserBusiness = iUserBusiness;
        }

        // POST: api/CreateTask/SendToGoogleChat
        [Authorize]
        [HttpPost("SendToGoogleChat")]
        public async Task<IActionResult> SendToGoogleChat([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Extract logged-in user ID from claims
                EmployeeMasterEntity Emp = GetLoggedInUserId();

                var result = await taskBusiness.SendToGoogleChatAsync(request, Emp);

                return Ok(new
                {
                    message = "Status sent to Google Chat successfully",
                    processedTasks = request.Tasks?.Count ?? 0,
                    messageTemplate = request.MessageTemplate,
                    employeeId = Emp.EmployeeID,
                    success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send to Google Chat");
                return StatusCode(500, new
                {
                    error = "Failed to send to Google Chat",
                    message = ex.Message
                });
            }
        }

        // POST: api/CreateTask/ScheduleTask
        [Authorize]
        [HttpPost("ScheduleTask")]
        public async Task<IActionResult> ScheduleTask([FromBody] ScheduleTaskRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Extract logged-in user ID from claims
                var employeeId = GetLoggedInUserId();

                var result = await taskBusiness.ScheduleTaskAsync(request, employeeId);

                return Ok(new
                {
                    message = "Task scheduled successfully",
                    scheduledTasks = request.ScheduledTasks?.Count ?? 0,
                    employeeId = employeeId,
                    success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to schedule task");
                return StatusCode(500, new
                {
                    error = "Failed to schedule task",
                    message = ex.Message
                });
            }
        }
        // GET: api/CreateTask/GetTasksByEmployee
        [Authorize]
        [HttpGet("GetTasksByEmployee")]
        public async Task<IActionResult> GetTasksByEmployee()
        {
            try
            {
                var employeeId = GetLoggedInUserId();

                // Implementation can be added to service if needed
                return Ok(new
                {
                    message = "Feature can be implemented to get tasks",
                    employeeId = employeeId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get tasks");
                return StatusCode(500, new { error = "Failed to retrieve tasks", message = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("SaveWebhooksURL")]
        public async Task<IActionResult> SaveWebhooksURL([FromBody] WebhooksUrlRequestModel webhooksUrl)
        {
            try
            {
                if (webhooksUrl.Url != null)
                {
                    var employee = GetLoggedInUserId();
                    var result = await taskBusiness.SaveWebhooksURL(webhooksUrl, employee);

                    return Ok(new
                    {
                        message = "Webhooks URL saved successfully",
                        webhooksUrl = webhooksUrl,
                        employeeId = employee.EmployeeID
                    });
                }
                return BadRequest(new { error = "Webhooks URL cannot be null or empty" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save webhooks URL");
                return StatusCode(500, new { error = "Failed to save webhooks URL", message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("GetWebhooks")]
        public async Task<IActionResult> GetWebhooks()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var employee = GetLoggedInUserId();
                var result = await taskBusiness.GetWebhooks(employee);

                return Ok(new
                {
                    message = "Webhooks URL saved successfully",
                    webhooksUrl = result.Data[0],
                    employeeId = employee.EmployeeID
                });
                return BadRequest(new { error = "Webhooks URL cannot be null or empty" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save webhooks URL");
                return StatusCode(500, new { error = "Failed to save webhooks URL", message = ex.Message });
            }
        }

        // GET: api/CreateTask/GetScheduledTasks
        [Authorize]
        [HttpGet("GetScheduledTasks")]
        public async Task<IActionResult> GetScheduledTasks()
        {
            try
            {
                var employeeId = GetLoggedInUserId();

                // Implementation can be added to service if needed
                return Ok(new
                {
                    message = "Feature can be implemented to get scheduled tasks",
                    employeeId = employeeId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get scheduled tasks");
                return StatusCode(500, new { error = "Failed to retrieve scheduled tasks", message = ex.Message });
            }
        }
        private EmployeeMasterEntity GetLoggedInUserId()
        {
            try
            {
                // Method 1: Using your existing business logic
                var userDetails = iUserBusiness.GetLoggedInUserDetails(HttpContext.User);
                if (userDetails != null)
                {
                    return userDetails;
                }
                throw new UnauthorizedAccessException("User ID not found in claims");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to extract user ID from claims");
                throw new UnauthorizedAccessException("Unable to identify logged-in user");
            }
        }
    }
}
