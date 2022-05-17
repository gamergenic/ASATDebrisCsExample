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
using Newtonsoft.Json;
using System.Linq;

#nullable enable

namespace Debris
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("debris.exe");
                Console.WriteLine("\nusage:");
                Console.WriteLine("debris [-u <username>] [-p <password>] <objectid>");
                Console.WriteLine("\nalternatively, environment vars SPACETRACK_USER and SPACETRACK_PASSWORD can supply credentials.");
                return;
            }

            string? userRef = Environment.GetEnvironmentVariable("SPACETRACK_USER");
            string? passwordRef = Environment.GetEnvironmentVariable("SPACETRACK_PASSWORD");

            for (int i = 0; i < args.Length - 1; ++i)
            {
                switch(args[i])
                {
                    case "-u":
                        userRef = args[i + 1];
                        break;
                    case "-p":
                        passwordRef = args[i + 1];
                        break;
                }
            }

            if(string.IsNullOrEmpty(userRef) || string.IsNullOrEmpty(passwordRef))
            {
                Console.WriteLine(userRef == null ? "Missing user" : "Missing password");
                return;
            }

            string ObjectId = args[args.Length - 1];

            Console.WriteLine("Querying SpaceTrack for Object {0}...", ObjectId);

            SpaceTrack spaceTrack = new SpaceTrack(userRef, passwordRef);

            string jsonResponse = spaceTrack.GetSpaceTrackDebrisData(ObjectId);

            if(jsonResponse == null)
            {
                Console.WriteLine("Request failed... unauthorized user/password?");
                return;
            }

            GPObject[] deserializedResponse;
            try
            {
                deserializedResponse = JsonConvert.DeserializeObject<GPObject[]>(jsonResponse);
            }
            catch(Exception e)
            {
                Console.WriteLine("Could not parse response: {0}", e.ToString());
                return;
            }

            int cataloged = deserializedResponse.Count();
            int remaining = deserializedResponse.Count(o => o.DECAY_DATE == null);

            Console.WriteLine("...{0} in catalog, {1} remaining.", cataloged, remaining);
        }
    }
}
