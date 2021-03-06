﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.DeliveryOrderPusatDetail
{
    public class DeliveryOrderPusatDetailModel : BaseModel
    {
        public int DeliveryOrderPusatId { get; set; }
        public int ProductId { get; set; }
        public string namabarang { get; set; }
        public int VendorId { get; set; }
        public string namavendor { get; set; }
        public Nullable<double> satuan { get; set; }
        public Nullable<double> harga { get; set; }
        public Nullable<double> stok_prev { get; set; }
        public Nullable<double> total_req { get; set; }
        public Nullable<double> total_dist { get; set; }
        public Nullable<double> sisa_stok { get; set; }
        public Nullable<double> qty { get; set; }
        public Nullable<double> qty_add { get; set; }
        public string reason_add { get; set; }
        public Nullable<double> qty_final { get; set; }
        public string remark { get; set; }
        public Nullable<double> total { get; set; }
        public Nullable<double> qty_unit { get; set; }
        public Nullable<double> qty_box { get; set; }
        public Nullable<bool> Recived { get; set; }
        public string No_Do_Vendor { get; set; }
        public Nullable<System.DateTime> Tgl_Do_Vendor { get; set; }
        public string SendBy { get; set; }
        public Nullable<int> statusop { get; set; }
    }
}
