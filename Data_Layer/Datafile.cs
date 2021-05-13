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

      DataDB dbref;
      public Datafile()
      {
         dbref = new DataDB();

      }


      public bool isUserRegistered(String username, String pw)
      {
         if (dbref.isUserRegistered(username, pw) == true)
         {
            return
               true;
         }
         else
            return
               false;



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
