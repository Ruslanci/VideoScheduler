using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoVideoPlayer
{
    public class VideoLabelManager
    {
        private int labelCounter;
        private int initialX;
        private int initialY;
        private int verticalSpacing;
        private Control parentControl;
        private Panel panel;

        public VideoLabelManager(Control parentControl, int initialX, int initialY, int verticalSpacing)
        {
            this.labelCounter = 0;
            this.initialX = initialX;
            this.initialY = initialY;
            this.verticalSpacing = verticalSpacing;
            this.parentControl = parentControl;

            panel = new Panel();
            panel.AutoScroll = true;
            panel.Dock = DockStyle.Fill;
            parentControl.Controls.Add(panel);
        }

        public void AddLabel(string text)
        {
            Button editButton = new Button
            {
                Text = "Изменить",
                Tag = labelCounter,
                Location = new Point(initialX, initialY + labelCounter * verticalSpacing)
            };
            editButton.Click += EditButton_Click;

            Button deleteButton = new Button
            {
                Text = "Удалить",
                Tag = labelCounter, 
                Location = new Point(initialX, initialY + labelCounter * verticalSpacing + 30)
            };
            deleteButton.Click += DeleteButton_Click;

            Label videoLabel = new Label
            {
                Text = $"{labelCounter + 1}. {text}",
                AutoSize = true,
                Location = new Point(initialX + 100, initialY + labelCounter * verticalSpacing)
            };

            labelCounter++;

            // panel.Controls.Add(editButton);
            // panel.Controls.Add(deleteButton);
            panel.Controls.Add(videoLabel);
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            Button editButton = (Button)sender;
            int labelIndex = (int)editButton.Tag;
            MessageBox.Show($"Editing video label {labelIndex}");
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            Button deleteButton = (Button)sender;
            int labelIndex = (int)deleteButton.Tag;
            MessageBox.Show($"Deleting video label {labelIndex}");
        }
    }
}
