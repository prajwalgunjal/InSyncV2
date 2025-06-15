using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.RequestModels
{
    public class WebhooksUrlRequestModel
    {
        public string Name { get; set; } = null;
        [Url]
        public string Url { get; set; }
    }
}
