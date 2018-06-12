using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public interface IPhotoModel
    {
        string ThumbnailPath { get; }
        string PhotoPath { get; }
        string Date { get; set; }
        bool DeletePhoto();
    }
}