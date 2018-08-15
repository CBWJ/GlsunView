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

namespace GlsunView.Controllers
{
    public class MachineFrameController : ShareListController
    {
        // GET: MachineFrame
        public ActionResult Index(int id)
        {
            MachineFrame frame = null;
            MachineShelf shelf = null;
            MachineRoom room = null;
            using(var ctx = new GlsunViewEntities())
            {
                frame = ctx.MachineFrame.Find(id);
                shelf = ctx.MachineShelf.Find(frame.MSID);
                room = ctx.MachineRoom.Find(shelf.MRID);
            }
            var tcp = TcpClientServicePool.GetService(frame.MFIP, frame.MFPort.Value);
            //设备整体状态信息
            NMUCommService nmu = new NMUCommService(tcp);
            DeviceOverview deviceView = new DeviceOverview();
            deviceView.IP = frame.MFIP;
            deviceView.Port = frame.MFPort.Value;
            deviceView.MCUType = frame.MFMCUType;
            //主控卡信息
            NMUInfo nmuInfo = new NMUInfo();
            //设备信息
            DeviceInfo info = new DeviceInfo();
            //卡槽信息
            List<CardSlotInfo> cardSlotInfo = new List<CardSlotInfo>();
            try
            {
                if (tcp != null)
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
                    tcp.IsBusy = false;
                }
                else
                {
                    throw new TimeoutException("设备连接超时");
                }
            }
            catch (Exception ex)
            {
                deviceView.Type = "NoResponse";
                deviceView.Unit = 4;
            }
            if (deviceView.Slots == null) deviceView.Slots = new List<Slot>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ViewBag.DeviceView = serializer.Serialize(deviceView);
            ViewBag.CardSlotInfo = serializer.Serialize(cardSlotInfo);
            return View(info);
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
                    Status = e.IsInsert ? "在位" : "NA",
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
                            slotInfo.WorkMode = "NA";
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
                        slotInfo.CardType = "NA";
                        slotInfo.HardwareVersion = "NA";
                        slotInfo.SoftwareVersion = "NA";
                        slotInfo.WorkMode = "NA";
                        slotInfo.CurrentAlarm = "NA";
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
        public ActionResult Create(MachineFrame frame)
        {
            var json = new JsonResult();
            try
            {
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
        public ActionResult Edit(MachineFrame frame)
        {
            var json = new JsonResult();
            try
            {
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
            try
            {
                string key = string.Format("card_info_{0}:{1}", ip, port);
                var tcp = TcpClientServicePool.GetService(ip, port);
                if (tcp == null) throw new Exception("null object");
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
            return ret;
        }
    }
}
