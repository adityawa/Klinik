using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MasterData;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Klinik.Features
{
    public class EmployeeHandler : BaseFeatures, IBaseFeatures<EmployeeResponse, EmployeeRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public EmployeeHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create or edit employee
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
                        qry.Email = request.RequestEmployeeData.Email;
                        qry.EmpType = request.RequestEmployeeData.EmpType;
                        qry.EmpDept = request.RequestEmployeeData.EmpDept;
                      
                        qry.ModifiedBy = request.RequestEmployeeData.ModifiedBy ?? "SYSTEM";
                        qry.ModifiedDate = DateTime.Now;

                        // update
                        _unitOfWork.EmployeeRepository.Update(qry);
                        resultAffected = _unitOfWork.Save();

                        if (resultAffected > 0)
                        {
                            response.Status = ClinicEnums.Status.SUCCESS.ToString();
                            response.Message = $"Employee {qry.EmpName} with ID {qry.id} has been successfully updated";
                        }
                        else
                        {
                            response.Status = ClinicEnums.Status.ERROR.ToString();
                            response.Message = "Employee Update Failed";
                        }
                    }
                    else
                    {
                        response.Status = ClinicEnums.Status.ERROR.ToString();
                        response.Message = "Employee Update Failed";
                    }
                }
                else
                {
                    var EmployeeEntity = Mapper.Map<EmployeeModel, Employee>(request.RequestEmployeeData);
                    EmployeeEntity.CreatedBy = request.RequestEmployeeData.CreatedBy ?? "SYSTEM";
                    EmployeeEntity.CreatedDate = DateTime.Now;

                    // insert 
                    _unitOfWork.EmployeeRepository.Insert(EmployeeEntity);
                    resultAffected = _unitOfWork.Save();

                    if (resultAffected > 0)
                    {
                        response.Status = ClinicEnums.Status.SUCCESS.ToString();

                        response.Message = $"Employee {EmployeeEntity.EmpName} with ID {EmployeeEntity.id} has been successfully added";

                    }
                    else
                    {
                        response.Status = ClinicEnums.Status.ERROR.ToString();
                        response.Message = "Employee Add Failed";
                    }
                }
            }
            catch
            {
                response.Status = ClinicEnums.Status.ERROR.ToString();
                response.Message = CommonUtils.GetGeneralErrorMesg();
            }

            return response;
        }

        /// <summary>
        /// Get all employee list
        /// </summary>
        /// <returns></returns>
        public IList<EmployeeModel> GetAllEmployee()
        {
            var qry = _unitOfWork.EmployeeRepository.Get();
            IList<EmployeeModel> employees = new List<EmployeeModel>();
            foreach (var item in qry)
            {
                var _clinic = Mapper.Map<Employee, EmployeeModel>(item);
                employees.Add(_clinic);
            }

            return employees;
        }

        /// <summary>
        /// Get employee details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public EmployeeResponse GetDetail(EmployeeRequest request)
        {
            EmployeeResponse response = new EmployeeResponse();

            var qry = _unitOfWork.EmployeeRepository.Query(x => x.id == request.RequestEmployeeData.Id, null);
            if (qry.FirstOrDefault() != null)
            {

                response.Entity = Mapper.Map<Employee, EmployeeModel>(qry.FirstOrDefault());
            }
            return response;
        }

        /// <summary>
        /// Get employee list of data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public EmployeeResponse GetListData(EmployeeRequest request)
        {
            List<EmployeeModel> lists = new List<EmployeeModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Employee>(true);
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
                            qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.EmpID), includes: x => x.GeneralMaster);
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
                var prData = Mapper.Map<Employee, EmployeeModel>(item);
                long _empDeptId = prData.EmpDept ?? 0;
                prData.EmpDeptDesc = _unitOfWork.MasterRepository.GetFirstOrDefault(x => x.Id == _empDeptId && x.Type == ClinicEnums.MasterTypes.Department.ToString()).Name ?? "";
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

        /// <summary>
        /// Remove employee data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
                        response.Status = ClinicEnums.Status.SUCCESS.ToString();
                        response.Message = $"Employee {isExist.EmpName} with ID {isExist.id} has been successfully removed";
                    }
                    else
                    {
                        response.Status = ClinicEnums.Status.ERROR.ToString();
                        response.Message = $"Employee Removal Failed!";
                    }
                }
                else
                {
                    response.Status = ClinicEnums.Status.ERROR.ToString();
                    response.Message = $"Employee Removal Failed!";
                }
            }
            catch
            {
                response.Status = ClinicEnums.Status.ERROR.ToString();
                response.Message = CommonUtils.GetGeneralErrorMesg(); ;
            }

            return response;
        }
    }
}