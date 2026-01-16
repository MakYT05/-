using System;
using System.Collections.Generic;
using InsuranceApp.Models;
using Microsoft.Data.Sqlite;

namespace InsuranceApp.Services
{
    public static class DatabaseService
    {
        private static readonly string connectionString = "Data Source=insurance.db;";

        public static void Initialize()
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();

            string createClients = @"CREATE TABLE IF NOT EXISTS Клиенты (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        ФИО TEXT NOT NULL,
                                        Телефон TEXT NOT NULL
                                    );";
            string createProperties = @"CREATE TABLE IF NOT EXISTS Имущество (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Тип TEXT NOT NULL,
                                        Стоимость REAL NOT NULL,
                                        Адрес TEXT NOT NULL
                                    );";
            string createPolicies = @"CREATE TABLE IF NOT EXISTS Полисы (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        КлиентId INTEGER,
                                        ИмуществоId INTEGER,
                                        ДатаНачала TEXT,
                                        ДатаОкончания TEXT,
                                        Премия REAL,
                                        FOREIGN KEY(КлиентId) REFERENCES Клиенты(Id),
                                        FOREIGN KEY(ИмуществоId) REFERENCES Имущество(Id)
                                    );";
            string createRequests = @"CREATE TABLE IF NOT EXISTS Заявки (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                ФИО TEXT NOT NULL,
                                Телефон TEXT NOT NULL,
                                ТипИмущества TEXT NOT NULL,
                                Стоимость REAL NOT NULL,
                                Адрес TEXT NOT NULL,
                                Премия REAL NOT NULL,
                                Срок INTEGER NOT NULL
                            );";
            string createBookings = @"CREATE TABLE IF NOT EXISTS Бронирования (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            ФИО TEXT NOT NULL,
                            Телефон TEXT NOT NULL,
                            Дата TEXT NOT NULL,
                            Время TEXT NOT NULL,
                            Статус TEXT DEFAULT 'активна'
                        );";
            string createUsers = @"CREATE TABLE IF NOT EXISTS Пользователи (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Логин TEXT NOT NULL UNIQUE,
                                    Пароль TEXT NOT NULL,
                                    Роль TEXT NOT NULL DEFAULT 'user',
                                    ФИО TEXT NOT NULL
                                );";

            using var cmd = new SqliteCommand(createClients, conn);
            cmd.CommandText = createUsers;
            cmd.ExecuteNonQuery();
            cmd.CommandText = createProperties;
            cmd.ExecuteNonQuery();
            cmd.CommandText = createPolicies;
            cmd.ExecuteNonQuery();
            cmd.CommandText = createRequests;
            cmd.ExecuteNonQuery();
            cmd.CommandText = createBookings;
            cmd.ExecuteNonQuery();
        }

        public static void ЗарегистрироватьПользователя(string логин, string пароль, string фио, string роль = "user")
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = @"INSERT INTO Пользователи (Логин, Пароль, ФИО, Роль) 
                           VALUES (@login, @password, @fio, @role)";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@login", логин);
            cmd.Parameters.AddWithValue("@password", пароль);
            cmd.Parameters.AddWithValue("@fio", фио);
            cmd.Parameters.AddWithValue("@role", роль);
            cmd.ExecuteNonQuery();
        }

        public static (bool успех, string роль, string фио) ПроверитьАвторизацию(string логин, string пароль)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "SELECT Пароль, Роль, ФИО FROM Пользователи WHERE Логин = @login";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@login", логин);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string сохраненныйПароль = reader.GetString(0);
                string роль = reader.GetString(1);
                string фио = reader.GetString(2);
                
                if (сохраненныйПароль == пароль)
                {
                    return (true, роль, фио);
                }
            }
            return (false, "", "");
        }

        public static List<Пользователь> ПолучитьПользователей()
        {
            List<Пользователь> list = new();
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "SELECT * FROM Пользователи";
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Пользователь
                {
                    Id = reader.GetInt32(0),
                    Логин = reader.GetString(1),
                    Пароль = reader.GetString(2),
                    Роль = reader.GetString(3),
                    ФИО = reader.GetString(4)
                });
            }
            return list;
        }

        public static void ОбновитьРольПользователя(int id, string роль)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "UPDATE Пользователи SET Роль = @role WHERE Id = @id";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@role", роль);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public static void УдалитьПользователя(int id)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "DELETE FROM Пользователи WHERE Id = @id";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public static void ДобавитьБронирование(string фио, string телефон, string дата, string время)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = @"INSERT INTO Бронирования (ФИО, Телефон, Дата, Время) 
                           VALUES (@fio, @tel, @date, @time)";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@fio", фио);
            cmd.Parameters.AddWithValue("@tel", телефон);
            cmd.Parameters.AddWithValue("@date", дата);
            cmd.Parameters.AddWithValue("@time", время);
            cmd.ExecuteNonQuery();
        }

        public static List<Бронирование> ПолучитьБронирования()
        {
            List<Бронирование> list = new();
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "SELECT * FROM Бронирования ORDER BY Дата, Время";
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Бронирование
                {
                    Id = reader.GetInt32(0),
                    ФИО = reader.GetString(1),
                    Телефон = reader.GetString(2),
                    Дата = reader.GetString(3),
                    Время = reader.GetString(4),
                    Статус = reader.GetString(5)
                });
            }
            return list;
        }

        public static void УдалитьБронирование(int id)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "DELETE FROM Бронирования WHERE Id = @id";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public static void ДобавитьЗаявку(Заявка z)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = @"INSERT INTO Заявки (ФИО, Телефон, ТипИмущества, Стоимость, Адрес, Премия, Срок)
                           VALUES (@fio, @tel, @tip, @stoim, @adr, @prem, @srok)";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@fio", z.ФИО);
            cmd.Parameters.AddWithValue("@tel", z.Телефон);
            cmd.Parameters.AddWithValue("@tip", z.ТипИмущества);
            cmd.Parameters.AddWithValue("@stoim", z.Стоимость);
            cmd.Parameters.AddWithValue("@adr", z.Адрес);
            cmd.Parameters.AddWithValue("@prem", z.Премия);
            cmd.Parameters.AddWithValue("@srok", z.Срок);
            cmd.ExecuteNonQuery();
        }

        public static List<Заявка> ПолучитьЗаявки()
        {
            List<Заявка> list = new();
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "SELECT * FROM Заявки";
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Заявка
                {
                    Id = reader.GetInt32(0),
                    ФИО = reader.GetString(1),
                    Телефон = reader.GetString(2),
                    ТипИмущества = reader.GetString(3),
                    Стоимость = reader.GetDecimal(4),
                    Адрес = reader.GetString(5),
                    Премия = reader.GetDecimal(6),
                    Срок = reader.GetInt32(7)
                });
            }
            return list;
        }

        public static void УдалитьЗаявку(int id)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "DELETE FROM Заявки WHERE Id = @id";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public static void ПринятьЗаявку(int id)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();

            Заявка z = null!;
            string sqlGet = "SELECT * FROM Заявки WHERE Id = @id";
            using (var cmd = new SqliteCommand(sqlGet, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    z = new Заявка
                    {
                        Id = reader.GetInt32(0),
                        ФИО = reader.GetString(1),
                        Телефон = reader.GetString(2),
                        ТипИмущества = reader.GetString(3),
                        Стоимость = reader.GetDecimal(4),
                        Адрес = reader.GetString(5),
                        Премия = reader.GetDecimal(6),
                        Срок = reader.GetInt32(7)
                    };
                }
            }

            if (z == null)
                throw new Exception("Заявка не найдена");

            string sqlClient = "INSERT INTO Клиенты (ФИО, Телефон) VALUES (@fio, @tel); SELECT last_insert_rowid();";
            int clientId;
            using (var cmd = new SqliteCommand(sqlClient, conn))
            {
                cmd.Parameters.AddWithValue("@fio", z.ФИО);
                cmd.Parameters.AddWithValue("@tel", z.Телефон);
                clientId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            string sqlProperty = "INSERT INTO Имущество (Тип, Стоимость, Адрес) VALUES (@tip, @stoim, @adr); SELECT last_insert_rowid();";
            int propertyId;
            using (var cmd = new SqliteCommand(sqlProperty, conn))
            {
                cmd.Parameters.AddWithValue("@tip", z.ТипИмущества);
                cmd.Parameters.AddWithValue("@stoim", z.Стоимость);
                cmd.Parameters.AddWithValue("@adr", z.Адрес);
                propertyId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            string sqlPolicy = @"INSERT INTO Полисы (КлиентId, ИмуществоId, ДатаНачала, ДатаОкончания, Премия) 
                                 VALUES (@c, @p, @start, @end, @prem)";
            using (var cmd = new SqliteCommand(sqlPolicy, conn))
            {
                cmd.Parameters.AddWithValue("@c", clientId);
                cmd.Parameters.AddWithValue("@p", propertyId);
                cmd.Parameters.AddWithValue("@start", DateTime.Today.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@end", DateTime.Today.AddDays(z.Срок).ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@prem", z.Премия);
                cmd.ExecuteNonQuery();
            }
        
            УдалитьЗаявку(id);
        }

        public static void ОтклонитьЗаявку(int id)
        {
            УдалитьЗаявку(id);
        }

        public static void ДобавитьКлиента(string фио, string телефон)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "INSERT INTO Клиенты (ФИО, Телефон) VALUES (@fio, @tel)";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@fio", фио);
            cmd.Parameters.AddWithValue("@tel", телефон);
            cmd.ExecuteNonQuery();
        }

        public static List<Клиент> ПолучитьКлиентов()
        {
            List<Клиент> list = new();
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "SELECT * FROM Клиенты";
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Клиент
                {
                    Id = reader.GetInt32(0),
                    ФИО = reader.GetString(1),
                    Телефон = reader.GetString(2)
                });
            }
            return list;
        }

        public static void УдалитьКлиента(int id)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();

            string deletePolicies = "DELETE FROM Полисы WHERE КлиентId = @id";
            using var cmd1 = new SqliteCommand(deletePolicies, conn);
            cmd1.Parameters.AddWithValue("@id", id);
            cmd1.ExecuteNonQuery();

            string sql = "DELETE FROM Клиенты WHERE Id = @id";
            using var cmd2 = new SqliteCommand(sql, conn);
            cmd2.Parameters.AddWithValue("@id", id);
            cmd2.ExecuteNonQuery();
        }

        public static void ДобавитьИмущество(string тип, decimal стоимость, string адрес)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "INSERT INTO Имущество (Тип, Стоимость, Адрес) VALUES (@t, @v, @a)";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@t", тип);
            cmd.Parameters.AddWithValue("@v", стоимость);
            cmd.Parameters.AddWithValue("@a", адрес);
            cmd.ExecuteNonQuery();
        }

        public static List<Имущество> ПолучитьИмущество()
        {
            List<Имущество> list = new();
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "SELECT * FROM Имущество";
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Имущество
                {
                    Id = reader.GetInt32(0),
                    Тип = reader.GetString(1),
                    Стоимость = reader.GetDecimal(2),
                    Адрес = reader.GetString(3)
                });
            }
            return list;
        }

        public static void УдалитьИмущество(int id)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();

            string deletePolicies = "DELETE FROM Полисы WHERE ИмуществоId = @id";
            using var cmd1 = new SqliteCommand(deletePolicies, conn);
            cmd1.Parameters.AddWithValue("@id", id);
            cmd1.ExecuteNonQuery();

            string sql = "DELETE FROM Имущество WHERE Id = @id";
            using var cmd2 = new SqliteCommand(sql, conn);
            cmd2.Parameters.AddWithValue("@id", id);
            cmd2.ExecuteNonQuery();
        }

        public static void ДобавитьПолис(int клиентId, int имуществоId, decimal премия, DateTime начало, DateTime конец)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = @"INSERT INTO Полисы (КлиентId, ИмуществоId, ДатаНачала, ДатаОкончания, Премия) 
                           VALUES (@c, @p, @s, @e, @pr)";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@c", клиентId);
            cmd.Parameters.AddWithValue("@p", имуществоId);
            cmd.Parameters.AddWithValue("@s", начало.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@e", конец.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@pr", премия);
            cmd.ExecuteNonQuery();
        }

        public static List<Полис> ПолучитьПолисы()
        {
            List<Полис> list = new();
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "SELECT * FROM Полисы";
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Полис
                {
                    Id = reader.GetInt32(0),
                    КлиентId = reader.GetInt32(1),
                    ИмуществоId = reader.GetInt32(2),
                    ДатаНачала = DateTime.Parse(reader.GetString(3)),
                    ДатаОкончания = DateTime.Parse(reader.GetString(4)),
                    Премия = reader.GetDecimal(5)
                });
            }
            return list;
        }

        public static void УдалитьПолис(int id)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "DELETE FROM Полисы WHERE Id = @id";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}