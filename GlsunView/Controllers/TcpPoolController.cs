using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.CommService;
using GlsunView.Models;

namespace GlsunView.Controllers
{
    public class TcpPoolController : Controller
    {
        // GET: TcpPool
        public ActionResult Index()
        {
            List<TcpPoolInfo> info = new List<TcpPoolInfo>();
            foreach(var item in TcpClientServicePool.ServiceSet)
            {
                var itemInfo = new TcpPoolInfo();
                itemInfo.Client = item.Key;
                var list = TcpClientServicePool.ServiceSet[item.Key];
                if(list != null)
                {
                    itemInfo.Count = list.Count;
                    itemInfo.BusyCount = (from l in list
                                          where l.IsBusy == true
                                          select l).Count();
                    itemInfo.FreeCount = itemInfo.Count - itemInfo.BusyCount;
                }
                else
                {
                    itemInfo.Count = 0;
                    itemInfo.BusyCount = 0;
                    itemInfo.FreeCount = 0;
                }
                info.Add(itemInfo);
            }

            return View(info);
        }
    }
}