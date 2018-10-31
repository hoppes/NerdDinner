using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NerdDinner.Controllers
{
    public class DinnersController : Controller
    {
        // GET: Dinners
        public void Index()
        {
            Response.Write("<h1>Coming Soon</h1>");
        }

        // GET: /Dinners/Details/[Id]
        public void Details(int id)
        {
            Response.Write("<h1>Details DinnerID: " + id + "<h1>");
        }
    }
}