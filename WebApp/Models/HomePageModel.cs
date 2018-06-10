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
        private List<Name> names = new List<Name>();
        public List<Name> Names { get { return names; } set { names = value; } }
        string outputDir;

        public HomePageModel()
		{
            getDetails();
        }

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

        private void getNumPics()
        {
            try
            {
                string currentDir = HttpContext.Current.Server.MapPath("~");
                numPics = System.IO.Directory.GetFiles(currentDir, "*.jpg", SearchOption.AllDirectories).Count() / 2;
            }
            //what to do in case of exception??
            catch (Exception e)
            {
                numPics = 999;
            }
        }

        private void getDetails()
        {
            string[] words;
            string name;
            int id;
            Name n;
            string line;
            names.Add(new Models.Name("chaviva", 99));
            /*try
            {   // Open the text file using a stream reader.
                StreamReader sr = new StreamReader("details.txt");
                {
                        // Read the stream to a string, and write the string to the console.
                        while ((line = sr.ReadLine()) != null)
                        {
                            words = line.Split(' ');
                            name = words[0] += words[1];
                            id = int.Parse(words[2]);
                            this.names.Add(new Name(name, id));
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }*/
        }

    }
}