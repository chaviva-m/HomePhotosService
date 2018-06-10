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
			configModel.DeleteDirRequest();
			//HOW DO WE KNOW WHEN IT HAS BEEN DELETED??
			//SHOULD I USE FUTURE?
			//SHOULD THIS FUNCTION REDIRECT?
			return RedirectToAction("Config", configModel);

		}

		public ActionResult Thumbnails()
		{
			ThumbnailsModel thumbnailsModel = new ThumbnailsModel(configModel.OutputDirectory);
			return View(thumbnailsModel);
		}

        
		
    }
}