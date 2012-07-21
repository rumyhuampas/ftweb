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
        public static FixtureHelper fixtureHelper;

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

                if (ChampionshipController.teamsHelper.selectedTeams.Count > 3)
                {
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
            catch
            {
                return View();
            }
        }

        public ActionResult DeleteTeam(int teamId)
        {
            ChampionshipController.teamsHelper.DeleteTeam(teamId);
            return RedirectToAction("Create", "Championship");
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
 
        public ActionResult Delete(int champId)
        {
            try
            {
                IQueryable<championship_teams> list = from cts in db.championship_teams
                                                      where cts.championship_Id == champId
                                                      select cts;
                foreach (championship_teams ct in list)
                {
                    db.DeleteObject(ct);
                }

                IQueryable<championship_matches> champMatches = from cm in db.championship_matches
                                                                where cm.championship_Id == champId
                                                                select cm;
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
                return RedirectToAction("Index", "Championship").WithFlash(new { msgerror = ex.Message });
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
            return View(champ);
        }
    }
}
