using System.Collections.Generic;
using System;
using System.Linq;
using System.Web.Mvc;
using Doris.DAL;

namespace Doris.Controllers 
{
    public class BaseController : Controller 
    {
        public readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public SelectList CitySelectList => new SelectList(_unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort)), "Id", "Name");
        public SelectList DistrictSelectList(int? cityId) => new SelectList(_unitOfWork.DistrictRepository.Get(a => a.Active && a.CityId == cityId, q => q.OrderBy(a => a.Sort)), "Id", "Name");
        public SelectList WardSelectList(int? districtId) => new SelectList(_unitOfWork.WardRepository.Get(a => a.Active && a.DistrictId == districtId, q => q.OrderBy(a => a.Sort)), "Id", "Name"); 
         
        public JsonResult GetCities(string city ="")
        { 
            var cities = _unitOfWork.CityRepository
                .GetQuery(a => a.Active && a.Name.ToLower().Contains(city.ToLower()), q => q.OrderBy(a => a.Sort)).Select(a => new { a.Id, a.Name });
            return Json(cities, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDistrict(int? cityId)
        {
            var districts = _unitOfWork.DistrictRepository
                .GetQuery(a => a.Active && a.CityId == cityId, q => q.OrderBy(a => a.Sort)).Select(a => new { a.Id, a.Name });
            return Json(districts, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWard(int? districtId)
        {
            var wards = _unitOfWork.WardRepository
                .GetQuery(a => a.Active && a.DistrictId == districtId, q => q.OrderBy(a => a.Sort)).Select(a => new { a.Id, a.Name });
            return Json(wards, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}