﻿namespace Klinik.Entities.MasterData
{
    public class PoliModel : BaseModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string CreatedDateStr { get; set; }
        public string ModifiedDateStr { get; set; }
    }
}
