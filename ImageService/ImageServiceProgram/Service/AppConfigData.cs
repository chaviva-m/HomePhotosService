using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceProgram.Service
{
    public sealed class AppConfigData
    {

        private static readonly AppConfigData instance = new AppConfigData();
        public static AppConfigData Instance
        {
            get { return instance; }
        }

        private string outputDir = ConfigurationManager.AppSettings["OutputDir"];
        public string OutputDir
        {
            get { return outputDir; }
        }

        private string eventSourceName = ConfigurationManager.AppSettings["SourceName"];
        public string EventSourceName
        {
            get { return eventSourceName; }
        }

        private string logName = ConfigurationManager.AppSettings["LogName"];
        public string LogName
        {
            get { return logName; }
        }

        private int thumbnailSize = Int32.Parse(ConfigurationManager.AppSettings["ThumbnailSize"]);
        public int ThumbnailSize
        {
            get { return thumbnailSize; }
        }

        private string[] directories = ConfigurationManager.AppSettings["Handler"].Split(';');
        public string[] Directories
        {
            get { return directories; }
        }
    }
}
