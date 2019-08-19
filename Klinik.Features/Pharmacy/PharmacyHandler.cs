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

namespace Klinik.Features.Pharmacy
{
    public class PharmacyHandler : BaseFeatures
    {
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

                    foreach (var item in request.Data.Medicines)
                    {
						var existEntity = _context.FormExamineMedicineDetails.FirstOrDefault(x => x.FormExamineMedicineID == item.Id);
						if (existEntity == null)
						{
							FormExamineMedicineDetail detail = Mapper.Map<FormExamineMedicineDetailModel, FormExamineMedicineDetail>(item.Detail);
							detail.CreatedBy = request.Account.UserCode;
							detail.CreatedDate = DateTime.Now;
							detail.FormExamineMedicineID = item.Id;

							_context.FormExamineMedicineDetails.Add(detail);
							_context.SaveChanges();
						}
						else
						{
							existEntity.ProductID = item.Detail.ProductID;
							existEntity.ProductName = item.Detail.ProductName;
							existEntity.Qty = item.Detail.Qty;
							existEntity.Note = item.Detail.Note;
							existEntity.ProcessType = item.Detail.ProcessType;
							existEntity.ModifiedBy = request.Account.UserCode;
							existEntity.ModifiedDate = DateTime.Now;
							_context.SaveChanges();
						}
                    }
                    
                    transaction.Commit();
                    response.Message = Messages.PrescriptionValidationSuccess;
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
