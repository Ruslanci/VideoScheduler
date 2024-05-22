using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoVideoPlayer
{
    public partial class Form1 : Form
    {

        private string videoPath;

        public Form1()
        {
            InitializeComponent();
        }

        private void chooseVideoButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Video Files|*.mp4;*.avi;*.mkv";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                videoPath = ofd.FileName;
            }
        }

        private void addVideoButton_Click(object sender, EventArgs e)
        {
            int verticalSpacing = 10; // Spacing between controls

            // Create a new instance of the Button
            Button newChooseVideoButton = new Button();
            newChooseVideoButton.Location = new Point(128, this.chooseVideoButton.Bottom + verticalSpacing);
            newChooseVideoButton.Name = "chooseVideoButton" + (this.Controls.OfType<Button>().Count(b => b.Name.StartsWith("chooseVideoButton")) + 1);
            newChooseVideoButton.Size = new Size(157, 30);
            newChooseVideoButton.Text = "Выбрать видео";
            newChooseVideoButton.UseVisualStyleBackColor = true;
            newChooseVideoButton.Click += new EventHandler(this.chooseVideoButton_Click);

            // Create a new instance of the DateTimePicker
            DateTimePicker newDateTimePicker = new DateTimePicker();
            newDateTimePicker.Format = DateTimePickerFormat.Time;
            newDateTimePicker.Location = new Point(350, this.dateTimePicker1.Bottom + verticalSpacing);
            newDateTimePicker.Name = "dateTimePicker" + (this.Controls.OfType<DateTimePicker>().Count() + 1);
            newDateTimePicker.Size = new Size(87, 20);

            // Create a new instance of the NumericUpDown
            NumericUpDown newNumericUpDown = new NumericUpDown();
            newNumericUpDown.Location = new Point(560, this.numericUpDown1.Bottom + verticalSpacing);
            newNumericUpDown.Name = "numericUpDown" + (this.Controls.OfType<NumericUpDown>().Count() + 1);
            newNumericUpDown.Size = new Size(120, 20);
            newNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });

            // Calculate new Y position for the addVideoButton
            int newYPosition = Math.Max(newChooseVideoButton.Bottom, Math.Max(newDateTimePicker.Bottom, newNumericUpDown.Bottom)) + verticalSpacing;

            // Update the position of addVideoButton
            this.addVideoButton.Location = new Point(this.addVideoButton.Location.X, newYPosition);

            // Add the new controls to the form
            this.Controls.Add(newChooseVideoButton);
            this.Controls.Add(newDateTimePicker);
            this.Controls.Add(newNumericUpDown);
        }



        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
