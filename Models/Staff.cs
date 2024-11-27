using System;
using System.Collections.Generic;
using System.Data.SQLite;
using ChildrenGarden.Database;

namespace ChildrenGarden.Models
{
    public class Staff
    {
        public int Idstaff { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Position { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime HireDate { get; set; }

        // Метод додавання співробітника до бази даних
        public void Add()
        {
            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = @"
                INSERT INTO staff (name, surname, position, phone, email, hire_date)
                VALUES (@name, @surname, @position, @phone, @email, @hire_date);";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", Name);
                    command.Parameters.AddWithValue("@surname", Surname);
                    command.Parameters.AddWithValue("@position", Position);
                    command.Parameters.AddWithValue("@phone", Phone);
                    command.Parameters.AddWithValue("@email", Email);
                    command.Parameters.AddWithValue("@hire_date", HireDate);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        // Метод отримання списку всіх співробітників
        public static List<Staff> GetAll()
        {
            var staffList = new List<Staff>();

            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM staff;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var staff = new Staff
                            {
                                Idstaff = Convert.ToInt32(reader["idstaff"]),
                                Name = reader["name"].ToString(),
                                Surname = reader["surname"].ToString(),
                                Position = reader["position"].ToString(),
                                Phone = reader["phone"].ToString(),
                                Email = reader["email"].ToString(),
                                HireDate = DateTime.Parse(reader["hire_date"].ToString())
                            };

                            staffList.Add(staff);
                        }
                    }
                }

                connection.Close();
            }

            return staffList;
        }

        // Метод для отримання співробітника за ID
        public static Staff GetById(int id)
        {
            Staff staff = null;

            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM staff WHERE idstaff = @id;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            staff = new Staff
                            {
                                Idstaff = Convert.ToInt32(reader["idstaff"]),
                                Name = reader["name"].ToString(),
                                Surname = reader["surname"].ToString(),
                                Position = reader["position"].ToString(),
                                Phone = reader["phone"].ToString(),
                                Email = reader["email"].ToString(),
                                HireDate = DateTime.Parse(reader["hire_date"].ToString())
                            };
                        }
                    }
                }

                connection.Close();
            }

            return staff;
        }

        // Метод видалення співробітника
        public void Delete()
        {
            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Оновлення груп, де співробітник призначений вчителем
                        string updateGroupsQuery = "UPDATE groups SET teacher_id = NULL WHERE teacher_id = @staffId;";
                        using (var command = new SQLiteCommand(updateGroupsQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@staffId", Idstaff);
                            command.ExecuteNonQuery();
                        }

                        // Видалення співробітника
                        string deleteStaffQuery = "DELETE FROM staff WHERE idstaff = @staffId;";
                        using (var command = new SQLiteCommand(deleteStaffQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@staffId", Idstaff);
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Помилка при видаленні співробітника: {ex.Message}");
                    }
                }

                connection.Close();
            }
        }

        // Перевизначення ToString() для зручного відображення
        public override string ToString()
        {
            return $"{Name} {Surname} - {Position}";
        }
    }
}