using System;
using System.Collections.Generic;
using System.Data.SQLite;
using ChildrenGarden.Database;

namespace ChildrenGarden.Models
{
    public class Payment
    {
        public int Idpayments { get; set; }
        public int ParentId { get; set; }
        public int ChildId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Way { get; set; }

        // Пов'язані об'єкти
        public Parent Parent { get; set; }
        public Child Child { get; set; }

        // Метод додавання платежу
        public void Add()
        {
            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = @"
                INSERT INTO payments (parent_id, child_id, amount, date, way)
                VALUES (@parent_id, @child_id, @amount, @date, @way);";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@parent_id", ParentId);
                    command.Parameters.AddWithValue("@child_id", ChildId);
                    command.Parameters.AddWithValue("@amount", Amount);
                    command.Parameters.AddWithValue("@date", Date);
                    command.Parameters.AddWithValue("@way", Way);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        // Метод отримання списку всіх платежів
        public static List<Payment> GetAll()
        {
            var payments = new List<Payment>();

            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM payments;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var payment = new Payment
                            {
                                Idpayments = Convert.ToInt32(reader["idpayments"]),
                                ParentId = Convert.ToInt32(reader["parent_id"]),
                                ChildId = reader["child_id"] != DBNull.Value ? Convert.ToInt32(reader["child_id"]) : 0,
                                Amount = Convert.ToDecimal(reader["amount"]),
                                Date = DateTime.Parse(reader["date"].ToString()),
                                Way = reader["way"].ToString()
                            };

                            // Завантажуємо об'єкти Parent та Child
                            payment.Parent = Parent.GetById(payment.ParentId);
                            if (payment.ChildId != 0)
                            {
                                payment.Child = Child.GetById(payment.ChildId);
                            }

                            payments.Add(payment);
                        }
                    }
                }

                connection.Close();
            }

            return payments;
        }

        public void Delete()
        {
            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = "DELETE FROM payments WHERE idpayments = @paymentId;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@paymentId", Idpayments);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public static List<Payment> GetByChildId(int childId)
        {
            var payments = new List<Payment>();

            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM payments WHERE child_id = @childId;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@childId", childId);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var payment = new Payment
                            {
                                Idpayments = Convert.ToInt32(reader["idpayments"]),
                                ParentId = Convert.ToInt32(reader["parent_id"]),
                                ChildId = Convert.ToInt32(reader["child_id"]),
                                Amount = Convert.ToDecimal(reader["amount"]),
                                Date = DateTime.Parse(reader["date"].ToString()),
                                Way = reader["way"].ToString()
                            };

                            // Завантажуємо пов'язані об'єкти
                            payment.Parent = Parent.GetById(payment.ParentId);
                            payment.Child = Child.GetById(payment.ChildId);

                            payments.Add(payment);
                        }
                    }
                }

                connection.Close();
            }

            return payments;
        }

        public static void DeleteByParentId(int parentId, SQLiteConnection connection, SQLiteTransaction transaction)
        {
            string deletePaymentsQuery = "DELETE FROM payments WHERE parent_id = @parentId;";
            using (var command = new SQLiteCommand(deletePaymentsQuery, connection, transaction))
            {
                command.Parameters.AddWithValue("@parentId", parentId);
                command.ExecuteNonQuery();
            }
        }

        // Метод видалення платежів за ChildId (використовується в методі Delete дитини)
        public static void DeleteByChildId(int childId, SQLiteConnection connection, SQLiteTransaction transaction)
        {
            string deletePaymentsQuery = "DELETE FROM payments WHERE child_id = @childId;";
            using (var command = new SQLiteCommand(deletePaymentsQuery, connection, transaction))
            {
                command.Parameters.AddWithValue("@childId", childId);
                command.ExecuteNonQuery();
            }
        }

        // Перевизначення ToString() для зручного відображення
        public override string ToString()
        {
            string childInfo = Child != null ? $" ({Child.Name} {Child.Surname})" : "";
            return $"{Date.ToShortDateString()} - {Parent.Name} {Parent.Surname}{childInfo}: {Amount:C} ({Way})";
        }
    }
}