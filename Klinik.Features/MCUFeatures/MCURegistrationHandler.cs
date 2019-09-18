using AutoMapper;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MCUPackageEntities;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.MCUFeatures
{
    public class MCURegistrationHandler : BaseFeatures
    {
        public MCURegistrationHandler(IUnitOfWork unitOfWork, KlinikDBEntities context = null)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public MCURegistrationResponse GetListMCURegistration(MCURegistrationRequest request)
        {
            List<MCURegistrationModel> lsMcuReg = new List<MCURegistrationModel>();
           

            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<MCURegistrationInterface>(true);
          

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.REG_NUMBER.Contains(request.SearchValue)|| p.EMPL_NAME.Contains(request.SearchValue) || p.SCHEDULE_CODE.Contains(request.SearchValue) );
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "RESERVE_DATE":
                            qry = _unitOfWork.MCURegistrationRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.RESERVE_DATE));
                            break;
                        case "EMPL_NAME":
                            qry = _unitOfWork.MCURegistrationRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.EMPL_NAME));
                            break;
                        
                        default:
                            qry = _unitOfWork.MCURegistrationRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.REG_ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "RESERVE_DATE":
                            qry = _unitOfWork.MCURegistrationRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.RESERVE_DATE));
                            break;
                        case "EMPL_NAME":
                            qry = _unitOfWork.MCURegistrationRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.EMPL_NAME));
                            break;

                        default:
                            qry = _unitOfWork.MCURegistrationRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.REG_ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.MCURegistrationRepository.Get(searchPredicate, null);
            }

            foreach(var item in qry)
            {
                var mcuModel = Mapper.Map<MCURegistrationInterface, MCURegistrationModel>(item);
                lsMcuReg.Add(mcuModel);
            }

            int totalRequest = lsMcuReg.Count();
            var data = lsMcuReg.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new MCURegistrationResponse
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
