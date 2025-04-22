using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Entity
{
    public class EmployeeMasterEntity
    {
        [Key]
        public int EmployeeID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(256)]
        public string PasswordHash { get; set; }

        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(50)]
        public string Designation { get; set; }

        [Required]
        [StringLength(20)]
        public string EmployeeCode { get; set; }

        [StringLength(50)]
        public string Department { get; set; }

        [StringLength(50)]
        public string OfficeLocation { get; set; }
        public int? ReportingManagerID { get; set; }
        public int? TeamID { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime DateOfJoining { get; set; }

        public bool IsHR { get; set; } = false;
        public bool IsCOO { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"EmployeeID: {EmployeeID}");
            sb.AppendLine($"Name: {Name}");
            sb.AppendLine($"UserName: {UserName}");
            sb.AppendLine($"PasswordHash: {PasswordHash}"); // Be cautious about displaying this in production logs
            sb.AppendLine($"PhoneNumber: {PhoneNumber}");
            sb.AppendLine($"Email: {Email}");
            sb.AppendLine($"Designation: {Designation}");
            sb.AppendLine($"EmployeeCode: {EmployeeCode}");
            sb.AppendLine($"Department: {Department}");
            sb.AppendLine($"OfficeLocation: {OfficeLocation}");
            sb.AppendLine($"ReportingManagerID: {ReportingManagerID}");
            sb.AppendLine($"TeamID: {TeamID}");
            sb.AppendLine($"DateOfJoining: {DateOfJoining.ToShortDateString()}");
            sb.AppendLine($"IsHR: {IsHR}");
            sb.AppendLine($"IsCOO: {IsCOO}");
            sb.AppendLine($"CreatedDate: {CreatedDate}");
            sb.AppendLine($"UpdatedDate: {UpdatedDate}");
            sb.AppendLine($"IsActive: {IsActive}");
            sb.AppendLine($"IsDeleted: {IsDeleted}");
            return sb.ToString();
        }

    }

}
