using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GlsunView.Domain;
using System.Text;
using System.Web.Mvc;

namespace GlsunView
{
    public static class AuthorityButtonHelper
    {
        public static MvcHtmlString AuthorityButtonList(this HtmlHelper html, IEnumerable<v_UserModuleAuthority> userModuleAuth)
        {
            StringBuilder result = new StringBuilder();
            if(userModuleAuth != null)
                foreach(var e in userModuleAuth)
                {
                    TagBuilder tagButton = new TagBuilder("button");
                    TagBuilder tagSpan = new TagBuilder("span");
                    tagSpan.AddCssClass(e.AIcon);

                    tagButton.MergeAttribute("type", "button");
                    tagButton.MergeAttribute("data-selected-id", "0");
                    tagButton.MergeAttribute("id", "btn" + e.ACode);
                    tagButton.AddCssClass(e.AClassName);
                    tagButton.InnerHtml = tagSpan.ToString() + "&nbsp;" + e.AName;
                    result.AppendLine(tagButton.ToString());
                }
            return MvcHtmlString.Create(result.ToString());
        }
    }
}