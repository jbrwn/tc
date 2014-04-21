using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TileCook.Web.Controllers
{
    public class LayersController : Controller
    {
        //
        // GET: /Layers/

        public ActionResult Index()
        {
            return View(LayerCache.GetLayers());
        }

        public ActionResult Preview(string id)
        {
            Layer layer = LayerCache.GetLayer(id);
            return View(layer);
        }
    }
}
