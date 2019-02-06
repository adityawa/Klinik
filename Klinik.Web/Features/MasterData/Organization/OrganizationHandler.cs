using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.DataAccess.Interfaces;
using Klinik.Web.DataAccess.Concrete;
using Klinik.Web.Features.MasterData.Organization;
using AutoMapper;
using LinqKit;

namespace Klinik.Web.Features.MasterData.Organization
{

    public class OrganizationHandler
    {
        private IUnitOfWork<Klinik.Web.Organization> _unitOfWork;

        public OrganizationHandler(KlinikDBEntities context)
        {
            _unitOfWork = new UnitOfWork<Klinik.Web.Organization>(context);
        }

        public  OrganizationResponse GetOrganizationData(OrganizationRequest request)
        {
            IList<OrganizationData> lists = new List<OrganizationData>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Web.Organization>();
            if(!String.IsNullOrEmpty(request.searchValue) && !String.IsNullOrWhiteSpace(request.searchValue))
            {
                searchPredicate = searchPredicate.And(p => p.OrgCode.Contains(request.searchValue) || p.OrgName.Contains(request.searchValue) || p.Clinic.Name.Contains(request.searchValue));
            }
            qry = _unitOfWork.ModelRepository.Get(searchPredicate,null, includes: x => x.Clinic);

            if (!(string.IsNullOrEmpty(request.sortColumn) && string.IsNullOrEmpty(request.sortColumnDir)))
            {
                if (request.sortColumnDir == "asc")
                {
                    switch (request.sortColumn.ToLower())
                    {
                        case "orgcode":
                            qry = _unitOfWork.ModelRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.OrgCode), includes: x=>x.Clinic);
                            break;
                        case "orgname":
                            qry = _unitOfWork.ModelRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.OrgName), includes: x => x.Clinic);
                            break;
                    }
                }
                else
                {
                    switch (request.sortColumn.ToLower())
                    {
                        case "orgcode":
                            qry = _unitOfWork.ModelRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.OrgCode), includes: x => x.Clinic);
                            break;
                        case "orgname":
                            qry = _unitOfWork.ModelRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.OrgName), includes: x => x.Clinic);
                            break;
                    }
                }
               
            }
            foreach (var item in qry)
            {
                var orData = Mapper.Map<Web.Organization, OrganizationData>(item);

                #region ::OBSOLETE::
                //OrganizationData orData = new OrganizationData();
                //orData.Id = item.ID;
                //orData.Klinik = item.Clinic.Name;
                //orData.OrgCode = item.OrgCode;
                //orData.OrgName = item.OrgName;
                #endregion

                lists.Add(orData);
            }

           
            int totalRequest = lists.Count();
            var data = lists.Skip(request.skip).Take(request.pageSize).ToList();


            var response = new OrganizationResponse
            {
                draw = request.draw,
                recordsFiltered = totalRequest,
                recordsTotal = totalRequest,
                Data = data
            };

            return response;
        }
    }
}