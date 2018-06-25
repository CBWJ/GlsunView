using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.Models;
using GlsunView.Infrastructure.Util;
using GlsunView.Infrastructure.Concrete;
using GlsunView.CommService;
using System.IO;
using System.Web.Script.Serialization;
using System.Net.Sockets;

namespace GlsunView.Controllers
{
    public class DeviceController : Controller
    {
        private TopologyLogger _topoLogger = new TopologyLogger();
        // GET: Device
        public ActionResult Index(int id)
        {
            Device d = null;
            using(var ctx = new GlsunViewEntities())
            {
                d = ctx.Device.Find(id);
            }
            //IP地址->连接服务->实例化一个状态类->填充状态
            TcpClientService tcp = new TcpClientService(d.DAddress, d.DPort.Value);
            //var tcp = TcpClientServiceTool.GetService(d.DAddress, d.DPort.Value);
            NMUCommService nmu = new NMUCommService(tcp);
            DeviceOverview deviceView = new DeviceOverview();
            deviceView.IP = d.DAddress;
            deviceView.Port = d.DPort.Value;
            try
            {
                //tcp.Connect();
                tcp.ConnectTimeout = 3000;
                if (tcp.ConnectWithTimeout())
                {
                    deviceView.RefreshStatus(nmu);
                    //TcpClientServiceTool.SetServiceFree(tcp);
                }
                else
                {
                    throw new TimeoutException("设备连接超时");
                }
            }
            catch(Exception ex)
            {
                deviceView.Type = "NoResponse";
                deviceView.Unit = 4;
            }
            if (deviceView.Slots == null) deviceView.Slots = new List<Slot>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ViewBag.DeviceInfo = serializer.Serialize(deviceView);
            return View(d);
        }

        public ActionResult GetCardInfo(string ip, int port)
        {
            //更新插卡信息
            //更新卡的状态
            var ret = new JsonResult();
            try
            {
                DeviceStatusSet set = new DeviceStatusSet();
                using (TcpClientService tcp = new TcpClientService(ip, port))
                {
                    tcp.Connect();
                    //var tcp = TcpClientServiceTool.GetService(ip, port);
                    NMUCommService nmu = new NMUCommService(tcp);
                    DeviceOverview deviceView = new DeviceOverview();
                    CardCommService cardService = null;
                    deviceView.RefreshStatus(nmu);
                    set.Overview = deviceView;
                    NMUInfo numInfo = new NMUInfo();
                    NMUCommService nmuService = new NMUCommService(tcp);
                    numInfo.RefreshStatus(nmuService);
                    set.NMUInfo = numInfo;
                    set.CardsInfo = new List<object>();
                    foreach (var e in deviceView.Slots)
                    {
                        if (e.IsInsert)
                        {
                            if (e.CardType == "EDFA")
                            {
                                cardService = new EDFACommService(tcp, e.SlotNumber);
                                EDFAInfo edfaInfo = new EDFAInfo();
                                edfaInfo.RefreshData(cardService);
                                e.CardInfo = edfaInfo;
                            }
                            else if (e.CardType == "OEO")
                            {
                                cardService = new OEOCommService(tcp, e.SlotNumber);
                                OEOInfo oeoInfo = new OEOInfo();
                                oeoInfo.RefreshData(cardService);
                                e.CardInfo = oeoInfo;
                            }
                            else if (e.CardType == "OLP")
                            {
                                cardService = new OLPCommService(tcp, e.SlotNumber);
                                OLPInfo olpInfo = new OLPInfo();
                                olpInfo.RefreshData(cardService);
                                e.CardInfo = olpInfo;
                            }
                        }
                    }
                    //TcpClientServiceTool.SetServiceFree(tcp);
                    tcp.Dispose();
                }
                ret.Data = new { Code = "", Data = set };
            }
            catch(Exception ex)
            {
                ret.Data = new { Code = "Exception", Data = ex.Message + "\n" + ex.StackTrace };
            }
            return ret; 
        }

        // GET: Device/Details/5
        public ActionResult Details(int id)
        {
            Device d = null;
            using (var ctx = new GlsunViewEntities())
            {
                d = ctx.Device.Find(id);
            }
            ViewBag.Action = "Details";
            return View("Create", d);
        }

        // GET: Device/Create
        public ActionResult Create(int id)
        {
            Device d = new Device();
            d.CoordinateX = 100;
            d.CoordinateY = 100;
            d.SID = id;
            ViewBag.Action = "Create";
            return View(d);
        }

        // POST: Device/Create
        [HttpPost]
        public ActionResult Create(Device device, HttpPostedFileBase iconFile)
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
                    var newDevice = device.CopyProperty();
                    newDevice.CreatorID = loginUser.ID;
                    newDevice.CreationTime = DateTime.Now;
                    if (string.IsNullOrWhiteSpace(newDevice.DIcon))
                    {
                        newDevice.DIcon = iconFileName;
                    }
                    ctx.Device.Add(newDevice);
                    ctx.SaveChanges();
                    device.ID = newDevice.ID;
                    device.DIcon = newDevice.DIcon;
                }
                var data = new TopologyNode
                {
                    ID = device.ID,
                    Name = device.DName,
                    Address = device.DAddress,
                    Icon = device.DIcon,
                    X = device.CoordinateX.Value,
                    Y = device.CoordinateY.Value
                };
                json.Data = new { Code = "", Data = data, Message = "保存成功" };
                //日志记录
                _topoLogger.Record(loginUser, "添加设备", "", "成功", "", device.ID, device.DName, "设备");
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = device, Message = ex.Message };
                //日志记录
                _topoLogger.Record(loginUser, "添加设备", "", "失败", string.Format("发生异常：{0}"), device.ID, device.DName, "设备");
            }
            return json;
        }
        private string SaveIcon(HttpPostedFileBase iconFile)
        {
            var iconPath = Server.MapPath("~/image/topo");
            DirectoryInfo directoryInfo = new DirectoryInfo(iconPath);
            var imgFiles = directoryInfo.GetFiles();
            var subnetFiles = (from f in imgFiles
                               where f.Name.ToLower().Contains("device")
                               select f).ToList();
            var fileType = iconFile.FileName.Substring(iconFile.FileName.IndexOf(".") + 1);
            var fileName = "device" + (subnetFiles.Count + 100).ToString() + "." + fileType;
            var filePath = Path.Combine(iconPath, fileName);
            byte[] byteIcon = new byte[iconFile.ContentLength];
            iconFile.InputStream.Read(byteIcon, 0, byteIcon.Length);
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                fs.Write(byteIcon, 0, byteIcon.Length);
            }
            return fileName;
        }
        // GET: Device/Edit/5
        public ActionResult Edit(int id)
        {
            Device d = null;
            using(var ctx = new GlsunViewEntities())
            {
                d = ctx.Device.Find(id);
            }
            ViewBag.Action = "Edit";
            return View("Create", d);
        }

        // POST: Device/Edit/5
        [HttpPost]
        public ActionResult Edit(Device device, HttpPostedFileBase iconFile)
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
                    var deviceModify = ctx.Device.Find(device.ID);

                    deviceModify.DName = device.DName;
                    deviceModify.DAddress = device.DAddress;
                    deviceModify.DPort = device.DPort;
                    deviceModify.DType = device.DType;
                    deviceModify.DIcon = device.DIcon;
                    deviceModify.DProtocal = device.DProtocal;
                    deviceModify.CoordinateX = device.CoordinateX;
                    deviceModify.CoordinateY = device.CoordinateY;
                    deviceModify.Remark = device.Remark;
                    deviceModify.EditorID = loginUser.ID;
                    deviceModify.EditingTime = DateTime.Now;

                    if (!string.IsNullOrWhiteSpace(iconFileName))
                    {
                        deviceModify.DIcon = iconFileName;
                        device.DIcon = iconFileName;
                    }
                    ctx.SaveChanges();
                }
                var data = new TopologyNode
                {
                    ID = device.ID,
                    Name = device.DName,
                    Address = device.DAddress,
                    Icon = device.DIcon,
                    X = device.CoordinateX.Value,
                    Y = device.CoordinateY.Value
                };
                json.Data = new { Code = "", Data = data, Message = "保存成功" };
                //日志记录
                _topoLogger.Record(loginUser, "修改设备", "", "成功", "", device.ID, device.DName, "设备");
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = device, Message = ex.Message };
                //日志记录
                _topoLogger.Record(loginUser, "修改设备", "", "失败", string.Format("发生异常：{0}"), device.ID, device.DName, "设备");
            }
            return json;
        }

        // GET: Device/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Device/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var json = new JsonResult();
            User loginUser = null;
            Device device = null;
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    loginUser = (from u in ctx.User
                                 where u.ULoginName == HttpContext.User.Identity.Name
                                 select u).FirstOrDefault();
                    var deviceDel = ctx.Device.Find(id);
                    device = deviceDel.CopyProperty();
                    var deviceLineDel = from l in ctx.DeviceLine
                                        where l.DIDA == id || l.DIDB == id
                                        select l;
                    ctx.Device.Remove(deviceDel);
                    //ctx.DeviceLine.RemoveRange(deviceLineDel);
                    foreach(var l in deviceLineDel)
                    {
                        var dA = ctx.Device.Find(l.DIDA);
                        var dB = ctx.Device.Find(l.DIDB);
                        var detail = string.Format("{0}-{1}", dA.DName, dB.DName);
                        _topoLogger.Record(loginUser, "删除设备连线", detail, "成功", "删除设备时关联删除", l.ID, l.DLName, "设备连线");
                        ctx.DeviceLine.Remove(l);
                    }
                    ctx.SaveChanges();
                }
                json.Data = new { Code = "", Data = id, Message = "删除成功" };
                //日志记录
                _topoLogger.Record(loginUser, "删除设备", "", "成功", "", device.ID, device.DName, "设备");
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = id, Message = ex.Message };
                //日志记录
                _topoLogger.Record(loginUser, "删除设备", "", "失败", string.Format("发生异常：{0}"), device.ID, device.DName, "设备");
            }
            return json;
        }

        [HttpPost]
        public ActionResult ChangeCoordinate(int id, int x, int y)
        {
            var json = new JsonResult();
            TopologyNode node = null;
            User loginUser = null;
            Device device = null;
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    loginUser = (from u in ctx.User
                                     where u.ULoginName == HttpContext.User.Identity.Name
                                     select u).FirstOrDefault();
                    var deviceModify = ctx.Device.Find(id);
                    device = deviceModify.CopyProperty();
                    if (deviceModify != null)
                    {
                        deviceModify.CoordinateX = x;
                        deviceModify.CoordinateY = y;
                        deviceModify.EditorID = loginUser.ID;
                        deviceModify.EditingTime = DateTime.Now;
                        ctx.SaveChanges();
                        node = new TopologyNode
                        {
                            ID = deviceModify.ID,
                            Name = deviceModify.DName,
                            Address = deviceModify.DAddress,
                            Icon = deviceModify.DIcon,
                            X = deviceModify.CoordinateX.Value,
                            Y = deviceModify.CoordinateY.Value
                        };
                    }

                }
                json.Data = new { Code = "", Data = node, Message = "重命名成功" };
                //日志记录
                _topoLogger.Record(loginUser, "修改设备", "重命名", "成功", "", device.ID, device.DName, "设备");
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = node, Message = ex.Message };
                //日志记录
                _topoLogger.Record(loginUser, "修改设备", "重命名", "失败", string.Format("发生异常：{0}"), device.ID, device.DName, "设备");
            }
            return json;
        }

        [HttpPost]
        public ActionResult Rename(int id, string name)
        {
            //序列化有外键约束的EF实体对象会报错
            var json = new JsonResult();
            TopologyNode node = null;
            User loginUser = null;
            Device device = null;
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    loginUser = (from u in ctx.User
                                     where u.ULoginName == HttpContext.User.Identity.Name
                                     select u).FirstOrDefault();
                    var deviceModify = ctx.Device.Find(id);
                    device = deviceModify.CopyProperty();
                    if (deviceModify != null)
                    {
                        deviceModify.DName = name;
                        deviceModify.EditorID = loginUser.ID;
                        deviceModify.EditingTime = DateTime.Now;
                        node = new TopologyNode
                        {
                            ID = deviceModify.ID,
                            Name = deviceModify.DName,
                            Address = deviceModify.DAddress,
                            Icon = deviceModify.DIcon,
                            X = deviceModify.CoordinateX.Value,
                            Y = deviceModify.CoordinateY.Value
                        };
                        ctx.SaveChanges();
                    }
                }
                json.Data = new { Code = "", Data = node, Message = "重命名成功" };
                //日志记录
                _topoLogger.Record(loginUser, "修改设备", "重命名", "成功", "", device.ID, device.DName, "设备");
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = node, Message = ex.Message };
                //日志记录
                _topoLogger.Record(loginUser, "修改设备", "重命名", "失败", string.Format("发生异常：{0}"), device.ID, device.DName, "设备");
            }
            return json;
        }
    }
}
