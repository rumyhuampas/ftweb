using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FT.Models;
using System.Web.Mvc;

namespace FT.Extensions
{
    public class TeamsHelper
    {
        private ftEntities db;
        public List<team> selectedTeams;

        public TeamsHelper()
        {
            selectedTeams = new List<team>();
        }

        public void AddIfNotExist(team t)
        {
            bool found = false;
            for (int i = 0; i < selectedTeams.Count; i++)
            {
                if (selectedTeams[i].Id == t.Id)
                {
                    found = true;
                    break;
                }
            }
            if (found == false)
            {
                selectedTeams.Add(t);
            }
        }

        public void Clear()
        {
            selectedTeams.Clear();
        }

        public void DeleteTeam(int teamId)
        {
            for (int i = 0; i < selectedTeams.Count; i++)
            {
                if (selectedTeams[i].Id == teamId)
                {
                    selectedTeams.RemoveAt(i);
                    break;
                }
            }
        }

        public SelectList GetAllTeams()
        {
            db = new ftEntities();
            var teams = (from t in db.teams
                           select t).OrderBy(team => team.Name);
            return new SelectList(teams, "Id", "Name");
        }
    }
}