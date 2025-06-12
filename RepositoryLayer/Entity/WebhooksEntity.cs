using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Entity
{
    public class WebhooksEntity
    {
        [Key]
        public int WebhookID { get; set; }

        [Required]
        [StringLength(500)]
        public string WebhooksURL { get; set; }
        public int EmployeeID { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        // Optional: if Employee is a related table
        //public virtual EmployeeMasterEntity Employee { get; set; }
    }
}
