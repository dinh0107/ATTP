using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace ATTP.Filters
{
    public class MemberFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookie = filterContext.HttpContext.Request.Cookies[".ASPXAUTHMEMBER"];
            if (cookie == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {{"action", "Login"}, {"controller", "User"}});
            }
            else
            {
                var ticketInfo = FormsAuthentication.Decrypt(cookie.Value);
                var data = ticketInfo.UserData;
                filterContext.RouteData.Values["UserName"] = data.Split('|')[0];
                filterContext.RouteData.Values["Avatar"] = data.Split('|')[1];
                filterContext.RouteData.Values["Id"] = data.Split('|')[2];
                filterContext.RouteData.Values["Email"] = ticketInfo.Name;
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class MemberLoginFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookie = filterContext.HttpContext.Request.Cookies[".ASPXAUTHMEMBER"];
            if (cookie == null)
            {
                filterContext.RouteData.Values["Username"] = "";
                filterContext.RouteData.Values["Avatar"] = "";
                filterContext.RouteData.Values["Id"] = null;
                filterContext.RouteData.Values["Email"] = "";
            }
            else
            {
                var ticketInfo = FormsAuthentication.Decrypt(cookie.Value);
                var arrData = ticketInfo?.UserData.Split('|');
                filterContext.RouteData.Values["Username"] = arrData[0];
                filterContext.RouteData.Values["Avatar"] = arrData[1];
                filterContext.RouteData.Values["Id"] = arrData[2];
                filterContext.RouteData.Values["Email"] = ticketInfo.Name;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}