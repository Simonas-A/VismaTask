using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VismaTask.Models;

namespace VismaTask
{
    public class CommandHandler
    {
        private readonly List<Meeting> meetings;
        private readonly string user;
        private readonly string path;
        private readonly string[] filters;

        public CommandHandler(string user, string path)
        {
            meetings = IOController.ReadMeetings(path);
            this.user = user;
            this.path = path;
            filters = new string[8];
        }

        public int HandleList(string args)
        {
            string[] values = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string[] filterHeaders = { "-d", "-pf", "-pl", "-c", "-t", "-s", "-e", "-n" };

            for (int i = 0; i < filters.Length; i++)
            {
                int index = Array.IndexOf(values, filterHeaders[i]);

                if (index < 0)
                {
                    filters[i] = "";
                    continue;
                }

                if (index < values.Length - 1)
                {
                    if (filterHeaders.Contains(values[index + 1]))
                    {
                        return -16;
                    }
                    DateTime dt;
                    if ((
                        (values[index] == "-s" || values[index] == "-e" ) &&
                        !DateTime.TryParse(values[index + 1], out dt)))
                    {
                        return -7;
                    }
                    
                    filters[i] = values[index + 1];
                }
                else
                {
                    return -16;
                }
            }

            return 5;
        }

        public string Handle(string command)
        {
            if (command == null)
            {
                return GetStatusMessage(-1);
            }

            string[] values = command.Split(' ', 2);
            string args = values.Length > 1 ? values[1] : "";
            int status;


            switch (values[0])
            {

                case "create":
                    
                    Meeting? meeting = HandleCreate(args, out status);

                    if (meeting != null)
                        meetings.Add(meeting);

                    break;

                case "delete":

                    status = HandleDelete(args, user);
                    break;

                case "add":

                    status = HandleAdd(args);
                    break;

                case "remove":

                    status = HandleRemove(args);
                    break;

                case "list":
                    status = HandleList(args);
                    break;

                case "help":
                    status = -15;
                    break;
                default:
                    status = -13;
                    break;
            }

            if (status > 0)
            {
                IOController.WriteMeetings(path, meetings);
                
            }

            return GetStatusMessage(status);
        }

        private string GetStatusMessage(int status)
        {
            switch (status)
            {
                case 0:
                    return "opa";
                case 1:
                    return "Meeting creation successful";
                case 2:
                    return "Meeting deletion successful";
                case 3:
                    return "Person added successfully";
                case 4:
                    return "Person removed successfully";
                case 5:
                    return FormatList();
                case -1:
                    return "Empty command";
                case -2:
                    return "Invalid meeting creation arguments";
                case -3:
                    return "Too few arguments";
                case -4:
                    return "Meeting not found";
                case -5:
                    return "You don't have permissions to delete this meeting";
                case -6:
                    return "Meeting deletion failed";
                case -7:
                    return "Invalid time format";
                case -8:
                    return "Person is busy at that time";
                case -9:
                    return "Meeting already contains this person";
                case -10:
                    return "Cannot remove responsible person from the meeting";
                case -11:
                    return "Invalid meeting category argument";
                case -12:
                    return "Invalid meeting type argument";
                case -13:
                    return "Invalid command";
                case -14:
                    return "Person was not found";
                case -15:
                    return Help();
                case -16:
                    return "Invalid filter arguments";
                case -17:
                    return "Ending time cannot be before starting time";
                default:
                    return "";
            }
        }

        private string Help()
        {
            return File.ReadAllText("..//..//..//help.txt");
        }

        private string FormatList()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Meeting meeting in meetings
                                                .Where(m => m.Description.Contains(filters[0]) &&
                                                (filters[1] == "" || m.ResponsiblePerson.FirstName.Equals(filters[1])) &&
                                                (filters[2] == "" || m.ResponsiblePerson.LastName.Equals(filters[2])) &&
                                                (filters[3] == "" || m.Category.ToString().Equals(filters[3])) &&
                                                (filters[4] == "" || m.Type.ToString().Equals(filters[4])) &&
                                                (filters[5] == "" || (m.StartDate - DateTime.Parse(filters[5])).TotalDays > 0 ) &&
                                                (filters[6] == "" || (DateTime.Parse(filters[6]) - m.EndDate).TotalDays > 0) &&
                                                (filters[7] == "" || (m.People.Count >= Convert.ToInt32(filters[7]))
                                                )))
            {
                sb.AppendLine(meeting.ToString());

                foreach (Person person in meeting.People)
                {
                    sb.AppendLine($"\t{person.ToString()}");
                }
            }
            return sb.ToString();
        }

        private int HandleRemove(string args)
        {
            string[] values = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (values.Length < 3)
                return -3;

            string meetingName = values[0];
            Person person = new Person(values[1], values[2]);
            Meeting? meeting = meetings.FirstOrDefault(m => m.Name == meetingName);

            if (meeting == null)
                return -4;

            if (meeting.ResponsiblePerson.Equals(person))
                return -10;

            return meeting.People.Remove(person) ? 4 : -14;
        }

        private int HandleAdd(string args)
        {
            string[] values = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (values.Length < 5)
                return -3;

            string meetingName = values[0];
            string firstName = values[1];
            string lastName = values[2];
            DateTime time;
            if (!DateTime.TryParse($"{values[3]} {values[4]}", out time))
                return -7;

            Person person = new Person(firstName, lastName);

            //Check if person is already attending another meeting at that time
            if (meetings.Any(m => 
                        m.People.Contains(person) &&
                        m.Name != meetingName &&
                        m.StartDate <= time &&
                        m.EndDate >= time))

                return -8;

            Meeting? meeting = meetings.FirstOrDefault(m => m.Name == meetingName);

            if (meeting == null)
                return -4;

            if (meeting.People.Contains(person))
                return -9;
            
            meeting.People.Add(person);
            return 3;
        }

        private int HandleDelete(string args, string user)
        {
            Meeting? meeting = meetings.FirstOrDefault(m => m.Name == args);
            if (meeting == null)
                return -4;

            if (user != $"{meeting.ResponsiblePerson.FirstName} {meeting.ResponsiblePerson.LastName}")
                return -5;

            return meetings.Remove(meeting) ? 2 : -6;
        }

        private Meeting? HandleCreate(string args, out int status)
        {
            string[] values = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (values.Length < 9)
            {
                status = -3;
                return null;
            }

            string name = values[0];
            string personName = values[1];
            string personLastName = values[2];

            int addition = 0;

            string description;
            string[] desc = args.Split('"', StringSplitOptions.RemoveEmptyEntries);
            if (desc.Length < 3)
                description = values[3];
            else
            {
                description = desc[1];
                addition = description.Count(d => d == ' ');
            }


            MeetingCategory category;

            switch ( values[4 + addition])
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
                    status = -11;
                    return null;

            }

            MeetingType type;

            switch (values[5 + addition])
            {
                case "Live":
                    type = MeetingType.Live;
                    break;

                case "InPerson":
                    type = MeetingType.InPerson;
                    break;

                default:
                    status = -12;
                    return null;

            }

            DateTime startDate;
            if (!DateTime.TryParse($"{values[6 + addition]} {values[7 + addition]}", out startDate))
            {
                status = -7;
                return null;
            }

            DateTime endDate;
            if (!DateTime.TryParse($"{values[8 + addition]} {values[9 + addition]}", out endDate))
            {
                status = -7;
                return null;
            }

            if ((endDate - startDate).TotalMinutes < 0)
            {
                status = -17;
                return null;
            }

            status = 1;
            return new Meeting(name, new Person(personName, personLastName), description, category, type, startDate, endDate);
        }
    }
}
