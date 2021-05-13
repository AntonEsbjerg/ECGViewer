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
            dataObject = new Datafile();
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

    }
}