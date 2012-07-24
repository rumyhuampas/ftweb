﻿using System;
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
        public static ftEntities db;
        public static TeamsHelper teamsHelper;
        public static championship champHelper;
        public static FixtureHelper fixtureHelper;
        public static string selectedType = "SINGLE";

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

        public ActionResult Details(int champId)
        {
            fixtureHelper = new FixtureHelper(champId);
            fixtureHelper.BuildFixture();

            championship champ = (from c in db.championships
                                  where c.Id == champId
                                  select c).First();
            return View(champ);
        }

        //
        // GET: /Championship/Create

        public ActionResult Create()
        {
            if (ChampionshipController.teamsHelper == null) ChampionshipController.teamsHelper = new TeamsHelper();
            if (ChampionshipController.champHelper == null) ChampionshipController.champHelper = new championship();
            return View(ChampionshipController.champHelper);
        }

        public static SelectList GetChampTypes()
        {
            List<SelectListItem> types = new List<SelectListItem>();
            types.Add(new SelectListItem{
                Text = "SINGLE",
                Value = "SINGLE",
                Selected = true
            });
            types.Add(new SelectListItem{
                Text = "DOUBLE",
                Value = "DOUBLE"
            });

            return new SelectList(types, "Text", "Value");
        }

        public JsonResult GetChampTeams(string champType)
        {
            if (champType == "SINGLE")
            {
                var players = (from p in db.players
                             select p).OrderBy(player => player.Name);
                return Json(new SelectList(players, "Id", "Name"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var teams = (from t in db.teams
                             select t).OrderBy(team => team.Name);
                return Json(new SelectList(teams, "Id", "Name"), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CleanSelectedTeams()
        {
            if (ChampionshipController.teamsHelper != null) ChampionshipController.teamsHelper.Clear();
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetSelectedType(string champType)
        {
            selectedType = champType;
            return Json(selectedType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSelectedType()
        {
            return Json(selectedType, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /Championship/Create

        [HttpPost]
        public ActionResult Create(championship champObj, string btnSubmit)
        {
            try
            {
                ChampionshipController.champHelper = champObj;
                string champType = Request["champtype"];
                if (btnSubmit == "AddTeam")
                {
                    int champTeamId = Convert.ToInt32(Request["champteams"]);
                    ChampTeam team = new ChampTeam();
                    team.Id = champTeamId;
                    if(champType == "SINGLE"){
                        player p = (from players in db.players
                                    where players.Id == champTeamId
                                    select players).First();
                        team.Name = p.Name;
                        team.Type = "PLAYER";
                    }
                    else{
                        team t = (from teams in db.teams
                                  where teams.Id == champTeamId
                                  select teams).First();
                        team.Name = t.Name;
                        team.Type = "TEAM";
                    }

                    ChampionshipController.teamsHelper.AddIfNotExist(team);
                    return View(champObj);
                }

                if (ChampionshipController.teamsHelper.selectedTeams.Count > 3)
                {
                    champObj.Type = champType;
                    db.AddTochampionships(champObj);
                    db.SaveChanges();
                    championship_teams ct = null;
                    foreach (ChampTeam t in ChampionshipController.teamsHelper.selectedTeams)
                    {
                        ct = new championship_teams();
                        ct.championship_Id = champObj.Id;
                        ct.team_Id = t.Id;
                        db.AddTochampionship_teams(ct);
                    }
                    db.SaveChanges();

                    GenerateMatches(champObj.Id);

                    ChampionshipController.teamsHelper = null;
                    ChampionshipController.champHelper = null;

                    return RedirectToAction("List", "Championship").WithFlash(new { msginfo = "Championship successfully created." });
                }
                else
                {
                    return RedirectToAction("Create", "Championship").WithFlash(new { msgerror = "Teams must be at least 4." });
                }
            }
            catch(Exception ex)
            {
                return RedirectToAction("List", "Championship").WithFlash(new { msgerror = ex.Message });
            }
        }

        public ActionResult DeleteTeam(int teamId)
        {
            ChampionshipController.teamsHelper.DeleteTeam(teamId);
            return RedirectToAction("Create", "Championship");
        }
        
        //
        // GET: /Championship/Edit/5
 
        /*public ActionResult Edit(int id)
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
        }*/

        //
        // GET: /Championship/Delete/5
 
        public ActionResult Delete(int champId)
        {
            try
            {
                IQueryable<championship_teams> champTeams = from cts in db.championship_teams
                                                      where cts.championship_Id == champId
                                                      select cts;
                foreach (championship_teams ct in champTeams)
                {
                    db.DeleteObject(ct);
                }

                IQueryable<championship_matches> champMatches = from cm in db.championship_matches
                                                                where cm.championship_Id == champId
                                                                select cm;
                List<championship_matches> champMatchesList = champMatches.ToList();
                foreach (championship_matches cm in champMatchesList)
                {
                    match m = (from matches in db.matches
                               where matches.Id == cm.match_Id
                               select matches).First();
                    db.DeleteObject(m);
                }

                foreach (championship_matches cm in champMatches)
                {
                    db.DeleteObject(cm);
                }
                db.SaveChanges();

                championship c = (from champs in db.championships
                                  where champs.Id == champId
                                  select champs).First();
                db.DeleteObject(c);
                db.SaveChanges();

                return RedirectToAction("List", "Championship").WithFlash(new { msginfo = "Championship successfully deleted" });
            }
            catch(Exception ex)
            {
                return RedirectToAction("List", "Championship").WithFlash(new { msgerror = ex.Message });
            }
        }

        public void GenerateMatches(int champId)
        {
            IQueryable<championship_matches> champMatches = from cm in db.championship_matches
                                                            where cm.championship_Id == champId
                                                            select cm;
            foreach (championship_matches cm in champMatches)
            {
                db.DeleteObject(cm);
            }
            db.SaveChanges();

            IQueryable<championship_teams> champTeams = from cts in db.championship_teams
                                                        where cts.championship_Id == champId
                                                        select cts;

            List<championship_teams> champList = champTeams.ToList();
            for (int i = 0; i < champList.Count; i++)
            {
                for (int j = i + 1; j < champList.Count; j++)
                {
                    match m = new match();
                    m.team_a_Id = champList[i].team_Id;
                    m.team_b_Id = champList[j].team_Id;
                    db.AddTomatches(m);
                    db.SaveChanges();
                    championship_matches cm = new championship_matches();
                    cm.championship_Id = champId;
                    cm.match_Id = m.Id;
                    cm.type = "GROUP";
                    db.AddTochampionship_matches(cm);
                    db.SaveChanges();
                }
            }
        }

        public ActionResult GetMatches(int champId)
        {
            championship champ = (from c in db.championships
                                  where c.Id == champId
                                  select c).First();
            return View(champ);
        }

        public ActionResult Playoffs(int champId)
        {
            championship champ = (from c in db.championships
                                  where c.Id == champId
                                  select c).First();
            GeneratePlayoffMatches(champId);
            return View(champ);
        }

        public void GeneratePlayoffMatches(int champId)
        {
            fixtureHelper.GenerateChampPlayoffsMatches();

            if (fixtureHelper.playoffsMatches.Count == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    match m = new match();
                    m.team_a_Id = fixtureHelper.rows[i].Team.Id;
                    m.team_b_Id = fixtureHelper.rows[i + 2].Team.Id;
                    db.AddTomatches(m);
                    db.SaveChanges();
                    championship_matches cm = new championship_matches();
                    cm.championship_Id = champId;
                    cm.match_Id = m.Id;
                    cm.type = "SEMIFINAL";
                    db.AddTochampionship_matches(cm);
                    db.SaveChanges();

                    fixtureHelper.GenerateChampPlayoffsMatches();
                }
            }
            else
            {
                ChampTeam teamA = null;
                ChampTeam teamB = null;
                int semiCount = 0;
                bool createFinal = false;
                foreach (FixtureMatch fm in fixtureHelper.playoffsMatches)
                {
                    int teamARes = 0;
                    int teamBRes = 0;
                    if (fm.type == "SEMIFINAL")
                    {
                        if (fm.result.Count == 0)
                        {
                            createFinal = false;
                            break;
                        }
                        semiCount++;
                        foreach (MatchRes res in fm.result)
                        {
                            teamARes += res.teamA;
                            teamBRes += res.teamB;
                        }
                        if (teamARes > teamBRes)
                        {
                            if (semiCount == 1) teamA = fm.teamA;
                            else
                            {
                                teamB = fm.teamA;
                                createFinal = true;
                            }
                        }
                        else
                        {
                            if (semiCount == 1) teamA = fm.teamB;
                            else
                            {
                                teamB = fm.teamB;
                                createFinal = true;
                            }
                        }
                    }
                    else
                    {
                        if (fm.type == "FINAL")
                        {
                            createFinal = false;
                            if (fm.result.Count != 0)
                            {
                                foreach (MatchRes res in fm.result)
                                {
                                    teamARes += res.teamA;
                                    teamBRes += res.teamB;
                                }
                                championship champ = (from c in db.championships
                                                      where c.Id == champId
                                                      select c).First();
                                if (teamARes > teamBRes) champ.champion_Id = fm.teamA.Id;
                                else champ.champion_Id = fm.teamB.Id;
                                db.SaveChanges();
                            }
                            break;
                        }
                    }
                }

                if (createFinal)
                {
                    match m = new match();
                    m.team_a_Id = teamA.Id;
                    m.team_b_Id = teamB.Id;
                    db.AddTomatches(m);
                    db.SaveChanges();
                    championship_matches cm = new championship_matches();
                    cm.championship_Id = champId;
                    cm.match_Id = m.Id;
                    cm.type = "FINAL";
                    db.AddTochampionship_matches(cm);
                    db.SaveChanges();

                    fixtureHelper.GenerateChampPlayoffsMatches();
                }
            }
        }

        public static bool EnablePlayoffs()
        {
            foreach (var m in fixtureHelper.matches)
            {
                if (m.result.Count == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
