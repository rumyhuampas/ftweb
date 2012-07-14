using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FT.Models;

namespace FT.Extensions
{
    public class FixtureRow
    {
        public team Team;
        public int Played;
        public int Won;
        public int Lost;
        public int SetsWon;
        public int SetsLost;
        public int SetsDiff { get { return SetsWon - SetsLost; } }
        public int GamesWon;
        public int GamesLost;
        public int GamesDiff { get { return GamesWon - GamesLost; } }
        public int Points;
    }

    public class FixtureHelper
    {
        private ftEntities db;
        private int champId;
        public List<FixtureRow> rows;

        public FixtureHelper(int championshipId)
        {
            db = new ftEntities();
            champId = championshipId;
            rows = new List<FixtureRow>();
        }

        public void BuildFixture()
        {
            IQueryable<championship_teams> champTeamsList = from ct in db.championship_teams
                                                            where ct.championship_Id == champId
                                                            select ct;

            List<championship_teams> champTeams = champTeamsList.ToList();
            foreach (championship_teams ct in champTeams)
            {
                rows.Add(GenerateTeamResults(ct.team));
            }
            OrderTeams(rows);
        }

        private FixtureRow GenerateTeamResults(team team)
        {
            FixtureRow row = new FixtureRow();
            row.Team = team;

            var teamResults = (from champT in db.championship_teams
                               join champM in db.championship_matches on champT.championship_Id equals champM.championship_Id
                               join m in db.matches on champM.match_Id equals m.Id
                               join mr in db.match_results on m.Id equals mr.match_Id
                               where champT.championship_Id == champId
                               && champT.team_Id == team.Id
                               && (m.team_a_Id == champT.team_Id || m.team_b_Id == champT.team_Id)
                               select new { m.Id, m.team_a_Id, m.team_b_Id, mr.Set, mr.team_a_games, mr.team_b_games }).OrderBy(m => m.Id);

            int currentMatch = -1;
            int setCount = 0;
            foreach (var teamRow in teamResults)
            {
                if (currentMatch == -1) currentMatch = teamRow.Id;
                if (currentMatch != teamRow.Id)
                {
                    row.Played++;
                    if (setCount > 0)
                    {
                        row.Won++;
                        row.Points += 3;
                    }
                    else
                    {
                        row.Lost++;
                    }

                    currentMatch = teamRow.Id;
                    setCount = 0;
                }
                if (team.Id == teamRow.team_a_Id)
                {
                    //team A
                    row.GamesWon += teamRow.team_a_games;
                    row.GamesLost += teamRow.team_b_games;
                    if (teamRow.team_a_games > teamRow.team_b_games)
                    {
                        row.SetsWon++;
                        setCount++;
                    }
                    else
                    {
                        row.SetsLost++;
                        setCount--;
                    }
                }
                else
                {
                    //team B
                    row.GamesWon += teamRow.team_b_games;
                    row.GamesLost += teamRow.team_a_games;
                    if (teamRow.team_b_games > teamRow.team_a_games)
                    {
                        row.SetsWon++;
                        setCount++;
                    }
                    else
                    {
                        row.SetsLost++;
                        setCount--;
                    }
                }
            }
            if (teamResults.ToList().Count > 0)
            {
                row.Played++;
                if (setCount > 0)
                {
                    row.Won++;
                    row.Points += 3;
                }
                else
                {
                    row.Lost++;
                }
            }

            return row;
        }

        private void OrderTeams(List<FixtureRow> rows)
        {
            bool changed = false;
            bool doChange = false;
            do
            {
                changed = false;
                for (int i = 0; i < rows.Count - 1; i++)
                {
                    doChange = false;
                    if (rows[i].Points < rows[i + 1].Points) doChange = true;
                    if (rows[i].Points == rows[i + 1].Points && rows[i].SetsDiff < rows[i + 1].SetsDiff) doChange = true;
                    if (rows[i].Points == rows[i + 1].Points && rows[i].SetsDiff == rows[i + 1].SetsDiff && rows[i].GamesDiff < rows[i + 1].GamesDiff) doChange = true;

                    if (doChange == true)
                    {
                        FixtureRow aux = rows[i + 1];
                        rows[i + 1] = rows[i];
                        rows[i] = aux;
                        changed = true;
                    }
                }
            }
            while (changed == true);
        }
    }
}