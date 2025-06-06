namespace Client
{
    partial class CaptchaForm
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
            captchaPictureBox = new PictureBox();
            label1 = new Label();
            captchaTextBox = new TextBox();
            refreshBtn = new Button();
            submitBtn = new Button();
            captchaExpirationLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)captchaPictureBox).BeginInit();
            SuspendLayout();
            // 
            // captchaPictureBox
            // 
            captchaPictureBox.Location = new Point(12, 12);
            captchaPictureBox.Name = "captchaPictureBox";
            captchaPictureBox.Size = new Size(337, 209);
            captchaPictureBox.TabIndex = 0;
            captchaPictureBox.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(42, 246);
            label1.Name = "label1";
            label1.Size = new Size(49, 15);
            label1.TabIndex = 1;
            label1.Text = "Answer:";
            // 
            // captchaTextBox
            // 
            captchaTextBox.Location = new Point(97, 243);
            captchaTextBox.MaxLength = 6;
            captchaTextBox.Name = "captchaTextBox";
            captchaTextBox.Size = new Size(222, 23);
            captchaTextBox.TabIndex = 2;
            // 
            // refreshBtn
            // 
            refreshBtn.Location = new Point(42, 281);
            refreshBtn.Name = "refreshBtn";
            refreshBtn.Size = new Size(113, 23);
            refreshBtn.TabIndex = 3;
            refreshBtn.Text = "Refresh Captcha";
            refreshBtn.UseVisualStyleBackColor = true;
            refreshBtn.Click += refreshBtn_Click;
            // 
            // submitBtn
            // 
            submitBtn.Location = new Point(207, 281);
            submitBtn.Name = "submitBtn";
            submitBtn.Size = new Size(112, 23);
            submitBtn.TabIndex = 4;
            submitBtn.Text = "Submit Captcha";
            submitBtn.UseVisualStyleBackColor = true;
            submitBtn.Click += submitBtn_Click;
            // 
            // captchaExpirationLabel
            // 
            captchaExpirationLabel.AutoSize = true;
            captchaExpirationLabel.Location = new Point(116, 308);
            captchaExpirationLabel.Name = "captchaExpirationLabel";
            captchaExpirationLabel.Size = new Size(136, 15);
            captchaExpirationLabel.TabIndex = 5;
            captchaExpirationLabel.Text = "Captcha not created yet.";
            // 
            // CaptchaForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(371, 332);
            Controls.Add(captchaExpirationLabel);
            Controls.Add(submitBtn);
            Controls.Add(refreshBtn);
            Controls.Add(captchaTextBox);
            Controls.Add(label1);
            Controls.Add(captchaPictureBox);
            Name = "CaptchaForm";
            Text = "CaptchaForm";
            Load += CaptchaForm_Load;
            ((System.ComponentModel.ISupportInitialize)captchaPictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox captchaPictureBox;
        private Label label1;
        private TextBox captchaTextBox;
        private Button refreshBtn;
        private Button submitBtn;
        private Label captchaExpirationLabel;
    }
}