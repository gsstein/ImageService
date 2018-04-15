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
        private IImageServiceModal Modal;                      // The Modal Object
        private Dictionary<CommandEnum, ICommand> Commands;

        public ImageController(IImageServiceModal modal)
        {
            Modal = modal;                    // Storing the Modal Of The System
            Commands = new Dictionary<CommandEnum, ICommand>()
            {
                { CommandEnum.NewFile, new NewFileCommand(Modal) }
            };
        }

        public string ExecuteCommand(CommandEnum commandID, string[] args, out bool resultSuccesful)
        {
            ICommand value;
            Commands.TryGetValue(commandID, out value);
            return value.Execute(args, out resultSuccesful);
        }
    }
}
