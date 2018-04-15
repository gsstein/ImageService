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
        private string Date;                    // The date path (\year\month)

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

        /**
         * From https://stackoverflow.com/questions/180030/how-can-i-find-out-when-a-picture-was-actually-taken-in-c-sharp-running-on-vista 
         **/
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

        /**
         * From https://stackoverflow.com/questions/2808887/create-thumbnail-image
         * */
        private void CreateThumbnail(string path)
        {
            Image image = Image.FromFile(path);
            Image thumb = image.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);
            thumb.Save(Path.ChangeExtension(ThumbnailFolder + Date + Path.GetFileName(path), "thumb"));
        }

        public string AddFile(string path, out bool result)
        {
            FileInfo fi = new FileInfo(path);
            while (FileIsLocked(fi))
            {
                // Wait while file is in use
            }

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
                    File.Move(path, newPath);
                    CreateThumbnail(newPath);
                }
                catch (Exception e)
                {
                    return e.ToString();
                }
                result = true;
                return newPath;
            }

            return path + " does not exist";
        }

        public void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /**
         * From https://stackoverflow.com/questions/10982104/wait-until-file-is-completely-written
         * */
        private bool FileIsLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }
    }
}
