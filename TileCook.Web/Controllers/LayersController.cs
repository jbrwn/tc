using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TileCook.Web.Models;

namespace TileCook.Web.Controllers
{
    public class LayersController : Controller
    {
        private ILayerRepository _repository;
        
        public LayersController()
        {
            this._repository = new LayerRepository();
        }

        public ActionResult Index()
        {
            return View(this._repository.GetAll());
        }

        public ActionResult Preview(string id)
        {
            Layer layer = this._repository.Get(id);
            if (layer == null)
            {
                return HttpNotFound();
            }
            return View(layer);
        }
    }
}
