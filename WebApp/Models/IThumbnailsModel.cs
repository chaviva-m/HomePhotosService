using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public interface IThumbnailsModel
    {
        Dictionary<string, string> Thumbnails { get; }
            
    }
}
