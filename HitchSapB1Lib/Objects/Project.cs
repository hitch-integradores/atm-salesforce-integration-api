using System;
using System.Collections.Generic;

namespace HitchSapB1Lib.Objects.Definition
{
    public class Project
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public List<UserField> UserFields { get; set; }
    }
}