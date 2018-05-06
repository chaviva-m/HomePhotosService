using ImageServiceProgram.Communication;
using ImageServiceProgram.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceProgram.Commands
{
    class GetConfigCommand : ICommand
    {
        private IImageServer server;


        public GetConfigCommand(IImageServer server)
        {
            this.server = server;
        }

        public string Execute(string[] args, out bool result)
        {
            AppConfigData clientChannel = AppConfigData.Instance;


        }
    }
}
