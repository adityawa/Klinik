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
            EmployeeResponse response = new EmployeeResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.EmployeeRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        qry.EmpName = request.Data.EmpName;
                        qry.BirthDate = request.Data.Birthdate;
                        qry.Gender = request.Data.Gender;
                        qry.Email = request.Data.Email;
                        qry.EmpType = request.Data.EmpType;
                        qry.EmpDept = request.Data.EmpDept;
                        qry.ModifiedBy = request.Data.ModifiedBy ?? "SYSTEM";
                        qry.ModifiedDate = DateTime.Now;

                        // update
                        _unitOfWork.EmployeeRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();

                        if (resultAffected > 0)
                        {
                            response.Message = $"Employee {qry.EmpName} with ID {qry.id} has been successfully updated";
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = "Employee Update Failed";
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Employee Update Failed";
                    }
                }
                else
                {
                    var EmployeeEntity = Mapper.Map<EmployeeModel, Employee>(request.Data);
                    EmployeeEntity.CreatedBy = request.Data.CreatedBy ?? "SYSTEM";
                    EmployeeEntity.CreatedDate = DateTime.Now;

                    // insert 
                    _unitOfWork.EmployeeRepository.Insert(EmployeeEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = $"Employee {EmployeeEntity.EmpName} with ID {EmployeeEntity.id} has been successfully added";

                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Employee Add Failed";
                    }
                }
            }
            catch
            {
                response.Status = false;
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

            var qry = _unitOfWork.EmployeeRepository.Query(x => x.id == request.Data.Id, null);
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
            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.EmpID.Contains(request.SearchValue) || p.EmpName.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
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
                    switch (request.SortColumn.ToLower())
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
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new EmployeeResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
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

            try
            {
                var isExist = _unitOfWork.EmployeeRepository.GetById(request.Data.Id);
                if (isExist.id > 0)
                {
                    _unitOfWork.EmployeeRepository.Delete(isExist.id);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = $"Employee {isExist.EmpName} with ID {isExist.id} has been successfully removed";
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = $"Employee Removal Failed!";
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = $"Employee Removal Failed!";
                }
            }
            catch
            {
                response.Status = false;
                response.Message = CommonUtils.GetGeneralErrorMesg(); ;
            }

            return response;
        }
    }
}