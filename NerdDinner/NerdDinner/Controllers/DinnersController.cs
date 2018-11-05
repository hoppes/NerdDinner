using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NerdDinner.Models;

namespace NerdDinner.Controllers
{
    public class DinnersController : Controller
    {

        DinnerRepository dinnerRepository = new DinnerRepository();

        //
        // GET: /Dinners/
        //    /Dinners/[paging]

        public ActionResult Index(int? page)
        {

            //var dinners = dinnerRepository.FindUpcomingDinners().ToList();

            const int pageSize = 5;

            var upcomingDinners = dinnerRepository.FindUpcomingDinners();
            var paginatedDinners = new PaginatedList<Dinner>(upcomingDinners, page ?? 0, pageSize);

            return View(paginatedDinners);
        }

        //
        // GET: /Dinners/Details/[ID]

        public ActionResult Details(int id)
        {

            Dinner dinner = dinnerRepository.GetDinner(id);

            if (dinner == null)
                return View("NotFound");
            else
                return View(dinner);
        }

        //
        // GET: /Dinners/Edit/[ID]
        [Authorize]
        public ActionResult Edit(int id)
        {
            Dinner dinner = dinnerRepository.GetDinner(id);

            //ViewData["Countries"] = new SelectList(PhoneValidator.Countries, dinner.Country);

            if (!dinner.IsHostedBy(User.Identity.Name))
                return View("InvalidOwner");

            return View(new DinnerFormViewModel(dinner));
        }

        //
        // POST: /Dinners/Edit/[ID]
        [AcceptVerbs(HttpVerbs.Post), Authorize]
        public ActionResult Edit(int id, FormCollection formValues)
        {

            Dinner dinner = dinnerRepository.GetDinner(id);

            if (!dinner.IsHostedBy(User.Identity.Name))
                return View("InvalidOwner");

            try
            {
                UpdateModel(dinner);
                dinnerRepository.Save();

                return RedirectToAction("Details", new { id = dinner.DinnerID });

            }
            catch
            {
                foreach (var issue in dinner.GetRuleViolations())
                {
                    ModelState.AddModelError(issue.PropertyName, issue.ErrorMessage);

                    //ViewData["Countries"] = new SelectList(PhoneValidator.Countries, dinner.Country);
                }


                return View(new DinnerFormViewModel(dinner));
            }     

        }

        //
        // GET: /Dinners/Create/
        [Authorize]
        public ActionResult Create() {

        Dinner dinner = new Dinner()
        {
            EventDate = DateTime.Now.AddDays(7)
        };

            return View(new DinnerFormViewModel(dinner));

        }

        //
        // POST: /Dinners/Create/
        [AcceptVerbs(HttpVerbs.Post), Authorize]
        public ActionResult Create(Dinner dinner)
        {

            if (ModelState.IsValid)
            {

                try
                {
                    dinner.HostedBy = User.Identity.Name;

                    RSVP rsvp = new RSVP();
                    rsvp.AttendeeName = User.Identity.Name;
                    dinner.RSVPs.Add(rsvp);

                    dinnerRepository.Add(dinner);
                    dinnerRepository.Save();

                    return RedirectToAction("Details", new { id = dinner.DinnerID });
                }
                catch
                {
                    foreach (var issue in dinner.GetRuleViolations())
                    {
                        ModelState.AddModelError(issue.PropertyName, issue.ErrorMessage);
                    }

                    return View(new DinnerFormViewModel(dinner));
                }
            }

            return View(new DinnerFormViewModel(dinner));
        }


        //
        // GET: /Dinners/Delete/[ID]
        public ActionResult Delete(int id)
        {

            Dinner dinner = dinnerRepository.GetDinner(id);

            if (dinner == null)
                return View("NotFound");
            else
                return View(dinner);
        }

        //
        // POST: /Dinners/Delete/[ID]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(int id, string confirmButton)
        {

            Dinner dinner = dinnerRepository.GetDinner(id);

            if (dinner == null)
                return View("NotFound");

            dinnerRepository.Delete(dinner);
            dinnerRepository.Save();

            return View("Deleted");
        }
    }
}