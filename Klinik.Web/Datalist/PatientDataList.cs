using Datalist;
using Klinik.Data.DataRepository;
using Klinik.Resources;
using System;
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
            List<QueuePoli> queueList = _context.QueuePolis.Where(x => x.ClinicID == _clinicID &&
            x.TransactionDate.Year == DateTime.Today.Year &&
            x.TransactionDate.Month == DateTime.Today.Month &&
            x.TransactionDate.Day == DateTime.Today.Day).ToList();

            foreach (var item in patientClinicList)
            {
                Patient patient = _context.Patients.FirstOrDefault(x => x.ID == item.PatientID);
                if (patient != null && !queueList.Any(x => x.PatientID == item.PatientID && x.Status == 0))
                {
                    PatientDataListModel patientModel = MapFrom(patient);
                    result.Add(patientModel);
                }
            }

            return result.AsQueryable();
        }

        private PatientDataListModel MapFrom(Patient patient)
        {
            string address = patient.Address;

            if (patient.CityID.HasValue)
            {
                City city = _context.Cities.FirstOrDefault(x => x.Id == patient.CityID.Value);
                if (city != null)
                {
                    if (!string.IsNullOrEmpty(city.Kelurahan))
                        address += ", " + city.Kelurahan;
                    if (!string.IsNullOrEmpty(city.Kecamatan))
                        address += ", " + city.Kecamatan;
                    if (!string.IsNullOrEmpty(city.City1))
                        address += ", " + city.City1;
                    if (!string.IsNullOrEmpty(city.Province))
                        address += ", " + city.Province;
                }
            }

            PatientDataListModel patientModel = new PatientDataListModel
            {
                Id = patient.ID,
                Name = patient.Name,
                MRNumber = patient.MRNumber,
                KTPNumber = patient.KTPNumber,
                BPJSNumber = patient.BPJSNumber,
                Birthdate = patient.BirthDate,
                Address = address,
                BloodType = patient.BloodType,
                Gender = patient.Gender == "M" ? Messages.Male : Messages.Female,
                PatientType = patient.Type.HasValue && patient.Type.Value == 2 ? Messages.Company : Messages.General,
            };

            return patientModel;
        }
    }
}