using Datalist;
using Klinik.Data.DataRepository;
using System.Collections.Generic;
using System.Linq;

namespace Klinik.Web
{
    public class PatientDataList : MvcDatalist<PatientDataListModel>
    {
        private KlinikDBEntities _context { get; }
        private long _clinicID { get; }

        public PatientDataList(KlinikDBEntities context, long clinicID)
        {
            _context = context;
            _clinicID = clinicID;
        }

        public PatientDataList()
        {
            Url = "AllPatient";
            Title = "Patient";

            Filter.Sort = "Name";
            Filter.Order = DatalistSortOrder.Asc;
        }

        public override IQueryable<PatientDataListModel> GetModels()
        {
            List<PatientDataListModel> result = new List<PatientDataListModel>();
            List<PatientClinic> patientClinicList = _context.PatientClinics.Where(x => x.ClinicID == _clinicID).ToList();
            foreach (var item in patientClinicList)
            {
                Patient patient = _context.Patients.FirstOrDefault(x => x.ID == item.PatientID);
                if (patient != null)
                {
                    PatientDataListModel patientModel = MapFrom(patient);
                    result.Add(patientModel);
                }
            }

            return result.AsQueryable();
        }

        private PatientDataListModel MapFrom(Patient patient)
        {
            PatientDataListModel patientModel = new PatientDataListModel
            {
                Id = patient.ID,
                Name = patient.Name,
                MRNumber = patient.MRNumber,
                KTPNumber = patient.KTPNumber,
                Birthdate = patient.BirthDate,
                Address = patient.Address
            };

            return patientModel;
        }
    }
}