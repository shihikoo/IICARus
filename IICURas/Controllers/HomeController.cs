using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using IICURas.Authorization;
using IICURas.Models.ViewModels;

namespace IICURas.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {

            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }


            if (User.IsInRole("Temp")){
                ViewBag.Message = "The trail has not yet started. Please check the website again on March 16, 2015. Thank you.";     
            }

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Reward()
        {
            return View();
        }

        public ActionResult ReviewFAQ()
        {
            return View();
        }

        [Authorize(Roles = "Administrator, Randomizer, uploader")]
        public ActionResult About()
        {
            return View();
        }

        [Authorize(Roles = "Administrator, Randomizer, uploader")]
        public ActionResult FAQ()
        {
            return View();
        }

        [Authorize]
        public ActionResult Instruction()
        {
            return View();
        }

        private readonly Models.IICURasContext _db = new Models.IICURasContext();

        [Authorize(Roles = "Administrator")]
        public bool AcceptanceInput()
        {
            var path = Server.MapPath("~/acceptanceInput.txt");

            List<string> pubNumbers = new List<string>();
            List<string> desicions = new List<string>();
            var nlines = 0;
                using (var readFile = new StreamReader(path))
                {
                    string line;

                    while ((line = readFile.ReadLine()) != null)
                    {
                        var row = line.Split('\t');
                        pubNumbers.Add((row[0]).Substring(0,15));
                        desicions.Add(row[1]);
                        nlines++;
                    }
                }


            var recoreds = _db.Records.Where(r => r.DeleteRecord == false);

            for (int ii = 0; ii < nlines; ii++)
            {
                var pubNumber = pubNumbers[ii];
                var record = recoreds.Where(
                        r => r.PaperNumber == pubNumber);

                if (!record.Any()) continue;
                foreach (var rec in record)
                {
                    rec.MNAcceptance = desicions[ii];
                }
            }
            _db.SaveChanges();
            return true;
        }

        [Authorize(Roles = "Administrator")]
        public bool COIInput()
        {
            var path = Server.MapPath("~/COIFunding.txt");

            List<string> pubNumbers = new List<string>();
            List<string> coi = new List<string>();
            List<string> funding = new List<string>();
            var nlines = 0;

            using (var readFile = new StreamReader(path))
            {

                string line;

                while ((line = readFile.ReadLine()) != null)
                {
                    var values = line.Split('\t');
                    var nvalue = values.Count();


                    pubNumbers.Add((values[0]).Substring(0, 15));
                    coi.Add(values[1]);
                    funding.Add(values[2]);
                    nlines++;
                }
            }

            var recoreds = _db.Records.Where(r => r.DeleteRecord == false);

            for (int ii = 0; ii < nlines; ii++)
            {
                var pubNumber = pubNumbers[ii];
                var record = recoreds.Where(
                        r => r.PaperNumber == pubNumber);

                if (!record.Any()) continue;
                foreach (var rec in record)
                {
                    rec.ConflicOfIntest = coi[ii];
                    rec.Funding = funding[ii];
                }
            }

            _db.SaveChanges();

            return true;
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Report()
        {
            var reportVM = new ReportViewModel(_db.UserProfiles.ToList(),
                _db.Records.Where(r => r.DeleteRecord == false && r.ReviewCompletions.Any(rc => rc.Status == Enums.Status.Current.ToString() && rc.ComplesionDate != null)).ToList());

            return View(reportVM);
        }

    }
}
