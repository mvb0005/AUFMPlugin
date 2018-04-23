using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using AUFMPlugin.Properties;
using Newtonsoft.Json;

namespace AUFMPlugin
{
    class Library
    {
        
        private static WebClient client = new WebClient();
        private static String cookie = Settings.Default.Cookie;
        public static bool LoggedIn = true;

        public static String getHttpRequest(String location)
        {
            try
            {
                client.Headers.Add(HttpRequestHeader.Cookie, cookie);
                String responseString = client.DownloadString(new Uri(Settings.Default.Url + location));
                return responseString;
            }
            catch (Exception e)
            {
                if (e.Message == "The remote server returned an error: (401) Unauthorized.")
                {
                    LoggedIn = false;
                    Settings.Default.Cookie = "";
                    System.Windows.Forms.MessageBox.Show("Login Required");
                }
                return "error";
            }

        }

        public static String postHttpRequest(String location, String json)
        {
            try
            {
                client.Headers.Add(HttpRequestHeader.Cookie, cookie);
                client.Headers.Add("Content-type", "application/json");
                var response = client.UploadString(new Uri(Settings.Default.Url + location), json);
                client.Headers.Clear();
                return response;
            }
            catch (Exception e)
            {
                if (e.Message == "The remote server returned an error: (401) Unauthorized.")
                {
                    LoggedIn = false;
                    Settings.Default.Cookie = "";
                    System.Windows.Forms.MessageBox.Show("Login Required");
                }
                return e.Message;
            }
        }

        public static String login(String User, String Pass)
        {
            try
            {
                client.Headers.Add("Content-type", "application/json");
                User u = new User();
                u.email = User;
                u.password = Pass;
                var url = Settings.Default.Url + "/api/login";
                var response = client.UploadString(new Uri(url), JsonConvert.SerializeObject(u).ToString());
                cookie = client.ResponseHeaders["Set-Cookie"].ToString();
                Settings.Default.Cookie = cookie;
                Settings.Default.Save();
                LoggedIn = true;
                return "Login Successful";
            } catch (Exception)
            {
                Settings.Default.Cookie = "";
                cookie = "";
                Settings.Default.Save();
                client.Headers.Clear();
                LoggedIn = false;
                return "Login Failed";

            }
        }

        public static String putHttpRequest(string location, string data)
        {
            try
            {
                var response = client.UploadString(Settings.Default.Url + location, "PUT", data);
                return response;
            }
            catch (Exception e)
            {
                if (e.Message == "The remote server returned an error: (401) Unauthorized.")
                {
                    LoggedIn = false;
                    Settings.Default.Cookie = "";
                    System.Windows.Forms.MessageBox.Show("Login Required");
                }
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

    public class User
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}
