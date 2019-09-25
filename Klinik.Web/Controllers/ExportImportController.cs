using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Klinik.Data.DataRepository;
using OfficeOpenXml;
using Klinik.Entities.MasterData;
using System.Data;
using ClosedXML.Excel;
using System.IO;
using System.Data.Entity.Validation;
using LinqToExcel;
using System.Data.OleDb;

namespace Klinik.Web.Controllers
{
    public class ExportImportController : Controller
    {
        private KlinikDBEntities db = new KlinikDBEntities();
        // GET: ExportImport
        public ActionResult Index()
        {
            return View();
        }
        #region ::Gudang::
        [HttpPost]
        public FileResult ExportGudang()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Gudang");
            dt.Columns.AddRange(new DataColumn[2] { new DataColumn("name"),
                                            new DataColumn("ClinicId")

            });

            var sales = from sale in entities.Gudangs.Take(11)
                        select sale;

            foreach (var sale in sales)
            {
                dt.Rows.Add(sale.name, sale.ClinicId);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Gudang.xlsx");
                }
            }
        }
        [HttpPost]
        public ActionResult ImportGudang()
        {

            int count = 0;
            var result = false;
            String path = Server.MapPath("/") + "\\DataImport\\Gudang.xlsx";
            var package = new ExcelPackage(new System.IO.FileInfo(path));
            int startColumn = 1;
            int startRow = 1;
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            object data = null;
            KlinikDBEntities db = new KlinikDBEntities();
            do
            {
                data = worksheet.Cells[startRow, startColumn].Value;
                object column1 = worksheet.Cells[startRow + 1, startColumn].Value;
                object column2 = worksheet.Cells[startRow + 1, startColumn+1].Value;
                //object createddate = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                if (data != null && column1 != null)
                {
                    var isSuccess = saveClassG(column1.ToString(), column2.ToString(),/*Convert.ToDateTime(createddate),*/ db);
                    if (isSuccess)
                    {
                        count++;
                    }
                }
                startRow++;
            }
            while (data != null);

            return RedirectToAction("GudangList", "MasterData");
        }
        public bool saveClassG(String column1, String column2,/*DateTime createddate, */KlinikDBEntities db)
        {
            var result = false;
            //createddate = DateTime.Now;

            if (db.Gudangs.Where(t => t.name.Equals(column1)).Count() == 0)
            {
                var item = new Gudang();
                item.name = column1;
                item.ClinicId = Convert.ToInt64(column2);
                item.RowStatus = 0;
                item.CreatedBy = Convert.ToString(Session["Code"]);
                item.CreatedDate = DateTime.Now;
                db.Gudangs.Add(item);
                db.SaveChanges();

            }


            return result;
        }
        #endregion
        #region ::Service::
        [HttpPost]
        public FileResult ExportService()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Sheet1");
            dt.Columns.AddRange(new DataColumn[3] { new DataColumn("Code"),
                                            new DataColumn("Name"),
                                            new DataColumn("Price")
            });

            var services = from service in entities.Services.Take(11)
                           select service;

            foreach (var servis in services)
            {
                dt.Rows.Add(servis.Code, servis.Name, servis.Price);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Services.xlsx");
                }
            }
        }
        public ActionResult ImportService()
        {
            List<string> datax = new List<string>();
            int count = 0;
            
                String path = Server.MapPath("/") + "\\DataImport\\Services.xlsx";
                var package = new ExcelPackage(new System.IO.FileInfo(path));
                int startColumn = 1;
                int startRow = 1;
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                object data = null;
                KlinikDBEntities db = new KlinikDBEntities();
                do
                {
                    data = worksheet.Cells[startRow, startColumn].Value;
                    object code = worksheet.Cells[startRow + 1, startColumn].Value;
                    object name = worksheet.Cells[startRow + 1, startColumn + 1].Value;
                    object price = worksheet.Cells[startRow + 1, startColumn + 2].Value;
                    object row = worksheet.Cells[startRow + 1, startColumn + 3].Value;
                    object createdby = worksheet.Cells[startRow + 1, startColumn + 4].Value;
                    object createddate = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                    if (data != null && name != null)
                    {
                        var isSuccess = saveClass(name.ToString(), code.ToString(), price.ToString(), 1, "SYSTEM", Convert.ToDateTime(createddate), db);
                        if (isSuccess)
                        {
                            count++;
                        }
                    }
                    startRow++;
                }
                while (data != null);
            
            return RedirectToAction("ServiceList", "MasterData");
        }
        public bool saveClass(String name, String code, String price, Int16 row, String createdby, DateTime createddate, KlinikDBEntities db)
        {
            var result = false;
            createddate = DateTime.Now;
            
                if (db.Services.Where(t => t.Name.Equals(name)).Count() == 0)
                {
                    var item = new Service();
                    item.Code = code;
                    item.Name = name;
                    item.Price = Convert.ToDecimal(price);
                    item.RowStatus = 0;
                    item.CreatedBy = Convert.ToString(Session["Code"]);
                    item.CreatedDate = DateTime.Now;
                    db.Services.Add(item);
                    db.SaveChanges();

                }

           
            return result;
        }
        #endregion
        #region ::PoliService::
        [HttpPost]
        public FileResult ExportPoliService()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("PoliService");
            dt.Columns.AddRange(new DataColumn[3] { new DataColumn("ServicesID"),
                                            new DataColumn("ClinicID"),
                                            new DataColumn("PoliID")
            });

            var pservices = from pservice in entities.PoliServices.Take(11)
                            select pservice;

            foreach (var pservis in pservices)
            {
                dt.Rows.Add(pservis.ServicesID, pservis.ClinicID, pservis.PoliID);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PoliService.xlsx");
                }
            }
        }
        public ActionResult ImportPoliService()
        {
            List<string> datax = new List<string>();
            int count = 0;
            
                String path = Server.MapPath("/") + "\\DataImport\\PoliService.xlsx";
                var package = new ExcelPackage(new System.IO.FileInfo(path));
                int startColumn = 1;
                int startRow = 1;
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                object data = null;
                KlinikDBEntities db = new KlinikDBEntities();
                do
                {
                    data = worksheet.Cells[startRow, startColumn].Value;
                    object column1 = worksheet.Cells[startRow + 1, startColumn].Value;
                    object column2 = worksheet.Cells[startRow + 1, startColumn + 1].Value;
                    object column3 = worksheet.Cells[startRow + 1, startColumn + 2].Value;
                    if (data != null )
                    {
                        var isSuccess = saveClassPS(column1.ToString(), column2.ToString(), column3.ToString(), db);
                        if (isSuccess)
                        {
                            count++;
                        }
                    }
                    startRow++;
                }
                while (data != null);
           
            return RedirectToAction("PoliServiceList", "MasterData");
        }
        public bool saveClassPS(String column1, String column2, String column3, KlinikDBEntities db)
        {
            var result = false;
            //createddate = DateTime.Now;
            
            


               
                    var item = new PoliService();
                    item.ServicesID = Convert.ToInt32(column1);
                    item.ClinicID = Convert.ToInt64(column2);
                    item.PoliID = Convert.ToInt32(column3);
                    item.RowStatus = 0;
                    item.CreatedBy = Convert.ToString(Session["Code"]);
                    item.CreatedDate = DateTime.Now;
                    db.PoliServices.Add(item);
                    db.SaveChanges();

                

           
            return result;
        }
        #endregion
        #region ::Poli::
        [HttpPost]
        public FileResult ExportPoli()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Poli");
            dt.Columns.AddRange(new DataColumn[3] { new DataColumn("Code"),
                                            new DataColumn("Name"),
                                            new DataColumn("Type")
            });

            var polis = from poli in entities.Polis.Take(11)
                        select poli;

            foreach (var poli in polis)
            {
                dt.Rows.Add(poli.Code, poli.Name, poli.Type);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Poli.xlsx");
                }
            }
        }
        public ActionResult ImportPoli()
        {

            int count = 0;
            var result = false;
            String path = Server.MapPath("/") + "\\DataImport\\Poli.xlsx";
            var package = new ExcelPackage(new System.IO.FileInfo(path));
            int startColumn = 1;
            int startRow = 1;
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            object data = null;
            KlinikDBEntities db = new KlinikDBEntities();
            do
            {
                data = worksheet.Cells[startRow, startColumn].Value;
                object column1 = worksheet.Cells[startRow + 1, startColumn].Value;
                object column2 = worksheet.Cells[startRow + 1, startColumn + 1].Value;
                object column3 = worksheet.Cells[startRow + 1, startColumn + 2].Value;
                //object createddate = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                if (data != null && column2 != null)
                {
                    var isSuccess = saveClassPo(column1.ToString(), column2.ToString(), column3.ToString(),/*Convert.ToDateTime(createddate),*/ db);
                    if (isSuccess)
                    {
                        count++;
                    }
                }
                startRow++;
            }
            while (data != null);

            return RedirectToAction("PoliList", "MasterData");
        }
        public bool saveClassPo(String column1, String column2, String column3,/*DateTime createddate, */KlinikDBEntities db)
        {
            var result = false;
            //createddate = DateTime.Now;

            if (db.Polis.Where(t => t.Name.Equals(column2)).Count() == 0)
            {
                var item = new Poli();
                item.Code = Convert.ToString(column1);
                item.Name = Convert.ToString(column2);
                item.Type = Convert.ToInt16(column3);
                item.RowStatus = 0;
                item.CreatedBy = Convert.ToString(Session["Code"]);
                item.CreatedDate = DateTime.Now;
                db.Polis.Add(item);
                db.SaveChanges();

            }


            return result;
        }

        #endregion
        #region ::Vendor::
        [HttpPost]
        public FileResult ExportVendor()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Sheet1");
            dt.Columns.AddRange(new DataColumn[1] {
                                            new DataColumn("namavendor")
            });

            var vendors = from v in entities.Vendors.Take(11)
                          select v;

            foreach (var vendor in vendors)
            {
                dt.Rows.Add(vendor.namavendor);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Vendor.xlsx");
                }
            }
        }
        
        public ActionResult ImportVendor()
        {
           
            int count = 0;
            var result = false;
            String path = Server.MapPath("/") + "\\DataImport\\Vendor.xlsx";
            var package = new ExcelPackage(new System.IO.FileInfo(path));
            int startColumn = 1;
            int startRow = 1;
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            object data = null;
            KlinikDBEntities db = new KlinikDBEntities();
            do
            {
                data = worksheet.Cells[startRow, startColumn].Value;
                object column1 = worksheet.Cells[startRow + 1, startColumn].Value;
                //object createddate = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                if (data != null && column1 != null)
                {
                    var isSuccess = saveClassV(column1.ToString(), /*Convert.ToDateTime(createddate),*/ db);
                    if (isSuccess)
                    {
                        count++;
                    }
                }
                startRow++;
            }
            while (data != null);

            return RedirectToAction("VendorList", "MasterData");
        }
        public bool saveClassV(String column1, /*DateTime createddate, */KlinikDBEntities db)
        {
            var result = false;
            //createddate = DateTime.Now;

            if (db.Vendors.Where(t => t.namavendor.Equals(column1)).Count() == 0)
            {
                var item = new Vendor();
                item.namavendor = column1;
                item.RowStatus = 0;
                item.CreatedBy = Convert.ToString(Session["Code"]);
                item.CreatedDate = DateTime.Now;
                db.Vendors.Add(item);
                db.SaveChanges();

            }


            return result;
        }

        #endregion
        #region ::Patient::
        [HttpPost]
        public FileResult ExportPatient()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Sheet1");
            dt.Columns.AddRange(new DataColumn[15] {
                                            new DataColumn("EmployeeID"),
                                            new DataColumn("FamilyRelationshipID"),
                                            new DataColumn("MRNumber"),
                                            new DataColumn("Name"),
                                            new DataColumn("Gender"),
                                            new DataColumn("MaritalStatus"),
                                            new DataColumn("BirthDate"),
                                            new DataColumn("KTPNumber"),
                                            new DataColumn("Address"),
                                            new DataColumn("CityID"),
                                            new DataColumn("HPNumber"),
                                            new DataColumn("Type"),
                                            new DataColumn("BloodType"),
                                            new DataColumn("BPJSNumber"),
                                            new DataColumn("PatientKey")
            });

            var patients = from pasien in entities.Patients.Take(11)
                           select pasien;

            foreach (var patient in patients)
            {
                dt.Rows.Add(patient.EmployeeID, patient.FamilyRelationshipID, patient.MRNumber, patient.Name, patient.Gender, patient.MaritalStatus, patient.BirthDate, patient.KTPNumber, patient.Address, patient.CityID, patient.HPNumber, patient.Type, patient.BloodType, patient.BPJSNumber, patient.PatientKey);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Patient.xlsx");
                }
            }
        }
        [HttpPost]
        public ActionResult ImportPatient()
        {

            int count = 0;
            var result = false;
            String path = Server.MapPath("/") + "\\DataImport\\Patient.xlsx";
            var package = new ExcelPackage(new System.IO.FileInfo(path));
            int startColumn = 1;
            int startRow = 1;
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            object data = null;
            KlinikDBEntities db = new KlinikDBEntities();
            do
            {
                data = worksheet.Cells[startRow, startColumn].Value;
                object column1 = worksheet.Cells[startRow + 1, startColumn].Value;
                object column2 = worksheet.Cells[startRow + 1, startColumn + 1].Value;
                object column3 = worksheet.Cells[startRow + 1, startColumn + 2].Value;
                object column4 = worksheet.Cells[startRow + 1, startColumn + 3].Value;
                object column5 = worksheet.Cells[startRow + 1, startColumn + 4].Value;
                object column6 = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                object column7 = worksheet.Cells[startRow + 1, startColumn + 6].Value;
                object column8 = worksheet.Cells[startRow + 1, startColumn + 7].Value;
                object column9 = worksheet.Cells[startRow + 1, startColumn + 8].Value;
                object column10 = worksheet.Cells[startRow + 1, startColumn + 9].Value;
                object column11 = worksheet.Cells[startRow + 1, startColumn + 10].Value;
                object column12 = worksheet.Cells[startRow + 1, startColumn + 11].Value;
                object column13 = worksheet.Cells[startRow + 1, startColumn + 12].Value;
                object column14 = worksheet.Cells[startRow + 1, startColumn + 13].Value;
                object column15 = worksheet.Cells[startRow + 1, startColumn + 14].Value;


                //object createddate = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                if (data != null && column4 != null)
                {
                    var isSuccess = saveClassP(column1.ToString(), column2.ToString(), column3.ToString(), column4.ToString(), column5.ToString(), column6.ToString(), column7.ToString(), column8.ToString(), column9.ToString(), column10.ToString(), column11.ToString(), column12.ToString(), column13.ToString(), column14.ToString(), column15.ToString(), /*Convert.ToDateTime(createddate),*/ db);
                    if (isSuccess)
                    {
                        count++;
                    }
                }
                startRow++;
            }
            while (data != null);

            return RedirectToAction("PasienList", "Patient");
        }
        public bool saveClassP(String column1, String column2, String column3, String column4, String column5, String column6, String column7, String column8, String column9, String column10, String column11, String column12, String column13, String column14, String column15, /*DateTime createddate, */KlinikDBEntities db)
        {
            var result = false;
            //createddate = DateTime.Now;

            if (db.Patients.Where(t => t.Name.Equals(column4)).Count() == 0)
            {
                var item = new Patient();
                item.EmployeeID = Convert.ToInt64(column1);
                item.FamilyRelationshipID=Convert.ToInt16(column2);
                item.MRNumber=Convert.ToString(column3);
                item.Name = Convert.ToString(column4);
                item.Gender = Convert.ToString(column5);
                item.MaritalStatus = Convert.ToString(column6);
                item.BirthDate = Convert.ToDateTime(column7);
                item.KTPNumber = Convert.ToString(column8);
                item.Address = Convert.ToString(column9);
                item.CityID = Convert.ToInt32(column10);
                item.HPNumber = Convert.ToString(column11);
                item.Type = Convert.ToInt16(column12);
                item.BloodType = Convert.ToString(column13);
                item.BPJSNumber = Convert.ToString(column14);
                item.PatientKey = Convert.ToString(column15);
                item.RowStatus = 0;
                item.CreatedBy = Convert.ToString(Session["Code"]);
                item.CreatedDate = DateTime.Now;
                db.Patients.Add(item);
                db.SaveChanges();

            }


            return result;
        }
        #endregion
        #region ::LabItem::
        [HttpPost]
        public FileResult ExportLab()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Sheet1");
            dt.Columns.AddRange(new DataColumn[5] {
                                            new DataColumn("Code"),
                                            new DataColumn("Name"),
                                            new DataColumn("LabItemCategoryID"),
                                            new DataColumn("Normal"),
                                            new DataColumn("Price")
            });

            var labitems = from labitem in entities.LabItems.Take(11)
                           select labitem;

            foreach (var k in labitems)
            {
                dt.Rows.Add(k.Code, k.Name, k.LabItemCategoryID, k.Normal, k.Price);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LabItem.xlsx");
                }
            }
        }
        public ActionResult ImportLab()
        {

            int count = 0;
            var result = false;
            String path = Server.MapPath("/") + "\\DataImport\\LabItem.xlsx";
            var package = new ExcelPackage(new System.IO.FileInfo(path));
            int startColumn = 1;
            int startRow = 1;
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            object data = null;
            KlinikDBEntities db = new KlinikDBEntities();
            do
            {
                data = worksheet.Cells[startRow, startColumn].Value;
                object column1 = worksheet.Cells[startRow + 1, startColumn].Value;
                object column2 = worksheet.Cells[startRow + 1, startColumn + 1].Value;
                object column3 = worksheet.Cells[startRow + 1, startColumn + 2].Value;
                object column4 = worksheet.Cells[startRow + 1, startColumn + 3].Value;
                object column5 = worksheet.Cells[startRow + 1, startColumn + 4].Value;

                //object createddate = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                if (data != null && column2 != null)
                {
                    var isSuccess = saveClassL(column1.ToString(), column2.ToString(), column3.ToString(), column4.ToString(), column5.ToString(), /*Convert.ToDateTime(createddate),*/ db);
                    if (isSuccess)
                    {
                        count++;
                    }
                }
                startRow++;
            }
            while (data != null);

            return RedirectToAction("LabItemList", "MasterData");
        }
        public bool saveClassL(String column1, String column2, String column3, String column4, String column5, /*DateTime createddate, */KlinikDBEntities db)
        {
            var result = false;
            //createddate = DateTime.Now;

            if (db.LabItems.Where(t => t.Name.Equals(column2)).Count() == 0)
            {
                var item = new LabItem();
                item.Code = Convert.ToString(column1);
                item.Name = Convert.ToString(column2);
                item.LabItemCategoryID = Convert.ToInt32(column3);
                item.Normal = Convert.ToString(column4);
                item.Price = Convert.ToDecimal(column5);
                item.RowStatus = 0;
                item.CreatedBy = Convert.ToString(Session["Code"]);
                item.CreatedDate = DateTime.Now;
                db.LabItems.Add(item);
                db.SaveChanges();

            }


            return result;
        }


        #endregion
        #region ::LabItemCategory::
        [HttpPost]
        public FileResult ExportLabCategory()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Sheet1");
            dt.Columns.AddRange(new DataColumn[3] {
                                            new DataColumn("LabType"),
                                            new DataColumn("PoliID"),
                                            new DataColumn("Name")
            });

            var labitemcs = from labitemc in entities.LabItemCategories.Take(11)
                            select labitemc;

            foreach (var k in labitemcs)
            {
                dt.Rows.Add(k.LabType, k.Name, k.PoliID);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LabItemCategory.xlsx");
                }
            }
        }
        public ActionResult ImportLabCategory()
        {

            int count = 0;
            var result = false;
            String path = Server.MapPath("/") + "\\DataImport\\LabItemCategory.xlsx";
            var package = new ExcelPackage(new System.IO.FileInfo(path));
            int startColumn = 1;
            int startRow = 1;
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            object data = null;
            KlinikDBEntities db = new KlinikDBEntities();
            do
            {
                data = worksheet.Cells[startRow, startColumn].Value;
                object column1 = worksheet.Cells[startRow + 1, startColumn].Value;
                object column2 = worksheet.Cells[startRow + 1, startColumn + 1].Value;
                object column3 = worksheet.Cells[startRow + 1, startColumn + 2].Value;

                //object createddate = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                if (data != null && column3 != null)
                {
                    var isSuccess = saveClassLC(column1.ToString(), column2.ToString(), column3.ToString(), /*Convert.ToDateTime(createddate),*/ db);
                    if (isSuccess)
                    {
                        count++;
                    }
                }
                startRow++;
            }
            while (data != null);

            return RedirectToAction("LabItemCategoryList", "MasterData");
        }
        public bool saveClassLC(String column1, String column2, String column3, /*DateTime createddate, */KlinikDBEntities db)
        {
            var result = false;
            //createddate = DateTime.Now;

            if (db.LabItemCategories.Where(t => t.Name.Equals(column3)).Count() == 0)
            {
                var item = new LabItemCategory();
                item.LabType = Convert.ToString(column1);
                item.PoliID = Convert.ToInt32(column2);
                item.Name = Convert.ToString(column3);
                item.RowStatus = 0;
                item.CreatedBy = Convert.ToString(Session["Code"]);
                item.CreatedDate = DateTime.Now;
                db.LabItemCategories.Add(item);
                db.SaveChanges();

            }


            return result;
        }


        #endregion
        #region ::Product::
        [HttpPost]
        public FileResult ExportProduct()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Sheet1");
            dt.Columns.AddRange(new DataColumn[6] {
                                            new DataColumn("Code"),
                                            new DataColumn("Name"),
                                            new DataColumn("ClinicID"),
                                            new DataColumn("ProductCategoryID"),
                                            new DataColumn("ProductUnitID"),
                                            new DataColumn("RetailPrice")
            });

            var products = from product in entities.Products.Take(11)
                           select product;

            foreach (var k in products)
            {
                dt.Rows.Add(k.Code, k.Name, k.ProductCategoryID, k.ProductUnitID, k.ClinicID, k.RetailPrice);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Product.xlsx");
                }
            }
        }
        public ActionResult ImportProduct()
        {

            int count = 0;
            var result = false;
            String path = Server.MapPath("/") + "\\DataImport\\Product.xlsx";
            var package = new ExcelPackage(new System.IO.FileInfo(path));
            int startColumn = 1;
            int startRow = 1;
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            object data = null;
            KlinikDBEntities db = new KlinikDBEntities();
            do
            {
                data = worksheet.Cells[startRow, startColumn].Value;
                object column1 = worksheet.Cells[startRow + 1, startColumn].Value;
                object column2 = worksheet.Cells[startRow + 1, startColumn + 1].Value;
                object column3 = worksheet.Cells[startRow + 1, startColumn + 2].Value;
                object column4 = worksheet.Cells[startRow + 1, startColumn + 3].Value;
                object column5 = worksheet.Cells[startRow + 1, startColumn + 4].Value;
                object column6 = worksheet.Cells[startRow + 1, startColumn + 5].Value;

                //object createddate = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                if (data != null && column2 != null)
                {
                    var isSuccess = saveClassP(column1.ToString(), column2.ToString(), column3.ToString(), column4.ToString(), column5.ToString(), column6.ToString(), /*Convert.ToDateTime(createddate),*/ db);
                    if (isSuccess)
                    {
                        count++;
                    }
                }
                startRow++;
            }
            while (data != null);

            return RedirectToAction("ProductList", "MasterData");
        }
        public bool saveClassP(String column1, String column2, String column3, String column4, String column5, String column6, /*DateTime createddate, */KlinikDBEntities db)
        {
            var result = false;
            //createddate = DateTime.Now;

            if (db.Products.Where(t => t.Name.Equals(column2)).Count() == 0)
            {
                var item = new Product();
                item.Code = Convert.ToString(column1);
                item.Name = Convert.ToString(column2);
                item.ClinicID = Convert.ToInt64(column3);
                item.ProductCategoryID = Convert.ToInt32(column4);
                item.ProductUnitID = Convert.ToInt32(column5);
                item.RetailPrice = Convert.ToInt16(column6);
                item.RowStatus = 0;
                item.CreatedBy = Convert.ToString(Session["Code"]);
                item.CreatedDate = DateTime.Now;
                db.Products.Add(item);
                db.SaveChanges();

            }


            return result;
        }


        #endregion
        #region ::ProductCategory::
        [HttpPost]
        public FileResult ExportProductCategory()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("ProductCategory");
            dt.Columns.AddRange(new DataColumn[1] {

                                            new DataColumn("Name")
            });

            var productcs = from productc in entities.ProductCategories.Take(11)
                            select productc;

            foreach (var k in productcs)
            {
                dt.Rows.Add(k.Name);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ProductCategory.xlsx");
                }
            }
        }
        public ActionResult ImportProductCategory()
        {

            int count = 0;
            var result = false;
            String path = Server.MapPath("/") + "\\DataImport\\ProductCategory.xlsx";
            var package = new ExcelPackage(new System.IO.FileInfo(path));
            int startColumn = 1;
            int startRow = 1;
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            object data = null;
            KlinikDBEntities db = new KlinikDBEntities();
            do
            {
                data = worksheet.Cells[startRow, startColumn].Value;
                object column1 = worksheet.Cells[startRow + 1, startColumn].Value;

                //object createddate = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                if (data != null && column1 != null)
                {
                    var isSuccess = saveClassPC(column1.ToString(), /*Convert.ToDateTime(createddate),*/ db);
                    if (isSuccess)
                    {
                        count++;
                    }
                }
                startRow++;
            }
            while (data != null);

            return RedirectToAction("ProductCategoryList", "MasterData");
        }
        public bool saveClassPC(String column1, /*DateTime createddate, */KlinikDBEntities db)
        {
            var result = false;
            //createddate = DateTime.Now;

            if (db.ProductCategories.Where(t => t.Name.Equals(column1)).Count() == 0)
            {
                var item = new ProductCategory();
                item.Name = Convert.ToString(column1);
                item.RowStatus = 0;
                item.CreatedBy = Convert.ToString(Session["Code"]);
                item.CreatedDate = DateTime.Now;
                db.ProductCategories.Add(item);
                db.SaveChanges();

            }


            return result;
        }


        #endregion
        #region ::ProductUnit::
        [HttpPost]
        public FileResult ExportProductUnit()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("ProductUnit");
            dt.Columns.AddRange(new DataColumn[2] {
                                            new DataColumn("Code"),
                                            new DataColumn("Name")
            });

            var productunits = from productunit in entities.ProductUnits.Take(11)
                               select productunit;

            foreach (var k in productunits)
            {
                dt.Rows.Add(k.Code, k.Name);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ProductUnit.xlsx");
                }
            }
        }
        public ActionResult ImportProductUnit()
        {

            int count = 0;
            var result = false;
            String path = Server.MapPath("/") + "\\DataImport\\ProductUnit.xlsx";
            var package = new ExcelPackage(new System.IO.FileInfo(path));
            int startColumn = 1;
            int startRow = 1;
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            object data = null;
            KlinikDBEntities db = new KlinikDBEntities();
            do
            {
                data = worksheet.Cells[startRow, startColumn].Value;
                object column1 = worksheet.Cells[startRow + 1, startColumn].Value;
                object column2 = worksheet.Cells[startRow + 1, startColumn + 1].Value;

                //object createddate = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                if (data != null && column2 != null)
                {
                    var isSuccess = saveClassPU(column1.ToString(), column2.ToString(), /*Convert.ToDateTime(createddate),*/ db);
                    if (isSuccess)
                    {
                        count++;
                    }
                }
                startRow++;
            }
            while (data != null);

            return RedirectToAction("ProductUnitList", "MasterData");
        }
        public bool saveClassPU(String column1, String column2, /*DateTime createddate, */KlinikDBEntities db)
        {
            var result = false;
            //createddate = DateTime.Now;

            if (db.ProductUnits.Where(t => t.Name.Equals(column2)).Count() == 0)
            {
                var item = new ProductUnit();
                item.Code = Convert.ToString(column1);
                item.Name = Convert.ToString(column2);
                item.RowStatus = 0;
                item.CreatedBy = Convert.ToString(Session["Code"]);
                item.CreatedDate = DateTime.Now;
                db.ProductUnits.Add(item);
                db.SaveChanges();

            }


            return result;
        }


        #endregion
        #region ::ProductMedicine::
        [HttpPost]
        public FileResult ExportProductMedicine()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("ProductMedicine");
            dt.Columns.AddRange(new DataColumn[3] {
                                            new DataColumn("ProductID"),
                                            new DataColumn("MedicineID"),
                                            new DataColumn("Amount")
            });

            var productms = from productm in entities.ProductMedicines.Take(11)
                            select productm;

            foreach (var k in productms)
            {
                dt.Rows.Add(k.ProductID, k.MedicineID, k.Amount);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ProductMedicine.xlsx");
                }
            }
        }
        public ActionResult ImportProductMedicine()
        {

            int count = 0;
            var result = false;
            String path = Server.MapPath("/") + "\\DataImport\\ProductMedicine.xlsx";
            var package = new ExcelPackage(new System.IO.FileInfo(path));
            int startColumn = 1;
            int startRow = 1;
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            object data = null;
            KlinikDBEntities db = new KlinikDBEntities();
            do
            {
                data = worksheet.Cells[startRow, startColumn].Value;
                object column1 = worksheet.Cells[startRow + 1, startColumn].Value;
                object column2 = worksheet.Cells[startRow + 1, startColumn + 1].Value;
                object column3 = worksheet.Cells[startRow + 1, startColumn + 2].Value;

                //object createddate = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                if (data != null && column2 != null)
                {
                    var isSuccess = saveClassPM(column1.ToString(), column2.ToString(), column3.ToString(), /*Convert.ToDateTime(createddate),*/ db);
                    if (isSuccess)
                    {
                        count++;
                    }
                }
                startRow++;
            }
            while (data != null);

            return RedirectToAction("ProductMedicineList", "MasterData");
        }
        public bool saveClassPM(String column1, String column2, String column3, /*DateTime createddate, */KlinikDBEntities db)
        {
            var result = false;
            //createddate = DateTime.Now;

            if (db.ProductMedicines.Where(t => t.MedicineID.Equals(column2)).Count() == 0)
            {
                var item = new ProductMedicine();
                item.ProductID = Convert.ToInt32(column1);
                item.MedicineID = Convert.ToInt32(column2);
                item.Amount = Convert.ToDouble(column3);
                item.RowStatus = 0;
                item.CreatedBy = Convert.ToString(Session["Code"]);
                item.CreatedDate = DateTime.Now;
                db.ProductMedicines.Add(item);
                db.SaveChanges();

            }


            return result;
        }


        #endregion
        #region ::Medicine::
        [HttpPost]
        public FileResult ExportMedicine()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Sheet1");
            dt.Columns.AddRange(new DataColumn[1] {
                                            new DataColumn("Name")
            });

            var medicines = from medicine in entities.Medicines.Take(11)
                            select medicine;

            foreach (var k in medicines)
            {
                dt.Rows.Add(k.Name);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Medicine.xlsx");
                }
            }
        }
        public ActionResult ImportMedicine()
        {

            int count = 0;
            var result = false;
            String path = Server.MapPath("/") + "\\DataImport\\Medicine.xlsx";
            var package = new ExcelPackage(new System.IO.FileInfo(path));
            int startColumn = 1;
            int startRow = 1;
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            object data = null;
            KlinikDBEntities db = new KlinikDBEntities();
            do
            {
                data = worksheet.Cells[startRow, startColumn].Value;
                object column1 = worksheet.Cells[startRow + 1, startColumn].Value;

                //object createddate = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                if (data != null && column1 != null)
                {
                    var isSuccess = saveClassM(column1.ToString(), /*Convert.ToDateTime(createddate),*/ db);
                    if (isSuccess)
                    {
                        count++;
                    }
                }
                startRow++;
            }
            while (data != null);

            return RedirectToAction("MedicineeList", "MasterData");
        }
        public bool saveClassM(String column1, /*DateTime createddate, */KlinikDBEntities db)
        {
            var result = false;
            //createddate = DateTime.Now;

            if (db.Medicines.Where(t => t.Name.Equals(column1)).Count() == 0)
            {
                var item = new Medicine();
                item.Name = Convert.ToString(column1);
                item.RowStatus = 0;
                item.CreatedBy = Convert.ToString(Session["Code"]);
                item.CreatedDate = DateTime.Now;
                db.Medicines.Add(item);
                db.SaveChanges();

            }


            return result;
        }



        #endregion
        #region ::Clinic::
        [HttpPost]
        public FileResult ExportClinic()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Sheet1");
            dt.Columns.AddRange(new DataColumn[12] {
                                            new DataColumn("Code"),
                                            new DataColumn("Name"),
                                            new DataColumn("Address"),
                                            new DataColumn("LegalNumber"),
                                            new DataColumn("LegalDate"),
                                            new DataColumn("ContactNumber"),
                                            new DataColumn("Email"),
                                            new DataColumn("Lat"),
                                            new DataColumn("Long"),
                                            new DataColumn("CityID"),
                                            new DataColumn("ClinicType"),
                                            new DataColumn("ReffID")
            });

            var clinics = from klinik in entities.Clinics.Take(11)
                          select klinik;

            foreach (var k in clinics)
            {
                dt.Rows.Add(k.Code, k.Name, k.Address, k.LegalNumber, k.LegalDate, k.ContactNumber, k.Email, k.Lat, k.Long, k.CityID, k.ClinicType, k.ReffID);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Clinic.xlsx");
                }
            }
        }
        public ActionResult ImportClinic()
        {

            int count = 0;
            var result = false;
            String path = Server.MapPath("/") + "\\DataImport\\Clinic.xlsx";
            var package = new ExcelPackage(new System.IO.FileInfo(path));
            int startColumn = 1;
            int startRow = 1;
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            object data = null;
            KlinikDBEntities db = new KlinikDBEntities();
            do
            {
                data = worksheet.Cells[startRow, startColumn].Value;
                object column1 = worksheet.Cells[startRow + 1, startColumn].Value;
                object column2 = worksheet.Cells[startRow + 1, startColumn + 1].Value;
                object column3 = worksheet.Cells[startRow + 1, startColumn + 2].Value;
                object column4 = worksheet.Cells[startRow + 1, startColumn + 3].Value;
                object column5 = worksheet.Cells[startRow + 1, startColumn + 4].Value;
                object column6 = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                object column7 = worksheet.Cells[startRow + 1, startColumn + 6].Value;
                object column8 = worksheet.Cells[startRow + 1, startColumn + 7].Value;
                object column9 = worksheet.Cells[startRow + 1, startColumn + 8].Value;
                object column10 = worksheet.Cells[startRow + 1, startColumn + 9].Value;
                object column11 = worksheet.Cells[startRow + 1, startColumn + 10].Value;
                object column12 = worksheet.Cells[startRow + 1, startColumn + 11].Value;
                //object createddate = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                if (data != null && column2 != null)
                {
                    var isSuccess = saveClassC(column1.ToString(), column2.ToString(), column3.ToString(), column4.ToString(), column5.ToString(), column6.ToString(), column7.ToString(), column8.ToString(), column9.ToString(), column10.ToString(), column11.ToString(), column12.ToString(),/*Convert.ToDateTime(createddate),*/ db);
                    if (isSuccess)
                    {
                        count++;
                    }
                }
                startRow++;
            }
            while (data != null);

            return RedirectToAction("ClinicList", "MasterData");
        }
        public bool saveClassC(String column1, String column2, String column3, String column4, String column5, String column6, String column7, String column8, String column9, String column10, String column11, String column12,/*DateTime createddate, */KlinikDBEntities db)
        {
            var result = false;
            //createddate = DateTime.Now;

            if (db.Clinics.Where(t => t.Name.Equals(column2)).Count() == 0)
            {
                var item = new Clinic();
                item.Code = Convert.ToString(column1);
                item.Name = Convert.ToString(column2);
                item.Address = Convert.ToString(column3);
                item.LegalNumber = Convert.ToString(column4);
                item.LegalDate = Convert.ToDateTime(column5);
                item.ContactNumber = Convert.ToString(column6);
                item.Email = Convert.ToString(column7);
                item.Lat = Convert.ToDouble(column8);
                item.Long = Convert.ToDouble(column9);
                item.CityID = Convert.ToInt32(column10);
                item.ClinicType = Convert.ToInt16(column11);
                item.ReffID = Convert.ToInt32(column12);
                item.RowStatus = 0;
                item.CreatedBy = Convert.ToString(Session["Code"]);
                item.CreatedDate = DateTime.Now;
                db.Clinics.Add(item);
                db.SaveChanges();

            }


            return result;
        }


        #endregion
        #region ::Doctor::
        [HttpPost]
        public FileResult ExportDoctor()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Doctor");
            dt.Columns.AddRange(new DataColumn[13] {
                                            new DataColumn("Code"),
                                            new DataColumn("Name"),
                                            new DataColumn("Address"),
                                            new DataColumn("SpecialistID"),
                                            new DataColumn("TypeID"),
                                            new DataColumn("KTPNumber"),
                                            new DataColumn("STRNumber"),
                                            new DataColumn("STRValidFrom"),
                                            new DataColumn("STRValidTo"),
                                            new DataColumn("HPNumber"),
                                            new DataColumn("Remark"),
                                            new DataColumn("Email"),
                                            new DataColumn("EmployeeID")
            });

            var doctors = from doctor in entities.Doctors.Take(11)
                          select doctor;

            foreach (var k in doctors)
            {
                dt.Rows.Add(k.Code, k.Name, k.Address, k.SpecialistID, k.TypeID, k.KTPNumber, k.STRNumber, k.STRValidFrom, k.STRValidTo, k.HPNumber, k.Remark, k.Email, k.EmployeeID);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Doctor.xlsx");
                }
            }
        }
        public ActionResult ImportDoctor()
        {

            int count = 0;
            var result = false;
            String path = Server.MapPath("/") + "\\DataImport\\Doctor.xlsx";
            var package = new ExcelPackage(new System.IO.FileInfo(path));
            int startColumn = 1;
            int startRow = 1;
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            object data = null;
            KlinikDBEntities db = new KlinikDBEntities();
            do
            {
                data = worksheet.Cells[startRow, startColumn].Value;
                object column1 = worksheet.Cells[startRow + 1, startColumn].Value;
                object column2 = worksheet.Cells[startRow + 1, startColumn + 1].Value;
                object column3 = worksheet.Cells[startRow + 1, startColumn + 2].Value;
                object column4 = worksheet.Cells[startRow + 1, startColumn + 3].Value;
                object column5 = worksheet.Cells[startRow + 1, startColumn + 4].Value;
                object column6 = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                object column7 = worksheet.Cells[startRow + 1, startColumn + 6].Value;
                object column8 = worksheet.Cells[startRow + 1, startColumn + 7].Value;
                object column9 = worksheet.Cells[startRow + 1, startColumn + 8].Value;
                object column10 = worksheet.Cells[startRow + 1, startColumn + 9].Value;
                object column11 = worksheet.Cells[startRow + 1, startColumn + 10].Value;
                object column12 = worksheet.Cells[startRow + 1, startColumn + 11].Value;
                object column13 = worksheet.Cells[startRow + 1, startColumn + 12].Value;
                //object createddate = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                if (data != null && column2 != null)
                {
                    var isSuccess = saveClassD(column1.ToString(), column2.ToString(), column3.ToString(), column4.ToString(), column5.ToString(), column6.ToString(), column7.ToString(), column8.ToString(), column9.ToString(), column10.ToString(), column11.ToString(), column12.ToString(), column13.ToString(),/*Convert.ToDateTime(createddate),*/ db);
                    if (isSuccess)
                    {
                        count++;
                    }
                }
                startRow++;
            }
            while (data != null);

            return RedirectToAction("Index", "Doctor");
        }
        public bool saveClassD(String column1, String column2, String column3, String column4, String column5, String column6, String column7, String column8, String column9, String column10, String column11, String column12, String column13,/*DateTime createddate, */KlinikDBEntities db)
        {
            var result = false;
            //createddate = DateTime.Now;

            if (db.Doctors.Where(t => t.Name.Equals(column2)).Count() == 0)
            {
                var item = new Doctor();
                item.Code = Convert.ToString(column1);
                item.Name = Convert.ToString(column2);
                item.Address = Convert.ToString(column3);
                item.SpecialistID = Convert.ToInt32(column4);
                item.TypeID = Convert.ToInt32(column5);
                item.KTPNumber = Convert.ToString(column6);
                item.STRNumber = Convert.ToString(column7);
                item.STRValidFrom = Convert.ToDateTime(column8);
                item.STRValidTo = Convert.ToDateTime(column9);
                item.HPNumber = Convert.ToString(column10);
                item.Remark = Convert.ToString(column11);
                item.Email = Convert.ToString(column12);
                item.EmployeeID = Convert.ToInt64(column13);
                item.RowStatus = 0;
                item.CreatedBy = Convert.ToString(Session["Code"]);
                item.CreatedDate = DateTime.Now;
                db.Doctors.Add(item);
                db.SaveChanges();

            }


            return result;
        }

        #endregion
        #region ::Employee::
        [HttpPost]
        public FileResult ExportEmployee()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Employee");
            dt.Columns.AddRange(new DataColumn[10] {
                                            new DataColumn("EmpID"),
                                            new DataColumn("EmpName"),
                                            new DataColumn("BirthDate"),
                                            new DataColumn("ReffEmpID"),
                                            new DataColumn("Gender"),
                                            new DataColumn("EmpType"),
                                            new DataColumn("KTPNumber"),
                                            new DataColumn("LastEmpID"),
                                            new DataColumn("HPNumber"),
                                            new DataColumn("Email")
            });

            var emps = from emp in entities.Employees.Take(11)
                       select emp;

            foreach (var k in emps)
            {
                dt.Rows.Add(k.EmpID, k.EmpName, k.BirthDate, k.ReffEmpID, k.Gender, k.EmpType, k.KTPNumber, k.LastEmpID, k.HPNumber, k.Email);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employee.xlsx");
                }
            }
        }
        public ActionResult ImportEmployee()
        {

            int count = 0;
            var result = false;
            String path = Server.MapPath("/") + "\\DataImport\\Employee.xlsx";
            var package = new ExcelPackage(new System.IO.FileInfo(path));
            int startColumn = 1;
            int startRow = 1;
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            object data = null;
            KlinikDBEntities db = new KlinikDBEntities();
            do
            {
                data = worksheet.Cells[startRow, startColumn].Value;
                object column1 = worksheet.Cells[startRow + 1, startColumn].Value;
                object column2 = worksheet.Cells[startRow + 1, startColumn + 1].Value;
                object column3 = worksheet.Cells[startRow + 1, startColumn + 2].Value;
                object column4 = worksheet.Cells[startRow + 1, startColumn + 3].Value;
                object column5 = worksheet.Cells[startRow + 1, startColumn + 4].Value;
                object column6 = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                object column7 = worksheet.Cells[startRow + 1, startColumn + 6].Value;
                object column8 = worksheet.Cells[startRow + 1, startColumn + 7].Value;
                object column9 = worksheet.Cells[startRow + 1, startColumn + 8].Value;
                object column10 = worksheet.Cells[startRow + 1, startColumn + 9].Value;

                //object createddate = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                if (data != null && column2 != null)
                {
                    var isSuccess = saveClassE(column1.ToString(), column2.ToString(), column3.ToString(), column4.ToString(), column5.ToString(), column6.ToString(), column7.ToString(), column8.ToString(), column9.ToString(), column10.ToString(), /*Convert.ToDateTime(createddate),*/ db);
                    if (isSuccess)
                    {
                        count++;
                    }
                }
                startRow++;
            }
            while (data != null);

            return RedirectToAction("EmployeeList", "MasterData");
        }
        public bool saveClassE(String column1, String column2, String column3, String column4, String column5, String column6, String column7, String column8, String column9, String column10, /*DateTime createddate, */KlinikDBEntities db)
        {
            var result = false;
            //createddate = DateTime.Now;

            if (db.Employees.Where(t => t.EmpName.Equals(column2)).Count() == 0)
            {
                var item = new Employee();
                item.EmpID = Convert.ToString(column1);
                item.EmpName = Convert.ToString(column2);
                item.BirthDate = Convert.ToDateTime(column3);
                item.ReffEmpID = Convert.ToString(column4);
                item.Gender = Convert.ToString(column5);
                item.EmpType = Convert.ToInt16(column6);
                item.KTPNumber = Convert.ToString(column7);
                item.LastEmpID = Convert.ToString(column8);
                item.HPNumber = Convert.ToString(column9);
                item.Email = Convert.ToString(column10);
                item.RowStatus = 0;
                item.CreatedBy = Convert.ToString(Session["Code"]);
                item.CreatedDate = DateTime.Now;
                db.Employees.Add(item);
                db.SaveChanges();

            }


            return result;
        }

        #endregion
        #region ::PoliSchedule::
        [HttpPost]
        public FileResult ExportPoliSchedule()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("PoliSchedule");
            dt.Columns.AddRange(new DataColumn[8] {
                                            new DataColumn("ClinicID"),
                                            new DataColumn("DoctorID"),
                                            new DataColumn("PoliID"),
                                            new DataColumn("StartDate"),
                                            new DataColumn("EndDate"),
                                            new DataColumn("ReffID"),
                                            new DataColumn("Remark"),
                                            new DataColumn("Status")
            });

            var poliscs = from polisc in entities.PoliSchedules.Take(11)
                          select polisc;

            foreach (var k in poliscs)
            {
                dt.Rows.Add(k.ClinicID, k.DoctorID, k.PoliID, k.StartDate, k.EndDate, k.ReffID, k.Remark, k.Status);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PoliSchedule.xlsx");
                }
            }
        }
        [HttpPost]
        public ActionResult ImportPoliSchedule()
        {

            int count = 0;
            var result = false;
            String path = Server.MapPath("/") + "\\DataImport\\PoliSchedule.xlsx";
            var package = new ExcelPackage(new System.IO.FileInfo(path));
            int startColumn = 1;
            int startRow = 1;
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            object data = null;
            KlinikDBEntities db = new KlinikDBEntities();
            do
            {
                data = worksheet.Cells[startRow, startColumn].Value;
                object column1 = worksheet.Cells[startRow + 1, startColumn].Value;
                object column2 = worksheet.Cells[startRow + 1, startColumn + 1].Value;
                object column3 = worksheet.Cells[startRow + 1, startColumn + 2].Value;
                object column4 = worksheet.Cells[startRow + 1, startColumn + 3].Value;
                object column5 = worksheet.Cells[startRow + 1, startColumn + 4].Value;
                object column6 = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                object column7 = worksheet.Cells[startRow + 1, startColumn + 6].Value;
                object column8 = worksheet.Cells[startRow + 1, startColumn + 7].Value;
                //object createddate = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                if (data != null && column2 != null)
                {
                    var isSuccess = saveClassPS(column1.ToString(), column2.ToString(), column3.ToString(), column4.ToString(), column5.ToString(), column6.ToString(), column7.ToString(), column8.ToString(),/*Convert.ToDateTime(createddate),*/ db);
                    if (isSuccess)
                    {
                        count++;
                    }
                }
                startRow++;
            }
            while (data != null);

            return RedirectToAction("Index", "PoliSchedule");
        }
        public bool saveClassPS(String column1, String column2, String column3, String column4, String column5, String column6, String column7, String column8, /*DateTime createddate, */KlinikDBEntities db)
        {
            var result = false;
            //createddate = DateTime.Now;

            if (db.PoliSchedules.Where(t => t.DoctorID.Equals(column2)).Count() == 0)
            {
                var item = new PoliSchedule();
                item.ClinicID = Convert.ToInt64(column1);
                item.DoctorID = Convert.ToInt32(column2);
                item.PoliID = Convert.ToInt32(column3);
                item.StartDate = Convert.ToDateTime(column4);
                item.EndDate = Convert.ToDateTime(column5);
                item.ReffID = Convert.ToInt64(column6);
                item.Remark = Convert.ToString(column7);
                item.Status = Convert.ToInt32(column8);
                item.RowStatus = 0;
                item.CreatedBy = Convert.ToString(Session["Code"]);
                item.CreatedDate = DateTime.Now;
                db.PoliSchedules.Add(item);
                db.SaveChanges();

            }


            return result;
        }

        #endregion
        #region ::PoliScheduleMaster::
        [HttpPost]
        public FileResult ExportPoliScheduleMaster()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("PoliScheduleMaster");
            dt.Columns.AddRange(new DataColumn[6] {
                                            new DataColumn("ClinicID"),
                                            new DataColumn("DoctorID"),
                                            new DataColumn("PoliID"),
                                            new DataColumn("StartTime"),
                                            new DataColumn("EndTime"),
                                            new DataColumn("Day")
            });

            var poliscms = from poliscm in entities.PoliScheduleMasters.Take(11)
                           select poliscm;

            foreach (var k in poliscms)
            {
                dt.Rows.Add(k.ClinicID, k.DoctorID, k.PoliID, k.StartTime, k.EndTime, k.Day);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PoliScheduleMaster.xlsx");
                }
            }
        }
        [HttpPost]
        public ActionResult ImportPoliScheduleMaster()
        {

            int count = 0;
            var result = false;
            String path = Server.MapPath("/") + "\\DataImport\\PoliScheduleMaster.xlsx";
            var package = new ExcelPackage(new System.IO.FileInfo(path));
            int startColumn = 1;
            int startRow = 1;
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            object data = null;
            KlinikDBEntities db = new KlinikDBEntities();
            do
            {
                data = worksheet.Cells[startRow, startColumn].Value;
                object column1 = worksheet.Cells[startRow + 1, startColumn].Value;
                object column2 = worksheet.Cells[startRow + 1, startColumn + 1].Value;
                object column3 = worksheet.Cells[startRow + 1, startColumn + 2].Value;
                object column4 = worksheet.Cells[startRow + 1, startColumn + 3].Value;
                object column5 = worksheet.Cells[startRow + 1, startColumn + 4].Value;
                object column6 = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                //object createddate = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                if (data != null && column2 != null)
                {
                    var isSuccess = saveClassPSM(column1.ToString(), column2.ToString(), column3.ToString(), column4.ToString(), column5.ToString(), column6.ToString(),/*Convert.ToDateTime(createddate),*/ db);
                    if (isSuccess)
                    {
                        count++;
                    }
                }
                startRow++;
            }
            while (data != null);

            return RedirectToAction("Master", "PoliSchedule");
        }
        public bool saveClassPSM(String column1, String column2, String column3, String column4, String column5, String column6, /*DateTime createddate, */KlinikDBEntities db)
        {
            var result = false;
            //createddate = DateTime.Now;
            if (db.PoliScheduleMasters.Where(t => t.DoctorID.Equals(column2)).Count() == 0)
            {
                var item = new PoliScheduleMaster();
                item.ClinicID = Convert.ToInt64(column1);
                item.DoctorID = Convert.ToInt32(column2);
                item.PoliID = Convert.ToInt32(column3);
                item.StartTime = TimeSpan.Parse(column4);
                item.EndTime = TimeSpan.Parse(column5);
                item.Day = Convert.ToInt32(column6);
                item.RowStatus = 0;
                item.CreatedBy = Convert.ToString(Session["Code"]);
                item.CreatedDate = DateTime.Now;
                db.PoliScheduleMasters.Add(item);
                db.SaveChanges();

            }


            return result;
        }
        #endregion
        #region ::User::
        [HttpPost]
        public FileResult ExportUser()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("User");
            dt.Columns.AddRange(new DataColumn[7] {
                                            new DataColumn("OrganizationID"),
                                            new DataColumn("UserName"),
                                            new DataColumn("Password"),
                                            new DataColumn("EmployeeID"),
                                            new DataColumn("ExpiredDate"),
                                            new DataColumn("ResetPasswordCode"),
                                            new DataColumn("Status")
            });

            var users = from user in entities.Users.Take(11)
                        select user;

            foreach (var k in users)
            {
                dt.Rows.Add(k.OrganizationID, k.UserName, k.Password, k.EmployeeID, k.ResetPasswordCode, k.Status, k.ExpiredDate);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "User.xlsx");
                }
            }
        }
        
        public ActionResult ImportUser()
        {

            int count = 0;
            var result = false;
            String path = Server.MapPath("/") + "\\DataImport\\User.xlsx";
            var package = new ExcelPackage(new System.IO.FileInfo(path));
            int startColumn = 1;
            int startRow = 1;
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            object data = null;
            KlinikDBEntities db = new KlinikDBEntities();
            do
            {
                data = worksheet.Cells[startRow, startColumn].Value;
                object column1 = worksheet.Cells[startRow + 1, startColumn].Value;
                object column2 = worksheet.Cells[startRow + 1, startColumn + 1].Value;
                object column3 = worksheet.Cells[startRow + 1, startColumn + 2].Value;
                object column4 = worksheet.Cells[startRow + 1, startColumn + 3].Value;
                object column5 = worksheet.Cells[startRow + 1, startColumn + 4].Value;
                object column6 = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                object column7 = worksheet.Cells[startRow + 1, startColumn + 6].Value;
                //object createddate = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                if (data != null && column2 != null)
                {
                    var isSuccess = saveClassU(column1.ToString(), column2.ToString(), column3.ToString(), column4.ToString(), column5.ToString(), column6.ToString(), column7.ToString(),/*Convert.ToDateTime(createddate),*/ db);
                    if (isSuccess)
                    {
                        count++;
                    }
                }
                startRow++;
            }
            while (data != null);

            return RedirectToAction("User", "MasterData");
        }
        public bool saveClassU(String column1, String column2, String column3, String column4, String column5, String column6, String column7, KlinikDBEntities db)
        {
            var result = false;
            //createddate = DateTime.Now;

            if (db.Users.Where(t => t.UserName.Equals(column2)).Count() == 0)
            {
                var item = new User();
                item.OrganizationID = Convert.ToInt64(column1);
                item.UserName = Convert.ToString(column2);
                item.Password = Convert.ToString(column3);
                item.EmployeeID = Convert.ToInt64(column4);
                item.ExpiredDate = Convert.ToDateTime(column5);
                item.ResetPasswordCode = Convert.ToString(column6);
                item.Status = Convert.ToBoolean(column7);
                item.RowStatus = 0;
                item.CreatedBy = Convert.ToString(Session["Code"]);
                item.CreatedDate = DateTime.Now;
                db.Users.Add(item);
                db.SaveChanges();

            }


            return result;
        }

        #endregion
        #region ::Organization::
        [HttpPost]
        public FileResult ExportOrganization()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Sheet1");
            dt.Columns.AddRange(new DataColumn[3] {
                                            new DataColumn("OrgCode"),
                                            new DataColumn("OrgName"),
                                            new DataColumn("KlinikID")
            });

            var users = from user in entities.Organizations.Take(11)
                        select user;

            foreach (var k in users)
            {
                dt.Rows.Add(k.OrgCode, k.OrgName, k.KlinikID);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Organization.xlsx");
                }
            }
        }
        public ActionResult ImportOrganization()
        {

            int count = 0;
            var result = false;
            String path = Server.MapPath("/") + "\\DataImport\\Organization.xlsx";
            var package = new ExcelPackage(new System.IO.FileInfo(path));
            int startColumn = 1;
            int startRow = 1;
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            object data = null;
            KlinikDBEntities db = new KlinikDBEntities();
            do
            {
                data = worksheet.Cells[startRow, startColumn].Value;
                object column1 = worksheet.Cells[startRow + 1, startColumn].Value;
                object column2 = worksheet.Cells[startRow + 1, startColumn + 1].Value;
                object column3 = worksheet.Cells[startRow + 1, startColumn + 2].Value;
                //object createddate = worksheet.Cells[startRow + 1, startColumn + 5].Value;
                if (data != null && column2 != null)
                {
                    var isSuccess = saveClassO(column1.ToString(), column2.ToString(), column3.ToString(),/*Convert.ToDateTime(createddate),*/ db);
                    if (isSuccess)
                    {
                        count++;
                    }
                }
                startRow++;
            }
            while (data != null);

            return RedirectToAction("Organization", "MasterData");
        }
        public bool saveClassO(String column1, String column2, String column3,/*DateTime createddate, */KlinikDBEntities db)
        {
            var result = false;
            //createddate = DateTime.Now;

            if (db.Organizations.Where(t => t.OrgName.Equals(column2)).Count() == 0)
            {
                var item = new Organization();
                item.OrgCode = Convert.ToString(column1);
                item.OrgName = Convert.ToString(column2);
                item.KlinikID = Convert.ToInt64(column3);
                item.RowStatus = 0;
                item.CreatedBy = Convert.ToString(Session["Code"]);
                item.CreatedDate = DateTime.Now;
                db.Organizations.Add(item);
                db.SaveChanges();

            }


            return result;
        }
        #endregion



    }
}