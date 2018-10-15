using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;


    namespace LiveState
    {
        public class GZipWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                HttpWebRequest webrequest = (HttpWebRequest)base.GetWebRequest(address);
                webrequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                return webrequest;
            }
        }
    }

