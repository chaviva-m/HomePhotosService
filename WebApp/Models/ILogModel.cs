using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Models
{
    interface ILogModel
    {
        List<Log> LogMessages { get; set; }
    }
}
