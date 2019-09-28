using AutoMapper;
using Klinik.Common;
using Klinik.Common.Enumerations;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Letter;
using Klinik.Entities.Loket;
using Klinik.Entities.MasterData;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.SuratReferensi.SuratLabReferensi
{
    public class RujukanLabHandler : BaseFeatures
    {
        public RujukanLabHandler(IUnitOfWork unitOfWork, KlinikDBEntities context = null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }
        public List<LoketModel> GetFormMedicalIds(int patientId)
        {
            List<LoketModel> lokets = new List<LoketModel>();
            var qry = _unitOfWork.RegistrationRepository.Get(x => x.PatientID == patientId && x.FormMedicalID > 0)
                .Select(x => new
                {
                    x.TransactionDate,
                    x.FormMedicalID
                }).Distinct();

            foreach (var item in qry)
            {
                lokets.Add(new LoketModel
                {
                    FormMedicalID = item.FormMedicalID ?? 0,
                    TransactionDateStr = item.TransactionDate.ToString("yyyy-MM-dd")
                });
            }

            return lokets;

        }
        public RujukanLabResponse SaveSuratRujukanLab(RujukanLabRequest request)
        {
            int _resultAffected = 0;
            request.Data.LetterType = LetterEnum.LabReferenceLetter.ToString();
            request.Data.AutoNumber = GetLatestAutoNoSurat(LetterEnum.LabReferenceLetter.ToString(), request.Data.Account.ClinicID) + 1;
            request.Data.Year = DateTime.Now.Year;

            var _dob = _unitOfWork.PatientRepository.GetById(request.Data.ForPatient);
            if (_dob != null)
            {
                request.Data.PatientAge = CommonUtils.GetPatientAge(_dob.BirthDate);
            }

            var response = new RujukanLabResponse { };

            try
            {
                var cekExist = _unitOfWork.LetterRepository.Get(x => x.FormMedicalID == request.Data.FormMedicalID && x.LetterType==request.Data.LetterType);
                if (!cekExist.Any())
                {
                    var _entity = Mapper.Map<LabReferenceLetterModel, Letter>(request.Data);
                    _unitOfWork.LetterRepository.Insert(_entity);
                    _resultAffected = _unitOfWork.Save();
                }

                //get detail patient
                var _pasien = _unitOfWork.PatientRepository.GetById(request.Data.ForPatient);
                response.Patient = Mapper.Map<Patient, PatientModel>(_pasien);
                if (response.Entity == null)
                    response.Entity = new LabReferenceLetterModel();
                response.Entity.PatientAge = request.Data.PatientAge;
                response.Entity.strCekdate = request.Data.Cekdate == null ? "" : ((DateTime)request.Data.Cekdate).ToString("MM-dd-yyyy");
                response.Entity.FormMedicalID = request.Data.FormMedicalID;
                response.Status = true;

            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message.ToString();
            }
            return response;
        }

        public List<LabItemModel> GetPreviousSelectedLabItem(long FormMedicalID)
        {

            List<int> _selLabItemIds = new List<int>();
            var qry = _unitOfWork.FormExamineLabRepository.Get(x => x.FormMedicalID == FormMedicalID).Select(p => p.LabItemID);
            foreach (int iLabItemId in qry)
            {
                _selLabItemIds.Add(iLabItemId);
            }

            List<LabItemModel> lists = new List<LabItemModel>();

            var q = _unitOfWork.LabItemRepository.Get(x => _selLabItemIds.Contains(x.ID));
            foreach (var item in q)
            {
                var mapping = Mapper.Map<LabItem, LabItemModel>(item);
                lists.Add(mapping);
            }

            return lists;
        }

        public RujukanLabResponse SaveAndPreview(RujukanLabRequest request)
        {
            var response = new RujukanLabResponse();
            int result = 0;
            try
            {
                var _existingIds = _unitOfWork.SuratRujukanLabKeluarRepository.Get(x => x.FormMedicalID == request.Data.SuratRujukanLabKeluar.FormMedicalID).Select(x => x.Id);
                foreach (var _id in _existingIds)
                {
                    _unitOfWork.SuratRujukanLabKeluarRepository.Delete(_id);
                }

                var delResult = _unitOfWork.Save();

                foreach (int labId in request.Data.SuratRujukanLabKeluar.ListOfLabItemId)
                {
                    var entity = new SuratRujukanLabKeluar
                    {
                        FormMedicalID = request.Data.SuratRujukanLabKeluar.FormMedicalID,
                        NoSurat = GetNoSurat(request.Data.SuratRujukanLabKeluar.FormMedicalID),
                        DokterPengirim = request.Data.SuratRujukanLabKeluar.DokterPengirim,
                        LabItemId = labId,
                        CreatedDate = DateTime.Now,
                        CreatedBy = request.Data.Account.UserName
                    };

                    _unitOfWork.SuratRujukanLabKeluarRepository.Insert(entity);
                }
                result = _unitOfWork.Save();
                if (result > 0)
                {
                    if (response.Entity == null)
                        response.Entity = new LabReferenceLetterModel();
                    response.Status = true;
                    response.Entity.FormMedicalID = request.Data.SuratRujukanLabKeluar.FormMedicalID;
                }
            }
            catch (Exception )
            {
                response.Status = false;
                response.Message = Messages.GeneralError;
            }

            return response;
        }

        private string GetNoSurat(long FrmMedId)
        {
            var noSurat = string.Empty;
            var LetterNo = _unitOfWork.LetterRepository
                .Get(x => x.LetterType == LetterEnum.LabReferenceLetter.ToString() && x.FormMedicalID == FrmMedId)
                .FirstOrDefault();
            if (LetterNo != null)
            {
                noSurat = $"{ LetterNo.AutoNumber}/klinik/{DateTime.Now.Year}/{DateTime.Now.Month}";
            }

            return noSurat;
        }

        public RujukanLabResponse GetDetailSuratRujukanLab(RujukanLabRequest request)
        {
            var response = new RujukanLabResponse { };
            response.Entity = new LabReferenceLetterModel();
            response.Entity.SuratRujukanLabKeluar = new SuratRujukanKeluarModel();
            if (request.Data.Account != null)
            {

                var _detail = _unitOfWork.LetterRepository.Get(x => x.FormMedicalID == request.Data.FormMedicalID).FirstOrDefault();
                var _miscData = _unitOfWork.SuratRujukanLabKeluarRepository.GetFirstOrDefault(x => x.FormMedicalID == request.Data.FormMedicalID);
                var suratrujukanlabkeluar = new SuratRujukanKeluarModel
                {
                    DokterPengirim = _miscData.DokterPengirim,
                    NoSurat = _miscData.NoSurat,

                };

                var _detailPatient = _unitOfWork.PatientRepository.GetById(_detail.ForPatient);
                var _labSelected = _context.SuratRujukanLabKeluars.Where(x => x.FormMedicalID == request.Data.FormMedicalID).Select(x => new
                {
                    LabId = x.LabItemId,
                    LabItemName = x.LabItem.Name,
                    LabItemCategoryID = x.LabItem.LabItemCategoryID,
                    Category = x.LabItem.LabItemCategory.Name
                });
                List<int> selectedLabId = _labSelected.Select(x => x.LabId).ToList();
                var _notSelectedLab = _unitOfWork.FormExamineLabRepository
                    .Get(x => x.FormMedicalID == request.Data.FormMedicalID && !selectedLabId.Contains((int)x.LabItemID))
                    .Select(x => new
                    {
                        LabId = x.LabItemID ?? 0,
                        LabItemName = x.LabItem.Name,
                        LabItemCategoryID = x.LabItem.LabItemCategoryID,
                        Category = x.LabItem.LabItemCategory.Name
                    });

                response.Entity.PatientAge = _detail.PatientAge;
                response.Entity.strCekdate = _detail.Cekdate == null ? "" : ((DateTime)_detail.Cekdate).ToString("dd/MM/yyyy");
                response.Patient = Mapper.Map<Patient, PatientModel>(_detailPatient);
                if (response.ListLabs == null)
                    response.ListLabs = new List<LabItemModel>();
                foreach (var item in _labSelected)
                {
                    response.ListLabs.Add(new LabItemModel
                    {
                        Id = item.LabId,
                        Name = item.LabItemName,
                        LabItemCategoryName = item.Category,
                        LabItemCategoryID = item.LabItemCategoryID,
                        Code = "v"
                    });
                }

                foreach (var item2 in _notSelectedLab)
                {
                    response.ListLabs.Add(new LabItemModel
                    {
                        Id = item2.LabId,
                        Name = item2.LabItemName,
                        LabItemCategoryName = item2.Category,
                        LabItemCategoryID = item2.LabItemCategoryID,
                        Code = ""
                    });
                }

                response.Entity.SuratRujukanLabKeluar = suratrujukanlabkeluar;
            }
            else
            {
                response.Status = false;
                response.Message = Resources.Messages.UnauthorizedAccess;
            }

            return response;
        }
    }
}
