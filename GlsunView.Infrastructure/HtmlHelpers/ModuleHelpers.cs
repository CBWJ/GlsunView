using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using GlsunView.Domain;

namespace GlsunView.Infrastructure.HtmlHelpers
{
    public static class ModuleHelpers
    {
        public static MvcHtmlString ModuleMenu(this HtmlHelper html, IEnumerable<Module> modules)
        {
            string menuHtml = ModuleTraverse(0, modules);
            return MvcHtmlString.Create(menuHtml);
        }

        //递归创建列表
        public static  string ModuleTraverse(int moduleID, IEnumerable<Module> modules)
        {
            string result = "";
            if (modules == null)
                return result;
            var tops = from m in modules
                       where m.MParentID == moduleID
                       orderby m.MSortingNumber
                       select m;
            if (tops.ToList().Count == 0)
                return result;
            StringBuilder sbLINodes = new StringBuilder();
            TagBuilder tagUL = new TagBuilder("ul");
            foreach (var module in tops)
            {
                TagBuilder tagLI = new TagBuilder("li");
                TagBuilder tagLink = new TagBuilder("a");
                TagBuilder tagImage = new TagBuilder("img");
                TagBuilder tagIleft = new TagBuilder("i");
                TagBuilder tagIRight = new TagBuilder("i");
                TagBuilder tagSpan = new TagBuilder("span");

                tagSpan.SetInnerText(module.MName);
                tagLink.Attributes["href"] = module.MUrl;
                //左边图标
                if (module.MIconType == "font")
                {
                    tagIleft.AddCssClass("fa " + module.MIcon + " fa-fw");
                    if(module.MParentID == 0)
                    {
                        tagIleft.AddCssClass("menu-icon");
                    }
                    tagLink.InnerHtml = tagIleft.ToString();
                }
                else if (module.MIconType == "image")
                {
                    tagImage.Attributes["src"] = module.MIcon;
                    tagLink.InnerHtml = tagImage.ToString();
                }
                //标题
                tagLink.InnerHtml += tagSpan.ToString();
                //右边箭头
                bool hasChild = (from m in modules
                                 where m.MParentID == module.ID
                                 select m).ToList().Count > 0;
                if (hasChild)
                {
                    tagIRight.AddCssClass("right-tag fa fa-fw");
                }
                tagLink.InnerHtml += tagIRight.ToString();

                tagLI.InnerHtml = tagLink.ToString();
                tagLI.Attributes["data-id"] = module.ID.ToString();
                if (hasChild)
                {
                    tagLI.InnerHtml += ModuleTraverse(module.ID, modules);
                }
                sbLINodes.AppendLine(tagLI.ToString());
            }
            tagUL.InnerHtml = sbLINodes.ToString();
            result = tagUL.ToString();
            return result;
        }
    }
}
