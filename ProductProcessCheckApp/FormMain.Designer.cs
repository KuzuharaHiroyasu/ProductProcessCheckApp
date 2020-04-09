namespace ProductProcessCheckApp
{
    partial class FormMain
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel12 = new System.Windows.Forms.Panel();
            this.lblCurrentDate = new System.Windows.Forms.Label();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblAddress = new System.Windows.Forms.Label();
            this.lblModelName = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnEEPROM = new System.Windows.Forms.Button();
            this.btnFinish = new System.Windows.Forms.Button();
            this.btnAcceleSensor = new System.Windows.Forms.Button();
            this.btnMike = new System.Windows.Forms.Button();
            this.btnWearSensor = new System.Windows.Forms.Button();
            this.btnVibration = new System.Windows.Forms.Button();
            this.btnLED = new System.Windows.Forms.Button();
            this.btnBattery = new System.Windows.Forms.Button();
            this.btnInspectTitle = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.lblRate = new System.Windows.Forms.Label();
            this.lblNumNG = new System.Windows.Forms.Label();
            this.lblNumGood = new System.Windows.Forms.Label();
            this.lblNumTotal = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblTitleRate = new System.Windows.Forms.Label();
            this.lblTitleNG = new System.Windows.Forms.Label();
            this.lblTitleGood = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.panel8 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.panel9 = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.panel10 = new System.Windows.Forms.Panel();
            this.btnNG = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.panel11 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel12.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel10.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.panel12);
            this.panel1.Controls.Add(this.btnDisconnect);
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Location = new System.Drawing.Point(10, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1067, 43);
            this.panel1.TabIndex = 0;
            // 
            // panel12
            // 
            this.panel12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel12.Controls.Add(this.lblCurrentDate);
            this.panel12.Location = new System.Drawing.Point(839, 4);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(225, 36);
            this.panel12.TabIndex = 2;
            // 
            // lblCurrentDate
            // 
            this.lblCurrentDate.AutoSize = true;
            this.lblCurrentDate.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblCurrentDate.Location = new System.Drawing.Point(12, 8);
            this.lblCurrentDate.Name = "lblCurrentDate";
            this.lblCurrentDate.Size = new System.Drawing.Size(159, 19);
            this.lblCurrentDate.TabIndex = 0;
            this.lblCurrentDate.Text = "--/--/---- --:--";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.BackColor = System.Drawing.Color.MediumTurquoise;
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Font = new System.Drawing.Font("MS UI Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnDisconnect.ForeColor = System.Drawing.Color.White;
            this.btnDisconnect.Location = new System.Drawing.Point(715, 3);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(89, 37);
            this.btnDisconnect.TabIndex = 1;
            this.btnDisconnect.Text = "切断";
            this.btnDisconnect.UseVisualStyleBackColor = false;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.51936F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.48064F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 286F));
            this.tableLayoutPanel1.Controls.Add(this.lblVersion, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblAddress, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblModelName, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(699, 36);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lblVersion
            // 
            this.lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblVersion.Location = new System.Drawing.Point(203, 8);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(204, 19);
            this.lblVersion.TabIndex = 2;
            this.lblVersion.Text = "Ver：----------";
            // 
            // lblAddress
            // 
            this.lblAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAddress.AutoSize = true;
            this.lblAddress.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblAddress.Location = new System.Drawing.Point(414, 8);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(281, 19);
            this.lblAddress.TabIndex = 1;
            this.lblAddress.Text = "BDアドレス[-:-:-:-:-:-]";
            // 
            // lblModelName
            // 
            this.lblModelName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblModelName.AutoSize = true;
            this.lblModelName.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblModelName.Location = new System.Drawing.Point(4, 8);
            this.lblModelName.Name = "lblModelName";
            this.lblModelName.Size = new System.Drawing.Size(192, 19);
            this.lblModelName.TabIndex = 0;
            this.lblModelName.Text = "機種名：----------";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnEEPROM);
            this.panel2.Controls.Add(this.btnFinish);
            this.panel2.Controls.Add(this.btnAcceleSensor);
            this.panel2.Controls.Add(this.btnMike);
            this.panel2.Controls.Add(this.btnWearSensor);
            this.panel2.Controls.Add(this.btnVibration);
            this.panel2.Controls.Add(this.btnLED);
            this.panel2.Controls.Add(this.btnBattery);
            this.panel2.Controls.Add(this.btnInspectTitle);
            this.panel2.Location = new System.Drawing.Point(12, 62);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(220, 563);
            this.panel2.TabIndex = 1;
            // 
            // btnEEPROM
            // 
            this.btnEEPROM.BackColor = System.Drawing.Color.White;
            this.btnEEPROM.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnEEPROM.Location = new System.Drawing.Point(19, 436);
            this.btnEEPROM.Name = "btnEEPROM";
            this.btnEEPROM.Size = new System.Drawing.Size(188, 42);
            this.btnEEPROM.TabIndex = 8;
            this.btnEEPROM.Text = "  ７：EEPROM";
            this.btnEEPROM.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEEPROM.UseVisualStyleBackColor = false;
            // 
            // btnFinish
            // 
            this.btnFinish.BackColor = System.Drawing.Color.White;
            this.btnFinish.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnFinish.Location = new System.Drawing.Point(19, 503);
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Size = new System.Drawing.Size(188, 42);
            this.btnFinish.TabIndex = 7;
            this.btnFinish.Text = "完了";
            this.btnFinish.UseVisualStyleBackColor = false;
            // 
            // btnAcceleSensor
            // 
            this.btnAcceleSensor.BackColor = System.Drawing.Color.White;
            this.btnAcceleSensor.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnAcceleSensor.Location = new System.Drawing.Point(19, 315);
            this.btnAcceleSensor.Name = "btnAcceleSensor";
            this.btnAcceleSensor.Size = new System.Drawing.Size(188, 42);
            this.btnAcceleSensor.TabIndex = 6;
            this.btnAcceleSensor.Text = "  ５：加速度センサー";
            this.btnAcceleSensor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAcceleSensor.UseVisualStyleBackColor = false;
            // 
            // btnMike
            // 
            this.btnMike.BackColor = System.Drawing.Color.White;
            this.btnMike.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnMike.Location = new System.Drawing.Point(19, 254);
            this.btnMike.Name = "btnMike";
            this.btnMike.Size = new System.Drawing.Size(188, 42);
            this.btnMike.TabIndex = 5;
            this.btnMike.Text = "  ４：マイク";
            this.btnMike.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMike.UseVisualStyleBackColor = false;
            // 
            // btnWearSensor
            // 
            this.btnWearSensor.BackColor = System.Drawing.Color.White;
            this.btnWearSensor.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnWearSensor.Location = new System.Drawing.Point(19, 380);
            this.btnWearSensor.Name = "btnWearSensor";
            this.btnWearSensor.Size = new System.Drawing.Size(188, 42);
            this.btnWearSensor.TabIndex = 4;
            this.btnWearSensor.Text = "  ６：装着センサー";
            this.btnWearSensor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnWearSensor.UseVisualStyleBackColor = false;
            // 
            // btnVibration
            // 
            this.btnVibration.BackColor = System.Drawing.Color.White;
            this.btnVibration.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnVibration.Location = new System.Drawing.Point(19, 195);
            this.btnVibration.Name = "btnVibration";
            this.btnVibration.Size = new System.Drawing.Size(188, 42);
            this.btnVibration.TabIndex = 3;
            this.btnVibration.Text = "  ３：バイブレーション";
            this.btnVibration.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVibration.UseVisualStyleBackColor = false;
            // 
            // btnLED
            // 
            this.btnLED.BackColor = System.Drawing.Color.White;
            this.btnLED.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnLED.Location = new System.Drawing.Point(19, 134);
            this.btnLED.Name = "btnLED";
            this.btnLED.Size = new System.Drawing.Size(188, 42);
            this.btnLED.TabIndex = 2;
            this.btnLED.Text = "  ２：ＬＥＤ";
            this.btnLED.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLED.UseVisualStyleBackColor = false;
            // 
            // btnBattery
            // 
            this.btnBattery.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnBattery.BackColor = System.Drawing.Color.White;
            this.btnBattery.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnBattery.Location = new System.Drawing.Point(18, 73);
            this.btnBattery.Name = "btnBattery";
            this.btnBattery.Size = new System.Drawing.Size(188, 42);
            this.btnBattery.TabIndex = 1;
            this.btnBattery.Text = "  １：充電";
            this.btnBattery.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBattery.UseVisualStyleBackColor = false;
            // 
            // btnInspectTitle
            // 
            this.btnInspectTitle.BackColor = System.Drawing.Color.White;
            this.btnInspectTitle.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnInspectTitle.Location = new System.Drawing.Point(19, 14);
            this.btnInspectTitle.Name = "btnInspectTitle";
            this.btnInspectTitle.Size = new System.Drawing.Size(188, 42);
            this.btnInspectTitle.TabIndex = 0;
            this.btnInspectTitle.Text = "検査項目";
            this.btnInspectTitle.UseVisualStyleBackColor = false;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.tableLayoutPanel3);
            this.panel3.Controls.Add(this.tableLayoutPanel2);
            this.panel3.Location = new System.Drawing.Point(252, 62);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(451, 160);
            this.panel3.TabIndex = 2;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel3.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 104F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 93F));
            this.tableLayoutPanel3.Controls.Add(this.lblRate, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.lblNumNG, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.lblNumGood, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.lblNumTotal, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(20, 95);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(417, 51);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // lblRate
            // 
            this.lblRate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblRate.AutoSize = true;
            this.lblRate.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblRate.Location = new System.Drawing.Point(347, 15);
            this.lblRate.Name = "lblRate";
            this.lblRate.Size = new System.Drawing.Size(43, 20);
            this.lblRate.TabIndex = 3;
            this.lblRate.Text = "0.0%";
            this.lblRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNumNG
            // 
            this.lblNumNG.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblNumNG.AutoSize = true;
            this.lblNumNG.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblNumNG.Location = new System.Drawing.Point(259, 15);
            this.lblNumNG.Name = "lblNumNG";
            this.lblNumNG.Size = new System.Drawing.Size(19, 20);
            this.lblNumNG.TabIndex = 2;
            this.lblNumNG.Text = "0";
            this.lblNumNG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNumGood
            // 
            this.lblNumGood.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblNumGood.AutoSize = true;
            this.lblNumGood.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblNumGood.Location = new System.Drawing.Point(153, 15);
            this.lblNumGood.Name = "lblNumGood";
            this.lblNumGood.Size = new System.Drawing.Size(19, 20);
            this.lblNumGood.TabIndex = 1;
            this.lblNumGood.Text = "0";
            this.lblNumGood.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNumTotal
            // 
            this.lblNumTotal.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblNumTotal.AutoSize = true;
            this.lblNumTotal.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblNumTotal.Location = new System.Drawing.Point(45, 15);
            this.lblNumTotal.Name = "lblNumTotal";
            this.lblNumTotal.Size = new System.Drawing.Size(19, 20);
            this.lblNumTotal.TabIndex = 0;
            this.lblNumTotal.Text = "0";
            this.lblNumTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 104F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 93F));
            this.tableLayoutPanel2.Controls.Add(this.lblTitleRate, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblTitleNG, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblTitleGood, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(20, 14);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(417, 54);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // lblTitleRate
            // 
            this.lblTitleRate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblTitleRate.AutoSize = true;
            this.lblTitleRate.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTitleRate.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblTitleRate.Location = new System.Drawing.Point(334, 17);
            this.lblTitleRate.Name = "lblTitleRate";
            this.lblTitleRate.Size = new System.Drawing.Size(69, 20);
            this.lblTitleRate.TabIndex = 3;
            this.lblTitleRate.Text = "不良率";
            this.lblTitleRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitleNG
            // 
            this.lblTitleNG.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblTitleNG.AutoSize = true;
            this.lblTitleNG.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTitleNG.ForeColor = System.Drawing.Color.DeepPink;
            this.lblTitleNG.Location = new System.Drawing.Point(251, 17);
            this.lblTitleNG.Name = "lblTitleNG";
            this.lblTitleNG.Size = new System.Drawing.Size(36, 20);
            this.lblTitleNG.TabIndex = 2;
            this.lblTitleNG.Text = "NG";
            this.lblTitleNG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitleGood
            // 
            this.lblTitleGood.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblTitleGood.AutoSize = true;
            this.lblTitleGood.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTitleGood.ForeColor = System.Drawing.Color.MediumSeaGreen;
            this.lblTitleGood.Location = new System.Drawing.Point(130, 17);
            this.lblTitleGood.Name = "lblTitleGood";
            this.lblTitleGood.Size = new System.Drawing.Size(64, 20);
            this.lblTitleGood.TabIndex = 1;
            this.lblTitleGood.Text = "GOOD";
            this.lblTitleGood.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(30, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "合計";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.White;
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.label5);
            this.panel6.Location = new System.Drawing.Point(252, 242);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(451, 117);
            this.panel6.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label5.Location = new System.Drawing.Point(125, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(184, 24);
            this.label5.TabIndex = 0;
            this.label5.Text = "マイク(Chart Area)";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.White;
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel7.Controls.Add(this.label6);
            this.panel7.Location = new System.Drawing.Point(252, 378);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(451, 129);
            this.panel7.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label6.Location = new System.Drawing.Point(87, 51);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(282, 24);
            this.label6.TabIndex = 1;
            this.label6.Text = "加速度センサー(Chart Area)";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.White;
            this.panel8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel8.Controls.Add(this.label7);
            this.panel8.Location = new System.Drawing.Point(252, 528);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(451, 126);
            this.panel8.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.White;
            this.label7.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label7.Location = new System.Drawing.Point(97, 51);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(258, 24);
            this.label7.TabIndex = 1;
            this.label7.Text = "装着センサー(Chart Area)";
            // 
            // panel9
            // 
            this.panel9.BackColor = System.Drawing.Color.White;
            this.panel9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel9.Controls.Add(this.lblStatus);
            this.panel9.Location = new System.Drawing.Point(728, 62);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(349, 116);
            this.panel9.TabIndex = 6;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(13, 14);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(37, 15);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "状態";
            // 
            // panel10
            // 
            this.panel10.BackColor = System.Drawing.Color.White;
            this.panel10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel10.Controls.Add(this.btnNG);
            this.panel10.Controls.Add(this.btnConnect);
            this.panel10.Location = new System.Drawing.Point(728, 197);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(349, 80);
            this.panel10.TabIndex = 7;
            // 
            // btnNG
            // 
            this.btnNG.BackColor = System.Drawing.Color.Goldenrod;
            this.btnNG.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnNG.ForeColor = System.Drawing.Color.White;
            this.btnNG.Location = new System.Drawing.Point(184, 12);
            this.btnNG.Name = "btnNG";
            this.btnNG.Size = new System.Drawing.Size(127, 57);
            this.btnNG.TabIndex = 1;
            this.btnNG.Text = "NG(手動)";
            this.btnNG.UseVisualStyleBackColor = false;
            this.btnNG.Click += new System.EventHandler(this.btnNG_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnConnect.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnConnect.ForeColor = System.Drawing.Color.White;
            this.btnConnect.Location = new System.Drawing.Point(37, 12);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(127, 57);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "接続";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // panel11
            // 
            this.panel11.BackColor = System.Drawing.Color.White;
            this.panel11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel11.Location = new System.Drawing.Point(728, 300);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(349, 354);
            this.panel11.TabIndex = 8;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1106, 675);
            this.Controls.Add(this.panel11);
            this.Controls.Add(this.panel10);
            this.Controls.Add(this.panel9);
            this.Controls.Add(this.panel8);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormMain";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel12.ResumeLayout(false);
            this.panel12.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            this.panel10.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnAcceleSensor;
        private System.Windows.Forms.Button btnMike;
        private System.Windows.Forms.Button btnWearSensor;
        private System.Windows.Forms.Button btnVibration;
        private System.Windows.Forms.Button btnLED;
        private System.Windows.Forms.Button btnBattery;
        private System.Windows.Forms.Button btnInspectTitle;
        private System.Windows.Forms.Button btnEEPROM;
        private System.Windows.Forms.Button btnFinish;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.Label lblModelName;
        private System.Windows.Forms.Label lblCurrentDate;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnNG;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label lblRate;
        private System.Windows.Forms.Label lblNumNG;
        private System.Windows.Forms.Label lblNumGood;
        private System.Windows.Forms.Label lblNumTotal;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lblTitleRate;
        private System.Windows.Forms.Label lblTitleNG;
        private System.Windows.Forms.Label lblTitleGood;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblStatus;
    }
}

