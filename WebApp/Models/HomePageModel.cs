using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WebApp.CommandInfrastructure;
using WebApp.Models;

namespace WebApp.Models
{
    public class HomePageModel
    {
        clientChannel clientChannel = clientChannel.Instance;
        
        private string status;
        public string Status { get { return status; } set { status = value; } }
        private int numPics;
        public int NumPics { get { return numPics; } set { numPics = value; } }
        private List<Student> names = new List<Student>();
        public List<Student> Names { get { return names; } set { names = value; } }
        string outputDir;

		/// <summary>
		/// constructor. read details from disk.
		/// </summary>
        public HomePageModel()
		{
            getDetails();
        }

		/// <summary>
		/// refresh home page - get service status from server, and count pictures in directory.
		/// </summary>
		/// <param name="outputDir">pictures output directory path</param>
		public void Refresh(string outputDir)
		{
			CommandReceivedEventArgs cmdArgs;
			string[] args = { "" };
			clientChannel.SendCommand(new CommandReceivedEventArgs((int)CommandEnum.GetStatusCommand, args, ""));
			cmdArgs = clientChannel.ReadCommand();
			if (cmdArgs != null)
			{
				Status = cmdArgs.Args[0];
			}
			this.outputDir = outputDir;
			getNumPics();
		}

		/// <summary>
		/// count pictures in output directory
		/// </summary>
        private void getNumPics()
        {
            try
            {
                string thumbnailsDir = Path.Combine(outputDir, "Thumbnails");
                numPics = System.IO.Directory.GetFiles(thumbnailsDir, "*.jpg", SearchOption.AllDirectories).Count() +
                    System.IO.Directory.GetFiles(thumbnailsDir, "*.png", SearchOption.AllDirectories).Count()  +
                    System.IO.Directory.GetFiles(thumbnailsDir, "*.gif", SearchOption.AllDirectories).Count() +
                    System.IO.Directory.GetFiles(thumbnailsDir, "*.bmp", SearchOption.AllDirectories).Count();
            }
            catch (Exception)
            {
                numPics = 0;
            }
        }

		/// <summary>
		/// read student details from file.
		/// </summary>
        private void getDetails()
        {
            string[] words;
            string name;
            int id;
            string line;
            string currentDir = HttpContext.Current.Server.MapPath("~");
            string addr = Path.Combine(currentDir, "details.txt");
            try
            {   // Open the text file using a stream reader.
                StreamReader sr = new StreamReader(@addr);
                {
                    
                    // Read the stream to a string, and write the string to the console.
                    while ((line = sr.ReadLine()) != null)
                        {
                            words = line.Split(' ');
                            name = words[0] + " " + words[1];
                            id = int.Parse(words[2]);
                            this.names.Add(new Student(name, id));
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

    }
}