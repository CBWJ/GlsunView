using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using System.Web.Script.Serialization;
using GlsunView.Models;

namespace GlsunView.Controllers
{
    public class TopologyController : ShareListController
    {
        // GET: Topology
        public ActionResult Index()
        {
            IEnumerable<Subnet> subnets = null;
            IEnumerable<TopologyNode> nodes = null;
            IEnumerable<TopologyLine> lines = null;
            SceneLocation sceneLoc = null;
            var nodesJSON = string.Empty;
            using (var ctx = new GlsunViewEntities())
            {
                subnets = ctx.Subnet.ToList();
                lines = (from l in ctx.SubnetLine
                        select new TopologyLine
                        {
                            ID = l.ID,
                            Name = l.SLName,
                            NodeIDA = l.SIDA.Value,
                            NodeIDZ = l.SIDB.Value
                        }).ToList();
                sceneLoc = (from l in ctx.SceneLocation
                            where l.SceneType == "subnet"
                            select l).FirstOrDefault();
                if(sceneLoc == null)
                {
                    sceneLoc = new SceneLocation
                    {
                        CoordinateX = 0,
                        CoordinateY = 0,
                        SceneType = "subnet"
                    };
                    ctx.SceneLocation.Add(sceneLoc);
                    ctx.SaveChanges();
                }
            }
            nodes = (from n in subnets
                     select new TopologyNode
                     {
                         ID = n.ID,
                         Name = n.SName,
                         Address = n.SAddress,
                         Icon = n.SIcon,
                         X = n.CoordinateX.Value,
                         Y = n.CoordinateY.Value
                     }).ToList();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ViewBag.Nodes = serializer.Serialize(nodes); 
            ViewBag.Lines = serializer.Serialize(lines);
            ViewBag.SceneLocation = serializer.Serialize(sceneLoc);
            SetAuthorityData();
            return View();
        }
        [HttpPost]
        public void SaveSceneLocation(SceneLocation location)
        {
            using (var ctx = new GlsunViewEntities())
            {
                var loc = (from l in ctx.SceneLocation
                                    where l.ID == location.ID
                                    select l).FirstOrDefault();
                loc.CoordinateX = location.CoordinateX;
                loc.CoordinateY = location.CoordinateY;
                ctx.SaveChanges();
            }
        }
    }
}