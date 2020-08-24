namespace Localizer
{
    partial class formMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCHRRAM2CHRROM = new System.Windows.Forms.Button();
            this.btnInit = new System.Windows.Forms.Button();
            this.btnCopyBankTo6000 = new System.Windows.Forms.Button();
            this.btnLoadInclude = new System.Windows.Forms.Button();
            this.btnHijackInterrupt = new System.Windows.Forms.Button();
            this.btnPatchWriteE000 = new System.Windows.Forms.Button();
            this.btnImportCHR = new System.Windows.Forms.Button();
            this.btnSwitchCHR = new System.Windows.Forms.Button();
            this.btnPatchJSRDrawVRAM = new System.Windows.Forms.Button();
            this.btnSwitchCHRForIntro = new System.Windows.Forms.Button();
            this.btnRebuildIntroNametable = new System.Windows.Forms.Button();
            this.btnImportFontForIntro = new System.Windows.Forms.Button();
            this.btnIntroLocalization = new System.Windows.Forms.Button();
            this.btnInGameHUDLocalization = new System.Windows.Forms.Button();
            this.btnImportFontForStageClear = new System.Windows.Forms.Button();
            this.btnStageClearLocalization = new System.Windows.Forms.Button();
            this.btnEndingLocalization = new System.Windows.Forms.Button();
            this.btnImportFontForEnding = new System.Windows.Forms.Button();
            this.btnGameOverLocalization = new System.Windows.Forms.Button();
            this.btnImportFontForGameOver = new System.Windows.Forms.Button();
            this.btnCreateNewROM = new System.Windows.Forms.Button();
            this.btnStaffLocalization = new System.Windows.Forms.Button();
            this.btnImportFontForStaff = new System.Windows.Forms.Button();
            this.btnStaffLocalization2 = new System.Windows.Forms.Button();
            this.btnTitleLocalization = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCHRRAM2CHRROM
            // 
            this.btnCHRRAM2CHRROM.Location = new System.Drawing.Point(12, 12);
            this.btnCHRRAM2CHRROM.Name = "btnCHRRAM2CHRROM";
            this.btnCHRRAM2CHRROM.Size = new System.Drawing.Size(118, 40);
            this.btnCHRRAM2CHRROM.TabIndex = 0;
            this.btnCHRRAM2CHRROM.Text = "改mapper为19且CHRRAM转CHRROM";
            this.btnCHRRAM2CHRROM.UseVisualStyleBackColor = true;
            this.btnCHRRAM2CHRROM.Click += new System.EventHandler(this.btnCHRRAM2CHRROM_Click);
            // 
            // btnInit
            // 
            this.btnInit.BackColor = System.Drawing.Color.Red;
            this.btnInit.Location = new System.Drawing.Point(12, 104);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(118, 40);
            this.btnInit.TabIndex = 1;
            this.btnInit.Text = "导入核心代码：Init";
            this.btnInit.UseVisualStyleBackColor = false;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // btnCopyBankTo6000
            // 
            this.btnCopyBankTo6000.BackColor = System.Drawing.Color.Red;
            this.btnCopyBankTo6000.Location = new System.Drawing.Point(12, 150);
            this.btnCopyBankTo6000.Name = "btnCopyBankTo6000";
            this.btnCopyBankTo6000.Size = new System.Drawing.Size(118, 40);
            this.btnCopyBankTo6000.TabIndex = 2;
            this.btnCopyBankTo6000.Text = "导入核心代码：CopyBankTo6000";
            this.btnCopyBankTo6000.UseVisualStyleBackColor = false;
            this.btnCopyBankTo6000.Click += new System.EventHandler(this.btnCopyBankTo6000_Click);
            // 
            // btnLoadInclude
            // 
            this.btnLoadInclude.BackColor = System.Drawing.Color.Red;
            this.btnLoadInclude.Location = new System.Drawing.Point(12, 58);
            this.btnLoadInclude.Name = "btnLoadInclude";
            this.btnLoadInclude.Size = new System.Drawing.Size(118, 40);
            this.btnLoadInclude.TabIndex = 3;
            this.btnLoadInclude.Text = "载入标签表";
            this.btnLoadInclude.UseVisualStyleBackColor = false;
            this.btnLoadInclude.Click += new System.EventHandler(this.btnLoadInclude_Click);
            // 
            // btnHijackInterrupt
            // 
            this.btnHijackInterrupt.BackColor = System.Drawing.Color.Red;
            this.btnHijackInterrupt.Location = new System.Drawing.Point(12, 196);
            this.btnHijackInterrupt.Name = "btnHijackInterrupt";
            this.btnHijackInterrupt.Size = new System.Drawing.Size(118, 40);
            this.btnHijackInterrupt.TabIndex = 4;
            this.btnHijackInterrupt.Text = "导入核心代码：HijackInterrupt";
            this.btnHijackInterrupt.UseVisualStyleBackColor = false;
            this.btnHijackInterrupt.Click += new System.EventHandler(this.btnHijackInterrupt_Click);
            // 
            // btnPatchWriteE000
            // 
            this.btnPatchWriteE000.Location = new System.Drawing.Point(136, 12);
            this.btnPatchWriteE000.Name = "btnPatchWriteE000";
            this.btnPatchWriteE000.Size = new System.Drawing.Size(118, 40);
            this.btnPatchWriteE000.TabIndex = 5;
            this.btnPatchWriteE000.Text = "写$E000改为jsr CopyBankTo6000";
            this.btnPatchWriteE000.UseVisualStyleBackColor = true;
            this.btnPatchWriteE000.Click += new System.EventHandler(this.btnPatchWriteE000_Click);
            // 
            // btnImportCHR
            // 
            this.btnImportCHR.Location = new System.Drawing.Point(136, 58);
            this.btnImportCHR.Name = "btnImportCHR";
            this.btnImportCHR.Size = new System.Drawing.Size(118, 40);
            this.btnImportCHR.TabIndex = 6;
            this.btnImportCHR.Text = "导入所有CHR";
            this.btnImportCHR.UseVisualStyleBackColor = true;
            this.btnImportCHR.Click += new System.EventHandler(this.btnImportCHR_Click);
            // 
            // btnSwitchCHR
            // 
            this.btnSwitchCHR.BackColor = System.Drawing.Color.Red;
            this.btnSwitchCHR.Location = new System.Drawing.Point(136, 104);
            this.btnSwitchCHR.Name = "btnSwitchCHR";
            this.btnSwitchCHR.Size = new System.Drawing.Size(118, 40);
            this.btnSwitchCHR.TabIndex = 7;
            this.btnSwitchCHR.Text = "导入核心代码：SwitchCHR";
            this.btnSwitchCHR.UseVisualStyleBackColor = false;
            this.btnSwitchCHR.Click += new System.EventHandler(this.btnSwitchCHR_Click);
            // 
            // btnPatchJSRDrawVRAM
            // 
            this.btnPatchJSRDrawVRAM.Location = new System.Drawing.Point(136, 150);
            this.btnPatchJSRDrawVRAM.Name = "btnPatchJSRDrawVRAM";
            this.btnPatchJSRDrawVRAM.Size = new System.Drawing.Size(118, 40);
            this.btnPatchJSRDrawVRAM.TabIndex = 8;
            this.btnPatchJSRDrawVRAM.Text = "jsr DrawVRAM改为jsr SwitchCHR";
            this.btnPatchJSRDrawVRAM.UseVisualStyleBackColor = true;
            this.btnPatchJSRDrawVRAM.Click += new System.EventHandler(this.btnPatchJSRDrawVRAM_Click);
            // 
            // btnSwitchCHRForIntro
            // 
            this.btnSwitchCHRForIntro.BackColor = System.Drawing.Color.Red;
            this.btnSwitchCHRForIntro.Location = new System.Drawing.Point(136, 196);
            this.btnSwitchCHRForIntro.Name = "btnSwitchCHRForIntro";
            this.btnSwitchCHRForIntro.Size = new System.Drawing.Size(118, 40);
            this.btnSwitchCHRForIntro.TabIndex = 9;
            this.btnSwitchCHRForIntro.Text = "导入核心代码：SwitchCHRForIntro";
            this.btnSwitchCHRForIntro.UseVisualStyleBackColor = false;
            this.btnSwitchCHRForIntro.Click += new System.EventHandler(this.btnSwitchCHRForIntro_Click);
            // 
            // btnRebuildIntroNametable
            // 
            this.btnRebuildIntroNametable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnRebuildIntroNametable.Location = new System.Drawing.Point(260, 58);
            this.btnRebuildIntroNametable.Name = "btnRebuildIntroNametable";
            this.btnRebuildIntroNametable.Size = new System.Drawing.Size(118, 40);
            this.btnRebuildIntroNametable.TabIndex = 10;
            this.btnRebuildIntroNametable.Text = "重建intro界面的nametable";
            this.btnRebuildIntroNametable.UseVisualStyleBackColor = false;
            this.btnRebuildIntroNametable.Click += new System.EventHandler(this.btnRebuildIntroNametable_Click);
            // 
            // btnImportFontForIntro
            // 
            this.btnImportFontForIntro.Location = new System.Drawing.Point(260, 12);
            this.btnImportFontForIntro.Name = "btnImportFontForIntro";
            this.btnImportFontForIntro.Size = new System.Drawing.Size(118, 40);
            this.btnImportFontForIntro.TabIndex = 11;
            this.btnImportFontForIntro.Text = "intro界面CHR导入汉字字模";
            this.btnImportFontForIntro.UseVisualStyleBackColor = true;
            this.btnImportFontForIntro.Click += new System.EventHandler(this.btnImportFontForIntro_Click);
            // 
            // btnIntroLocalization
            // 
            this.btnIntroLocalization.BackColor = System.Drawing.Color.Red;
            this.btnIntroLocalization.Location = new System.Drawing.Point(260, 104);
            this.btnIntroLocalization.Name = "btnIntroLocalization";
            this.btnIntroLocalization.Size = new System.Drawing.Size(118, 40);
            this.btnIntroLocalization.TabIndex = 12;
            this.btnIntroLocalization.Text = "intro界面汉化文本";
            this.btnIntroLocalization.UseVisualStyleBackColor = false;
            this.btnIntroLocalization.Click += new System.EventHandler(this.btnIntroLocalization_Click);
            // 
            // btnInGameHUDLocalization
            // 
            this.btnInGameHUDLocalization.Location = new System.Drawing.Point(260, 150);
            this.btnInGameHUDLocalization.Name = "btnInGameHUDLocalization";
            this.btnInGameHUDLocalization.Size = new System.Drawing.Size(118, 40);
            this.btnInGameHUDLocalization.TabIndex = 13;
            this.btnInGameHUDLocalization.Text = "汉化游戏内HUD";
            this.btnInGameHUDLocalization.UseVisualStyleBackColor = true;
            this.btnInGameHUDLocalization.Click += new System.EventHandler(this.btnInGameHUDLocalization_Click);
            // 
            // btnImportFontForStageClear
            // 
            this.btnImportFontForStageClear.Location = new System.Drawing.Point(384, 12);
            this.btnImportFontForStageClear.Name = "btnImportFontForStageClear";
            this.btnImportFontForStageClear.Size = new System.Drawing.Size(118, 40);
            this.btnImportFontForStageClear.TabIndex = 14;
            this.btnImportFontForStageClear.Text = "过关界面CHR导入汉字字模";
            this.btnImportFontForStageClear.UseVisualStyleBackColor = true;
            this.btnImportFontForStageClear.Click += new System.EventHandler(this.btnImportFontForStageClear_Click);
            // 
            // btnStageClearLocalization
            // 
            this.btnStageClearLocalization.BackColor = System.Drawing.Color.Red;
            this.btnStageClearLocalization.Location = new System.Drawing.Point(384, 58);
            this.btnStageClearLocalization.Name = "btnStageClearLocalization";
            this.btnStageClearLocalization.Size = new System.Drawing.Size(118, 40);
            this.btnStageClearLocalization.TabIndex = 15;
            this.btnStageClearLocalization.Text = "过关界面汉化文本";
            this.btnStageClearLocalization.UseVisualStyleBackColor = false;
            this.btnStageClearLocalization.Click += new System.EventHandler(this.btnStageClearLocalization_Click);
            // 
            // btnEndingLocalization
            // 
            this.btnEndingLocalization.BackColor = System.Drawing.Color.Red;
            this.btnEndingLocalization.Location = new System.Drawing.Point(384, 150);
            this.btnEndingLocalization.Name = "btnEndingLocalization";
            this.btnEndingLocalization.Size = new System.Drawing.Size(118, 40);
            this.btnEndingLocalization.TabIndex = 17;
            this.btnEndingLocalization.Text = "通关界面汉化文本";
            this.btnEndingLocalization.UseVisualStyleBackColor = false;
            this.btnEndingLocalization.Click += new System.EventHandler(this.btnEndingLocalization_Click);
            // 
            // btnImportFontForEnding
            // 
            this.btnImportFontForEnding.Location = new System.Drawing.Point(384, 104);
            this.btnImportFontForEnding.Name = "btnImportFontForEnding";
            this.btnImportFontForEnding.Size = new System.Drawing.Size(118, 40);
            this.btnImportFontForEnding.TabIndex = 16;
            this.btnImportFontForEnding.Text = "通关界面CHR导入汉字字模";
            this.btnImportFontForEnding.UseVisualStyleBackColor = true;
            this.btnImportFontForEnding.Click += new System.EventHandler(this.btnImportFontForEnding_Click);
            // 
            // btnGameOverLocalization
            // 
            this.btnGameOverLocalization.BackColor = System.Drawing.Color.Red;
            this.btnGameOverLocalization.Location = new System.Drawing.Point(508, 58);
            this.btnGameOverLocalization.Name = "btnGameOverLocalization";
            this.btnGameOverLocalization.Size = new System.Drawing.Size(118, 40);
            this.btnGameOverLocalization.TabIndex = 19;
            this.btnGameOverLocalization.Text = "GameOver界面汉化文本";
            this.btnGameOverLocalization.UseVisualStyleBackColor = false;
            this.btnGameOverLocalization.Click += new System.EventHandler(this.btnGameOverLocalization_Click);
            // 
            // btnImportFontForGameOver
            // 
            this.btnImportFontForGameOver.Location = new System.Drawing.Point(508, 12);
            this.btnImportFontForGameOver.Name = "btnImportFontForGameOver";
            this.btnImportFontForGameOver.Size = new System.Drawing.Size(118, 40);
            this.btnImportFontForGameOver.TabIndex = 18;
            this.btnImportFontForGameOver.Text = "GameOver界面CHR导入汉字字模";
            this.btnImportFontForGameOver.UseVisualStyleBackColor = true;
            this.btnImportFontForGameOver.Click += new System.EventHandler(this.btnImportFontForGameOver_Click);
            // 
            // btnCreateNewROM
            // 
            this.btnCreateNewROM.BackColor = System.Drawing.Color.Black;
            this.btnCreateNewROM.ForeColor = System.Drawing.Color.White;
            this.btnCreateNewROM.Location = new System.Drawing.Point(623, 269);
            this.btnCreateNewROM.Name = "btnCreateNewROM";
            this.btnCreateNewROM.Size = new System.Drawing.Size(83, 80);
            this.btnCreateNewROM.TabIndex = 20;
            this.btnCreateNewROM.Text = "摧毁当前ROM并创建新ROM";
            this.btnCreateNewROM.UseVisualStyleBackColor = false;
            this.btnCreateNewROM.Click += new System.EventHandler(this.btnCreateNewROM_Click);
            // 
            // btnStaffLocalization
            // 
            this.btnStaffLocalization.Location = new System.Drawing.Point(508, 150);
            this.btnStaffLocalization.Name = "btnStaffLocalization";
            this.btnStaffLocalization.Size = new System.Drawing.Size(118, 40);
            this.btnStaffLocalization.TabIndex = 21;
            this.btnStaffLocalization.Text = "Staff界面汉化文本";
            this.btnStaffLocalization.UseVisualStyleBackColor = false;
            this.btnStaffLocalization.Click += new System.EventHandler(this.btnStaffLocalization_Click);
            // 
            // btnImportFontForStaff
            // 
            this.btnImportFontForStaff.Location = new System.Drawing.Point(508, 104);
            this.btnImportFontForStaff.Name = "btnImportFontForStaff";
            this.btnImportFontForStaff.Size = new System.Drawing.Size(118, 40);
            this.btnImportFontForStaff.TabIndex = 22;
            this.btnImportFontForStaff.Text = "Staff界面CHR导入汉字字模";
            this.btnImportFontForStaff.UseVisualStyleBackColor = true;
            this.btnImportFontForStaff.Click += new System.EventHandler(this.btnImportFontForStaff_Click);
            // 
            // btnStaffLocalization2
            // 
            this.btnStaffLocalization2.Location = new System.Drawing.Point(508, 196);
            this.btnStaffLocalization2.Name = "btnStaffLocalization2";
            this.btnStaffLocalization2.Size = new System.Drawing.Size(118, 40);
            this.btnStaffLocalization2.TabIndex = 23;
            this.btnStaffLocalization2.Text = "Staff界面汉化文本";
            this.btnStaffLocalization2.UseVisualStyleBackColor = false;
            this.btnStaffLocalization2.Click += new System.EventHandler(this.btnStaffLocalization2_Click);
            // 
            // btnTitleLocalization
            // 
            this.btnTitleLocalization.Location = new System.Drawing.Point(632, 12);
            this.btnTitleLocalization.Name = "btnTitleLocalization";
            this.btnTitleLocalization.Size = new System.Drawing.Size(118, 40);
            this.btnTitleLocalization.TabIndex = 24;
            this.btnTitleLocalization.Text = "汉化标题界面";
            this.btnTitleLocalization.UseVisualStyleBackColor = true;
            this.btnTitleLocalization.Click += new System.EventHandler(this.btnTitleLocalization_Click);
            // 
            // formMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 361);
            this.Controls.Add(this.btnTitleLocalization);
            this.Controls.Add(this.btnStaffLocalization2);
            this.Controls.Add(this.btnImportFontForStaff);
            this.Controls.Add(this.btnStaffLocalization);
            this.Controls.Add(this.btnCreateNewROM);
            this.Controls.Add(this.btnGameOverLocalization);
            this.Controls.Add(this.btnImportFontForGameOver);
            this.Controls.Add(this.btnEndingLocalization);
            this.Controls.Add(this.btnImportFontForEnding);
            this.Controls.Add(this.btnStageClearLocalization);
            this.Controls.Add(this.btnImportFontForStageClear);
            this.Controls.Add(this.btnInGameHUDLocalization);
            this.Controls.Add(this.btnIntroLocalization);
            this.Controls.Add(this.btnImportFontForIntro);
            this.Controls.Add(this.btnRebuildIntroNametable);
            this.Controls.Add(this.btnSwitchCHRForIntro);
            this.Controls.Add(this.btnPatchJSRDrawVRAM);
            this.Controls.Add(this.btnSwitchCHR);
            this.Controls.Add(this.btnImportCHR);
            this.Controls.Add(this.btnPatchWriteE000);
            this.Controls.Add(this.btnHijackInterrupt);
            this.Controls.Add(this.btnLoadInclude);
            this.Controls.Add(this.btnCopyBankTo6000);
            this.Controls.Add(this.btnInit);
            this.Controls.Add(this.btnCHRRAM2CHRROM);
            this.Name = "formMain";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCHRRAM2CHRROM;
        private System.Windows.Forms.Button btnInit;
        private System.Windows.Forms.Button btnCopyBankTo6000;
        private System.Windows.Forms.Button btnLoadInclude;
        private System.Windows.Forms.Button btnHijackInterrupt;
        private System.Windows.Forms.Button btnPatchWriteE000;
        private System.Windows.Forms.Button btnImportCHR;
        private System.Windows.Forms.Button btnSwitchCHR;
        private System.Windows.Forms.Button btnPatchJSRDrawVRAM;
        private System.Windows.Forms.Button btnSwitchCHRForIntro;
        private System.Windows.Forms.Button btnRebuildIntroNametable;
        private System.Windows.Forms.Button btnImportFontForIntro;
        private System.Windows.Forms.Button btnIntroLocalization;
        private System.Windows.Forms.Button btnInGameHUDLocalization;
        private System.Windows.Forms.Button btnImportFontForStageClear;
        private System.Windows.Forms.Button btnStageClearLocalization;
        private System.Windows.Forms.Button btnEndingLocalization;
        private System.Windows.Forms.Button btnImportFontForEnding;
        private System.Windows.Forms.Button btnGameOverLocalization;
        private System.Windows.Forms.Button btnImportFontForGameOver;
        private System.Windows.Forms.Button btnCreateNewROM;
        private System.Windows.Forms.Button btnStaffLocalization;
        private System.Windows.Forms.Button btnImportFontForStaff;
        private System.Windows.Forms.Button btnStaffLocalization2;
        private System.Windows.Forms.Button btnTitleLocalization;
    }
}

