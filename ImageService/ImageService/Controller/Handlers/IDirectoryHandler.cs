using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller.Handlers
{
    public interface IDirectoryHandler
    {
        //event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed
        void StartHandleDirectory(string dirPath);             // The Function Receives the directory to Handle
        void OnCommandReceived(object sender, CommandReceivedEventArgs e);     // The Event that will be activated upon new Command
    }
}
