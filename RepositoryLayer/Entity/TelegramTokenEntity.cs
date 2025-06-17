using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Entity
{
    public class TelegramTokenEntity
    {
        [Key]
        public int TokenID { get; set; }
        public string TokenName { get; set; }

        [Required]
        [StringLength(500)]
        public string ChannelName { get; set; }
        public int EmployeeID { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;
    }
}
