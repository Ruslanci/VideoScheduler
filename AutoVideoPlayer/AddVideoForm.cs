using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoVideoPlayer
{
    public class AddVideoForm : Form
    {
        private DateTimePicker dateTimePicker;
        private NumericUpDown numericUpDown;
        private Button confirmButton;

        public DateTime ChosenTime { get; private set; }
        public decimal NumberOfPlays { get; private set; }

        public AddVideoForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Выберите время для проигрывания и количество проигрываний";
            this.Size = new Size(250, 250);

            Label dateTimeLabel = new Label();
            dateTimeLabel.Text = "Время";
            dateTimeLabel.Dock = DockStyle.Top;

            dateTimePicker = new DateTimePicker();
            dateTimePicker.Format = DateTimePickerFormat.Custom;
            dateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm";
            dateTimePicker.MinDate = DateTime.Now;
            dateTimePicker.Dock = DockStyle.Top;

            Label numericUpDownLabel = new Label();
            numericUpDownLabel.Text = "Повторы";
            numericUpDownLabel.Dock = DockStyle.Top;

            numericUpDown = new NumericUpDown();
            numericUpDown.Minimum = 1;
            numericUpDown.Maximum = 100;
            numericUpDown.Value = 1;
            numericUpDown.Dock = DockStyle.Top;

            confirmButton = new Button();
            confirmButton.Text = "OK";
            confirmButton.DialogResult = DialogResult.OK;
            confirmButton.Dock = DockStyle.Bottom;
            confirmButton.Click += ConfirmButton_Click;

            this.Controls.Add(dateTimeLabel);
            this.Controls.Add(dateTimePicker);
            this.Controls.Add(numericUpDownLabel);
            this.Controls.Add(numericUpDown);
            this.Controls.Add(confirmButton);
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            if (ValidateAndSetValues(dateTimePicker.Value, numericUpDown.Value))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        public bool ValidateAndSetValues(DateTime selectedDateTime, decimal selectedNumberOfPlays)
        {
            if (selectedDateTime < DateTime.Now)
            {
                MessageBox.Show("Можно выбрать время не раньше текущего.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            ChosenTime = selectedDateTime;
            NumberOfPlays = selectedNumberOfPlays;
            return true;
        }
    }
}
