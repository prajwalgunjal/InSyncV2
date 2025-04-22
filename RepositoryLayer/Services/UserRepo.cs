using CommonLayer.RequestModels;
using CommonLayer.ResponseModel;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RepositoryLayer.Services
{
    public class UserRepo : IUserRepo
    {
        private InSyncContext syncContext;
        private readonly IConfiguration configuration;
        private readonly IDistributedCache _redisCache;

        public UserRepo(InSyncContext syncContext, IConfiguration configuration,IDistributedCache _redisCache)
        {
            this.syncContext = syncContext;
            this.configuration = configuration;
            this._redisCache = _redisCache;
        }

        public EmployeeMasterEntity RegisterEmployee(EmployeeMasterModel employeeMasterModel)
        {
            try
            {
                // Check if an employee with the same UserName or Email already exists
                var existingEmployee = syncContext.EmployeeMaster
                    .FirstOrDefault(e => e.UserName == employeeMasterModel.UserName || e.Email == employeeMasterModel.Email);
                if (existingEmployee != null)
                {
                    throw new Exception("An employee with the same username or email already exists.");
                }

                var newEmployee = new EmployeeMasterEntity
                {
                    Name = employeeMasterModel.Name,
                    UserName = employeeMasterModel.UserName,
                    PasswordHash = employeeMasterModel.Password, // You should hash the password before saving it
                    PhoneNumber = employeeMasterModel.PhoneNumber,
                    Email = employeeMasterModel.Email,
                    Designation = employeeMasterModel.Designation,
                    EmployeeCode = employeeMasterModel.EmployeeCode,
                    Department = employeeMasterModel.Department,
                    OfficeLocation = employeeMasterModel.OfficeLocation,
                    ReportingManagerID = employeeMasterModel.ReportingManagerID,
                    TeamID = employeeMasterModel.TeamID,
                    DateOfJoining = employeeMasterModel.DateOfJoining,
                    IsHR = employeeMasterModel.IsHR,
                    IsCOO = employeeMasterModel.IsCOO,
                    CreatedDate = DateTime.Now,  // Setting created date to current timestamp
                    UpdatedDate = DateTime.Now,  // Setting updated date to current timestamp
                    IsActive = true,  // New employee is active by default
                    IsDeleted = false  // New employee is not deleted by default
                };

                syncContext.EmployeeMaster.Add(newEmployee);
                syncContext.SaveChanges();
                return newEmployee;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while registering the employee.", ex);
            }
        }
        public string Login(LoginModel loginModel)
        {
            try
            {
                string encodedPassword = loginModel.Password;
                var checkEmail = syncContext.EmployeeMaster.FirstOrDefault(x => x.Email == loginModel.Email);
                var checkPassword = syncContext.EmployeeMaster.FirstOrDefault(x => x.PasswordHash == encodedPassword);

                if (checkEmail != null && checkPassword != null)
                {
                    var token = GenerateToken(checkEmail.Email, checkEmail.EmployeeID);
                    return token;
                }

                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public EmployeeMasterEntity GetLoggedInUserDetails(ClaimsPrincipal userPrincipal)
        {
            try
            {
                if (userPrincipal == null || !userPrincipal.Identity.IsAuthenticated)
                {
                    return null; // Or throw an appropriate exception
                }

                var userIdClaim = userPrincipal.Claims.FirstOrDefault(c => c.Type == "userID");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var loggedInUserId))
                {
                    var loggedInUserEmailClaim = userPrincipal.Claims.FirstOrDefault(c => c.Type == "Email");
                    string loggedInUserEmail = loggedInUserEmailClaim?.Value;

                    var userDeatils = syncContext.EmployeeMaster.FirstOrDefault(e => e.EmployeeID == loggedInUserId && e.Email == loggedInUserEmail);
                    if (userDeatils != null)
                    {
                        return userDeatils;
                    }
                }

                return null; // Or throw an appropriate exception if userID claim is missing or invalid
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ResponseModel<ForgetPasswordModel> ForgetPassword(ClaimsPrincipal userPrincipal, string CurrentPass,string newPass)
        {
            try
            {
                var userIdClaim = userPrincipal.Claims.FirstOrDefault(c => c.Type == "userID");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var loggedInUserId))
                {
                    var loggedInUser = syncContext.EmployeeMaster.Find(loggedInUserId);
                    if (loggedInUser == null)
                    {
                        return new ResponseModel<ForgetPasswordModel> { Success = false, Message = "User not found." };
                    }
                    // 1. Verify the current password
                    if(loggedInUser.PasswordHash == CurrentPass)
                    {
                        // 2. Update the password
                        loggedInUser.PasswordHash = newPass;
                        syncContext.SaveChanges();
                        return new ResponseModel<ForgetPasswordModel> { Success = true, Message = "Password updated successfully."};
                    }
                }
                return new ResponseModel<ForgetPasswordModel> { Success = false, Message = "Current password is incorrect." };
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private string GenerateToken(string Email, int userID)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var claims = new[]
                {
                new Claim("Email",Email), // you can use enum from claimtypes 
                new Claim("userID",userID.ToString())
            };
                var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
                    configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ForgetPasswordModel forgetPassowrdModel(string email)
        {
            try
            {
                if (CheckEmail(email))
                {
                    var employee = syncContext.EmployeeMaster.FirstOrDefault(x => x.Email == email);
                    if (employee != null)
                    {
                        var token = GenerateToken(employee.Email, employee.EmployeeID);
                        long OTP = GenerateOTP();
                        
                        if (OTP > 0)
                        {
                            var forgetPasswordModel = new ForgetPasswordModel
                            {
                                Email = employee.Email,
                                Token = token,
                                EmployeeID = employee.EmployeeID,
                                ExpiryDate = DateTime.Now,
                                OTP = OTP
                            };
                            if (SendEmail(email, OTP))
                            {
                                // Implement Redis logic here
                                string cacheKey = $"ForgetPasswordOTP:{email}";
                                var cacheEntryOptions = new DistributedCacheEntryOptions()
                                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)); // 10 min expiery time
                                var modelJson = JsonSerializer.Serialize(forgetPasswordModel);
                                _redisCache.SetString(cacheKey, modelJson, cacheEntryOptions);
                                return forgetPasswordModel; // Return the model after storing in Redis
                            }
                        }
                        // implement redis logic here
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ResponseModel<ForgetPasswordModel> ForgetPasswordUsingOTP(long OTP, string email)
        {
            try
            {
                string cacheKey = $"ForgetPasswordOTP:{email}";
                var modelJson = _redisCache.GetString(cacheKey);
                if (modelJson != null)
                {
                    var forgetPasswordModel = JsonSerializer.Deserialize<ForgetPasswordModel>(modelJson);
                    if (forgetPasswordModel != null && forgetPasswordModel.OTP == OTP)
                    {
                        ResponseModel<ForgetPasswordModel> responseModel = new()
                        {
                            Success= true,
                            Message = "OTP verified successfully.",
                            Data = forgetPasswordModel
                        };
                        return responseModel;
                    }
                }
                ResponseModel<ForgetPasswordModel> response = new()
                {
                    Success = false,
                    Message = "Invalid OTP or OTP expired.",
                    Data = null
                };
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private bool SendEmail(string toEmail, long OTP)
        {
            try
            {
                string fromName = "InSync";
                string userName = "amitkumarnayak40@gmail.com";
                string fromAddress = "amitkumarnayak40@gmail.com";
                string Password = "uefewqwrywfuosqh";
                string Host = "smtp.gmail.com";
                int Port = 25;
                string subject = "Your One-Time Password (OTP)";
                string body = CreateEmailBody(OTP);
                SmtpClient smtp = new SmtpClient(Host, Port);
                smtp.EnableSsl = Convert.ToBoolean(Environment.GetEnvironmentVariable("MailIsSSL", EnvironmentVariableTarget.Machine) ?? "false");
                smtp.Credentials = new NetworkCredential(userName, Password);
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(fromAddress);
                string[] recipients = toEmail.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string recipient in recipients)
                {
                    msg.To.Add(recipient.Trim());
                }
                msg.Subject = subject;
                msg.Body = body;
                msg.IsBodyHtml = true;
                smtp.Send(msg);
                Console.WriteLine("Email sent successfully.");
                msg.Dispose();
                smtp.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private string CreateEmailBody(long OTP)
        {
            try
            {
                string htmlBody = $@"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Your Colorful One-Time Password (OTP)</title>
            <style>
                body {{
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    background-color: #e0f7fa; /* Light Cyan */
                    color: #333;
                    margin: 0;
                    padding: 30px;
                    display: flex;
                    justify-content: center;
                    align-items: center;
                    min-height: 250px;
                    text-align: center; /* Center align body text */
                }}
                .container {{
                    background-color: #fff;
                    padding: 40px;
                    border-radius: 12px;
                    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
                    text-align: center;
                }}
                h2 {{
                    color: #ff9800; /* Amber */
                    margin-bottom: 30px;
                }}
                .otp-container {{
                    background-color: #fbe9e7; /* Light Orange */
                    padding: 20px 30px;
                    border-radius: 8px;
                    margin-bottom: 30px;
                    font-size: 2em;
                    letter-spacing: 15px;
                    font-weight: bold;
                    color: #2196f3; /* Blue */
                    display: flex;
                    justify-content: center;
                    align-items: center;
                }}
                .otp-value {{
                    margin-right: 15px;
                }}
                .copy-button {{
                    background-color: #4caf50; /* Green */
                    color: #fff;
                    border: none;
                    padding: 12px 18px;
                    border-radius: 6px;
                    cursor: pointer;
                    font-size: 1em;
                    transition: background-color 0.3s ease;
                }}
                .copy-button:hover {{
                    background-color: #388e3c;
                }}
                p {{
                    margin-top: 25px;
                    font-size: 1em;
                    color: #757575; /* Gray */
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h2>Your Colorful One-Time Password (OTP)</h2>
                <div class='otp-container'>
                    <span class='otp-value' id='otpValue'>{OTP}</span>
                    <button class='copy-button' onclick='copyOTP()'>Copy OTP</button>
                </div>
                <p>Please use this <strong style='color: #2196f3;'>one-time password (OTP)</strong> to verify your account.</p>
                <p>It is valid for a limited time for your security.</p>
                <p>If you did not request this OTP, please ignore this email.</p>
            </div>

            <script>
                function copyOTP() {{
                    const otpValueElement = document.getElementById('otpValue');
                    const otpText = otpValueElement.textContent;

                    navigator.clipboard.writeText(otpText)
                        .then(() => {{
                            alert('OTP copied to clipboard!');
                        }})
                        .catch(err => {{
                            console.error('Failed to copy OTP: ', err);
                            alert('Failed to copy OTP. Please copy it manually.');
                        }});
                }}
            </script>
        </body>
        </html>
        ";
                return htmlBody;
            }
            catch (Exception ex)
            {
                // Consider logging the exception
                Console.WriteLine($"Error creating colorful email body: {ex.Message}");
                return $"An error occurred while generating the colorful OTP email body.";
            }
        }

        public long GenerateOTP()
        {
            try
            {
                Random random = new Random();
                long otp = random.Next(100000, 999999);
                return otp;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public bool CheckEmail(string email)
        {
            try
            {
                bool emailExists = syncContext.EmployeeMaster.Any(x => x.Email == email);
                if (emailExists)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
