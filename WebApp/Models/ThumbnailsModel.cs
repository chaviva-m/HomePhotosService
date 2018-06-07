using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
	public class ThumbnailsModel
	{
		private string outputDir;
		private string thumbnailsDir;

		public Dictionary<string, string> Thumbnails { get { return ThumbnailsList(); } }
	
		public ThumbnailsModel(string outputDir)
		{
			//string relativePath = AbsolutePath.Replace(HttpContext.Current.Server.MapPath("~/"), "~/").Replace(@"\", "/");
			//string relativePath = HttpContext.Current.Request.Url.AbsolutePath.Replace(HttpContext.Current.Server.MapPath("~/"), "~/").Replace(@"\", "/");
			//this.outputDir = Path.Combine(relativePath, Path.GetFileName(outputDir));
			//this.outputDir = Path.Combine("~", Path.GetFileName(outputDir)).Replace(@"\", "/");
			//this.thumbnailsDir = Path.Combine(this.outputDir, "Thumbnails").Replace(@"\", "/");
			this.outputDir = outputDir;
			this.thumbnailsDir = Path.Combine(this.outputDir, "Thumbnails");
			//this.outputDir = Path.Combine("~", Path.GetFileName(outputDir));
			//this.thumbnailsDir = Path.Combine(this.outputDir, "Thumbnails");
			
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
			string solutionDir = Directory.GetParent(currentDir).Parent.FullName;
			string relativePath = absolutePath.Replace(solutionDir, "");
			return relativePath;
		}
	}
}