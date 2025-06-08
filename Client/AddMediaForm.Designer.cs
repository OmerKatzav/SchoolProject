namespace Client
{
    partial class AddMediaForm
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
            pickThumbnailBtn = new Button();
            thumbnailPictureBox = new PictureBox();
            pickMediaBtn = new Button();
            UploadBtn = new Button();
            thumbnailFileLabel = new Label();
            mediaFileLabel = new Label();
            label1 = new Label();
            nameTextBox = new TextBox();
            ((System.ComponentModel.ISupportInitialize)thumbnailPictureBox).BeginInit();
            SuspendLayout();
            // 
            // pickThumbnailBtn
            // 
            pickThumbnailBtn.Location = new Point(12, 525);
            pickThumbnailBtn.Name = "pickThumbnailBtn";
            pickThumbnailBtn.Size = new Size(101, 23);
            pickThumbnailBtn.TabIndex = 0;
            pickThumbnailBtn.Text = "Pick Thumbnail";
            pickThumbnailBtn.UseVisualStyleBackColor = true;
            pickThumbnailBtn.Click += pickThumbnailBtn_Click;
            // 
            // thumbnailPictureBox
            // 
            thumbnailPictureBox.Location = new Point(12, 12);
            thumbnailPictureBox.Name = "thumbnailPictureBox";
            thumbnailPictureBox.Size = new Size(458, 458);
            thumbnailPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            thumbnailPictureBox.TabIndex = 1;
            thumbnailPictureBox.TabStop = false;
            // 
            // pickMediaBtn
            // 
            pickMediaBtn.Location = new Point(12, 566);
            pickMediaBtn.Name = "pickMediaBtn";
            pickMediaBtn.Size = new Size(101, 23);
            pickMediaBtn.TabIndex = 2;
            pickMediaBtn.Text = "Pick Media";
            pickMediaBtn.UseVisualStyleBackColor = true;
            pickMediaBtn.Click += pickMediaBtn_Click;
            // 
            // UploadBtn
            // 
            UploadBtn.Location = new Point(199, 608);
            UploadBtn.Name = "UploadBtn";
            UploadBtn.Size = new Size(75, 23);
            UploadBtn.TabIndex = 3;
            UploadBtn.Text = "Upload";
            UploadBtn.UseVisualStyleBackColor = true;
            UploadBtn.Click += UploadBtn_Click;
            // 
            // thumbnailFileLabel
            // 
            thumbnailFileLabel.AutoSize = true;
            thumbnailFileLabel.Location = new Point(129, 529);
            thumbnailFileLabel.Name = "thumbnailFileLabel";
            thumbnailFileLabel.Size = new Size(0, 15);
            thumbnailFileLabel.TabIndex = 4;
            // 
            // mediaFileLabel
            // 
            mediaFileLabel.AutoSize = true;
            mediaFileLabel.Location = new Point(129, 570);
            mediaFileLabel.Name = "mediaFileLabel";
            mediaFileLabel.Size = new Size(0, 15);
            mediaFileLabel.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 495);
            label1.Name = "label1";
            label1.Size = new Size(78, 15);
            label1.TabIndex = 6;
            label1.Text = "Media Name:";
            // 
            // nameTextBox
            // 
            nameTextBox.Location = new Point(96, 492);
            nameTextBox.Name = "nameTextBox";
            nameTextBox.Size = new Size(374, 23);
            nameTextBox.TabIndex = 7;
            // 
            // AddMediaForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(482, 700);
            Controls.Add(nameTextBox);
            Controls.Add(label1);
            Controls.Add(mediaFileLabel);
            Controls.Add(thumbnailFileLabel);
            Controls.Add(UploadBtn);
            Controls.Add(pickMediaBtn);
            Controls.Add(thumbnailPictureBox);
            Controls.Add(pickThumbnailBtn);
            Name = "AddMediaForm";
            Text = "AddMediaForm";
            ((System.ComponentModel.ISupportInitialize)thumbnailPictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button pickThumbnailBtn;
        private PictureBox thumbnailPictureBox;
        private Button pickMediaBtn;
        private Button UploadBtn;
        private Label thumbnailFileLabel;
        private Label mediaFileLabel;
        private Label label1;
        private TextBox nameTextBox;
    }
}