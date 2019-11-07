using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Form;
using Klinik.Entities.Loket;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Persistence = Klinik.Data.DataRepository;
using LinqKit;
using Klinik.Entities.Pharmacy;

namespace Klinik.Features.Pharmacy
{
    public class PharmacyHandler : BaseFeatures
    {
        private const string ORAL = "oral";
        private const string CONCOCTION = "concoction";
        private const string PIECE = "piece";
        public PharmacyHandler(IUnitOfWork unitOfWork, KlinikDBEntities context = null)
        {
            _unitOfWork = unitOfWork;
            _context = context;

        }

        public PharmacyResponse CreateOrEdit(PharmacyRequest request)
        {
            long _cashierID = 0;
            long _farmasiID = 0;
            var _qry = _unitOfWork.PoliRepository.GetFirstOrDefault(x => x.Name == Constants.NameConstant.Kasir);
            if (_qry != null)
                _cashierID = _qry.ID;

            var _qry2 = _unitOfWork.PoliRepository.GetFirstOrDefault(x => x.Name == Constants.NameConstant.Farmasi);
            if (_qry2 != null)
                _farmasiID = _qry2.ID;
            PharmacyResponse response = new PharmacyResponse();

            var _orgID = _unitOfWork.OrganizationRepository.GetFirstOrDefault(x => x.OrgCode == request.Account.Organization) == null ? 0 : _unitOfWork.OrganizationRepository.GetFirstOrDefault(x => x.OrgCode == request.Account.Organization).ID;
            var _gudangID = _unitOfWork.GudangRepository.GetFirstOrDefault(x => x.OrganizationId == _orgID) == null ? 0 : _unitOfWork.GudangRepository.GetFirstOrDefault(x => x.OrganizationId == _orgID).id;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var queue = _context.QueuePolis.FirstOrDefault(x => x.FormMedicalID == request.Data.FormMedicalID
                                && x.PoliTo == _farmasiID
                                && x.RowStatus == 0);

                    // set as waiting
                    queue.Status = 1;
                    _context.SaveChanges();

                    #region :: INSERT To PRODUCT::
                    // unit & category ini sementara hardcode dulu, nantinya bakal dipilih dari screen farmasi
                    var _prdCatIdOral = _unitOfWork.ProductCategoryRepository.Get(x => x.Name.ToLower() == ORAL && x.RowStatus == 0).FirstOrDefault() == null ? 0 : _unitOfWork.ProductCategoryRepository.Get(x => x.Name.ToLower() == ORAL && x.RowStatus == 0).FirstOrDefault().ID;
                    var _prdCatIdRacikan = _unitOfWork.ProductCategoryRepository.Get(x => x.Name.ToLower() == CONCOCTION && x.RowStatus == 0).FirstOrDefault() == null ? 0 : _unitOfWork.ProductCategoryRepository.Get(x => x.Name.ToLower() == CONCOCTION && x.RowStatus == 0).FirstOrDefault().ID;

                    var _prdUnitId = _unitOfWork.ProductUnitRepository.Get(x => x.Name.ToLower() == PIECE).FirstOrDefault() == null ? 0 : _unitOfWork.ProductUnitRepository.Get(x => x.Name.ToLower() == PIECE).FirstOrDefault().ID;

                    foreach (var tobeInsert in request.Data.Medicines)
                    {

                        if (!String.IsNullOrEmpty(tobeInsert.Detail.ProcessType))
                        {
                            var entity = new Product
                            {

                                ClinicID = request.Account.ClinicID,
                                Name = tobeInsert.Detail.ProductName,
                                ProductCategoryID = tobeInsert.Detail.ProcessType.ToLower() == MedicineTypeEnum.REQUEST.ToString().ToLower() ? _prdCatIdOral : _prdCatIdRacikan,
                                ProductUnitID = _prdUnitId,
                                RetailPrice = CountRetailPriceForRacikan(request.Data.ObatRacikanKomponens, tobeInsert.Id) / Convert.ToDecimal(tobeInsert.Qty ?? 1),
                                RowStatus = 0,
                                CreatedBy = request.Account.UserName,
                                CreatedDate = DateTime.Now

                            };

                            _context.Products.Add(entity);
                            _context.SaveChanges();
                            tobeInsert.Detail.ProductID = entity.ID;
                        }

                    }
                    #endregion

                    #region :: FORM EXAMINE DETAIL::
                    var _getFrmExId = _unitOfWork.FormExamineRepository.Get(x => x.FormMedicalID == request.Data.FormMedicalID).FirstOrDefault().ID;
                    var idFormExamineMedicine = _unitOfWork.FormExamineMedicineRepository.Get(x => x.FormExamineID == _getFrmExId).Select(x => x.ID).ToList();
                    var existingMedDetail = _unitOfWork.FormExamineMedicineDetailRepository.Get(x => idFormExamineMedicine.Contains(x.FormExamineMedicineID ?? 0));
                    foreach (var tobedelete in existingMedDetail)
                    {
                        _context.FormExamineMedicineDetails.Remove(tobedelete);
                        _context.SaveChanges();
                    }

                    foreach (var tobeInsert in request.Data.Medicines)
                    {
                        var _idExMed = tobeInsert.Id;
                        var entity = Mapper.Map<FormExamineMedicineDetailModel, FormExamineMedicineDetail>(tobeInsert.Detail);
                        entity.FormExamineMedicineID = _idExMed;

                        entity.CreatedDate = DateTime.Now;
                        entity.CreatedBy = request.Account.UserName;
                        _context.FormExamineMedicineDetails.Add(entity);
                        _context.SaveChanges();
                        tobeInsert.Detail.Id = entity.ID;
                    }
                    #endregion

                    #region ::INSERT To Product In Gudang::
                    foreach (var tobeInsert in request.Data.Medicines)
                    {
                        var entity = new Klinik.Data.DataRepository.ProductInGudang();
                        if (!String.IsNullOrEmpty(tobeInsert.Detail.ProcessType))
                        {
                            if (tobeInsert.Detail.ProcessType.ToLower() == "racik")
                            {

                                entity = new Klinik.Data.DataRepository.ProductInGudang
                                {
                                    GudangId = _gudangID,
                                    stock = Convert.ToInt32(Math.Round(tobeInsert.Detail.Qty ?? 0)),
                                    ProductId = (Int32)tobeInsert.Detail.Id,
                                    CreatedBy = request.Account.UserName,
                                    CreatedDate = DateTime.Now,
                                    RetailPrice = CountRetailPriceForRacikan(request.Data.ObatRacikanKomponens, tobeInsert.Id) / Convert.ToDecimal(tobeInsert.Qty ?? 1),
                                    RowStatus = 0,
                                };


                            }

                            if (tobeInsert.Detail.ProcessType.ToLower() == "request")
                            {

                                entity = new Klinik.Data.DataRepository.ProductInGudang
                                {
                                    stock = 0,
                                    GudangId = _gudangID,
                                    ProductId = (Int32)tobeInsert.Detail.Id,
                                    CreatedBy = request.Account.UserName,
                                    CreatedDate = DateTime.Now,
                                    RowStatus = 0
                                };


                            }
                            _context.ProductInGudangs.Add(entity);
                            _context.SaveChanges();
                        }


                        else
                        {

                            //just update stock
                            var tobeUpdate = _unitOfWork.ProductInGudangRepository.GetFirstOrDefault(x => x.ProductId == tobeInsert.Detail.ProductID && x.GudangId == _gudangID);
                            if (tobeUpdate != null)
                            {
                                tobeUpdate.stock = tobeUpdate.stock - Convert.ToInt32(Math.Round(tobeInsert.Detail.Qty ?? 0));
                                _context.SaveChanges();
                            }
                        }



                    }

                    #endregion

                    #region ::INSERT INTO PRODUCT IN GUDANG FOR KOMPONEN::
                    List<KomponenObatRacikan> CollOfRacikanKomponen = new List<KomponenObatRacikan>();
                    if (request.Data.ObatRacikanKomponens != null)
                    {
                        CollOfRacikanKomponen = JsonConvert.DeserializeObject<List<KomponenObatRacikan>>(request.Data.ObatRacikanKomponens);
                        foreach (var itemDet in CollOfRacikanKomponen)
                        {
                            int id_komponen = Convert.ToInt32(itemDet.Id);
                            var tobeUpdate2 = _unitOfWork.ProductInGudangRepository.GetFirstOrDefault(x => x.ProductId == id_komponen && x.GudangId == _gudangID);
                            if (tobeUpdate2 != null)
                            {
                                tobeUpdate2.stock = tobeUpdate2.stock - Convert.ToInt32(Math.Round(Convert.ToDecimal(itemDet.Amount ?? "0")));
                                _context.SaveChanges();
                            }
                        }
                    }

                    #endregion

                    #region ::INSERT TO PR for new Medicine::
                    var _nullProcess = request.Data.Medicines.Where(x => x.Detail.ProcessType == null);
                    var newReqMed = request.Data.Medicines.Except(_nullProcess).Where(x => x.Detail.ProcessType.ToLower() == "request");

                    foreach (var itemPrhdr in newReqMed)
                    {
                        var entityPrHdr = new Persistence.PurchaseRequest
                        {
                            FormMedicalID = request.Data.FormMedicalID,
                            prnumber = $"PR-{DateTime.Now.ToString("yyyyMMddhhmmss")}",//will be changed with autogenerate PR No
                            prdate = DateTime.Now,
                            RowStatus = 0,
                            CreatedBy = request.Account.UserName,
                            CreatedDate = DateTime.Now
                        };
                        _context.PurchaseRequests.Add(entityPrHdr);
                        _context.SaveChanges();

                        var IdPrHdr = entityPrHdr.id;

                        //insert to PR detail
                        var entPRDetail = new PurchaseRequestDetail
                        {
                            PurchaseRequestId = IdPrHdr,
                            ProductId = itemPrhdr.Detail.ProductID ?? 0,
                            namabarang = itemPrhdr.Detail.ProductName,
                            qty = itemPrhdr.Detail.Qty,
                            qty_add = itemPrhdr.Detail.Qty,
                            reason_add = "REQUEST FROM PHARMACY",
                            CreatedBy = request.Account.UserName,
                            CreatedDate = DateTime.Now
                        };

                        _context.PurchaseRequestDetails.Add(entPRDetail);
                        _context.SaveChanges();
                    }

                    #endregion

                    #region ::CEK IN QUEUE POLI WHETHER PATIENT already go to cashier


                    if (_cashierID > 0)
                    {
                        var _qryHistoryPoli = _unitOfWork.RegistrationRepository.Get(x => x.PoliTo == _cashierID && x.FormMedicalID == request.Data.FormMedicalID);
                        if (_qryHistoryPoli.Count <= 0)
                        {

                            // create a new registration
                            QueuePoli queuePoli = new QueuePoli();
                            queuePoli.FormMedicalID = request.Data.FormMedicalID;
                            queuePoli.PoliFrom = (Int32)_farmasiID;
                            queuePoli.PoliTo = (Int32)_cashierID;
                            queuePoli.CreatedBy = request.Account.UserCode;
                            queuePoli.CreatedDate = DateTime.Now;
                            queuePoli.TransactionDate = DateTime.Now;
                            queuePoli.Status = (int)RegistrationStatusEnum.New;
                            queuePoli.SortNumber = GenerateSortNumber((int)_cashierID, 0);
                            queuePoli.ClinicID = request.Account.ClinicID;
                            var _patientID = _unitOfWork.RegistrationRepository.GetFirstOrDefault(x => x.FormMedicalID == request.Data.FormMedicalID);
                            queuePoli.PatientID = _patientID == null ? 0 : _patientID.PatientID;
                            _context.QueuePolis.Add(queuePoli);
                            _context.SaveChanges();
                        }
                    }
                    #endregion

                    transaction.Commit();
                    response.Status = true;
                    response.Message = Messages.PrescriptionProcessSuccess;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    response.Status = false;
                    response.Message = Messages.GeneralError;

                    ErrorLog(ClinicEnums.Module.PHARMACY, Constants.Command.EDIT_FORM_EXAMINE_MEDICINE, request.Account, ex);
                }
            }

            return response;
        }

        private decimal CountRetailPriceForRacikan(string jsonRacikan, long headerID)
        {
            decimal decResult = 0;
            if (!string.IsNullOrEmpty(jsonRacikan))
            {
                List<KomponenObatRacikan> components = new List<KomponenObatRacikan>();
                components = JsonConvert.DeserializeObject<List<KomponenObatRacikan>>(jsonRacikan);


                var _q1 = components.Where(x => x.Id_ObatRacik == headerID.ToString());
                foreach (var item in _q1)
                {
                    decResult += (Convert.ToDecimal(item.Price) * Convert.ToDecimal(item.Amount));
                }
            }

            return decResult;
        }
        public static List<Int32> GetSelectedPharmacyItem(long IdQueue)
        {
            List<Int32> PharmacyItemIds = new List<Int32>();
            var _getFormMedical = _unitOfWork.RegistrationRepository.GetById(IdQueue);
            if (_getFormMedical != null)
            {
                var qryLabItems = _unitOfWork.FormExamineMedicineRepository.Get(x => x.FormExamine.FormMedicalID == _getFormMedical.FormMedicalID);
                foreach (var item in qryLabItems)
                {
                    PharmacyItemIds.Add((int)item.ID);
                }
            }
            return PharmacyItemIds;
        }

        public LoketResponse GetListData(LoketRequest request)
        {
            Expression<Func<QueuePoli, bool>> _serachCriteria = x => x.PoliTo == request.Data.PoliToID;

            List<LoketModel> lists = base.GetFarmasiBaseLoketData(request, _serachCriteria);
            int totalRequest = lists.Count();
            var response = new LoketResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = lists
            };

            return response;
        }

        public PharmacyResponse GetListPengambilanObat(PharmacyRequest request)
        {
            List<PrescriptionModel> lists = new List<PrescriptionModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<FormExamineMedicineDetail>(true);
            searchPredicate = searchPredicate.And(x => x.Status == null);
            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "formmedical":
                            qry = _unitOfWork.FormExamineMedicineDetailRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.FormExamineMedicine.FormExamine.FormMedicalID));
                            break;

                        default:
                            qry = _unitOfWork.FormExamineMedicineDetailRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "formmedical":
                            qry = _unitOfWork.FormExamineMedicineDetailRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.FormExamineMedicine.FormExamine.FormMedicalID));
                            break;

                        default:
                            qry = _unitOfWork.FormExamineMedicineDetailRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.FormExamineMedicineDetailRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var _ready = Mapper.Map<FormExamineMedicineDetail, PrescriptionModel>(item);
                lists.Add(_ready);
            }

            lists = lists.GroupBy(x => x.FormMedicalID).Select(g => g.First()).ToList();
            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new PharmacyResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        public List<long> GetMedicineWasReceivedByPatient(long frmMedId)
        {
            var _get_frmExId = _unitOfWork.FormExamineRepository.Get(x => x.FormMedicalID == frmMedId).Select(x => x.ID).ToList();
            var _hdr = _unitOfWork.FormExamineMedicineRepository.Get(x => _get_frmExId.Contains(x.FormExamineID ?? 0));
            List<long> FrmExMedHdrIds = _hdr.Select(x => x.ID).ToList();
            var selectedIds = _unitOfWork.FormExamineMedicineDetailRepository.Get(x => FrmExMedHdrIds.Contains(x.FormExamineMedicineID ?? 0) && x.Status != null).Select(x => x.ID).ToList();
            return selectedIds;
        }

        public ListObatResponse ListAllObat(PharmacyRequest request)
        {
            List<FormExamineMedicineDetailModel> details = new List<FormExamineMedicineDetailModel>();

            var _get_frmExId = _unitOfWork.FormExamineRepository.Get(x => x.FormMedicalID == request.Data.FormMedicalID).Select(x => x.ID).ToList();
            var _hdr = _unitOfWork.FormExamineMedicineRepository.Get(x => _get_frmExId.Contains(x.FormExamineID ?? 0));
            List<long> FrmExMedHdrIds = new List<long>();
            FrmExMedHdrIds = _hdr.Select(x => x.ID).ToList();
            var _detail = _unitOfWork.FormExamineMedicineDetailRepository.Get(x => FrmExMedHdrIds.Contains(x.FormExamineMedicineID ?? 0) && x.Status == null);
            foreach (var item in _detail)
            {
                var temp = new FormExamineMedicineDetailModel
                {
                    Id = item.ID,
                    ProductName = item.ProductName,
                    Qty = item.Qty,
                    Dosis = item.FormExamineMedicine.Dose,
                    RemarksUse = item.FormExamineMedicine.RemarkUse
                };
                details.Add(temp);
            }

            int totalRequest = details.Count();
            var data = details.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new ListObatResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = details,

            };

            return response;
        }

        public PharmacyResponse UpdateStatusObat(PharmacyRequest request)
        {
            var response = new PharmacyResponse();
            int result_affected = 0;
            List<string> additionalMesg = new List<string>();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (long _id in request.idSelectedobat)
                    {
                        var _existing = _context.FormExamineMedicineDetails.SingleOrDefault(x => x.ID == _id);
                        if (_existing != null)
                        {
                            //cek apakah stok nya >= dr request
                            var _getCurrStock = _unitOfWork.ProductInGudangRepository.GetFirstOrDefault(x => x.ProductId == _id);
                            var _getQtyNeed = _existing.Qty;

                            var _currStock = _getCurrStock == null ? 0 : _getCurrStock.stock;
                            if (_currStock < _getQtyNeed && _existing.ProcessType == "Request")
                            {
                                //ga bisa ambil obat
                                additionalMesg.Add(_existing.ProductName);
                            }
                            else //if (_currStock >= _getQtyNeed)
                            {
                                //bisa ambil, kurangi stoknya utk yg request
                                if (_existing.ProcessType == "Request")
                                {
                                    //ga bisa ambil obat
                                    var exist = _unitOfWork.ProductInGudangRepository.GetFirstOrDefault(x => x.ProductId == _id);
                                    if (exist != null)
                                    {
                                        exist.stock = Convert.ToInt32(Convert.ToDouble(exist.stock) - _existing.Qty);
                                        exist.ModifiedDate = DateTime.Now;
                                        exist.ModifiedBy = request.Account.UserName;
                                    }
                                }
                                _existing.Status = StatusAmbilObat.R.ToString();
                                _existing.ModifiedBy = request.Account.UserName;
                                _existing.ModifiedDate = DateTime.Now;
                                _existing.TanggalAmbilObat = DateTime.Now;

                                result_affected += _context.SaveChanges();
                            }

                        }

                    }
                    transaction.Commit();
                    if (result_affected > 0)
                    {
                        response.Status = true;
                        response.Message = Messages.DataHasBeenUpdated;
                    }

                    response.AdditionalMessages = additionalMesg;

                }
                catch (Exception)
                {
                    transaction.Rollback();
                    response.Status = false;
                    response.Message = Messages.GeneralError.ToString();
                }
            }

            return response;
        }



    }
}
