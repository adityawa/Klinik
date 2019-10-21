using AutoMapper;
using Klinik.Common;
using Klinik.Common.Enumerations;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Form;
using Klinik.Entities.Letter;
using Klinik.Entities.RealisasiSuratRujukanEntities;
using Klinik.Features.HospitalFeatures;
using Klinik.Resources;
using LinqKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.RealisasiSuratRujukan
{
    public class RealisasiSuratRujukanHandler : BaseFeatures
    {
        public RealisasiSuratRujukanHandler(IUnitOfWork unitOfWork, KlinikDBEntities context=null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public RealisasiSuratRujukanResponse GetSuratRujukan(RealisasiSuratRujukanRequest request)
        {
            
            List<RealisasiSuratRujukanModel> letters = new List<RealisasiSuratRujukanModel>();
           
            IQueryable<Letter> temp = _context.Letters.Where(x => x.LetterType == LetterEnum.MedicalReferenceLetter.ToString() && x.Action == null);

            foreach (var item in temp)
            {
                letters.Add(new RealisasiSuratRujukanModel
                {
                    Id=item.Id,
                    NoSurat = item.NoSurat,
                    PatientID=item.ForPatient??0,
                    PatientName=item.ForPatient==null?"":_unitOfWork.PatientRepository.GetById(item.ForPatient).Name,
                    RSRujukan= new HospitalHandler(_unitOfWork).GetHospitalName( JsonConvert.DeserializeObject<InfoRujukan>(item.OtherInfo).RSRujukan),
                    DoctorName = JsonConvert.DeserializeObject<InfoRujukan>(item.OtherInfo).NamaDokter,
                    FormMedicalID=item.FormMedicalID??0
                });
            }

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                letters = letters.Where(x => x.NoSurat.Contains(request.SearchValue) || x.RSRujukan.Contains(request.SearchValue) || x.PatientName.Contains(request.SearchValue)).ToList();
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "rsrujukan":
                            letters = letters.OrderBy(x => x.RSRujukan).ToList();
                            break;
                        

                        default:
                            letters = letters.OrderBy(x => x.NoSurat).ToList();
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "rsrujukan":
                            letters = letters.OrderByDescending(x => x.RSRujukan).ToList();
                            break;


                        default:
                            letters = letters.OrderByDescending(x => x.NoSurat).ToList();
                            break;
                    }
                }
            }

            int totalRequest = letters.Count();
            var data = letters.Skip(request.Skip).Take(request.PageSize).ToList();


            var response = new RealisasiSuratRujukanResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };
            return response;
        }

        public FormExamineResponse CreateOrEdit(FormExamineRequest request)
        {
            FormExamineResponse response = new FormExamineResponse();

            if (request.Data.Id > 0)
            {
                try
                {
                    var qry = _unitOfWork.FormExamineRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<FormExamine, FormExamineModel>(qry);

                        // update data

                        _unitOfWork.FormExamineRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated2, "PoliFormExamine", qry.ID);

                            CommandLog(true, ClinicEnums.Module.FORM_EXAMINE, Constants.Command.EDIT_FORM_EXAMINE, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "PoliFormExamine");

                            CommandLog(false, ClinicEnums.Module.FORM_EXAMINE, Constants.Command.EDIT_FORM_EXAMINE, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "PoliFormExamine");

                        CommandLog(false, ClinicEnums.Module.FORM_EXAMINE, Constants.Command.EDIT_FORM_EXAMINE, request.Data.Account, request.Data);
                    }
                }
                catch (Exception ex)
                {
                    response.Status = false;
                    response.Message = Messages.GeneralError;

                    if (request.Data != null && request.Data.Id > 0)
                        ErrorLog(ClinicEnums.Module.FORM_EXAMINE, Constants.Command.EDIT_FORM_EXAMINE, request.Data.Account, ex);
                    else
                        ErrorLog(ClinicEnums.Module.FORM_EXAMINE, Constants.Command.ADD_FORM_EXAMINE, request.Data.Account, ex);
                }
            }
            else
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        List<FormExamineMedicine> formExamineMedicine = new List<FormExamineMedicine>();
                        foreach (var item in request.Data.MedicineDataList)
                        {
                            var medicine = Mapper.Map<FormExamineMedicineModel, FormExamineMedicine>(item);
                            medicine.CreatedBy = request.Data.Account.UserCode;
                            medicine.CreatedDate = DateTime.Now;

                            formExamineMedicine.Add(medicine);
                        }

                        foreach (var item in request.Data.LabDataList)
                        {
                            var lab = Mapper.Map<FormExamineLabModel, FormExamineLab>(item);
                            lab.FormMedicalID = request.Data.LoketData.FormMedicalID;
                            lab.CreatedBy = request.Data.Account.UserCode;
                            lab.CreatedDate = DateTime.Now;

                            _context.FormExamineLabs.Add(lab);
                            _context.SaveChanges();
                        }

                        List<FormExamineService> formExamineService = new List<FormExamineService>();
                        foreach (var item in request.Data.ServiceDataList)
                        {
                            var service = Mapper.Map<FormExamineServiceModel, FormExamineService>(item);
                            service.CreatedBy = request.Data.Account.UserCode;
                            service.CreatedDate = DateTime.Now;

                            formExamineService.Add(service);
                        }

                       

                        FormExamine formExamine = Mapper.Map<FormExamineModel, FormExamine>(request.Data.ExamineData);
                        formExamine.CreatedBy = request.Data.Account.UserCode;
                        formExamine.CreatedDate = DateTime.Now;
                        formExamine.FormExamineMedicines = formExamineMedicine;
                        formExamine.FormExamineServices = formExamineService;

                        // save the form examine data
                        _context.FormExamines.Add(formExamine);
                        _context.SaveChanges();

                        var isExist = _unitOfWork.LetterRepository.GetFirstOrDefault(x => x.NoSurat == request.Data.NoSurat && x.LetterType == LetterEnum.MedicalReferenceLetter.ToString());
                        if (isExist != null)
                        {
                            isExist.Action = "CLAIMED";
                            _context.SaveChanges();
                        }
                        transaction.Commit();

                        response.Message = Messages.DataSaved;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        response.Status = false;
                        response.Message = Messages.GeneralError;

                        if (request.Data != null && request.Data.Id > 0)
                            ErrorLog(ClinicEnums.Module.FORM_EXAMINE, Constants.Command.EDIT_FORM_EXAMINE, request.Data.Account, ex);
                        else
                            ErrorLog(ClinicEnums.Module.FORM_EXAMINE, Constants.Command.ADD_FORM_EXAMINE, request.Data.Account, ex);
                    }
                }
            }

            return response;
        }

        /// <summary>
        /// Generate the sort number
        /// </summary>
        /// <param name="poliID"></param>
        /// <returns></returns>
        private int GenerateSortNo(int poliID, int doctorID)
        {
            List<QueuePoli> currentQueueList = _unitOfWork.RegistrationRepository.Get(x => x.PoliTo == poliID &&
            x.TransactionDate.Year == DateTime.Today.Year &&
            x.TransactionDate.Month == DateTime.Today.Month &&
            x.TransactionDate.Day == DateTime.Today.Day);

            if (doctorID > 0 && currentQueueList.Count > 0)
                currentQueueList = currentQueueList.Where(x => x.DoctorID == doctorID).ToList();

            int sortNumber = currentQueueList.Count + 1;

            return sortNumber;
        }
    }
}
