//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Klinik.Web.DataAccess.DataRepository
{
    using System;
    using System.Collections.Generic;
    
    public partial class Log
    {
        public long Id { get; set; }
        public System.DateTime Start { get; set; }
        public string Module { get; set; }
        public long Account { get; set; }
        public string Command { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Status { get; set; }
    }
}