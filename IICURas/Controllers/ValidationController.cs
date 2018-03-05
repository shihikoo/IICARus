using System.Web.Mvc;
using System.Web.UI;
using System.Linq;

namespace IICURas.Controllers
{
    
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    [AllowAnonymous]
    public class ValidationController : Controller
    {   
        public JsonResult ValidatePaperNumber(string paperNumber)
        {
            using (var db = new IICURas.Models.IICURasContext())
            {
                var result = db.Records.Any(r => r.PaperNumber == paperNumber);
                return Json(db.Records.Any(r => r.PaperNumber == paperNumber), JsonRequestBehavior.AllowGet);
            }
        }
           
    }
}




