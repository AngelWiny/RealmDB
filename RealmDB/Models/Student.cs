using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealmDB.Models
{
    public class Student : RealmObject
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
