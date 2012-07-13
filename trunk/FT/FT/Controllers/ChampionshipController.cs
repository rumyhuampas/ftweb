using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FT.Models;
using FT.Extensions;

namespace FT.Controllers
{
    public class ChampionshipController : Controller
    {
        private ftEntities db;
        public static TeamsHelper teamsHelper;
        public static championship champHelper;

        public ChampionshipController()
        {
            db = new ftEntities();
        }
        //
        // GET: /Championship/

        public ActionResult Index()
        {
            ChampionshipController.champHelper = null;
            ChampionshipController.teamsHelper = null;
            return View();
        }

        public ActionResult List()
        {
            ChampionshipController.champHelper = null;
            ChampionshipController.teamsHelper = null;
            return View(db.championships.OrderBy(champ => champ.Name));
        }

        //
        // GET: /Championship/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Championship/Create

        public ActionResult Create()
        {
            if (ChampionshipController.teamsHelper == null) ChampionshipController.teamsHelper = new TeamsHelper();
            if (ChampionshipController.champHelper == null) ChampionshipController.champHelper = new championship();
            return View(ChampionshipController.champHelper);
        } 

        //
        // POST: /Championship/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Championship/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Championship/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Championship/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Championship/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
