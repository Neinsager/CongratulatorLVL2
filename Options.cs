using System;
using System.Collections.Generic;
using System.Linq;

namespace CongratulatorLVL2
{
    internal static class Options
    {
        private static readonly DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
        private static readonly int daysInCurrentYear = new DateOnly(currentDate.Year, 12, 31).DayOfYear;
        /// <summary>
        /// Возвращает слово в падеже, зависимом от заданного числа.
        /// </summary>
        private static string DecliningTime(int number, string nominativ, string genetiv, string plural)
        {
            var titles = new[] { nominativ, genetiv, plural };
            var cases = new[] { 2, 0, 1, 1, 1, 2 };
            return number + " " + titles[number % 100 > 4 && number % 100 < 20 ? 2 : cases[(number % 10 < 5) ? number % 10 : 5]];
        }
        /// <summary>
        /// Посчитать количество дней до дня рождения.
        /// </summary>
        private static int GetDaysToBirthday(Person person)
        {

            var birthdayDay = new DateOnly(currentDate.Year, person.BirthdayDate.Month, person.BirthdayDate.Day);
            //Проверка было ли день рождение в текущем году.
            if (currentDate > birthdayDay) birthdayDay.AddYears(1);
            return (daysInCurrentYear - currentDate.DayOfYear + birthdayDay.DayOfYear) % daysInCurrentYear;
        }
        /// <summary>
        /// Проверка ведденной даты.
        /// </summary>
        private static DateOnly CheckCorrectDate()
        {
            var dateResult = new DateOnly();
            while (!(DateOnly.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", out dateResult)
                    && dateResult.Year - currentDate.Year < 0))
            {
                Console.WriteLine("Неправильная дата!");
            }
            return dateResult;
        }
        /// <summary>
        /// Поиск человека если такой есть в списке.
        /// </summary>
        private static Person FindPerson(List<Person> persons, string name, DateOnly date)
            => persons.FirstOrDefault(p => p.Name == name && p.BirthdayDate == date);
        /// <summary>
        /// Добавить данные о дне рождении человека в список.
        /// </summary>
        public static void AddNewPerson()
        {
            Console.WriteLine("Введите новое имя:");
            var name = Console.ReadLine();
            Console.WriteLine("Введите дату рождения формата дд.мм.гггг:");
            var date = CheckCorrectDate();
            using (ApplicationContext db = new ApplicationContext())
            {
                var persons = db.Persons.ToList();
                if (persons.Contains(FindPerson(persons, name, date)))
                    Console.WriteLine("Человек с такими данными уже есть в списке!");
                else
                {
                    db.Persons.Add(new Person { Name = name, BirthdayDate = date });
                    db.SaveChanges();
                    Console.WriteLine("Данные о человеке были добавлены!\n");
                }
            }
        }
        /// <summary>
        /// Удалить данные о дне рождении человека из списка.
        /// </summary>
        public static void RemovePerson()
        {
            Console.WriteLine("Введите имя человека которого нужно удалить из списка:");
            var name = Console.ReadLine();
            Console.WriteLine("Введите дату рождения формата дд.мм.гггг данного человека:");
            var date = CheckCorrectDate();

            using (ApplicationContext db = new ApplicationContext())
            {
                var persons = db.Persons.ToList();
                var person = FindPerson(persons, name, date);
                if (!persons.Contains(person))
                {
                    Console.WriteLine("Человека с таким данными нет в списке! Проверьте корректность ввода имени и даты!\n");
                }
                else
                {
                    db.Persons.Remove(person);
                    db.SaveChanges();
                    Console.WriteLine("Данные об этом человеке успешно удалены из списка!\n");
                }
            }
        }
        /// <summary>
        /// Изменение данных о дне рождении человека.
        /// </summary>
        public static void ChangePerson()
        {
            using (var db = new ApplicationContext())
            {
                var persons = db.Persons.ToList();
                Console.WriteLine("Введите имя которое нужно изменить:");
                var name = Console.ReadLine();
                Console.WriteLine("Введите дату рождения формата дд.мм.гггг соотвествующего человека:");
                var date = CheckCorrectDate();
                Console.WriteLine($"Что нужно изменить?" +
                    $"\nВведите 1 если хотите изменить имя;" +
                    $"\nВведите 2 если хотите изменить дату рождения.");
                var choose = 0;
                while (!(int.TryParse(Console.ReadLine(), out choose) && choose == 1 || choose == 2))
                {
                    Console.WriteLine("Вы ввели недопустимое значение, повторите попытку:");
                }
                if (!persons.Contains(FindPerson(persons, name, date)))
                {
                    Console.WriteLine("Человека с такими данными нет в списке!\n");
                }
                else
                {
                    foreach (var person in persons)
                    {
                        if (person.Name == name && person.BirthdayDate.Equals(date))
                        {
                            if (choose == 1)
                            {
                                Console.WriteLine("Введите новое имя:");
                                person.Name = Console.ReadLine();
                                db.Persons.Update(person);
                                db.SaveChanges();
                                Console.WriteLine("Имя было изменено успешно.\n");
                            }
                            if (choose == 2)
                            {
                                Console.WriteLine("Введите новую дату рождения формата дд.мм.гггг:");
                                person.BirthdayDate = CheckCorrectDate();
                                db.Persons.Update(person);
                                db.SaveChanges();
                                Console.WriteLine("Дата рождения была изменена успешно!\n");
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Отобразить дни рождения сегодня.
        /// </summary>
        public static void ShowTodaysBirthdays()
        {
            using (var db = new ApplicationContext())
            {
                var persons = db.Persons.ToList();
                Console.WriteLine("Сегодня день рождения празднует:");
                foreach (var person in persons)
                {
                    if (person.BirthdayDate.Day == currentDate.Day && person.BirthdayDate.Month == currentDate.Month)
                    {
                        Console.WriteLine(person + ", сегодня исполняется " +
                            $"{DecliningTime(currentDate.Year - person.BirthdayDate.Year, "год", "года", "лет")}!");
                    }
                }
                Console.WriteLine();
            }
        }
        /// <summary>
        /// Отобразить будущие дни рождения.
        /// </summary>
        public static void ShowNextBirthdays()
        {
            using (var db = new ApplicationContext())
            {
                var persons = db.Persons.ToList();

                Console.WriteLine("Следующий день рождения празднует:");
                var personsBufferList = new List<Person>();
                var daysSpan = 30;
                foreach (var person in persons)
                {
                    var daysToBirthday = GetDaysToBirthday(person);
                    if (daysToBirthday > 0 && daysToBirthday <= daysSpan)
                    {
                        personsBufferList.Add(person);
                    }
                }
                if (personsBufferList.Count == 0)
                {
                    Console.WriteLine("Дат с ближайшим днем рождения не оказалось.");
                }
                personsBufferList.Sort((firstPerson, secondPerson) => GetDaysToBirthday(firstPerson).CompareTo(GetDaysToBirthday(secondPerson)));
                foreach (var person in personsBufferList)
                {
                    Console.WriteLine(person
                                    + $", исполнится {DecliningTime(currentDate.Year - person.BirthdayDate.Year, "год", "года", "лет")}"
                                    + $" через {DecliningTime(GetDaysToBirthday(person), "день", "дня", "дней")}!");
                }
                Console.WriteLine();
            }
        }
        /// <summary>
        /// Отобразить весь список дней рождения.
        /// </summary>
        public static void PrintListOfPerson()
        {
            using (var db = new ApplicationContext())
            {
                var persons = db.Persons.ToList();
                persons.Sort((person1, person2) => person1.BirthdayDate.CompareTo(person2.BirthdayDate));
                Console.WriteLine("Список всех добавленных дней рождения:\n");
                foreach (var person in persons)
                {
                    Console.WriteLine(person);
                }
                Console.WriteLine();
            }
        }
        /// <summary>
        /// Отобразить список возможных действий.
        /// </summary>
        public static void PrintChoiseMenu()
        {
            Console.WriteLine("Выберите действие:\n" +
                "1 - Добавить данные о дне рождении;\n" +
                "2 - Удалить данные о дне рождении;\n" +
                "3 - Изменить данные о дне рождении;\n" +
                "4 - Отобразить дни рождения сегодня;\n" +
                "5 - Отобразить будущие дни рождения;\n" +
                "6 - Отобразить все дни рождения;\n" +
                "0 - Завершение работу программы.");
        }
    }
}