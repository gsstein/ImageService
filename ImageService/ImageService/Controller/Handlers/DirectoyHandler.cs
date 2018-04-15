using ImageService.Modal;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using System.Text.RegularExpressions;

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        private IImageController Controller;              // The Image Processing Controller
        private ILoggingService Logging;
        private FileSystemWatcher DirWatcher;             // The Watcher of the Dir
        private string DirPath;                              // The Path of directory

        /*
         *  Watches the specified directory
         **/
        public DirectoyHandler(ILoggingService log, IImageController controller)
        {
            Logging = log;
            Controller = controller;
        }

        // The Function Receives the directory to Handle
        public void StartHandleDirectory(string dirPath)
        {
            if (!dirPath.EndsWith("\\")) {
                dirPath += "\\";
            }

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            DirPath = dirPath;
            InitializeFileWatcher();
        }

        private void InitializeFileWatcher()
        {
            DirWatcher = new FileSystemWatcher(DirPath)
            {
                NotifyFilter = NotifyFilters.LastWrite
           | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Filter = "*.*"
            };
            DirWatcher.Created += new FileSystemEventHandler(OnCreated);

            /* Add later
            DirWatcher.Deleted += new FileSystemEventHandler(OnDeleted);
            DirWatcher.Renamed += new FileSystemEventHandler(OnRenamed);
            */

            DirWatcher.EnableRaisingEvents = true;
        }

        //Called when a new file is added
        private void OnCreated(object source, FileSystemEventArgs e)
        {
            List<string> extensions = new List<string>() { ".jpg", ".png", ".gif", ".bmp" };
            
            string extension = Path.GetExtension(e.FullPath);
            if (!extensions.Contains(extension))
            {
                return;
            }

            Logging.Log(e.Name + " was added to " + DirPath, MessageTypeEnum.INFO);
            string[] args = { e.FullPath };
            string info = Controller.ExecuteCommand(CommandEnum.NewFile, args, out bool result);

            if (result)
            {
                Logging.Log(e.Name + " moved to " + DirPath, MessageTypeEnum.INFO);
            } else
            {
                Logging.Log(e.Name + " failed to move to " + DirPath + ": " + info, MessageTypeEnum.FAIL);
            }
        }

        private void OnDeleted(object source, FileSystemEventArgs e) {/*TODO*/}

        private void OnRenamed(object source, FileSystemEventArgs e) {/*TODO*/}

        // The Event that will be activated upon new Command
        public void OnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            if (e.CommandID.Equals(CommandEnum.Close))
            {
                CloseDirectory();
            }
            else
            {
                Controller.ExecuteCommand(e.CommandID, e.Args, out bool result);
            }
        }

        private void CloseDirectory()
        {
            DirWatcher.EnableRaisingEvents = false;
            Logging.Log("Closed " + DirPath, MessageTypeEnum.INFO);
        }
    }
}
