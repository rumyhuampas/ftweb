using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FT.Models;

namespace FT.Controllers
{
    public class PlayersHelper
    {
        private ftEntities db;
        public List<player> selectedPlayers;

        public PlayersHelper(){
            selectedPlayers = new List<player>();
        }

        public void AddIfNotExist(player p)
        {
            bool found = false;
            for (int i = 0; i < selectedPlayers.Count; i++)
            {
                if (selectedPlayers[i].Id == p.Id)
                {
                    found = true;
                    break;
                }
            }
            if (found == false)
            {
                selectedPlayers.Add(p);
            }
        }

        public void Clear()
        {
            selectedPlayers.Clear();
        }

        public SelectList GetAllPlayers()
        {
            db = new ftEntities();
            var players = (from p in db.players
                           select p).OrderBy(player => player.Name);
            return new SelectList(players, "Id", "Name");
        }
    }

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
                    return RedirectToAction("Create");
                }

                db.AddToteams(teamObj);
                db.SaveChanges();
                teams_players tp = null;
                foreach (player p in playersHelper.selectedPlayers)
                {
                    tp = new teams_players();
                    tp.TeamId = teamObj.Id;
                    tp.PlayersId = p.Id;
                    db.AddToteams_players(tp);
                }
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
