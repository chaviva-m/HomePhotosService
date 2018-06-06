
using CommandInfrastructure.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommandInfrastructure
{
    class Program
    {
        static void Main(string[] args)
        {
			ThumbnailsModel tm = new ThumbnailsModel(@"C: \Users\Acer\Pictures\outputDirectory");
			Dictionary<string, string> all_tn = tm.Thumbnails;
			foreach(string file in all_tn.Keys)
			{
				Console.WriteLine(all_tn[file]);
			}
			int c = all_tn.Count();
			Console.WriteLine(c);
        }
    }
}
