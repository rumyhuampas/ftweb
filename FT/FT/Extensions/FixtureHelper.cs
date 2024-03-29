﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FT.Models;

namespace FT.Extensions
{
    public class FixtureRow
    {
        public ChampTeam Team;
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

    public class MatchRes{
        public int teamA;
        public int teamB;
    }

    public class FixtureMatch{
        public ChampTeam teamA;
        public ChampTeam teamB;
        public List<MatchRes> result;
        public string type;

        public FixtureMatch()
        {
            result = new List<MatchRes>();
        }

        public void AddRes(int teamARes, int teamBRes){
            MatchRes res = new MatchRes();
            res.teamA = teamARes;
            res.teamB = teamBRes;
            result.Add(res);
        }
    }

    public class FixtureHelper
    {
        private ftEntities db;
        private int champId;
        private championship champ;
        public List<FixtureRow> rows;
        public List<FixtureMatch> matches;
        public List<FixtureMatch> playoffsMatches;

        public FixtureHelper(int championshipId)
        {
            db = new ftEntities();
            champId = championshipId;
            champ = (from c in db.championships
                     where c.Id == champId
                     select c).First();
            rows = new List<FixtureRow>();
            matches = new List<FixtureMatch>();
            playoffsMatches = new List<FixtureMatch>();
        }

        public void BuildFixture()
        {
            GenerateChampMatches();

            IQueryable<championship_teams> champTeamsList = from ct in db.championship_teams
                                                            where ct.championship_Id == champId
                                                            select ct;

            List<championship_teams> champTeams = champTeamsList.ToList();
            foreach (championship_teams ct in champTeams)
            {
                rows.Add(GenerateTeamResults(BuildChampTeam(ct.team_Id)));
            }
            OrderTeams(rows);
        }

        private ChampTeam BuildChampTeam(int id)
        {
            ChampTeam team = new ChampTeam();
            team.Id = id;
            if (champ.Type == "SINGLE")
            {
                player pObj = (from p in db.players
                               where p.Id == team.Id
                               select p).First();
                team.Name = pObj.Name;
                team.Type = "PLAYER";
            }
            else
            {
                team tObj = (from t in db.teams
                             where t.Id == team.Id
                             select t).First();
                team.Name = tObj.Name;
                team.Type = "TEAM";
            }
            return team;
        }

        private void GenerateChampMatches()
        {
            IQueryable<match> champMatchesList = from cm in db.championship_matches
                                                 join m in db.matches on cm.match_Id equals m.Id
                                                 where cm.championship_Id == champId && cm.type == "GROUP"
                                                 select m;
            List<match> champMatches = champMatchesList.ToList();
            foreach (match m in champMatches)
            {
                FixtureMatch FMatch = new FixtureMatch();
                FMatch.teamA = BuildChampTeam(m.team_a_Id);
                FMatch.teamB = BuildChampTeam(m.team_b_Id);
                FMatch.type = "GROUP";

                var matchResults = (from mr in db.match_results
                                    where mr.match_Id == m.Id
                                    select mr).OrderBy(mr => mr.Set);

                foreach (var item in matchResults)
                {
                    FMatch.AddRes(item.team_a_games, item.team_b_games);
                }

                matches.Add(FMatch);
            }
        }

        private FixtureRow GenerateTeamResults(ChampTeam team)
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

        public void GenerateChampPlayoffsMatches()
        {
            playoffsMatches.Clear();

            IQueryable<match> semiMatches = from cm in db.championship_matches
                                            join m in db.matches on cm.match_Id equals m.Id
                                            where cm.championship_Id == champId && cm.type == "SEMIFINAL"
                                            select m;
            List<match> semiMatchesList = semiMatches.ToList();
            foreach (match m in semiMatchesList)
            {
                FixtureMatch FMatch = new FixtureMatch();
                FMatch.teamA = BuildChampTeam(m.team_a_Id);
                FMatch.teamB = BuildChampTeam(m.team_b_Id);
                FMatch.type = "SEMIFINAL";

                var matchResults = (from mr in db.match_results
                                    where mr.match_Id == m.Id
                                    select mr).OrderBy(mr => mr.Set);

                foreach (var item in matchResults)
                {
                    FMatch.AddRes(item.team_a_games, item.team_b_games);
                }

                playoffsMatches.Add(FMatch);
            }

            IQueryable<match> finalMatchList = from cm in db.championship_matches
                                               join m in db.matches on cm.match_Id equals m.Id
                                               where cm.championship_Id == champId && cm.type == "FINAL"
                                               select m;

            match finalMatchObj = null;
            if (finalMatchList.Count() > 0)
            {
                finalMatchObj = finalMatchList.First();
                FixtureMatch FinalMatch = new FixtureMatch();
                FinalMatch.teamA = BuildChampTeam(finalMatchObj.team_a_Id);
                FinalMatch.teamB = BuildChampTeam(finalMatchObj.team_b_Id);
                FinalMatch.type = "FINAL";

                var finalMatchResults = (from mr in db.match_results
                                         where mr.match_Id == finalMatchObj.Id
                                         select mr).OrderBy(mr => mr.Set);

                foreach (var item in finalMatchResults)
                {
                    FinalMatch.AddRes(item.team_a_games, item.team_b_games);
                }

                playoffsMatches.Add(FinalMatch);
            }
        }
    }
}