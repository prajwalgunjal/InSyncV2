using BusinessLayer.Interfaces;
using CommonLayer.RequestModels;
using CommonLayer.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using System.Security.Claims;

namespace InSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private IUserBusiness iUserBusiness;
        private readonly ILogger<EmployeeController> logger;
        public EmployeeController(IUserBusiness iUserBusiness, ILogger<EmployeeController> logger)
        {
            this.iUserBusiness = iUserBusiness;
            this.logger = logger;
        }
        [HttpPost]
        // request url:-  localhost/Controller_name/MethodRoute
        [Route("RegisterEmployee")]
        public ActionResult Registeration(EmployeeMasterModel registrationModel)
        {
            try
            {
                var result = iUserBusiness.RegisterEmployee(registrationModel);
                if (result != null)
                {
                    logger.LogInformation("Registered information");
                    return Ok(new ResponseModel<EmployeeMasterEntity> { Success = true, Message = "Registred Successfull", Data = result });
                }
                else
                {
                    return BadRequest(new ResponseModel<EmployeeMasterEntity> { Success = false, Message = "Not Registred", Data = null });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                throw ex;
            }
        }
        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginModel loginModel)
        {
            try
            {
                var result = iUserBusiness.Login(loginModel);
                if (result != null)
                {
                    return Ok(new ResponseModel<string> { Success = true, Message = "Login Successfull", Data = result });
                }

                else
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Email Not Found", Data = null });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Authorize] // Ensure only authenticated users can access this action
        [HttpPost]
        [Route("GetAllUsers")]
        public IActionResult GetUserDetails()
        {
            try
            {
                var result = iUserBusiness.GetLoggedInUserDetails(HttpContext.User);
                if (result != null)
                {
                    return Ok(new ResponseModel<string> { Success = true, Message = "Login Successfull", Data = result.ToString() });
                }

                else
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Email Not Found", Data = null });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [HttpPost]
        [Route("Test")]
        public IActionResult Test()
        {
            try
            {
                return Ok(new ResponseModel<string> { Success = true, Message = "Test Successfull", Data = "Test" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [HttpPost]
        [Route("ForgetPassword")]
        public IActionResult ForgetPassword( string EmailID)
        {
            try
            {
                if (!string.IsNullOrEmpty(EmailID))
                {
                    var res = iUserBusiness.forgetPassowrdModel(EmailID);
                    if(!string.IsNullOrEmpty(res.Token))
                        return Ok(new ResponseModel<string> { Success = true, Message = $"OTP Mail Sent to your {EmailID}", Data = "Forget Password" });
                }
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Email Not Found", Data = null });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        [Route("ForgetPasswordUsingOTP")]
        public IActionResult ForgetPasswordUsingOTP(long OTP, string email)
        {
            try
            {

                if (OTP>0)
                {
                    var res = iUserBusiness.ForgetPasswordUsingOTP(OTP, email);
                    if(!string.IsNullOrEmpty(res.Data.Token))
                        return Ok(new ResponseModel<string> { Success = true, Message = $"This is your Token you can set new Password now", Data = $"{res.Data.Token}" });
                }
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Email Not Found", Data = null });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Authorize]
        [HttpPost]
        [Route("ResetPassword")]
        public IActionResult ResetPassword( string CurrentPassword,string newPassword) 
        {
            try
            {
                if (HttpContext.User!= null)
                {
                    var res = iUserBusiness.ForgetPassword(HttpContext.User, CurrentPassword, newPassword);
                    if (res.Success)
                        return Ok(new ResponseModel<string> { Success = true, Message = "Password reset Successfull", Data = "Forget Password" });
                }
                return BadRequest(new ResponseModel<string> { Success = false, Message = "Email Not Found", Data = null });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
    