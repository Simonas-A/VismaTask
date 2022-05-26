using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VismaTask.Models;

namespace VismaTask
{
    internal static class CommandHandler
    {
        public static int Handle(string command, List<Meeting> meetings, string user)
        {
            if (command == null)
            {
                return -1;
            }

            string[] values = command.Split(' ', 1);

            switch (values[0])
            {
                case "create":
                    if (values.Length < 8)
                        return -1;
                    Meeting meeting = HandleCreate(values[1]);

                    if (meeting == null)
                    {
                        return -1;
                    }
                    
                    meetings.Add(HandleCreate(values[1]));

                    return 1;

                case "delete":
                    if (values.Length < 2)
                        return -1;
                    return 2;

                case "add":
                    if (values.Length < 3)
                        return -1;
                    return 3;

                case "remove":
                    if (values.Length < 3)
                        return -1;
                    return 4;

                case "list":
                    return 5;

                default:
                    return 0;

            }

        }
        public static Meeting HandleCreate(string args)
        {
            string[] values = args.Split(' ');
            if (values.Length < 7)
            {
                return null;
            }

            string name = values[0];
            string personName = values[1];
            string personLastName = values[2];

            StringBuilder description = new StringBuilder();

            int offset;

            for (offset = 0; offset < length; offset++)
            {

            }


            MeetingCategory category;

            switch ( values[4])
            {
                case "TeamBuilding":
                    category = MeetingCategory.TeamBuilding;
                    break;

                case "CodeMonkey":
                    category = MeetingCategory.CodeMonkey;
                    break;

                case "Hub":
                    category = MeetingCategory.Hub;
                    break;

                case "Short":
                    category = MeetingCategory.Short;
                    break;

                default:
                    return null;

            }

            MeetingType type;

            switch (values[5])
            {
                case "Live":
                    type = MeetingType.Live;
                    break;

                case "InPerson":
                    type = MeetingType.InPerson;
                    break;

                default:
                    return null;

            }

            DateTime startDate;
            if (!DateTime.TryParse(values[6],out startDate))
            {
                return null;
            }

            DateTime endDate;
            if (!DateTime.TryParse(values[7], out endDate))
            {
                return null;
            }

            return new Meeting(name, new Person(personName, personLastName), description, category, type, startDate, endDate);
        }
    }
}
