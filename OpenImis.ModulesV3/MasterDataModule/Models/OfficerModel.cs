﻿using Newtonsoft.Json;
using OpenImis.ModulesV3.Helpers;
using System;

namespace OpenImis.ModulesV3.MasterDataModule.Models
{
    public class OfficerModel
    {
        public int OfficerId { get; set; }
        public Guid OfficerUUID { get; set; }
        public string Code { get; set; }
        public string LastName { get; set; }
        public string OtherNames { get; set; }
        public string Phone { get; set; }
        public int? LocationId { get; set; }
        public string OfficerIDSubst { get; set; }
        [JsonConverter(typeof(IsoDateSerializer))]
        public DateTime WorksTo { get; set; }
    }
}
