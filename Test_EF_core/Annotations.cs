//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Test_EF_core
//{
//    // Анотація являє собою налаштування співставлення моделей і таблиць з допомогою атрибутів
//    // Більшість класів анотації знаходяться в просторі імен System.ComponentModel.DataAnnotations

//    // НАЛАШТУВАННЯ КЛЮЧА
//    // Для встановлення властивості в якості первинного ключа використовується атрибут [Key]
//    public class Phone0
//    {
//        // Тепер властивість Ident буде розглядатись в якості первинного ключа
//        [Key]
//        public int Ident { get; set; }
//        public string Name { get; set; }

//        // Щоб встановити ключ в якості ідентифікатора(щоб поле автоматично заповнбвалось послідовними даними), можна використати атрибут
//        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public int _Ident { get; set; }

//        // Якщо властивість, позначена атрибутом Key, являє собою тип int, то до неї атрибут DatabaseGenerated(DatabaseGeneratedOption.Identity)
//        // - буде застосований автоматично, і його можна не вказувати явно
//    }

//    // АТРИБУТ Required
//    // - вказує, що дна властивість обов'язкова для встановлення, тобто буде мати визначення NOT NULL в БД
//    public class Phone1
//    {
//        [Key]
//        public int Ident { get; set; }
//        [Required]
//        public string Name { get; set; }
//    }

//    // АТРИБУТ MaxLength и MinLength
//    // - встановлюють максимальну і мінімальну кількість символів в рядковій властивості
//    public class Phone2
//    {
//        [Key]
//        public int Ident { get; set; }
//        [MaxLength(20)]
//        public string Name { get; set; }
//    }

//    // АТРИБУТ NotMapped
//    // - за замовчуванням всі публічні властивості співставляються з відповідними колонками в таблиці. Проте така поведінка не завжди є необхідною
//    // Інколи вимагається, навпаки, виключити певну властивість, щоб для неї не створювалась колонка в таблиці. І для таких цілей існує атрибут NotMapped
//    // Щоб задіяти атрибут NotMapped, необхідно підключити простір імен System.ComponentModel.DataAnnotations.Schema
//    public class Phone3
//    {
//        [Key]
//        public int Ident { get; set; }
//        [Required]
//        public string Name { get; set; }
//        [NotMapped]
//        public int Discount { get; set; }
//    }

//    // Співставленя з табличкою і колонкою
//    // Entity Framework при створенні і співставлення таблички і колонки використовує імена моделей і їх властивості
//    // Проте ми можемо перевизначити цю поведінку за допомоою атрибутів Table і Column
//    [Table("Mobiles")]
//    public class Phone4
//    {
//        public int Id { get; set; }
//        [Column("ModelName")]
//        public string Name { get; set; }
//        // Тепер сутність Phone4 буде співставлятись з таблицею Mobiles, а властивість Name зі колонкою ModelName
//    }

//    // Встановлення зовнішнього ключа - Foreign Key
//    // - щоб встановити зовнішній ключ для звзяку з іншою сутністю, використовується атрибут ForeignKey
//    public class Phone5
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }

//        public int? CompId { get; set; }
//        [ForeignKey("CompId")]
//        public Company0 Company { get; set; }
//        // В даному випадку зовнішнім ключем для звязку з моделлю Company буде служити властивість CompId
//    }

//    public class Company0
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }
//    }

//    // НАЛАШТУВАННЯ ІНДЕКСА
//    // Для встановлення індекса для колонки для відповідної властивості моделі застосовується атрибут [Index]
//    public class Phone6
//    {
//        [Index]
//        public int Id { get; set; }
//        public string Name { get; set; }
//    }

//    // АТРИБУТ ConcurrencyCheck
//    // - дозволяє вирішити проблему паралелізму, коли з одним і тим же записом в таблиці можуть працювати одночасно декілька користувачів
//    public class Phone7
//    {
//        public int Id { get; set; }
//        [ConcurrencyCheck]
//        public string Name { get; set; }


//    }
//    // Може бути така ситуація, коли одночасно обо'є користувачів намагаються змінити значення властивості Nameusing(FluentContext db = new FluentContext())
//    //{
//    //    Phone phone = db.Phones.Find(1);
//    //    phone.Name = "Nokia N9";
//    //    db.SaveChanges();
//    //}
//    // В звичайному режимі Entity Framework при оновленні переглядає Id, і якщо Id запису в таблиці співпадає з Id моделі що передається, то рядок в таблиці оновлюється.
//    // При використанні атрибута ConcurrencyCheck, EF перевіряє не лише на Id, але і на вихідне значення властивості Name.\
//    // І якщо воно співпадає з тим, що наявне в таблиці, то запис оновлюється.
//    // Проте, якщо не співпадає (тобто хтось вже встиг його замінити), то EF генерує вийняток DbUpdateConcurrencyException
//}
