using Klinik.Entities.Account;
using Klinik.Entities.MasterData;
using System;

namespace Klinik.Entities
{
    public class BaseModel
    {
        public long Id { get; set; }
        public short RowStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public AccountModel Account { get; set; }
        //public PoliModel CreatedDateStr { get; set; }
        //public PoliModel ModifiedDateStr { get; set; }
        //public BaseModel()
        //{
        //    CreatedDateStr = new  PoliModel();
        //    ModifiedDateStr = new PoliModel();
        //}
        
        //public DoctorModel Extends BaseModel;
    }
    //class Date
    //{
    //    public string CreatedDateStr { get; set; }
    //    public string ModifiedDateStr { get; set; }
    //}
    //class Doctor : Date {
        
    //}


}