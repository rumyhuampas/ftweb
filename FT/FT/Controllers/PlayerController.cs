using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FT.Models;

namespace FT.Controllers
{
    public class PlayerController : Controller
    {
        private ftEntities db;

        public PlayerController()
        {
            db = new ftEntities();
        }
        //
        // GET: /Player/

        public ActionResult Index()
        {
            return View(db.players.OrderBy(players => players.Name));
            //return View();
        }

        //
        // GET: /Player/Details/5

        public ActionResult Details(int playerId)
        {
            var player = (from p in db.players
                        where p.Id == playerId
                        select p).First();
            return View(player);
        }

        //
        // GET: /Player/Create

        public ActionResult Create()
        {
            return View(new player());
        } 

        //
        // POST: /Player/Create

        [HttpPost]
        public ActionResult Create(player playerObj)
        {
            try
            {
                // TODO: Add insert logic here
                db.AddToplayers(playerObj);
                db.SaveChanges();
                return RedirectToAction("Index", "Player").WithFlash(new { msginfo = "Player successfully created." });
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Player/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Player/Edit/5

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
        // GET: /Player/Delete/5
 
        public ActionResult Delete(int playerId)
        {
            try
            {
                IQueryable<team_player> list = from tps in db.team_player
                                               where tps.Player_Id == playerId
                                               select tps;
                foreach (team_player tp in list)
                {
                    db.DeleteObject(tp);
                }

                player p = (from players in db.players
                          where players.Id == playerId
                          select players).First();
                db.DeleteObject(p);
                db.SaveChanges();

                return RedirectToAction("Index", "Player").WithFlash(new { msginfo = "Player successfully deleted." });
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "Player").WithFlash(new { msgerror = ex.Message });
            }
        }
    }
}
