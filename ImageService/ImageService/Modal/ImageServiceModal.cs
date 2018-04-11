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

        public ImageServiceModal(string OutputFolder, int thumbnailSize)
        {
            m_OutputFolder = OutputFolder;
            m_OutputFolder = AddBackslash(m_OutputFolder);
            m_thumbnailSize = thumbnailSize;
            CreateThumbNailFolder();
        }

        private string AddBackslash(string folder)
        {
            if (!folder.EndsWith("\\"))
            {
                folder += "\\";
            }
            return folder;
        }

        private void CreateThumbNailFolder()
        {
            m_thumbnailFolder = m_OutputFolder + "Thumbnails\\";
            CreateFolder(m_thumbnailFolder);
        }

        public static DateTime GetDateTakenFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                PropertyItem propItem = myImage.GetPropertyItem(36867);
                string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                return DateTime.Parse(dateTaken);
            }
        }

        private void CreateDateDirectory(DateTime dt)
        {
            CreateFolder(m_OutputFolder + date);
            CreateFolder(m_thumbnailFolder + date);
        }

        private void CreateThumbNail(string path)
        {
            Image image = Image.FromFile(path);
            Image thumb = image.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);
            thumb.Save(Path.ChangeExtension(m_thumbnailFolder + date + Path.GetFileName(path), "thumb"));
        }

        public void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public string AddFile(string path, out bool result)
        {
            result = false;

            if(File.Exists(path))
            {
                string newPath = "";
                try
                {
                    DateTime dt = GetDateTakenFromImage(path);
                    date = dt.Year + "\\" + dt.Month + "\\";
                    CreateDateDirectory(dt);
                    newPath += m_OutputFolder + date + Path.GetFileName(path);
                    File.Copy(path, newPath, true);
                    CreateThumbNail(newPath);
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
    }
}
