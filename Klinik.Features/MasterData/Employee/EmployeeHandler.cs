using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MasterData;
using Klinik.Resources;
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
        /// 
        private const string EmployeeRelationshipCode = "E";
        public EmployeeHandler(IUnitOfWork unitOfWork, KlinikDBEntities context=null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Create or edit employee
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public EmployeeResponse CreateOrEdit(EmployeeRequest request)
        {
            EmployeeResponse response = new EmployeeResponse();
           
                #region ::OLD::
                //var qry = _unitOfWork.EmployeeRepository.GetById(request.Data.EmpID);
                //if (qry != null)
                //{
                //    // save the old data
                //    var _oldentity = Mapper.Map<Employee, EmployeeModel>(qry);

                //    // update data
                //    qry.EmpName = request.Data.EmpName;
                //    qry.BirthDate = request.Data.Birthdate;
                //    qry.Gender = request.Data.Gender;
                //    qry.Email = request.Data.Email;
                //    qry.EmpType = request.Data.EmpType;
                //    qry.KTPNumber = request.Data.KTPNumber;
                //    qry.HPNumber = request.Data.HPNumber;
                //    qry.ReffEmpID = request.Data.ReffEmpID;
                //    qry.LastEmpID = request.Data.LastEmpID;
                //    qry.ModifiedBy = request.Data.ModifiedBy ?? "SYSTEM";
                //    qry.ModifiedDate = DateTime.Now;

                //    // update
                //    _unitOfWork.EmployeeRepository.Update(qry);
                //    int resultAffected = _unitOfWork.Save();

                //    if (resultAffected > 0)
                //    {
                //        response.Message = string.Format(Messages.ObjectHasBeenUpdated, "Employee", qry.EmpName, qry.id);

                //        CommandLog(true, ClinicEnums.Module.MASTER_EMPLOYEE, Constants.Command.EDIT_EMPLOYEE, request.Data.Account, request.Data, _oldentity);
                //    }
                //    else
                //    {
                //        response.Status = false;
                //        response.Message = string.Format(Messages.UpdateObjectFailed, "Employee");

                //        CommandLog(false, ClinicEnums.Module.MASTER_EMPLOYEE, Constants.Command.EDIT_EMPLOYEE, request.Data.Account, request.Data, _oldentity);
                //    }
                //}
                //else
                //{
                //    response.Status = false;
                //    response.Message = string.Format(Messages.UpdateObjectFailed, "Employee");

                //    CommandLog(false, ClinicEnums.Module.MASTER_EMPLOYEE, Constants.Command.EDIT_EMPLOYEE, request.Data.Account, request.Data);
                //}
                #endregion
                using (var transaction = _context.Database.BeginTransaction())
                {
                    int _resultAffected = 0;
                    try
                    {
                        var _qry = _context.Employees.SingleOrDefault(x => x.EmpID == request.Data.EmpID);
                        if (_qry != null)
                        {
                            // save the old data
                            var _oldentity = Mapper.Map<Employee, EmployeeModel>(_qry);
                            // update data
                            _qry.EmpName = request.Data.EmpName;
                            _qry.BirthDate = request.Data.Birthdate;
                            _qry.Gender = request.Data.Gender;
                            _qry.Email = request.Data.Email;
                            _qry.EmpType = request.Data.EmpType;
                            _qry.KTPNumber = request.Data.KTPNumber;
                            _qry.HPNumber = request.Data.HPNumber;
                            _qry.ReffEmpID = request.Data.ReffEmpID;
                            _qry.LastEmpID = request.Data.LastEmpID;
                            _qry.ModifiedBy = request.Data.ModifiedBy ?? "SYSTEM";
                            _qry.ModifiedDate = DateTime.Now;
                            _resultAffected = _context.SaveChanges();

                            if (_resultAffected > 0)
                                CommandLog(true, ClinicEnums.Module.MASTER_EMPLOYEE, Constants.Command.EDIT_EMPLOYEE, request.Data.Account, request.Data, _oldentity);

                            //cek employee assignment, if empid exist with joint date
                            long _emplId = GetEmployeeNo(request.Data.EmpID);
                            if (_emplId == 0 && !String.IsNullOrEmpty(request.Data.LastEmpId))
                                _emplId = GetEmployeeNo(request.Data.LastEmpId);

                            var _qryEmpAss = _context.EmployeeAssignments.FirstOrDefault(x => x.EmployeeID == _emplId && x.StartDate == request.Data.StartDate);
                            if (_qryEmpAss != null)
                            {
                                _resultAffected = 0;
                                _qryEmpAss.StartDate = request.Data.StartDate;
                                _qryEmpAss.EndDate = request.Data.EndDate;
                                _qryEmpAss.Department = request.Data.Department;
                                _qryEmpAss.Region = request.Data.Region;
                                _qryEmpAss.BusinessUnit = request.Data.BussinesUnit;
                                _qryEmpAss.EmpStatus = request.Data.EmpStatus;
                                _qryEmpAss.LastEmpID = request.Data.LastEmpId;
                                _qryEmpAss.ModifiedBy = request.Data.ModifiedBy ?? "SYSTEM";
                                _qryEmpAss.ModifiedDate = DateTime.Now;
                                _resultAffected = _context.SaveChanges();

                                var newEmployeeAssignment = new EmployeeAssignment
                                {
                                    EmployeeID = _qry.id,
                                    BusinessUnit = request.Data.BussinesUnit,
                                    Department = request.Data.Department,
                                    StartDate = request.Data.StartDate,
                                    EndDate = request.Data.EndDate,
                                    Region = request.Data.Region,
                                    EmpStatus = request.Data.EmpStatus,
                                    LastEmpID = request.Data.LastEmpID
                                };
                                if(_resultAffected>0)
                                    CommandLog(true, ClinicEnums.Module.EMPLOYEE_ASSIGNMENT, Constants.Command.EDIT_EMPLOYEEASSIGNMENT, request.Data.Account, newEmployeeAssignment, _qryEmpAss);
                            }
                            else
                            {
                                var _qryLastEmployee = _context.EmployeeAssignments.SingleOrDefault(p => p.LastEmpID == request.Data.LastEmpID);
                                if (_qryLastEmployee != null)
                                {
                                    _qryLastEmployee.EndDate = request.Data.StartDate;
                                    _resultAffected = _context.SaveChanges();
                                    if(_resultAffected>0)
                                        CommandLog(true, ClinicEnums.Module.EMPLOYEE_ASSIGNMENT, $"Edit end date in employee assignment from {_qryLastEmployee.EndDate.Value.ToString()} to {request.Data.StartDate.ToString()}", request.Data.Account);
                                }

                                //insert new employee assignment
                                var _entityEmpAssignment = new EmployeeAssignment
                                {
                                    EmployeeID = _qry.id,
                                    BusinessUnit = request.Data.BussinesUnit,
                                    Department = request.Data.Department,
                                    StartDate = request.Data.StartDate,
                                    EndDate = request.Data.EndDate,
                                    Region = request.Data.Region,
                                    EmpStatus = request.Data.EmpStatus,
                                    LastEmpID = request.Data.LastEmpID
                                };
                                _context.EmployeeAssignments.Add(_entityEmpAssignment);
                                _resultAffected = _context.SaveChanges();
                            }


                            //cek is empstatus in employee table == empstatus in employee assignment
                        }
                        else
                        {
                            //insert new employee
                            if (request.Data.EmpTypeDesc != EmployeeRelationshipCode)
                            {
                                request.Data.EmpID = string.Format("{0}-{1}", request.Data.EmpID, request.Data.StatusCode);
                            }

                            var _employeeEntity = Mapper.Map<EmployeeModel, Employee>(request.Data);
                            _context.Employees.Add(_employeeEntity);
                            _resultAffected = _context.SaveChanges();
                            if(_resultAffected>0)
                                CommandLog(true, ClinicEnums.Module.MASTER_EMPLOYEE, Constants.Command.ADD_NEW_EMPLOYEE, request.Data.Account, _employeeEntity);
                            var _employeeAssignmentEntity = new EmployeeAssignment
                            {
                                EmployeeID = _employeeEntity.id,
                                BusinessUnit = request.Data.BussinesUnit,
                                Department = request.Data.Department,
                                StartDate = request.Data.StartDate,
                                EndDate = request.Data.EndDate,
                                Region = request.Data.Region,
                                EmpStatus = request.Data.EmpStatus,
                                LastEmpID = request.Data.LastEmpID
                            };

                            _context.EmployeeAssignments.Add(_employeeAssignmentEntity);

                            _resultAffected = _context.SaveChanges();
                            if(_resultAffected>0)
                                CommandLog(true, ClinicEnums.Module.EMPLOYEE_ASSIGNMENT, Constants.Command.ADD_EMPLOYEEASSIGNMENT, request.Data.Account, _employeeAssignmentEntity);

                        }

                        transaction.Commit();
                        response.Message = string.Format(Messages.ObjectHasBeenUpdated, "Employee", _qry.EmpName, _qry.EmpID);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "Employee");

                        CommandLog(false, ClinicEnums.Module.MASTER_EMPLOYEE, Constants.Command.EDIT_EMPLOYEE, request.Data.Account, request.Data);
                    }

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
                            qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.EmpID), includes: p => new { p.FamilyRelationship, p.EmployeeStatu, p.EmployeeAssignments });
                            break;
                        case "empname":
                            qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.EmpName), includes: p => new { p.FamilyRelationship, p.EmployeeStatu, p.EmployeeAssignments });
                            break;

                        default:
                            
                            qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.id),  p=>p.EmployeeAssignments, p=>p.FamilyRelationship, prop=>prop.EmployeeStatu);
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "empid":
                            qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.EmpID), includes: p => new { p.FamilyRelationship, p.EmployeeStatu, p.EmployeeAssignments });
                            break;
                        case "empname":
                            qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.EmpName), includes: p => new { p.FamilyRelationship, p.EmployeeStatu, p.EmployeeAssignments });
                            break;

                        default:
                            qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.id), includes: p => new { p.FamilyRelationship, p.EmployeeStatu, p.EmployeeAssignments });
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.EmployeeRepository.Get(searchPredicate, null, includes: p => new { p.FamilyRelationship, p.EmployeeStatu, p.EmployeeAssignments });
            }

            foreach (var item in qry)
            {
                var prData = Mapper.Map<Employee, EmployeeModel>(item);
                
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
            int result_affected = 0;
            #region ::OLD::
            //try
            //{
            //    var isExist = _unitOfWork.EmployeeRepository.GetById(request.Data.Id);
            //    if (isExist.id > 0)
            //    {
            //        _unitOfWork.EmployeeRepository.Delete(isExist.id);
            //        int resultAffected = _unitOfWork.Save();
            //        if (resultAffected > 0)
            //        {
            //            response.Message = string.Format(Messages.ObjectHasBeenRemoved, "Employee", isExist.EmpName, isExist.id);
            //        }
            //        else
            //        {
            //            response.Status = false;
            //            response.Message = string.Format(Messages.RemoveObjectFailed, "Employee");
            //        }
            //    }
            //    else
            //    {
            //        response.Status = false;
            //        response.Message = string.Format(Messages.RemoveObjectFailed, "Employee");
            //    }
            //}
            //catch
            //{
            //    response.Status = false;
            //    response.Message = Messages.GeneralError; ;
            //}
            #endregion

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    if (request.Data.EmpTypeDesc.ToLower() == "employee")
                    {
                        //remove first in Employee Assignment

                        var isExistEmpAssignment = _unitOfWork.EmployeeAssignmentRepository.GetFirstOrDefault(x => x.EmployeeID == GetEmployeeNo(request.Data.EmpID));
                        if (isExistEmpAssignment != null)
                        {
                            var temp = isExistEmpAssignment;
                            _context.EmployeeAssignments.Remove(isExistEmpAssignment);
                            result_affected = _context.SaveChanges();
                            if(result_affected>0)
                                CommandLog(true, ClinicEnums.Module.EMPLOYEE_ASSIGNMENT, Constants.Command.DELETE_EMPLOYEE, request.Data.Account, null, temp);
                        }

                        var isExistInEmployee = _unitOfWork.EmployeeRepository.GetById(request.Data.Id);
                        if (isExistInEmployee != null)
                        {
                            var oldEmployee = isExistInEmployee;
                            result_affected = 0;
                            _context.Employees.Remove(isExistInEmployee);
                            result_affected = _context.SaveChanges();

                            if (result_affected > 0)
                                CommandLog(true, ClinicEnums.Module.MASTER_EMPLOYEE, Constants.Command.DELETE_EMPLOYEE, request.Data.Account, null, oldEmployee);
                        }
                       
                    }

                    else
                    {
                        var isExistInEmployee = _unitOfWork.EmployeeRepository.GetById(request.Data.Id);
                        if (isExistInEmployee != null)
                        {
                            var oldEmployee = isExistInEmployee;
                            _context.Employees.Remove(isExistInEmployee);
                            result_affected = _context.SaveChanges();
                            if (result_affected > 0)
                                CommandLog(true, ClinicEnums.Module.MASTER_EMPLOYEE, Constants.Command.DELETE_EMPLOYEE, request.Data.Account, null, oldEmployee);
                        }
                    }

                    transaction.Commit();
                    response.Message = string.Format(Messages.ObjectHasBeenRemoved, "Employee", request.Data.EmpName, request.Data.EmpID);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                      response.Status = false;
                     response.Message = Messages.GeneralError;
                }
            }
            return response;
        }

        private long GetEmployeeNo(string employeeCd)
        {
            long _emplId = 0;
            var _qry = _unitOfWork.EmployeeRepository.GetFirstOrDefault(x => x.EmpID == employeeCd);
            if (_qry.id != 0)
            {
                _emplId = _qry.id;
            }
            return _emplId;
        }


    }
}