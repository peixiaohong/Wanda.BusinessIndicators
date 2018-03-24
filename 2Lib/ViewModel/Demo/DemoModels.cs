using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LJTH.HR.KPI.ViewModel.Demo
{

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public int Gender { get; set; }
        public string GenderText
        {
            get
            {
                return Gender == 0 ? "Male" : "Female";
            }
        }
        public Person(string name, int age, int gender)
        {
            this.Name = name; this.Age = age; this.Gender = gender;
        }
        public static List<Person> Persons = new List<Person>(){
                                           new Person("Alice",10,1),
                                           new Person("Bob",16,0),
                                           new Person("Clair",12,1),
                                           new Person("Dun",13,0),
                                           new Person("Frank",14,1),
                                           new Person("Grace",15,0) 
                                           };
    }
}
