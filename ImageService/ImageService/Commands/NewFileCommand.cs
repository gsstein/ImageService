using ImageService.Infrastructure;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ImageService.Commands
{
    public class NewFileCommand : ICommand
    {
        private IImageServiceModal Modal;

        public NewFileCommand(IImageServiceModal modal)
        {
            Modal = modal;            // Storing the Modal
        }

        public string Execute(string[] args, out bool result)
        {
            return Modal.AddFile(args[0], out result);
        }
    }
}
