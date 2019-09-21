using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MasterData;
using Klinik.Features.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class ReportsHandler:BaseFeatures
    {
        public ReportsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<LookupCategory> GetLookUpByName(string name)
        {
            return _unitOfWork.LookUpCategoryRepository.Query(x => x.LookUpName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public Top10DiseaseReportResponse GenerateTop10DiseasesReport(Top10DiseaseReportRequest request)
        {
            var response = new Top10DiseaseReportResponse();


            return response;
        }


        public Top10ReferalReportResponse GenerateTop10ReferalReport(Top10ReferalReportRequest request)
        {
            var response = new Top10ReferalReportResponse();


            return response;
        }
    }
}
