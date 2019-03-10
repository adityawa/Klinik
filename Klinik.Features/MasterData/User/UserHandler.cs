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
    public class UserHandler : BaseFeatures, IBaseFeatures<UserResponse, UserRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public UserHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create or edit user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public UserResponse CreateOrEdit(UserRequest request)
        {
            int resultAffected = 0;
            UserResponse response = new UserResponse();

            try
            {
                if (request.RequestUserData.Id > 0)
                {
                    var qry = _unitOfWork.UserRepository.GetById(request.RequestUserData.Id);
                    if (qry != null)
                    {
                        qry.OrganizationID = request.RequestUserData.OrgID;
                        qry.ExpiredDate = request.RequestUserData.ExpiredDate ?? DateTime.Now.AddDays(100);
                        qry.Status = request.RequestUserData.Status;
                        qry.ModifiedBy = request.RequestUserData.ModifiedBy;
                        qry.ModifiedDate = DateTime.Now;
                        _unitOfWork.UserRepository.Update(qry);
                        resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();
                            response.Message = $"Success Update User {qry.UserName} with Id {qry.ID}";
                        }
                        else
                        {
                            response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                            response.Message = "Update Data Failed";
                        }
                    }
                    else
                    {
                        response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                        response.Message = "Update Data Failed";
                    }
                }
                else
                {
                    request.RequestUserData.Password = CommonUtils.Encryptor(request.RequestUserData.Password, CommonUtils.KeyEncryptor);
                    request.RequestUserData.ExpiredDate = request.RequestUserData.ExpiredDate ?? DateTime.Now.AddDays(100);
                    var UserEntity = Mapper.Map<UserModel, User>(request.RequestUserData);
                    UserEntity.CreatedBy = request.RequestUserData.CreatedBy ?? "SYSTEM";
                    UserEntity.CreatedDate = DateTime.Now;
                    _unitOfWork.UserRepository.Insert(UserEntity);
                    resultAffected = _unitOfWork.Save();

                    if (resultAffected > 0)
                    {
                        response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();
                        response.Message = $"Success Add new User {UserEntity.UserName} with Id {UserEntity.ID}";
                    }
                    else
                    {
                        response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                        response.Message = "Add Data Failed";
                    }
                }
            }
            catch
            {
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = CommonUtils.GetGeneralErrorMesg();
            }

            return response;
        }

        /// <summary>
        /// Get user details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public UserResponse GetDetail(UserRequest request)
        {
            UserResponse response = new UserResponse();

            var qry = _unitOfWork.UserRepository.Query(x => x.ID == request.RequestUserData.Id, null);
            if (qry.FirstOrDefault() != null)
            {

                response.Entity = Mapper.Map<User, UserModel>(qry.FirstOrDefault());
            }
            return response;
        }

        /// <summary>
        /// Get list of user data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public UserResponse GetListData(UserRequest request)
        {
            List<UserModel> lists = new List<UserModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<User>(true);
            if (!String.IsNullOrEmpty(request.searchValue) && !String.IsNullOrWhiteSpace(request.searchValue))
            {
                searchPredicate = searchPredicate.And(p => p.UserName.Contains(request.searchValue) || p.Organization.OrgName.Contains(request.searchValue) || p.Employee.EmpName.Contains(request.searchValue));
            }

            if (!(string.IsNullOrEmpty(request.sortColumn) && string.IsNullOrEmpty(request.sortColumnDir)))
            {
                if (request.sortColumnDir == "asc")
                {
                    switch (request.sortColumn.ToLower())
                    {
                        case "username":
                            qry = _unitOfWork.UserRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.UserName));
                            break;
                        case "employeename":
                            qry = _unitOfWork.UserRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Employee.EmpName));
                            break;
                        case "organizationname":
                            qry = _unitOfWork.UserRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Organization.OrgName));
                            break;

                        default:
                            qry = _unitOfWork.UserRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.sortColumn.ToLower())
                    {
                        case "username":
                            qry = _unitOfWork.UserRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.UserName));
                            break;
                        case "employeename":
                            qry = _unitOfWork.UserRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Employee.EmpName));
                            break;
                        case "organizationname":
                            qry = _unitOfWork.UserRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Organization.OrgName));
                            break;

                        default:
                            qry = _unitOfWork.UserRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.UserRepository.Get(searchPredicate, null);
            }
            foreach (var item in qry)
            {
                var prData = Mapper.Map<User, UserModel>(item);

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.skip).Take(request.pageSize).ToList();

            var response = new UserResponse
            {
                draw = request.draw,
                recordsFiltered = totalRequest,
                recordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Remove user data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public UserResponse RemoveData(UserRequest request)
        {
            UserResponse response = new UserResponse();
            int resultAffected = 0;

            try
            {
                var isExist = _unitOfWork.UserRepository.GetById(request.RequestUserData.Id);
                if (isExist.ID > 0)
                {
                    _unitOfWork.UserRepository.Delete(isExist.ID);
                    resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();
                        response.Message = $"Success remove User {isExist.UserName} with Id {isExist.ID}";
                    }
                    else
                    {
                        response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                        response.Message = $"Remove User Failed!";
                    }
                }
                else
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = $"Remove User Failed!";
                }
            }
            catch
            {
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = CommonUtils.GetGeneralErrorMesg(); ;
            }
            return response;
        }
    }
}