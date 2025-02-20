namespace ClientGameApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            loginTextBox = new MaterialSkin.Controls.MaterialMaskedTextBox();
            loginBtn = new MaterialSkin.Controls.MaterialButton();
            SuspendLayout();
            // 
            // loginTextBox
            // 
            loginTextBox.AllowPromptAsInput = true;
            loginTextBox.AnimateReadOnly = false;
            loginTextBox.AsciiOnly = false;
            loginTextBox.BackgroundImageLayout = ImageLayout.None;
            loginTextBox.BeepOnError = false;
            loginTextBox.CutCopyMaskFormat = MaskFormat.IncludeLiterals;
            loginTextBox.Depth = 0;
            loginTextBox.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            loginTextBox.HidePromptOnLeave = false;
            loginTextBox.HideSelection = true;
            loginTextBox.InsertKeyMode = InsertKeyMode.Default;
            loginTextBox.LeadingIcon = null;
            loginTextBox.Location = new Point(202, 133);
            loginTextBox.Mask = "";
            loginTextBox.MaxLength = 32767;
            loginTextBox.MouseState = MaterialSkin.MouseState.OUT;
            loginTextBox.Name = "loginTextBox";
            loginTextBox.PasswordChar = '\0';
            loginTextBox.PrefixSuffix = MaterialSkin.Controls.MaterialMaskedTextBox.PrefixSuffixTypes.Prefix;
            loginTextBox.PrefixSuffixText = "Enter Your Name";
            loginTextBox.PromptChar = '_';
            loginTextBox.ReadOnly = false;
            loginTextBox.RejectInputOnFirstFailure = false;
            loginTextBox.ResetOnPrompt = true;
            loginTextBox.ResetOnSpace = true;
            loginTextBox.RightToLeft = RightToLeft.No;
            loginTextBox.SelectedText = "";
            loginTextBox.SelectionLength = 0;
            loginTextBox.SelectionStart = 0;
            loginTextBox.ShortcutsEnabled = true;
            loginTextBox.Size = new Size(405, 48);
            loginTextBox.SkipLiterals = true;
            loginTextBox.TabIndex = 0;
            loginTextBox.TabStop = false;
            loginTextBox.TextAlign = HorizontalAlignment.Left;
            loginTextBox.TextMaskFormat = MaskFormat.IncludeLiterals;
            loginTextBox.TrailingIcon = null;
            loginTextBox.UseSystemPasswordChar = false;
            loginTextBox.ValidatingType = null;
            // 
            // loginBtn
            // 
            loginBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            loginBtn.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            loginBtn.Depth = 0;
            loginBtn.HighEmphasis = true;
            loginBtn.Icon = null;
            loginBtn.Location = new Point(325, 271);
            loginBtn.Margin = new Padding(4, 6, 4, 6);
            loginBtn.MouseState = MaterialSkin.MouseState.HOVER;
            loginBtn.Name = "loginBtn";
            loginBtn.NoAccentTextColor = Color.Empty;
            loginBtn.Size = new Size(64, 36);
            loginBtn.TabIndex = 1;
            loginBtn.Text = "Login";
            loginBtn.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            loginBtn.UseAccentColor = false;
            loginBtn.UseVisualStyleBackColor = true;
            loginBtn.Click += loginBtn_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(loginBtn);
            Controls.Add(loginTextBox);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MaterialSkin.Controls.MaterialMaskedTextBox loginTextBox;
        private MaterialSkin.Controls.MaterialButton loginBtn;
    }
}
