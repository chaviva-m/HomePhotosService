﻿using System;
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

        public HomePageModel(string dir)
        {
            //add methods to client channel's event
            clientChannel.CommandReceived += getStatus;
            getNumPics();
            getDetails();
            this.outputDir = dir;

        }

        private void getStatus(object sender, CommandInfrastructure.CommandReceivedEventArgs cmdArgs)
        {
            if (cmdArgs.CommandID == (int)CommandEnum.LogHistoryCommand)
            {
                //iterate over array and add logs to LogMessages type, message
                Status = cmdArgs.Args[0];

            }
        }

        private void getNumPics()
        {
            try
            {
                string currentDir = HttpContext.Current.Server.MapPath("~");
                string thumbnailsDir = Path.Combine(outputDir, "Thumbnails");
                numPics = System.IO.Directory.GetFiles(thumbnailsDir, "*.jpg", SearchOption.AllDirectories).Count() +
                    System.IO.Directory.GetFiles(thumbnailsDir, "*.png", SearchOption.AllDirectories).Count()  +
                    System.IO.Directory.GetFiles(thumbnailsDir, "*.gif", SearchOption.AllDirectories).Count() +
                    System.IO.Directory.GetFiles(thumbnailsDir, "*.bmp", SearchOption.AllDirectories).Count();
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
                            this.names.Add(new Name(name, id));
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