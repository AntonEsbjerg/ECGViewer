using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using DTO;


namespace Data_Layer
{
    public class DataDB : IData
    {
        const String db = "F21ST2ITS2au675718";
        private SqlConnection OpenConnectionST
        {
            get
            {
                var con = new SqlConnection("Data Source=st-i4dab.uni.au.dk;Initial Catalog=" + db + ";integrated Security=false;User ID=" + db + ";Password=" + db + ";Connect Timeout=30;Encrypt=False;TrustServerCertificate=False");
                con.Open();
                return con;
            }
        }
        public DataDB()
        {

        }

        public bool isUserRegistered(String username, String pw)
        {
            bool result;
         SqlDataReader rdr;
         string selectString = "select * from gyldigelogin where Brugernavn= '" + username + "'and Adgangskode= '" + pw + "'";


         using (SqlCommand cmd = new SqlCommand(selectString, OpenConnectionST))
         {
            rdr = cmd.ExecuteReader();
         }
         if (rdr.Read())
         {
               result = true;
                return con;
            }
        }
        public List<DTO_lokalinfo> downloadLokalinfo()
        {
            SqlDataReader rdr; bool stemi_suspected; DateTime dato; int ekgmaaleid; int antalmaalinger; string sfp_maaltagerfornavn;
            string sfp_maaltagerefternavn; string sfp_maaltagermedarbjdnr; string sfp_mt_kommentar; string sfp_mt_org; string borger_fornavn;
            string borger_efternavn; string borger_cprnr;
            List<DTO_lokalinfo> lokalinfo = new List<DTO_lokalinfo>();
            string insertStringParam = ("Select * from EKGMAELING where stemi_paavist IS NOT NULL");
            using (SqlCommand cmd = new SqlCommand(insertStringParam, OpenConnectionST))
            {
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    if (Convert.ToInt32(rdr["stemi_mistaenkt"]) == 1)
                    {
                        stemi_suspected = true;
                    }
                    else
                    {
                        stemi_suspected = false;
                    }
                    ekgmaaleid = Convert.ToInt32(rdr["ekgmaaleid"]);
                    dato = Convert.ToDateTime(rdr["dato"]);
                    antalmaalinger = Convert.ToInt32(rdr["antalmaalinger"]);
                    sfp_maaltagerfornavn = Convert.ToString(rdr["sfp_maaltagerfornavn"]);
                    sfp_maaltagerefternavn = Convert.ToString(rdr["sfp_maltagerefternavn"]);
                    sfp_maaltagermedarbjdnr = Convert.ToString(rdr["sfp_maaltagermedarbjnr"]);
                    sfp_mt_org = Convert.ToString(rdr["sfp_mt_org"]);
                    sfp_mt_kommentar = Convert.ToString(rdr["sfp_mt_kommentar"]);
                    borger_fornavn = Convert.ToString(rdr["borger_fornavn"]);
                    borger_efternavn = Convert.ToString(rdr["borger_efternavn"]);
                    borger_cprnr = Convert.ToString(rdr["borger_cprnr"]);
                    lokalinfo.Add(new DTO_lokalinfo(stemi_suspected, dato, ekgmaaleid, antalmaalinger, sfp_maaltagerfornavn, sfp_maaltagerefternavn, sfp_maaltagermedarbjdnr,
                       sfp_mt_kommentar, sfp_mt_org, borger_fornavn, borger_efternavn, borger_cprnr));
                }
            }
            OpenConnectionST.Close();
            return lokalinfo;
        }
        public List<DTO_ECG> getECGData(String måleID)
        {
            List<DTO_ECG> ecg = new List<DTO_ECG>();
            SqlDataReader rdr;
            byte[] bytesArr = new byte[8];
            double[] tal;
            string selectString = ("Select raa_data From EKGDATA where ekgmaaleid= " + måleID );
            using (SqlCommand cmd = new SqlCommand(selectString, connect))
            {
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                    bytesArr = (byte[])rdr["raa_data"];
                tal = new double[bytesArr.Length / 8];

                for (int i = 0, j = 0; i < bytesArr.Length; i += 8, j++)
                    tal[j] = BitConverter.ToDouble(bytesArr, i);
            }
            connect.Close();
            for (int i = 0; i < tal.Length; i++)
            {
                ecg.Add(new DTO_ECG(tal[i]));
            }
            return ecg;
        }
        public List<DTO_id> fillComboBox()
        {
            string socsecNB;
            string måleID;
            List<DTO_id> tempsocsecNB = new List<DTO_id>();
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

    }
}
