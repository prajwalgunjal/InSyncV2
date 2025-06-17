using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.RequestModels
{
    public class TelegramWebhookRequest
    {
        public string telegramToken { get; set; }
        public string channelName { get; set; }
    }
}
