//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Klinik.Data.DataRepository
{
    using System;
    using System.Collections.Generic;
    
    public partial class Log
    {
        public long ID { get; set; }
        public System.DateTime Start { get; set; }
        public string Module { get; set; }
        public long Account { get; set; }
        public string Command { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Status { get; set; }
        public short RowStatus { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
