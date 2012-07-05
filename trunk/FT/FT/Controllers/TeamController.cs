using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FT.Models;

namespace FT.Controllers
{
    public class TeamPlayer
    {
        private ftEntities db;
        public team team { get; set; }
        public List<player> players { get; set; }

        public TeamPlayer()
        {
            db = new ftEntities();
        }
        public List<player> GetAllPlayers()
        {
            return db.players.ToList();
        }
    }

    public class TeamController : Controller
    {
        private ftEntities db;

        public TeamController()
        {
            db = new ftEntities();
        }
        //
        // GET: /Team/

        public ActionResult Index()
        {
            return View(db.teams.OrderBy(teams => teams.Name));
            //return View();
        }

        //
        // GET: /Team/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Team/Create

        public ActionResult Create()
        {
            TeamPlayer teamplayer = new TeamPlayer();
            teamplayer.team = new team();
            return View(teamplayer);
        } 

        //
        // POST: /Team/Create

        [HttpPost]
        public ActionResult Create(team teamObj)
        {
            try
            {
                // TODO: Add insert logic here
                db.AddToteams(teamObj);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Team/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Team/Edit/5

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
        // GET: /Team/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Team/Delete/5

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
