using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.RequestModels
{
    public class TaskDataRequest
    {
        [Required]
        [StringLength(20)]
        public string Type { get; set; } // "start", "end", etc.

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } // "Not Started", "In Progress", "Completed", "On Hold"

        [StringLength(100)]
        public string ClickupId { get; set; } = "";

        [StringLength(1000)]
        public string AdditionalNotes { get; set; } = "";
    }
}
