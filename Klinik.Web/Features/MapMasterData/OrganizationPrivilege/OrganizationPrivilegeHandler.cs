using Klinik.Web.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Klinik.Web.Models.MappingMaster;
using LinqKit;
using Klinik.Web.Infrastructure;
using Klinik.Web.Enumerations;
using Klinik.Web.DataAccess.DataRepository;

namespace Klinik.Web.Features.MapMasterData.OrganizationPrivilege
{
    public class OrganizationPrivilegeHandler : BaseFeatures
    {
       
        public OrganizationPrivilegeHandler(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public OrganizationPrivilegeResponse CreateOrEdit(OrganizationPrivilegeRequest request)
        {
            int resultAffected = 0;

            OrganizationPrivilegeResponse response = new OrganizationPrivilegeResponse();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var toberemove = _context.OrganizationPrivileges.Where(x => x.OrgID == request.RequestOrgPrivData.OrgID);
                    _context.OrganizationPrivileges.RemoveRange(toberemove);
                    _context.SaveChanges();

                    //insert new
                    foreach (long _privId in request.RequestOrgPrivData.PrivilegeIDs)
                    {
                        var orgpprivilege = new Web.DataAccess.DataRepository.OrganizationPrivilege
                        {
                            OrgID = request.RequestOrgPrivData.OrgID,
                            PrivilegeID = _privId
                        };
                        _context.OrganizationPrivileges.Add(orgpprivilege);
                    }

                    resultAffected = _context.SaveChanges();

                    transaction.Commit();
                    response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();
                    response.Message = "Data Successfully Saved";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = Common.GetGeneralErrorMesg();
                }
            }


            return response;
        }

  
        public OrganizationPrivilegeResponse GetListData(OrganizationPrivilegeRequest request)
        {
            var qry = _unitOfWork.OrgPrivRepository.Get(x => x.OrgID == request.RequestOrgPrivData.OrgID);
            OrganizationPrivilegeModel _model = new OrganizationPrivilegeModel();
            
            if (qry.Count > 0)
                _model.OrgID = qry.FirstOrDefault().OrgID;

            if (_model.PrivilegeIDs == null)
                _model.PrivilegeIDs = new List<long>();
            foreach (var item in qry)
            {
                _model.PrivilegeIDs.Add(item.PrivilegeID);
            }
            var response = new OrganizationPrivilegeResponse
            {
                Entity = _model
            };
            return response;
        }

      
    }
}