using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Form;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Klinik.Features.Pharmacy
{
    public class PharmacyValidator : BaseFeatures
    {
        private const string EDIT_PRIVILEGE_NAME = "EDIT_FORM_EXAMINE_MEDICINE";

        public PharmacyValidator(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public PharmacyResponse Validate(PharmacyRequest request)
        {
            bool isHavePrivilege = true;
            PharmacyResponse response = new PharmacyResponse();
            try
            {

                #region VALIDATE is any bigger than stock
                foreach (var item in request.Data.Medicines)
                {
                    if (((decimal)item.Detail.Qty > item.Stock) && string.IsNullOrEmpty(item.Detail.ProcessType))
                    {
                        response.Status = false;
                        response.Message = $"Qty order of item {item.Detail.ProductName} is out of stock";
                        break;
                    }
                }
                #endregion

                #region VALIDATE for Komponen Racikan
                List<KomponenObatRacikan> CollOfRacikanKomponen = new List<KomponenObatRacikan>();
                if (request.Data.ObatRacikanKomponens != null)
                {
                    CollOfRacikanKomponen = JsonConvert.DeserializeObject<List<KomponenObatRacikan>>(request.Data.ObatRacikanKomponens);
                    foreach (var item1 in CollOfRacikanKomponen)
                    {
                        var _convStock = Convert.ToDecimal(item1.Stock.Replace('.', ','));
                        var _convAmt = Convert.ToDecimal(item1.Amount.Replace('.', ','));
                        if (_convStock < _convAmt)
                        {
                            response.Status = false;
                            response.Message = $"Qty order of item {item1.Name} is out of stock";
                            break;
                        }
                    }
                }
               
                #endregion

                #region ::VALIDATE IF Product Name different with resep::
                foreach (var item in request.Data.Medicines)
                {
                    if (!item.ProductName.Equals(item.Detail.ProductName) && item.Detail.ProcessType==null)
                    {
                        if (item.Detail.Note == null)
                        {
                            response.Status = false;
                            response.Message = $"Please fill reason if any different between prescript from Doctor. Prescript : {item.ProductName} Apoteker : {item.Detail.ProductName}";
                            break;
                        }
                        else if (String.IsNullOrWhiteSpace(item.Detail.Note))
                        {
                            response.Status = false;
                            response.Message = $"Please fill reason if any different between prescript from Doctor. Prescript : {item.ProductName} Apoteker : {item.Detail.ProductName}";
                            break;
                        }

                    }
                }
                #endregion
                isHavePrivilege = IsHaveAuthorization(EDIT_PRIVILEGE_NAME, request.Account.Privileges.PrivilegeIDs);
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
                response = new PharmacyHandler(_unitOfWork, _context).CreateOrEdit(request);
            }

            return response;
        }
    }
}
