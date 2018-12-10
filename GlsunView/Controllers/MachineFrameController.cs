using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.Models;
using System.Text;
using GlsunView.Infrastructure.Util;
using GlsunView.CommService;
using System.Web.Script.Serialization;
using System.IO;

namespace GlsunView.Controllers
{
    public class MachineFrameController : ShareListController
    {
        // GET: MachineFrame
        public ActionResult Index(int id)
        {
            DeviceOverview deviceView;
            DeviceInfo info;
            List<CardSlotInfo> cardSlotInfo;
            GetDeviceOverView(id, out deviceView, out info, out cardSlotInfo);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ViewBag.DeviceView = serializer.Serialize(deviceView);
            ViewBag.CardSlotInfo = serializer.Serialize(cardSlotInfo);
            ViewBag.MFID = id;
            return View(info);
        }

        private static void GetDeviceOverView(int id, out DeviceOverview deviceView, out DeviceInfo info, out List<CardSlotInfo> cardSlotInfo)
        {
            MachineFrame frame = null;
            MachineShelf shelf = null;
            MachineRoom room = null;
            using (var ctx = new GlsunViewEntities())
            {
                frame = ctx.MachineFrame.Find(id);
                shelf = ctx.MachineShelf.Find(frame.MSID);
                room = ctx.MachineRoom.Find(shelf.MRID);
            }
            var tcp = TcpClientServicePool.GetService(frame.MFIP, frame.MFPort.Value);
            //设备整体状态信息
            NMUCommService nmu = new NMUCommService(tcp);
            deviceView = new DeviceOverview();
            deviceView.IP = frame.MFIP;
            deviceView.Port = frame.MFPort.Value;
            deviceView.MCUType = frame.MFMCUType;
            //主控卡信息
            NMUInfo nmuInfo = new NMUInfo();
            //设备信息
            info = new DeviceInfo();
            //卡槽信息
            cardSlotInfo = new List<CardSlotInfo>();
            if (tcp != null)
            {
                try
                {
                    deviceView.RefreshStatus(nmu);
                    nmuInfo.RefreshStatus(nmu);
                    info = new DeviceInfo
                    {
                        Type = string.Format("{0}-{1}U-{2}", frame.MFName, deviceView.Unit, deviceView.Type),
                        Shelf = string.Format("{0}-{1}，第{2}层", shelf.MSName, shelf.MSNumber, frame.MSLayer),
                        Room = room.MRName,
                        Address = room.MRAddress,
                        Description = room.MRDescription,
                        SerialNumber = nmuInfo.Serial_Number,
                        IP = frame.MFIP,
                        Mask = nmuInfo.Subnet_Mask,
                        MACAddr = deviceView.MACAddr
                    };
                    cardSlotInfo = GetCardSlotInfo(tcp, deviceView);
                }
                catch (Exception ex)
                {
                    deviceView.Type = "NoResponse";
                    deviceView.Unit = 4;
                }
                finally
                {
                    TcpClientServicePool.FreeService(tcp);
                }
            }
            else
            {
                throw new TimeoutException("设备连接超时");
            }
            
            if (deviceView.Slots == null) deviceView.Slots = new List<Slot>();
        }

        public ActionResult DeviceIndex(int id)
        {
            Device d = null;
            using (var ctx = new GlsunViewEntities())
            {
                d = ctx.Device.Find(id);
            }
            int mfID = 0;
            if (d != null)
                mfID = d.MFID;
            DeviceOverview deviceView;
            DeviceInfo info;
            List<CardSlotInfo> cardSlotInfo;
            GetDeviceOverView(mfID, out deviceView, out info, out cardSlotInfo);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ViewBag.DeviceView = serializer.Serialize(deviceView);
            ViewBag.CardSlotInfo = serializer.Serialize(cardSlotInfo);
            return View("Index", info);
        }

        private static List<CardSlotInfo> GetCardSlotInfo(TcpClientService tcp, DeviceOverview deviceView)
        {
            List<CardSlotInfo> cardSlotInfo = new List<CardSlotInfo>();
            var context = new GlsunViewEntities();
            foreach (var e in deviceView.Slots)
            {
                CardSlotInfo slotInfo = new CardSlotInfo
                {
                    Slot = e.SlotNumber,
                    Status = e.IsInsert ? "在位" : "N/A",
                    CardType = e.CardType
                };
                //其他信息
                switch (e.CardType)
                {
                    case "EDFA":
                        EDFACommService srvEDFA = new EDFACommService(tcp, e.SlotNumber);
                        EDFAInfo edfaInfo = new EDFAInfo();
                        edfaInfo.RefreshData(srvEDFA);
                        switch (edfaInfo.Work_Mode)
                        {
                            case 0:
                                slotInfo.WorkMode = "其他";
                                break;
                            case 1:
                                slotInfo.WorkMode = "ACC模式";
                                break;
                            case 2:
                                slotInfo.WorkMode = "APC模式";
                                break;
                            case 3:
                                slotInfo.WorkMode = "AGC模式";
                                break;
                            default:
                                slotInfo.WorkMode = "";
                                break;
                        }
                        slotInfo.HardwareVersion = edfaInfo.Hardware_Version;
                        slotInfo.SoftwareVersion = edfaInfo.Software_Version;
                        var alarm = context.AlarmInformation
                            .Where(a => a.DAddress == deviceView.IP && a.AISlot == slotInfo.Slot)
                            .OrderByDescending(a => a.AITime).FirstOrDefault();
                        if (alarm != null) slotInfo.CurrentAlarm = alarm.AIContent;
                        else slotInfo.CurrentAlarm = "";
                        break;
                    case "OEO":
                        OEOCommService srvOEO = new OEOCommService(tcp, e.SlotNumber);
                        OEOInfo oeoInfo = new OEOInfo();
                        oeoInfo.RefreshData(srvOEO);
                        slotInfo.HardwareVersion = oeoInfo.Hardware_Version;
                        slotInfo.SoftwareVersion = oeoInfo.Software_Version;
                        
                        //SFP模块是否有转发
                        bool bTranspond = false;
                        //SFP模块是否有环回
                        bool bLoopback = false;
                        foreach(var spf in oeoInfo.SFPSet)
                        {
                            if (spf.Work_Mode == 1)
                                bTranspond = true;
                            if (spf.Work_Mode == 3)
                                bLoopback = true;
                        }
                        if(bTranspond && bLoopback)
                        {
                            slotInfo.WorkMode = "转发+环回";
                        }
                        else if (bTranspond)
                        {
                            slotInfo.WorkMode = "转发";
                        }
                        else if (bLoopback)
                        {
                            slotInfo.WorkMode = "环回";
                        }
                        else
                        {
                            slotInfo.WorkMode = "N/A";
                        }
                        var alarmOEO = context.AlarmInformation
                            .Where(a => a.DAddress == deviceView.IP && a.AISlot == slotInfo.Slot)
                            .OrderByDescending(a => a.AITime).FirstOrDefault();
                        if (alarmOEO != null)
                            slotInfo.CurrentAlarm = alarmOEO.AIContent;
                        else
                            slotInfo.CurrentAlarm = "";
                        break;
                    case "OLP":
                        OLPCommService srvOLP = new OLPCommService(tcp, e.SlotNumber);
                        OLPInfo olpInfo = new OLPInfo();
                        olpInfo.RefreshData(srvOLP);
                        slotInfo.HardwareVersion = olpInfo.Hardware_Version;
                        slotInfo.SoftwareVersion = olpInfo.Software_Version;
                        switch (olpInfo.Work_Mode)
                        {
                            case 0:
                                slotInfo.WorkMode = "手动";
                                break;
                            case 1:
                                slotInfo.WorkMode = "自动";
                                break;
                            default:
                                break;
                        }
                        var alarmOLP = context.AlarmInformation
                            .Where(a => a.DAddress == deviceView.IP && a.AISlot == slotInfo.Slot)
                            .OrderByDescending(a => a.AITime).FirstOrDefault();
                        if (alarmOLP != null)
                            slotInfo.CurrentAlarm = alarmOLP.AIContent;
                        else
                            slotInfo.CurrentAlarm = "";
                        break;
                    default:
                        slotInfo.CardType = "N/A";
                        slotInfo.HardwareVersion = "N/A";
                        slotInfo.SoftwareVersion = "N/A";
                        slotInfo.WorkMode = "N/A";
                        slotInfo.CurrentAlarm = "N/A";
                        break;
                }
                cardSlotInfo.Add(slotInfo);
            }
            context.Dispose();
            return cardSlotInfo;
        }

        public ActionResult List(int page = 1, int pageSize = 10)
        {
            IEnumerable<v_MachineFrame> frames = null;
            PagingInfo pagingInfo = null;
            using (var ctx = new GlsunViewEntities())
            {
                frames = ctx.v_MachineFrame.OrderBy(ms => ms.ID)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                pagingInfo = new PagingInfo
                {
                    TotalItems = ctx.v_MachineFrame.Count(),
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    ShowPageCount = 5
                };
            }
            ViewBag.PagingInfo = pagingInfo;
            SetAuthorityData();
            return View(frames);
        }
        // GET: MachineFrame/Details/5
        public ActionResult Details(int id)
        {
            MachineFrame frame = null;
            int roomID = 0;
            using (var ctx = new GlsunViewEntities())
            {
                frame = ctx.MachineFrame.Find(id);
                if (frame != null)
                {
                    roomID = (from shelf in ctx.MachineShelf
                              join room in ctx.MachineRoom
                              on shelf.MRID equals room.ID
                              where shelf.ID == frame.MSID.Value
                              select room.ID).First();
                }
            }
            SetMachineData();
            ViewBag.Action = "Details";
            ViewBag.RoomID = roomID;
            return View("Create", frame);
        }

        // GET: MachineFrame/Create
        public ActionResult Create()
        {
            MachineFrame frame = new MachineFrame();
            SetMachineData();
            ViewBag.Action = "Create";
            return View(frame);
        }

        private void SetMachineData()
        {
            //机房数据
            StringBuilder sbRooms = new StringBuilder();
            //机房对应机架数据
            StringBuilder sbRoomShelfs = new StringBuilder();
            //机架层数数据
            StringBuilder sbShelfLayers = new StringBuilder();
            sbRooms.Append("[");
            sbRoomShelfs.Append("{");
            sbShelfLayers.Append("{");
            using (var ctx = new GlsunViewEntities())
            {
                int count = 0;
                foreach (var room in ctx.MachineRoom)
                {
                    sbRooms.AppendFormat("{{id: {0}, text: '{1}'}}", room.ID, room.MRName);
                    count++;

                    sbRoomShelfs.AppendFormat("{0}:[", room.ID);
                    var shelfs = ctx.MachineShelf.Where(s => s.MRID == room.ID);
                    int cnt = 0;
                    foreach (var shelf in shelfs)
                    {
                        sbRoomShelfs.AppendFormat("{{id: {0}, text: '{1}'}}", shelf.ID, shelf.MSName);
                        cnt++;
                        if (cnt != shelfs.Count())
                        {
                            sbRoomShelfs.Append(",");
                        }
                    }
                    sbRoomShelfs.Append("]");

                    if (count != ctx.MachineRoom.Count())
                    {
                        sbRooms.Append(",");
                        sbRoomShelfs.Append(",");
                    }
                }
                count = 0;
                foreach (var shelf in ctx.MachineShelf)
                {
                    sbShelfLayers.AppendFormat("{0}:{1}", shelf.ID, shelf.MSLayers);
                    count++;
                    if (count != ctx.MachineShelf.Count())
                    {
                        sbShelfLayers.Append(",");
                    }
                }
            }
            sbRooms.Append("]");
            sbRoomShelfs.Append("}");
            sbShelfLayers.Append("}");
            ViewBag.RoomData = sbRooms.ToString();
            ViewBag.RoomShelfData = sbRoomShelfs.ToString();
            ViewBag.ShelfLayerData = sbShelfLayers.ToString();
        }

        // POST: MachineFrame/Create
        [HttpPost]
        public ActionResult Create(MachineFrame frame, HttpPostedFileBase iconFile)
        {
            var json = new JsonResult();
            try
            {
                string iconFileName = "";
                if (iconFile != null)
                {
                    iconFileName = SaveIcon(iconFile);
                }
                using (var ctx = new GlsunViewEntities())
                {
                    var loginUser = (from u in ctx.User
                                     where u.ULoginName == HttpContext.User.Identity.Name
                                     select u).FirstOrDefault();
                    if (loginUser != null)
                    {
                        frame.CreatorID = loginUser.ID;
                        frame.CreationTime = DateTime.Now;
                    }
                    //图标没值
                    if (string.IsNullOrWhiteSpace(frame.MFIcon))
                    {
                        frame.MFIcon = iconFileName;
                    }
                    ctx.MachineFrame.Add(frame);
                    ctx.SaveChanges();
                }
                json.Data = new { Code = "", Data = "", Message = "保存成功" };
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = "", Message = ex.Message };
            }
            return json;
        }

        // GET: MachineFrame/Edit/5
        public ActionResult Edit(int id)
        {
            MachineFrame frame = null;
            int roomID = 0;
            using (var ctx = new GlsunViewEntities())
            {
                frame = ctx.MachineFrame.Find(id);
                if(frame != null)
                {
                    roomID = (from shelf in ctx.MachineShelf
                             join room in ctx.MachineRoom
                             on shelf.MRID equals room.ID
                             where shelf.ID == frame.MSID.Value
                             select room.ID).First();
                }
            }
            SetMachineData();
            ViewBag.Action = "Edit";
            ViewBag.RoomID = roomID;
            return View("Create", frame);
        }

        // POST: MachineFrame/Edit/5
        [HttpPost]
        public ActionResult Edit(MachineFrame frame, HttpPostedFileBase iconFile)
        {
            var json = new JsonResult();
            try
            {
                string iconFileName = "";
                if (iconFile != null)
                {
                    iconFileName = SaveIcon(iconFile);
                }
                using (var ctx = new GlsunViewEntities())
                {
                    var loginUser = (from u in ctx.User
                                     where u.ULoginName == HttpContext.User.Identity.Name
                                     select u).FirstOrDefault();

                    var frameModify = (from r in ctx.MachineFrame
                                       where r.ID == frame.ID
                                       select r).FirstOrDefault();

                    frameModify.MFName = frame.MFName;
                    frameModify.MFIP = frame.MFIP;
                    frameModify.MFPort = frame.MFPort;
                    frameModify.MSID = frame.MSID;
                    frameModify.MFMCUType = frame.MFMCUType;
                    frameModify.MSLayer = frame.MSLayer;
                    frameModify.MFIcon = frame.MFIcon;
                    frameModify.Remark = frame.Remark;
                    frameModify.EditorID = loginUser.ID;
                    frameModify.EditingTime = DateTime.Now;
                    //图标没值
                    if (string.IsNullOrWhiteSpace(frameModify.MFIcon))
                    {
                        frameModify.MFIcon = iconFileName;
                    }
                    ctx.SaveChanges();
                }
                json.Data = new { Code = "", Data = "", Message = "保存成功" };
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = "", Message = ex.Message };
            }
            return json;
        }

        // GET: MachineFrame/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MachineFrame/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var json = new JsonResult();
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    var frameDelete = ctx.MachineFrame.Find(id);
                    ctx.MachineFrame.Remove(frameDelete);
                    ctx.SaveChanges();
                }
                json.Data = new { Code = "", Data = id, Message = "删除成功" };
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = id, Message = ex.Message };
            }
            return json;
        }

        public ActionResult GetCardInfo(string ip, int port)
        {
            //更新插卡信息
            //更新卡的状态
            var ret = new JsonResult();
            var tcp = TcpClientServicePool.GetService(ip, port);
            if (tcp != null)
            {
                try
                {
                    string key = string.Format("card_info_{0}:{1}", ip, port);
                    NMUCommService nmu = new NMUCommService(tcp);
                    DeviceOverview deviceView = new DeviceOverview();
                    deviceView.RefreshStatus(nmu);
                    deviceView.IP = ip;
                    deviceView.Port = port;
                    var cardSlotInfo = GetCardSlotInfo(tcp, deviceView);
                    tcp.IsBusy = false;
                    ret.Data = new { Code = "", Data = cardSlotInfo };
                }
                catch (Exception ex)
                {
                    ret.Data = new { Code = "Exception", Data = ex.Message + "\n" + ex.StackTrace };
                }
                finally
                {
                    TcpClientServicePool.FreeService(tcp);
                }
            }
            else
            {
                ret.Data = new { Code = "Exception", Data = "获取TCP连接失败" };
            }
            return ret;
        }

        [HttpPost]
        public ActionResult GetMachineFrame(int id)
        {
            var json = new JsonResult();
            try
            {
                MachineFrame frame = null;
                using (var ctx = new GlsunViewEntities())
                {
                    frame = ctx.MachineFrame.Find(id);
                }
                json.Data = new { Code = "", Data = new MachineFrame{ MFIP = frame.MFIP, MFPort = frame.MFPort, MFMCUType = frame.MFMCUType},
                    Message = "保存成功" };
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = "", Message = ex.Message };
            }
            return json;
        }
        /// <summary>
        /// 机框图标选择
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult IconList(string type = "frame")
        {
            var iconPath = Server.MapPath("~/image/frame");
            DirectoryInfo directoryInfo = new DirectoryInfo(iconPath);
            var imgFiles = directoryInfo.GetFiles();
            var groupFiles = (from f in imgFiles
                              where f.Name.ToLower().Contains(type)
                              select f).ToList();
            ViewBag.IconPath = "/image/frame/";
            return View("~/Views/Subnet/IconList.cshtml", groupFiles);
        }
        /// <summary>
        /// 保存图标
        /// </summary>
        /// <param name="iconFile"></param>
        /// <returns></returns>
        private string SaveIcon(HttpPostedFileBase iconFile)
        {
            var iconPath = Server.MapPath("~/image/frame");
            DirectoryInfo directoryInfo = new DirectoryInfo(iconPath);
            var imgFiles = directoryInfo.GetFiles();
            var subnetFiles = (from f in imgFiles
                               where f.Name.ToLower().Contains("frame")
                               select f).ToList();
            var fileType = iconFile.FileName.Substring(iconFile.FileName.IndexOf(".") + 1);
            var fileName = "frame" + (subnetFiles.Count + 100).ToString() + "." + fileType;
            var filePath = Path.Combine(iconPath, fileName);
            byte[] byteIcon = new byte[iconFile.ContentLength];
            iconFile.InputStream.Read(byteIcon, 0, byteIcon.Length);
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                fs.Write(byteIcon, 0, byteIcon.Length);
            }
            return fileName;
        }
    }
}
