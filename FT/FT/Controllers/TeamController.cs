﻿using System;
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
        public static team teamHelper;

        public TeamController()
        {
            db = new ftEntities();
        }
        //
        // GET: /Team/

        public ActionResult Index()
        {
            TeamController.teamHelper = null;
            TeamController.playersHelper = null;
            List<team> teamList = db.teams.OrderBy(teams => teams.Name).ToList();
            return View(teamList);
            //return View();
        }

        //
        // GET: /Team/Details/5

        public ActionResult Details(int teamId)
        {
            var team = (from t in db.teams
                     where t.Id == teamId
                     select t).First();
            return View(team);
        }

        //
        // GET: /Team/Create

        public ActionResult Create()
        {
            if (TeamController.playersHelper == null) TeamController.playersHelper = new PlayersHelper();
            if(TeamController.teamHelper == null) TeamController.teamHelper = new team();
            return View(TeamController.teamHelper);
        } 

        //
        // POST: /Team/Create

        [HttpPost]
        public ActionResult Create(team teamObj, string btnSubmit)
        {
            try
            {
                TeamController.teamHelper = teamObj;

                if (btnSubmit == "AddPlayer")
                {
                    int playerId = Convert.ToInt32(Request["playerId"]);
                    player p = (from player in db.players
                                where player.Id == playerId
                                select player).First();
                    TeamController.playersHelper.AddIfNotExist(p);
                    return View(teamObj);
                }

                if (TeamController.playersHelper.selectedPlayers.Count == 2)
                {
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
                    TeamController.teamHelper = null;
                    TeamController.playersHelper = null;

                    return RedirectToAction("Index", "Team").WithFlash(new { msginfo = "Team successfully created." });
                }
                else
                {
                    return RedirectToAction("Create", "Team").WithFlash(new { msgerror = "Team must have two players." });
                }
            }
            catch
            {
                return View();
            }
        }

        public ActionResult DeletePlayer(int playerId)
        {
            TeamController.playersHelper.DeletePlayer(playerId);
            return RedirectToAction("Create", "Team");
        }

        //
        // GET: /Team/Edit/5
 
        /*public ActionResult Edit(int id)
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
        }*/

        //
        // GET: /Team/Delete/5
 
        public ActionResult Delete(int teamId)
        {
            try
            {
                IQueryable<team_player> list = from tps in db.team_player
                                               where tps.Team_Id == teamId
                                               select tps;
                foreach (team_player tp in list)
                {
                    db.DeleteObject(tp);
                }

                team t = (from teams in db.teams
                          where teams.Id == teamId
                          select teams).First();
                db.DeleteObject(t);
                db.SaveChanges();

                return RedirectToAction("Index", "Team").WithFlash(new { msginfo = "Team successfully deleted." });
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "Player").WithFlash(new { msgerror = ex.Message });
            }
        }
    }
}
