using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Entity
{
    public class GoogleChatLogEntity
    {
        [Key]
        public int LogId { get; set; }

        [Required]
        [StringLength(50)]
        public string MessageTemplate { get; set; }

        [Required]
        [StringLength(2000)]
        public string FormattedMessage { get; set; }

        [Required]
        public int TaskCount { get; set; }

        public bool IsSuccessful { get; set; } = false;
        public string ErrorMessage { get; set; } = "";
        public DateTime SentDate { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int EmployeeID{ get; set; } = 0;
    }
}
