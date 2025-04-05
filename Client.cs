using System;
namespace Pract1
{
    public class Client
    {
        public int PersonalCode { get; set; }
        public string FullName { get; set; }
        public int ContactPhone { get; set; }
        public bool Privileged { get; set; }

        public Client(int personalCode, string fullName, int contactPhone, bool privileged = false)
        {
            PersonalCode = personalCode;
            FullName = fullName;
            ContactPhone = contactPhone;
            Privileged = privileged;
        }
    }
}

