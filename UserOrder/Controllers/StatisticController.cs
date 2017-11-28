using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UserOrder.Models;

namespace UserOrder.Controllers
{
    public class StatisticController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Statistic
        public ActionResult Index()
        {
            List<Statistic> listsMonth = new List<Statistic>();
            //根据月份对数据进行分组
            List<Statistic> listsm = (from uoi in db.UserOrderInfoSet
                                      join o in db.OrderSet on uoi.OrderId equals o.Id
                                      where o.OrderDate.Year == DateTime.Now.Year && uoi.State == "已预订"
                                      orderby o.OrderDate.Month descending
                                      select new Statistic
                                      {
                                          Month = o.OrderDate.Month,
                                          PeopleN=uoi.PeopleNumber

                                      }).ToList();
            listsMonth = (from lm in listsm
                          group lm by new
                          {
                              lm.Month
                          } into m
                          select new Statistic
                          {
                              Month = m.Key.Month,
                              Amount = m.Sum(p => p.PeopleN)
                          }).ToList();

            List<Statistic> listsYear = new List<Statistic>();
            List<Statistic> listsy = (from o in db.OrderSet
                                      join uoi in db.UserOrderInfoSet on o.Id equals uoi.OrderId
                                      where uoi.State == "已预订"
                                      orderby o.OrderDate.Year descending
                                      select new Statistic
                                      {
                                          Year = o.OrderDate.Year,
                                          PeopleN = uoi.PeopleNumber

                                      }).ToList();
            listsYear = (from ly in listsy
                         group ly by new
                         {
                             ly.Year
                         } into y
                         select new Statistic
                         {
                             Year = y.Key.Year,
                             Amount = y.Sum(py => py.PeopleN)
                         }).ToList();

            ViewData["listsYear"] = listsYear;
            Statistic st = listsYear.FirstOrDefault();
            ViewData["yearAmount"] = st.Amount;

            return View(listsMonth);
        }
        //MonthDetails Get
        public ActionResult MonthDetails(int year,int month)
        {
            List<Order> listOrder = (from o in db.OrderSet where o.OrderDate.Month == month && o.OrderDate.Year == year orderby o.Id descending select o).ToList();
            

            List<Order> Orders = new List<Order>();
            foreach (Order lo in listOrder)
            {
                List<UserOrderInfo> listUoi = (from uoi in db.UserOrderInfoSet where uoi.OrderId == lo.Id && uoi.State == "已预订" select uoi).ToList();
                int Amount = 0;
                foreach (UserOrderInfo uoi in listUoi)
                {
                    Amount += uoi.PeopleNumber;
                }
                lo.Amount = Amount;
                lo.Describe = lo.Describe.Replace("\n", "<br />");

                Orders.Add(lo);
            }
            
            ViewBag.Year = year;
            String smonth = month.ToString();

            if (month < 10)
            {
                smonth = "0" + month;
            }

            ViewBag.Month = smonth;
            
            
            return View(Orders);
        }
    }
}