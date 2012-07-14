using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FT.Models;

namespace FT.Extensions
{
    public class FixtureHelper
    {
        private ftEntities db;
        private int champId;

        public FixtureHelper(int championshipId)
        {
            db = new ftEntities();
            champId = championshipId;
        }

        public void BuildFixture()
        {

        }
    }
}