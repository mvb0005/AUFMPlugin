using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using AUFMPlugin.Properties;

namespace AUFMPlugin
{
    class Library
    {
        private static String url = "https://aufm-backend.herokuapp.com/api/";
        private static readonly WebClient client = new WebClient();

        public static String getHttpRequest(String location)
        {
            try
            {

                String responseString = client.DownloadString(new Uri(Settings.Default.Url + location));
                return responseString;
            }
            catch (WebException)
            {
                return "error";
            }

        }

        public static String postHttpRequest(String location, String json)
        {
            try
            {
                client.Headers.Add("Content-type", "application/json");
                var response = client.UploadString(new Uri(Settings.Default.Url + location), json);
                client.Headers.Clear();
                return response;
            }
            catch (WebException e)
            {
                return e.Message;
            }
        }

        public static void login(string User, string Pass)
        {
            
        }

        public static String putHttpRequest(string location, string data)
        {
            try
            {
                var response = client.UploadString(Settings.Default.Url + location, "PUT", data);
                return response;
            }
            catch (WebException e)
            {
                return e.Message;
            }
        }

    }

    public class Protocol
    {
        public int protocol_id { get; set; }
        public string value { get; set; }
    }

    public class Building
    {
        public int building_id { get; set; }
        public string name { get; set; }

        public override bool Equals(object obj)
        {
            return ((Building) obj).name == this.name;
        }
    }

    public class PartProtocol
    {
        public int element_id { get; set; }
        public int part_id { get; set; }
        public Protocol[] protocols { get; set; }
    }

    public class Part
    {
        public int building_id { get; set; }
        public int element_id { get; set; }
        public String part_name { get; set; }
    }
}
