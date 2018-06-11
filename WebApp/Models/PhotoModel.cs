using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
	public class PhotoModel
	{
		private string thumbnailPath;
		public string ThumbnailPath { get { return thumbnailPath; } private set { thumbnailPath = value; } }
		private string path;
		public string PhotoPath { get{ return path; } private set { path = value; } }
		private string date;
		public string Date { get { return date; } set { date = value; } }

		/// <summary>
		/// construct photo model
		/// </summary>
		/// <param name="thumbnailPath">path of thumbnail of photo</param>
		/// <param name="photoDate">date of photo</param>
		public PhotoModel(string thumbnailPath, string photoDate)
		{
			ThumbnailPath = thumbnailPath;
			PhotoPath = FindPhotoPath(thumbnailPath);
			Date = photoDate;
		}

		/// <summary>
		/// get path to the photo
		/// </summary>
		/// <param name="thumbnailPath">path of thumbnail</param>
		/// <returns>return path to photo</returns>
		private string FindPhotoPath(string thumbnailPath)
		{
			string thumbnailDir = "Thumbnails";
			string photoPath = thumbnailPath.Replace(thumbnailDir, "");
			return photoPath;
		}

		/// <summary>
		/// delete photo and thumbnail fom output directory
		/// </summary>
		/// <returns>true on success, else false</returns>
		public bool DeletePhoto()
		{
			try
			{
				File.Delete(HttpContext.Current.Server.MapPath(Path.Combine("~", PhotoPath)));
				File.Delete(HttpContext.Current.Server.MapPath(Path.Combine("~", ThumbnailPath)));
				return true;
			} catch(Exception)
			{
				return false;
			}
		}
	}
}