namespace shooting
{
    partial class Main
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.TimerLabel = new System.Windows.Forms.Label();
            this.Emark = new System.Windows.Forms.Label();
            this.Enum = new System.Windows.Forms.Label();
            this.Smark = new System.Windows.Forms.Label();
            this.Snum = new System.Windows.Forms.Label();
            this.HPmark = new System.Windows.Forms.Label();
            this.HPnum = new System.Windows.Forms.Label();
            this.Highscore = new System.Windows.Forms.Label();
            this.Highmark = new System.Windows.Forms.Label();
            this.Q = new System.Windows.Forms.Label();
            this.D1 = new System.Windows.Forms.Label();
            this.D2 = new System.Windows.Forms.Label();
            this.D3 = new System.Windows.Forms.Label();
            this.D4 = new System.Windows.Forms.Label();
            this.amonum = new System.Windows.Forms.Label();
            this.death_b = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TimerLabel
            // 
            this.TimerLabel.BackColor = System.Drawing.Color.Transparent;
            this.TimerLabel.Font = new System.Drawing.Font("Impact", 39.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TimerLabel.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.TimerLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.TimerLabel.Location = new System.Drawing.Point(390, 1124);
            this.TimerLabel.Name = "TimerLabel";
            this.TimerLabel.Size = new System.Drawing.Size(399, 97);
            this.TimerLabel.TabIndex = 0;
            this.TimerLabel.Text = "--:--:--.--";
            this.TimerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Emark
            // 
            this.Emark.AutoSize = true;
            this.Emark.BackColor = System.Drawing.Color.Transparent;
            this.Emark.Font = new System.Drawing.Font("Impact", 39.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Emark.ForeColor = System.Drawing.Color.White;
            this.Emark.Location = new System.Drawing.Point(924, 763);
            this.Emark.Name = "Emark";
            this.Emark.Size = new System.Drawing.Size(146, 98);
            this.Emark.TabIndex = 1;
            this.Emark.Text = "E × ";
            // 
            // Enum
            // 
            this.Enum.BackColor = System.Drawing.Color.Transparent;
            this.Enum.Font = new System.Drawing.Font("Impact", 39.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Enum.ForeColor = System.Drawing.Color.White;
            this.Enum.Location = new System.Drawing.Point(1075, 875);
            this.Enum.Name = "Enum";
            this.Enum.Size = new System.Drawing.Size(151, 67);
            this.Enum.TabIndex = 2;
            this.Enum.Text = "10/10";
            this.Enum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Smark
            // 
            this.Smark.AutoSize = true;
            this.Smark.BackColor = System.Drawing.Color.Transparent;
            this.Smark.Font = new System.Drawing.Font("Impact", 39.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Smark.ForeColor = System.Drawing.Color.White;
            this.Smark.Location = new System.Drawing.Point(947, 999);
            this.Smark.Name = "Smark";
            this.Smark.Size = new System.Drawing.Size(234, 98);
            this.Smark.TabIndex = 3;
            this.Smark.Text = "STAGE";
            // 
            // Snum
            // 
            this.Snum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Snum.BackColor = System.Drawing.Color.Transparent;
            this.Snum.Font = new System.Drawing.Font("Impact", 39.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Snum.ForeColor = System.Drawing.Color.White;
            this.Snum.Location = new System.Drawing.Point(1050, 1109);
            this.Snum.Name = "Snum";
            this.Snum.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Snum.Size = new System.Drawing.Size(146, 65);
            this.Snum.TabIndex = 4;
            this.Snum.Text = "-/-";
            this.Snum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // HPmark
            // 
            this.HPmark.BackColor = System.Drawing.Color.Transparent;
            this.HPmark.Font = new System.Drawing.Font("Impact", 39.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HPmark.ForeColor = System.Drawing.Color.White;
            this.HPmark.Location = new System.Drawing.Point(311, 859);
            this.HPmark.Name = "HPmark";
            this.HPmark.Size = new System.Drawing.Size(100, 70);
            this.HPmark.TabIndex = 5;
            this.HPmark.Text = "HP";
            this.HPmark.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // HPnum
            // 
            this.HPnum.BackColor = System.Drawing.Color.Transparent;
            this.HPnum.Font = new System.Drawing.Font("Impact", 39.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HPnum.ForeColor = System.Drawing.Color.White;
            this.HPnum.Location = new System.Drawing.Point(697, 875);
            this.HPnum.Name = "HPnum";
            this.HPnum.Size = new System.Drawing.Size(119, 72);
            this.HPnum.TabIndex = 6;
            this.HPnum.Text = "100";
            this.HPnum.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Highscore
            // 
            this.Highscore.BackColor = System.Drawing.Color.Transparent;
            this.Highscore.Font = new System.Drawing.Font("Impact", 20.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Highscore.ForeColor = System.Drawing.Color.GreenYellow;
            this.Highscore.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.Highscore.Location = new System.Drawing.Point(574, 1022);
            this.Highscore.Name = "Highscore";
            this.Highscore.Size = new System.Drawing.Size(144, 50);
            this.Highscore.TabIndex = 7;
            this.Highscore.Text = "--:--:--.--";
            this.Highscore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Highmark
            // 
            this.Highmark.BackColor = System.Drawing.Color.Transparent;
            this.Highmark.Font = new System.Drawing.Font("Impact", 20.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Highmark.ForeColor = System.Drawing.Color.GreenYellow;
            this.Highmark.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.Highmark.Location = new System.Drawing.Point(367, 1022);
            this.Highmark.Name = "Highmark";
            this.Highmark.Size = new System.Drawing.Size(159, 50);
            this.Highmark.TabIndex = 8;
            this.Highmark.Text = "Highscore";
            this.Highmark.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Q
            // 
            this.Q.BackColor = System.Drawing.Color.Transparent;
            this.Q.Font = new System.Drawing.Font("Impact", 20.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Q.ForeColor = System.Drawing.Color.White;
            this.Q.Location = new System.Drawing.Point(42, 1197);
            this.Q.Name = "Q";
            this.Q.Size = new System.Drawing.Size(43, 39);
            this.Q.TabIndex = 9;
            this.Q.Text = "Q";
            this.Q.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // D1
            // 
            this.D1.BackColor = System.Drawing.Color.Transparent;
            this.D1.Font = new System.Drawing.Font("Impact", 20.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.D1.ForeColor = System.Drawing.Color.White;
            this.D1.Location = new System.Drawing.Point(130, 1135);
            this.D1.Name = "D1";
            this.D1.Size = new System.Drawing.Size(43, 39);
            this.D1.TabIndex = 10;
            this.D1.Text = "1";
            this.D1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // D2
            // 
            this.D2.BackColor = System.Drawing.Color.Transparent;
            this.D2.Font = new System.Drawing.Font("Impact", 20.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.D2.ForeColor = System.Drawing.Color.White;
            this.D2.Location = new System.Drawing.Point(190, 1135);
            this.D2.Name = "D2";
            this.D2.Size = new System.Drawing.Size(43, 39);
            this.D2.TabIndex = 11;
            this.D2.Text = "2";
            this.D2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // D3
            // 
            this.D3.BackColor = System.Drawing.Color.Transparent;
            this.D3.Font = new System.Drawing.Font("Impact", 20.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.D3.ForeColor = System.Drawing.Color.White;
            this.D3.Location = new System.Drawing.Point(159, 1200);
            this.D3.Name = "D3";
            this.D3.Size = new System.Drawing.Size(43, 39);
            this.D3.TabIndex = 12;
            this.D3.Text = "3";
            this.D3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // D4
            // 
            this.D4.BackColor = System.Drawing.Color.Transparent;
            this.D4.Font = new System.Drawing.Font("Impact", 20.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.D4.ForeColor = System.Drawing.Color.White;
            this.D4.Location = new System.Drawing.Point(228, 1200);
            this.D4.Name = "D4";
            this.D4.Size = new System.Drawing.Size(43, 39);
            this.D4.TabIndex = 13;
            this.D4.Text = "4";
            this.D4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // amonum
            // 
            this.amonum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.amonum.BackColor = System.Drawing.Color.Transparent;
            this.amonum.Font = new System.Drawing.Font("Impact", 39.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.amonum.ForeColor = System.Drawing.Color.White;
            this.amonum.Location = new System.Drawing.Point(136, 948);
            this.amonum.Name = "amonum";
            this.amonum.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.amonum.Size = new System.Drawing.Size(189, 72);
            this.amonum.TabIndex = 14;
            this.amonum.Text = "99 / 99";
            this.amonum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // death_b
            // 
            this.death_b.Font = new System.Drawing.Font("MS UI Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.death_b.Location = new System.Drawing.Point(555, 500);
            this.death_b.Name = "death_b";
            this.death_b.Size = new System.Drawing.Size(200, 45);
            this.death_b.TabIndex = 16;
            this.death_b.Text = "RESTART";
            this.death_b.UseVisualStyleBackColor = true;
            this.death_b.Click += new System.EventHandler(this.death_b_Click);
            // 
            // Main
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(1284, 1061);
            this.Controls.Add(this.amonum);
            this.Controls.Add(this.Emark);
            this.Controls.Add(this.Enum);
            this.Controls.Add(this.HPmark);
            this.Controls.Add(this.death_b);
            this.Controls.Add(this.D4);
            this.Controls.Add(this.D3);
            this.Controls.Add(this.D2);
            this.Controls.Add(this.D1);
            this.Controls.Add(this.Q);
            this.Controls.Add(this.Highmark);
            this.Controls.Add(this.Highscore);
            this.Controls.Add(this.HPnum);
            this.Controls.Add(this.Snum);
            this.Controls.Add(this.Smark);
            this.Controls.Add(this.TimerLabel);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Main";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Main_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Main_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label TimerLabel;
        private System.Windows.Forms.Label Emark;
        private System.Windows.Forms.Label Enum;
        private System.Windows.Forms.Label Smark;
        private System.Windows.Forms.Label Snum;
        private System.Windows.Forms.Label HPmark;
        private System.Windows.Forms.Label HPnum;
        private System.Windows.Forms.Label Highscore;
        private System.Windows.Forms.Label Highmark;
        private System.Windows.Forms.Label Q;
        private System.Windows.Forms.Label D1;
        private System.Windows.Forms.Label D2;
        private System.Windows.Forms.Label D3;
        private System.Windows.Forms.Label D4;
        private System.Windows.Forms.Label amonum;
        private System.Windows.Forms.Button death_b;
    }
}

