using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentScheduler;

namespace GlsunView.Common
{
    public class LisenceRegistry : Registry
    {
        public LisenceRegistry()
        {
            // Schedule a simple job to run at a specific time
            //每天23:59分刷新Lisence信息
            Schedule(() => LisenceHelper.ParseLisence()).ToRunEvery(1).Days().At(23, 59);
        }
    }
}