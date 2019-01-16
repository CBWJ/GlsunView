using GlsunView.Common;
using GlsunView.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlsunView.Controllers
{
    public class LisenceController : Controller
    {
        [AllowAnonymous]
        public ActionResult Exception()
        {
            ViewBag.MachineNumber = LisenceHelper.LocalMachineNumber;
            return View((object)LisenceHelper.Message);
        }
    }
}