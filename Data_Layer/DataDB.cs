using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;


namespace Data_Layer
{
   class DataDB : IData
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

         SqlDataReader rdr, rdr2;
         string selectString = "select * from gyldigelogin where Brugernavn= '" + username + "'";
         string selectString2 = "select * from gyldigelogin where Adgangskode= '" + pw + "'";

         using (SqlCommand cmd = new SqlCommand(selectString, OpenConnectionST))
         {
            rdr = cmd.ExecuteReader();
         }
         if (rdr.Read())
         {
            using (SqlCommand cmd = new SqlCommand(selectString2, OpenConnectionST))
            {
               rdr2 = cmd.ExecuteReader();
            }

            if (rdr2.Read())
            {
               result = true;
            }
            else
            {
               result = false;
            }


         }
         else
            result = false;

         return
            result;
      }


   }
}

