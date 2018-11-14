using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace medicalsite
{
    class tableData
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public int FK { get; set; }
        public string image { get; set; }
        public string Specialty { get; set; }
        public string MeSH_Codes { get; set; }
        public string ICD9Codes { get; set; }
    }
}
