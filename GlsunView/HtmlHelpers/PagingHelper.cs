using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Models;
using System.Text;

namespace GlsunView
{
    /// <summary>
    /// 分页生成工具，依赖于PagingInfo分页信息对象
    /// </summary>
    public static class PagingHelper
    {
        public static MvcHtmlString PageLinks(this HtmlHelper html, PagingInfo pagingInfo, 
            Func<int, string> pageUrl)
        {
            StringBuilder result = new StringBuilder();
            TagBuilder tagUL = new TagBuilder("ul");
            tagUL.AddCssClass("pagination");
            tagUL.AddCssClass("pagination-sm");
            TagBuilder tagPreviousLI = new TagBuilder("li");
            TagBuilder tagFirstLI = new TagBuilder("li");
            TagBuilder tagA = new TagBuilder("a");
            //首页
            tagFirstLI.AddCssClass("paginate_button");
            tagA.SetInnerText("首页");
            if (pagingInfo.CurrentPage == 1)
            {
                tagFirstLI.AddCssClass("disabled");
            }
            else
            {
                tagA.MergeAttribute("href", pageUrl(1));
            }
            tagFirstLI.InnerHtml = tagA.ToString();
            result.Append(tagFirstLI.ToString());
            //上一页
            tagA = new TagBuilder("a");
            tagA.SetInnerText("上页");
            tagPreviousLI.AddCssClass("paginate_button");
            if (pagingInfo.CurrentPage == 1)
            {
                tagPreviousLI.AddCssClass("disabled");
            }
            else
            {
                tagA.MergeAttribute("href", pageUrl(pagingInfo.CurrentPage - 1));
            }
            tagPreviousLI.InnerHtml = tagA.ToString();
            result.Append(tagPreviousLI.ToString());
            //显示页
            for (int i = pagingInfo.ShowFirstPage; i <= pagingInfo.ShowLastPage; ++i)
            {
                TagBuilder tagLI = new TagBuilder("li");
                tagLI.AddCssClass("paginate_button");
                if(pagingInfo.CurrentPage == i)
                {
                    tagLI.AddCssClass("active");
                }
                tagA = new TagBuilder("a");
                tagA.SetInnerText(i.ToString());
                tagA.MergeAttribute("href", pageUrl(i));

                tagLI.InnerHtml = tagA.ToString();
                result.Append(tagLI.ToString());
            }

            TagBuilder tagNextLI = new TagBuilder("li");
            tagA = new TagBuilder("a");
            tagA.SetInnerText("下页");
            tagNextLI.AddCssClass("paginate_button");

            if (pagingInfo.CurrentPage == pagingInfo.TotalPages)
            {
                tagNextLI.AddCssClass("disabled");
            }
            else
            {
                tagA.MergeAttribute("href", pageUrl(pagingInfo.CurrentPage + 1));
            }
            tagNextLI.InnerHtml = tagA.ToString();
            result.Append(tagNextLI.ToString());
            //尾页
            TagBuilder tagLastLI = new TagBuilder("li");
            tagA = new TagBuilder("a");
            tagA.SetInnerText("尾页");
            tagLastLI.AddCssClass("paginate_button");
            if (pagingInfo.CurrentPage == pagingInfo.TotalPages)
            {
                tagLastLI.AddCssClass("disabled");
            }
            else
            {
                tagA.MergeAttribute("href", pageUrl(pagingInfo.TotalPages));
            }
            tagLastLI.InnerHtml = tagA.ToString();
            result.Append(tagLastLI.ToString());

            tagUL.InnerHtml = result.ToString();
            return MvcHtmlString.Create(tagUL.ToString());
        }
    }
}