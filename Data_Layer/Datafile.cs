using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DTO;

namespace Data_Layer
{
    public class Datafile : IData
    {
        private FileStream input;
        private StreamReader reader;
        public Datafile() { }


        public bool isUserRegistered(String username, String pw)
        {
            bool result = false;

            input = new FileStream("Registered Users.txt", FileMode.Open, FileAccess.Read);
            reader = new StreamReader(input);

            string inputRecord;
            string[] inputFields;

            while ((inputRecord = reader.ReadLine()) != null)
            {
                inputFields = inputRecord.Split(';');

                if (inputFields[0] == username && inputFields[1] == pw)
                {
                    result = true;
                    break;
                }
            }

            reader.Close();

            return result;
        }
    }

    //public List<DTO_ECG> getECGdata(String socsecNb)
    //{
    //   List<DTO_ECG> ecglist = new List<DTO_ECG>();

    //   foreach (var ECG in collection)
    //   {
    //      ecglist.add(ECG);
    //   }

    //   return ecglist;

    //}
}