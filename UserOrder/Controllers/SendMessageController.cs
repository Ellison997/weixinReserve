using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.QY.AdvancedAPIs;
using Senparc.Weixin.QY.Entities;
using Senparc.Weixin.QY.Containers;
using Senparc.Weixin.QY.AdvancedAPIs.Mass;
using System.Configuration;

namespace UserOrder.Controllers
{
    public class SendMessageController : Controller
    {
        // GET: SendMessage
        public void SendTest(string content,string toParty )
        {
            String corpId = ConfigurationManager.AppSettings["CorpId"];         //企业ID
            String secret = ConfigurationManager.AppSettings["Secret"];
            String agentId = ConfigurationManager.AppSettings["AgentId"];


            String accessToken=AccessTokenContainer.BuildingKey(corpId, secret);
            int safe = 0;
            String toUser = ConfigurationManager.AppSettings["toUser"];
            String toTag = ConfigurationManager.AppSettings["toTag"];
            int timeOut = 10000;
            MassResult massResult= MassApi.SendText(accessToken, agentId, content, toUser, toParty,toTag,safe,timeOut);

        }
    }
}