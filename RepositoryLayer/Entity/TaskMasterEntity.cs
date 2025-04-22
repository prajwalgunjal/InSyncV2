using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Entity
{
    public class TaskMasterEntity
    {
        [Key]
        public int TaskID { get; set; }

        public string Description { get; set; }

        public string Clickup { get; set; }

        public string WebhooksURL { get; set; }

        [ForeignKey("Employee")]
        public int? EmployeeID { get; set; }
        public EmployeeMasterEntity Employee { get; set; } // Navigation property
        public int? TeamID { get; set; }
        //public TeamMasterEntity Team { get; set; } // Navigation property (assuming you have a TeamMasterEntity)

        [StringLength(20)]
        public string Status { get; set; } // "ToDo", "InProgress", "DevDone", "Hold", "LiveReady", "Live", "QADone", "QAReady", "QAReopen"

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;

        public string RCA { get; set; }

        [StringLength(50)]
        public string DayStartTime { get; set; }

        [StringLength(50)]
        public string DayEndTime { get; set; }
    }
}
