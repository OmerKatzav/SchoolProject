namespace Client
{
    partial class ForgotPasswordForm
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
            submitBtn = new Button();
            label1 = new Label();
            label2 = new Label();
            emailBox = new TextBox();
            passwordBox = new TextBox();
            SuspendLayout();
            // 
            // submitBtn
            // 
            submitBtn.Location = new Point(125, 137);
            submitBtn.Name = "submitBtn";
            submitBtn.Size = new Size(75, 23);
            submitBtn.TabIndex = 0;
            submitBtn.Text = "Submit";
            submitBtn.UseVisualStyleBackColor = true;
            submitBtn.Click += submitBtn_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(31, 39);
            label1.Name = "label1";
            label1.Size = new Size(39, 15);
            label1.TabIndex = 1;
            label1.Text = "Email:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(32, 76);
            label2.Name = "label2";
            label2.Size = new Size(87, 15);
            label2.TabIndex = 2;
            label2.Text = "New Password:";
            // 
            // emailBox
            // 
            emailBox.Location = new Point(125, 39);
            emailBox.MaxLength = 254;
            emailBox.Name = "emailBox";
            emailBox.Size = new Size(156, 23);
            emailBox.TabIndex = 3;
            // 
            // passwordBox
            // 
            passwordBox.Location = new Point(125, 73);
            passwordBox.MaxLength = 128;
            passwordBox.Name = "passwordBox";
            passwordBox.PasswordChar = '*';
            passwordBox.Size = new Size(156, 23);
            passwordBox.TabIndex = 4;
            // 
            // ForgotPasswordForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(342, 184);
            Controls.Add(passwordBox);
            Controls.Add(emailBox);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(submitBtn);
            Name = "ForgotPasswordForm";
            Text = "ForgotPasswordForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button submitBtn;
        private Label label1;
        private Label label2;
        private TextBox emailBox;
        private TextBox passwordBox;
    }
}