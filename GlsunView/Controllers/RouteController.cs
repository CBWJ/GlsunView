using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.Models;
using System.Text;
using GlsunView.CommService;
using GlsunView.Infrastructure.Util;

namespace GlsunView.Controllers
{
    public class RouteController : ShareListController
    {
        // GET: Route
        public ActionResult Index(int id)
        {
            RouteViewModel routeView = new RouteViewModel();
            using (var ctx = new GlsunViewEntities())
            {
                Route route = ctx.Route.Find(id);
                MachineFrame frameA = ctx.MachineFrame.Find(route.RAMFID);
                MachineFrame frameB = ctx.MachineFrame.Find(route.RBMFID);

                routeView.RouteName = route.RName;
                routeView.AName = route.RAName;
                routeView.AIP = frameA.MFIP;
                routeView.APort = frameA.MFPort.Value;
                routeView.ASlot = route.RASlot.Value;
                routeView.ACardPosition = string.Format("A框{0}-盘{1}", routeView.AIP, routeView.ASlot);
                routeView.ACardType = "OLP";
                routeView.BName = route.RBName;
                routeView.BIP = frameB.MFIP;
                routeView.BPort = frameB.MFPort.Value;
                routeView.BSlot = route.RBSlot.Value;
                routeView.BCardPosition = string.Format("B框{0}-盘{1}", routeView.BIP, routeView.BSlot);
                routeView.BCardType = "OLP";
            }
            OLPInfo olpInfo = new OLPInfo();
            var tcp = TcpClientServicePool.GetService(routeView.AIP, routeView.APort);
            if(tcp != null)
            {
                OLPCommService service = new OLPCommService(tcp, routeView.ASlot);
                try
                {
                    olpInfo.RefreshData(service);
                    routeView.ACardType = olpInfo.Card_Type;
                    routeView.AWorkRoute = olpInfo.Manual_Switch_Channel;
                }
                catch
                {
                    routeView.ACardType = "";
                    routeView.AWorkRoute = 1;
                }
                finally
                {
                    tcp.IsBusy = false;
                }
            }
            tcp = TcpClientServicePool.GetService(routeView.BIP, routeView.BPort);
            if (tcp != null)
            {
                OLPCommService service = new OLPCommService(tcp, routeView.BSlot);
                try
                {
                    olpInfo.RefreshData(service);
                    routeView.BCardType = olpInfo.Card_Type;
                    routeView.BWorkRoute = olpInfo.Manual_Switch_Channel;
                }
                catch
                {
                    routeView.BCardType = "";
                    routeView.BWorkRoute = 1;
                }
                finally
                {
                    tcp.IsBusy = false;
                }
            }
            return View(routeView);
        }
        /// <summary>
        /// 设备连线跳转到路由
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult LinkRoute(int id)
        {
            RouteViewModel routeView = new RouteViewModel();
            using (var ctx = new GlsunViewEntities())
            {
                DeviceLine line = ctx.DeviceLine.Find(id);
                Route route = ctx.Route.Find(line.RID);
                
                MachineFrame frameA = ctx.MachineFrame.Find(route.RAMFID);
                MachineFrame frameB = ctx.MachineFrame.Find(route.RBMFID);                

                if (route != null)
                {
                    routeView.RouteName = route.RName;
                    routeView.AName = route.RAName;
                    routeView.AIP = frameA.MFIP;
                    routeView.APort = frameA.MFPort.Value;
                    routeView.ASlot = route.RASlot.Value;
                    routeView.ACardPosition = string.Format("A框{0}-盘{1}", routeView.AIP, routeView.ASlot);
                    routeView.ACardType = "OLP";
                    routeView.BName = route.RBName;
                    routeView.BIP = frameB.MFIP;
                    routeView.BPort = frameB.MFPort.Value;
                    routeView.BSlot = route.RBSlot.Value;
                    routeView.BCardPosition = string.Format("B框{0}-盘{1}", routeView.BIP, routeView.BSlot);
                    routeView.BCardType = "OLP";
                }
            }
            OLPInfo olpInfo = new OLPInfo();
            var tcp = TcpClientServicePool.GetService(routeView.AIP, routeView.APort);
            if (tcp != null)
            {
                OLPCommService service = new OLPCommService(tcp, routeView.ASlot);
                try
                {
                    olpInfo.RefreshData(service);
                    routeView.ACardType = olpInfo.Card_Type;
                    routeView.AWorkRoute = olpInfo.Manual_Switch_Channel;
                }
                catch
                {
                    routeView.ACardType = "";
                    routeView.AWorkRoute = 1;
                }
                finally
                {
                    tcp.IsBusy = false;
                }
            }
            tcp = TcpClientServicePool.GetService(routeView.BIP, routeView.BPort);
            if (tcp != null)
            {
                OLPCommService service = new OLPCommService(tcp, routeView.BSlot);
                try
                {
                    olpInfo.RefreshData(service);
                    routeView.BCardType = olpInfo.Card_Type;
                    routeView.BWorkRoute = olpInfo.Manual_Switch_Channel;
                }
                catch
                {
                    routeView.BCardType = "";
                    routeView.BWorkRoute = 1;
                }
                finally
                {
                    tcp.IsBusy = false;
                }
            }
            return View("Index", routeView);
        }
        /// <summary>
        /// 配置路由
        /// </summary>
        /// <param name="model"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult ConfigRoute(RouteViewModel model,string property, int value)
        {
            JsonResult result = new JsonResult();
            var tcpA = TcpClientServicePool.GetService(model.AIP, model.APort);
            var tcpB = TcpClientServicePool.GetService(model.BIP, model.BPort);
            if (tcpA == null)
            {
                result.Data = new { Code = "Exception", Data = "获取A端TCP连接失败" };
            }
            else if (tcpB == null)
            {
                result.Data = new { Code = "Exception", Data = "获取B端TCP连接失败" };
            }
            if(tcpA != null && tcpB != null)
            {
                try
                {
                    List<string> listException = new List<string>();
                    //设置方法名
                    string methodName = "Set" + property.Replace("_", "");
                    OLPCommService serviceA = new OLPCommService(tcpA, model.ASlot);
                    bool ret = (bool)ReflectionHelper.InvokeMethod(serviceA, methodName, new object[] { value });
                    if (!ret)
                        listException.Add("A端线路");
                    OLPCommService serviceB = new OLPCommService(tcpB, model.BSlot);
                    ret = (bool)ReflectionHelper.InvokeMethod(serviceB, methodName, new object[] { value });
                    if (!ret)
                        listException.Add("B端线路");
                    if (listException.Count == 0)
                    {
                        result.Data = new { Code = "", Data = "配置成功" };
                    }
                    else
                    {
                        string data = "配置失败：";
                        foreach (var e in listException)
                        {
                            data += e + " ";
                        }
                        result.Data = new { Code = "Exception", Data = data };
                    }
                }
                catch (Exception ex)
                {
                    result.Data = new { Code = "Exception", Data = ex.Message };
                }
                finally
                {
                    tcpA.IsBusy = false;
                    tcpB.IsBusy = false;
                }
            }
            return result;
        }
        /// <summary>
        /// 路由列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult List(int page = 1, int pageSize = 10)
        {
            IEnumerable<v_Route> routes = null;
            PagingInfo pagingInfo = null;
            using (var ctx = new GlsunViewEntities())
            {
                routes = ctx.v_Route.OrderBy(mr => mr.ID)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                //分页信息
                pagingInfo = new PagingInfo
                {
                    TotalItems = ctx.v_MachineRoom.Count(),
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    ShowPageCount = 5
                };
            }
            ViewBag.PagingInfo = pagingInfo;
            SetAuthorityData();
            return View(routes);
        }
        // GET: Route/Details/5
        public ActionResult Details(int id)
        {
            Route route = null;
            using (var ctx = new GlsunViewEntities())
            {
                route = ctx.Route.Find(id);
            }
            ViewBag.Action = "Details";
            SetOptionData();
            return View("Create", route);
        }

        // GET: Route/Create
        public ActionResult Create()
        {
            Route route = new Route();
            SetOptionData();
            return View(route);
        }

        private void SetOptionData()
        {
            //路由组数据
            StringBuilder sbGroups = new StringBuilder();
            //机框数据
            StringBuilder sbFrames = new StringBuilder();
            sbGroups.Append("[");
            sbFrames.Append("[");
            using (var ctx = new GlsunViewEntities())
            {
                int count = 0;
                foreach (var group in ctx.RouteGroup)
                {
                    sbGroups.AppendFormat("{{id: {0}, text: '{1}'}}", group.ID, group.RGName);
                    count++;
                    if (count != ctx.RouteGroup.Count())
                    {
                        sbGroups.Append(",");
                    }
                }
                count = 0;
                foreach (var frame in ctx.MachineFrame)
                {
                    sbFrames.AppendFormat("{{id: {0}, text: '{1}'}}", frame.ID, frame.MFName);
                    count++;
                    if (count != ctx.MachineFrame.Count())
                    {
                        sbFrames.Append(",");
                    }
                }
            }
            sbGroups.Append("]");
            sbFrames.Append("]");
            ViewBag.GroupData = sbGroups.ToString();
            ViewBag.FrameData = sbFrames.ToString();
        }

        // POST: Route/Create
        [HttpPost]
        public ActionResult Create(Route route)
        {
            var json = new JsonResult();
            User loginUser = null;
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    loginUser = (from u in ctx.User
                                 where u.ULoginName == HttpContext.User.Identity.Name
                                 select u).FirstOrDefault();
                    var frameA = ctx.MachineFrame.Find(route.RAMFID);
                    var frameB = ctx.MachineFrame.Find(route.RBMFID);
                    route.RAIP = frameA.MFIP;
                    route.RBIP = frameB.MFIP;
                    route.CreationTime = DateTime.Now;
                    route.CreatorID = loginUser.ID;

                    ctx.Route.Add(route);
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

        // GET: Route/Edit/5
        public ActionResult Edit(int id)
        {
            Route route = null;
            using (var ctx = new GlsunViewEntities())
            {
                route = ctx.Route.Find(id);
            }
            ViewBag.Action = "Edit";
            SetOptionData();
            return View("Create", route);
        }

        // POST: Route/Edit/5
        [HttpPost]
        public ActionResult Edit(Route route)
        {
            var json = new JsonResult();
            User loginUser = null;
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    loginUser = (from u in ctx.User
                                 where u.ULoginName == HttpContext.User.Identity.Name
                                 select u).FirstOrDefault();
                    var modifyRoute = ctx.Route.Find(route.ID);
                    modifyRoute.RName = route.RName;
                    modifyRoute.RType = route.RType;
                    modifyRoute.RGID = route.RGID;
                    modifyRoute.RAName = route.RAName;
                    modifyRoute.RAMFID = route.RAMFID;
                    modifyRoute.RAIP = route.RAIP;
                    modifyRoute.RASlot = route.RASlot;
                    modifyRoute.RBName = route.RBName;
                    modifyRoute.RBMFID = route.RBMFID;
                    modifyRoute.RBIP = route.RBIP;
                    modifyRoute.RBSlot = route.RBSlot;
                    modifyRoute.Remark = route.Remark;
                    modifyRoute.EditorID = loginUser.ID;
                    modifyRoute.EditingTime = DateTime.Now;
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

        // GET: Route/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Route/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var json = new JsonResult();
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    ctx.Route.Remove(ctx.Route.Find(id));
                    ctx.SaveChanges();
                }
                json.Data = new { Code = "", Data = "", Message = "删除成功" };
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = "", Message = ex.Message };
            }
            return json;
        }

        public string GetRouteOption(int id)
        {
            //路由组数据
            StringBuilder sbRoute = new StringBuilder();
            sbRoute.Append("[");
            using (var ctx = new GlsunViewEntities())
            {
                int count = 0;
                var routes = ctx.Route.ToList();
                if (id > 0)
                    routes = routes.Where(r => r.RGID == id).ToList();
                foreach (var r in routes)
                {
                    sbRoute.AppendFormat("{{id: {0}, text: '{1}'}}", r.ID, r.RName);
                    count++;
                    if (count != routes.Count())
                    {
                        sbRoute.Append(",");
                    }
                }
            }
            sbRoute.Append("]");
            return sbRoute.ToString();
        }
    }
}
