using System;

namespace CongratulatorLVL2
{
    internal class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly BirthdayDate { get; set; }

        public override string ToString()
        {
            return Name + ", дата рождения: " + BirthdayDate;
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
