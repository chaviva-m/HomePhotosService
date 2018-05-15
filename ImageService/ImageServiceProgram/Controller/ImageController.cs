using ImageServiceProgram.Commands;
using Communication.Commands;
using Communication.Commands.Enums;
using ImageServiceProgram.ImageModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageServiceProgram.Controller
{
    public class ImageController : IImageController
    {
        private Dictionary<int, ICommand> Commands;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="modal">image service modal</param>
        public ImageController(Dictionary<int, ICommand> commands)
        {
            this.Commands = commands;
        }

        /// <summary>
        /// command execution
        /// </summary>
        /// <param name="commandID">unique command id</param>
        /// <param name="args">argument</param>
        /// <param name="result">true- if successful, false otherwise</param>
        /// <returns></returns>
        public string ExecuteCommand(int commandID, string[] args,out bool result) 
        {

            //Task<Tuple<bool,string>> execution = new Task< Tuple < bool,string>> (() => {
            string str;
                //bool res;
                if (Commands.ContainsKey(commandID))
                {
                    result = true;
                    str = Commands[commandID].Execute(args, out result);
                    //return new Tuple<bool, string>(res,str);
                }
                else
                {

                    result = false;
                    str = "command not found";
                    //return new Tuple<bool, string>(res, str);
                }

            //});
            //execution.Start();
            //var tuple = execution.Result;
            //result = tuple.Item1;
            //return tuple.Item2;
            return str;
        }

    }
}
