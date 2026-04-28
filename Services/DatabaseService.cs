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
                                    Телефон TEXT NOT NULL,
                                    Паспорт TEXT,
                                    Адрес TEXT
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
                                    ФИО TEXT NOT NULL,
                                    Телефон TEXT
                                );";
            string createNotifications = @"CREATE TABLE IF NOT EXISTS Уведомления (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                ПользовательId INTEGER NOT NULL,
                                ПолисId INTEGER NOT NULL,
                                Тип TEXT NOT NULL,
                                Сообщение TEXT NOT NULL,
                                ДатаСоздания TEXT NOT NULL,
                                Прочитано INTEGER DEFAULT 0,
                                ПутьКФайлу TEXT,
                                FOREIGN KEY(ПользовательId) REFERENCES Пользователи(Id),
                                FOREIGN KEY(ПолисId) REFERENCES Полисы(Id)
                            );";

            using var cmd = new SqliteCommand(createClients, conn);
            cmd.ExecuteNonQuery();
            cmd.CommandText = createProperties;
            cmd.ExecuteNonQuery();
            cmd.CommandText = createPolicies;
            cmd.ExecuteNonQuery();
            cmd.CommandText = createRequests;
            cmd.ExecuteNonQuery();
            cmd.CommandText = createBookings;
            cmd.ExecuteNonQuery();
            cmd.CommandText = createUsers;
            cmd.ExecuteNonQuery();
            cmd.CommandText = createNotifications;
            cmd.ExecuteNonQuery();
        }

        public static void ОбновитьТаблицуПользователей()
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            try
            {
                string addColumn = "ALTER TABLE Пользователи ADD COLUMN Телефон TEXT";
                using var cmd = new SqliteCommand(addColumn, conn);
                cmd.ExecuteNonQuery();
            }
            catch { }
        }

        public static void ЗарегистрироватьПользователя(string логин, string пароль, string фио, string телефон, string паспорт = "", string адрес = "", string роль = "user")
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = @"INSERT INTO Пользователи (Логин, Пароль, ФИО, Телефон, Паспорт, Адрес, Роль) 
                           VALUES (@login, @password, @fio, @phone, @pass, @addr, @role)";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@login", логин);
            cmd.Parameters.AddWithValue("@password", пароль);
            cmd.Parameters.AddWithValue("@fio", фио);
            cmd.Parameters.AddWithValue("@phone", телефон);
            cmd.Parameters.AddWithValue("@pass", паспорт);
            cmd.Parameters.AddWithValue("@addr", адрес);
            cmd.Parameters.AddWithValue("@role", роль);
            cmd.ExecuteNonQuery();
        }

        public static (bool успех, string роль, string фио, string телефон) ПроверитьАвторизацию(string логин, string пароль)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "SELECT Пароль, Роль, ФИО, Телефон FROM Пользователи WHERE Логин = @login";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@login", логин);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string сохраненныйПароль = reader.GetString(0);
                string роль = reader.GetString(1);
                string фио = reader.GetString(2);
                
                string телефон = "";
                if (!reader.IsDBNull(3))
                {
                    телефон = reader.GetString(3);
                }
                
                if (сохраненныйПароль == пароль)
                {
                    return (true, роль, фио, телефон);
                }
            }
            return (false, "", "", "");
        }

        public static List<Пользователь> GetUsers()
        {
            List<Пользователь> list = new();
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "SELECT Id, Логин, Пароль, Роль, ФИО, Телефон, Паспорт, Адрес FROM Пользователи";
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var пользователь = new Пользователь
                {
                    Id = reader.GetInt32(0),
                    Логин = reader.GetString(1),
                    Пароль = reader.GetString(2),
                    Роль = reader.GetString(3),
                    ФИО = reader.GetString(4),
                    Телефон = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    Паспорт = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    Адрес = reader.IsDBNull(7) ? "" : reader.GetString(7)
                };

                list.Add(пользователь);
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

        public static void ОбновитьТелефонПользователя(string логин, string телефон)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "UPDATE Пользователи SET Телефон = @phone WHERE Логин = @login";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@phone", телефон);
            cmd.Parameters.AddWithValue("@login", логин);
            cmd.ExecuteNonQuery();
        }

        public static string ПолучитьТелефонПользователя(string логин)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "SELECT Телефон FROM Пользователи WHERE Логин = @login";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@login", логин);
            var result = cmd.ExecuteScalar();
            return result?.ToString() ?? "";
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
        
            int userId = GetUserIdByPhone(z.Телефон);
            Пользователь user = null;
            if (userId > 0)
            {
                user = GetUserById(userId);
            }

            string sqlCheckClient = "SELECT Id FROM Клиенты WHERE Телефон = @tel";
            int clientId;
            using (var cmd = new SqliteCommand(sqlCheckClient, conn))
            {
                cmd.Parameters.AddWithValue("@tel", z.Телефон);
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    clientId = Convert.ToInt32(result);

                    if (user != null)
                    {
                        string updateClient = "UPDATE Клиенты SET Паспорт = @pass, Адрес = @addr WHERE Id = @id";
                        using var cmdUpdate = new SqliteCommand(updateClient, conn);
                        cmdUpdate.Parameters.AddWithValue("@pass", user.Паспорт ?? "");
                        cmdUpdate.Parameters.AddWithValue("@addr", user.Адрес ?? "");
                        cmdUpdate.Parameters.AddWithValue("@id", clientId);
                        cmdUpdate.ExecuteNonQuery();
                    }
                }
                else
                {
                    string sqlClient = @"INSERT INTO Клиенты (ФИО, Телефон, Паспорт, Адрес) 
                                        VALUES (@fio, @tel, @pass, @addr); SELECT last_insert_rowid();";
                    using var cmd2 = new SqliteCommand(sqlClient, conn);
                    cmd2.Parameters.AddWithValue("@fio", z.ФИО);
                    cmd2.Parameters.AddWithValue("@tel", z.Телефон);

                    if (user != null)
                    {
                        cmd2.Parameters.AddWithValue("@pass", user.Паспорт ?? "");
                        cmd2.Parameters.AddWithValue("@addr", user.Адрес ?? "");
                    }
                    else
                    {
                        cmd2.Parameters.AddWithValue("@pass", "");
                        cmd2.Parameters.AddWithValue("@addr", z.Адрес);
                    }

                    clientId = Convert.ToInt32(cmd2.ExecuteScalar());
                }
            }

            string sqlCheckProperty = "SELECT Id FROM Имущество WHERE Адрес = @adr AND Тип = @tip";
            int propertyId;
            using (var cmd = new SqliteCommand(sqlCheckProperty, conn))
            {
                cmd.Parameters.AddWithValue("@adr", z.Адрес);
                cmd.Parameters.AddWithValue("@tip", z.ТипИмущества);
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    propertyId = Convert.ToInt32(result);
                }
                else
                {
                    string sqlProperty = "INSERT INTO Имущество (Тип, Стоимость, Адрес) VALUES (@tip, @stoim, @adr); SELECT last_insert_rowid();";
                    using var cmd2 = new SqliteCommand(sqlProperty, conn)
                    {
                        Parameters = 
                        {
                            new SqliteParameter("@tip", z.ТипИмущества),
                            new SqliteParameter("@stoim", z.Стоимость),
                            new SqliteParameter("@adr", z.Адрес)
                        }
                    };
                    propertyId = Convert.ToInt32(cmd2.ExecuteScalar());
                }
            }

            DateTime датаНачала = DateTime.Today;
            DateTime датаОкончания = DateTime.Today.AddDays(z.Срок);

            string sqlPolicy = @"INSERT INTO Полисы (КлиентId, ИмуществоId, ДатаНачала, ДатаОкончания, Премия) 
                                 VALUES (@c, @p, @start, @end, @prem); SELECT last_insert_rowid();";

            int policyId;
            using (var cmd = new SqliteCommand(sqlPolicy, conn))
            {
                cmd.Parameters.AddWithValue("@c", clientId);
                cmd.Parameters.AddWithValue("@p", propertyId);
                cmd.Parameters.AddWithValue("@start", датаНачала.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@end", датаОкончания.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@prem", z.Премия);
                policyId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            if (user != null && userId > 0)
            {
                try
                {
                    var client = GetClientById(clientId);
                    var property = GetPropertyById(propertyId);
                    var policy = GetPolicyById(policyId);

                    string pdfPath = PdfGeneratorService.GenerateInsurancePolicy(z, client, property, policy, user);

                    string message = $"Ваша заявка №{z.Id} одобрена! Сформирован договор страхования №{policyId}. " +
                                    $"Пожалуйста, распечатайте договор, подпишите его и принесите в офис по адресу: г. Севастополь, ул. Ленина, 1. " +
                                    $"Время работы офиса: пн-пт с 9:00 до 18:00.";

                    ДобавитьУведомление(userId, policyId, "Одобрение заявки", message, pdfPath);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка при создании PDF или уведомления: {ex.Message}");
                }
            }

            УдалитьЗаявку(id);
        }

        private static int GetUserIdByPhone(string телефон)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "SELECT Id FROM Пользователи WHERE Телефон = @phone";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@phone", телефон);
            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public static void ОтклонитьЗаявку(int id)
        {
            УдалитьЗаявку(id);
        }

        public static List<Клиент> GetClients()
        {
            List<Клиент> list = new();
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "SELECT Id, ФИО, Телефон, Паспорт, Адрес FROM Клиенты";
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var client = new Клиент
                {
                    Id = reader.GetInt32(0),
                    ФИО = reader.GetString(1),
                    Телефон = reader.GetString(2)
                };

                if (!reader.IsDBNull(3))
                    client.Паспорт = reader.GetString(3);
                else
                    client.Паспорт = "";

                if (!reader.IsDBNull(4))
                    client.Адрес = reader.GetString(4);
                else
                    client.Адрес = "";

                list.Add(client);
            }
            return list;
        }

        public static void AddClient(string фио, string телефон, string паспорт = "", string адрес = "")
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "INSERT INTO Клиенты (ФИО, Телефон, Паспорт, Адрес) VALUES (@fio, @tel, @pass, @addr)";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@fio", фио);
            cmd.Parameters.AddWithValue("@tel", телефон);
            cmd.Parameters.AddWithValue("@pass", паспорт ?? "");
            cmd.Parameters.AddWithValue("@addr", адрес ?? "");
            cmd.ExecuteNonQuery();
        }

        public static void DeleteClient(int id)
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

        public static List<Имущество> GetProperties()
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

        public static void AddProperty(string тип, decimal стоимость, string адрес)
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

        public static void DeleteProperty(int id)
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

        public static List<Полис> GetPolicies()
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

        public static void AddPolicy(int клиентId, int имуществоId, decimal премия, DateTime начало, DateTime конец)
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

        public static void DeletePolicy(int id)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "DELETE FROM Полисы WHERE Id = @id";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public static bool СменитьПароль(string логин, string старыйПароль, string новыйПароль)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();

            string sqlCheck = "SELECT Пароль FROM Пользователи WHERE Логин = @login";
            using var cmdCheck = new SqliteCommand(sqlCheck, conn);
            cmdCheck.Parameters.AddWithValue("@login", логин);

            var result = cmdCheck.ExecuteScalar();
            if (result == null || result.ToString() != старыйПароль)
            {
                return false;
            }

            string sqlUpdate = "UPDATE Пользователи SET Пароль = @newPass WHERE Логин = @login";
            using var cmdUpdate = new SqliteCommand(sqlUpdate, conn);
            cmdUpdate.Parameters.AddWithValue("@newPass", новыйПароль);
            cmdUpdate.Parameters.AddWithValue("@login", логин);
            cmdUpdate.ExecuteNonQuery();

            return true;
        }

        public static void ДобавитьУведомление(int пользовательId, int полисId, string тип, string сообщение, string путьКФайлу)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = @"INSERT INTO Уведомления (ПользовательId, ПолисId, Тип, Сообщение, ДатаСоздания, Прочитано, ПутьКФайлу) 
                           VALUES (@userId, @policyId, @type, @message, @date, 0, @filePath)";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@userId", пользовательId);
            cmd.Parameters.AddWithValue("@policyId", полисId);
            cmd.Parameters.AddWithValue("@type", тип);
            cmd.Parameters.AddWithValue("@message", сообщение);
            cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@filePath", путьКФайлу);
            cmd.ExecuteNonQuery();
        }

        public static List<Уведомление> ПолучитьУведомленияПользователя(int пользовательId)
        {
            List<Уведомление> list = new();
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "SELECT * FROM Уведомления WHERE ПользовательId = @userId ORDER BY ДатаСоздания DESC";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@userId", пользовательId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Уведомление
                {
                    Id = reader.GetInt32(0),
                    ПользовательId = reader.GetInt32(1),
                    ПолисId = reader.GetInt32(2),
                    Тип = reader.GetString(3),
                    Сообщение = reader.GetString(4),
                    ДатаСоздания = DateTime.Parse(reader.GetString(5)),
                    Прочитано = reader.GetInt32(6) == 1,
                    ПутьКФайлу = reader.IsDBNull(7) ? "" : reader.GetString(7)
                });
            }
            return list;
        }

        public static void ОтметитьУведомлениеКакПрочитанное(int уведомлениеId)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "UPDATE Уведомления SET Прочитано = 1 WHERE Id = @id";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", уведомлениеId);
            cmd.ExecuteNonQuery();
        }

        public static int GetUserIdByLogin(string логин)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "SELECT Id FROM Пользователи WHERE Логин = @login";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@login", логин);
            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public static Пользователь GetUserById(int id)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "SELECT Id, Логин, Пароль, Роль, ФИО, Телефон, Паспорт, Адрес FROM Пользователи WHERE Id = @id";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var user = new Пользователь
                {
                    Id = reader.GetInt32(0),
                    Логин = reader.GetString(1),
                    Пароль = reader.GetString(2),
                    Роль = reader.GetString(3),
                    ФИО = reader.GetString(4),
                    Телефон = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    Паспорт = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    Адрес = reader.IsDBNull(7) ? "" : reader.GetString(7)
                };
                return user;
            }
            return null;
        }

        public static Клиент GetClientById(int id)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "SELECT Id, ФИО, Телефон, Паспорт, Адрес FROM Клиенты WHERE Id = @id";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var client = new Клиент
                {
                    Id = reader.GetInt32(0),
                    ФИО = reader.GetString(1),
                    Телефон = reader.GetString(2)
                };
                
                if (!reader.IsDBNull(3))
                    client.Паспорт = reader.GetString(3);
                else
                    client.Паспорт = "";
                    
                if (!reader.IsDBNull(4))
                    client.Адрес = reader.GetString(4);
                else
                    client.Адрес = "";
                    
                return client;
            }
            return null;
        }

        public static Имущество GetPropertyById(int id)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "SELECT Id, Тип, Стоимость, Адрес FROM Имущество WHERE Id = @id";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Имущество
                {
                    Id = reader.GetInt32(0),
                    Тип = reader.GetString(1),
                    Стоимость = reader.GetDecimal(2),
                    Адрес = reader.GetString(3)
                };
            }
            return null;
        }

        public static Полис GetPolicyById(int id)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            string sql = "SELECT Id, КлиентId, ИмуществоId, ДатаНачала, ДатаОкончания, Премия FROM Полисы WHERE Id = @id";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Полис
                {
                    Id = reader.GetInt32(0),
                    КлиентId = reader.GetInt32(1),
                    ИмуществоId = reader.GetInt32(2),
                    ДатаНачала = DateTime.Parse(reader.GetString(3)),
                    ДатаОкончания = DateTime.Parse(reader.GetString(4)),
                    Премия = reader.GetDecimal(5)
                };
            }
            return null;
        }

        public static void ОбновитьВсеТаблицы()
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            
            try
            {
                string addUserPassport = "ALTER TABLE Пользователи ADD COLUMN Паспорт TEXT";
                using var cmd1 = new SqliteCommand(addUserPassport, conn);
                cmd1.ExecuteNonQuery();
            }
            catch { }

            try
            {
                string addUserAddress = "ALTER TABLE Пользователи ADD COLUMN Адрес TEXT";
                using var cmd2 = new SqliteCommand(addUserAddress, conn);
                cmd2.ExecuteNonQuery();
            }
            catch { }

            try
            {
                string addClientPassport = "ALTER TABLE Клиенты ADD COLUMN Паспорт TEXT";
                using var cmd3 = new SqliteCommand(addClientPassport, conn);
                cmd3.ExecuteNonQuery();
            }
            catch { }

            try
            {
                string addClientAddress = "ALTER TABLE Клиенты ADD COLUMN Адрес TEXT";
                using var cmd4 = new SqliteCommand(addClientAddress, conn);
                cmd4.ExecuteNonQuery();
            }
            catch { }
        }

        public static void ОбновитьДанныеКлиентов()
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();

            var users = GetUsers();

            foreach (var user in users)
            {
                if (string.IsNullOrEmpty(user.Телефон))
                    continue;

                string sqlFind = "SELECT Id FROM Клиенты WHERE Телефон = @tel";
                using var cmdFind = new SqliteCommand(sqlFind, conn);
                cmdFind.Parameters.AddWithValue("@tel", user.Телефон);
                var result = cmdFind.ExecuteScalar();

                if (result != null)
                {
                    int clientId = Convert.ToInt32(result);
                    
                    string sqlUpdate = "UPDATE Клиенты SET Паспорт = @pass, Адрес = @addr WHERE Id = @id";
                    using var cmdUpdate = new SqliteCommand(sqlUpdate, conn);
                    cmdUpdate.Parameters.AddWithValue("@pass", user.Паспорт ?? "");
                    cmdUpdate.Parameters.AddWithValue("@addr", user.Адрес ?? "");
                    cmdUpdate.Parameters.AddWithValue("@id", clientId);
                    cmdUpdate.ExecuteNonQuery();
                }
            }
        }
    }
}