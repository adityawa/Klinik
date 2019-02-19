using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.DataAccess;
using Klinik.Web.Models.MasterData;
using AutoMapper;
using Klinik.Web.Enumerations;

namespace Klinik.Web.Features.MasterData.GeneralMaster
{
    public class MasterHandler : BaseFeatures
    {
        public MasterHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IQueryable<Web.GeneralMaster> GetMasterDataByType(string type)
        {
            return _unitOfWork.MasterRepository.Query(x => x.Type.Equals(type));
        }

        
    }
}