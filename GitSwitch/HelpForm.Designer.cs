namespace GitSwitch
{
    partial class HelpForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.HelpRichTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // HelpRichTextBox
            // 
            this.HelpRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.HelpRichTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HelpRichTextBox.HideSelection = false;
            this.HelpRichTextBox.Location = new System.Drawing.Point(12, 12);
            this.HelpRichTextBox.Name = "HelpRichTextBox";
            this.HelpRichTextBox.ReadOnly = true;
            this.HelpRichTextBox.Size = new System.Drawing.Size(992, 582);
            this.HelpRichTextBox.TabIndex = 0;
            this.HelpRichTextBox.Text = "";
            // 
            // HelpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1017, 606);
            this.Controls.Add(this.HelpRichTextBox);
            this.Name = "HelpForm";
            this.Text = "GitSwitch Help";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox HelpRichTextBox;
    }
}