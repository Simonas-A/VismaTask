using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VismaTask.Models;
using System.Text.Json;

namespace VismaTask
{
    internal static class IOController
    {
        internal static List<Meeting> ReadMeetings(string path)
        {
            if (!File.Exists(path))
                return new List<Meeting>();

            List<Meeting>? meetings = JsonSerializer.Deserialize<List<Meeting>>(File.ReadAllText(path));

            return meetings == null ? new List<Meeting>() : meetings;
        }

        internal static void WriteMeetings(string path, List<Meeting> meetings)
        {
            string json = JsonSerializer.Serialize(meetings);
            File.WriteAllText(path, json);
        }
    }
}
