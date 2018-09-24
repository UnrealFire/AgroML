using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using ASPMVC_AgroML.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ASPMVC_AgroML
{
    public class Tools
    {
        public static Forecast MeteoToForecast(Meteo meteo)
        {
            var getUri = "https://ibm-watson-ml.mybluemix.net/v3/identity/token";

            var urpass = "64edb5a5-dc9a-4090-9a9a-cc34c8a1de8d" + ":" + "fd161844-67f7-4e8d-8514-4128b36b9a14";
            var bytesText = System.Text.Encoding.UTF8.GetBytes(urpass);
            var payloadBase64 = Convert.ToBase64String(bytesText);
            var header = $"Basic {payloadBase64}";

            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(getUri);
            WebHeaderCollection myWebHeaderCollection = wrGETURL.Headers;
            myWebHeaderCollection.Add("Authorization:" + header);


            HttpWebResponse myHttpWebResponse = (HttpWebResponse) wrGETURL.GetResponse();

            Stream objStream;
            objStream = myHttpWebResponse.GetResponseStream();

            StreamReader objReader = new StreamReader(objStream);

            var sLine = objReader.ReadLine();
            objReader.Close();

            dynamic token = JObject.Parse(sLine);




            var postUri =
                "https://ibm-watson-ml.mybluemix.net/v3/wml_instances/8ce11eb2-d41f-4805-8de6-a4ed9ffb6691/published_models/f3fc9e53-ee82-4ffd-bc00-2620ea00d6d8/deployments/95909d84-1c78-436b-8b76-ea9215813dde/online";

            var jsonPayload = new
            {
                fields = new[] {"temp", "hudm"},
                values = new[] {new[] {meteo.Temp, meteo.Hum}}
            };

            var payloadSerialized = JsonConvert.SerializeObject(jsonPayload);


            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(postUri);
            WebHeaderCollection myWebHeaderCollection2 = request.Headers;
            request.ContentType = "application/json;charset=UTF-8";
            myWebHeaderCollection2.Add("Authorization:" + "Bearer " + token.token);
            request.Method = "POST";
            request.ContentLength = payloadSerialized.Length;

            Console.WriteLine(myWebHeaderCollection2.AllKeys[0]);
            Console.WriteLine();

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(payloadSerialized);

            }

            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            Stream stream2 = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream2);

            string data = reader.ReadToEnd();

            reader.Close();
            stream2.Close();

            dynamic dataParsed = JObject.Parse(data);
            var probs = dataParsed.values[0][4];

            

            var forecast = new Forecast()
            {
                BurRz = probs[1]*100,
                ChernBakt = probs[4]*100,
                KarlGol = probs[3]*100,
                KornGnil = probs[5]*100,
                None = probs[0]*100,
                PilnGol = probs[2]*100
            };

            return forecast;
        }

    }
}