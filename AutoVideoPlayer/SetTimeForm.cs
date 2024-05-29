using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoVideoPlayer
{
    public class SetTimeForm : Form
    {
        private DateTimePicker dateTimePicker;
        private NumericUpDown numericUpDown;
        private Button confirmButton;

        public DateTime ChosenTime { get; private set; }
        public decimal NumberOfPlays { get; private set; }

        public SetTimeForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Initialize form and controls
            this.Text = "Выберите время для проигрывания и количество проигрываний";
            this.Size = new Size(250, 250); // Increased size to accommodate labels

            // DateTimePicker
            Label dateTimeLabel = new Label();
            dateTimeLabel.Text = "Время";
            dateTimeLabel.Dock = DockStyle.Top;

            dateTimePicker = new DateTimePicker();
            dateTimePicker.Format = DateTimePickerFormat.Custom;
            dateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm";
            dateTimePicker.MinDate = DateTime.Now; // Restrict to current time and onwards
            dateTimePicker.Dock = DockStyle.Top;

            // NumericUpDown
            Label numericUpDownLabel = new Label();
            numericUpDownLabel.Text = "Повторы";
            numericUpDownLabel.Dock = DockStyle.Top;

            numericUpDown = new NumericUpDown();
            numericUpDown.Minimum = 1; // Minimum number of repetitions
            numericUpDown.Maximum = 100; // Maximum number of repetitions
            numericUpDown.Value = 1; // Default value
            numericUpDown.Dock = DockStyle.Top;

            // OK Button
            confirmButton = new Button();
            confirmButton.Text = "OK";
            confirmButton.DialogResult = DialogResult.OK;
            confirmButton.Dock = DockStyle.Bottom;
            confirmButton.Click += ConfirmButton_Click;

            // Add controls to the form
            this.Controls.Add(dateTimeLabel);
            this.Controls.Add(dateTimePicker);
            this.Controls.Add(numericUpDownLabel);
            this.Controls.Add(numericUpDown);
            this.Controls.Add(confirmButton);
        }


        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            DateTime selectedDateTime = dateTimePicker.Value;
            decimal selectedNumberOfPlays = numericUpDown.Value;

            if (selectedDateTime.TimeOfDay.Minutes < DateTime.Now.TimeOfDay.Minutes)
            {
                MessageBox.Show("Можно выбрать время не раньше текущего.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Don't close the form if the selection is invalid
            }

            // Set chosen date/time and close the form
            ChosenTime = selectedDateTime;
            NumberOfPlays = selectedNumberOfPlays;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
