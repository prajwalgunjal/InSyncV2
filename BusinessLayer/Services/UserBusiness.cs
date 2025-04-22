using BusinessLayer.Interfaces;
using CommonLayer.RequestModels;
using CommonLayer.ResponseModel;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class UserBusiness : IUserBusiness
    {
        private IUserRepo userRepo;
        public UserBusiness(IUserRepo userRepo)
        {
            this.userRepo = userRepo;
        }
        public EmployeeMasterEntity RegisterEmployee(EmployeeMasterModel employeeMasterModel)
        {
            return userRepo.RegisterEmployee(employeeMasterModel);
        }
        public string Login(LoginModel loginModel)
        {
            return userRepo.Login(loginModel);
        }
        public EmployeeMasterEntity GetLoggedInUserDetails(ClaimsPrincipal userPrincipal)
        {
            return userRepo.GetLoggedInUserDetails(userPrincipal);
        }
        public ForgetPasswordModel forgetPassowrdModel(string email)
        {
            return userRepo.forgetPassowrdModel(email);
        }
        public ResponseModel<ForgetPasswordModel> ForgetPasswordUsingOTP(long OTP, string email)
        {
            return userRepo.ForgetPasswordUsingOTP(OTP, email);
        }
        public ResponseModel<ForgetPasswordModel> ForgetPassword(ClaimsPrincipal userPrincipal, string CurrentPass, string newPass)
        {
            return userRepo.ForgetPassword(userPrincipal, CurrentPass, newPass);
        }
    }
}
