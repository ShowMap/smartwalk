﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.Logging;
using Orchard.Themes;
using SmartWalk.Server.Services.ImportService;

namespace SmartWalk.Server.Controllers
{
    [HandleError, Themed]
    public class ToolsController : Controller {

        private readonly IImportService _importService;
        private readonly IOrchardServices _orchardServices;

        public ILogger Logger { get; set; }


        public ToolsController(IImportService importService, IOrchardServices orchardServices) {
            _importService = importService;

            _orchardServices = orchardServices;

            Logger = NullLogger.Instance;
        }

        public ActionResult ImportXmlData()
        {
            return View();
        }

        [Themed]
        public ActionResult ImportXmlDataAction()
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
                return new HttpUnauthorizedResult();

            var log = new List<string>();

            try
            {
                _importService.ImportXmlData(log);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex, "Error occured while importing XML data");
                return new ContentResult { Content = GetContent(log, ex.Message) };
            }

            return new ContentResult { Content = GetContent(log) };
        } 

        private static string GetContent(IEnumerable<string> log, string error = null)
        {
            return log.Aggregate((i, j) => i + "<br/>" + j) +
                   (error != null ? ("<br/>Error: " + error) : "<br/>Import completed!");
        }
    }
}