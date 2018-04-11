﻿using ImageService.Controller;
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

namespace ImageService.Server
{
    /*
     * ImageServer creates a Handler for each folder that we want to watch
     * 
     */
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        private IDictionary<CommandEnum, ICommand> m_commands;
        private string outputFolder;
        private string[] foldersToWatch;
    
        #endregion

        #region Properties
        public event EventHandler<CommandReceivedEventArgs> CommandReceived;          // The event that notifies about a new Command being recieved
        #endregion

        public ImageServer(ILoggingService logger)
        {
            m_logging = logger;
        }

        public void Start()
        {
            int.TryParse(ConfigurationManager.AppSettings["ThumbnailSize"], out int thumbnailSize);
            m_controller = new ImageController(new ImageServiceModal(ConfigurationManager.AppSettings["OutputDir"], thumbnailSize));
            string str = ConfigurationManager.AppSettings["Handler"];
            String[] foldersToWatch = str.Split(';');

            foreach (string folder in foldersToWatch)
            {
                DirectoyHandler handler = new DirectoyHandler(m_logging, m_controller);
                handler.StartHandleDirectory(folder);
            }
        }

        protected virtual void OnCommandReceived(CommandReceivedEventArgs e)
        {
            CommandReceived(this, e);
            EventHandler<CommandReceivedEventArgs> handler = CommandReceived;
            if (handler != null)
            {
                handler(this, e);
            }
            
        }


    }
}
