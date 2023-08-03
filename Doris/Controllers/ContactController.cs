using Doris.DAL;
using Doris.Models;
using Doris.ViewModel;
using PagedList;
using System.Linq;
using System.Web.Mvc;

namespace Doris.Controllers
{
    [Authorize]
    public class ContactController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        #region Contact
        public ActionResult ListContact(int? page, string name)
        {
            var pageNumber = page ?? 1;
            const int pageSize = 15;
            var contact = _unitOfWork.ContactRepository.GetQuery(orderBy: l => l.OrderByDescending(a => a.Id));

            if (!string.IsNullOrEmpty(name))
            {
                contact = contact.Where(l => l.FullName.Contains(name));
            }
            var model = new ListContactViewModel
            {
                Contacts = contact.ToPagedList(pageNumber, pageSize),
                Name = name
            };
            return View(model);
        }
        [HttpPost]
        public bool DeleteContact(int contactId = 0)
        {
            var contact = _unitOfWork.ContactRepository.GetById(contactId);
            if (contact == null)
            {
                return false;
            }
            _unitOfWork.ContactRepository.Delete(contact);
            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region Subcribe
        public ActionResult ListSubcribe(int? page, string name)
        {
            var pageNumber = page ?? 1;
            var pageSize = 15;
            var subcribes = _unitOfWork.SubcribeRepository.GetQuery(orderBy: l => l.OrderByDescending(a => a.Id));
            if (!string.IsNullOrEmpty(name))
            {
                subcribes = subcribes.Where(l => l.Email.ToLower().Contains(name.ToLower()));
            }
            var model = new ListSubcribeViewModel
            {
                Subcribes = subcribes.ToPagedList(pageNumber, pageSize),
                Name = name
            };
            return View(model);
        }
        [HttpPost]
        public bool DeleteSubcribe(int subId = 0)
        {
            var subcribe = _unitOfWork.SubcribeRepository.GetById(subId);
            if (subcribe == null)
            {
                return false;
            }
            _unitOfWork.SubcribeRepository.Delete(subcribe);
            _unitOfWork.Save();
            return true;
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}