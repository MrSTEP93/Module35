using System.Collections.Generic;
using System;
using UnsocNetwork.Models;

namespace UnsocNetwork.Data
{
    public class UserGenerator
    {
        public readonly string[] maleNames = new string[] { "Алексей", "Борян", "Василий", "Игорь", "Даниил", "Сергей", "Евгений", "Григорий", "Витек", "Миха" };
        public readonly string[] femaleNames = new string[] { "Анна", "Мария", "Станислава", "Елена", "Юлия", "Настя" };
        public readonly string[] lastNames = new string[] { "Тестов", "Туголуков", "Потапов", "Шкуров", "Лысенков" };

        public List<User> Populate(int count)
        {
            var users = new List<User>();
            for (int i = 1; i < count; i++)
            {
                string firstName;
                var rand = new Random();

                var male = rand.Next(1, 2) == 1;

                var lastName = lastNames[rand.Next(0, lastNames.Length - 1)];
                if (male)
                {
                    firstName = maleNames[rand.Next(0, maleNames.Length - 1)];
                }
                else
                {
                    lastName += "a";
                    firstName = femaleNames[rand.Next(0, femaleNames.Length - 1)];
                }

                var item = new User()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    BirthDate = DateTime.Now.AddDays(-rand.Next(1, (DateTime.Now - DateTime.Now.AddYears(-25)).Days)),
                    Email = "test" + rand.Next(0, 1204) + "@test.com",
                };

                item.UserName = item.Email;
                item.PathToPhoto = "https://thispersondoesnotexist.com";

                users.Add(item);
            }

            return users;
        }
    }
}
