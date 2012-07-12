using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FT.Models;
using System.Web.Mvc;

namespace FT.Extensions
{
    public class PlayersHelper
    {
        private ftEntities db;
        public List<player> selectedPlayers;

        public PlayersHelper()
        {
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

        public void DeletePlayer(int playerId)
        {
            for (int i = 0; i < selectedPlayers.Count; i++)
            {
                if (selectedPlayers[i].Id == playerId)
                {
                    selectedPlayers.RemoveAt(i);
                    break;
                }
            }
        }

        public SelectList GetAllPlayers()
        {
            db = new ftEntities();
            var players = (from p in db.players
                           select p).OrderBy(player => player.Name);
            return new SelectList(players, "Id", "Name");
        }
    }
}