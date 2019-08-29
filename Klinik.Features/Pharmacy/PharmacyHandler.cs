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
using Persistence= Klinik.Data.DataRepository;
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
            PharmacyResponse response = new PharmacyResponse();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var queue = _context.QueuePolis.FirstOrDefault(x => x.FormMedicalID == request.Data.FormMedicalID
                                && x.PoliTo == (int)PoliEnum.Farmasi
                                && x.RowStatus == 0);

                    // set as waiting
                    queue.Status = 1;
                    _context.SaveChanges();

                   

                    #region :: INSERT To PRODUCT::
                    // unit & category ini sementara hardcode dulu, nantinya bakal dipilih dari screen farmasi
                    var _prdCatIdOral = _unitOfWork.ProductCategoryRepository.Get(x => x.Name.ToLower() == ORAL).FirstOrDefault() == null ? 0 : _unitOfWork.ProductCategoryRepository.Get(x => x.Name.ToLower() == ORAL).FirstOrDefault().ID;
                    var _prdCatIdRacikan = _unitOfWork.ProductCategoryRepository.Get(x => x.Name.ToLower() == CONCOCTION).FirstOrDefault() == null ? 0 : _unitOfWork.ProductCategoryRepository.Get(x => x.Name.ToLower() == CONCOCTION).FirstOrDefault().ID;

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
                                RetailPrice = 0,
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
                                    stock = Convert.ToInt32(Math.Round(tobeInsert.Detail.Qty ?? 0)),
                                    ProductId = (Int32)tobeInsert.Detail.Id,
                                    CreatedBy = request.Account.UserName,
                                    CreatedDate = DateTime.Now,
                                    RowStatus = 0
                                };


                            }

                            if (tobeInsert.Detail.ProcessType.ToLower() == "request")
                            {

                                entity = new Klinik.Data.DataRepository.ProductInGudang
                                {
                                    stock = 0,
                                    ProductId = (Int32)tobeInsert.Detail.Id,
                                    CreatedBy = request.Account.UserName,
                                    CreatedDate = DateTime.Now,
                                    RowStatus = 0
                                };


                            }
                        }


                        else
                        {
                            entity = new Klinik.Data.DataRepository.ProductInGudang
                            {
                                stock = -Convert.ToInt32(Math.Round(tobeInsert.Detail.Qty ?? 0)),
                                ProductId = tobeInsert.Detail.ProductID,
                                CreatedBy = request.Account.UserName,
                                CreatedDate = DateTime.Now,
                                RowStatus = 0
                            };

                        }

                        _context.ProductInGudangs.Add(entity);
                        _context.SaveChanges();

                    }

                    #endregion

                    #region ::INSERT INTO PRODUCT IN GUDANG FOR KOMPONEN::
                    List<KomponenObatRacikan> CollOfRacikanKomponen = new List<KomponenObatRacikan>();
                    CollOfRacikanKomponen = JsonConvert.DeserializeObject<List<KomponenObatRacikan>>(request.Data.ObatRacikanKomponens);
                    foreach (var itemDet in CollOfRacikanKomponen)
                    {
                        var _convAmt = Convert.ToDecimal(itemDet.Amount.Replace('.', ','));
                        var entDetil = new Klinik.Data.DataRepository.ProductInGudang
                        {
                            stock=-Convert.ToInt32(Math.Round( _convAmt)),
                            ProductId=itemDet.Id==null?0:Convert.ToInt32(itemDet.Id),
                            CreatedBy=request.Account.UserName,
                            CreatedDate=DateTime.Now,
                            RowStatus=0
                        };

                        _context.ProductInGudangs.Add(entDetil);
                        _context.SaveChanges();
                    }
                    #endregion



                    #region ::INSERT TO PR for new Medicine::
                    var _nullProcess= request.Data.Medicines.Where(x => x.Detail.ProcessType == null);
                    var newReqMed = request.Data.Medicines.Except(_nullProcess).Where(x=>x.Detail.ProcessType.ToLower()=="request");
                   
                    foreach (var itemPrhdr in newReqMed)
                    {
                        var entityPrHdr = new Persistence.PurchaseRequest
                        {
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
                            CreatedBy=request.Account.UserName,
                            CreatedDate=DateTime.Now
                        };

                        _context.PurchaseRequestDetails.Add(entPRDetail);
                        _context.SaveChanges();
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
    }
}
