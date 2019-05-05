using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Form;
using Klinik.Entities.Loket;
using Klinik.Resources;
using Klinik.Entities.Cashier;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Klinik.Features.Cashier
{
    public class CashierHandler : BaseFeatures
    {
        public CashierHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public CashierResponse GetDetail(long patienid)
        {
            List<CashierModel> lists = new List<CashierModel>();
            dynamic data;
            CashierResponse cashierResponse = new CashierResponse();

            long formmedicalid = _unitOfWork.FormMedicalRepository.Get(a => a.PatientID == patienid).Select(x => x.ID).FirstOrDefault();
            long examineid = _unitOfWork.FormExamineRepository.Get(x => x.FormMedicalID == formmedicalid).Select(x => x.ID).FirstOrDefault();
            if (formmedicalid != 0)
            {
                var formexeminelab = _unitOfWork.FormExamineLabRepository.Query(x => x.FormMedicalID == formmedicalid );
                var FormExamineMedicine = _unitOfWork.FormExamineMedicineRepository.Get(x => x.FormExamineID == examineid);
                var FormExamineservice = _unitOfWork.FormExamineServiceRepository.Get(x => x.FormExamineID == examineid);
                if(formexeminelab != null)
                {
                    foreach (var item in formexeminelab)
                    {
                        var labdata = new CashierModel
                        {
                            ItemName = item.LabItem.Name,
                            Price = Convert.ToInt32(item.LabItem.Price)
                        };

                        lists.Add(labdata);
                    }
                }

                if (FormExamineservice != null)
                {
                    foreach (var item in FormExamineservice)
                    {
                        var labdata = new CashierModel
                        {
                            ItemName = item.Service.Name,
                            Price = Convert.ToInt32(item.Service.Price)
                        };

                        lists.Add(labdata);
                    }
                }

                if (FormExamineMedicine != null)
                {
                    foreach (var item in FormExamineMedicine)
                    {
                        var labdata = new CashierModel
                        {
                            ItemName = item.Product.Name,
                            Price = Convert.ToInt32(item.Product.RetailPrice)
                        };

                        lists.Add(labdata);
                    }
                }

                
                data = lists.ToList();
                cashierResponse = new CashierResponse
                {
                    Data = data
                };
            }

            return cashierResponse;
        }

        public FormMedical update(long medicalid, FormMedical request)
        {
            FormMedical response = new FormMedical();
            var qry = _unitOfWork.FormMedicalRepository.GetById(medicalid);
            try
            {
                qry.BenefitPaid = request.BenefitPaid;
                qry.BenefitPlan = request.BenefitPlan;
                qry.DiscountAmount = request.DiscountAmount;
                qry.DiscountPercent = request.DiscountPercent;
                qry.TotalPrice = request.TotalPrice;
                qry.Remark = request.Remark;
                _unitOfWork.FormMedicalRepository.Update(qry);
                _unitOfWork.Save();
                response = qry;

            }
            catch (Exception ex)
            {

            }

            return response;
        }

    }
}
