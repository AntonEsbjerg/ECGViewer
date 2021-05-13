using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class DTO_id
    {
        public String måleID { get; set; }
        public String borgerCPR { get; set; }

        public DTO_id(String måleid, String borgercpr)
        {
            måleID = måleid;
            borgerCPR = borgercpr;
        }
    }
}
