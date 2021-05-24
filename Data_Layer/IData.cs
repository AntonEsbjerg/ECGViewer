using System;
using System.Collections.Generic;
using System.Text;
using DTO;

namespace Data_Layer
{
    public interface IData
    {
        bool isUserRegistered(String socSecNb, String pw);
        List<DTO_ECG> getECGData(String måleID);
        List<DTO_id> fillComboBox();
        DTO_lokalinfo downloadLokalinfo();
        void uploadSTEMI(DTO_lokalinfo nySTEMI);
        void uploadNoSTEMI(DTO_lokalinfo nyNoSTEMI);
    }
}
