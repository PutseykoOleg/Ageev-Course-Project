
namespace TextLibrary
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.textArea = new System.Windows.Forms.TextBox();
            this.listOfSentences = new System.Windows.Forms.TextBox();
            this.listOfWords = new System.Windows.Forms.TextBox();
            this.readSentenceButton = new System.Windows.Forms.Button();
            this.readWordButton = new System.Windows.Forms.Button();
            this.readTextButton = new System.Windows.Forms.Button();
            this.writeButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.errorMessage = new System.Windows.Forms.Label();
            this.correctButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textArea
            // 
            this.textArea.Location = new System.Drawing.Point(50, 22);
            this.textArea.Multiline = true;
            this.textArea.Name = "textArea";
            this.textArea.Size = new System.Drawing.Size(448, 347);
            this.textArea.TabIndex = 0;
            this.textArea.TextChanged += new System.EventHandler(this.textArea_TextChanged);
            // 
            // listOfSentences
            // 
            this.listOfSentences.Location = new System.Drawing.Point(528, 57);
            this.listOfSentences.Multiline = true;
            this.listOfSentences.Name = "listOfSentences";
            this.listOfSentences.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.listOfSentences.Size = new System.Drawing.Size(251, 137);
            this.listOfSentences.TabIndex = 1;
            // 
            // listOfWords
            // 
            this.listOfWords.Location = new System.Drawing.Point(528, 230);
            this.listOfWords.Multiline = true;
            this.listOfWords.Name = "listOfWords";
            this.listOfWords.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.listOfWords.Size = new System.Drawing.Size(251, 139);
            this.listOfWords.TabIndex = 2;
            // 
            // readSentenceButton
            // 
            this.readSentenceButton.Location = new System.Drawing.Point(50, 442);
            this.readSentenceButton.Name = "readSentenceButton";
            this.readSentenceButton.Size = new System.Drawing.Size(189, 34);
            this.readSentenceButton.TabIndex = 3;
            this.readSentenceButton.Text = "Считать предложение";
            this.readSentenceButton.UseVisualStyleBackColor = true;
            this.readSentenceButton.Click += new System.EventHandler(this.readSentenceButton_Click);
            // 
            // readWordButton
            // 
            this.readWordButton.Location = new System.Drawing.Point(245, 442);
            this.readWordButton.Name = "readWordButton";
            this.readWordButton.Size = new System.Drawing.Size(189, 34);
            this.readWordButton.TabIndex = 4;
            this.readWordButton.Text = "Считать слово";
            this.readWordButton.UseVisualStyleBackColor = true;
            this.readWordButton.Click += new System.EventHandler(this.readWordButton_Click);
            // 
            // readTextButton
            // 
            this.readTextButton.Location = new System.Drawing.Point(50, 482);
            this.readTextButton.Name = "readTextButton";
            this.readTextButton.Size = new System.Drawing.Size(384, 34);
            this.readTextButton.TabIndex = 5;
            this.readTextButton.Text = "Считать весь текст";
            this.readTextButton.UseVisualStyleBackColor = true;
            this.readTextButton.Click += new System.EventHandler(this.readTextButton_Click);
            // 
            // writeButton
            // 
            this.writeButton.Location = new System.Drawing.Point(590, 442);
            this.writeButton.Name = "writeButton";
            this.writeButton.Size = new System.Drawing.Size(189, 34);
            this.writeButton.TabIndex = 6;
            this.writeButton.Text = "Записать все в файл";
            this.writeButton.UseVisualStyleBackColor = true;
            this.writeButton.Click += new System.EventHandler(this.writeButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(525, 203);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "Найденные слова:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(525, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(181, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "Найденные предложения:";
            // 
            // errorMessage
            // 
            this.errorMessage.AutoSize = true;
            this.errorMessage.ForeColor = System.Drawing.Color.Red;
            this.errorMessage.Location = new System.Drawing.Point(50, 392);
            this.errorMessage.Name = "errorMessage";
            this.errorMessage.Size = new System.Drawing.Size(0, 17);
            this.errorMessage.TabIndex = 9;
            // 
            // correctButton
            // 
            this.correctButton.Location = new System.Drawing.Point(590, 482);
            this.correctButton.Name = "correctButton";
            this.correctButton.Size = new System.Drawing.Size(189, 34);
            this.correctButton.TabIndex = 10;
            this.correctButton.Text = "Исправить";
            this.correctButton.UseVisualStyleBackColor = true;
            this.correctButton.Click += new System.EventHandler(this.correctButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(829, 544);
            this.Controls.Add(this.correctButton);
            this.Controls.Add(this.errorMessage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.writeButton);
            this.Controls.Add(this.readTextButton);
            this.Controls.Add(this.readWordButton);
            this.Controls.Add(this.readSentenceButton);
            this.Controls.Add(this.listOfWords);
            this.Controls.Add(this.listOfSentences);
            this.Controls.Add(this.textArea);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Course Project";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textArea;
        private System.Windows.Forms.TextBox listOfSentences;
        private System.Windows.Forms.TextBox listOfWords;
        private System.Windows.Forms.Button readSentenceButton;
        private System.Windows.Forms.Button readWordButton;
        private System.Windows.Forms.Button readTextButton;
        private System.Windows.Forms.Button writeButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label errorMessage;
        private System.Windows.Forms.Button correctButton;
    }
}

