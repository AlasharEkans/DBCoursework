using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ChildrenGarden.Models;

namespace ChildrenGarden.Forms
{
    public partial class StaffTab : UserControl
    {
        private ListBox listBoxStaff;
        private TextBox textBoxName;
        private TextBox textBoxSurname;
        private TextBox textBoxPosition;
        private TextBox textBoxPhone;
        private TextBox textBoxEmail;
        private DateTimePicker dateTimePickerHireDate;
        private Button buttonAdd;
        private Button buttonDelete; // Додано кнопку видалення

        public StaffTab()
        {
            InitializeComponents();
            LoadStaff();
        }

        private void InitializeComponents()
        {
            listBoxStaff = new ListBox();
            textBoxName = new TextBox();
            textBoxSurname = new TextBox();
            textBoxPosition = new TextBox();
            textBoxPhone = new TextBox();
            textBoxEmail = new TextBox();
            dateTimePickerHireDate = new DateTimePicker();
            buttonAdd = new Button();
            buttonDelete = new Button(); // Ініціалізація кнопки видалення

            int leftMargin = 10;
            int topMargin = 10;
            int labelWidth = 120;
            int controlWidth = 200;
            int controlHeight = 25;
            int spacing = 5;

            listBoxStaff.Location = new System.Drawing.Point(leftMargin, topMargin);
            listBoxStaff.Size = new System.Drawing.Size(300, 400);
            listBoxStaff.SelectedIndexChanged += ListBoxStaff_SelectedIndexChanged;

            int formLeft = leftMargin + 310;
            int currentTop = topMargin;

            var labelName = new Label();
            labelName.Text = "Ім'я:";
            labelName.Location = new System.Drawing.Point(formLeft, currentTop);
            labelName.Size = new System.Drawing.Size(labelWidth, controlHeight);
            textBoxName.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            textBoxName.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            var labelSurname = new Label();
            labelSurname.Text = "Прізвище:";
            labelSurname.Location = new System.Drawing.Point(formLeft, currentTop);
            labelSurname.Size = new System.Drawing.Size(labelWidth, controlHeight);
            textBoxSurname.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            textBoxSurname.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            var labelPosition = new Label();
            labelPosition.Text = "Посада:";
            labelPosition.Location = new System.Drawing.Point(formLeft, currentTop);
            labelPosition.Size = new System.Drawing.Size(labelWidth, controlHeight);
            textBoxPosition.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            textBoxPosition.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            var labelPhone = new Label();
            labelPhone.Text = "Телефон:";
            labelPhone.Location = new System.Drawing.Point(formLeft, currentTop);
            labelPhone.Size = new System.Drawing.Size(labelWidth, controlHeight);
            textBoxPhone.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            textBoxPhone.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            var labelEmail = new Label();
            labelEmail.Text = "Email:";
            labelEmail.Location = new System.Drawing.Point(formLeft, currentTop);
            labelEmail.Size = new System.Drawing.Size(labelWidth, controlHeight);
            textBoxEmail.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            textBoxEmail.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            var labelHireDate = new Label();
            labelHireDate.Text = "Дата найму:";
            labelHireDate.Location = new System.Drawing.Point(formLeft, currentTop);
            labelHireDate.Size = new System.Drawing.Size(labelWidth, controlHeight);
            dateTimePickerHireDate.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            dateTimePickerHireDate.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            buttonAdd.Text = "Додати";
            buttonAdd.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            buttonAdd.Click += buttonAdd_Click;

            buttonDelete.Text = "Видалити";
            buttonDelete.Location = new System.Drawing.Point(formLeft + labelWidth + buttonAdd.Width + 10, currentTop);
            buttonDelete.Click += buttonDelete_Click;

            Controls.Add(listBoxStaff);
            Controls.Add(labelName);
            Controls.Add(textBoxName);
            Controls.Add(labelSurname);
            Controls.Add(textBoxSurname);
            Controls.Add(labelPosition);
            Controls.Add(textBoxPosition);
            Controls.Add(labelPhone);
            Controls.Add(textBoxPhone);
            Controls.Add(labelEmail);
            Controls.Add(textBoxEmail);
            Controls.Add(labelHireDate);
            Controls.Add(dateTimePickerHireDate);
            Controls.Add(buttonAdd);
            Controls.Add(buttonDelete);
        }

        private void LoadStaff()
        {
            listBoxStaff.Items.Clear();
            List<Staff> staffList = Staff.GetAll();
            foreach (var staff in staffList)
            {
                listBoxStaff.Items.Add(staff);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Перевірка на порожні поля
            if (string.IsNullOrWhiteSpace(textBoxName.Text) ||
                string.IsNullOrWhiteSpace(textBoxSurname.Text) ||
                string.IsNullOrWhiteSpace(textBoxPosition.Text))
            {
                MessageBox.Show("Будь ласка, заповніть всі обов'язкові поля (Ім'я, Прізвище та Посада).");
                return;
            }

            var staff = new Staff
            {
                Name = textBoxName.Text.Trim(),
                Surname = textBoxSurname.Text.Trim(),
                Position = textBoxPosition.Text.Trim(),
                Phone = string.IsNullOrWhiteSpace(textBoxPhone.Text) ? null : textBoxPhone.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(textBoxEmail.Text) ? null : textBoxEmail.Text.Trim(),
                HireDate = dateTimePickerHireDate.Value
            };
            staff.Add();
            LoadStaff();
            ClearForm();

            // Викликаємо подію для оновлення в інших вкладках
            StaffAdded?.Invoke(this, EventArgs.Empty);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listBoxStaff.SelectedItem == null)
            {
                MessageBox.Show("Будь ласка, оберіть співробітника для видалення.");
                return;
            }

            var selectedStaff = (Staff)listBoxStaff.SelectedItem;

            // Перевірка наявності груп, де співробітник є вчителем
            var groupsWithTeacher = Group.GetAll().FindAll(g => g.TeacherId == selectedStaff.Idstaff);

            string message = "Ви впевнені, що хочете видалити цього співробітника?";
            if (groupsWithTeacher.Count > 0)
            {
                message += "\nВін/вона призначений(а) вчителем у деяких групах. Ці групи будуть оновлені.";
            }

            DialogResult result = MessageBox.Show(message, "Підтвердження видалення", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                try
                {
                    selectedStaff.Delete();
                    LoadStaff();
                    ClearForm();
                    MessageBox.Show("Співробітника успішно видалено.");

                    // Викликаємо подію, якщо потрібна для оновлення в інших вкладках
                    StaffDeleted?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при видаленні співробітника: {ex.Message}");
                }
            }
        }

        private void ClearForm()
        {
            textBoxName.Text = "";
            textBoxSurname.Text = "";
            textBoxPosition.Text = "";
            textBoxPhone.Text = "";
            textBoxEmail.Text = "";
            dateTimePickerHireDate.Value = DateTime.Now;
        }

        private void ListBoxStaff_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Можна додати заповнення полів при виборі співробітника зі списку
        }

        // Подія для повідомлення інших вкладок про видалення співробітника
        public event EventHandler StaffDeleted;

        // Подія для повідомлення інших вкладок про додавання співробітника
        public event EventHandler StaffAdded;
    }
}