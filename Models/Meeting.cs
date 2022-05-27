using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VismaTask.Models
{
    internal class Meeting : IEquatable<Meeting>
    {
        public Meeting(string name, Person responsiblePerson, string description, MeetingCategory category, MeetingType type, DateTime startDate, DateTime endDate)
        {
            Name = name;
            ResponsiblePerson = responsiblePerson;
            Description = description;
            Category = category;
            Type = type;
            StartDate = startDate;
            EndDate = endDate;
            People = new List<Person>();
        }

        public List<Person> People { get; set; }
        public string Name { get; set; }
        public Person ResponsiblePerson { get; set; }
        public string Description { get; set; }
        public MeetingCategory Category { get; set; }
        public MeetingType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool Equals(Meeting? other)
        {
            if (other == null) return false;

            return other.Name == Name && other.StartDate == StartDate;
        }

        public override string ToString()
        {
            return $"{Name} by {ResponsiblePerson} {Description} {Category} {Type} {StartDate} {EndDate}";
        }
    }
}
