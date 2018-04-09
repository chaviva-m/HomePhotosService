using ImageServiceProgram.Commands;
using ImageServiceProgram.Infrastructure;
using ImageServiceProgram.Infrastructure.Enums;
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
        private IImageServiceModal Modal;                      // The Modal Object
        private Dictionary<int, ICommand> Commands;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="modal">image service modal</param>
        public ImageController(IImageServiceModal modal)
        {
            this.Modal = modal;                    // Storing the Modal Of The System
            Commands = new Dictionary<int, ICommand>()
            {
                {(int)CommandEnum.NewFileCommand, new NewFileCommand(this.Modal) }
            };
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

            Task<Tuple<bool,string>> execution = new Task< Tuple < bool,string>> (() => {

                bool res;
                if (Commands.ContainsKey(commandID))
                {
                    res = true;
                    string str = Commands[commandID].Execute(args, out res);
                    return new Tuple<bool, string>(res,str);

                }
                else
                {

                    res = false;
                    string str = "command not found";
                    return new Tuple<bool, string>(res, str);
                }
                
            });
            execution.Start();
            var tuple = execution.Result;
            result = tuple.Item1;
            return tuple.Item2;
        }

    }
}
