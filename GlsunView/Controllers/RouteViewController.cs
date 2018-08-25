using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.Models;
using System.Text;

namespace GlsunView.Controllers
{
    public class RouteViewController : Controller
    {
        // GET: RouteView
        public ActionResult Index()
        {
            List<MachineTreeNode> nodes = new List<MachineTreeNode>();
            int nodeId = 1;
            string iconPath = "../../image/route/";
            using (var ctx = new GlsunViewEntities())
            {
                foreach(var g in ctx.RouteGroup)
                {
                    //路由组
                    var groupNode = new MachineTreeNode
                    {
                        DataID = g.ID,
                        Name = g.RGName,
                        ID = nodeId++,
                        PID = 0,
                        Open = true,
                        Icon = g.RGIcon,
                        IconPath = iconPath
                    };
                    if (string.IsNullOrWhiteSpace(groupNode.Icon))
                        groupNode.Icon = "group.png";
                    nodes.Add(groupNode);
                    //路由
                    foreach(var r in ctx.Route.Where(r => r.RGID == g.ID))
                    {
                        var routeNode = new MachineTreeNode
                        {
                            DataID = r.ID,
                            Name =r.RName,
                            ID = nodeId++,
                            PID = groupNode.ID,
                            Open = false,
                            Icon = "route.png",
                            IconPath = iconPath
                        };
                        nodes.Add(routeNode);
                    }
                }
            }
            StringBuilder sbText = new StringBuilder();
            sbText.Append("[");
            int cnt = 0;
            foreach (var e in nodes)
            {
                if (cnt == 0)
                {
                    sbText.Append(e.ToJSONObject());
                }
                else
                {
                    sbText.AppendFormat(",{0}", e.ToJSONObject());
                }
                cnt++;
            }
            sbText.Append("]");
            ViewBag.TreeNodes = sbText.ToString();
            return View();
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        public ActionResult RouteGroupList()
        {
            List<RouteGroup> groups = null;
            using (var ctx = new GlsunViewEntities())
            {
                groups = ctx.RouteGroup.OrderBy(r => r.ID).ToList();
            }
            return View(groups);
        }
    }
}