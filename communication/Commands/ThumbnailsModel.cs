using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandInfrastructure.Commands
{
	public class ThumbnailsModel
	{
		private string outputDir;
		private string thumbnailsDir;

		public Dictionary<string, string> Thumbnails { get { return ThumbnailsList(); } }

		public ThumbnailsModel(string outputDir)
		{
			this.outputDir = outputDir;
			this.thumbnailsDir = Path.Combine(outputDir, "Thumbnails");
		}

		private Dictionary<string, string> ThumbnailsList()
		{
			string[] allThumbnails = Directory.GetFiles(thumbnailsDir, "*.*", SearchOption.AllDirectories);
			Dictionary<string, string> allThumbnailsMap = new Dictionary<string, string>();
			foreach (string thumbnail in allThumbnails)
			{
				string month = Directory.GetParent(thumbnail).Name;
				string year = Directory.GetParent(thumbnail).Parent.Name;
				string date = Path.Combine(month, year);
				allThumbnailsMap.Add(thumbnail, date);
			}
			return allThumbnailsMap;
		}
	}
}
