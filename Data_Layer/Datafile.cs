using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using DTO;

namespace Data_Layer
{
    public class Datafile : IData
    {
        private FileStream input;
        private StreamReader reader;
        public Datafile() { }

        private SqlConnection connect
        {
            get
            {
                var con = new SqlConnection(@"Data Source=st-i4dab.uni.au.dk;Initial Catalog=ST2PRJ2OffEKGDatabase;Integrated Security=False;User ID=ST2PRJ2OffEKGDatabase;Password=ST2PRJ2OffEKGDatabase;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False");

                con.Open();

                return con;
            }
        }
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
        public List<DTO_id> fillComboBox()
        {
            string socsecNB;
            string måleID;
            List<DTO_id> tempsocsecNB= new List<DTO_id>(); 
            SqlDataReader rdr;

            string insertStringParam = ("Select borger_cprnr,ekgmaaleid from EKGMAELING where borger_cprnr IS NOT NULL");
            using (SqlCommand cmd = new SqlCommand(insertStringParam, connect))
            {
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    socsecNB = Convert.ToString(rdr["borger_cprnr"]);
                    måleID = Convert.ToString(rdr["ekgmaaleid"]);
                    tempsocsecNB.Add(new DTO_id(socsecNB, måleID));
                }
            }
            connect.Close();
            return tempsocsecNB;
        }
        public List<DTO_ECG> loadECG(String måleID)
        {
            List<DTO_ECG> ecg = new List<DTO_ECG>();
            SqlDataReader rdr;
            byte[] bytesArr = new byte[8];
            double[] tal;
            string selectString = "Select raa_data From EKGDATA  where ekgmaaleid = " + måleID;
            using (SqlCommand cmd = new SqlCommand(selectString, connect))
            {
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                    bytesArr = (byte[])rdr["raa_data"];
                tal = new double[bytesArr.Length / 8];

                for (int i = 0, j = 0; i < bytesArr.Length; i += 8, j++)
                    tal[j] = BitConverter.ToDouble(bytesArr, i);
            }
            for (int i = 0; i < tal.Length; i++)
            {
                ecg.Add(new DTO_ECG(tal[i]));
            }
            connect.Close();
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