﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

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
                String responseString = client.DownloadString(new Uri(url + location));
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
                var response = client.UploadString(new Uri(url + location), json);
                return response;
            }
            catch (WebException e)
            {
                return e.Message;
            }
        }

        public static String putHttpRequest(string location, string data)
        {
            try
            {
                var response = client.UploadString(url + location, "PUT", data);
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
    }
}