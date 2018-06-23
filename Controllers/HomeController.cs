using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BeltExam.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BeltExam.Controllers
{
    public class HomeController : Controller
    {
        private BeltContext _context;
        public HomeController(BeltContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View("LogReg");
        }
        [Route("home")]
        public IActionResult Home()
        {
            int? Id = HttpContext.Session.GetInt32("LoggedUser");
            if(Id != null)
            {
                User LoggedUser = _context.Users.SingleOrDefault(user => user.UserId == Id);
                ViewBag.name = LoggedUser.FirstName;
                ViewBag.UserId = Id;
                List<Plan> Plans = _context.Plans.Include(p => p.Participants).ThenInclude(p => p.User).OrderBy(d => d.Start).ToList();
                foreach(var plan in Plans)
                {
                    if(plan.Start < DateTime.Now)
                    {
                        Plans.Remove(plan);
                    }
                }
                ViewBag.Activities = Plans;
                ViewBag.Now = DateTime.Now;
                List<Plan> myPlans = new List<Plan>();
                foreach(var plan in Plans)
                {
                    foreach(var person in plan.Participants)
                    {
                        if(person.UserId == Id)
                        {
                            myPlans.Add(plan);
                        }
                    }
                    if(plan.CreatorId == Id)
                    {
                        myPlans.Add(plan);
                    }
                }
                List<int> overlaps = new List<int>();
                foreach(var activity in Plans)
                {
                    foreach(var plan in myPlans)
                    {
                        if(plan.Start < activity.End && activity.Start < plan.End)
                        {
                            overlaps.Add(activity.PlanId);
                        }
                    }
                }
                ViewBag.overlaps = overlaps;
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [Route("new_activity")]
        public IActionResult NewActivity()
        {
            int? Id = HttpContext.Session.GetInt32("LoggedUser");
            if(Id != null)
            {
                User LoggedUser = _context.Users.SingleOrDefault(user => user.UserId == Id);
                ViewBag.name = LoggedUser.FirstName;
                ViewBag.UserId = Id;
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [Route("create_activity")]
        public IActionResult CreateActivity(ValidPlan newplan)
        {
            int? Id = HttpContext.Session.GetInt32("LoggedUser");
            if(Id != null)
            {
                if(ModelState.IsValid)
                {
                    TimeSpan ts = new TimeSpan(0, 0, 0);
                    if(newplan.Unit == "Hours")
                    {
                        int duration = Int32.Parse(newplan.Duration);
                        ts = new TimeSpan(duration, 0, 0);
                    }
                    else if(newplan.Unit == "Minutes")
                    {
                        int duration = Int32.Parse(newplan.Duration);
                        ts = new TimeSpan(0, duration, 0);
                    }
                    else if(newplan.Unit == "Days")
                    {
                        int duration = Int32.Parse(newplan.Duration);
                        ts = new TimeSpan(duration, 0, 0, 0);
                    }
                    User LoggedUser = _context.Users.SingleOrDefault(user => user.UserId == Id);
                    DateTime end = newplan.Start.Add(ts);
                    Plan plan = new Plan
                    {
                        CreatorId = (int)Id,
                        Title = newplan.Title,
                        Coordinator = LoggedUser.FirstName,
                        Start = newplan.Start,
                        End = end,
                        Duration = newplan.Duration + " " + newplan.Unit,
                        Description = newplan.Description,
                        created_at = DateTime.Now
                    };
                    _context.Add(plan);
                    _context.SaveChanges();
                    List<Plan> Plans = _context.Plans.OrderByDescending(w => w.created_at).ToList();
                    int thisPlan = Plans.First().PlanId;
                    return Redirect($"activity/{thisPlan}");
                }
                else
                {
                    return View("NewActivity");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        [Route("activity/{id}")]
        public IActionResult Wedding(int id)
        {
            Plan RetrievedPlan = _context.Plans.Include(w => w.Participants).ThenInclude(g => g.User).SingleOrDefault(plan => plan.PlanId == id);
            ViewBag.activity = RetrievedPlan;
            int? UserId = HttpContext.Session.GetInt32("LoggedUser");
            bool flag = false;
            foreach(var person in RetrievedPlan.Participants)
            {
                if(person.UserId == UserId)
                {
                    flag = true;
                }
            }
            ViewBag.flag = flag;
            ViewBag.UserId = UserId;
            return View("Activity");
        }
        [HttpGet]
        [Route("join/{id}")]
        public IActionResult RSVP(int id)
        {
            int? UserId = HttpContext.Session.GetInt32("LoggedUser");
            User LoggedUser = _context.Users.SingleOrDefault(user => user.UserId == UserId);
            RSVP newRSVP = new RSVP
            {
                UserId = (int)UserId,
                PlanId = id,
            };
            _context.Add(newRSVP);
            _context.SaveChanges();
            return RedirectToAction("Home");
        }
        [HttpGet]
        [Route("leave/{id}")]
        public IActionResult unRSVP(int id)
        {
            Plan RetrievedPlan = _context.Plans.Include(w => w.Participants).SingleOrDefault(wedding => wedding.PlanId == id);
            int? UserId = HttpContext.Session.GetInt32("LoggedUser");
            RSVP thisRSVP = RetrievedPlan.Participants.SingleOrDefault(g => g.UserId == UserId);
            _context.RSVPs.Remove(thisRSVP);
            _context.SaveChanges();
            return RedirectToAction("Home");
        }
        [HttpGet]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            int? UserId = HttpContext.Session.GetInt32("LoggedUser");
            Plan RetrievedPlan = _context.Plans.SingleOrDefault(plan => plan.PlanId == id);
            if(UserId == RetrievedPlan.CreatorId)
            {
                _context.Plans.Remove(RetrievedPlan);
                _context.SaveChanges();
                return RedirectToAction("Home");
            }
            else
            {
                return RedirectToAction("Home");
            }
        }
        [HttpPost]
        [Route("create")]
        public IActionResult Create(Register newuser)
        {
            if(ModelState.IsValid)
            {
                PasswordHasher<Register> Hasher = new PasswordHasher<Register>();
                newuser.Password = Hasher.HashPassword(newuser, newuser.Password);
                User createuser = new User
                {
                    FirstName = newuser.FirstName,
                    LastName = newuser.LastName,
                    Email = newuser.Email,
                    Password = newuser.Password
                };
                _context.Add(createuser);
                _context.SaveChanges();
                User LoggedUser = _context.Users.SingleOrDefault(user => user.Email == newuser.Email);
                HttpContext.Session.SetInt32("LoggedUser", LoggedUser.UserId);
                return RedirectToAction("Home");
            }
            else
            {
                return View("LogReg");
            }
        }
        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [Route("user_login")]
        public IActionResult UserLogin(string login_email, string login_password)
        {
            User LoggedUser = _context.Users.SingleOrDefault(user => user.Email == login_email);
            if(LoggedUser != null && login_password != null)
            {
                var Hasher = new PasswordHasher<User>();
                if(0 != Hasher.VerifyHashedPassword(LoggedUser, LoggedUser.Password, login_password))
                {
                    HttpContext.Session.SetInt32("LoggedUser", LoggedUser.UserId);
                    return RedirectToAction("Home");
                }
                else
                {
                    ViewBag.errors = "Login information is incorrect";
                    return View("LogReg");
                }
            }
            else
            {
                ViewBag.errors = "Login information is incorrect";
                return View("LogReg");
            }
        }
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
