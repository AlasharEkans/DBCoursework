using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ParentModel = ChildrenGarden.Models.Parent;
using ChildModel = ChildrenGarden.Models.Child;
using ChildrenGarden.Models;

namespace ChildrenGarden.Forms
{
    public partial class ParentsTab : UserControl
    {
        private ListBox listBoxParents;
        private TextBox textBoxName;
        private TextBox textBoxSurname;
        private TextBox textBoxPhone;
        private TextBox textBoxEmail;
        private TextBox textBoxAddress;
        private Button buttonAdd;
        private Button buttonDelete; // Додано кнопку видалення

        public ParentsTab()
        {
            InitializeComponents();
            LoadParents();
        }

        private void InitializeComponents()
        {
            listBoxParents = new ListBox();
            textBoxName = new TextBox();
            textBoxSurname = new TextBox();
            textBoxPhone = new TextBox();
            textBoxEmail = new TextBox();
            textBoxAddress = new TextBox();
            buttonAdd = new Button();
            buttonDelete = new Button(); // Ініціалізація кнопки видалення

            int leftMargin = 10;
            int topMargin = 10;
            int labelWidth = 120;
            int controlWidth = 200;
            int controlHeight = 25;
            int spacing = 5;

            listBoxParents.Location = new System.Drawing.Point(leftMargin, topMargin);
            listBoxParents.Size = new System.Drawing.Size(300, 400);
            listBoxParents.SelectedIndexChanged += ListBoxParents_SelectedIndexChanged;

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

            var labelAddress = new Label();
            labelAddress.Text = "Адреса:";
            labelAddress.Location = new System.Drawing.Point(formLeft, currentTop);
            labelAddress.Size = new System.Drawing.Size(labelWidth, controlHeight);
            textBoxAddress.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            textBoxAddress.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            buttonAdd.Text = "Додати";
            buttonAdd.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            buttonAdd.Click += buttonAdd_Click;

            buttonDelete.Text = "Видалити";
            buttonDelete.Location = new System.Drawing.Point(formLeft + labelWidth + buttonAdd.Width + 10, currentTop);
            buttonDelete.Click += buttonDelete_Click;

            Controls.Add(listBoxParents);
            Controls.Add(labelName);
            Controls.Add(textBoxName);
            Controls.Add(labelSurname);
            Controls.Add(textBoxSurname);
            Controls.Add(labelPhone);
            Controls.Add(textBoxPhone);
            Controls.Add(labelEmail);
            Controls.Add(textBoxEmail);
            Controls.Add(labelAddress);
            Controls.Add(textBoxAddress);
            Controls.Add(buttonAdd);
            Controls.Add(buttonDelete);
        }

        private void LoadParents()
        {
            listBoxParents.Items.Clear();
            List<ParentModel> parents = ParentModel.GetAll();
            foreach (var parent in parents)
            {
                listBoxParents.Items.Add(parent);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Перевірка на порожні поля
            if (string.IsNullOrWhiteSpace(textBoxName.Text) ||
                string.IsNullOrWhiteSpace(textBoxSurname.Text))
            {
                MessageBox.Show("Будь ласка, заповніть всі обов'язкові поля (Ім'я та Прізвище).");
                return;
            }

            var parent = new ParentModel
            {
                Name = textBoxName.Text.Trim(),
                Surname = textBoxSurname.Text.Trim(),
                Phone = string.IsNullOrWhiteSpace(textBoxPhone.Text) ? null : textBoxPhone.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(textBoxEmail.Text) ? null : textBoxEmail.Text.Trim(),
                Address = string.IsNullOrWhiteSpace(textBoxAddress.Text) ? null : textBoxAddress.Text.Trim()
            };
            parent.Add();
            LoadParents();
            ClearForm();

            // Викликаємо подію для оновлення в інших вкладках
            ParentAdded?.Invoke(this, EventArgs.Empty);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listBoxParents.SelectedItem == null)
            {
                MessageBox.Show("Будь ласка, оберіть батька/матір для видалення.");
                return;
            }

            var selectedParent = (ParentModel)listBoxParents.SelectedItem;

            // Перевірка наявності пов'язаних дітей
            var children = ChildModel.GetByParentId(selectedParent.Idparents);

            string message = "Ви впевнені, що хочете видалити цього батька/матір?";
            if (children.Count > 0)
            {
                message += "\nЦе також видалить всіх дітей цього батька/матері та пов'язані записи.";
            }

            DialogResult result = MessageBox.Show(message, "Підтвердження видалення", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                try
                {
                    selectedParent.Delete();
                    LoadParents();
                    ClearForm();
                    MessageBox.Show("Батька/матір та пов'язані записи успішно видалено.");

                    // Викликаємо подію, якщо потрібна для оновлення в інших вкладках
                    ParentDeleted?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при видаленні батька/матері: {ex.Message}");
                }
            }
        }

        private void ClearForm()
        {
            textBoxName.Text = "";
            textBoxSurname.Text = "";
            textBoxPhone.Text = "";
            textBoxEmail.Text = "";
            textBoxAddress.Text = "";
        }

        private void ListBoxParents_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        public event EventHandler ParentDeleted;

        // Подія для повідомлення інших вкладок про додавання батька/матері
        public event EventHandler ParentAdded;
    }
}