using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceProgram.Commands
{
    class SaveFileCommand : ICommand
    {

        /// <summary>
        /// execute saveFileCommand
        /// </summary>
        /// <param name="args">
        /// args[0] holds image in base64string
        /// args[1] image name
        /// args[2] holds path of directory to save the file in 
        /// args[3] client ID </param>
        /// <param name="result">result of command</param>
        /// <returns>return string indicating if command was successful</returns>
        public string Execute(string[] args, out bool result)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(args[0]);


            string handler = args[2];

            Image image;
            string name;
            ImageFormat format;

            //save byte array as image in handler
            using (MemoryStream mStream = new MemoryStream(imageBytes))
            {
                image = Image.FromStream(mStream);
                name = args[1];
                try
                {
                    format = GetImageFormat(name);
                }
                catch (Exception e)
                {
                    result = false;
                    return e.Message;
                }
                image.Save(Path.Combine(handler, name), format);
                result = true;
                return "successfully saved image: " + name;
            }
        }

        /// <summary>
        /// get format of image
        /// </summary>
        /// <param name="name">name of image</param>
        /// <returns>format of image</returns>
        private ImageFormat GetImageFormat(string name)
        {
            string ext = Path.GetExtension(name);
            if (string.IsNullOrEmpty(ext))
                throw new ArgumentException(
                    string.Format("Unable to determine file extension for fileName: {0}", name));

            switch (ext.ToLower())
            {
                case @".bmp":
                    return ImageFormat.Bmp;

                case @".gif":
                    return ImageFormat.Gif;

                case @".jpg":
                case @".jpeg":
                    return ImageFormat.Jpeg;

                case @".png":
                    return ImageFormat.Png;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}