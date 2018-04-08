//using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
//using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.Modal
{
    public class ImageServiceModal : IImageServiceModal
    {
        #region Members
        private string m_OutputFolder;            // The Output Folder
        private int m_thumbnailSize;              // The Size Of The Thumbnail Size

        #endregion

        public ImageServiceModal(string OutputFolder, int thumbnailSize = 60)
        {
            m_OutputFolder = OutputFolder;
            m_thumbnailSize = thumbnailSize;
        }

        public string AddFile(string path, out bool result)
        {
            result = false;

            if(File.Exists(path))
            {
                string newPath = "";
                try
                {
                    newPath = m_OutputFolder + Path.GetFileName(path);
                    File.Copy(path, newPath, true);
                }
                catch (Exception e)
                {
                    return e.ToString();
                }
                result = true;
                return newPath;
            }

            return "File does not exist";
        }

        public void CreateFolder(string path, string folderName)
        {
            try
            {
                System.IO.Directory.CreateDirectory(path + folderName);
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    
}
