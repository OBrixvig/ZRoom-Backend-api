using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZRoomLoginLibrary.Models
{
    public class User
    {
        private string _email;

        public string Name { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }

        public string Email
        {
            get => _email;
            set 
            {
                if (!Regex.IsMatch(value, "^[^@\\s]+@edu\\.zealand\\.dk$"))
                {
                    throw new ArgumentException("Invalid email, the email must end with @edu.zealand.dk");
                }

                _email = value;
            }
        }


        public User(string email, string name, int studentId, int classId)
        {
            Email = email;
            Name = name;
            StudentId = studentId;
            ClassId = classId;
        }
    }
}
