using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_Tim_Viec.Models
{
    public class dataRecord
    {

        public dataRecord() { }

        public dataRecord(string href, string name, string nameCompany, string salary, string country, string date)
        {
            this.href = href;
            this.name = name;
            this.nameCompany = nameCompany;
            this.salary = salary;
            this.country = country;
            this.date = date;
        }
        public string name { get; set; }
        public string salary { get; set; }
        public string country { get; set; }
        public string href { get; set; }
        public string nameCompany { get; set; }
        public string date { get; set; }
    }
}