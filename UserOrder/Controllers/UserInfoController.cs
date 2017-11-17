using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.QY.AdvancedAPIs;
using Senparc.Weixin.QY.Containers;
using Senparc.Weixin.QY.AdvancedAPIs.OAuth2;
using Senparc.Weixin.QY.Entities;

namespace UserOrder.Controllers
{
    public class UserInfoController : Controller
    {
        /// <param name="corpId">企业的CorpID</param>
        /// <param name="redirectUrl">授权后重定向的回调链接地址，请使用urlencode对链接进行处理</param>
        /// <param name="agentId">企业应用的id。当scope是snsapi_userinfo或snsapi_privateinfo时，该参数必填。意redirect_uri的域名必须与该应用的可信域名一致。</param>
        /// <param name="state">重定向后会带上state参数，企业可以填写a-zA-Z0-9的参数值</param>
        /// <param name="responseType">返回类型，此时固定为：code</param>
        /// <param name="scope">应用授权作用域，此时固定为：snsapi_base</param>
        /// #wechat_redirect 微信终端使用此参数判断是否需要带上身份信息
        /// 员工点击后，页面将跳转至 redirect_uri/?code=CODE&state=STATE，企业可根据code参数获得员工的userid。
        /// 
        // GET: UserInfo
        public ActionResult GetUserInfo()
        {
            //GetUserInfoResult userInfo = new GetUserInfoResult();
            String corpId= "wxcf995f1b81a19a41";       //企业ID
            String redirectUrl= "27.211.236.58:53902";      //重定向URL
            String state="cg";            //状态参数
            String agentId = "1000003";      //企业应用ID
            String responseType = "code";       //返回类型，此时固定为：code
            String scope = "snsapi_userinfo";       //应用授权作用域，此时固定为：snsapi_base   snsapi_userinfo
            //通过授权获取到 Code
            String codeUrl=OAuth2Api.GetCode(corpId,redirectUrl,state,agentId,responseType,scope);
            ViewData["url"] = codeUrl;
            /*
            Console.WriteLine(codeUrl);
            String secret = "pxPQ6jDtAtBQf34dci2QPpcXc1nhgoBSQUQNhxXGfII";
            AccessTokenResult accessTokenResult = AccessTokenContainer.GetTokenResult(corpId,secret,false);
            String accessToken = accessTokenResult.access_token;
            Console.WriteLine(accessToken);
            */        
            return View();
        }
    }
}