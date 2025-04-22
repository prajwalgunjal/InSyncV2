using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using CommonLayer.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Entity;

namespace InSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : Controller
    {
        private ITaskBusiness taskBusiness;
        private IUserBusiness iUserBusiness;
        private readonly ILogger<TaskController> logger;
        public TaskController(ITaskBusiness taskBusiness, ILogger<TaskController> logger, IUserBusiness iUserBusiness)
        {
            this.taskBusiness = taskBusiness;
            this.logger = logger;
            this.iUserBusiness = iUserBusiness;
        }
        [Authorize]
        [HttpPost]
        [Route("CreateTask")]
        public IActionResult CreateTask(TaskMasterEntity task)
        {
            try
            {
                task.Employee = iUserBusiness.GetLoggedInUserDetails(HttpContext.User);
                taskBusiness.CreateTask(task);
                return Ok(new ResponseModel<string> { Success = true, Message = "Create Task Successfull", Data = "Create Task" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }
        [Authorize]
        [HttpPost]
        [Route("UpdateTask")]
        public IActionResult UpdateTask()
        {
            try
            {
                return Ok(new ResponseModel<string> { Success = true, Message = "Create Task Successfull", Data = "Create Task" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }
        [Authorize]
        [HttpPost]
        [Route("DeleteTask")]
        public IActionResult DeleteTask()
        {
            try
            {
                return Ok(new ResponseModel<string> { Success = true, Message = "Create Task Successfull", Data = "Create Task" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }
        [Authorize]
        [HttpPost]
        [Route("ViewAllTasks")]
        public IActionResult ViewAllTasks()
        {
            try
            {
                return Ok(new ResponseModel<string> { Success = true, Message = "Create Task Successfull", Data = "Create Task" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }
    }
}
