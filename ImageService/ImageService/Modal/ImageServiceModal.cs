using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ImageService.Modal
{
    public class ImageServiceModal : IImageServiceModal
    {
        private string OutputFolder;            // The Output Folder
        private string ThumbnailFolder;
        private int ThumbnailSize;              // The Size Of The Thumbnail Size
        private static Regex r = new Regex(":");
        private string Date;

        public ImageServiceModal(string outputFolder, int thumbnailSize)
        {
            OutputFolder = outputFolder;
            OutputFolder = AddBackslash(OutputFolder);
            ThumbnailSize = thumbnailSize;
            CreateThumbnailFolder();
        }

        private string AddBackslash(string folder)
        {
            if (!folder.EndsWith("\\"))
            {
                folder += "\\";
            }
            return folder;
        }

        private void CreateThumbnailFolder()
        {
            ThumbnailFolder = OutputFolder + "Thumbnails\\";
            CreateFolder(ThumbnailFolder);
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
            CreateFolder(OutputFolder + Date);
            CreateFolder(ThumbnailFolder + Date);
        }

        private void CreateThumbnail(string path)
        {
            Image image = Image.FromFile(path);
            Image thumb = image.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);
            thumb.Save(Path.ChangeExtension(ThumbnailFolder + Date + Path.GetFileName(path), "thumb"));
        }

        public string AddFile(string path, out bool result)
        {
            result = false;

            if (File.Exists(path))
            {
                string newPath = "";
                try
                {
                    DateTime dt = GetDateTakenFromImage(path);
                    Date = dt.Year + "\\" + dt.Month + "\\";
                    CreateDateDirectory(dt);
                    newPath += OutputFolder + Date + Path.GetFileName(path);
                    File.Copy(path, newPath, true);
                    CreateThumbnail(newPath);
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

        public void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
