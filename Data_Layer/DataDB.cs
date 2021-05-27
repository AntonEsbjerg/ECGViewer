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
        private SqlConnection OpenConnectionST
        {
            get
            {
                var con = new SqlConnection(@"Data Source=DESKTOP-PDTN5JP\SQLEXPRESS;Initial Catalog=LokalDatabase;User ID=LokalDatabase;Password=LokalDatabase;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
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
            string selectString = "select * from db_owner.gyldigelogin where Brugernavn= '" + username + "'and Adgangskode= '" + pw + "'";


            using (SqlCommand cmd = new SqlCommand(selectString, OpenConnectionST))
            {
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    result = true;
                }
                else result = false;
            }
            OpenConnectionST.Close();
            return result;
        }
        private SqlConnection connect
        {
            get
            {
                var con = new SqlConnection(@"Data Source=st-i4dab.uni.au.dk;Initial Catalog=ST2PRJ2OffEKGDatabase;Integrated Security=False;User ID=ST2PRJ2OffEKGDatabase;Password=ST2PRJ2OffEKGDatabase;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False");
                con.Open();
                return con;
            }
        }
        public DTO_lokalinfo downloadLokalinfo()
        {
            SqlDataReader rdr; bool stemi_suspected; DateTime dato; int ekgmaaleid; int antalmaalinger; string sfp_maaltagerfornavn;
            string sfp_maaltagerefternavn; string sfp_maaltagermedarbjdnr; string sfp_mt_kommentar; string sfp_mt_org; string borger_fornavn;
            string borger_efternavn; string borger_cprnr; int ekgdataid; int samplerate_hz; int interval_sec; int interval_min; bool doctor_overview;
            string dataformat; string bin_eller_tekst; string maaleformat_type; DateTime start_tid; string kommentar; string maaleenhed_identifikation;
            List<double> tal = new List<double>(); byte[] bytesArr = new byte[800]; List<DTO_ECG> lokalECG= new List<DTO_ECG>();
            SqlDataReader rdr1;
            DTO_lokalinfo lokalinfo;
            string insertStringParam = ("Select * from db_owner.EKGMAELING where stemi_paavist IS NULL and ekgmaaleid=(SELECT max(ekgmaaleid) FROM db_owner.EKGMAELING)");

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

                    string insertStringParam1 = ("Select * from db_owner.EKGDATA where ekgmaaleid= "+ekgmaaleid);
                    using (SqlCommand command = new SqlCommand(insertStringParam1, OpenConnectionST))
                    {
                        rdr1 = command.ExecuteReader();
                        if (rdr1.Read())
                        {
                            bytesArr = (byte[])rdr1["raa_data"];
                            for (int i = 0, j = 0; i < bytesArr.Length; i += 8, j++)
                            {
                                tal.Add(BitConverter.ToDouble(bytesArr, i));
                            }
                            foreach (var item in tal)
                            {
                                lokalECG.Add(new DTO_ECG(item));
                            }
                            ekgdataid = Convert.ToInt32(rdr1["ekgdataid"]);
                            samplerate_hz = Convert.ToInt32(rdr1["samplerate_hz"]);
                            interval_sec = Convert.ToInt32(rdr1["interval_sec"]);
                            interval_min = Convert.ToInt32(rdr1["interval_min"]);
                            dataformat = Convert.ToString(rdr1["data_format"]);
                            bin_eller_tekst = Convert.ToString(rdr1["bin_eller_tekst"]);
                            maaleformat_type = Convert.ToString(rdr1["maaleformat_type"]);
                            start_tid = Convert.ToDateTime(rdr1["start_tid"]);
                            kommentar = Convert.ToString(rdr1["kommentar"]);
                            ekgmaaleid = Convert.ToInt32(rdr1["ekgmaaleid"]);
                            maaleenhed_identifikation = Convert.ToString(rdr1["maalenehed_identifikation"]);
                            doctor_overview = true;
                            lokalinfo = new DTO_lokalinfo(stemi_suspected, dato, ekgmaaleid, antalmaalinger, sfp_maaltagerfornavn, sfp_maaltagerefternavn, sfp_maaltagermedarbjdnr,
                                sfp_mt_kommentar, sfp_mt_org, borger_fornavn, borger_efternavn, borger_cprnr, ekgdataid, lokalECG, samplerate_hz, interval_sec, interval_min, dataformat,
                                bin_eller_tekst, maaleformat_type, start_tid, kommentar, maaleenhed_identifikation,doctor_overview);
                            OpenConnectionST.Close();
                            return lokalinfo;
                        }
                    }
                }
            }
            OpenConnectionST.Close();
            return lokalinfo = new DTO_lokalinfo();
        }
        public List<DTO_ECG> getECGData(String måleID)
        {
            List<DTO_ECG> ecg = new List<DTO_ECG>();
            SqlDataReader rdr;
            byte[] bytesArr = new byte[8];
            double[] tal;
            string selectString = ("Select raa_data From EKGDATA where ekgmaaleid= " + Convert.ToInt32(måleID) );
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

            string readStringParam = ("Select borger_cprnr,ekgmaaleid from EKGMAELING where borger_cprnr IS NOT NULL");
            using (SqlCommand cmd = new SqlCommand(readStringParam, connect))
            {
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    socsecNB = Convert.ToString(rdr["borger_cprnr"]);
                    måleID = Convert.ToString(rdr["ekgmaaleid"]);
                    tempsocsecNB.Add(new DTO_id(måleID, socsecNB));
                }
            }
            connect.Close();
            return tempsocsecNB;
        }
        public void uploadSTEMI(DTO_lokalinfo nySTEMI)
        {
            DTO_ECG[] tal;
            double[] ecgVoltage=new double[500];
            string insertStringDOEDBData = "INSERT INTO EKGDATA (raa_data,samplerate_hz,interval_sec,interval_min,data_format," +
                "bin_eller_tekst,maaleformat_type,start_tid,kommentar,ekgmaaleid,maalenehed_identifikation) " +
                "VALUES (@raa_data, @samplerate_hz, @interval_sec, @interval_min, @data_format, @bin_eller_tekst, " +
                "@maaleformat_type,@start_tid,@kommentar,@ekgmaaleid,@maalenehed_identifikation)";
            string insertStringDOEDBMaeling= "INSERT INTO EKGMAELING (dato,antalmaalinger,sfp_maaltagerfornavn,sfp_maltagerefternavn," +
                "sfp_maaltagermedarbjnr, sfp_mt_org,sfp_mt_kommentar,sfp_ansvfornavn,sfp_ansvefternavn,sfp_ansvrmedarbjnr,sfp_ans_org," +
                "sfp_anskommentar,borger_fornavn,borger_efternavn,borger_beskrivelse,borger_cprnr) " +
                "VALUES (@dato,@antalmaalinger,@sfp_maaltagerfornavn, @sfp_maltagerefternavn,@sfp_maaltagermedarbjnr," +
                "@sfp_mt_org,@sfp_mt_kommentar, @sfp_ansvfornavn,@sfp_ansvefternavn,@sfp_ansvrmedarbjnr,@sfp_ans_org," +
                "@sfp_anskommentar,@borger_fornavn,@borger_efternavn,@borger_beskrivelse,@borger_cprnr)";
            using (SqlCommand command = new SqlCommand(insertStringDOEDBData, connect))
            {
                tal=nySTEMI._lokalECG.ToArray();
                for (int i = 0; i < tal.Length; i++)
                {
                    ecgVoltage[i] = tal[i].ECGVoltage;
                }
                command.Parameters.AddWithValue("@raa_data", ecgVoltage.SelectMany(value => BitConverter.GetBytes(value)).ToArray());
                command.Parameters.AddWithValue("@samplerate_hz", nySTEMI._samplerate_hz);
                command.Parameters.AddWithValue("@interval_sec", nySTEMI._interval_sec);
                command.Parameters.AddWithValue("@interval_min", nySTEMI._interval_min);
                command.Parameters.AddWithValue("@data_format", nySTEMI._dataformat);
                command.Parameters.AddWithValue("@bin_eller_tekst", nySTEMI._bin_eller_tekst);
                command.Parameters.AddWithValue("@maaleformat_type", nySTEMI._maaleformat_type);
                command.Parameters.AddWithValue("@start_tid", nySTEMI._start_tid);
                command.Parameters.AddWithValue("@kommentar", nySTEMI._kommentar);
                command.Parameters.AddWithValue("@ekgmaaleid", nySTEMI._ekgmaaleid);
                command.Parameters.AddWithValue("@maalenehed_identifikation", nySTEMI._maaleenhed_identifikation);
                command.ExecuteNonQuery();
            }
            using (SqlCommand command = new SqlCommand(insertStringDOEDBMaeling, connect))
            {
                command.Parameters.AddWithValue("@dato", nySTEMI._dato);
                command.Parameters.AddWithValue("@antalmaalinger", nySTEMI._antalmaalinger);
                command.Parameters.AddWithValue("@sfp_maaltagerfornavn", nySTEMI._sfp_maaltagerfornavn);
                command.Parameters.AddWithValue("@sfp_maltagerefternavn", nySTEMI._sfp_maaltagerefternavn);
                command.Parameters.AddWithValue("@sfp_maaltagermedarbjnr", nySTEMI._sfp_maaltagermedarbjdnr);
                command.Parameters.AddWithValue("@sfp_mt_org", nySTEMI._sfp_mt_org);
                command.Parameters.AddWithValue("@sfp_mt_kommentar", nySTEMI._sfp_mt_kommentar);
                command.Parameters.AddWithValue("@sfp_ansvfornavn", "");
                command.Parameters.AddWithValue("@sfp_ansvefternavn", "");
                command.Parameters.AddWithValue("@sfp_ansvrmedarbjnr", "");
                command.Parameters.AddWithValue("@sfp_ans_org", "");
                command.Parameters.AddWithValue("@sfp_anskommentar", "");
                command.Parameters.AddWithValue("@borger_fornavn", nySTEMI._borger_fornavn);
                command.Parameters.AddWithValue("@borger_efternavn", nySTEMI._borger_efternavn);
                command.Parameters.AddWithValue("@borger_beskrivelse", "");
                command.Parameters.AddWithValue("@borger_cprnr", nySTEMI._borger_cprnr);
                command.ExecuteNonQuery();
            }

            SqlCommand command1 = new SqlCommand("UPDATE db_owner.EKGMAELING SET stemi_paavist=@værdi where ekgmaaleid=@ekgmaaleid", OpenConnectionST);
            command1.Parameters.AddWithValue("@værdi", 1);
            command1.Parameters.AddWithValue("@ekgmaaleid", nySTEMI._ekgmaaleid);
            command1.ExecuteNonQuery();
            OpenConnectionST.Close();
        }
        public void uploadNoSTEMI(DTO_lokalinfo nyNoSTEMI)
        {
            SqlCommand command1 = new SqlCommand("UPDATE db_owner.EKGMAELING SET stemi_paavist=@værdi where ekgmaaleid=@ekgmaaleid", OpenConnectionST);
            command1.Parameters.AddWithValue("@værdi", 0);
            command1.Parameters.AddWithValue("@ekgmaaleid", nyNoSTEMI._ekgmaaleid);
            command1.ExecuteNonQuery();
            OpenConnectionST.Close();
        }
    }
}
