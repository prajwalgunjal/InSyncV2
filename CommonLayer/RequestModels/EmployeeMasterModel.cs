using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.RequestModels
{
    public class EmployeeMasterModel
    {
        //[Required]
        //[StringLength(100)]
        public string Name { get; set; }

        //[Required]
        //[StringLength(50)]
        public string UserName { get; set; }

        //[Required]
        //[StringLength(256)]
        public string Password { get; set; }  // This is the user's password (to be hashed in the backend)

        //[StringLength(15)]
        public string PhoneNumber { get; set; }

        //[StringLength(100)]
        //[EmailAddress]
        public string Email { get; set; }

        //[StringLength(50)]
        public string Designation { get; set; }

        //[Required]
        //[StringLength(20)]
        public string EmployeeCode { get; set; }

        //[StringLength(50)]
        public string Department { get; set; }

        //[StringLength(50)]
        public string OfficeLocation { get; set; }

        public int? ReportingManagerID { get; set; }

        public int? TeamID { get; set; }

        //[Required]
        public DateTime DateOfJoining { get; set; }

        public bool IsHR { get; set; } = false;
        public bool IsCOO { get; set; } = false;

    }
}
