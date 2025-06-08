namespace Client
{
    partial class MainForm
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
            components = new System.ComponentModel.Container();
            mediaListView = new ListView();
            thumbnailImageList = new ImageList(components);
            playBtn = new Button();
            timeLabel = new Label();
            searchBox = new TextBox();
            label2 = new Label();
            pauseBtn = new Button();
            addMediaButton = new Button();
            removeMediaBtn = new Button();
            SuspendLayout();
            // 
            // mediaListView
            // 
            mediaListView.LargeImageList = thumbnailImageList;
            mediaListView.Location = new Point(12, 52);
            mediaListView.MultiSelect = false;
            mediaListView.Name = "mediaListView";
            mediaListView.Size = new Size(810, 424);
            mediaListView.TabIndex = 0;
            mediaListView.UseCompatibleStateImageBehavior = false;
            // 
            // thumbnailImageList
            // 
            thumbnailImageList.ColorDepth = ColorDepth.Depth32Bit;
            thumbnailImageList.ImageSize = new Size(256, 256);
            thumbnailImageList.TransparentColor = Color.Transparent;
            // 
            // playBtn
            // 
            playBtn.Location = new Point(12, 490);
            playBtn.Name = "playBtn";
            playBtn.Size = new Size(75, 23);
            playBtn.TabIndex = 1;
            playBtn.Text = "Play";
            playBtn.UseVisualStyleBackColor = true;
            playBtn.Click += playBtn_Click;
            // 
            // timeLabel
            // 
            timeLabel.AutoSize = true;
            timeLabel.Location = new Point(356, 586);
            timeLabel.Name = "timeLabel";
            timeLabel.Size = new Size(0, 15);
            timeLabel.TabIndex = 5;
            timeLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // searchBox
            // 
            searchBox.Location = new Point(303, 23);
            searchBox.Name = "searchBox";
            searchBox.Size = new Size(229, 23);
            searchBox.TabIndex = 6;
            searchBox.TextChanged += searchBox_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(252, 26);
            label2.Name = "label2";
            label2.Size = new Size(45, 15);
            label2.TabIndex = 7;
            label2.Text = "Search:";
            // 
            // pauseBtn
            // 
            pauseBtn.Enabled = false;
            pauseBtn.Location = new Point(356, 490);
            pauseBtn.Name = "pauseBtn";
            pauseBtn.Size = new Size(75, 23);
            pauseBtn.TabIndex = 8;
            pauseBtn.Text = "Pause";
            pauseBtn.UseVisualStyleBackColor = true;
            pauseBtn.Click += pauseBtn_Click;
            // 
            // addMediaButton
            // 
            addMediaButton.Location = new Point(700, 490);
            addMediaButton.Name = "addMediaButton";
            addMediaButton.Size = new Size(104, 23);
            addMediaButton.TabIndex = 9;
            addMediaButton.Text = "Add New Media";
            addMediaButton.UseVisualStyleBackColor = true;
            addMediaButton.Visible = false;
            addMediaButton.Click += addMediaButton_Click;
            // 
            // removeMediaBtn
            // 
            removeMediaBtn.Location = new Point(598, 490);
            removeMediaBtn.Name = "removeMediaBtn";
            removeMediaBtn.Size = new Size(96, 23);
            removeMediaBtn.TabIndex = 10;
            removeMediaBtn.Text = "Remove Media";
            removeMediaBtn.UseVisualStyleBackColor = true;
            removeMediaBtn.Visible = false;
            removeMediaBtn.Click += removeMediaBtn_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(834, 523);
            Controls.Add(removeMediaBtn);
            Controls.Add(addMediaButton);
            Controls.Add(pauseBtn);
            Controls.Add(label2);
            Controls.Add(searchBox);
            Controls.Add(timeLabel);
            Controls.Add(playBtn);
            Controls.Add(mediaListView);
            Name = "MainForm";
            Text = "MainForm";
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView mediaListView;
        private Button playBtn;
        private Label timeLabel;
        private TextBox searchBox;
        private Label label2;
        private ImageList thumbnailImageList;
        private Button pauseBtn;
        private Button addMediaButton;
        private Button removeMediaBtn;
    }
}