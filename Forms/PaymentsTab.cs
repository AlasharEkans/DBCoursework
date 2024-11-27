using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ParentModel = ChildrenGarden.Models.Parent;
using ChildModel = ChildrenGarden.Models.Child;
using ChildrenGarden.Models;

namespace ChildrenGarden.Forms
{
    public partial class PaymentsTab : UserControl
    {
        private ListBox listBoxPayments;
        private ComboBox comboBoxParent;
        private ComboBox comboBoxChild;
        private TextBox textBoxAmount;
        private DateTimePicker dateTimePickerDate;
        private ComboBox comboBoxWay;
        private Button buttonAdd;
        private Button buttonDelete; // Додано кнопку видалення

        public PaymentsTab()
        {
            InitializeComponents();
            LoadPayments();
            LoadParents();
            LoadChildren();
        }

        private void InitializeComponents()
        {
            listBoxPayments = new ListBox();
            comboBoxParent = new ComboBox();
            comboBoxChild = new ComboBox();
            textBoxAmount = new TextBox();
            dateTimePickerDate = new DateTimePicker();
            comboBoxWay = new ComboBox();
            buttonAdd = new Button();
            buttonDelete = new Button(); // Ініціалізація кнопки видалення

            int leftMargin = 10;
            int topMargin = 10;
            int labelWidth = 120;
            int controlWidth = 200;
            int controlHeight = 25;
            int spacing = 5;

            listBoxPayments.Location = new System.Drawing.Point(leftMargin, topMargin);
            listBoxPayments.Size = new System.Drawing.Size(300, 400);
            listBoxPayments.SelectedIndexChanged += ListBoxPayments_SelectedIndexChanged;

            int formLeft = leftMargin + 310;
            int currentTop = topMargin;

            var labelParent = new Label();
            labelParent.Text = "Батько/Мати:";
            labelParent.Location = new System.Drawing.Point(formLeft, currentTop);
            labelParent.Size = new System.Drawing.Size(labelWidth, controlHeight);
            comboBoxParent.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            comboBoxParent.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            var labelChild = new Label();
            labelChild.Text = "Дитина:";
            labelChild.Location = new System.Drawing.Point(formLeft, currentTop);
            labelChild.Size = new System.Drawing.Size(labelWidth, controlHeight);
            comboBoxChild.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            comboBoxChild.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            var labelAmount = new Label();
            labelAmount.Text = "Сума:";
            labelAmount.Location = new System.Drawing.Point(formLeft, currentTop);
            labelAmount.Size = new System.Drawing.Size(labelWidth, controlHeight);
            textBoxAmount.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            textBoxAmount.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            var labelDate = new Label();
            labelDate.Text = "Дата:";
            labelDate.Location = new System.Drawing.Point(formLeft, currentTop);
            labelDate.Size = new System.Drawing.Size(labelWidth, controlHeight);
            dateTimePickerDate.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            dateTimePickerDate.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            var labelWay = new Label();
            labelWay.Text = "Спосіб оплати:";
            labelWay.Location = new System.Drawing.Point(formLeft, currentTop);
            labelWay.Size = new System.Drawing.Size(labelWidth, controlHeight);
            comboBoxWay.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            comboBoxWay.Size = new System.Drawing.Size(controlWidth, controlHeight);
            comboBoxWay.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxWay.Items.AddRange(new string[] { "Готівка", "Картка", "Банківський переказ", "Мобільний додаток" });
            currentTop += controlHeight + spacing;

            buttonAdd.Text = "Додати";
            buttonAdd.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            buttonAdd.Click += buttonAdd_Click;

            buttonDelete.Text = "Видалити";
            buttonDelete.Location = new System.Drawing.Point(formLeft + labelWidth + buttonAdd.Width + 10, currentTop);
            buttonDelete.Click += buttonDelete_Click;

            Controls.Add(listBoxPayments);
            Controls.Add(labelParent);
            Controls.Add(comboBoxParent);
            Controls.Add(labelChild);
            Controls.Add(comboBoxChild);
            Controls.Add(labelAmount);
            Controls.Add(textBoxAmount);
            Controls.Add(labelDate);
            Controls.Add(dateTimePickerDate);
            Controls.Add(labelWay);
            Controls.Add(comboBoxWay);
            Controls.Add(buttonAdd);
            Controls.Add(buttonDelete);
        }

        private void LoadPayments()
        {
            listBoxPayments.Items.Clear();
            List<Payment> payments = Payment.GetAll();
            foreach (var payment in payments)
            {
                listBoxPayments.Items.Add(payment);
            }
        }

        public void LoadParents()
        {
            comboBoxParent.Items.Clear();
            List<ParentModel> parents = ParentModel.GetAll();
            foreach (var parent in parents)
            {
                comboBoxParent.Items.Add(parent);
            }
        }

        public void LoadChildren()
        {
            comboBoxChild.Items.Clear();
            List<ChildModel> children = ChildModel.GetAll();
            foreach (var child in children)
            {
                comboBoxChild.Items.Add(child);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (comboBoxParent.SelectedItem == null ||
                comboBoxChild.SelectedItem == null ||
                string.IsNullOrWhiteSpace(textBoxAmount.Text) ||
                comboBoxWay.SelectedItem == null)
            {
                MessageBox.Show("Будь ласка, заповніть всі поля.");
                return;
            }

            if (!decimal.TryParse(textBoxAmount.Text.Trim(), out decimal amount))
            {
                MessageBox.Show("Будь ласка, введіть коректну суму.");
                return;
            }

            var payment = new Payment
            {
                ParentId = ((ParentModel)comboBoxParent.SelectedItem).Idparents,
                ChildId = ((ChildModel)comboBoxChild.SelectedItem).ID,
                Amount = amount,
                Date = dateTimePickerDate.Value,
                Way = comboBoxWay.SelectedItem.ToString()
            };
            payment.Add();
            LoadPayments();
            ClearForm();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listBoxPayments.SelectedItem == null)
            {
                MessageBox.Show("Будь ласка, оберіть платіж для видалення.");
                return;
            }

            var selectedPayment = (Payment)listBoxPayments.SelectedItem;

            DialogResult result = MessageBox.Show("Ви впевнені, що хочете видалити цей платіж?", "Підтвердження видалення", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                try
                {
                    selectedPayment.Delete();
                    LoadPayments();
                    ClearForm();
                    MessageBox.Show("Платіж успішно видалено.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при видаленні платежу: {ex.Message}");
                }
            }
        }

        private void ClearForm()
        {
            comboBoxParent.SelectedIndex = -1;
            comboBoxChild.SelectedIndex = -1;
            textBoxAmount.Text = "";
            dateTimePickerDate.Value = DateTime.Now;
            comboBoxWay.SelectedIndex = -1;
        }

        private void ListBoxPayments_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Можна додати заповнення полів при виборі платежу зі списку
        }
    }
}