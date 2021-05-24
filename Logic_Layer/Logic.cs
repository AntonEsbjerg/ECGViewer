using System;
using Data_Layer;
using DTO;
using System.Collections.Generic;


namespace Logic_Layer
{
    public class Logic
    {
        private IData dataObject;
        public Logic()
        {
            dataObject = new DataDB();
        }
        
        public bool checkLogin(String username, String pw)
        {
            if (dataObject.isUserRegistered(username, pw) == true)
                return true;
            else
                return false;
        }
        public List<DTO_id> ID()
        {
            List<DTO_id> id = new List<DTO_id>();
            foreach (var item in dataObject.fillComboBox())
            {
                id.Add(item);
            }
            return id;
        }
        public List<DTO_ECG> ECGData(string måleID)
        {
            List<DTO_ECG> ecg = new List<DTO_ECG>();
            foreach (var item in dataObject.getECGData(måleID))
            {
                ecg.Add(item);
            }
            return ecg;
        }
        public DTO_lokalinfo GetLokalinfo()
        {
            DTO_lokalinfo info = dataObject.downloadLokalinfo();
            return info;
        }
        public void uploadSTEMI(DTO_lokalinfo nySTEMI)
        {
            dataObject.uploadSTEMI(nySTEMI);
        }
        public void uploadNoSTEMI(DTO_lokalinfo nyNoSTEMI)
        {
            dataObject.uploadNoSTEMI(nyNoSTEMI);
        }
    }
}