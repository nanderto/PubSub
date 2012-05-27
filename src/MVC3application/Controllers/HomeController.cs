using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC3application.Models;
using BusinessLogic;

namespace MVC3application.Controllers
{
    public class HomeController : Controller
    {
        private static MVC3application.Models.Users _usrs = new MVC3application.Models.Users();
        public ActionResult Index()
        {
            ViewModel.Message = "Welcome to ASP.NET MVC!";

            return View(_usrs._usrList); 

        }

        public ViewResult Details(string id) 
        { 
            return View(_usrs.GetUser(id)); 
        } 

        public ActionResult About()
        {
            return View();
        }

        public ViewResult Edit(string id) 
        {     
            return View(_usrs.GetUser(id)); 
        }   
        
        [HttpPost] 
        public ViewResult Edit(UserModel um) 
        {       
            if (!TryUpdateModel(um)) 
            {         
                ViewModel.updateError = "Update Failure";         
                return View(um);     
            }       
            // ToDo: add persistent to DB.    
            BusinessLogic.UserManagerServiceAdaptor sa = new BusinessLogic.UserManagerServiceAdaptor();
            Entities.User u = new Entities.User();
            u.FirstName = um.FirstName;
            u.LastName = um.LastName;
            sa.Update(u);
            //_usrs.Update(um);     
            return View("Details", um); 
        } 

    }
}
