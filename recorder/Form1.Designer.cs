
namespace recorder
{
    partial class Form1 
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnRecording = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.recSingal = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnRecording
            // 
            this.btnRecording.Location = new System.Drawing.Point(116, 12);
            this.btnRecording.Name = "btnRecording";
            this.btnRecording.Size = new System.Drawing.Size(75, 23);
            this.btnRecording.TabIndex = 1;
            this.btnRecording.Text = "REC";
            this.btnRecording.UseVisualStyleBackColor = true;
            this.btnRecording.Click += new System.EventHandler(this.btnRecording_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(229, 12);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop/Save";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // recSingal
            // 
            this.recSingal.AutoSize = true;
            this.recSingal.ForeColor = System.Drawing.Color.Gray;
            this.recSingal.Location = new System.Drawing.Point(39, 17);
            this.recSingal.Name = "recSingal";
            this.recSingal.Size = new System.Drawing.Size(17, 12);
            this.recSingal.TabIndex = 3;
            this.recSingal.Text = "●";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(316, 48);
            this.Controls.Add(this.recSingal);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnRecording);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "Recorder with Mouse Trace";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnRecording;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label recSingal;
    }
}

