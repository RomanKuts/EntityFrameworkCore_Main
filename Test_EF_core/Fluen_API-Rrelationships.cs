//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Test_EF_core
//{
//    // Розглянемо відношення між моделями у Fluent API

//    // Зв'язок ОДИН - до НУЛЯ, або - ОДНОГО ---------------------------------------------------------------------------------------------------------------------------
//    // При такому звязку для одної моделі наявність другої необов'язкове, наприклад
//    public class Phone
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }

//        public Company Company { get; set; }
//    }

//    public class Company
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }

//        public Phone BestSeller { get; set; }
//    }
//    // Смартфон обов'язково має виробника, проте виробник може не мати найбільш популярного по продажах телефона.
//    // Тобто в даному випадку зв'язок один до нуля чи до одного
//    // В Fluent API такий зв'язок налаштовується наступним чином

//    class Fluen_API_Rrelationships : DbContext
//    {
//        protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<Phone>()
//                .HasRequired(c => c.Company)
//                .WithOptional(c => c.BestSeller);

//            base.OnModelCreating(modelBuilder);
//        }
//        // Метод HasRequired() - вказує, що для сутності Phone обов'язково повинен бути вказана навігаційна властивість Company
//        // А метод WithOptional() - навпаки встановлює необов'язковий звязок між об'єктами попереднього виразу - Company і його властивістю BestSeller

//        // Зв'язок ОДИН - до - ОДНОГО ---------------------------------------------------------------------------------------------------------------------------------
//        // В даній конфігурації вже обидва об'єкти звязку повинні мати посилання один на одного
//        protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<Phone>()
//                .HasRequired(c => c.Company)
//                .WithRequiredPrincipal(c => c.BestSeller);

//            base.OnModelCreating(modelBuilder);
//        }
//        // Метод WithRequiredPrincipal() - налаштовує обов'язковий звязок і встановлює одну з сутностей в якості основної
//        // Так, в даному випадку основною сутністю встановлюється модель - Phone: WithRequiredPrincipal(c => c.BestSeller)
//        // А таблиця. на яку відображається модель Company, буде містити зовнішній ключ до таблиці Phones
//    }

//    // Звязок БАГТО - до - БАГАТЬОХ ---------------------------------------------------------------------------------------------------------------------------
//    // Для того щоб реалізувати цей звязок, нам необіхдно внести в зміни в наші моделі
//    // Припустимо в нас є ситуація, коли будь яка з моделей містить список об'єктів з іншої моделі
//    // Наприклад, компанія може виробляти декілька телефонів, а над одним телефоном можуть працювати декілька компаній
//    public class Phone
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }

//        public ICollection<Company> Companies { get; set; }

//        public Phone()
//        {
//            Companies = new List<Company>();
//        }
//    }

//    public class Company
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }

//        public ICollection<Phone> Phones { get; set; }

//        public Company()
//        {
//            Phones = new List<Phone>();

//        }
//    }

//    class Fluen_API_Rrelationships_1 : DbContext
//    {
//        protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        {
//            // Тоді налаштування зв'язку між ними буде вигяладати наступним чином
//            modelBuilder.Entity<Phone>()
//                .HasMany(p => p.Companies)
//                .WithMany(c => c.Phones);

//            base.OnModelCreating(modelBuilder);

//            // HasMany() - встановлює множинний звязок між об'єктом Phone і об'єктами Company
//            // А метод WithMany() - додає зворотній множинний звязок між об'єктом Company і об'єктами Phone

//            // В результаті при такій роботі з базою даних буде сформована третя таблиця посередник між двома сутностями
//            // Як ми знаємо, щоб створити звязок багато до багатьох, нам потірбна проміжна таблиця, так от це вона і буде

//            // Проте оскільки таблиця створиться автоматично, нас може не влаштовувати назва самої таблиці. або її колонок, тому це ми можемо змінити задавши явно

//            modelBuilder.Entity<Phone>()
//                .HasMany(p => p.Companies)
//                .WithMany(c => c.Phones)
//                .Map(m =>
//                {
//                    m.ToTable("MobileCompanies");
//                    m.MapLeftKey("MobileId");
//                    m.MapRightKey("CompanyId");
//                }); ;
//        }
//    }

//    // Звязок ОДИН - до - БАГАТЬОХ ---------------------------------------------------------------------------------------------------------------------------
//    // При звязку один до багатьох, одна модель може посилатись на множину об'єктів з іншої моделі
//    // Наприклад одна компанія виробляє безліч телефонів

//    // Для цього нам також необхідно внести зміни в класи моделей, і вони будуть виглядати наступним чином
//    public class Phone
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }

//        public Company Company { get; set; }
//    }

//    public class Company
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }
//        public ICollection<Phone> Phones { get; set; }
//        public Company()
//        {
//            Phones = new List<Phone>();
//        }
//    }

//    // А сам звязок прописаний за допомогою Fluent API буде виглядати так
//    class Fluen_API_Rrelationships_2 : DbContext
//    {
//        protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<Company>()
//                .HasMany(p => p.Phones)
//                .WithRequired(p => p.Company);
//        }
//    }
//    // Метод HasMany() - встановлює множинний звязок між об'єктом Company і об'єктом Phone
//    // А метод WithRequired() вимагає обовязково встановити властивість Company у класу Phone
//    // Після генерації таблиць, таблиця для моделей Phone буде містити колонку-зовнішній ключ Company_Id для звзяку з таблицею Companies

//    // НАЛАШТУВАННЯ ЗОВНІШНЬОГО КЛЮЧА ---------------------------------------------------------------------------------------------------------------------------
//    // Так буває, що нас може не влаштовувати назва колонки і зовнішнього ключа, що нам генерує за замовчуванням EF
//    // З допомогою метода HasForeignKey() - ми можемо перевизначити дію за замовчуванням
//    // Для цього визначимо властивість, яка буде представляти зовнішній ключ

//    public class Phone
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }

//        public Company Company { get; set; }
//        public int Manufacturer { get; set; }
//    }

//    // Тепер налаштуємо властивість Manufacturer  в якості зовнішнього ключа до таблиці Companies
//    class Fluen_API_Rrelationships_3 : DbContext
//    {
//        protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<Company>()
//                .HasMany(p => p.Phones)
//                .WithRequired(p => p.Company)
//                .HasForeignKey(s => s.Manufacturer);
//        }
//    }
//}


