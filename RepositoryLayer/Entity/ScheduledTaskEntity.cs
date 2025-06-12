using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Entity
{
    public class ScheduledTaskEntity
    {
        [Key]
        public int ScheduledTaskId { get; set; }

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

        [Required]
        [Column(TypeName = "date")]
        public DateTime ScheduledDate { get; set; }

        [Required]
        [Column(TypeName = "time")]
        public TimeSpan ScheduledTime { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; }

        public bool IsExecuted { get; set; } = false;
        public DateTime? ExecutedDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        // Foreign key
        public int? TaskId { get; set; }
        public virtual TaskEntity Task { get; set; }
        public int EmployeeID { get; set; } = 0;

    }
}
