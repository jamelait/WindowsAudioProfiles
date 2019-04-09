using LiteDB;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsAudioProfiles.Entity;

namespace WindowsAudioProfiles.CommandLine
{
    class Program
    {
        public static readonly string ConnectionString = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WindowsAudioProfiles", @"WindowsAudioProfiles.db");

        static void Main(string[] args)
        {
            if (!File.Exists(ConnectionString))
            {
                Console.WriteLine("Database not found at");
                Console.WriteLine(ConnectionString);
                Console.ReadLine();
                return;
            }

            if (args.Length == 1)
            {
                switch (args[0])
                {
                    case "list":
                        ListProfiles();
                        break;
                    default:
                        PrintUsage();
                        break;
                }
            }
            else if (args.Length == 2)
            {
                switch (args[0])
                {
                    case "set":
                        var name = args[1];
                        Console.WriteLine("Searching " + name);
                        var profile = Find(name);
                        if (profile == null)
                        {
                            Console.WriteLine("Not found.");
                        }
                        else
                        {
                            Console.WriteLine("Found: " + profile);
                            Console.WriteLine("Applying...");
                            ApplyProfile(profile);
                            Console.WriteLine("Done.");
                            return;
                        }
                        break;
                    default:
                        PrintUsage();
                        break;
                }
            }
            else
            {
                PrintUsage();
            }

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage: audiobalance.exe list");
            Console.WriteLine("Usage: audiobalance.exe set NAME");
        }

        static void ListProfiles()
        {
            Console.WriteLine("\tLIST");
            using (var db = new LiteDatabase(ConnectionString))
            {
                var collection = db.GetCollection<Profile>("profiles");
                var results = collection.FindAll().ToList();
                foreach (var profile in results)
                {
                    Console.WriteLine(profile);
                }
                Console.WriteLine("\nTotal: " + results.Count);
            }
        }

        static Profile Find(string name)
        {
            using (var db = new LiteDatabase(ConnectionString))
            {
                // Get customer collection
                var collection = db.GetCollection<Profile>("profiles");

                // Use Linq to query documents
                var profile = collection.FindOne(x => x.Name.ToLowerInvariant().Equals(name.ToLowerInvariant()));
                return profile;
            }
        }

        public static void ApplyProfile(Profile p)
        {
            MMDeviceEnumerator devEnum = new MMDeviceEnumerator();

            MMDevice defaultDevice = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Communications);

            defaultDevice.AudioEndpointVolume.Channels[0].VolumeLevelScalar = p.Left;
            defaultDevice.AudioEndpointVolume.Channels[1].VolumeLevelScalar = p.Right;
        }
    }
}
