using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MappingMaster;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Klinik.Features
{
    public class OrganizationPrivilegeHandler : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="context"></param>
        public OrganizationPrivilegeHandler(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Create or edit organization privilege
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
                        var orgpprivilege = new OrganizationPrivilege
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
                    response.Message = CommonUtils.GetGeneralErrorMesg();
                }
            }

            return response;
        }

        /// <summary>
        /// Get list of organization privilege data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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