using Doris.DAL;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Doris.Models
{

    public class UserFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                using (var entities = new DataEntities())
                {
                    var id = (FormsIdentity)filterContext.HttpContext.User.Identity;
                    var ticket = id.Ticket;
                    var password = ticket.UserData;
                    var email = HttpContext.Current.User.Identity.Name;

                    var author = entities.Users.SingleOrDefault(a => a.Email == email && a.Password == password);
                    if (author != null)
                    {
                        filterContext.RouteData.Values["User"] = author;
                    }
                    else
                    {
                        filterContext.HttpContext.Response.Redirect("/");
                    }
                }
            }
            else
            {
                filterContext.HttpContext.Response.Redirect("/");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}