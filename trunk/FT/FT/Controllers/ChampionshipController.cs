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
        public ActionResult Create(championship champObj, string btnSubmit)
        {
            try
            {
                ChampionshipController.champHelper = champObj;

                if (btnSubmit == "AddTeam")
                {
                    int teamId = Convert.ToInt32(Request["teamId"]);
                    team t = (from teams in db.teams
                                where teams.Id == teamId
                                select teams).First();
                    ChampionshipController.teamsHelper.AddIfNotExist(t);
                    return View(champObj);
                }

                db.AddTochampionships(champObj);
                db.SaveChanges();
                championship_teams ct = null;
                foreach (team t in ChampionshipController.teamsHelper.selectedTeams)
                {
                    ct = new championship_teams();
                    ct.championship_Id = champObj.Id;
                    ct.team_Id = t.Id;
                    db.AddTochampionship_teams(ct);
                }
                db.SaveChanges();
                ChampionshipController.teamsHelper = null;
                ChampionshipController.champHelper = null;

                return RedirectToAction("List");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult DeleteTeam(int teamId)
        {
            ChampionshipController.teamsHelper.DeleteTeam(teamId);
            return RedirectToAction("Create");
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
            try
            {
                IQueryable<championship_teams> list = from cts in db.championship_teams
                                                      where cts.championship_Id == id
                                                      select cts;
                foreach (championship_teams ct in list)
                {
                    db.DeleteObject(ct);
                }

                championship c = (from champs in db.championships
                                  where champs.Id == id
                                  select champs).First();
                db.DeleteObject(c);
                db.SaveChanges();

                return RedirectToAction("List");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Championship/GenerateMatches/5

        public ActionResult GenerateMatches(int champId)
        {
            try
            {
                IQueryable<championship_teams> champTeams = from cts in db.championship_teams
                                                      where cts.championship_Id == champId
                                                      select cts;

                foreach (championship_teams ct in champTeams)
                {
                    int teamId = ct.team_Id;
                    IQueryable<championship_teams> otherChampTeams = from cts in db.championship_teams
                                                          where cts.championship_Id == champId && cts.team_Id != teamId
                                                          select cts;
                    foreach (championship_teams oct in otherChampTeams)
                    {
                        IQueryable<match> matchs = from m in db.matches
                                                   where (m.team_a_Id == teamId && m.team_b_Id != oct.team_Id) ||
                                                   (m.team_b_Id == teamId && m.team_a_Id != oct.team_Id)
                                                   select m;
                        if (matchs.Count() == 0)
                        {
                            match m = new match();
                            m.team_a_Id = teamId;
                            m.team_b_Id = oct.team_Id;
                            db.AddTomatches(m);
                            db.SaveChanges();

                            championship_matches cm = new championship_matches();
                            cm.championship_Id = champId;
                            cm.match_Id = m.Id;
                            db.SaveChanges();
                        }
                    }
                }

                /*IQueryable<championship_teams> list = from cts in db.championship_teams
                                                      where cts.championship_Id == id
                                                      select cts;
                foreach (championship_teams ct in list)
                {
                    db.DeleteObject(ct);
                }

                championship c = (from champs in db.championships
                                  where champs.Id == id
                                  select champs).First();
                db.DeleteObject(c);
                db.SaveChanges();*/

                return RedirectToAction("List");
            }
            catch
            {
                return View();
            }
        }
    }
}
