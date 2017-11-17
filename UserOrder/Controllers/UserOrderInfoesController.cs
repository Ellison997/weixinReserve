using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UserOrder.Models;
using Senparc.Weixin.QY.Containers;
using Senparc.Weixin.Work.AdvancedAPIs.OAuth2;
using Senparc.Weixin.QY.Entities;
using Senparc.Weixin.Work.AdvancedAPIs;
using System.Configuration;
using Senparc.Weixin.QY.AdvancedAPIs;
using Senparc.Weixin.QY.AdvancedAPIs.MailList;

namespace UserOrder.Controllers
{
    [Route("UserOrderInfoes")]
    public class UserOrderInfoesController : Controller
    {
        
        private ApplicationDbContext db = new ApplicationDbContext();
        DateTime Tooday = DateTime.Now.Date;
        DateTime Enday = DateTime.Today.AddDays(86399F / 86400);
        String corpId = ConfigurationManager.AppSettings["CorpId"];       //企业ID
        String secret = ConfigurationManager.AppSettings["Secret"];
        int adminToDepartment = int.Parse(ConfigurationManager.AppSettings["adminToDepartment"]);       //管理部门ID
        bool ifAdmin = false;

        //获取预约管理部门人员
        [Route("GetStaff")]
        public List<string> GetStaffs() {
            int fetchChild = 0;
            int status = 0;
            int? maxJsonLength = null;
            String accessToken = AccessTokenContainer.BuildingKey(corpId, secret);
            GetDepartmentMemberResult gdmr = Senparc.Weixin.QY.AdvancedAPIs.MailListApi.GetDepartmentMember(accessToken, adminToDepartment,fetchChild, status, maxJsonLength);
            List<UserList_Simple> listSimple = gdmr.userlist;
            List<String> names = new List<string>();
            foreach (UserList_Simple us in listSimple) {
                String name = us.name;
                names.Add(name);
            }

            return names;
        }



        [Route("GetUser")]
        public ActionResult GetUser(String code) {
            if (code == null)
            {
                return RedirectToAction("Anomaly");
            }

            AccessTokenResult accessTokenResult = AccessTokenContainer.GetTokenResult(corpId, secret, false);
            String accessToken = accessTokenResult.access_token;

            //根据Code和 AccessToken 获取 user_ticket
            // GetUserInfoResult userInfo = OAuth2Api.GetUserId(accessToken,code);
            String user_ticket = Senparc.Weixin.Work.AdvancedAPIs.OAuth2Api.GetUserId(accessToken, code).user_ticket;
            GetUserDetailResult userDetail = Senparc.Weixin.Work.AdvancedAPIs.OAuth2Api.GetUserDetail(accessToken, user_ticket);
            String uName = userDetail.name;
            Session["uname"] = uName;
            return RedirectToAction("Index");

        }

        private Order GetNewestOrder() {
            Order order = (from o in db.OrderSet where o.OrderDate >= Tooday && o.OrderDate < Enday orderby o.OrderDate descending select o).FirstOrDefault();
            return order;
        }

        // GET: UserOrderInfoes
        [Route("Index")]
        public ActionResult Index()
        {
            Order order = null;
            int sum = 0;
            ViewData["uoi"] = null;
            String uname = (String)Session["uname"];

            //判断当前用户是否为管理员
            List<string> names = GetStaffs();
            ifAdmin = names.Contains(uname);
            ViewData["ifAdmin"] = ifAdmin;
            Session["ifAdmin"] = ifAdmin;



            //判断今日商家是否发布预定            
            int dateSum = (from od in db.OrderSet where od.OrderDate >= Tooday && od.OrderDate < Enday select od).Count();
            if (dateSum != 0)
            {
                //获取到今日最新的菜单
                order = GetNewestOrder();
                order.Describe = order.Describe.Replace("\n", "<br />");
                sum = (from usi in db.UserOrderInfoSet where usi.OrderId == order.Id && usi.State == "已预订" select usi).Count();
            }
            else
            {
                return RedirectToAction("NoRelease");
            }
            //判断用户今日是否预定
            
            IQueryable<UserOrderInfo> uois = db.UserOrderInfoSet.Where(o => o.UName == uname && o.Order.OrderDate >= Tooday && o.Order.OrderDate < Enday);
            int count = uois.Count();
            if (count > 0)
            {
                UserOrderInfo uoi = uois.FirstOrDefault();                
                sum = (from usi in db.UserOrderInfoSet where usi.OrderId == uoi.OrderId && usi.State == "已预订" select usi).Count();
                ViewData["uoi"] = uoi;
            }
                ViewData["sum"] = sum;

            


            return View(order);
        }
        

        [Route("Create")]
        public ActionResult Create(String uname,int oid)
        {
            IQueryable<UserOrderInfo> uois = db.UserOrderInfoSet.Where(o => o.UName == uname && o.OrderId==oid);

            if (uois.Count() > 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                UserOrderInfo uoi = new UserOrderInfo();
                uoi.UName = uname;
                uoi.OrderId = oid;
                uoi.OrderDateTime = DateTime.Now;
                uoi.State = "已预订";

                db.UserOrderInfoSet.Add(uoi);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
         }

        [Route("Update")]
        public ActionResult Update(int uid)
        {

                UserOrderInfo uoii = db.UserOrderInfoSet.Find(uid);
                uoii.State = "已预订";
                uoii.OrderDateTime = DateTime.Now;

                db.Entry(uoii).State = EntityState.Modified;
                db.SaveChanges();
           
           
            return RedirectToAction("Index");
                        
        }

        //取消预约
        [Route("Cancel")]
        public ActionResult Cancel(int uid) {
            UserOrderInfo uoii = db.UserOrderInfoSet.Find(uid);
            uoii.State = "已取消";
            uoii.CancelDateTime = DateTime.Now;
            db.Entry(uoii).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: orders/MsgSuccess 预定成功页
        private ActionResult MsgSuccess()
        {
            return View();
        }

        // GET: orders/MsgSuccess 取消预定成功页
        private ActionResult CancelSuccess()
        {
            return View();
        }

        // GET: orders/NoRelease 未发布页
        public ActionResult NoRelease()
        {
            ViewData["ifAdmin"] = Session["ifAdmin"];
            return View();
        }
        // GET: orders/Anomaly 系统异常页
        public ActionResult Anomaly() {
            ViewData["ifAdmin"] = Session["ifAdmin"];
            return View();
        }

        // GET: UserOrderInfoes/Edit/5
        private ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserOrderInfo userOrderInfo = db.UserOrderInfoSet.Find(id);
            if (userOrderInfo == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderId = new SelectList(db.OrderSet, "Id", "Describe", userOrderInfo.OrderId);
            return View(userOrderInfo);
        }

        // POST: UserOrderInfoes/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UName,OrderId")] UserOrderInfo userOrderInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userOrderInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OrderId = new SelectList(db.OrderSet, "Id", "Describe", userOrderInfo.OrderId);
            return View(userOrderInfo);
        }

        // GET: UserOrderInfoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserOrderInfo userOrderInfo = db.UserOrderInfoSet.Find(id);
            if (userOrderInfo == null)
            {
                return HttpNotFound();
            }
            return View(userOrderInfo);
        }

        // POST: UserOrderInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserOrderInfo userOrderInfo = db.UserOrderInfoSet.Find(id);
            db.UserOrderInfoSet.Remove(userOrderInfo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
