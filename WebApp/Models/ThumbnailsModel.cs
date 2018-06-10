using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
	public class ThumbnailsModel
	{
		private string thumbnailsDir;

		public Dictionary<string, string> Thumbnails { get { return ThumbnailsList(); } }
	
		public ThumbnailsModel(string outputDir)
		{
			this.thumbnailsDir = Path.Combine(outputDir, "Thumbnails");			
		}

		private Dictionary<string, string> ThumbnailsList()
		{
			string[] allThumbnails = Directory.GetFiles(thumbnailsDir, "*.*", SearchOption.AllDirectories);
			Dictionary<string, string> allThumbnailsMap = new Dictionary<string, string>();
			foreach (string thumbnail in allThumbnails)
			{
				string relativePath = RelativePath(thumbnail);
				string month = Directory.GetParent(relativePath).Name;
				string year = Directory.GetParent(relativePath).Parent.Name;
				string date = Path.Combine(year, month);
				allThumbnailsMap.Add(relativePath, date);
			}
			return allThumbnailsMap;
		}

		private string RelativePath(string absolutePath)
		{
			string currentDir = HttpContext.Current.Server.MapPath("~");
			string solutionDir = Directory.GetParent(currentDir).FullName;
			string relativePath = absolutePath.Replace(solutionDir, "");
			return relativePath;
		}
	}
}