using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.RequestModels
{
    public class ScheduleTaskRequest
    {
        [Required]
        public List<ScheduledTaskDataRequest> ScheduledTasks { get; set; } = new List<ScheduledTaskDataRequest>();
    }
}
