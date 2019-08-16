using Klinik.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class PurchaseOrderValidator : BaseFeatures
    {
        private const string ADD_M_PURCHASEORDER = "ADD_M_PURCHASEORDER";
        private const string EDIT_M_PURCHASEORDER = "EDIT_M_PURCHASEORDER";
        private const string DELETE_M_PURCHASEORDER = "DELETE_M_PURCHASEORDER";

        public PurchaseOrderValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
