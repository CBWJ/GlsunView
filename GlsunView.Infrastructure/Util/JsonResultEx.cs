using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GlsunView.Infrastructure.Util
{
    /// <summary>  
    /// 通过重载ExecuteResult方法，实现自定义序列化日期的实现   这个类 之后可以进行重写
    /// </summary>  
    public class JsonResultEx : JsonResult
    {
        public JsonResultEx() { }

        public JsonResultEx(object data, JsonRequestBehavior behavior)
        {
            base.Data = data;
            base.JsonRequestBehavior = behavior;
            this.DateTimeFormat = "yyyy-MM-dd hh:mm:ss";
        }

        public JsonResultEx(object data, String dateTimeFormat)
        {
            base.Data = data;
            base.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            this.DateTimeFormat = dateTimeFormat;
        }

        /// <summary>
        /// 日期格式
        /// </summary>
        public string DateTimeFormat { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if ((this.JsonRequestBehavior == JsonRequestBehavior.DenyGet) && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("MvcResources.JsonRequest_GetNotAllowed");
            }
            HttpResponseBase base2 = context.HttpContext.Response;
            if (!string.IsNullOrEmpty(this.ContentType))
            {
                base2.ContentType = this.ContentType;
            }
            else
            {
                base2.ContentType = "application/json";
            }
            if (this.ContentEncoding != null)
            {
                base2.ContentEncoding = this.ContentEncoding;
            }
            if (this.Data != null)
            {

                String jsonResult = JsonHelper.ObjectToJson(this.Data);

                //相应结果
                base2.Write(jsonResult);
            }

        }
    }
}
