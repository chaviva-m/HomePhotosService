using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Models
{
    interface IConfigModel
    {
        string OutputDirectory { get; set; }
        string SourceName { get;  }
        string LogName { get; }
        string ThumbnailSize { get;  }
        List<string> Directories { get; }
        string DirToRemove { get; set; }
        string DeleteDirRequest(out bool result);
    }
}
