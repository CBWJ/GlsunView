using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using System.Collections;
using GlsunView.Infrastructure.Concrete;
using Newtonsoft.Json;

namespace GlsunView.Controllers
{
    public class ShareListController : Controller
    {
        protected GlsunViewEntities db = new GlsunViewEntities();
        protected User LoginUser
        {
            get
            {
                using (var ctx = new GlsunViewEntities())
                {
                    var loginUser = (from u in ctx.User
                                     where u.ULoginName == HttpContext.User.Identity.Name
                                     select u).FirstOrDefault();
                    return loginUser;
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        /// <summary>
        /// 设置用户操作权限的数据
        /// </summary>
        public virtual void SetAuthorityData()
        {
            IEnumerable<v_UserModuleAuthority> userModuleAuth = null;
            string controllerName = (string)ControllerContext.RouteData.Values["controller"];
            string actionName = (string)ControllerContext.RouteData.Values["action"];
            string moduleUrl = controllerName + "/" + actionName;
            ViewBag.Controller = controllerName;
            ViewBag.Action = actionName;
            //拓扑管理用子级操作权限controllerName
            if (moduleUrl.ToLower() == "topology/index")
            {
                moduleUrl = "Subnet/Index";
            }
            using (var ctx = new GlsunViewEntities())
            {
                var loginUser = (from u in ctx.User
                                 where u.ULoginName == HttpContext.User.Identity.Name
                                 select u).FirstOrDefault();
                if (loginUser.UUserType == 0)
                {
                    var module = ctx.Module.Where(m => m.MUrl == moduleUrl).FirstOrDefault();
                    if (module != null)
                    {
                        var subSet = (from m in ctx.Module
                                      join ma in ctx.ModuleAuthority
                                      on m.ID equals ma.MID
                                      join a in ctx.Authority
                                      on ma.AID equals a.ID
                                      where m.ID == module.ID
                                      select new
                                      {
                                          AName = a.AName,
                                          AIcon = a.AIcon,
                                          ACode = a.ACode,
                                          AClassName = a.AClassName,
                                          AShowNumber = a.AShowNumber
                                      }).ToList();
                        userModuleAuth = from n in subSet
                                         orderby n.AShowNumber
                                         select new v_UserModuleAuthority
                                         {
                                             AName = n.AName,
                                             AIcon = n.AIcon,
                                             ACode = n.ACode,
                                             AClassName = n.AClassName
                                         };
                    }
                }
                else
                {
                    userModuleAuth = ctx.v_UserModuleAuthority
                        .Where(uma => uma.UID == loginUser.ID && uma.MUrl == moduleUrl)
                        .OrderBy(uma => uma.AShowNumber)
                        .ToList()
                        .Distinct(new UserModuleAuthorityComparer());
                }
            }
            ViewBag.UserModuleAuth = userModuleAuth;
        }

        public virtual void CreateAction()
        {
            ViewBag.Controller = (string)ControllerContext.RouteData.Values["controller"];
            ViewBag.Action = (string)ControllerContext.RouteData.Values["action"];
        }
        public virtual void EditAction()
        {
            ViewBag.Controller = (string)ControllerContext.RouteData.Values["controller"];
            ViewBag.Action = (string)ControllerContext.RouteData.Values["action"];
        }
        
        public virtual void DetailsAction()
        {
            ViewBag.Controller = (string)ControllerContext.RouteData.Values["controller"];
            ViewBag.Action = (string)ControllerContext.RouteData.Values["action"];
        }
        /// <summary>
        /// 添加模型，挂起添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        public void AddModelToContext<T>(T model)
        {
            var t = model.GetType();
            //设置模型值
            var prop = t.GetProperty("CreatorID");
            if (prop != null)
                prop.SetValue(model, LoginUser.ID);

            prop = t.GetProperty("CreationTime");
            if (prop != null)
                prop.SetValue(model, DateTime.Now);


            var tDB = db.GetType();
            //获取DbSet属性
            var modelSet = tDB.GetProperty(t.Name);
            //属性实例
            var modelSetInst = modelSet.GetValue(db);
            //获取Add方法
            var mAdd = modelSet.PropertyType.GetMethod("Add");
            //用实例调用Add
            mAdd.Invoke(modelSetInst, new object[] { model });
        }
        /// <summary>
        /// 简单的Model创建，直接返回视图
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        protected ActionResult CreateModel<T>(T model)
        {
            JsonResult ret = new JsonResult();
            try
            {

                var t = model.GetType();
                //设置模型值
                var prop = t.GetProperty("CreatorID");
                if (prop != null)
                    prop.SetValue(model, LoginUser.ID);

                prop = t.GetProperty("CreationTime");
                if (prop != null)
                    prop.SetValue(model, DateTime.Now);


                var tDB = db.GetType();
                //获取DbSet属性
                var modelSet = tDB.GetProperty(t.Name);
                //属性实例
                var modelSetInst = modelSet.GetValue(db);
                //获取Add方法
                var mAdd = modelSet.PropertyType.GetMethod("Add");
                //用实例调用Add
                mAdd.Invoke(modelSetInst, new object[] { model });

                db.SaveChanges();
                ret.Data = JsonConvert.SerializeObject(new
                {
                    status = 0,
                    message = "",
                    data = model
                });
            }
            catch (Exception ex)
            {
                ret.Data = JsonConvert.SerializeObject(new
                {
                    status = 1,
                    message = ex.Message,
                    data = model
                });
            }
            return ret;
        }
        public void EditModelToContext<T>(T model)
        {

            var t = model.GetType();
            //设置模型值
            var prop = t.GetProperty("EditorID");
            if (prop != null)
                prop.SetValue(model, LoginUser.ID);
            prop = t.GetProperty("EditingTime");
            if (prop != null)
                prop.SetValue(model, DateTime.Now);

            //获取ID
            prop = t.GetProperty("ID");
            var id = prop.GetValue(model);
            var tDB = db.GetType();
            //获取DbSet属性
            var modelSet = tDB.GetProperty(t.Name);
            //属性实例
            var modelSetInst = modelSet.GetValue(db);
            //获取Find方法
            var mFind = modelSet.PropertyType.GetMethod("Find");
            //用实例调用
            var editModel = mFind.Invoke(modelSetInst, new object[] { new object[] { id } });

            //修改Entity
            string[] expectProp = { "ID", "CreatorID", "CreationTime" };
            foreach (var p in t.GetProperties())
            {
                if (!expectProp.Contains(p.Name))
                {
                    var value = p.GetValue(model);
                    p.SetValue(editModel, value);
                }
            }
        }
        /// <summary>
        /// 简单的Model更新，直接返回视图
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        protected ActionResult EditModel<T>(T model)
        {
            JsonResult ret = new JsonResult();
            try
            {

                var t = model.GetType();
                //设置模型值
                var prop = t.GetProperty("EditorID");
                if (prop != null)
                    prop.SetValue(model, LoginUser.ID);
                prop = t.GetProperty("EditingTime");
                if (prop != null)
                    prop.SetValue(model, DateTime.Now);

                //获取ID
                prop = t.GetProperty("ID");
                var id = prop.GetValue(model);
                var tDB = db.GetType();
                //获取DbSet属性
                var modelSet = tDB.GetProperty(t.Name);
                //属性实例
                var modelSetInst = modelSet.GetValue(db);
                //获取Find方法
                var mFind = modelSet.PropertyType.GetMethod("Find");
                //用实例调用
                var editModel = mFind.Invoke(modelSetInst, new object[] { new object[] { id } });

                //修改Entity
                string[] expectProp = { "ID", "CreatorID", "CreationTime" };
                foreach (var p in t.GetProperties())
                {
                    if (!expectProp.Contains(p.Name))
                    {
                        var value = p.GetValue(model);
                        p.SetValue(editModel, value);
                    }
                }

                db.SaveChanges();
                ret.Data = JsonConvert.SerializeObject(new
                {
                    status = 0,
                    message = "",
                    data = model
                });
            }
            catch (Exception ex)
            {
                ret.Data = JsonConvert.SerializeObject(new
                {
                    status = 1,
                    message = ex.Message,
                    data = model
                });
            }
            return ret;
        }
        /// <summary>
        /// 从上下文删除模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idList"></param>
        public void DeleteModelFromContext<T>(T model)
        {
            var t = typeof(T);

            var tDB = db.GetType();
            //获取DbSet属性
            var modelSet = tDB.GetProperty(t.Name);
            //属性实例
            var modelSetInst = modelSet.GetValue(db);
            //获取Remove方法
            var mRemove = modelSet.PropertyType.GetMethod("Remove");
            //用实例调用
            mRemove.Invoke(modelSetInst, new object[] { model });
        }
        /// <summary>
        /// 简单的Model删除，直接返回视图
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idList"></param>
        /// <returns></returns>
        protected ActionResult DeleteModel<T>(List<int> idList)
        {
            JsonResult ret = new JsonResult();
            try
            {

                var t = typeof(T);

                var tDB = db.GetType();
                //获取DbSet属性
                var modelSet = tDB.GetProperty(t.Name);
                //属性实例
                var modelSetInst = modelSet.GetValue(db);
                //获取Find方法
                var mFind = modelSet.PropertyType.GetMethod("Find");
                //获取Remove方法
                var mRemove = modelSet.PropertyType.GetMethod("Remove");
                foreach (var id in idList)
                {
                    //用实例调用
                    var delModel = mFind.Invoke(modelSetInst, new object[] { new object[] { id } });
                    mRemove.Invoke(modelSetInst, new object[] { delModel });
                }

                db.SaveChanges();
                ret.Data = JsonConvert.SerializeObject(new
                {
                    status = 0,
                    message = "",
                    data = ""
                });
            }
            catch (Exception ex)
            {
                ret.Data = JsonConvert.SerializeObject(new
                {
                    status = 1,
                    message = ex.Message,
                    data = ""
                });
            }
            return ret;
        }
    }
}