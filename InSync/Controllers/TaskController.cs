using CommonLayer.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : Controller
    {
        private readonly ILogger<TaskController> logger;
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        [Route("CreateTask")]
        public IActionResult CreateTask()
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
