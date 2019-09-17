using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Form;
using Klinik.Entities.MasterData;
using Klinik.Entities.MedicalHistoryEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.HistoryMedical
{
    public class MedicalHistoryHandler : BaseFeatures
    {
        public MedicalHistoryHandler(IUnitOfWork unitOfWork, KlinikDBEntities context = null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public MedicalHistoryResponse getEmployeeBaseOnEmpNo(MedicalHistoryRequest request)
        {
            var response = new MedicalHistoryResponse();
            List<EmployeeModel> employees = new List<EmployeeModel>();
            var qryOnlyEmployee = _unitOfWork.EmployeeRepository.GetFirstOrDefault(x => x.EmpID == request.Data.EmployeeData.EmpID);
            var qryForSpouse = _unitOfWork.EmployeeRepository.Get(x => x.ReffEmpID == request.Data.EmployeeData.EmpID && x.EmpType > 1);
            if (qryOnlyEmployee!=null)
            {
                employees.Add(Mapper.Map<Employee, EmployeeModel>(qryOnlyEmployee));
            }

            response.Employees = employees;
            response.RecordsTotal = employees.Count;
            response.RecordsFiltered = employees.Count;
            return response;
        }

        public MedicalHistoryResponse getMedicalActivityHist(MedicalHistoryRequest request)
        {
            var response = new MedicalHistoryResponse();
            List<MedicalHistoryModel> members = null;

            long _employeeId = 0;
            if ((request.Data.EmployeeData.Id != 0))
            {
                if (members == null)
                    members = new List<MedicalHistoryModel>();
                _employeeId = request.Data.EmployeeData.Id;

                var tempRelationship = _unitOfWork.FamilyRelationshipRepository.Get(x => x.RowStatus == 0);
                var tempNecesity = _unitOfWork.MasterRepository.Get(x => x.Type == Constants.MasterType.NECESSITY_TYPE);

                var _qry = _unitOfWork.PatientRepository.Get(x => x.EmployeeID == _employeeId);
                foreach (var item in _qry)
                {
                    var _idRelationship = item.FamilyRelationshipID == null ? 0 : item.FamilyRelationshipID;
                    members.Add(new MedicalHistoryModel
                    {
                        IDPatient = item.ID,
                        PatientName = item.Name,
                        Relationship = tempRelationship.FirstOrDefault(x => x.ID == _idRelationship) == null ? "" : tempRelationship.FirstOrDefault(x => x.ID == _idRelationship).Name,
                    });
                }

                List<long> idPatients = new List<long>();
                idPatients = members.Select(x => x.IDPatient).Distinct().ToList();
                //get data formMedical with existing PatientID
                var _qryFormMed = _unitOfWork.FormMedicalRepository.Get(x => idPatients.Contains(x.PatientID ?? 0)).Select(x => x.ID).Distinct().ToList();
                var _qryFrmExm = _unitOfWork.FormExamineRepository.Get(x => _qryFormMed.Contains(x.FormMedicalID ?? 0));
                if (_qryFrmExm != null && _qryFrmExm.Count > 0)
                    members = new List<MedicalHistoryModel>();

                foreach (var itm in _qryFrmExm)
                {
                    members.Add(new MedicalHistoryModel
                    {
                        Id = itm.ID,
                        PatientName = itm.FormMedical.Patient.Name,
                        ClinicName = itm.FormMedical.Clinic.Name,
                        PoliName = itm.Poli.Name,
                        Keperluan = tempNecesity.Where(x => x.Value == itm.FormMedical.Necessity).FirstOrDefault() == null ? "" : tempNecesity.Where(x => x.Value == itm.FormMedical.Necessity).FirstOrDefault().Name,
                        Relationship = tempRelationship.FirstOrDefault(x => x.ID == itm.FormMedical.Patient.FamilyRelationshipID) == null ? "" : tempRelationship.FirstOrDefault(x => x.ID == itm.FormMedical.Patient.FamilyRelationshipID).Name,
                        FormMedicalId = itm.FormMedical.ID,
                        Tanggal = itm.FormMedical.StartDate.HasValue ? itm.FormMedical.StartDate.Value.ToShortDateString() : ""
                    });
                }
            }

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                members = members.Where(x => x.ClinicName.Contains(request.SearchValue) || x.PatientName.Contains(request.SearchValue) || x.PoliName.Contains(request.SearchValue)).ToList();
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "Tanggal":
                            members = members.OrderBy(x => Convert.ToDateTime(x.Tanggal)).ToList();
                            break;
                        case "PatientName":
                            members = members.OrderBy(x => x.PatientName).ToList();
                            break;
                        case "ClinicName":
                            members = members.OrderBy(x => x.ClinicName).ToList();
                            break;
                        case "PoliName":
                            members = members.OrderBy(x => x.PoliName).ToList();
                            break;

                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "Tanggal":
                            members = members.OrderByDescending(x => Convert.ToDateTime(x.Tanggal)).ToList();
                            break;
                        case "PatientName":
                            members = members.OrderByDescending(x => x.PatientName).ToList();
                            break;
                        case "ClinicName":
                            members = members.OrderByDescending(x => x.ClinicName).ToList();
                            break;
                        case "PoliName":
                            members = members.OrderByDescending(x => x.PoliName).ToList();
                            break;

                    }
                }
            }
            response.MedicalHistories = members;
            var data = members.Skip(request.Skip).Take(request.PageSize).ToList();

            response.Data = data;
            response.RecordsTotal = members.Count;
            response.RecordsFiltered = members.Count;

            return response;
        }

      
    }
}
