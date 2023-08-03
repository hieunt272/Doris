using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace Doris.Filters
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
                if (ticketInfo != null)
                {
                    var data = ticketInfo.UserData;
                    var arrData = data.Split('|');
                    filterContext.RouteData.Values["FullName"] = arrData[0];
                    filterContext.RouteData.Values["Level"] = arrData[1];
                    filterContext.RouteData.Values["ShopCode"] = arrData[2];
                    if (arrData.Length > 3)
                    {
                        filterContext.RouteData.Values["Avatar"] = arrData[3];
                    }
                    filterContext.RouteData.Values["MemberEmail"] = ticketInfo.Name;
                }
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
                filterContext.RouteData.Values["FullName"] = "";
                filterContext.RouteData.Values["Level"] = "";
                filterContext.RouteData.Values["ShopCode"] = "";
                filterContext.RouteData.Values["Avatar"] = "";
                filterContext.RouteData.Values["MemberEmail"] = "";

            }
            else
            {
                var ticketInfo = FormsAuthentication.Decrypt(cookie.Value);
                if (ticketInfo != null)
                {
                    var arrData = ticketInfo.UserData.Split('|');

                    filterContext.RouteData.Values["FullName"] = arrData[0];
                    filterContext.RouteData.Values["Level"] = arrData[1];
                    filterContext.RouteData.Values["ShopCode"] = arrData[2];
                    if (arrData.Length > 3)
                    {
                        filterContext.RouteData.Values["Avatar"] = arrData[3];
                    }
                    filterContext.RouteData.Values["MemberEmail"] = ticketInfo.Name;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}