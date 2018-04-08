using ImageService.Commands;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        private IImageServiceModal m_modal;                      // The Modal Object
        private Dictionary<CommandEnum, ICommand> commands;

        public ImageController(IImageServiceModal modal)
        {
            m_modal = modal;                    // Storing the Modal Of The System
            commands = new Dictionary<CommandEnum, ICommand>()
            {
                { CommandEnum.NewFile, new NewFileCommand(modal) }
            };
        }

        public string ExecuteCommand(CommandEnum commandID, string[] args, out bool resultSuccesful)
        {
            ICommand value;
            commands.TryGetValue(commandID, out value);
            return value.Execute(args, out resultSuccesful);
        }
    }
}
