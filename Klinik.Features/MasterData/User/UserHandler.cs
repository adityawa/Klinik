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
            UserResponse response = new UserResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.UserRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        qry.OrganizationID = request.Data.OrgID;
                        qry.ExpiredDate = request.Data.ExpiredDate ?? DateTime.Now.AddDays(100);
                        qry.Status = request.Data.Status;
                        qry.ModifiedBy = request.Data.ModifiedBy;
                        qry.ModifiedDate = DateTime.Now;

                        _unitOfWork.UserRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = $"Success Update User {qry.UserName} with Id {qry.ID}";
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = "Update Data Failed";
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Update Data Failed";
                    }
                }
                else
                {
                    request.Data.Password = CommonUtils.Encryptor(request.Data.Password, CommonUtils.KeyEncryptor);
                    request.Data.ExpiredDate = request.Data.ExpiredDate ?? DateTime.Now.AddDays(100);
                    var UserEntity = Mapper.Map<UserModel, User>(request.Data);
                    UserEntity.CreatedBy = request.Data.CreatedBy ?? "SYSTEM";
                    UserEntity.CreatedDate = DateTime.Now;

                    _unitOfWork.UserRepository.Insert(UserEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = $"Success Add new User {UserEntity.UserName} with Id {UserEntity.ID}";
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Add Data Failed";
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
        /// Get user details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public UserResponse GetDetail(UserRequest request)
        {
            UserResponse response = new UserResponse();

            var qry = _unitOfWork.UserRepository.Query(x => x.ID == request.Data.Id, null);
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
            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.UserName.Contains(request.SearchValue) || p.Organization.OrgName.Contains(request.SearchValue) || p.Employee.EmpName.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
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
                    switch (request.SortColumn.ToLower())
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
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new UserResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
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

            try
            {
                var isExist = _unitOfWork.UserRepository.GetById(request.Data.Id);
                if (isExist.ID > 0)
                {
                    _unitOfWork.UserRepository.Delete(isExist.ID);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = $"Success remove User {isExist.UserName} with Id {isExist.ID}";
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = $"Remove User Failed!";
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = $"Remove User Failed!";
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