using System;
using System.Collections.Generic;
using System.Text;
using DTO;

namespace Data_Layer
{
    public interface IData
    {
        bool isUserRegistered(String socSecNb, String pw);

        //public List<DTO_ECG> getECGdata(String socsecNb);
    }
}
