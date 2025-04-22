using CommonLayer.RequestModels;
using CommonLayer.ResponseModel;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Interface
{
    public interface IUserRepo
    {
        public EmployeeMasterEntity RegisterEmployee(EmployeeMasterModel employeeMasterModel);
        public string Login(LoginModel loginModel);
        public EmployeeMasterEntity GetLoggedInUserDetails(ClaimsPrincipal userPrincipal);
        public ForgetPasswordModel forgetPassowrdModel(string email);
        public ResponseModel<ForgetPasswordModel> ForgetPasswordUsingOTP(long OTP, string email);
        public ResponseModel<ForgetPasswordModel> ForgetPassword(ClaimsPrincipal userPrincipal, string CurrentPass, string newPass);

    }
}
