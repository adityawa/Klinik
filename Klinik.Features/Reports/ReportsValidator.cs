using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.Reports
{
    public class ReportsValidator: BaseFeatures
    {
        private const string DISPLAY_REPORTS = "VIEW_REPORTS";
        private const string DISPLAY_TOP_10_DISEASES_REPORT = "VIEW_TOP_10_DISEASES";
        private const string DISPLAY_TOP_10_REFERAL_REPORT = "VIEW_TOP_10_REFERALS";
        private const string DISPLAY_TOP_10_COST_REPORT = "VIEW_TOP_10_COST";
        private const string DISPLAY_TOP_10_REQUEST_REPORT = "VIEW_TOP_10_REQUEST";

        public ReportsValidator(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public void ValidateTop10DiseaseReport(Top10DiseaseReportRequest request,  out Top10DiseaseReportResponse response)
        {
            bool isHavePrivilege = true;
            response = new Top10DiseaseReportResponse();
            try
            {
                isHavePrivilege = IsHaveAuthorization(DISPLAY_TOP_10_DISEASES_REPORT, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            if (response.Status)
            {
                response = new ReportsHandler(_unitOfWork).GenerateTop10DiseasesReport(request);
            }
        }


        public void ValidateTop10ReferalReport(Top10ReferalReportRequest request, out Top10ReferalReportResponse response)
        {
            bool isHavePrivilege = true;
            response = new Top10ReferalReportResponse();
            try
            {
                isHavePrivilege = IsHaveAuthorization(DISPLAY_TOP_10_REFERAL_REPORT, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            if (response.Status)
            {
                response = new ReportsHandler(_unitOfWork).GenerateTop10ReferalReport(request);
            }
        }

        public void ValidateTop10CostReport(Top10CostReportRequest request, out Top10CostReportResponse response)
        {
            bool isHavePrivilege = true;
            response = new Top10CostReportResponse();

            try
            {
                isHavePrivilege = IsHaveAuthorization(DISPLAY_TOP_10_COST_REPORT, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            if (response.Status)
            {
                response = new ReportsHandler(_unitOfWork).GenerateTop10CostReport(request);
            }
        }

        public void ValidateTop10RequestReport(Top10RequestReportRequest request, out Top10RequestReportResponse response)
        {
            bool isHavePrivilege = true;
            response = new Top10RequestReportResponse();

            try
            {
                isHavePrivilege = IsHaveAuthorization(DISPLAY_TOP_10_REQUEST_REPORT, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            if (response.Status)
            {
                response = new ReportsHandler(_unitOfWork).GenerateTop10RequestReport(request);
            }
        }
    }
}
