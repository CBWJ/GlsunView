using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlsunView.Models
{
    public class PagingInfo
    {
        /// <summary>
        /// 条目总数
        /// </summary>
        public int TotalItems { get; set; }
        /// <summary>
        /// 每页大小
        /// </summary>
        public int ItemsPerPage { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// 显示页数
        /// </summary>
        public int ShowPageCount { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
            }
        }

        public int ShowFirstPage
        {
            get
            {
                //if (CurrentPage <= ShowPageCount)
                //    return 1;
                //else
                //{
                //    return CurrentPage;
                //}
                return ((CurrentPage-1) / ShowPageCount) * ShowPageCount + 1;
            }
        }

        public int ShowLastPage
        {
            get
            {
                if (TotalPages <= ShowPageCount)
                {
                    return Math.Min(ShowPageCount, TotalPages);
                }
                else
                {
                    return Math.Min(TotalPages, ShowFirstPage + ShowPageCount - 1);
                }
            }
        }
    }
}