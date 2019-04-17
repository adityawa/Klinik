using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.MasterData;
using Klinik.Features;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class MasterProductController : BaseController
    {
        public MasterProductController(IUnitOfWork unitOfWork, KlinikDBEntities context) :
            base(unitOfWork, context)
        {
        }

        #region ::PRODUCT::
        [CustomAuthorize("VIEW_M_PRODUCT")]
        public ActionResult ProductList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditProduct(ProductModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new ProductRequest
            {
                Data = _model
            };

            ProductResponse _response = new ProductResponse();

            new ProductValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.Organisasi = BindDropDownOrganization();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [CustomAuthorize("ADD_M_PRODUCT", "EDIT_M_PRODUCT")]
        public ActionResult CreateOrEditProduct()
        {
            ProductResponse _response = new ProductResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new ProductRequest
                {
                    Data = new ProductModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                ProductResponse resp = new ProductHandler(_unitOfWork).GetDetail(request);
                ProductModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.Organisasi = BindDropDownOrganization();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.Organisasi = BindDropDownOrganization();
                ViewBag.ActionType = ClinicEnums.Action.Add;
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetProductData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new ProductRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new ProductHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterProduct(int id)
        {
            ProductResponse _response = new ProductResponse();
            var request = new ProductRequest
            {
                Data = new ProductModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new ProductValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::PRODUCT CATEGORY::
        [CustomAuthorize("VIEW_M_PRODUCT_CATEGORY")]
        public ActionResult ProductCategoryList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditProductCategory(ProductCategoryModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new ProductCategoryRequest
            {
                Data = _model
            };

            ProductCategoryResponse _response = new ProductCategoryResponse();

            new ProductCategoryValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.Organisasi = BindDropDownOrganization();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [CustomAuthorize("ADD_M_PRODUCT_CATEGORY", "EDIT_M_PRODUCT_CATEGORY")]
        public ActionResult CreateOrEditProductCategory()
        {
            ProductCategoryResponse _response = new ProductCategoryResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new ProductCategoryRequest
                {
                    Data = new ProductCategoryModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                ProductCategoryResponse resp = new ProductCategoryHandler(_unitOfWork).GetDetail(request);
                ProductCategoryModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.Organisasi = BindDropDownOrganization();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.Organisasi = BindDropDownOrganization();
                ViewBag.ActionType = ClinicEnums.Action.Add;
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetProductCategoryData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new ProductCategoryRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new ProductCategoryHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterProductCategory(int id)
        {
            ProductCategoryResponse _response = new ProductCategoryResponse();
            var request = new ProductCategoryRequest
            {
                Data = new ProductCategoryModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new ProductCategoryValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::PRODUCT MEDICINE::
        [CustomAuthorize("VIEW_M_PRODUCT_MEDICINE")]
        public ActionResult ProductMedicineList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditProductMedicine(ProductMedicineModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new ProductMedicineRequest
            {
                Data = _model
            };

            ProductMedicineResponse _response = new ProductMedicineResponse();

            new ProductMedicineValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.Organisasi = BindDropDownOrganization();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [CustomAuthorize("ADD_M_PRODUCT_MEDICINE", "EDIT_M_PRODUCT_MEDICINE")]
        public ActionResult CreateOrEditProductMedicine()
        {
            ProductMedicineResponse _response = new ProductMedicineResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new ProductMedicineRequest
                {
                    Data = new ProductMedicineModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                ProductMedicineResponse resp = new ProductMedicineHandler(_unitOfWork).GetDetail(request);
                ProductMedicineModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.Organisasi = BindDropDownOrganization();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.Organisasi = BindDropDownOrganization();
                ViewBag.ActionType = ClinicEnums.Action.Add;
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetProductMedicineData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new ProductMedicineRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new ProductMedicineHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterProductMedicine(int id)
        {
            ProductMedicineResponse _response = new ProductMedicineResponse();
            var request = new ProductMedicineRequest
            {
                Data = new ProductMedicineModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new ProductMedicineValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::PRODUCT UNIT::
        [CustomAuthorize("VIEW_M_PRODUCT_UNIT")]
        public ActionResult ProductUnitList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditProductUnit(ProductUnitModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new ProductUnitRequest
            {
                Data = _model
            };

            ProductUnitResponse _response = new ProductUnitResponse();

            new ProductUnitValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.Organisasi = BindDropDownOrganization();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [CustomAuthorize("ADD_M_PRODUCT_UNIT", "EDIT_M_PRODUCT_UNIT")]
        public ActionResult CreateOrEditProductUnit()
        {
            ProductUnitResponse _response = new ProductUnitResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new ProductUnitRequest
                {
                    Data = new ProductUnitModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                ProductUnitResponse resp = new ProductUnitHandler(_unitOfWork).GetDetail(request);
                ProductUnitModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.Organisasi = BindDropDownOrganization();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.Organisasi = BindDropDownOrganization();
                ViewBag.ActionType = ClinicEnums.Action.Add;
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetProductUnitData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new ProductUnitRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new ProductUnitHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterProductUnit(int id)
        {
            ProductUnitResponse _response = new ProductUnitResponse();
            var request = new ProductUnitRequest
            {
                Data = new ProductUnitModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new ProductUnitValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
