using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using Klinik.Entities.MasterData;
using System.Data.OleDb;
using LinqToExcel;
using System.Net;
using System.Text.RegularExpressions;
using System.Data.Entity.Validation;
using Klinik.Data.DataRepository;

namespace Klinik.Web.Controllers
{
    public class ExportImportController : Controller
    {
        // GET: ExportImport
        private KlinikDBEntities db = new KlinikDBEntities();

        #region ::Gudang::
        [HttpPost]
        public FileResult ExportGudang()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Gudang");
            dt.Columns.AddRange(new DataColumn[7] { new DataColumn("name"),
                                            new DataColumn("ClinicId"),
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate"),
                                            new DataColumn("ModifiedBy"),
                                            new DataColumn("ModifiedDate"),
                                            new DataColumn("RowStatus")
            });

            var sales = from sale in entities.Gudangs.Take(11)
                        select sale;

            foreach (var sale in sales)
            {
                dt.Rows.Add(sale.name, sale.ClinicId, sale.CreatedBy, sale.CreatedDate, sale.ModifiedBy, sale.ModifiedDate, sale.RowStatus);
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
        public ActionResult UploadGudang(Gudang users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/fileDoc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Gudang$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "Gudang";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<Gudang>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            //if (a.Region != "")
                            //{
                            //    City TU = new Sale();
                            //    TU.AddedOn = a.AddedOn;
                            //    TU.Region = a.Region;
                            //    TU.Person = a.Person;
                            //    TU.Item = a.Item;
                            //    TU.Units = a.Units;
                            //    TU.UnitCost = a.UnitCost;
                            //    TU.Total = a.Total;
                            //    db.Sale.Add(TU);

                            //    db.SaveChanges();



                            //}
                            if (a.name != "")
                            {
                                Gudang TU = new Gudang();
                                TU.name = a.name;
                                TU.ClinicId = a.ClinicId;
                                TU.ModifiedBy = a.ModifiedBy;
                                TU.ModifiedDate = a.ModifiedDate;
                                TU.CreatedBy = a.CreatedBy;
                                TU.CreatedDate = a.CreatedDate;
                                TU.RowStatus = a.RowStatus;
                                db.Gudangs.Add(TU);

                                db.SaveChanges();



                            }
                            else
                            {
                                data.Add("<ul>");
                                //if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                                //if (a.Address == "" || a.Address == null) data.Add("<li> Address is required</li>");
                                //if (a.ContactNo == "" || a.ContactNo == null) data.Add("<li>ContactNo is required</li>");

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return RedirectToAction("GudangList", "MasterData"); ;
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ::Service::
        [HttpPost]
        public FileResult ExportService()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Service");
            dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Code"),
                                            new DataColumn("Name"),
                                            new DataColumn("Price"),
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate"),
                                            new DataColumn("ModifiedBy"),
                                            new DataColumn("ModifiedDate"),
                                            new DataColumn("RowStatus")
            });

            var services = from service in entities.Services.Take(11)
                        select service;

            foreach (var servis in services)
            {
                dt.Rows.Add(servis.Code, servis.Name, servis.Price, servis.CreatedBy, servis.CreatedDate, servis.ModifiedBy, servis.ModifiedDate, servis.RowStatus);
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
        [HttpPost]
        public ActionResult UploadService(Service users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/fileDoc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Service$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "Service";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<Service>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            //if (a.Region != "")
                            //{
                            //    City TU = new Sale();
                            //    TU.AddedOn = a.AddedOn;
                            //    TU.Region = a.Region;
                            //    TU.Person = a.Person;
                            //    TU.Item = a.Item;
                            //    TU.Units = a.Units;
                            //    TU.UnitCost = a.UnitCost;
                            //    TU.Total = a.Total;
                            //    db.Sale.Add(TU);

                            //    db.SaveChanges();



                            //}
                            if (a.Name != "")
                            {
                                Service TU = new Service();
                                TU.Code = a.Code;
                                TU.Name = a.Name;
                                TU.Price = a.Price;
                                TU.ModifiedBy = a.ModifiedBy;
                                TU.ModifiedDate = a.ModifiedDate;
                                TU.CreatedBy = a.CreatedBy;
                                TU.CreatedDate = a.CreatedDate;
                                TU.RowStatus = a.RowStatus;
                                db.Services.Add(TU);

                                db.SaveChanges();



                            }
                            else
                            {
                                data.Add("<ul>");
                                //if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                                //if (a.Address == "" || a.Address == null) data.Add("<li> Address is required</li>");
                                //if (a.ContactNo == "" || a.ContactNo == null) data.Add("<li>ContactNo is required</li>");

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return RedirectToAction("ServiceList", "MasterData"); ;
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ::PoliService::
        [HttpPost]
        public FileResult ExportPoliService()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("PoliService");
            dt.Columns.AddRange(new DataColumn[8] { new DataColumn("ServicesID"),
                                            new DataColumn("ClinicID"),
                                            new DataColumn("PoliID"),
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate"),
                                            new DataColumn("ModifiedBy"),
                                            new DataColumn("ModifiedDate"),
                                            new DataColumn("RowStatus")
            });

            var pservices = from pservice in entities.PoliServices.Take(11)
                           select pservice;

            foreach (var pservis in pservices)
            {
                dt.Rows.Add(pservis.ServicesID, pservis.ClinicID, pservis.PoliID, pservis.CreatedBy, pservis.CreatedDate, pservis.ModifiedBy, pservis.ModifiedDate, pservis.RowStatus);
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
        [HttpPost]
        public ActionResult UploadPoliService(PoliService users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/fileDoc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [PoliService$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "PoliService";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<PoliService>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            //if (a.Region != "")
                            //{
                            //    City TU = new Sale();
                            //    TU.AddedOn = a.AddedOn;
                            //    TU.Region = a.Region;
                            //    TU.Person = a.Person;
                            //    TU.Item = a.Item;
                            //    TU.Units = a.Units;
                            //    TU.UnitCost = a.UnitCost;
                            //    TU.Total = a.Total;
                            //    db.Sale.Add(TU);

                            //    db.SaveChanges();



                            //}
                            
                                PoliService TU = new PoliService();
                                TU.ServicesID = a.ServicesID;
                                TU.ClinicID = a.ClinicID;
                                TU.PoliID = a.PoliID;
                                TU.ModifiedBy = a.ModifiedBy;
                                TU.ModifiedDate = a.ModifiedDate;
                                TU.CreatedBy = a.CreatedBy;
                                TU.CreatedDate = a.CreatedDate;
                                TU.RowStatus = a.RowStatus;
                                db.PoliServices.Add(TU);

                                db.SaveChanges();



                            
                            //else
                            //{
                            //    data.Add("<ul>");
                            //    //if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                            //    //if (a.Address == "" || a.Address == null) data.Add("<li> Address is required</li>");
                            //    //if (a.ContactNo == "" || a.ContactNo == null) data.Add("<li>ContactNo is required</li>");

                            //    data.Add("</ul>");
                            //    data.ToArray();
                            //    return Json(data, JsonRequestBehavior.AllowGet);
                            //}
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return RedirectToAction("PoliServiceList", "MasterData"); ;
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ::Poli::
        [HttpPost]
        public FileResult ExportPoli()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Poli");
            dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Code"),
                                            new DataColumn("Name"),
                                            new DataColumn("Type"),
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate"),
                                            new DataColumn("ModifiedBy"),
                                            new DataColumn("ModifiedDate"),
                                            new DataColumn("RowStatus")
            });

            var polis = from poli in entities.Polis.Take(11)
                           select poli;

            foreach (var poli in polis)
            {
                dt.Rows.Add(poli.Code, poli.Name, poli.Type, poli.CreatedBy, poli.CreatedDate, poli.ModifiedBy, poli.ModifiedDate, poli.RowStatus);
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
        [HttpPost]
        public ActionResult UploadPoli(Poli users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/fileDoc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Poli$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "Poli";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<Poli>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            //if (a.Region != "")
                            //{
                            //    City TU = new Sale();
                            //    TU.AddedOn = a.AddedOn;
                            //    TU.Region = a.Region;
                            //    TU.Person = a.Person;
                            //    TU.Item = a.Item;
                            //    TU.Units = a.Units;
                            //    TU.UnitCost = a.UnitCost;
                            //    TU.Total = a.Total;
                            //    db.Sale.Add(TU);

                            //    db.SaveChanges();



                            //}
                            if (a.Name != "")
                            {
                                Poli TU = new Poli();
                                TU.Code = a.Code;
                                TU.Name = a.Name;
                                TU.Type = a.Type;
                                TU.ModifiedBy = a.ModifiedBy;
                                TU.ModifiedDate = a.ModifiedDate;
                                TU.CreatedBy = a.CreatedBy;
                                TU.CreatedDate = a.CreatedDate;
                                TU.RowStatus = a.RowStatus;
                                db.Polis.Add(TU);

                                db.SaveChanges();



                            }
                            else
                            {
                                data.Add("<ul>");
                                //if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                                //if (a.Address == "" || a.Address == null) data.Add("<li> Address is required</li>");
                                //if (a.ContactNo == "" || a.ContactNo == null) data.Add("<li>ContactNo is required</li>");

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return RedirectToAction("PoliList", "MasterData"); ;
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ::Vendor::
        [HttpPost]
        public FileResult ExportVendor()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Vendor");
            dt.Columns.AddRange(new DataColumn[6] { 
                                            new DataColumn("namavendor"),
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate"),
                                            new DataColumn("ModifiedBy"),
                                            new DataColumn("ModifiedDate"),
                                            new DataColumn("RowStatus")
            });

            var vendors = from v in entities.Vendors.Take(11)
                        select v;

            foreach (var vendor in vendors)
            {
                dt.Rows.Add(vendor.namavendor, vendor.CreatedBy, vendor.CreatedDate, vendor.ModifiedBy, vendor.ModifiedDate, vendor.RowStatus);
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
        [HttpPost]
        public ActionResult UploadVendor(Vendor users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/fileDoc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Vendor$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "Vendor";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<Vendor>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            //if (a.Region != "")
                            //{
                            //    City TU = new Sale();
                            //    TU.AddedOn = a.AddedOn;
                            //    TU.Region = a.Region;
                            //    TU.Person = a.Person;
                            //    TU.Item = a.Item;
                            //    TU.Units = a.Units;
                            //    TU.UnitCost = a.UnitCost;
                            //    TU.Total = a.Total;
                            //    db.Sale.Add(TU);

                            //    db.SaveChanges();



                            //}
                            if (a.namavendor != "")
                            {
                                Vendor TU = new Vendor();
                                TU.namavendor = a.namavendor;
                                TU.ModifiedBy = a.ModifiedBy;
                                TU.ModifiedDate = a.ModifiedDate;
                                TU.CreatedBy = a.CreatedBy;
                                TU.CreatedDate = a.CreatedDate;
                                TU.RowStatus = a.RowStatus;
                                db.Vendors.Add(TU);

                                db.SaveChanges();



                            }
                            else
                            {
                                data.Add("<ul>");
                                //if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                                //if (a.Address == "" || a.Address == null) data.Add("<li> Address is required</li>");
                                //if (a.ContactNo == "" || a.ContactNo == null) data.Add("<li>ContactNo is required</li>");

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return RedirectToAction("VendorList", "MasterData"); ;
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ::Patient::
        [HttpPost]
        public FileResult ExportPatient()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Patient");
            dt.Columns.AddRange(new DataColumn[20] {
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
                                            new DataColumn("PatientKey"),
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate"),
                                            new DataColumn("ModifiedBy"),
                                            new DataColumn("ModifiedDate"),
                                            new DataColumn("RowStatus")
            });

            var patients = from pasien in entities.Patients.Take(11)
                          select pasien;

            foreach (var patient in patients)
            {
                dt.Rows.Add(patient.EmployeeID, patient.FamilyRelationshipID, patient.MRNumber, patient.Name, patient.Gender, patient.MaritalStatus, patient.BirthDate, patient.KTPNumber, patient.Address, patient.CityID, patient.HPNumber, patient.Type, patient.BloodType, patient.BPJSNumber, patient.PatientKey, patient.CreatedBy, patient.CreatedDate, patient.ModifiedBy, patient.ModifiedDate, patient.RowStatus);
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
        public ActionResult UploadPatient(Patient users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/fileDoc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Patient$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "Patient";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<Patient>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            //if (a.Region != "")
                            //{
                            //    City TU = new Sale();
                            //    TU.AddedOn = a.AddedOn;
                            //    TU.Region = a.Region;
                            //    TU.Person = a.Person;
                            //    TU.Item = a.Item;
                            //    TU.Units = a.Units;
                            //    TU.UnitCost = a.UnitCost;
                            //    TU.Total = a.Total;
                            //    db.Sale.Add(TU);

                            //    db.SaveChanges();


                            //TU.MRNumber, TU.Name, TU.Gender, TU.MaritalStatus, TU.BirthDate, TU.KTPNumber, TU.Address, TU.CityID, TU.HPNumber, TU.Type, TU.BloodType, TU.BPJSNumber, TU.PatientKey, TU.ModifiedBy, TU.ModifiedDate, TU.CreatedBy, TU.CreatedDate, TU.RowStatus
                            //}
                            if (a.Name != "")
                            {
                                Patient TU = new Patient();
                                TU.EmployeeID= a.EmployeeID;
                                TU.FamilyRelationshipID= a.FamilyRelationshipID;
                                TU.MRNumber = a.MRNumber;
                                TU.Name = a.Name;
                                TU.Gender = a.Gender;
                                TU.MaritalStatus = a.MaritalStatus;
                                TU.BirthDate = a.BirthDate;
                                TU.KTPNumber = a.KTPNumber;
                                TU.Address = a.Address;
                                TU.CityID = a.CityID;
                                TU.HPNumber = a.HPNumber;
                                TU.Type = a.Type;
                                TU.BloodType = a.BloodType;
                                TU.PatientKey = a.PatientKey;
                                TU.BPJSNumber = a.BPJSNumber;
                                TU.RowStatus = a.RowStatus;
                                TU.ModifiedBy = a.ModifiedBy;
                                TU.ModifiedDate = a.ModifiedDate;
                                TU.CreatedBy = a.CreatedBy;
                                TU.CreatedDate = a.CreatedDate;

                                db.Patients.Add(TU);

                                db.SaveChanges();



                            }
                            else
                            {
                                data.Add("<ul>");
                                //if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                                //if (a.Address == "" || a.Address == null) data.Add("<li> Address is required</li>");
                                //if (a.ContactNo == "" || a.ContactNo == null) data.Add("<li>ContactNo is required</li>");

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return RedirectToAction("PasienList", "Patient"); ;
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ::LabItem::
        [HttpPost]
        public FileResult ExportLab()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("LabItem");
            dt.Columns.AddRange(new DataColumn[10] {
                                            new DataColumn("Code"),
                                            new DataColumn("Name"),
                                            new DataColumn("LabItemCategoryID"),
                                            new DataColumn("Normal"),
                                            new DataColumn("Price"),
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate"),
                                            new DataColumn("ModifiedBy"),
                                            new DataColumn("ModifiedDate"),
                                            new DataColumn("RowStatus")
            });

            var labitems = from labitem in entities.LabItems.Take(11)
                           select labitem;

            foreach (var k in labitems)
            {
                dt.Rows.Add(k.Code, k.Name, k.LabItemCategoryID, k.Normal, k.Price, k.CreatedBy, k.CreatedDate, k.ModifiedBy, k.ModifiedDate, k.RowStatus);
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
        [HttpPost]
        public ActionResult UploadLab(LabItem users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/fileDoc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [LabItem$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "LabItem";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<LabItem>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            //if (a.Region != "")
                            //{
                            //    City TU = new Sale();
                            //    TU.AddedOn = a.AddedOn;
                            //    TU.Region = a.Region;
                            //    TU.Person = a.Person;
                            //    TU.Item = a.Item;
                            //    TU.Units = a.Units;
                            //    TU.UnitCost = a.UnitCost;
                            //    TU.Total = a.Total;
                            //    db.Sale.Add(TU);

                            //    db.SaveChanges();


                            //TU.MRNumber, TU.Name, TU.Gender, TU.MaritalStatus, TU.BirthDate, TU.KTPNumber, TU.Address, TU.CityID, TU.HPNumber, TU.Type, TU.BloodType, TU.BPJSNumber, TU.PatientKey, TU.ModifiedBy, TU.ModifiedDate, TU.CreatedBy, TU.CreatedDate, TU.RowStatus
                            //}
                            if (a.Name != "")
                            {
                                LabItem TU = new LabItem();
                                TU.Code = a.Code;
                                TU.Name = a.Name;
                                TU.LabItemCategoryID = a.LabItemCategoryID;
                                TU.Normal = a.Normal;
                                TU.Price = a.Price;
                                TU.RowStatus = a.RowStatus;
                                TU.ModifiedBy = a.ModifiedBy;
                                TU.ModifiedDate = a.ModifiedDate;
                                TU.CreatedBy = a.CreatedBy;
                                TU.CreatedDate = a.CreatedDate;

                                db.LabItems.Add(TU);

                                db.SaveChanges();



                            }
                            else
                            {
                                data.Add("<ul>");
                                //if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                                //if (a.Address == "" || a.Address == null) data.Add("<li> Address is required</li>");
                                //if (a.ContactNo == "" || a.ContactNo == null) data.Add("<li>ContactNo is required</li>");

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return RedirectToAction("LabItemList", "MasterData"); ;
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ::LabItemCategory::
        [HttpPost]
        public FileResult ExportLabCategory()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("LabItemCategory");
            dt.Columns.AddRange(new DataColumn[8] {
                                            new DataColumn("LabType"),
                                            new DataColumn("PoliID"),
                                            new DataColumn("Name"),
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate"),
                                            new DataColumn("ModifiedBy"),
                                            new DataColumn("ModifiedDate"),
                                            new DataColumn("RowStatus")
            });

            var labitemcs = from labitemc in entities.LabItemCategories.Take(11)
                           select labitemc;

            foreach (var k in labitemcs)
            {
                dt.Rows.Add(k.LabType, k.Name, k.PoliID, k.CreatedBy, k.CreatedDate, k.ModifiedBy, k.ModifiedDate, k.RowStatus);
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
        [HttpPost]
        public ActionResult UploadLabCategory(LabItemCategory users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/fileDoc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [LabItemCategory$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "LabItemCategory";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<LabItemCategory>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            //if (a.Region != "")
                            //{
                            //    City TU = new Sale();
                            //    TU.AddedOn = a.AddedOn;
                            //    TU.Region = a.Region;
                            //    TU.Person = a.Person;
                            //    TU.Item = a.Item;
                            //    TU.Units = a.Units;
                            //    TU.UnitCost = a.UnitCost;
                            //    TU.Total = a.Total;
                            //    db.Sale.Add(TU);

                            //    db.SaveChanges();


                            //TU.MRNumber, TU.Name, TU.Gender, TU.MaritalStatus, TU.BirthDate, TU.KTPNumber, TU.Address, TU.CityID, TU.HPNumber, TU.Type, TU.BloodType, TU.BPJSNumber, TU.PatientKey, TU.ModifiedBy, TU.ModifiedDate, TU.CreatedBy, TU.CreatedDate, TU.RowStatus
                            //}
                            if (a.Name != "")
                            {
                                LabItemCategory TU = new LabItemCategory();
                                TU.LabType = a.LabType;
                                TU.Name = a.Name;
                                TU.PoliID = a.PoliID;
                                TU.RowStatus = a.RowStatus;
                                TU.ModifiedBy = a.ModifiedBy;
                                TU.ModifiedDate = a.ModifiedDate;
                                TU.CreatedBy = a.CreatedBy;
                                TU.CreatedDate = a.CreatedDate;

                                db.LabItemCategories.Add(TU);

                                db.SaveChanges();



                            }
                            else
                            {
                                data.Add("<ul>");
                                //if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                                //if (a.Address == "" || a.Address == null) data.Add("<li> Address is required</li>");
                                //if (a.ContactNo == "" || a.ContactNo == null) data.Add("<li>ContactNo is required</li>");

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return RedirectToAction("LabItemCategoryList", "MasterData"); ;
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ::Product::
        [HttpPost]
        public FileResult ExportProduct()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Product");
            dt.Columns.AddRange(new DataColumn[11] {
                                            new DataColumn("Code"),
                                            new DataColumn("Name"),
                                            new DataColumn("ClinicID"),
                                            new DataColumn("ProductCategoryID"),
                                            new DataColumn("ProductUnitID"),
                                            new DataColumn("RetailPrice"),
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate"),
                                            new DataColumn("ModifiedBy"),
                                            new DataColumn("ModifiedDate"),
                                            new DataColumn("RowStatus")
            });

            var products = from product in entities.Products.Take(11)
                           select product;

            foreach (var k in products)
            {
                dt.Rows.Add(k.Code, k.Name, k.ProductCategoryID, k.ProductUnitID, k.ClinicID, k.RetailPrice, k.CreatedBy, k.CreatedDate, k.ModifiedBy, k.ModifiedDate, k.RowStatus);
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
        [HttpPost]
        public ActionResult UploadProduct(Product users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/fileDoc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Product$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "Product";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<Product>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            //if (a.Region != "")
                            //{
                            //    City TU = new Sale();
                            //    TU.AddedOn = a.AddedOn;
                            //    TU.Region = a.Region;
                            //    TU.Person = a.Person;
                            //    TU.Item = a.Item;
                            //    TU.Units = a.Units;
                            //    TU.UnitCost = a.UnitCost;
                            //    TU.Total = a.Total;
                            //    db.Sale.Add(TU);

                            //    db.SaveChanges();


                            //TU.MRNumber, TU.Name, TU.Gender, TU.MaritalStatus, TU.BirthDate, TU.KTPNumber, TU.Address, TU.CityID, TU.HPNumber, TU.Type, TU.BloodType, TU.BPJSNumber, TU.PatientKey, TU.ModifiedBy, TU.ModifiedDate, TU.CreatedBy, TU.CreatedDate, TU.RowStatus
                            //}
                            if (a.Name != "")
                            {
                                Product TU = new Product();
                                TU.Code = a.Code;
                                TU.Name = a.Name;
                                TU.ProductCategoryID = a.ProductCategoryID;
                                TU.ProductUnitID = a.ProductUnitID;
                                TU.ClinicID = a.ClinicID;
                                TU.RetailPrice = a.RetailPrice;
                                TU.RowStatus = a.RowStatus;
                                TU.ModifiedBy = a.ModifiedBy;
                                TU.ModifiedDate = a.ModifiedDate;
                                TU.CreatedBy = a.CreatedBy;
                                TU.CreatedDate = a.CreatedDate;

                                db.Products.Add(TU);

                                db.SaveChanges();



                            }
                            else
                            {
                                data.Add("<ul>");
                                //if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                                //if (a.Address == "" || a.Address == null) data.Add("<li> Address is required</li>");
                                //if (a.ContactNo == "" || a.ContactNo == null) data.Add("<li>ContactNo is required</li>");

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return RedirectToAction("ProductList", "MasterData"); ;
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ::ProductCategory::
        [HttpPost]
        public FileResult ExportProductCategory()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("ProductCategory");
            dt.Columns.AddRange(new DataColumn[6] {
                                            
                                            new DataColumn("Name"),
                                            
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate"),
                                            new DataColumn("ModifiedBy"),
                                            new DataColumn("ModifiedDate"),
                                            new DataColumn("RowStatus")
            });

            var productcs = from productc in entities.ProductCategories.Take(11)
                           select productc;

            foreach (var k in productcs)
            {
                dt.Rows.Add(k.Name, k.CreatedBy, k.CreatedDate, k.ModifiedBy, k.ModifiedDate, k.RowStatus);
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
        [HttpPost]
        public ActionResult UploadProductCategory(ProductCategory users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/fileDoc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [ProductCategory$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "ProductCategory";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<ProductCategory>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            //if (a.Region != "")
                            //{
                            //    City TU = new Sale();
                            //    TU.AddedOn = a.AddedOn;
                            //    TU.Region = a.Region;
                            //    TU.Person = a.Person;
                            //    TU.Item = a.Item;
                            //    TU.Units = a.Units;
                            //    TU.UnitCost = a.UnitCost;
                            //    TU.Total = a.Total;
                            //    db.Sale.Add(TU);

                            //    db.SaveChanges();


                            //TU.MRNumber, TU.Name, TU.Gender, TU.MaritalStatus, TU.BirthDate, TU.KTPNumber, TU.Address, TU.CityID, TU.HPNumber, TU.Type, TU.BloodType, TU.BPJSNumber, TU.PatientKey, TU.ModifiedBy, TU.ModifiedDate, TU.CreatedBy, TU.CreatedDate, TU.RowStatus
                            //}
                            if (a.Name != "")
                            {
                                ProductCategory TU = new ProductCategory();
                                TU.Name = a.Name;
                                TU.RowStatus = a.RowStatus;
                                TU.ModifiedBy = a.ModifiedBy;
                                TU.ModifiedDate = a.ModifiedDate;
                                TU.CreatedBy = a.CreatedBy;
                                TU.CreatedDate = a.CreatedDate;

                                db.ProductCategories.Add(TU);

                                db.SaveChanges();



                            }
                            else
                            {
                                data.Add("<ul>");
                                //if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                                //if (a.Address == "" || a.Address == null) data.Add("<li> Address is required</li>");
                                //if (a.ContactNo == "" || a.ContactNo == null) data.Add("<li>ContactNo is required</li>");

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return RedirectToAction("ProductCategoryList", "MasterData"); ;
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ::ProductUnit::
        [HttpPost]
        public FileResult ExportProductUnit()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("ProductUnit");
            dt.Columns.AddRange(new DataColumn[7] {
                                            new DataColumn("Code"),
                                            new DataColumn("Name"),
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate"),
                                            new DataColumn("ModifiedBy"),
                                            new DataColumn("ModifiedDate"),
                                            new DataColumn("RowStatus")
            });

            var productunits = from productunit in entities.ProductUnits.Take(11)
                           select productunit;

            foreach (var k in productunits)
            {
                dt.Rows.Add(k.Code, k.Name, k.CreatedBy, k.CreatedDate, k.ModifiedBy, k.ModifiedDate, k.RowStatus);
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
        [HttpPost]
        public ActionResult UploadProductUnit(ProductUnit users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/fileDoc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [ProductUnit$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "ProductUnit";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<ProductUnit>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            //if (a.Region != "")
                            //{
                            //    City TU = new Sale();
                            //    TU.AddedOn = a.AddedOn;
                            //    TU.Region = a.Region;
                            //    TU.Person = a.Person;
                            //    TU.Item = a.Item;
                            //    TU.Units = a.Units;
                            //    TU.UnitCost = a.UnitCost;
                            //    TU.Total = a.Total;
                            //    db.Sale.Add(TU);

                            //    db.SaveChanges();


                            //TU.MRNumber, TU.Name, TU.Gender, TU.MaritalStatus, TU.BirthDate, TU.KTPNumber, TU.Address, TU.CityID, TU.HPNumber, TU.Type, TU.BloodType, TU.BPJSNumber, TU.PatientKey, TU.ModifiedBy, TU.ModifiedDate, TU.CreatedBy, TU.CreatedDate, TU.RowStatus
                            //}
                            if (a.Name != "")
                            {
                                ProductUnit TU = new ProductUnit();
                                TU.Code = a.Code;
                                TU.Name = a.Name;
                                TU.RowStatus = a.RowStatus;
                                TU.ModifiedBy = a.ModifiedBy;
                                TU.ModifiedDate = a.ModifiedDate;
                                TU.CreatedBy = a.CreatedBy;
                                TU.CreatedDate = a.CreatedDate;

                                db.ProductUnits.Add(TU);

                                db.SaveChanges();



                            }
                            else
                            {
                                data.Add("<ul>");
                                //if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                                //if (a.Address == "" || a.Address == null) data.Add("<li> Address is required</li>");
                                //if (a.ContactNo == "" || a.ContactNo == null) data.Add("<li>ContactNo is required</li>");

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return RedirectToAction("ProductUnitList", "MasterData"); ;
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ::ProductMedicine::
        [HttpPost]
        public FileResult ExportProductMedicine()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("ProductMedicine");
            dt.Columns.AddRange(new DataColumn[8] {
                                            new DataColumn("ProductID"),
                                            new DataColumn("MedicineID"),
                                            new DataColumn("Amount"),
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate"),
                                            new DataColumn("ModifiedBy"),
                                            new DataColumn("ModifiedDate"),
                                            new DataColumn("RowStatus")
            });

            var productms = from productm in entities.ProductMedicines.Take(11)
                           select productm;

            foreach (var k in productms)
            {
                dt.Rows.Add(k.ProductID, k.MedicineID, k.Amount, k.CreatedBy, k.CreatedDate, k.ModifiedBy, k.ModifiedDate, k.RowStatus);
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
        [HttpPost]
        public ActionResult UploadProductMedicine(ProductMedicine users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/fileDoc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [ProductMedicine$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "ProductMedicine";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<ProductMedicine>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            //if (a.Region != "")
                            //{
                            //    City TU = new Sale();
                            //    TU.AddedOn = a.AddedOn;
                            //    TU.Region = a.Region;
                            //    TU.Person = a.Person;
                            //    TU.Item = a.Item;
                            //    TU.Units = a.Units;
                            //    TU.UnitCost = a.UnitCost;
                            //    TU.Total = a.Total;
                            //    db.Sale.Add(TU);

                            //    db.SaveChanges();


                            //TU.MRNumber, TU.Name, TU.Gender, TU.MaritalStatus, TU.BirthDate, TU.KTPNumber, TU.Address, TU.CityID, TU.HPNumber, TU.Type, TU.BloodType, TU.BPJSNumber, TU.PatientKey, TU.ModifiedBy, TU.ModifiedDate, TU.CreatedBy, TU.CreatedDate, TU.RowStatus
                            //}

                                ProductMedicine TU = new ProductMedicine();
                                TU.ProductID = a.ProductID;
                                TU.MedicineID = a.MedicineID;
                                TU.Amount = a.Amount;
                                TU.RowStatus = a.RowStatus;
                                TU.ModifiedBy = a.ModifiedBy;
                                TU.ModifiedDate = a.ModifiedDate;
                                TU.CreatedBy = a.CreatedBy;
                                TU.CreatedDate = a.CreatedDate;

                                db.ProductMedicines.Add(TU);

                                db.SaveChanges();



                            
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return RedirectToAction("ProductMedicineList", "MasterData"); ;
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ::Medicine::
        [HttpPost]
        public FileResult ExportMedicine()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Medicine");
            dt.Columns.AddRange(new DataColumn[6] {
                                            new DataColumn("Name"),
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate"),
                                            new DataColumn("ModifiedBy"),
                                            new DataColumn("ModifiedDate"),
                                            new DataColumn("RowStatus")
            });

            var medicines = from medicine in entities.Medicines.Take(11)
                            select medicine;

            foreach (var k in medicines)
            {
                dt.Rows.Add(k.Name, k.CreatedBy, k.CreatedDate, k.ModifiedBy, k.ModifiedDate, k.RowStatus);
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
        [HttpPost]
        public ActionResult UploadMedicine(Medicine users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/fileDoc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Medicine$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "Medicine";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<Medicine>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            //if (a.Region != "")
                            //{
                            //    City TU = new Sale();
                            //    TU.AddedOn = a.AddedOn;
                            //    TU.Region = a.Region;
                            //    TU.Person = a.Person;
                            //    TU.Item = a.Item;
                            //    TU.Units = a.Units;
                            //    TU.UnitCost = a.UnitCost;
                            //    TU.Total = a.Total;
                            //    db.Sale.Add(TU);

                            //    db.SaveChanges();


                            //TU.MRNumber, TU.Name, TU.Gender, TU.MaritalStatus, TU.BirthDate, TU.KTPNumber, TU.Address, TU.CityID, TU.HPNumber, TU.Type, TU.BloodType, TU.BPJSNumber, TU.PatientKey, TU.ModifiedBy, TU.ModifiedDate, TU.CreatedBy, TU.CreatedDate, TU.RowStatus
                            //}

                            Medicine TU = new Medicine();
                            TU.Name = a.Name;
                            TU.RowStatus = a.RowStatus;
                            TU.ModifiedBy = a.ModifiedBy;
                            TU.ModifiedDate = a.ModifiedDate;
                            TU.CreatedBy = a.CreatedBy;
                            TU.CreatedDate = a.CreatedDate;

                            db.Medicines.Add(TU);

                            db.SaveChanges();




                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return RedirectToAction("MedicineList", "MasterData"); ;
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ::Clinic::
        [HttpPost]
        public FileResult ExportClinic()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Clinic");
            dt.Columns.AddRange(new DataColumn[17] {
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
                                            new DataColumn("ReffID"),
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate"),
                                            new DataColumn("ModifiedBy"),
                                            new DataColumn("ModifiedDate"),
                                            new DataColumn("RowStatus")
            });

            var clinics = from klinik in entities.Clinics.Take(11)
                           select klinik;

            foreach (var k in clinics)
            {
                dt.Rows.Add(k.Code, k.Name, k.Address, k.LegalNumber, k.LegalDate, k.ContactNumber, k.Email, k.Lat, k.Long, k.CityID, k.ClinicType, k.ReffID, k.CreatedBy, k.CreatedDate, k.ModifiedBy, k.ModifiedDate, k.RowStatus);
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
        [HttpPost]
        public ActionResult UploadClinic(Clinic users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/fileDoc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Clinic$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "Clinic";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<Clinic>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            //if (a.Region != "")
                            //{
                            //    City TU = new Sale();
                            //    TU.AddedOn = a.AddedOn;
                            //    TU.Region = a.Region;
                            //    TU.Person = a.Person;
                            //    TU.Item = a.Item;
                            //    TU.Units = a.Units;
                            //    TU.UnitCost = a.UnitCost;
                            //    TU.Total = a.Total;
                            //    db.Sale.Add(TU);

                            //    db.SaveChanges();


                            //TU.MRNumber, TU.Name, TU.Gender, TU.MaritalStatus, TU.BirthDate, TU.KTPNumber, TU.Address, TU.CityID, TU.HPNumber, TU.Type, TU.BloodType, TU.BPJSNumber, TU.PatientKey, TU.ModifiedBy, TU.ModifiedDate, TU.CreatedBy, TU.CreatedDate, TU.RowStatus
                            //}
                            if (a.Name != "")
                            {
                                Clinic TU = new Clinic();
                                TU.Code = a.Name;
                                TU.Name = a.Name;
                                TU.Address = a.Address;
                                TU.LegalDate = a.LegalDate;
                                TU.LegalNumber = a.LegalNumber;
                                TU.ContactNumber = a.ContactNumber;
                                TU.Email = a.Email;
                                TU.Lat = a.Lat;
                                TU.CityID = a.CityID;
                                TU.Long = a.Long;
                                TU.ClinicType = a.ClinicType;
                                TU.ReffID = a.ReffID;
                                TU.RowStatus = a.RowStatus;
                                TU.ModifiedBy = a.ModifiedBy;
                                TU.ModifiedDate = a.ModifiedDate;
                                TU.CreatedBy = a.CreatedBy;
                                TU.CreatedDate = a.CreatedDate;

                                db.Clinics.Add(TU);

                                db.SaveChanges();



                            }
                            else
                            {
                                data.Add("<ul>");
                                //if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                                //if (a.Address == "" || a.Address == null) data.Add("<li> Address is required</li>");
                                //if (a.ContactNo == "" || a.ContactNo == null) data.Add("<li>ContactNo is required</li>");

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return RedirectToAction("ClinicList", "MasterData"); ;
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ::Doctor::
        [HttpPost]
        public FileResult ExportDoctor()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Doctor");
            dt.Columns.AddRange(new DataColumn[18] {
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
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate"),
                                            new DataColumn("ModifiedBy"),
                                            new DataColumn("ModifiedDate"),
                                            new DataColumn("RowStatus"),
                                            new DataColumn("EmployeeID")
            });

            var doctors = from doctor in entities.Doctors.Take(11)
                          select doctor;

            foreach (var k in doctors)
            {
                dt.Rows.Add(k.Code, k.Name, k.Address, k.SpecialistID, k.TypeID, k.KTPNumber, k.STRNumber, k.STRValidFrom, k.STRValidTo, k.HPNumber, k.Remark, k.Email, k.CreatedBy, k.CreatedDate, k.ModifiedBy, k.ModifiedDate, k.RowStatus, k.EmployeeID);
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
        [HttpPost]
        public ActionResult UploadDoctor(Doctor users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/fileDoc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Doctor$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "Doctor";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<Doctor>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            //if (a.Region != "")
                            //{
                            //    City TU = new Sale();
                            //    TU.AddedOn = a.AddedOn;
                            //    TU.Region = a.Region;
                            //    TU.Person = a.Person;
                            //    TU.Item = a.Item;
                            //    TU.Units = a.Units;
                            //    TU.UnitCost = a.UnitCost;
                            //    TU.Total = a.Total;
                            //    db.Sale.Add(TU);

                            //    db.SaveChanges();


                            //TU.MRNumber, TU.Name, TU.Gender, TU.MaritalStatus, TU.BirthDate, TU.KTPNumber, TU.Address, TU.CityID, TU.HPNumber, TU.Type, TU.BloodType, TU.BPJSNumber, TU.PatientKey, TU.ModifiedBy, TU.ModifiedDate, TU.CreatedBy, TU.CreatedDate, TU.RowStatus
                            //}
                            if (a.Name != "")
                            {
                                Doctor TU = new Doctor();
                                TU.Code = a.Name;
                                TU.Name = a.Name;
                                TU.Address = a.Address;
                                TU.SpecialistID = a.SpecialistID;
                                TU.TypeID = a.TypeID;
                                TU.KTPNumber = a.KTPNumber;
                                TU.Email = a.Email;
                                TU.STRNumber = a.STRNumber;
                                TU.STRValidTo = a.STRValidTo;
                                TU.HPNumber = a.HPNumber;
                                TU.Remark = a.Remark;
                                TU.STRValidFrom = a.STRValidFrom;
                                TU.RowStatus = a.RowStatus;
                                TU.ModifiedBy = a.ModifiedBy;
                                TU.ModifiedDate = a.ModifiedDate;
                                TU.CreatedBy = a.CreatedBy;
                                TU.CreatedDate = a.CreatedDate;
                                TU.EmployeeID = a.EmployeeID;
                                db.Doctors.Add(TU);

                                db.SaveChanges();



                            }
                            else
                            {
                                data.Add("<ul>");
                                //if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                                //if (a.Address == "" || a.Address == null) data.Add("<li> Address is required</li>");
                                //if (a.ContactNo == "" || a.ContactNo == null) data.Add("<li>ContactNo is required</li>");

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return RedirectToAction("Index", "Doctor"); ;
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ::Employee::
        [HttpPost]
        public FileResult ExportEmployee()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Employee");
            dt.Columns.AddRange(new DataColumn[15] {
                                            new DataColumn("EmpID"),
                                            new DataColumn("EmpName"),
                                            new DataColumn("BirthDate"),
                                            new DataColumn("ReffEmpID"),
                                            new DataColumn("Gender"),
                                            new DataColumn("EmpType"),
                                            new DataColumn("KTPNumber"),
                                            new DataColumn("LastEmpID"),
                                            new DataColumn("HPNumber"),
                                            new DataColumn("Email"),
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate"),
                                            new DataColumn("ModifiedBy"),
                                            new DataColumn("ModifiedDate"),
                                            new DataColumn("RowStatus")
            });

            var emps = from emp in entities.Employees.Take(11)
                          select emp;

            foreach (var k in emps)
            {
                dt.Rows.Add(k.EmpID, k.EmpName, k.BirthDate, k.ReffEmpID, k.Gender, k.FamilyRelationship.Name, k.KTPNumber, k.LastEmpID, k.HPNumber, k.Email, k.CreatedBy, k.CreatedDate, k.ModifiedBy, k.ModifiedDate, k.RowStatus);
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
        [HttpPost]
        public ActionResult UploadEmployee(Employee users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/fileDoc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Employee$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "Employee";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<Employee>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            //if (a.Region != "")
                            //{
                            //    City TU = new Sale();
                            //    TU.AddedOn = a.AddedOn;
                            //    TU.Region = a.Region;
                            //    TU.Person = a.Person;
                            //    TU.Item = a.Item;
                            //    TU.Units = a.Units;
                            //    TU.UnitCost = a.UnitCost;
                            //    TU.Total = a.Total;
                            //    db.Sale.Add(TU);

                            //    db.SaveChanges();


                            //TU.MRNumber, TU.Name, TU.Gender, TU.MaritalStatus, TU.BirthDate, TU.KTPNumber, TU.Address, TU.CityID, TU.HPNumber, TU.Type, TU.BloodType, TU.BPJSNumber, TU.PatientKey, TU.ModifiedBy, TU.ModifiedDate, TU.CreatedBy, TU.CreatedDate, TU.RowStatus
                            //}
                            if (a.EmpName != "")
                            {
                                Employee TU = new Employee();
                                TU.EmpID = a.EmpID;
                                TU.EmpName = a.EmpName;
                                TU.BirthDate = a.BirthDate;
                                TU.EmpType = a.EmpType;
                                TU.LastEmpID = a.LastEmpID;
                                TU.KTPNumber = a.KTPNumber;
                                TU.Email = a.Email;
                                TU.ReffEmpID = a.ReffEmpID;
                                TU.Gender = a.Gender;
                                TU.HPNumber = a.HPNumber;
                                TU.RowStatus = a.RowStatus;
                                TU.ModifiedBy = a.ModifiedBy;
                                TU.ModifiedDate = a.ModifiedDate;
                                TU.CreatedBy = a.CreatedBy;
                                TU.CreatedDate = a.CreatedDate;

                                db.Employees.Add(TU);

                                db.SaveChanges();



                            }
                            else
                            {
                                data.Add("<ul>");
                                //if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                                //if (a.Address == "" || a.Address == null) data.Add("<li> Address is required</li>");
                                //if (a.ContactNo == "" || a.ContactNo == null) data.Add("<li>ContactNo is required</li>");

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return RedirectToAction("EmployeeList", "MasterData"); ;
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ::PoliSchedule::
        [HttpPost]
        public FileResult ExportPoliSchedule()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("PoliSchedule");
            dt.Columns.AddRange(new DataColumn[13] {
                                            new DataColumn("ClinicID"),
                                            new DataColumn("DoctorID"),
                                            new DataColumn("PoliID"),
                                            new DataColumn("StartDate"),
                                            new DataColumn("EndDate"),
                                            new DataColumn("ReffID"),
                                            new DataColumn("Remark"),
                                            new DataColumn("Status"),
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate"),
                                            new DataColumn("ModifiedBy"),
                                            new DataColumn("ModifiedDate"),
                                            new DataColumn("RowStatus")
            });

            var poliscs = from polisc in entities.PoliSchedules.Take(11)
                       select polisc;

            foreach (var k in poliscs)
            {
                dt.Rows.Add(k.ClinicID, k.DoctorID, k.PoliID, k.StartDate, k.EndDate, k.ReffID, k.Remark, k.Status, k.CreatedBy, k.CreatedDate, k.ModifiedBy, k.ModifiedDate, k.RowStatus);
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
        public ActionResult UploadPoliSchedule(PoliSchedule users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/fileDoc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [PoliSchedule$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "PoliSchedule";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<PoliSchedule>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            //if (a.Region != "")
                            //{
                            //    City TU = new Sale();
                            //    TU.AddedOn = a.AddedOn;
                            //    TU.Region = a.Region;
                            //    TU.Person = a.Person;
                            //    TU.Item = a.Item;
                            //    TU.Units = a.Units;
                            //    TU.UnitCost = a.UnitCost;
                            //    TU.Total = a.Total;
                            //    db.Sale.Add(TU);

                            //    db.SaveChanges();


                            //TU.MRNumber, TU.Name, TU.Gender, TU.MaritalStatus, TU.BirthDate, TU.KTPNumber, TU.Address, TU.CityID, TU.HPNumber, TU.Type, TU.BloodType, TU.BPJSNumber, TU.PatientKey, TU.ModifiedBy, TU.ModifiedDate, TU.CreatedBy, TU.CreatedDate, TU.RowStatus
                            //}
                            
                                PoliSchedule TU = new PoliSchedule();
                                TU.ClinicID = a.ClinicID;
                                TU.DoctorID = a.DoctorID;
                                TU.PoliID = a.PoliID;
                                TU.StartDate = a.StartDate;
                                TU.EndDate = a.EndDate;
                                TU.ReffID = a.ReffID;
                                TU.Remark = a.Remark;
                                TU.Status = a.Status;
                                TU.RowStatus = a.RowStatus;
                                TU.ModifiedBy = a.ModifiedBy;
                                TU.ModifiedDate = a.ModifiedDate;
                                TU.CreatedBy = a.CreatedBy;
                                TU.CreatedDate = a.CreatedDate;

                                db.PoliSchedules.Add(TU);

                                db.SaveChanges();



                           
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return RedirectToAction("Index", "PoliSchedule"); ;
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ::PoliScheduleMaster::
        [HttpPost]
        public FileResult ExportPoliScheduleMaster()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("PoliScheduleMaster");
            dt.Columns.AddRange(new DataColumn[11] {
                                            new DataColumn("ClinicID"),
                                            new DataColumn("DoctorID"),
                                            new DataColumn("PoliID"),
                                            new DataColumn("StartTime"),
                                            new DataColumn("EndTime"),
                                            new DataColumn("Day"),
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate"),
                                            new DataColumn("ModifiedBy"),
                                            new DataColumn("ModifiedDate"),
                                            new DataColumn("RowStatus")
            });

            var poliscms = from poliscm in entities.PoliScheduleMasters.Take(11)
                          select poliscm;

            foreach (var k in poliscms)
            {
                dt.Rows.Add(k.ClinicID, k.DoctorID, k.PoliID, k.StartTime, k.EndTime, k.Day, k.CreatedBy, k.CreatedDate, k.ModifiedBy, k.ModifiedDate, k.RowStatus);
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
        public ActionResult UploadPoliScheduleMaster(PoliScheduleMaster users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/fileDoc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [PoliScheduleMaster$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "PoliScheduleMaster";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<PoliScheduleMaster>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            //if (a.Region != "")
                            //{
                            //    City TU = new Sale();
                            //    TU.AddedOn = a.AddedOn;
                            //    TU.Region = a.Region;
                            //    TU.Person = a.Person;
                            //    TU.Item = a.Item;
                            //    TU.Units = a.Units;
                            //    TU.UnitCost = a.UnitCost;
                            //    TU.Total = a.Total;
                            //    db.Sale.Add(TU);

                            //    db.SaveChanges();


                            //TU.MRNumber, TU.Name, TU.Gender, TU.MaritalStatus, TU.BirthDate, TU.KTPNumber, TU.Address, TU.CityID, TU.HPNumber, TU.Type, TU.BloodType, TU.BPJSNumber, TU.PatientKey, TU.ModifiedBy, TU.ModifiedDate, TU.CreatedBy, TU.CreatedDate, TU.RowStatus
                            //}

                            PoliScheduleMaster TU = new PoliScheduleMaster();
                            TU.ClinicID = a.ClinicID;
                            TU.DoctorID = a.DoctorID;
                            TU.PoliID = a.PoliID;
                            TU.StartTime = a.StartTime;
                            TU.EndTime = a.EndTime;
                            TU.Day = a.Day;
                            TU.RowStatus = a.RowStatus;
                            TU.ModifiedBy = a.ModifiedBy;
                            TU.ModifiedDate = a.ModifiedDate;
                            TU.CreatedBy = a.CreatedBy;
                            TU.CreatedDate = a.CreatedDate;

                            db.PoliScheduleMasters.Add(TU);

                            db.SaveChanges();




                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return RedirectToAction("Master", "PoliSchedule"); ;
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ::User::
        [HttpPost]
        public FileResult ExportUser()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("User");
            dt.Columns.AddRange(new DataColumn[12] {
                                            new DataColumn("OrganizationID"),
                                            new DataColumn("UserName"),
                                            new DataColumn("Password"),
                                            new DataColumn("EmployeeID"),
                                            new DataColumn("ExpiredDate"),
                                            new DataColumn("ResetPasswordCode"),
                                            new DataColumn("Status"),
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate"),
                                            new DataColumn("ModifiedBy"),
                                            new DataColumn("ModifiedDate"),
                                            new DataColumn("RowStatus")
            });

            var users = from user in entities.Users.Take(11)
                       select user;

            foreach (var k in users)
            {
                dt.Rows.Add(k.OrganizationID, k.UserName, k.Password, k.EmployeeID, k.ResetPasswordCode, k.Status, k.ExpiredDate, k.CreatedBy, k.CreatedDate, k.ModifiedBy, k.ModifiedDate, k.RowStatus);
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
        [HttpPost]
        public ActionResult UploadUser(User users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/fileDoc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [User$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "User";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<User>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            //if (a.Region != "")
                            //{
                            //    City TU = new Sale();
                            //    TU.AddedOn = a.AddedOn;
                            //    TU.Region = a.Region;
                            //    TU.Person = a.Person;
                            //    TU.Item = a.Item;
                            //    TU.Units = a.Units;
                            //    TU.UnitCost = a.UnitCost;
                            //    TU.Total = a.Total;
                            //    db.Sale.Add(TU);

                            //    db.SaveChanges();


                            //TU.MRNumber, TU.Name, TU.Gender, TU.MaritalStatus, TU.BirthDate, TU.KTPNumber, TU.Address, TU.CityID, TU.HPNumber, TU.Type, TU.BloodType, TU.BPJSNumber, TU.PatientKey, TU.ModifiedBy, TU.ModifiedDate, TU.CreatedBy, TU.CreatedDate, TU.RowStatus
                            //}
                            if (a.UserName != "")
                            {
                                User TU = new User();
                                TU.OrganizationID = a.OrganizationID;
                                TU.UserName = a.UserName;
                                TU.Password = a.Password;
                                TU.EmployeeID = a.EmployeeID;
                                TU.ResetPasswordCode = a.ResetPasswordCode;
                                TU.Status = a.Status;
                                TU.ExpiredDate = a.ExpiredDate;
                                TU.RowStatus = a.RowStatus;
                                TU.ModifiedBy = a.ModifiedBy;
                                TU.ModifiedDate = a.ModifiedDate;
                                TU.CreatedBy = a.CreatedBy;
                                TU.CreatedDate = a.CreatedDate;

                                db.Users.Add(TU);

                                db.SaveChanges();



                            }
                            else
                            {
                                data.Add("<ul>");
                                //if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                                //if (a.Address == "" || a.Address == null) data.Add("<li> Address is required</li>");
                                //if (a.ContactNo == "" || a.ContactNo == null) data.Add("<li>ContactNo is required</li>");

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return RedirectToAction("UserList", "MasterData"); ;
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ::Organization::
        [HttpPost]
        public FileResult ExportOrganization()
        {
            KlinikDBEntities entities = new KlinikDBEntities();
            DataTable dt = new DataTable("Organization");
            dt.Columns.AddRange(new DataColumn[8] {
                                            new DataColumn("OrgCode"),
                                            new DataColumn("OrgName"),
                                            new DataColumn("KlinikID"),
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate"),
                                            new DataColumn("ModifiedBy"),
                                            new DataColumn("ModifiedDate"),
                                            new DataColumn("RowStatus")
            });

            var users = from user in entities.Organizations.Take(11)
                        select user;

            foreach (var k in users)
            {
                dt.Rows.Add(k.OrgCode, k.OrgName, k.KlinikID, k.CreatedBy, k.CreatedDate, k.ModifiedBy, k.ModifiedDate, k.RowStatus);
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
        [HttpPost]
        public ActionResult UploadOrganization(Organization users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/fileDoc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Organization$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "Organization";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<Organization>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            //if (a.Region != "")
                            //{
                            //    City TU = new Sale();
                            //    TU.AddedOn = a.AddedOn;
                            //    TU.Region = a.Region;
                            //    TU.Person = a.Person;
                            //    TU.Item = a.Item;
                            //    TU.Units = a.Units;
                            //    TU.UnitCost = a.UnitCost;
                            //    TU.Total = a.Total;
                            //    db.Sale.Add(TU);

                            //    db.SaveChanges();


                            //TU.MRNumber, TU.Name, TU.Gender, TU.MaritalStatus, TU.BirthDate, TU.KTPNumber, TU.Address, TU.CityID, TU.HPNumber, TU.Type, TU.BloodType, TU.BPJSNumber, TU.PatientKey, TU.ModifiedBy, TU.ModifiedDate, TU.CreatedBy, TU.CreatedDate, TU.RowStatus
                            //}
                            if (a.OrgName != "")
                            {
                                Organization TU = new Organization();
                                TU.OrgCode = a.OrgCode;
                                TU.OrgName = a.OrgName;
                                TU.KlinikID = a.KlinikID;
                                TU.RowStatus = a.RowStatus;
                                TU.ModifiedBy = a.ModifiedBy;
                                TU.ModifiedDate = a.ModifiedDate;
                                TU.CreatedBy = a.CreatedBy;
                                TU.CreatedDate = a.CreatedDate;

                                db.Organizations.Add(TU);

                                db.SaveChanges();



                            }
                            else
                            {
                                data.Add("<ul>");
                                //if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                                //if (a.Address == "" || a.Address == null) data.Add("<li> Address is required</li>");
                                //if (a.ContactNo == "" || a.ContactNo == null) data.Add("<li>ContactNo is required</li>");

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return RedirectToAction("OrganizationList", "MasterData"); ;
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}