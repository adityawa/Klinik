using AutoMapper;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Administration;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.Administration.LogonUsers
{
    public class LogonUserHandler : BaseFeatures
    {
        public LogonUserHandler(IUnitOfWork unitOfWork, KlinikDBEntities context = null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public LogonUserResponse UpdateStatusNonActive(LogonUserRequest request)
        {
            var response = new LogonUserResponse();
            try
            {
                var toBeUpdate = _unitOfWork.LogonUserRepository.GetById(request.Data.Id);
                toBeUpdate.Status = false;
                _unitOfWork.LogonUserRepository.Update(toBeUpdate);
                int tempResult = _unitOfWork.Save();
                if (tempResult > 0)
                {
                    response.Status = true;
                    response.Message = "Disable User Successfull";
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.UpdateObjectFailed, "Status User");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError.ToString();
            }

            return response;
        }

        public LogonUserResponse GetListActiveUser(LogonUserRequest request)
        {
            List<LogonUserModel> activeUsers = new List<LogonUserModel>();
            var response = new LogonUserResponse();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<LogonUser>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.Status == true);

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.UserName.Contains(request.SearchValue) || p.IPAddress.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "username":
                            qry = _unitOfWork.LogonUserRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.UserName));
                            break;

                        default:
                            qry = _unitOfWork.LogonUserRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Id));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "username":
                            qry = _unitOfWork.LogonUserRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.UserName));
                            break;


                        default:
                            qry = _unitOfWork.LogonUserRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Id));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.LogonUserRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var logon = Mapper.Map<LogonUser, LogonUserModel>(item);
                activeUsers.Add(logon);
            }

            int totalRequest = activeUsers.Count();
            var data = activeUsers.Skip(request.Skip).Take(request.PageSize).ToList();

            response = new LogonUserResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }
    }
}
