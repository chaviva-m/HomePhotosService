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

		public PhotoModel(string thumbnailPath, string photoDate)
		{
			ThumbnailPath = thumbnailPath;
			PhotoPath = FindPhotoPath(thumbnailPath);
			Date = photoDate;
		}

		private string FindPhotoPath(string thumbnailPath)
		{
			string thumbnailDir = "Thumbnails";
			string photoPath = thumbnailPath.Replace(thumbnailDir, "");
			return photoPath;
		}

		public void DeletePhoto()
		{
			
			File.Delete(HttpContext.Current.Server.MapPath(Path.Combine("~", PhotoPath)));
			File.Delete(HttpContext.Current.Server.MapPath(Path.Combine("~", ThumbnailPath)));
		}
	}
}