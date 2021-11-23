using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Test_EF_core
{
    // Модель описана класом - яка буде являти собою таблицю в базі даних
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public ICollection<Car> Cars { get; set; }
    }

    // Модель описана класом - яка буде являти собою таблицю в бвзі даних
    public class Car
    {
        [Key]
        public int Id { get; set; }
        public string Brend { get; set; }
        public int YearManufacture { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }

    // Клас що наслідується від EF класу DbContext - визначає контекст даних, використовується для взаємодії з базою даних
    public class Context : DbContext
    {
        // Являє собою своєрідну звязуючу ланку, між моделлю і таблицею в базі даних - являє собою набір об'єктів (Entity)  певного типу які зберігаються у відповідні таблиці
        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }

        // В конструкторі ми встановлюємо перевірку, якщо база даних не існує, тоді її створити
        public Context() => Database.EnsureCreated();

        // Перевизначаєме метод OnConfiguring - який встановлює підключення до бази даних
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CarsAndUsers;Trusted_Connection=True;");
    }

    public class Repository
    {
        private Context db;

        // В своїй операції з базою даних, являють собою CRUD операції (Create, Read, Update, Delete)
        // ТОБТО - СТВОРЕННЯ, ОТРИМАННЯ, ОНОВЛЕННЯ, ВИДАЛЕННЯ - Entity Framework Core - дозволяє легко виконувати ці дії

        // СТВРЕННЯ 
        public void AddToUser(User user)
        {
            using (db = new Context())
            {
                db.Users.Add(user);
                //db.Users.AddRange(user1, user2);
                db.SaveChanges();

                Console.WriteLine("User - доданий успiшно");
            }
        }

        // ОТРИМАННЯ
        public ICollection<User> GetAllUsersList()
        {
            using (db = new Context())
            {
                return db.Users.ToList();
            }
        }

        //ОНОВЛЕННЯ
        public void UpdateUser(int id, User newUser)
        {
            using (db = new Context())
            {
                User temprUser = db.Users.FirstOrDefault(u => u.Id == id);

                if (temprUser != null)
                {
                    temprUser.Name = newUser.Name;
                    temprUser.Age = newUser.Age;

                    db.Users.Update(temprUser);
                    //db.Users.UpdateRange(user1, user2);
                    db.SaveChanges();

                    Console.WriteLine($"Оновлення usera пiд id - {id}, пройшло успiшно");
                }
                else
                {
                    throw new Exception($"User за таким id - {id} не iснує");
                }
            }
        }

        // ВИДАЛЕННЯ
        public void DeleteFromUser(int id)
        {
            using (db = new Context())
            {
                User userForDelete = db.Users.FirstOrDefault(u => u.Id == id);

                if (userForDelete != null)
                {
                    db.Users.Remove(userForDelete);
                    //db.Users.RemoveRange(user1, user2);
                    db.SaveChanges();

                    Console.WriteLine($"Видалення usera пiд id - {id}, пройшло успiшно");
                }
                else
                {
                    throw new Exception($"User за таким id - {id} не iснує");
                }
            }
        }

        // LINQ to Entities
        public void LINQtoEntities()
        {
            // LINQ to Entities
            // - простий і інтуєтивно зрозумілий підхід для отримання даних з допомогою виразів, які по формі дуже близькі до виразів мови SQL
            // Хоча при роботі з базою даних ми оперуємо запитати LINQ, проте база розуміє лише запити на мову SQL
            // Тому між LINQ to Entities і базою даних існує спеціальний провідник, який дозволяє їм взаємодіяти
            // Цим провідником є провайдер EntityClient - він створює інтерфейс для взаємодії з провадером ADO.NET для SQL Serverа

            // Для початку взаємодії з базою даних створюється об'єкт EntityConnection
            // Через об'єкт EntityCommand  він відправляє запити, а з допомогою об'єкта EntityDataReader зчитує видобуті з бази дані
            // Проте нам не потрібно напряму зв'язуватись з цими об'єктами, фреймворк зробить усе це за нас
            // Наша ж робота в більшості зводиться до того, щоб написати запити до бази даних з допомогою LINQ

            // Роботу з LINQ to Entities, будемо розглядати на нашій існуючій базі, зі зв'язком один - до - багатьох

            // Вибір одного об'єкту за індексом
            //using (Context db = new Context())
            //{
            //    int userId = 4;
            //    var user = db.Users.Find(userId);
            //    Console.WriteLine($"Користувая: {user.Name} - {user.Age} рокiв");

            //    // Як альтернативу можемо використати метод FirstOrDefault/First
            //    var userF = db.Users.FirstOrDefault(x => x.Id == userId);
            //    Console.WriteLine($"Користувая: {userF.Name} - {userF.Age} рокiв");
            //}

            // Тепер зробимо проєкцію. Припустимо, нам потрібно додати в результат вибірки назви брендів машин, і рік випуску всіх машин, які відповідають нашому певному користувачу
            // Ми можемо використати метод Include - для підєднання до об'єкту зв'язаних даних із іншої таблиці
            //using (Context db = new Context())
            //{
            //    int userId = 3;
            //    var users = db.Users.Include(u => u.Cars).FirstOrDefault(x => x.Id == userId);
            //    users.Cars.ToList().ForEach(x => Console.WriteLine($"Користувая: {x.User.Name} - {x.User.Age} рокiв. \nАвтомобiль - {x.Id}. {x.Brend} - {x.YearManufacture}\n"));
            //}

            // - проте не завжди потрібні всі властивості вибраних об'єктів. В такому випадку ми можемо використати метод Select 
            // - для проєкції видобутих даних на нови тип
            //using (Context db = new Context())
            //{
            //    var cars = db.Cars.Select(car => new
            //    {
            //        Name = car.Brend,
            //        Age = DateTime.Now.Year - car.YearManufacture,
            //        Owner = car.User.Name
            //    });

            //    cars.ToList().ForEach(x => Console.WriteLine($"{x.Name}, вiк - {x.Age}. Власник - {x.Owner}"));
            //}

            // Умови, Сортування - даних
            //using (Context db = new Context())
            //{
            //    var result = db.Cars.Where(x => x.YearManufacture > 2010).OrderByDescending(x => x.YearManufacture);
            //    result.ToList().ForEach(x => Console.WriteLine($"Рiк виготовлення - {x.YearManufacture}, Автомобiль - {x.Brend}"));
            //}

            // Групування - даних (По назві бренду)
            //using (Context db = new Context())
            //{
            //    var result = db.Cars.AsEnumerable().GroupBy(x => x.Brend);
            //    result.ToList().ForEach(x => Console.WriteLine($"Автомобiль - {x.Key}"));
            //}

            // Об'єднання таблиць
            // Використання методу Join. 
            // В даному випадку таблиця Users i Cars має спільний критерій - id користувачів, по якому можна провести об'єднання таблиць
            //using (Context db = new Context())
            //{
            //    var cars = db.Cars.Join(db.Users,   // другий набір
            //        c => c.UserId,                  // властивість-селектор об'єкта з першого набору
            //        u => u.Id,                      // властивість-селектор об'єкта з другого набору
            //        (c, u) => new                   // результат
            //        {
            //            Name = c.Brend,
            //            Age = DateTime.Now.Year - c.YearManufacture,
            //            Owner = u.Name
            //        });

            //    cars.ToList().ForEach(x => Console.WriteLine($"{x.Name}, вiк - {x.Age}. Власник - {x.Owner}"));
            //}

            // Ряд методів LINQ дозволяє працювати з результатами вибірки як з множиною
            // Виконують операції на об'єднання, накладання, і відмінність двох вибірок

            // Для об'єднання двох вибірок використовується метод Union
            // Об'єднаємо дві таблиці і виведемо на екран разом імя користувача і імя бренду автомобіля
            using (Context db = new Context())
            {
                var result = db.Cars.Select(c => new { Name = c.Brend })
                    .Union(db.Users.Select(u => new { Name = u.Name }));

                result.ToList().ForEach(x => Console.WriteLine(x.Name));
            }

            // Для пошуку елементі які існують в обох таблицях використовується метод - Intersect
            //using (Context db = new Context())
            //{
            //    var cars = db.Cars.Where(c => c.YearManufacture < 2010)
            //        .Intersect(db.Cars.Where(c => c.Brend.Contains("Opel")));

            //    cars.ToList().ForEach(x => Console.WriteLine(x.Brend));
            //}

            // Відмінність - якщо нам необхідно знайти елементи першої вибірки, які відсутні у другі вибірці, виукористовуємо метод Except
            //using (Context db = new Context())
            //{
            //    var selector1 = db.Cars.Where(c => c.YearManufacture > 2009);
            //    var selector2 = db.Cars.Where(c => c.Brend.Contains("Opel"));
            //    var cars = selector1.Except(selector2);

            //    cars.ToList().ForEach(x => Console.WriteLine(x.Brend));
            //}
        }

    }

    class Program
    {

        static void Main()
        {
            Repository repository = new Repository();

            // СТВРЕННЯ
            //User user1 = new User() { Name = "Ivan", Age = 32 };
            //User user2 = new User() { Name = "Alex", Age = 23 };
            //User user3 = new User() { Name = "Alex", Age = 24 };
            //User user4 = new User() { Name = "Alex", Age = 25 };
           // repository.AddToUser(user1);
            //repository.AddToUser(user2);
            //repository.AddToUser(user3);
            //repository.AddToUser(user4);

            // ОТРИМАННЯ
            //repository.GetAllUsersList().ToList().ForEach(x => Console.WriteLine($"{x.Id}.{x.Name} - {x.Age}"));

            // ОНОВЛЕННЯ
            //User newUser = new User() { Name = "Roman", Age = 21 };
            //repository.UpdateUser(4, newUser);

            // ВИДАЛЕННЯ
            //repository.DeleteFromUser(1);


            // ==================================================================================================================
            repository.LINQtoEntities();
        }
    }
}
