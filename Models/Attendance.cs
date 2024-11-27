using System;
using System.Collections.Generic;
using System.Data.SQLite;
using ChildrenGarden.Database;

namespace ChildrenGarden.Models
{
    public class Attendance
    {
        public int Idattendance { get; set; }
        public int ChildId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }

        // Пов'язаний об'єкт
        public Child Child { get; set; }

        // Метод додавання запису відвідуваності
        public void Add()
        {
            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = @"
                INSERT INTO attendance (child_id, date, status, notes)
                VALUES (@child_id, @date, @status, @notes);";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@child_id", ChildId);
                    command.Parameters.AddWithValue("@date", Date);
                    command.Parameters.AddWithValue("@status", Status);
                    command.Parameters.AddWithValue("@notes", Notes);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        // Метод оновлення запису відвідуваності
        public void Update()
        {
            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = @"
                UPDATE attendance SET status = @status, notes = @notes WHERE idattendance = @idattendance;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@status", Status);
                    command.Parameters.AddWithValue("@notes", Notes);
                    command.Parameters.AddWithValue("@idattendance", Idattendance);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        // Метод отримання всіх записів відвідуваності
        public static List<Attendance> GetAll()
        {
            var attendanceList = new List<Attendance>();

            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM attendance;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var attendance = new Attendance
                            {
                                Idattendance = Convert.ToInt32(reader["idattendance"]),
                                ChildId = Convert.ToInt32(reader["child_id"]),
                                Date = DateTime.Parse(reader["date"].ToString()),
                                Status = reader["status"].ToString(),
                                Notes = reader["notes"].ToString()
                            };

                            // Завантажуємо об'єкт Child
                            attendance.Child = Child.GetById(attendance.ChildId);

                            attendanceList.Add(attendance);
                        }
                    }
                }

                connection.Close();
            }

            return attendanceList;
        }

        // Метод отримання відвідуваності за ChildId
        public static List<Attendance> GetByChildId(int childId)
        {
            var attendanceList = new List<Attendance>();

            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM attendance WHERE child_id = @childId;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@childId", childId);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var attendance = new Attendance
                            {
                                Idattendance = Convert.ToInt32(reader["idattendance"]),
                                ChildId = Convert.ToInt32(reader["child_id"]),
                                Date = DateTime.Parse(reader["date"].ToString()),
                                Status = reader["status"].ToString(),
                                Notes = reader["notes"].ToString()
                            };

                            // Завантажуємо об'єкт Child
                            attendance.Child = Child.GetById(attendance.ChildId);

                            attendanceList.Add(attendance);
                        }
                    }
                }

                connection.Close();
            }

            return attendanceList;
        }

        // Метод отримання запису відвідуваності за ChildId та датою
        public static Attendance GetByChildIdAndDate(int childId, DateTime date)
        {
            Attendance attendance = null;

            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM attendance WHERE child_id = @childId AND date = @date;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@childId", childId);
                    command.Parameters.AddWithValue("@date", date.Date);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            attendance = new Attendance
                            {
                                Idattendance = Convert.ToInt32(reader["idattendance"]),
                                ChildId = Convert.ToInt32(reader["child_id"]),
                                Date = DateTime.Parse(reader["date"].ToString()),
                                Status = reader["status"].ToString(),
                                Notes = reader["notes"].ToString()
                            };

                            // Завантажуємо об'єкт Child
                            attendance.Child = Child.GetById(attendance.ChildId);
                        }
                    }
                }

                connection.Close();
            }

            return attendance;
        }


        // Перевизначення ToString() для зручного відображення
        public override string ToString()
        {
            return $"{Date.ToShortDateString()} - {Child.Name} {Child.Surname}: {Status}";
        }
    }
}