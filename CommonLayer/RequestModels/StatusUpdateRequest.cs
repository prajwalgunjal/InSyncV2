using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.RequestModels
{
    public class StatusUpdateRequest
    {
        [Required]
        public List<TaskDataRequest> Tasks { get; set; } = new List<TaskDataRequest>();

        [Required]
        [StringLength(50)]
        public string MessageTemplate { get; set; }

        [Required]
        [StringLength(2000)]
        public string FormattedMessage { get; set; }
    }
}
