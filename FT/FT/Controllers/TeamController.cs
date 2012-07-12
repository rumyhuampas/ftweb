using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FT.Models;
using FT.Extensions;

namespace FT.Controllers
{
    public class TeamController : Controller
    {
        private ftEntities db;
        public static PlayersHelper playersHelper;

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
            if (playersHelper == null)
            {
                playersHelper = new PlayersHelper();
            }

            return View(new team());
        } 

        //
        // POST: /Team/Create

        [HttpPost]
        public ActionResult Create(team teamObj, string btnSubmit)
        {
            try
            {
                if (btnSubmit == "AddPlayer")
                {
                    int playerId = Convert.ToInt32(Request["playerId"]);
                    player p = (from player in db.players
                                where player.Id == playerId
                                select player).First();
                    playersHelper.AddIfNotExist(p);
                    return View(teamObj);
                }

                db.AddToteams(teamObj);
                db.SaveChanges();
                team_player tp = null;
                foreach (player p in playersHelper.selectedPlayers)
                {
                    tp = new team_player();
                    tp.Team_Id = teamObj.Id;
                    tp.Player_Id = p.Id;
                    db.AddToteam_player(tp);
                }
                db.SaveChanges();
                playersHelper = null;

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult DeletePlayer(team teamObj, int playerId)
        {
            playersHelper.DeletePlayer(playerId);
            return RedirectToAction("Create", teamObj);
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
