using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VismaTask.Models;

namespace VismaTask
{
    internal class CommandHandler
    {
        private readonly List<Meeting> meetings;

        public CommandHandler()
        {
            meetings = new List<Meeting>();
        }

        public string FormatList()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Meeting meeting in meetings)
            {
                sb.AppendLine(meeting.ToString());

                foreach (Person person in meeting.People)
                {
                    sb.AppendLine($"\t{person.ToString()}");
                }
            }
            return sb.ToString();
        }

        public int Handle(string command, string user)
        {
            if (command == null)
            {
                return -1;
            }

            string[] values = command.Split(' ', 2);

            switch (values[0])
            {
                case "create":
                    
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

                    return HandleDelete(values[1], user);

                case "add":

                    return HandleAdd(values[1]);

                case "remove":
                    
                    return HandleRemove(values[1]);

                case "list":
                    return 5;

                default:
                    return 0;

            }
        }

        private int HandleRemove(string args)
        {
            string[] values = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (values.Length < 3)
                return -1;

            string meetingName = values[0];
            Person person = new Person(values[1], values[2]);
            Meeting meeting = meetings.First(m => m.Name == meetingName);

            if (meeting == null)
                return -1;

            if (meeting.ResponsiblePerson.Equals(person))
                return -1;

            return meeting.People.Remove(person) ? 0 : -1;
        }

        private int HandleAdd(string args)
        {
            string[] values = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (values.Length < 4)
                return -1;

            string meetingName = values[0];
            string firstName = values[1];
            string lastName = values[2];
            DateTime time;
            if (!DateTime.TryParse(values[3], out time))
                return -2;

            Person person = new Person(firstName, lastName);

            //Check if person is already attending another meeting at that time
            if (meetings.Any(m => 
                        m.People.Contains(person) &&
                        m.Name != meetingName &&
                        m.StartDate <= time &&
                        m.EndDate >= time))

                return -1;

            Meeting meeting = meetings.First(m => m.Name == meetingName);

            if (meeting == null)
                return -1;

            if (meeting.People.Contains(person))
                return -1;
            
            meeting.People.Add(person);
            return 0;
        }

        private int HandleDelete(string args, string user)
        {
            Meeting meeting = meetings.First(m => m.Name == args);
            if (meeting == null)
                return -2;

            if (user != $"{meeting.ResponsiblePerson.FirstName} {meeting.ResponsiblePerson.LastName}")
                return -3;

            return meetings.Remove(meeting) ? 2 : -1;
        }

        private Meeting HandleCreate(string args)
        {
            string[] values = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (values.Length < 7)
            {
                return null;
            }

            string name = values[0];
            string personName = values[1];
            string personLastName = values[2];

            string description;
            string[] desc = args.Split('"', StringSplitOptions.RemoveEmptyEntries);
            if (desc.Length < 3)
                description = values[3];
            else
                description = desc[1];


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
