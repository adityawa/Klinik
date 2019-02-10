using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Klinik.Web.Features
{
    public interface IBaseFeatures<TResp, TReq> where TResp : class where TReq : class
    {
        TResp GetListData(TReq request);
        TResp CreateOrEdit(TReq request);
        TResp GetDetail(TReq request);
        TResp RemoveData(TReq request);
    }
}