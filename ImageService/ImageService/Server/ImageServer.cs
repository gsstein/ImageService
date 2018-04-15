using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Commands;
using System.Configuration;
using System.IO;

namespace ImageService.Server
{
    /*
     * ImageServer creates a Handler for each folder that we want to watch
     * 
     */
    public class ImageServer
    {
        private IImageController Controller;
        private ILoggingService Logging;

        public event EventHandler<CommandReceivedEventArgs> CommandReceived;          // The event that notifies about a new Command being received

        public ImageServer(ILoggingService logger)
        {
            Logging = logger;
        }


        public void Start()
        {
            // Create controller from output directory and thumbnail size
            string outputDir = ConfigurationManager.AppSettings["OutputDir"];
            HideOutputDir(outputDir);
            int.TryParse(ConfigurationManager.AppSettings["ThumbnailSize"], out int thumbnailSize);
            Controller = new ImageController(new ImageServiceModal(outputDir, thumbnailSize));

            // Create handler for each folder
            string handlers = ConfigurationManager.AppSettings["Handler"];
            string[] foldersToWatch = handlers.Split(';');
            CreateHandlers(foldersToWatch);
        }

        private void CreateHandlers(string[] foldersToWatch)
        {
            foreach (string folder in foldersToWatch)
            {
                DirectoyHandler handler = new DirectoyHandler(Logging, Controller);
                CommandReceived += handler.OnCommandReceived; // When ImageServer sends a command the event will trigger each handler's OnCommandReceived
                handler.StartHandleDirectory(folder);
            }
        } 

        private void HideOutputDir(string dir)
        {
            DirectoryInfo di;
            if (!Directory.Exists(dir))
            {
                di = Directory.CreateDirectory(dir);
            }
            else
            {
                di = new DirectoryInfo(dir);
            }
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
        }

        public void Stop()
        {
            //Sends the CLOSE command
            CommandReceivedEventArgs args = new CommandReceivedEventArgs(CommandEnum.Close, null, "");
            OnCommandReceived(args);
        }


        protected virtual void OnCommandReceived(CommandReceivedEventArgs e)
        {
            EventHandler<CommandReceivedEventArgs> handler = CommandReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

    }
}
