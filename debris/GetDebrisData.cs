/* Copyright (C) 2022 Chuck Noble <chuck@gamergenic.com>
 * This work is free.  You can redistribute it and /or modify it under the
 * terms of the Do What The Fuck You Want To Public License, Version 2,
 * as published by Sam Hocevar.  See http://www.wtfpl.net/ for more details.
 *
 * This program is free software. It comes without any warranty, to
 * the extent permitted by applicable law. You can redistribute it
 * and/or modify it under the terms of the Do What The Fuck You Want
 * To Public License, Version 2, as published by Sam Hocevar. See
 * http://www.wtfpl.net/ for more details.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace Debris
{
    public enum RadarCrossSection
    {
        [EnumMember(Value = "SMALL")]
        Small,

        [EnumMember(Value = "MEDIUM")]
        Medium,

        [EnumMember(Value = "LARGE")]
        Large
    };

    public enum ObjectType
    {
        [EnumMember(Value = "PAYLOAD")]
        Payload,
        
        [EnumMember(Value = "ROCKET BODY")]
        RocketBody,

        [EnumMember(Value = "Debris")]
        Debris,

        [EnumMember(Value = "UKNOWN")]
        Unknown
    };

    public struct GPObject
    {
        public string CCSDS_OMM_VERS;       // "2.0"
        public string COMMENT;              // "GENERATED VIA SPACE-TRACK.ORG API",
        public DateTime CREATION_DATE;      // "2022-05-11T19:29:02"
        public string ORIGINATOR;           // "18 SPCS"
        public string OBJECT_NAME;          // "COSMOS 1408 DEB"
        public string OBJECT_ID;            // "1982-092AEN"
        public string CENTER_NAME;          // "EARTH"
        public string REF_FRAME;            // "TEME"
        public string TIME_SYSTEM;          // "UTC"
        public string MEAN_ELEMENT_THEORY;  // "SGP4"
        public DateTime EPOCH;              // "2022-05-11T11:58:19.460640"
        public double MEAN_MOTION;          // "15.96738939"
        public double ECCENTRICITY;         // "0.00379040"
        public double INCLINATION;          // "82.4273"
        public double RA_OF_ASC_NODE;       // "310.1579"
        public double ARG_OF_PERICENTER;    // "256.9185"
        public double MEAN_ANOMALY;         // "102.7859"
        public int EPHEMERIS_TYPE;          // "0"
        public string CLASSIFICATION_TYPE;  // "U"
        public int NORAD_CAT_ID;            // "50424"
        public int ELEMENT_SET_NO;          // "999"
        public int REV_AT_EPOCH;            // "2296"
        public double BSTAR;                // "0.01648000000000"
        public double MEAN_MOTION_DOT;      // "0.05601322"
        public double MEAN_MOTION_DDOT;     // "0.0570840000000"
        public double SEMIMAJOR_AXIS;       // "6661.610"
        public double PERIOD;               // "90.184"
        public double APOAPSIS;             // "308.726"
        public double PERIAPSIS;            // "258.225"
        public ObjectType OBJECT_TYPE;          // "DEBRIS"
        public RadarCrossSection? RCS_SIZE;             // "SMALL"
        public string COUNTRY_CODE;         // "CIS"
        public DateTime? LAUNCH_DATE;        // "1982-09-16"
        public string SITE;                 // "PKMTR"
        public DateTime? DECAY_DATE;         // "2022-05-12"
        public int FILE;                    // "3440740"
        public int GP_ID;                   // "203277183"
        public string TLE_LINE0;            // "0 COSMOS 1408 DEB"
        public string TLE_LINE1;            // "1 50424U 82092AEN 22131.49883635  .05601322  57084-1  16480-1 0  9995"
        public string TLE_LINE2;            // "2 50424  82.4273 310.1579 0037904 256.9185 102.7859 15.96738939 22969"
    };

    public class SpaceTrack
    {
        string User;
        string Password;

        public SpaceTrack(string user, string password)
        {
            User = user;
            Password = password;
        }

        public class WebClientEx : WebClient
        {
            // Create the container to hold all Cookie objects
            private CookieContainer _cookieContainer = new CookieContainer();

            // Override the WebRequest method so we can store the cookie
            // container as an attribute of the Web Request object
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest request = base.GetWebRequest(address);

                if (request is HttpWebRequest)
                    (request as HttpWebRequest).CookieContainer = _cookieContainer;

                return request;
            }
        }

        public string GetSpaceTrackDebrisData(string objectId)
        {
            // Note:
            // The recommended URL for retrieving the newest propagable element set for all on-orbit objects is:
            // https://www.space-track.org/basicspacedata/query/class/gp/decay_date/null-val/epoch/%3Enow-30/orderby/norad_cat_id/format/json

            string uriBase = "https://www.space-track.org";
            string requestController = "/basicspacedata";
            string requestAction = "/query";
            string predicateValues = "/class/gp/OBJECT_ID/~~" + objectId + "/orderby/DECAY_DATE%20desc/emptyresult/show";
            string request = uriBase + requestController + requestAction + predicateValues;

            // Create new WebClient object to communicate with the service
            using (var client = new WebClientEx())
            {
                // Store the user authentication information
                var data = new NameValueCollection
                {
                    { "identity", User },
                    { "password", Password },
                };

                byte[] requestResponse;
                try
                {
                    // Generate the URL for the API Query and return the response
                    var response2 = client.UploadValues(uriBase + "/ajaxauth/login", data);
                    requestResponse = client.DownloadData(request);
                }
                catch(Exception e)
                {
                    return e.ToString();
                }

                return (System.Text.Encoding.Default.GetString(requestResponse));
            }
        }   // GetSpaceTrackDebrisData()
    }   // SpaceTrack Class
}   // namespace Debris