using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandInfrastructure.Commands;
using CommandInfrastructure.Commands.Enums;
using System.Windows.Input;
using ImageServiceProgram.TcpServer;
using System.ServiceProcess;
using System.ComponentModel;

namespace ImageServiceProgram.Commands
{
    class GetStatusCommand : ICommand
    {
        private IImageServer server;

        public GetStatusCommand(IImageServer server)
        {
            this.server = server;
        }

        public string Execute(string[] args, out bool result)
        {
            string stat = "";
            string[] stats = { stat };
            try
            {
                {
                    result = true;
                    switch(new ServiceController("ImageService").Status)
                    {
                        case (ServiceControllerStatus.Running):
                            stat = "running";
                            break;
                        case (ServiceControllerStatus.Stopped):
                            stat = "stopped";
                            break;
                        case (ServiceControllerStatus.Paused):
                            stat = "paused";
                            break;
                        default:
                            stat = "default";
                            break;
                    }
                    //rest of args for commandReceivedEventArgs
                    int id = (int)CommandEnum.GetStatusCommand;
                    string requestDirPath = "";
                    CommandReceivedEventArgs arg = new CommandReceivedEventArgs(id, stats, requestDirPath);
                    //send client the log history command and return result
                    return server.SendClientCommand(int.Parse(args[1]), arg, out result);

                }
            }
            catch (ArgumentException) { result = false;  return "could not get status"; }
            catch (Win32Exception) { result = false;  return "could not get status"; }
        }
    }
}
