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
	
		/// <summary>
		/// construct thumbnails model with thumbnails directory path
		/// </summary>
		/// <param name="outputDir">path to output directory (parent of thumbnails directory)</param>
		public ThumbnailsModel(string outputDir)
		{
			try
			{
				this.thumbnailsDir = Path.Combine(outputDir, "Thumbnails");
			} catch (Exception)
			{
				this.thumbnailsDir = "";
			}
		}

		/// <summary>
		/// get all thumbnails in thumbnails folder
		/// </summary>
		/// <returns>dictionary of thumbnails and their dates</returns>
		private Dictionary<string, string> ThumbnailsList()
		{
			Dictionary<string, string> allThumbnailsMap = new Dictionary<string, string>();
			try
			{
				string[] allThumbnails = Directory.GetFiles(thumbnailsDir, "*.*", SearchOption.AllDirectories);
				foreach (string thumbnail in allThumbnails)
				{
					string relativePath = RelativePath(thumbnail);
					string month = Directory.GetParent(relativePath).Name;
					string year = Directory.GetParent(relativePath).Parent.Name;
					string date = Path.Combine(year, month);
					allThumbnailsMap.Add(relativePath, date);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("couldn't get thumbnails from thumbnails directory:");
				Console.WriteLine(e.Message);
			}
			return allThumbnailsMap;
		}

		/// <summary>
		/// get relative path of thumbnail
		/// </summary>
		/// <param name="absolutePath">the absolute path of the thumbnail</param>
		/// <returns></returns>
		private string RelativePath(string absolutePath)
		{
			string relativePath;
			try
			{
				string currentDir = HttpContext.Current.Server.MapPath("~");
				string solutionDir = Directory.GetParent(currentDir).FullName;
				relativePath = absolutePath.Replace(solutionDir, "");
			} catch (Exception)
			{
				relativePath = "";
			}
			return relativePath;
		}
	}
}