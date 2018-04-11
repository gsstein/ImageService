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
      //  #region Members
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingService m_logging;
        private FileSystemWatcher m_dirWatcher;             // The Watcher of the Dir
        private string m_path;                              // The Path of directory
     //   #endregion

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed

        public DirectoyHandler(ILoggingService log, IImageController controller)
        {
            m_logging = log;
            m_controller = controller;
        }

        // The Function Recieves the directory to Handle
        public void StartHandleDirectory(string dirPath)
        {
            if (!dirPath.EndsWith("\\")) {
                dirPath += "\\";
            }
            m_path = dirPath;

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            m_path = dirPath;
            m_dirWatcher = new FileSystemWatcher(dirPath);
            m_dirWatcher.NotifyFilter = NotifyFilters.LastWrite
           | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            m_dirWatcher.Filter = "*.*";
            m_dirWatcher.Created += new FileSystemEventHandler(OnCreated);
            m_dirWatcher.EnableRaisingEvents = true;
            m_logging.Log("watching " + dirPath, MessageTypeEnum.INFO);
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            List<string> extensions = new List<string>() { ".jpg", ".png", ".gif", ".bmp" };
            
            string extension = Path.GetExtension(e.FullPath);
            if (!extensions.Contains(extension))
            {
                return;
            }
            m_logging.Log(e.Name + " was added to " + m_path, MessageTypeEnum.INFO);
            
            string[] args = { e.FullPath };

            string info = m_controller.ExecuteCommand(CommandEnum.NewFile, args, out bool result);
            m_logging.Log(info, MessageTypeEnum.INFO);

            if(result == true)
            {
                m_logging.Log(e.Name + " copied to ", MessageTypeEnum.FAIL);
            } else if(result == false)
            {
                m_logging.Log(e.Name + " failed to copy to ", MessageTypeEnum.INFO);
            }
            
        }

        // The Event that will be activated upon new Command
        public void OnCommandRecieved(object sender, CommandReceivedEventArgs e)
        {
            if(e.CommandID.Equals(CommandEnum.Close))
            {
                m_dirWatcher.Dispose();
                DirectoryCloseEventArgs args = new DirectoryCloseEventArgs(m_path, m_path + "closed");
                DirectoryClose.Invoke(sender, args);
            }
            m_controller.ExecuteCommand(e.CommandID, e.Args, out bool result);
        }
    }
}
