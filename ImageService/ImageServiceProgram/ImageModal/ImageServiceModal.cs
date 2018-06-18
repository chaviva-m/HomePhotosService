using CommandInfrastructure.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ImageServiceProgram.ImageModal
{
    public class ImageServiceModal : IImageServiceModal
    {
        #region Members
        private string OutputFolder;          
        private int ThumbnailSize;             
        private Regex Rgx = new Regex(":");
        #endregion

        /// <summary>
        /// image modal service constructor
        /// </summary>
        /// <param name="outputFolder where it will move files"></param>
        /// <param name="thumbnailSize the size of the thumbnail of the file"></param>
        public ImageServiceModal(string outputFolder, int thumbnailSize)
        {
            this.OutputFolder = outputFolder;
            this.ThumbnailSize = thumbnailSize;
        }

        public string AddFile(string path, out bool result)                          
        {
            //create output directory
            string resultDescription = CreateFolder(OutputFolder, FileAttributes.Hidden, out result);
            if (!result)
            {
                return resultDescription;
            }

            //extract date from image
            DateTime dateTime;
            Thread.Sleep(1000);

            resultDescription = ExtractDateOfImage(path, out dateTime, out result);
            if (!result)
            {
                return resultDescription;
            }
            //create folder for image according to date
            string relativeDateLocation = Path.Combine(dateTime.Year.ToString(), dateTime.ToString("MMMM"));
            string newLocation = Path.Combine(OutputFolder, relativeDateLocation);
            resultDescription = CreateFolder(newLocation, FileAttributes.Normal, out result);
            if (!result)
            {
                return resultDescription;
            }
            //move image to folder
            //string newPath = UniqueFileName(Path.Combine(newLocation, Path.GetFileName(path)));
			string newPath = Path.Combine(newLocation, Path.GetFileName(path));
			resultDescription = MoveFile(path, newPath, out result);
            if (!result)
            {
                return resultDescription;
            }
            //create folder for thumbnail image according to date
            string thumbnailLocation = Path.Combine(OutputFolder, "Thumbnails", relativeDateLocation);
            resultDescription = CreateFolder(thumbnailLocation, FileAttributes.Normal, out result);
            if (!result)
            {
                return resultDescription;
            }
            //create thumbnail of image in folder
            resultDescription = CreateThumbnail(newPath, thumbnailLocation, out result);
            if (!result)
            {
                return resultDescription;
            }
            //finished adding file
            result = true;
            return "Successfully added file " + newPath;
        }

        /// <summary>
        /// Creates folder according to path parameter, if folder doesn't already exist.
        /// If fileAtr is hidden, will set attribute of folder to hidden.
        /// </summary>
        /// <param name="path">path of new folder</param>
        /// <param name="result">result of action: true = success, false = failure</param>
        /// <param name="dirAtr">attribute of folder</param>        /// 
        /// <returns>string describing the result of function's action.</returns>
        public string CreateFolder(string path, FileAttributes dirAtr, out bool result)
        {
            try
            {
                if (!Directory.Exists(path)) 
                {
                    DirectoryInfo dir = Directory.CreateDirectory(path);
                    if (dirAtr == FileAttributes.Hidden && !dir.Attributes.HasFlag(FileAttributes.Hidden))
                    {
                        dir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                    }
                    result = true;
                    return "Created folder " + path;
                } else
                {
                    result = true;
                    return "Folder " + path + " exists.";
                }

            } catch (Exception e)                                                                           
            {
                result = false;
                return "Could not create folder " + path + ".\nProblem: " + e.Message;
            }
      
        }

        /// <summary>
        /// Moves file from sourceFile to destinationFile.
        /// </summary>
        /// <param name="sourceFile">source path of file</param>
        /// <param name="destinationFile">destination path of file</param>
        /// <param name="result">result of action: true = success, false = failure</param>
        /// <returns>string describing the result of function's action.</returns>
        public string MoveFile(string sourceFile, string destinationFile, out bool result)
        {
            try
            {
				//overwrite existing file with same name
				if (File.Exists(destinationFile))
				{
					File.Delete(destinationFile);
				}
                System.IO.File.Move(sourceFile, destinationFile);
                result = true;
                return "Moved file from " + sourceFile + " to " + destinationFile;
            } catch(Exception e)                                                                            
            {
                result = false;
                return "Could not move file " + sourceFile + " to " + destinationFile + ".\nProblem: " + e.Message;
            }
           
        }

        /// <summary>
        /// this function checks if file name already exists, if it does then the function adds a number to the name.
        /// </summary>
        /// <param name="file">file name to make unique</param>
        /// <returns>unique name for file</returns>
        private string UniqueFileName(string file)
        {
            string uniqueFileName = file;

            if (File.Exists(uniqueFileName))
            {
                string dirName = Path.GetDirectoryName(uniqueFileName);
                string fileName = Path.GetFileNameWithoutExtension(uniqueFileName);
                string extension = Path.GetExtension(uniqueFileName);
                int i = 1;
                while (File.Exists(uniqueFileName))
                {
                    uniqueFileName = Path.Combine(dirName, fileName + "(" + i + ")" + extension);
                    i++;
                }
            }
            return uniqueFileName;
        }

        /// <summary>
        /// Extracts information from an image about the date the image was taken on.
        /// </summary>
        /// <param name="path">source path of picture</param>
        /// <param name="dateTime">date and time that the picture was taken on</param>
        /// <param name="result">result of action: true = success, false = failure</param>
        /// <returns>string describing the result of function's action.</returns>
        private string ExtractDateOfImage(string path, out DateTime dateTime, out bool result)                   
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (Image picture = Image.FromStream(fs, false, false))
                {
                    try
                    {
                        PropertyItem propItem = picture.GetPropertyItem(36867);
                        string dateOfPicture = Rgx.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                        dateTime = DateTime.Parse(dateOfPicture);
                        result = true;
                        return "Extracted date taken from image " + path;
                    }
                    catch (Exception)                                                
                    {
                        //this method of extracting the date might return an inaccurate date
                        dateTime = new FileInfo(path).LastWriteTime;
                        result = true;
                        return "Extracted date taken from image " + path;
                    }
                }
            }
            catch (Exception)                                                       
            {
                //returning current date time
                dateTime = DateTime.Now;
                if (File.Exists(path))
                {
                    result = true;
                    return "Cannot extract date from " + path + ". Returning current date time instead.";
                } else
                {
                    result = false;
                    return "Cannot extract date from " + path + ". File does not exist.";
                }
            }
        }

        /// <summary>
        /// Creates thumbnail of image in newLocation
        /// </summary>
        /// <param name="imagePath">image path from which the function will create a thumbnail</param>
        /// <param name="thumbnailLocation">the folder where the function will save the thumbnail</param>
        /// <param name="result">result of action: true = success, false = failure</param>
        /// <returns>string describing the result of function's action.</returns>
        public string CreateThumbnail(string imagePath, string thumbnailLocation, out bool result)
        {
            try
            {
                using (Image image = Image.FromFile(imagePath))
                {
					//Size ThumbWidthHeight = GetThumbnailSize(image);
					Size ThumbWidthHeight = new Size(ThumbnailSize, ThumbnailSize);
					using (Image thumbnail = image.GetThumbnailImage(ThumbWidthHeight.Width, ThumbWidthHeight.Height, null, IntPtr.Zero))
                    {
                        string thumbPath = Path.Combine(thumbnailLocation, Path.GetFileName(imagePath));
                        thumbnail.Save(thumbPath);
                        result = true;
                        return "saved thumbnail of image " + imagePath;
                    }

                }
            }
            catch (Exception e)
            {
                result = false;
                return "Could not save thumbnail of image " + imagePath + ".\nProblem: " + e.Message;
            }
        }

        /// <summary>
        /// Calculates appropriate size for thumbnail of image acording to ThumbnailSize member of class.
        /// </summary>
        /// <param name="image">image for which the function will calculate thumbnail size</param>
        /// <returns>Size of thumbnail</returns>
        private Size GetThumbnailSize(Image image)
        {
            int thumbnailWidth = 0;
            int thumbnailHeight = 0;
            double scalingFactor = 0;
            //assign ThumbnailSize to bigger dimension
            if (image.Width > image.Height)
            {
                thumbnailWidth = ThumbnailSize;
                //scale height by same factor as width
                scalingFactor = (double)image.Width / thumbnailWidth;
                thumbnailHeight = (int)(image.Height / scalingFactor); 
            }
            else
            {
                thumbnailHeight = ThumbnailSize;
                //scale width by same factor as height
				scalingFactor = (double)image.Height / thumbnailHeight;
				thumbnailWidth = (int)(image.Width / scalingFactor);
			}
            return new Size(thumbnailWidth, thumbnailHeight);
        }
     }
}