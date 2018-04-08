
using ImageService.Logging.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging
{
    public class LoggingService : ILoggingService
    {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public void Log(string message, MessageTypeEnum type)
        {
            MessageReceivedEventArgs args = new MessageReceivedEventArgs();
            args.Message = message;
            args.Status = type;
            OnMessageReceived(args);
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            EventHandler<MessageReceivedEventArgs> handler = MessageReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
