﻿using System;
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
        static HomePageModel homePageModel = new HomePageModel(configModel.OutputDirectory);
		
		[HttpGet]
		public ActionResult Config()
		{
			return View(configModel);
		}

        [HttpGet]
        public ActionResult Logs()
        {
            return View(logModel);
        }

        [HttpGet]
        public ActionResult HomePage()
        {
            return View(homePageModel);
        }

        /*[HttpGet]
        public ActionResult FilterLog(string type)
        {
            
            logModel.LeaveLogType(type);
            return RedirectToAction("Logs");
        }*/

        [HttpGet]
		public ActionResult DeleteHandler(string handler)
		{
			//IS THIS A GOOD IDEA TO PUT IT IN configModel?
			configModel.DirToRemove = handler;
			return View(configModel);
		}

		[HttpGet]
		public ActionResult DeleteHandlerExecution()
		{
			//bool result;
			//string msg = configModel.DeleteDirRequest(out result);
			//if (result  == true)
			//{
			//	return RedirectToAction("Config", configModel);
			//} else
			//{
			//	ErrorModel errorModel = new ErrorModel(msg);
			//	return RedirectToAction("Error", errorModel);
			//}
			configModel.DeleteDirRequest();
			return RedirectToAction("Config", configModel);

		}

		[HttpGet]
		public ActionResult Thumbnails()
		{
			//make thumbnails Model static member of controller?
			ThumbnailsModel thumbnailsModel = new ThumbnailsModel(configModel.OutputDirectory);
			return View(thumbnailsModel);
		}

		[HttpGet]
		public ActionResult Photo(string path, string date)
		{
			PhotoModel photoModel = new PhotoModel(path, date);
			return View(photoModel);
		}

		[HttpGet]
		public ActionResult DeletePhoto(string thumbnailPath, string date)
		{
			PhotoModel photoModel = new PhotoModel(thumbnailPath, date);
			return View(photoModel);
		}

		[HttpGet]
		public ActionResult DeletePhotoExecution(string thumbnailPath, string date)
		{
			PhotoModel photoModel = new PhotoModel(thumbnailPath, date);
			photoModel.DeletePhoto();
			ThumbnailsModel thumbnailsModel = new ThumbnailsModel(configModel.OutputDirectory);
			return RedirectToAction("Thumbnails", thumbnailsModel);
		}

		[HttpGet]
		public ActionResult CancelDeletePhoto()
		{
			ThumbnailsModel thumbnailsModel = new ThumbnailsModel(configModel.OutputDirectory);
			return RedirectToAction("Thumbnails", thumbnailsModel);
		}

		
		[HttpGet]
		public ActionResult Error(ErrorModel errorModel)
		{
			//add model with error message
			return View(errorModel);
		}
    }
}