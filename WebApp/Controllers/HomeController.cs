using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WebApp.CommandInfrastructure;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
		static ConfigModel configModel = new ConfigModel();
        static LogModel logModel = new LogModel();
        static HomePageModel homePageModel = new HomePageModel();
		
		/// <summary>
		/// config view
		/// </summary>
		/// <returns>returns config view</returns>
		[HttpGet]
		public ActionResult Config()
		{
			return View(configModel);
		}

		/// <summary>
		/// log view
		/// </summary>
		/// <returns>return log view</returns>
        [HttpGet]
        public ActionResult Logs()
		{ 
            return View(logModel);
        }

		/// <summary>
		/// home page view
		/// </summary>
		/// <returns>return home page view</returns>
        [HttpGet]
        public ActionResult HomePage()
        {
			homePageModel.Refresh(configModel.OutputDirectory);
            return View(homePageModel);
        }

		/// <summary>
		/// delete handler validation view
		/// </summary>
		/// <param name="handler">handler to delete</param>
		/// <returns>return delete handler validation view</returns>
        [HttpGet]
		public ActionResult DeleteHandler(string handler)
		{
			configModel.DirToRemove = handler;
			return View(configModel);
		}

		/// <summary>
		/// delete requested handler
		/// </summary>
		/// <returns>on success, return config view. on error, return error view</returns>
		[HttpGet]
		public ActionResult DeleteHandlerExecution()
		{
			bool result;
			string msg = configModel.DeleteDirRequest(out result);
			if (result  == true)
			{
				return RedirectToAction("Config", configModel);
			} else
			{
				ErrorModel errorModel = new ErrorModel(msg);
				return RedirectToAction("Error", errorModel);
			}
		}

		/// <summary>
		/// thumbnails view
		/// </summary>
		/// <returns>return thumbnails view</returns>
		[HttpGet]
		public ActionResult Thumbnails()
		{
			ThumbnailsModel thumbnailsModel = new ThumbnailsModel(configModel.OutputDirectory);
			return View(thumbnailsModel);
		}

		/// <summary>
		/// return view of photo
		/// </summary>
		/// <param name="path">path to thumbnail of photo</param>
		/// <param name="date">date of photo</param>
		/// <returns>return photo view</returns>
		[HttpGet]
		public ActionResult Photo(string path, string date)
		{
			PhotoModel photoModel = new PhotoModel(path, date);
			return View(photoModel);
		}

		/// <summary>
		/// delete photo validation view
		/// </summary>
		/// <param name="thumbnailPath">path to thumbnail of photo</param>
		/// <param name="date">date of photo</param>
		/// <returns>return delete photo validation view</returns>
		[HttpGet]
		public ActionResult DeletePhoto(string thumbnailPath, string date)
		{
			PhotoModel photoModel = new PhotoModel(thumbnailPath, date);
			return View(photoModel);
		}

		/// <summary>
		/// delete requested photo
		/// </summary>
		/// <param name="thumbnailPath">path to thumbnail of photo</param>
		/// <param name="date">date of photo</param>
		/// <return>return thumbnails view on success, else return error</returns>
		[HttpGet]
		public ActionResult DeletePhotoExecution(string thumbnailPath, string date)
		{
			PhotoModel photoModel = new PhotoModel(thumbnailPath, date);
			bool result = photoModel.DeletePhoto();
			if (result == true)
			{
				ThumbnailsModel thumbnailsModel = new ThumbnailsModel(configModel.OutputDirectory);
				return RedirectToAction("Thumbnails", thumbnailsModel);
			} else
			{
				ErrorModel errorModel = new ErrorModel("error deleting photo");
				return RedirectToAction("Error", errorModel);
			}
		}

		/// <summary>
		/// cancel photo deletion
		/// </summary>
		/// <returns>return thumbnails view</returns>
		[HttpGet]
		public ActionResult CancelDeletePhoto()
		{
			ThumbnailsModel thumbnailsModel = new ThumbnailsModel(configModel.OutputDirectory);
			return RedirectToAction("Thumbnails", thumbnailsModel);
		}
		
		/// <summary>
		/// error view
		/// </summary>
		/// <param name="errorModel">error model with error message</param>
		/// <returns>return error view</returns>
		[HttpGet]
		public ActionResult Error(ErrorModel errorModel)
		{
			return View(errorModel);
		}
    }
}