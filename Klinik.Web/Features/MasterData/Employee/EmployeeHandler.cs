using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.DataAccess;
using Klinik.Web.Models.MasterData;
using Klinik.Web.Infrastructure;
using AutoMapper;
using LinqKit;
using Klinik.Web.Enumerations;

namespace Klinik.Web.Features.MasterData.Employee
{
    public class EmployeeHandler:BaseFeatures, IBaseFeatures<EmployeeResponse, EmployeeRequest>
    {
        public EmployeeHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public EmployeeResponse CreateOrEdit(EmployeeRequest request)
        {
            int resultAffected = 0;

            EmployeeResponse response = new EmployeeResponse();
            try
            {

                if (request.RequestEmployeeData.Id > 0)
                {
                    var qry = _unitOfWork.EmployeeRepository.GetById(request.RequestEmployeeData.Id);
                    if (qry != null)
                    {
                        qry.EmpName = request.RequestEmployeeData.EmpName;
                        qry.BirthDate = request.RequestEmployeeData.Birthdate;
                        qry.Gender = request.RequestEmployeeData.Gender;
                        qry.EmpType = request.RequestEmployeeData.EmpType;
                        qry.EmpDept = request.RequestEmployeeData.EmpDept;
                        qry.ModifiedBy = request.RequestEmployeeData.ModifiedBy??"SYSTEM";
                        qry.ModifiedDate = DateTime.Now;
                        _unitOfWork.EmployeeRepository.Update(qry);
                        resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Status = Enumerations.ClinicEnums.enumStatus.SUCCESS.ToString();
                            response.Message = $"Success Update Employee {qry.EmpName} with Id {qry.id}";
                        }
                        else
                        {
                            response.Status = Enumerations.ClinicEnums.enumStatus.ERROR.ToString();
                            response.Message = "Update Data Failed";
                        }
                    }
                    else
                    {
                        response.Status = Enumerations.ClinicEnums.enumStatus.ERROR.ToString();
                        response.Message = "Update Data Failed";
                    }

                }
                else
                {
                   
                    var EmployeeEntity = Mapper.Map<EmployeeModel, Web.Employee>(request.RequestEmployeeData);
                    EmployeeEntity.CreatedBy = request.RequestEmployeeData.CreatedBy ?? "SYSTEM";
                    EmployeeEntity.CreatedDate = DateTime.Now;
                    _unitOfWork.EmployeeRepository.Insert(EmployeeEntity);
                    resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Status = Enumerations.ClinicEnums.enumStatus.SUCCESS.ToString();
                        response.Message = $"Success Add new User {EmployeeEntity.EmpName} with Id {EmployeeEntity.id}";
                    }
                    else
                    {
                        response.Status = Enumerations.ClinicEnums.enumStatus.ERROR.ToString();
                        response.Message = "Add Data Failed";
                    }
                }


            }
            catch (Exception ex)
            {
                response.Status = Enumerations.ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = Common.GetGeneralErrorMesg();
            }

            return response;
        }

        public IList<EmployeeModel> GetAllEmployee()
        {
            var qry = _unitOfWork.EmployeeRepository.Get();
            IList<EmployeeModel> employees = new List<EmployeeModel>();
            foreach (var item in qry)
            {
                var _clinic = Mapper.Map<Web.Employee, EmployeeModel>(item);
                employees.Add(_clinic);
            }

            return employees;
        }

        public EmployeeResponse GetDetail(EmployeeRequest request)
        {
            EmployeeResponse response = new EmployeeResponse();

            var qry = _unitOfWork.EmployeeRepository.Query(x => x.id == request.RequestEmployeeData.Id, null);
            if (qry.FirstOrDefault() != null)
            {

                response.Entity = Mapper.Map<Web.Employee, EmployeeModel>(qry.FirstOrDefault());
            }
            return response;
        }

        public EmployeeResponse GetListData(EmployeeRequest request)
        {
            List<EmployeeModel> lists = new List<EmployeeModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.True<Klinik.Web.Employee>();
            if (!String.IsNullOrEmpty(request.searchValue) && !String.IsNullOrWhiteSpace(request.searchValue))
            {
                searchPredicate = searchPredicate.And(p => p.EmpID.Contains(request.searchValue) || p.EmpName.Contains(request.searchValue));
            }

            if (!(string.IsNullOrEmpty(request.sortColumn) && string.IsNullOrEmpty(request.sortColumnDir)))
            {
                if (request.sortColumnDir == "asc")
                {
                    switch (request.sortColumn.ToLower())
                    {
                        case "empid":
                            qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.EmpID), includes:x=>x.GeneralMaster);
                            break;
                        case "empname":
                            qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.EmpName), includes: x => x.GeneralMaster);
                            break;
                    
                        default:
                            qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.id), includes: x => x.GeneralMaster);
                            break;
                    }
                }
                else
                {
                    switch (request.sortColumn.ToLower())
                    {
                        case "empid":
                            qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.EmpID), includes: x => x.GeneralMaster);
                            break;
                        case "empname":
                            qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.EmpName), includes: x => x.GeneralMaster);
                            break;
                       
                        default:
                            qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.id), includes: x => x.GeneralMaster);
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, null, includes: x => x.GeneralMaster);
            }
            foreach (var item in qry)
            {
                var prData = Mapper.Map<Web.Employee, EmployeeModel>(item);
                long _empDeptId = prData.EmpDept??0;
                prData.EmpDeptDesc = _unitOfWork.MasterRepository.GetFirstOrDefault(x => x.Id == _empDeptId && x.Type==ClinicEnums.enumMasterTypes.Department.ToString()).Name ?? "";
                lists.Add(prData);
            }


            
            int totalRequest = lists.Count();
            var data = lists.Skip(request.skip).Take(request.pageSize).ToList();
           
            var response = new EmployeeResponse
            {
                draw = request.draw,
                recordsFiltered = totalRequest,
                recordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        public EmployeeResponse RemoveData(EmployeeRequest request)
        {
            EmployeeResponse response = new EmployeeResponse();
            int resultAffected = 0;
            try
            {
                var isExist = _unitOfWork.EmployeeRepository.GetById(request.RequestEmployeeData.Id);
                if (isExist.id > 0)
                {
                    _unitOfWork.EmployeeRepository.Delete(isExist.id);
                    resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Status = Enumerations.ClinicEnums.enumStatus.SUCCESS.ToString();
                        response.Message = $"Success remove Employee {isExist.EmpName} with Id {isExist.id}";
                    }
                    else
                    {
                        response.Status = Enumerations.ClinicEnums.enumStatus.ERROR.ToString();
                        response.Message = $"Remove User Failed!";
                    }
                }
                else
                {
                    response.Status = Enumerations.ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = $"Remove User Failed!";
                }
            }
            catch (Exception ex)
            {
                response.Status = Enumerations.ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = Common.GetGeneralErrorMesg(); ;
            }
            return response;
        }
    }
}