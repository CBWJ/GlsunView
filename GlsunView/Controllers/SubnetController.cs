using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.Models;
using System.IO;
using GlsunView.Infrastructure.Util;
using GlsunView.Infrastructure.Concrete;
using System.Web.Script.Serialization;

namespace GlsunView.Controllers
{
    public class SubnetController : ShareListController
    {
        private TopologyLogger _topoLogger = new TopologyLogger();
        // GET: Subnet
        public ActionResult Index(int id)
        {
            IEnumerable<TopologyNode> nodes = null;
            IEnumerable<TopologyLine> lines = null;
            SceneLocation sceneLoc = null;
            using (var ctx = new GlsunViewEntities())
            {
                var subnet = ctx.Subnet.Find(id);
                nodes = (from d in ctx.Device
                        where d.SID == id
                        select new TopologyNode
                        {
                            ID = d.ID,
                            Name = d.DName,
                            Address = d.DAddress,
                            Icon = d.DIcon,
                            X = d.CoordinateX.Value,
                            Y = d.CoordinateY.Value
                        }).ToList();
                var nodeIds = (from n in nodes
                               select n.ID).ToList();
                lines = (from l in ctx.DeviceLine
                        where nodeIds.Contains(l.DIDA.Value) || nodeIds.Contains(l.DIDB.Value)
                        select new TopologyLine
                        {
                            ID = l.ID,
                            Name = l.DLName,
                            NodeIDA = l.DIDA.Value,
                            NodeIDZ = l.DIDB.Value
                        }).ToList();
                sceneLoc = (from l in ctx.SceneLocation
                            where l.SceneType == "device" && l.UserID == id
                            select l).FirstOrDefault();
                if (sceneLoc == null)
                {
                    sceneLoc = new SceneLocation
                    {
                        CoordinateX = 0,
                        CoordinateY = 0,
                        SceneType = "device",
                        UserID = id
                    };
                    ctx.SceneLocation.Add(sceneLoc);
                    ctx.SaveChanges();
                }
            }
            ViewBag.Nodes = new JavaScriptSerializer().Serialize(nodes);
            ViewBag.Lines = new JavaScriptSerializer().Serialize(lines);
            ViewBag.SceneLocation = new JavaScriptSerializer().Serialize(sceneLoc);
            ViewBag.SubnetId = id;
            SetAuthorityData();
            return View();
        }

        // GET: Subnet/Details/5
        public ActionResult Details(int id)
        {
            Subnet model = null;
            using (var ctx = new GlsunViewEntities())
            {
                model = (from s in ctx.Subnet
                         where s.ID == id
                         select s).FirstOrDefault();
            }
            ViewBag.Action = "Details";
            return View("Create", model);
        }

        // GET: Subnet/Create
        public ActionResult Create()
        {
            Subnet net = new Subnet();
            net.CoordinateX = 100;
            net.CoordinateY = 100;
            ViewBag.Action = "Create";
            return View(net);
        }

        // POST: Subnet/Create
        [HttpPost]
        public ActionResult Create(Subnet subnet, HttpPostedFileBase iconFile)
        {
            var json = new JsonResult();
            User loginUser = null;
            try
            {
                string iconFileName = "";
                if(iconFile != null)
                {
                    iconFileName = SaveIcon(iconFile);
                }
                using (var ctx = new GlsunViewEntities())
                {
                    loginUser = (from u in ctx.User
                                     where u.ULoginName == HttpContext.User.Identity.Name
                                     select u).FirstOrDefault();
                    var newSubnet = subnet.CopyProperty();
                    newSubnet.CreatorID = loginUser.ID;
                    newSubnet.CreationTime = DateTime.Now;
                    if (string.IsNullOrWhiteSpace(newSubnet.SIcon))
                    {
                        newSubnet.SIcon = iconFileName;
                    }
                    ctx.Subnet.Add(newSubnet);
                    ctx.SaveChanges();
                    subnet.ID = newSubnet.ID;
                }
                json.Data = new { Code = "", Data = subnet, Message = "保存成功" };
                //日志记录
                _topoLogger.Record(loginUser, "添加子网", "", "成功", "", subnet.ID, subnet.SName, "子网");
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = subnet, Message = ex.Message };
                //日志记录
                _topoLogger.Record(loginUser, "添加子网", "", "失败", string.Format("发生异常：{0}", ex.Message), subnet.ID, subnet.SName, "子网");
            }
            return json;
        }
        private string SaveIcon(HttpPostedFileBase iconFile)
        {
            var iconPath = Server.MapPath("~/image/topo");
            DirectoryInfo directoryInfo = new DirectoryInfo(iconPath);
            var imgFiles = directoryInfo.GetFiles();
            var subnetFiles = (from f in imgFiles
                               where f.Name.ToLower().Contains("subnet")
                               select f).ToList();
            var fileType = iconFile.FileName.Substring(iconFile.FileName.IndexOf(".") + 1);
            var fileName = "subnet" + (subnetFiles.Count + 100).ToString() + "." + fileType;
            var filePath = Path.Combine(iconPath, fileName);
            byte[] byteIcon = new byte[iconFile.ContentLength];
            iconFile.InputStream.Read(byteIcon, 0, byteIcon.Length);
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                fs.Write(byteIcon, 0, byteIcon.Length);
            }
            return fileName;
        }
        // GET: Subnet/Edit/5
        public ActionResult Edit(int id)
        {
            Subnet model = null;
            using (var ctx = new GlsunViewEntities())
            {
                model = (from s in ctx.Subnet
                         where s.ID == id
                         select s).FirstOrDefault();
            }
            ViewBag.Action = "Edit";
            return View("Create", model);
        }

        // POST: Subnet/Edit/5
        [HttpPost]
        public ActionResult Edit(Subnet net, HttpPostedFileBase iconFile)
        {
            var json = new JsonResult();
            User loginUser = null;
            try
            {
                string iconFileName = "";
                if (iconFile != null)
                {
                    iconFileName = SaveIcon(iconFile);
                }
                using (var ctx = new GlsunViewEntities())
                {
                    loginUser = (from u in ctx.User
                                     where u.ULoginName == HttpContext.User.Identity.Name
                                     select u).FirstOrDefault();
                    var subnetEdit = (from s in ctx.Subnet
                                      where s.ID == net.ID
                                      select s).FirstOrDefault();
                    subnetEdit.SName = net.SName;
                    subnetEdit.SAddress = net.SAddress;
                    subnetEdit.SIcon = net.SIcon;
                    subnetEdit.CoordinateX = net.CoordinateX;
                    subnetEdit.CoordinateY = net.CoordinateY;
                    subnetEdit.Remark = net.Remark;
                    subnetEdit.EditorID = loginUser.ID;
                    subnetEdit.EditingTime = DateTime.Now;
                    //不为空说明上传了图片
                    if (!string.IsNullOrEmpty(iconFileName))
                    {
                        subnetEdit.SIcon = iconFileName;
                        net.SIcon = iconFileName;
                    }
                    
                    ctx.SaveChanges();
                }
                json.Data = new { Code = "", Data = net, Message = "保存成功" };
                //日志记录
                _topoLogger.Record(loginUser, "修改子网", "", "成功", "", net.ID, net.SName, "子网");
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = net, Message = ex.Message };
                //日志记录
                _topoLogger.Record(loginUser, "修改子网", "", "失败", string.Format("发生异常：{0}", ex.Message), net.ID, net.SName, "子网");
            }
            return json;
        }

        // GET: Subnet/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Subnet/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var json = new JsonResult();
            User loginUser = null;
            Subnet net = null;
            List<int> DeviceIds = new List<int>();
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    loginUser = (from u in ctx.User
                                 where u.ULoginName == HttpContext.User.Identity.Name
                                 select u).FirstOrDefault();
                    //子网的删除
                    var subnetDel = (from s in ctx.Subnet
                                     where s.ID == id
                                     select s).FirstOrDefault();
                    var subnetLineDel = from l in ctx.SubnetLine
                                         where l.SIDA == id || l.SIDB == id
                                         select l;
                    net = subnetDel.CopyProperty();
                    //子网下的设备一起删除
                    var deviceDel = ctx.Device.Where(d => d.SID == id).ToList();
                    DeviceIds = (from d in deviceDel
                                 select d.ID).ToList();
                    //删除设备及连线
                    foreach(var d in deviceDel)
                    {
                        var deviceLineDel = ctx.DeviceLine.Where(dl => dl.DIDA == d.ID || dl.DIDB == d.ID).FirstOrDefault();
                        //日志
                        if (deviceLineDel != null)
                        {
                            var dA = ctx.Device.Find(deviceLineDel.DIDA);
                            var dB = ctx.Device.Find(deviceLineDel.DIDB);
                            var detail = string.Format("{0}-{1}", dA.DName, dB.DName);
                            _topoLogger.Record(loginUser, "删除设备连线", detail, "成功", "删除子网时关联删除", deviceLineDel.ID, deviceLineDel.DLName, "设备连线");
                        }
                        ctx.DeviceLine.Remove(deviceLineDel);
                    }
                    foreach(var d in deviceDel)
                    {
                        _topoLogger.Record(loginUser, "删除设备", "", "成功", "删除子网时关联删除", d.ID, d.DName, "设备");
                        ctx.Device.Remove(d);
                    }
                    //删除子网连线
                    foreach(var sl in subnetLineDel)
                    {
                        var sA = ctx.Subnet.Find(sl.SIDA);
                        var sB = ctx.Subnet.Find(sl.SIDB);
                        var detail = string.Format("{0}-{1}", sA.SName, sB.SName);
                        _topoLogger.Record(loginUser, "删除子网连线", detail, "成功", "删除子网时关联删除", sl.ID, sl.SLName, "子网连线");
                        ctx.SubnetLine.Remove(sl);
                    }
                    ctx.Subnet.Remove(subnetDel);
                    //ctx.SubnetLine.RemoveRange(subnetLineDel);
                    ctx.SaveChanges();
                }
                json.Data = new { Code = "", Data = DeviceIds, Message = "删除成功" };
                //日志记录
                _topoLogger.Record(loginUser, "删除子网", "", "成功", "", net.ID, net.SName, "子网");
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = id, Message = ex.Message };
                //日志记录
                _topoLogger.Record(loginUser, "删除子网", "", "失败", string.Format("发生异常：{0}", ex.Message), net.ID, net.SName, "子网");
            }
            return json;
        }

        /// <summary>
        /// 拓扑图标选择
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult IconList(string type = "subnet")
        {
            var iconPath = Server.MapPath("~/image/topo");
            DirectoryInfo directoryInfo = new DirectoryInfo(iconPath);
            var imgFiles = directoryInfo.GetFiles();
            var subnetFiles = (from f in imgFiles
                               where f.Name.ToLower().Contains(type)
                               select f).ToList();
            return View(subnetFiles);
        }
        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Rename(int id, string name)
        {
            var json = new JsonResult();
            User loginUser = null;
            Subnet net = null;
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    loginUser = (from u in ctx.User
                                     where u.ULoginName == HttpContext.User.Identity.Name
                                     select u).FirstOrDefault();
                    var subnetModify = (from s in ctx.Subnet
                                     where s.ID == id
                                        select s).FirstOrDefault();
                    subnetModify.SName = name;
                    subnetModify.EditorID = loginUser.ID;
                    subnetModify.EditingTime = DateTime.Now;
                    net = subnetModify.CopyProperty();
                    net.Device = null;
                    ctx.SaveChanges();
                }
                json.Data = new { Code = "", Data = net, Message = "重命名成功" };
                //日志记录
                _topoLogger.Record(loginUser, "修改子网", "重命名", "成功", "", net.ID, net.SName, "子网");
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = net, Message = ex.Message };
                //日志记录
                _topoLogger.Record(loginUser, "修改子网", "重命名", "失败", string.Format("发生异常：{0}", ex.Message), net.ID, net.SName, "子网");
            }
            return json;
        }
        [HttpPost]
        public ActionResult ChangeCoordinate(int id, int x, int y)
        {
            var json = new JsonResult();
            User loginUser = null;
            Subnet net = null;
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    loginUser = (from u in ctx.User
                                     where u.ULoginName == HttpContext.User.Identity.Name
                                     select u).FirstOrDefault();
                    var subnetModify = (from s in ctx.Subnet
                                        where s.ID == id
                                        select s).FirstOrDefault();
                    subnetModify.CoordinateX = x;
                    subnetModify.CoordinateY = y;
                    subnetModify.EditorID = loginUser.ID;
                    subnetModify.EditingTime = DateTime.Now;
                    net = subnetModify.CopyProperty();
                    net.Device = null;
                    ctx.SaveChanges();
                }
                json.Data = new { Code = "", Data = net, Message = "坐标更新成功" };
                //日志记录
                _topoLogger.Record(loginUser, "修改子网", "坐标更新", "成功", "", net.ID, net.SName, "子网");
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = net, Message = ex.Message };
                //日志记录
                _topoLogger.Record(loginUser, "修改子网", "坐标更新", "失败", string.Format("发生异常：{0}", ex.Message), net.ID, net.SName, "子网");
            }
            return json;
        }
    }
}
