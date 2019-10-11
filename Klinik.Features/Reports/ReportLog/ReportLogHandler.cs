using AutoMapper;
using Klinik.Data;
using Klinik.Entities;
using Klinik.Entities.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.Reports.ReportLog
{
    public class ReportLogHandler:BaseFeatures
    {
        public ReportLogHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ReportLogResponse GetReportLogById(int Id, AccountModel model)
        {
            ReportLogResponse response = new ReportLogResponse();
            var reportLog = _unitOfWork.ReportLogRepository.GetById(Id);
            response.Data = new List<ReportLogModel>();

            if (reportLog != null)
            {
                var reportLogModel = Mapper.Map<Klinik.Data.DataRepository.ReportLog, ReportLogModel>(reportLog);
                reportLogModel.Account = model;
                response.Data.Add(reportLogModel);
            }
            return response;
        }

        public long CreateReportLog(ReportLogRequest request)
        {
            long result = 0;
            try
            {
                var lookupEntity = Mapper.Map<ReportLogModel, Data.DataRepository.ReportLog>(request.Data);
                lookupEntity.CreatedBy = request.Data.Account.UserCode;
                lookupEntity.CreatedDate = DateTime.Now;

                _unitOfWork.ReportLogRepository.Insert(lookupEntity);
                int resultAffected = _unitOfWork.Save();
                if (resultAffected != 0) result = lookupEntity.ID;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }
    }
}
