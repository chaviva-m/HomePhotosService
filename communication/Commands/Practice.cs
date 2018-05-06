using Communication.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace communication.Commands
{
    class Practice
    {
        public void practice(CommandReceivedEventArgs cmdArgs)
        {
            string output = JsonConvert.SerializeObject(cmdArgs);
            Console.WriteLine(output);

            CommandReceivedEventArgs deserializedProduct = JsonConvert.DeserializeObject<CommandReceivedEventArgs>(output);
            Console.WriteLine("des: ");
            Console.WriteLine("id: " + deserializedProduct.CommandID);
            Console.WriteLine("args: " + deserializedProduct.Args[0] + ", " + deserializedProduct.Args[1]);
            Console.WriteLine("path: " + deserializedProduct.RequestDirPath);
        }
    }
}
