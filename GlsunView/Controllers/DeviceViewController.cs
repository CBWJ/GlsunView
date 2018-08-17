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
    public class DeviceViewController : Controller
    {
        // GET: DeviceView
        public ActionResult Index()
        {
            List<MachineTreeNode> nodes = new List<MachineTreeNode>();
            int nodeId = 1;
            using(var ctx = new GlsunViewEntities())
            {
                foreach(var room in ctx.MachineRoom)
                {
                    //机房
                    var roomNode = new MachineTreeNode
                    {
                        DataID = room.ID,
                        Name = room.MRName,
                        ID = nodeId++,
                        PID = 0,
                        Open = true,
                        Icon = room.MRIcon
                    };
                    if (string.IsNullOrWhiteSpace(roomNode.Icon))
                        roomNode.Icon = "structural.png";
                    nodes.Add(roomNode);
                    //机架
                    foreach(var shelf in ctx.MachineShelf.Where(s => s.MRID == room.ID))
                    {
                        var shelfNode = new MachineTreeNode
                        {
                            DataID = shelf.ID,
                            Name = shelf.MSName,
                            ID = nodeId++,
                            PID = roomNode.ID,
                            Open = false,
                            Icon = shelf.MSIcon
                        };
                        if (string.IsNullOrWhiteSpace(shelfNode.Icon))
                            shelfNode.Icon = "cabinet.png";
                        nodes.Add(shelfNode);
                        //机框
                        foreach(var frame in ctx.MachineFrame.Where(f => f.MSID == shelf.ID))
                        {
                            var frameNode = new MachineTreeNode
                            {
                                DataID = frame.ID,
                                Name = frame.MFName,
                                ID = nodeId++,
                                PID = shelfNode.ID,
                                Open = false,
                                Icon = frame.MFIcon
                            };
                            if (string.IsNullOrWhiteSpace(frameNode.Icon))
                                frameNode.Icon = "wacom.png";
                            nodes.Add(frameNode);
                        }
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
        /// 机房列表
        /// </summary>
        /// <returns></returns>
        public ActionResult RoomList()
        {
            List<MachineRoom> rooms = null;
            using(var ctx = new GlsunViewEntities())
            {
                rooms = ctx.MachineRoom.OrderBy(r => r.ID).ToList();
            }
            return View(rooms);
        }
    }
}