namespace Client
{
    partial class LoginForm
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
            loginBtn = new Button();
            label1 = new Label();
            label2 = new Label();
            signUpLinkLabel = new LinkLabel();
            forgotPasswordLinkLabel = new LinkLabel();
            usernameBox = new TextBox();
            passwordBox = new TextBox();
            SuspendLayout();
            // 
            // loginBtn
            // 
            loginBtn.Location = new Point(96, 145);
            loginBtn.Name = "loginBtn";
            loginBtn.Size = new Size(75, 23);
            loginBtn.TabIndex = 0;
            loginBtn.Text = "Login";
            loginBtn.UseVisualStyleBackColor = true;
            loginBtn.Click += loginBtn_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(23, 64);
            label1.Name = "label1";
            label1.Size = new Size(63, 15);
            label1.TabIndex = 1;
            label1.Text = "Username:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(23, 106);
            label2.Name = "label2";
            label2.Size = new Size(60, 15);
            label2.TabIndex = 2;
            label2.Text = "Password:";
            // 
            // signUpLinkLabel
            // 
            signUpLinkLabel.AutoSize = true;
            signUpLinkLabel.Location = new Point(109, 184);
            signUpLinkLabel.Name = "signUpLinkLabel";
            signUpLinkLabel.Size = new Size(48, 15);
            signUpLinkLabel.TabIndex = 3;
            signUpLinkLabel.TabStop = true;
            signUpLinkLabel.Text = "Sign Up";
            signUpLinkLabel.LinkClicked += signUpLinkLabel_LinkClicked;
            // 
            // forgotPasswordLinkLabel
            // 
            forgotPasswordLinkLabel.AutoSize = true;
            forgotPasswordLinkLabel.Location = new Point(85, 210);
            forgotPasswordLinkLabel.Name = "forgotPasswordLinkLabel";
            forgotPasswordLinkLabel.Size = new Size(100, 15);
            forgotPasswordLinkLabel.TabIndex = 4;
            forgotPasswordLinkLabel.TabStop = true;
            forgotPasswordLinkLabel.Text = "Forgot Password?";
            forgotPasswordLinkLabel.LinkClicked += forgotPasswordLinkLabel_LinkClicked;
            // 
            // usernameBox
            // 
            usernameBox.Location = new Point(102, 61);
            usernameBox.MaxLength = 16;
            usernameBox.Name = "usernameBox";
            usernameBox.Size = new Size(150, 23);
            usernameBox.TabIndex = 5;
            // 
            // passwordBox
            // 
            passwordBox.Location = new Point(102, 103);
            passwordBox.MaxLength = 128;
            passwordBox.Name = "passwordBox";
            passwordBox.PasswordChar = '*';
            passwordBox.Size = new Size(150, 23);
            passwordBox.TabIndex = 6;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(286, 251);
            Controls.Add(passwordBox);
            Controls.Add(usernameBox);
            Controls.Add(forgotPasswordLinkLabel);
            Controls.Add(signUpLinkLabel);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(loginBtn);
            Name = "LoginForm";
            Text = "LoginForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button loginBtn;
        private Label label1;
        private Label label2;
        private LinkLabel signUpLinkLabel;
        private LinkLabel forgotPasswordLinkLabel;
        private TextBox usernameBox;
        private TextBox passwordBox;
    }
}