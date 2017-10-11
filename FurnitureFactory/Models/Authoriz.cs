using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FurnitureFactory.Models
{
    public static class Authoriz
    {
        public static Purchaser getAuthoriz(string username,string password,DBAgent.DBAgent agent)
        {
            Purchaser purchaser = null;
            if (agent.CheckAuthoriz(username, password))
            {
                purchaser = agent.getPurchaser(username, password);
            }
            return purchaser;
        }
    }
}