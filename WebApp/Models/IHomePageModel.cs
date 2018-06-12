using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public interface IHomePageModel
    {
        string Status { get; set; }
        List<Student> Students { get; set; }
        int NumPics { get; set; }
        void Refresh(string outputDir);
    }
}