using System;
using HitchSapB1Lib.Enums;

namespace HitchSapB1Lib.Objects.Marketing
{
    public class Contact
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string Title { get; set; }
        public string MiddleName { get; set; }
        public string Position { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public bool Active { get; set; }
        public string Email { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string MobilePhone { get; set; }
        public string CityOfBirth { get; set; }
        public DateTime? DateOfBirth { get; set; }   
        public Gender? Gender { get; set; }
        public string PlaceOfBirth { get; set; }
        public string Profession { get; set; }
    }
}
