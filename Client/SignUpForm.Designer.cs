namespace Client
{
    partial class SignUpForm
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
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            usernameBox = new TextBox();
            passwordBox = new TextBox();
            emailBox = new TextBox();
            signUpBtn = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(41, 35);
            label1.Name = "label1";
            label1.Size = new Size(39, 15);
            label1.TabIndex = 0;
            label1.Text = "Email:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(41, 73);
            label2.Name = "label2";
            label2.Size = new Size(63, 15);
            label2.TabIndex = 1;
            label2.Text = "Username:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(41, 114);
            label3.Name = "label3";
            label3.Size = new Size(60, 15);
            label3.TabIndex = 2;
            label3.Text = "Password:";
            // 
            // usernameBox
            // 
            usernameBox.Location = new Point(110, 70);
            usernameBox.MaxLength = 16;
            usernameBox.Name = "usernameBox";
            usernameBox.Size = new Size(163, 23);
            usernameBox.TabIndex = 3;
            // 
            // passwordBox
            // 
            passwordBox.Location = new Point(110, 111);
            passwordBox.Name = "passwordBox";
            passwordBox.PasswordChar = '*';
            passwordBox.Size = new Size(163, 23);
            passwordBox.TabIndex = 4;
            // 
            // emailBox
            // 
            emailBox.Location = new Point(110, 32);
            emailBox.MaxLength = 254;
            emailBox.Name = "emailBox";
            emailBox.Size = new Size(163, 23);
            emailBox.TabIndex = 5;
            // 
            // signUpBtn
            // 
            signUpBtn.Location = new Point(132, 157);
            signUpBtn.Name = "signUpBtn";
            signUpBtn.Size = new Size(75, 23);
            signUpBtn.TabIndex = 6;
            signUpBtn.Text = "Sign Up";
            signUpBtn.UseVisualStyleBackColor = true;
            signUpBtn.Click += signUpBtn_Click;
            // 
            // SignUpForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(341, 196);
            Controls.Add(signUpBtn);
            Controls.Add(emailBox);
            Controls.Add(passwordBox);
            Controls.Add(usernameBox);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "SignUpForm";
            Text = "SignUpForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox usernameBox;
        private TextBox passwordBox;
        private TextBox emailBox;
        private Button signUpBtn;
    }
}