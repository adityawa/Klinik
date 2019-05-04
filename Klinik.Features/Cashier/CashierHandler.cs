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

        public CashierResponse GetDetail(CashierRequest request)
        {
            List<CashierModel> list = new List<CashierModel>();
            dynamic data;

            long formmedicalid = _unitOfWork.FormMedicalRepository.Get(a => a.PatientID == request.Data.Id).Select(x => x.ID).FirstOrDefault();
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
                            price = item.LabItem.Price
                        };

                        list.Add(labdata);
                    }
                }

                if (FormExamineMedicine != null)
                {
                    foreach (var item in FormExamineMedicine)
                    {
                        var labdata = new CashierModel
                        {
                            ItemName = item.LabItem.Name,
                            price = item.LabItem.Price
                        };

                        list.Add(labdata);
                    }
                }
            }

            return response;
        }

    }
}
