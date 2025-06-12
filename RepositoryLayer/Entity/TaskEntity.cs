using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Entity
{
    public class TaskEntity
    {
        [Key]
        public int TaskId { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        [StringLength(100)]
        public string ClickupId { get; set; } = "";

        [StringLength(1000)]
        public string AdditionalNotes { get; set; } = "";

        [StringLength(50)]
        public string MessageTemplate { get; set; } = "";

        [StringLength(2000)]
        public string FormattedMessage { get; set; } = "";

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public int EmployeeID { get; set; } = 0;

        // Navigation property for scheduled tasks
        public virtual ICollection<ScheduledTaskEntity> ScheduledTasks { get; set; } = new List<ScheduledTaskEntity>();
    }
}
