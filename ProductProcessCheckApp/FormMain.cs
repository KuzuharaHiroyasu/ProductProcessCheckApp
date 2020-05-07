using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Diagnostics;
using Windows.Devices.Enumeration;
using Windows.UI.Core;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Security.Cryptography;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace ProductProcessCheckApp
{
    public partial class FormMain : Form
    {
        public BluetoothLEAdvertisementWatcher watcher;
        public DeviceWatcher deviceWatcher;
        public GattDeviceService gattService;
        public GattCharacteristic readCharacteristic, writeCharacteristic;
        public BluetoothLEDevice bleDevice;

        //public List<GattCharacteristic> characteristicList = new List<GattCharacteristic>();

        public Dictionary<string, BluetoothLEDevice> deviceList = new Dictionary<string, BluetoothLEDevice>();

        private Boolean startedFlag = false;
        private Timer connectTimer = new Timer();
        private DeviceStatus deviceStatus = DeviceStatus.NOT_CONNECT;

        private IniFile ini;
        private LogWriter log = new LogWriter();
        private string g_address;

        private int numGood = 0;
        private int numNotGood = 0;
        private bool isNGClick = false;
        
        private bool isDetectStartMike = false;
        private bool isDetectStartAccele = false;
        private bool isDetectStartWear = false;
        private int MAX_NUM_RECEIVED = 200;
        private int numReceivedBreath = 0;
        private int numReceivedAccele = 0;
        private int numReceivedWear = 0;
        List<byte[]> receivedBreathData  = new List<byte[]>();
        List<byte[]> receivedAcceleData = new List<byte[]>();
        List<byte[]> receivedWearData = new List<byte[]>();

        private int GraphDataNum = 200 + 1;
        Queue<double> MicDataRespQueue = new Queue<double>();
        Queue<double> AcceXDataRespQueue = new Queue<double>();
        Queue<double> AcceYDataRespQueue = new Queue<double>();
        Queue<double> AcceZDataRespQueue = new Queue<double>();
        Queue<double> PhotoDataRespQueue = new Queue<double>();
        object lockData = new object();

        bool micScanResult;
        bool acceScanResult;
        bool photoScanResult;

        private int breath_max = 0;
        private int breath_min = 0;
        private int acce_x_max = 0;
        private int acce_x_min = 0;
        private int acce_y_max = 0;
        private int acce_y_min = 0;
        private int acce_z_max = 0;
        private int acce_z_min = 0;
        private int photo_max = 0;
        private int photo_min = 0;

        public FormMain()
        {
            InitializeComponent();

            CustomGUI();

            LoadIniFile();

            initGraphShow();
        }

        private void initGraphShow()
        {
            MicDataRespQueue.Clear();
            AcceXDataRespQueue.Clear();
            AcceYDataRespQueue.Clear();
            AcceZDataRespQueue.Clear();
            PhotoDataRespQueue.Clear();

            for (int i = 0; i < GraphDataNum; i++)
            {
                MicDataRespQueue.Enqueue(0);
                AcceXDataRespQueue.Enqueue(0);
                AcceYDataRespQueue.Enqueue(0);
                AcceZDataRespQueue.Enqueue(0);
                PhotoDataRespQueue.Enqueue(0);
            }

            GraphUpdate_Mic();
            GraphUpdate_Acce();
            GraphUpdate_Photo();
        }

        private void GraphUpdate_Mic()
        {
            int cnt = 0;

            lock (lockData)
            {
                Series srs_micresp = chart_mic.Series["マイク"];
                srs_micresp.Points.Clear();
                cnt = 0;
                foreach (double data in MicDataRespQueue)
                {
                    srs_micresp.Points.AddXY(cnt, data);
                    cnt++;
                }
            }
            chart_mic.Invalidate();
        }

        private void GraphUpdate_Acce()
        {
            int cnt = 0;

            lock (lockData)
            {
                Series srs_acceXresp = chart_acce.Series["X軸"];
                Series srs_acceYresp = chart_acce.Series["Y軸"];
                Series srs_acceZresp = chart_acce.Series["Z軸"];
                srs_acceXresp.Points.Clear();
                srs_acceYresp.Points.Clear();
                srs_acceZresp.Points.Clear();
                cnt = 0;
                foreach (double data in AcceXDataRespQueue)
                {
                    srs_acceXresp.Points.AddXY(cnt, data);
                    cnt++;
                }
                cnt = 0;
                foreach (double data in AcceYDataRespQueue)
                {
                    srs_acceYresp.Points.AddXY(cnt, data);
                    cnt++;
                }
                cnt = 0;
                foreach (double data in AcceZDataRespQueue)
                {
                    srs_acceZresp.Points.AddXY(cnt, data);
                    cnt++;
                }
            }
            chart_acce.Invalidate();
        }

        private void GraphUpdate_Photo()
        {
            int cnt = 0;

            lock (lockData)
            {
                Series srs_photoresp = chart_photo.Series["装着センサー"];
                srs_photoresp.Points.Clear();
                cnt = 0;
                foreach (double data in PhotoDataRespQueue)
                {
                    srs_photoresp.Points.AddXY(cnt, data);
                    cnt++;
                }
            }
            chart_photo.Invalidate();
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            isNGClick = false;
            if (deviceStatus == DeviceStatus.NOT_CONNECT || deviceStatus == DeviceStatus.CONNECT_FAILED) //接続処理
            {
                if (textBoxSerialStart.Text != "" || textBoxSerialEnd.Text != "")
                {
                    if (textBoxSerialStart.Enabled == true && textBoxSerialEnd.Enabled == true)
                    {
                        uint planNum = Convert.ToUInt32(textBoxSerialEnd.Text) - Convert.ToUInt32(textBoxSerialStart.Text) + 1;

                        lblCheckPlanNum.Text = planNum.ToString();
                        log.serialLogWrite(textBoxSerialStart.Text, textBoxSerialEnd.Text);

                        textBoxSerialStart.Enabled = false;
                        textBoxSerialEnd.Enabled = false;
                    }

                    DisconnectDevice(false);

                    SetupSearchTimeOut();
                    SetupBluetooth();

                    lblStatus.Text = Constant.MSG_SLEEIM_IS_SEARCHING;
                }
            } else if (deviceStatus == DeviceStatus.CONNECT_SUCCESS) 
            {
                //[START]ボタンクリック
                sendCommandStatusChange();　//状態変更コマンド(0xB0)送信
            } else if (deviceStatus == DeviceStatus.DETECT_BATTERY_OK) 
            {
                //[OK(手動)]ボタンクリック
                sendCommandDetectBatteryFinish();
            } else if (deviceStatus == DeviceStatus.DETECT_LED_OK)
            {
                //[OK(手動)]ボタンクリック
                sendCommandDetectLEDFinish();
            } else if (deviceStatus == DeviceStatus.DETECT_VIBRATION_OK)
            {
                //[OK(手動)]ボタンクリック
                sendCommandDetectVibrationFinish();
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (watcher != null && deviceWatcher != null)
            {
                StopScanning();
            }

            var message = gattService == null ? Constant.MSG_SLEEIM_IS_NOT_CONNECTING : Constant.MSG_SLEEIM_DISCONNECTED;
            lblStatus.Text = message;

            DisconnectDevice(false);
            
            MessageBox.Show(message, Constant.APP_NAME);
        }

        private async void btnNG_Click(object sender, EventArgs e)
        {
            isNGClick = true;

            //[NG(手動)]ボタンクリック
            if (deviceStatus == DeviceStatus.STATUS_CHANGE_OK ||
                deviceStatus == DeviceStatus.DETECT_BATTERY_OK ||
                deviceStatus == DeviceStatus.DETECT_LED_OK ||
                deviceStatus == DeviceStatus.DETECT_VIBRATION_OK)
            {
                sendCommandSendPowerSWOff();
            }
        }

        private void UpdateDeviceStatus(DeviceStatus status, String statusMessage = "", bool showDialog = true)
        {
            deviceStatus = status;
            btnConnect.Enabled = true;
            if (status == DeviceStatus.CONNECT_SUCCESS)
            {
                btnConnect.Text = "START";
                btnDisconnect.Enabled = true;

                //Reset
                isDetectStartMike = false;
                isDetectStartAccele = false;
                isDetectStartWear = false;
                numReceivedBreath = 0;
                numReceivedAccele = 0;
                numReceivedWear = 0;
                receivedBreathData = new List<byte[]>();
                receivedAcceleData = new List<byte[]>();
                receivedWearData = new List<byte[]>();
            } else if (status == DeviceStatus.STATUS_CHANGE_OK)
            {
                btnConnect.Text = "OK(手動)";
            } else if (status == DeviceStatus.DETECT_VIBRATION_FINISH_OK)
            {
                btnConnect.Text = "自動検査";
                btnConnect.Enabled = false;
            } else if (status == DeviceStatus.NOT_CONNECT || status == DeviceStatus.CONNECT_FAILED)
            {
                deviceStatus = DeviceStatus.NOT_CONNECT;
                btnConnect.Text = "接続";
            }

            if (statusMessage != "")
            {
                lblStatus.Text = statusMessage;
                if(showDialog)
                {
                    MessageBox.Show(statusMessage, Constant.APP_NAME);
                }
            }
        }

        private void SetupBluetooth()
        {
            watcher = new BluetoothLEAdvertisementWatcher
            {
                //ScanningMode = BluetoothLEScanningMode.Active 
            };
            watcher.Received += DeviceFound;

            deviceWatcher = DeviceInformation.CreateWatcher();
            deviceWatcher.Added += DeviceAdded;
            deviceWatcher.Updated += DeviceUpdated;

            StartScanning();
        }

        private async void DisconnectDevice(bool isFinish)
        {
            if (gattService != null)
            {
                gattService.Dispose();
            }
            if (bleDevice != null)
            {
                await bleDevice.DeviceInformation.Pairing.UnpairAsync();
                bleDevice.Dispose();
            }

            gattService = null;
            bleDevice = null;
            readCharacteristic = null;
            writeCharacteristic = null;
            deviceList = new Dictionary<string, BluetoothLEDevice>();

            lblAddress.Text = "BDアドレス[-:-:-:-:-:-]";
            btnDisconnect.Enabled = false;
            btnNG.Enabled = false;
            btnNG.Text = "NG(手動)";
            UpdateDeviceStatus(DeviceStatus.NOT_CONNECT);

            //Reset all button
            btnBattery.BackColor      = Color.White;
            btnLED.BackColor          = Color.White;
            btnVibration.BackColor    = Color.White;
            btnMike.BackColor         = Color.White;
            btnAcceleSensor.BackColor = Color.White;
            btnWearSensor.BackColor   = Color.White;
            btnEEPROM.BackColor       = Color.White;
            if(!isFinish)
            {
                btnFinish.BackColor = Color.White;
                lblCheckResult.Text = "";
            }
        }

        private void LoadIniFile()
        {
            ini = new IniFile("Setting.ini");

            var modelName = ini.Read("MODEL", "MODEL_NAME");
            var version = ini.Read("BUILD_VER", "VERSION");
            var path = ini.Read("FILE_PATH", "PATH");

            lblModelName.Text = "機種名：" + (modelName != "" ? modelName : "-");
            lblVersion.Text = "Ver：" + (version != "" ? version : "-");

            LoadIniFileMicRange();

            LoadIniFileAcceRange();

            LoadIniFilePhotoRange();

            log.logFileCreate(path, modelName);

            log.verLogWrite(version);
        }

        private void LoadIniFileMicRange()
        {
            string sens_judge_data;

            //呼吸音範囲設定
            sens_judge_data = ini.Read("BREATH_MAX", "RANGE");
            if (sens_judge_data != "")
            {
                breath_max = int.Parse(sens_judge_data);
            }
            sens_judge_data = ini.Read("BREATH_MIN", "RANGE");
            if (sens_judge_data != "")
            {
                breath_min = int.Parse(sens_judge_data);
            }
        }

        private void LoadIniFileAcceRange()
        {
            string sens_judge_data;

            //加速度センサー範囲設定
            sens_judge_data = ini.Read("ACL_SEN_X_MAX", "RANGE");
            if (sens_judge_data != "")
            {
                acce_x_max = int.Parse(sens_judge_data);
            }
            sens_judge_data = ini.Read("ACL_SEN_X_MIN", "RANGE");
            if (sens_judge_data != "")
            {
                acce_x_min = int.Parse(sens_judge_data);
            }
            sens_judge_data = ini.Read("ACL_SEN_Y_MAX", "RANGE");
            if (sens_judge_data != "")
            {
                acce_y_max = int.Parse(sens_judge_data);
            }
            sens_judge_data = ini.Read("ACL_SEN_Y_MIN", "RANGE");
            if (sens_judge_data != "")
            {
                acce_y_min = int.Parse(sens_judge_data);
            }
            sens_judge_data = ini.Read("ACL_SEN_Z_MAX", "RANGE");
            if (sens_judge_data != "")
            {
                acce_z_max = int.Parse(sens_judge_data);
            }
            sens_judge_data = ini.Read("ACL_SEN_Z_MIN", "RANGE");
            if (sens_judge_data != "")
            {
                acce_z_min = int.Parse(sens_judge_data);
            }
        }

        private void LoadIniFilePhotoRange()
        {
            string sens_judge_data;

            //装着センサー範囲設定
            sens_judge_data = ini.Read("PHOTO_SEN_MAX", "RANGE");
            if (sens_judge_data != "")
            {
                photo_max = int.Parse(sens_judge_data);
            }
            sens_judge_data = ini.Read("PHOTO_SEN_MIN", "RANGE");
            if (sens_judge_data != "")
            {
                photo_min = int.Parse(sens_judge_data);
            }
        }

        private void CustomGUI()
        {
            var blueColor = ColorTranslator.FromHtml("#65a7e3");
            btnDisconnect.BackColor = blueColor;
            btnConnect.BackColor    = blueColor;
            btnNG.BackColor         = ColorTranslator.FromHtml("#ff842e");
            btnNG.Enabled           = false;

            lblTitleGood.ForeColor = ColorTranslator.FromHtml("#ff842e");
            lblTitleNG.ForeColor   = ColorTranslator.FromHtml("#fe0000");
            lblTitleRate.ForeColor = ColorTranslator.FromHtml("#246794");

            lblStatus.Text = "";
            this.Text = Constant.APP_NAME;
            this.lblCurrentDate.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm");

            Timer dateTimer = new Timer();
            dateTimer.Interval = 10 * 1000; //秒で計算
            dateTimer.Tick += new EventHandler(DateTimer_Tick);
            dateTimer.Start();
        }

        private async void DeviceFound(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            //Step1: SearchDevice
            string address = args.BluetoothAddress.ToString("x");
            address = Utility.getFormatDeviceAddress(address);

            try
            {
                var device = await SearchDevice(args);
                if (device != null)
                {
                    TextBox.CheckForIllegalCrossThreadCalls = false; //Avoid error when call from other thread 

                    //Step2: PairDevice
                    var message = "Sleeim[" + address + "]をペアリングしています";
                    lblStatus.Text = message;
                    await PairDevice(device);

                    //Step3: ConnectDevice
                    message = "Sleeim[" + address + "]を接続しています";
                    lblStatus.Text = message;

                    try
                    {
                        var isConnected = await ConnectDevice(device);
                        if (isConnected)
                        {
                            StopScanning();

                            UpdateDeviceStatus(DeviceStatus.CONNECT_SUCCESS, "Sleeim[" + address + "]を成功に接続しました", false);
                            lblAddress.Text = "BDアドレス[" + address + "]";

                            bleDevice = device;
                            startedFlag = false;

                            var isEnabled = await EnableNotification();
                            Debug.WriteLine("デバイスからPCに通知を" + (isEnabled ? "成功" : "失敗") + "に有効化しました");

                            await RegisterNotificationWhenValueChanged();
                        }
                        else
                        {
                            UpdateDeviceStatus(DeviceStatus.NOT_CONNECT, "Sleeim[" + address + "]を失敗に接続しました", false);
                        }
                    }
                    catch (Exception e)
                    {
                        UpdateDeviceStatus(DeviceStatus.NOT_CONNECT, "Sleeim[" + address + "]を失敗に接続しました", false);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        private void StartScanning()
        {
            watcher.Start();
            deviceWatcher.Start();
        }

        private void StopScanning()
        {
            try
            {
                if (watcher.Status == BluetoothLEAdvertisementWatcherStatus.Started) {
                    watcher.Stop();
                }
                
                if(deviceWatcher.Status == DeviceWatcherStatus.Started)
                {
                    deviceWatcher.Stop();
                }
            }
            catch (Exception e)
            {

            }
        }

        private async Task<BluetoothLEDevice> SearchDevice(BluetoothLEAdvertisementReceivedEventArgs args)
        {
            BluetoothLEDevice device = null;

            string address = args.BluetoothAddress.ToString("x");
            string localName = args.Advertisement.LocalName;
            if (localName == "Sleeim" && address != "" && !deviceList.ContainsKey(address))
            {
                deviceList.Add(address, device);
                Debug.WriteLine($"--- LocalName: {localName} ({address}), Advertisement Data: {args.Advertisement.ServiceUuids.Count} ---");

                device = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);
                if (device == null)
                {
                    Debug.WriteLine("Failed to find device for address " + address);
                }
                else
                {
                    Debug.WriteLine("Success: Found device " + (device.Name ?? "unnamed") + " for address " + address);
                    g_address = address;
                }
            }

            return device;
        }

        private async Task<Boolean> PairDevice(BluetoothLEDevice device)
        {
            if (!device.DeviceInformation.Pairing.CanPair)
            {
                Debug.WriteLine("Can Not Pair", Constant.APP_NAME);
            }
            else if (device.DeviceInformation.Pairing.IsPaired)
            {
                Debug.WriteLine("Is Paired");
                return true;
            } else {
                device.DeviceInformation.Pairing.Custom.PairingRequested += CustomOnPairingRequested;
                var result = await device.DeviceInformation.Pairing.Custom.PairAsync(DevicePairingKinds.ConfirmOnly, DevicePairingProtectionLevel.None);
                device.DeviceInformation.Pairing.Custom.PairingRequested -= CustomOnPairingRequested;

                //DevicePairingResult result = await device.DeviceInformation.Pairing.PairAsync(DevicePairingProtectionLevel.None);
                if (result != null)
                {
                    Debug.WriteLine($"Pairing Result: {result.Status}");
                    if (result.Status == DevicePairingResultStatus.Paired || result.Status == DevicePairingResultStatus.AlreadyPaired)
                    {
                        Debug.WriteLine("Paired OK");
                        return true;
                    }
                }
            }

            return false;
        }

        private async Task<bool> ConnectDevice(BluetoothLEDevice device)
        {
            var gatt = await device.GetGattServicesAsync();
            Debug.WriteLine($"{device.Name} Services: {gatt.Services.Count}, {gatt.Status}, {gatt.ProtocolError}");

            if (gatt.Status == GattCommunicationStatus.Success)
            {
                foreach (var service in gatt.Services)
                {
                    var charactersResult = await service.GetCharacteristicsAsync();
                    foreach (var chara in charactersResult.Characteristics)
                    {
                        //characteristicList.Add(chara);
                        Debug.WriteLine($"Characteristic UUID {chara.Uuid}, Permission:{chara.CharacteristicProperties}");
                    }

                    if(service.Uuid == new Guid(Constant.UUID_SERVICE))
                    {
                        this.gattService = service;

                        GattCharacteristicsResult readCharacteristicResult = await gattService.GetCharacteristicsForUuidAsync(new Guid(Constant.UUID_CHAR_READ));
                        GattCharacteristicsResult writeCharacteristicResult = await gattService.GetCharacteristicsForUuidAsync(new Guid(Constant.UUID_CHAR_WRITE));

                        readCharacteristic = readCharacteristicResult.Characteristics.FirstOrDefault();
                        writeCharacteristic = writeCharacteristicResult.Characteristics.FirstOrDefault();

                        if (readCharacteristic != null && writeCharacteristic != null)
                        {
                            Console.WriteLine($"Read Characteristic Selected: {Utility.GetUUIDString(readCharacteristic.Uuid)}");
                            Console.WriteLine($"Write Characteristic Selected: {Utility.GetUUIDString(writeCharacteristic.Uuid)}");

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void SetupSearchTimeOut()
        {
            startedFlag = true;
            connectTimer.Interval = Constant.MAX_SEARCH_TIME; //秒で計算
            connectTimer.Tick += new EventHandler(ConnectTimer_Tick);
            connectTimer.Start();
        }

        private void ConnectTimer_Tick(object sender, EventArgs e)
        {
            if (gattService != null)
            {
                connectTimer.Stop();
            }
            else if (startedFlag)
            {
                startedFlag = false;
                connectTimer.Stop();

                StopScanning();

                UpdateDeviceStatus(DeviceStatus.NOT_CONNECT, Constant.MSG_SLEEIM_NOT_FOUND, false);
            }
        }

        private void DateTimer_Tick(object sender, EventArgs e)
        {
            this.lblCurrentDate.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm");

            GraphUpdate_Mic();
            GraphUpdate_Acce();
            GraphUpdate_Photo();
        }

        public async Task<CommandResult> WriteCommandToDevice(byte[] commandData)
        {
            if (writeCharacteristic != null)
            {
                // byte to ibuffer
                DataWriter dataWriter = new DataWriter();
                dataWriter.WriteBytes(commandData);
                IBuffer writeBuffer = dataWriter.DetachBuffer();

                try
                {
                    GattCommunicationStatus result = await writeCharacteristic.WriteValueAsync(writeBuffer, GattWriteOption.WriteWithResponse);

                    Console.WriteLine($"WriteCharacterValue result:{result}");
                    if (result == GattCommunicationStatus.Unreachable)
                    {
                        Debug.WriteLine("Command Write Failed (Unreachable)");
                        return CommandResult.UNREACHABLE;
                    }
                    else if (result == GattCommunicationStatus.ProtocolError)
                    {
                        Debug.WriteLine("Command Write Failed (ProtocolError)");
                        return CommandResult.PROTOCOL_ERROR;
                    }
                    else if (result == GattCommunicationStatus.Success)
                    {
                        Debug.WriteLine("Command Write Successfully");
                        return CommandResult.SUCCESS;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Command Write Failed. Exception: " + ex.Message);
                }
            }
            else
            {
                UpdateDeviceStatus(DeviceStatus.NOT_CONNECT, Constant.MSG_SLEEIM_IS_NOT_CONNECTING);
                return CommandResult.NOT_CONNECT;
            }

            return CommandResult.UNKNOW_ERROR;
        }

        public async Task<byte[]> ReadValue(GattCharacteristic characteristic)
        {
            byte[] data = null; 

            if(characteristic != null)
            {
                GattReadResult result = await characteristic.ReadValueAsync();
                if (result.Status == GattCommunicationStatus.Success)
                {
                    CryptographicBuffer.CopyToByteArray(result.Value, out data);
                }
            }

            return data;
        }

        private void ReadCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            byte[] data = new byte[args.CharacteristicValue.Length];
            DataReader.FromBuffer(args.CharacteristicValue).ReadBytes(data);

            if (data != null && data.Length >= 1)
            {
                int commandCode = data[0];
                if (commandCode == Constant.CommandReceiveBreathVolume || 
                   commandCode == Constant.CommandReceiveAcceleSensor || 
                   commandCode == Constant.CommandReceiveWearSensor)
                {
                    executeWhenReceiveDataFromDevice(commandCode, data);
                } else if (data.Length >= 2)
                {
                    Debug.WriteLine("ReadCharacteristic_ValueChanged Command 0x" + commandCode.ToString("X") + " Start");

                    bool isSentOK = (data[1] == (int)CommandReturn.SUCCESS);

                    if (commandCode == Constant.CommandDetectMike)
                    {
                        if (isDetectStartMike) //sendCommandDetectMikeStart
                        {
                            isDetectStartMike = false;
                            log.scanLogWrite(g_address, isSentOK ? "OK" : "NG", "4");

                            //btnNGには、「マイク」検査開始コマンド受信後[自動検査]に変更する。（クリック動作無効化）
                            btnNG.Enabled = false;
                            btnNG.Text = "自動検査";

                            if (isSentOK)
                            {
                                btnMike.BackColor = Color.Yellow;
                                UpdateDeviceStatus(DeviceStatus.DETECT_MIKE_OK);
                            } else
                            {
                                isNGClick = true;
                                sendCommandSendPowerSWOff();
                            }
                        }
                        else //sendCommandDetectMikeFinish
                        {
                            UpdateCheckMikeResultOnTable(isSentOK);
                            if (isSentOK)
                            {
                                btnMike.BackColor = Color.White; //Reset
                                UpdateDeviceStatus(DeviceStatus.DETECT_MIKE_FINISH_OK);
                                sendCommandDetectAcceleSensorStart();
                            }
                        }
                    }
                    else if (commandCode == Constant.CommandDetectAcceleSensor)
                    {
                        if (isDetectStartAccele) //sendCommandDetectAcceleSensorStart
                        {
                            isDetectStartAccele = false;
                            log.scanLogWrite(g_address, isSentOK ? "OK" : "NG", "5");
                            if (isSentOK)
                            {
                                btnAcceleSensor.BackColor = Color.Yellow;
                                UpdateDeviceStatus(DeviceStatus.DETECT_ACCELE_SENSOR_OK);
                            }
                        }
                        else //sendCommandDetectAcceleSensorFinish
                        {
                            UpdateCheckAclResultOnTable(isSentOK);
                            if (isSentOK)
                            {
                                btnAcceleSensor.BackColor = Color.White; //Reset
                                UpdateDeviceStatus(DeviceStatus.DETECT_ACCELE_SENSOR_FINISH_OK);
                                sendCommandDetectWearSensorStart();
                            }
                        }
                    }
                    else if (commandCode == Constant.CommandDetectWearSensor)
                    {
                        if (isDetectStartWear) //sendCommandDetectWearSensorStart
                        {
                            isDetectStartWear = false;
                            log.scanLogWrite(g_address, isSentOK ? "OK" : "NG", "6");
                            if (isSentOK)
                            {
                                btnWearSensor.BackColor = Color.Yellow;
                                UpdateDeviceStatus(DeviceStatus.DETECT_WEAR_SENSOR_OK);
                            }
                        }
                        else //sendCommandDetectWearSensorFinish
                        {
                            UpdateCheckWearResultOnTable(isSentOK);
                            if (isSentOK)
                            {
                                btnWearSensor.BackColor = Color.White; //Reset
                                UpdateDeviceStatus(DeviceStatus.DETECT_WEAR_SENSOR_FINISH_OK);

                                System.Threading.Thread.Sleep(1500);
                                sendCommandDetectEEPROMStart();
                            }
                        }
                    }
                    else if (commandCode == Constant.CommandDetectEEPROM)
                    {
                        log.scanLogWrite(g_address, isSentOK ? "OK" : "NG", "7");
                        if (isSentOK)
                        {
                            btnEEPROM.BackColor = Color.White; //Reset
                            UpdateDeviceStatus(DeviceStatus.DETECT_EEPROM_OK);
                            sendCommandSendPowerSWOff();
                        }
                    }
                    else if (commandCode == Constant.CommandSendPowerSWOff)
                    {
                        UpdateCheckEEPROMResultOnTable(isSentOK);
                        if (isSentOK)
                        {
                            UpdateDeviceStatus(DeviceStatus.SEND_POWER_OFF_OK);
                            DisconnectDevice(true);

                            btnFinish.BackColor = Color.Yellow;

                            if (isNGClick)
                            {
                                lblCheckResult.Text = "NG";
                                lblCheckResult.ForeColor = Color.Red;

                                numNotGood++;
                                updateResultTable();

                                isNGClick = false;
                            } else
                            {
                                numGood++;
                                updateResultTable();
                                lblCheckResult.Text = "OK";
                            }
                        }
                    }

                    Debug.WriteLine("ReadCharacteristic_ValueChanged Command 0x" + commandCode.ToString("X") + " End");
                }
            }
        }

        private void executeWhenReceiveDataFromDevice(int commandCode, byte[] receivedData)
        {
            if (commandCode == Constant.CommandReceiveBreathVolume) //呼吸音送信[0xA7]の受信(デバイスから)
            {
                if (numReceivedBreath >= MAX_NUM_RECEIVED)
                {
                    //System.Threading.Thread.Sleep(500); //Wait to not receive more data, to send next command
                    return;
                }

                SetMicData(receivedData[1] << 2);
                numReceivedBreath++;
                lblStatus.Text = "デバイスから呼吸音を受信中...(Data " + numReceivedBreath + ")";
                Debug.WriteLine("Receive Data From Command 0x" + commandCode.ToString("X") + "(Count " + numReceivedBreath + ")");

                receivedBreathData.Add(receivedData);
                if (numReceivedBreath == MAX_NUM_RECEIVED)
                {
                    UpdateDeviceStatus(DeviceStatus.RECEIVE_BREATH_VOLUME_OK);

                    micScanResult = updateMikeChartArea(receivedBreathData); //呼吸音判定の処理

                    System.Threading.Thread.Sleep(1500);

                    sendCommandDetectMikeFinish();
                }
            }
            else if (commandCode == Constant.CommandReceiveAcceleSensor) //加速度センサー値送信[0xA8]の受信(デバイスから)
            {
                if (numReceivedAccele >= MAX_NUM_RECEIVED)
                {
                    //System.Threading.Thread.Sleep(500); //Wait to not receive more data, to send next command
                    return;
                }

                SetAcceData(receivedData[1], receivedData[3], receivedData[5]);
                numReceivedAccele++;
                lblStatus.Text = "デバイスから加速度センサー値を受信中...(Data " + numReceivedAccele + ")";
                Debug.WriteLine("Receive Data From Command 0x" + commandCode.ToString("X") + "(Count " + numReceivedAccele + ")");

                receivedAcceleData.Add(receivedData);
                if (numReceivedAccele == MAX_NUM_RECEIVED)
                {
                    UpdateDeviceStatus(DeviceStatus.RECEIVE_ACCELE_SENSOR_OK);

                    acceScanResult = updateAcceleSensorChartArea(receivedAcceleData); //加速度センサー判定の処理

                    System.Threading.Thread.Sleep(1500);

                    sendCommandDetectAcceleSensorFinish();
                }
            }
            else if (commandCode == Constant.CommandReceiveWearSensor) //装着センサー値送信[0xA9]の受信(デバイスから)
            {
                if (numReceivedWear >= MAX_NUM_RECEIVED)
                {
                    //System.Threading.Thread.Sleep(500); //Wait to not receive more data, to send next command
                    return;
                }

                SetPotoData(receivedData[1] << 2);
                numReceivedWear++;
                lblStatus.Text = "デバイスから装着センサー値を受信中...(Data " + numReceivedWear + ")";
                Debug.WriteLine("Receive Data From Command 0x" + commandCode.ToString("X") + "(Count " + numReceivedWear + ")");

                receivedWearData.Add(receivedData);
                if (numReceivedWear == MAX_NUM_RECEIVED)
                { 
                    UpdateDeviceStatus(DeviceStatus.RECEIVE_WEAR_SENSOR_OK);

                    photoScanResult = updateWearSensorChartArea(receivedWearData); //装着センサー判定の処理 

                    System.Threading.Thread.Sleep(1500);

                    sendCommandDetectWearSensorFinish();
                }
            }
        }

        private void SetMicData(int breathData)
        {
            lock (lockData)
            {
                // 呼吸データ
                if (MicDataRespQueue.Count >= GraphDataNum)
                {
                    MicDataRespQueue.Dequeue();
                }
                MicDataRespQueue.Enqueue(breathData);
            }
        }

        private void SetAcceData(int acceXData, int acceYData, int acceZData)
        {
            lock (lockData)
            {
                // 加速度データ
                if (AcceXDataRespQueue.Count >= GraphDataNum)
                {
                    AcceXDataRespQueue.Dequeue();
                }
                AcceXDataRespQueue.Enqueue(acceXData);

                if (AcceYDataRespQueue.Count >= GraphDataNum)
                {
                    AcceYDataRespQueue.Dequeue();
                }
                AcceYDataRespQueue.Enqueue(acceYData);

                if (AcceZDataRespQueue.Count >= GraphDataNum)
                {
                    AcceZDataRespQueue.Dequeue();
                }
                AcceZDataRespQueue.Enqueue(acceZData);
            }
        }

        private void SetPotoData(int photoData)
        {
            lock (lockData)
            {
                // 呼吸データ
                if (PhotoDataRespQueue.Count >= GraphDataNum)
                {
                    PhotoDataRespQueue.Dequeue();
                }
                PhotoDataRespQueue.Enqueue(photoData);
            }
        }

        //[START]ボタンクリック
        private async void sendCommandStatusChange()
        {
            bool isSentOk = false;
            byte commandCode = Constant.CommandStatusChange;
            byte commandStatus = 6; //状態 (0:待機状態, 3:GET状態, 4:SET状態(デバッグ機能), 5:プログラム更新状態(G1D), 6:生産工程検査)
            string commandName = "状態変更コマンド[0xB0]";

            byte[] commandData = new byte[] { commandCode, commandStatus };

            var result = await sendCommand(commandCode, commandName, commandData);
            if(result)
            {
                isSentOk = await isCommandSent(commandCode, commandName);
                if (isSentOk)
                {
                    log.scanLogWrite(g_address, "OK", "0");

                    btnNG.Enabled = true; //検査開始でクリック有効化する

                    UpdateDeviceStatus(DeviceStatus.STATUS_CHANGE_OK); //ボタンを[OK(手動)]にする
                    sendCommandDetectBatteryStart();
                }
            }

            if(!isSentOk)
            {
                log.scanLogWrite(g_address, "NG", "0");
            }
        }

        private async void sendCommandDetectBatteryStart()
        {
            bool isSentOk = false;
            byte commandCode = Constant.CommandDetectBattery;
            byte commandStatus = (byte)CommandFlag.START; 
            string commandName = "充電検査開始[0xA0]";

            byte[] commandData = new byte[] { commandCode, commandStatus };

            var result = await sendCommandAuto(commandCode, commandName, commandData);
            if (result)
            {
                isSentOk = await isCommandSent(commandCode, commandName);
                if (isSentOk)
                {
                    log.scanLogWrite(g_address, "OK", "1");

                    btnBattery.BackColor = Color.Yellow;
                    UpdateDeviceStatus(DeviceStatus.DETECT_BATTERY_OK);
                }  
            }

            if (!isSentOk)
            {
                log.scanLogWrite(g_address, "NG", "1");
            }
        }

        //[OK(手動)]ボタンクリック
        private async void sendCommandDetectBatteryFinish()
        {
            bool isSentOk = false;
            byte commandCode = Constant.CommandDetectBattery;
            byte commandStatus = (byte)CommandFlag.FINISH;
            string commandName = "充電検査終了[0xA0]";

            byte[] commandData = new byte[] { commandCode, commandStatus };

            var result = await sendCommand(commandCode, commandName, commandData);
            if (result)
            {
                isSentOk = await isCommandSent(commandCode, commandName);
                if (isSentOk)
                {
                    log.scanLogWrite(g_address, "OK", "1");

                    int okNum = Convert.ToInt32(lblNumCheckBatteryOK.Text) + 1;
                    lblNumCheckBatteryOK.Text = okNum.ToString();

                    btnBattery.BackColor = Color.White; //Reset
                    sendCommandDetectLEDStart();
                }
            }

            if(!isSentOk)
            {
                log.scanLogWrite(g_address, "NG", "1");

                int ngNum = Convert.ToInt32(lblNumCheckBatteryNG.Text) + 1;
                lblNumCheckBatteryNG.Text = ngNum.ToString();
            }
        }

        private async void sendCommandDetectLEDStart()
        {
            bool isSentOk = false;
            byte commandCode = Constant.CommandDetectLED;
            byte commandStatus = (byte)CommandFlag.START;
            string commandName = "LED検査開始[0xA1]";

            byte[] commandData = new byte[] { commandCode, commandStatus };

            var result = await sendCommandAuto(commandCode, commandName, commandData);
            if (result)
            {
                isSentOk = await isCommandSent(commandCode, commandName);
                if (isSentOk)
                {
                    log.scanLogWrite(g_address, "OK", "2");

                    btnLED.BackColor = Color.Yellow;
                    UpdateDeviceStatus(DeviceStatus.DETECT_LED_OK);
                }
            }

            if (!isSentOk)
            {
                log.scanLogWrite(g_address, "NG", "2");
            }
        }

        //[OK(手動)]ボタンクリック
        private async void sendCommandDetectLEDFinish()
        {
            bool isSentOk = false;
            byte commandCode = Constant.CommandDetectLED;
            byte commandStatus = (byte)CommandFlag.FINISH;
            string commandName = "LED検査終了[0xA1]";

            byte[] commandData = new byte[] { commandCode, commandStatus };

            var result = await sendCommand(commandCode, commandName, commandData);
            if (result)
            {
                isSentOk = await isCommandSent(commandCode, commandName);
                if (isSentOk)
                {
                    log.scanLogWrite(g_address, "OK", "2");

                    int okNum = Convert.ToInt32(lblNumCheckLedOK.Text) + 1;
                    lblNumCheckLedOK.Text = okNum.ToString();

                    btnLED.BackColor = Color.White; //Reset
                    sendCommandDetectVibrationStart();
                }
            }

            if (!isSentOk)
            {
                log.scanLogWrite(g_address, "NG", "2");

                int ngNum = Convert.ToInt32(lblNumCheckLedNG.Text) + 1;
                lblNumCheckLedNG.Text = ngNum.ToString();
            }
        }

        private async void sendCommandDetectVibrationStart()
        {
            bool isSentOk = false;
            byte commandCode = Constant.CommandDetectVibration;
            byte commandStatus = (byte)CommandFlag.START;
            string commandName = "バイブレーション検査開始[0xA2]";

            byte[] commandData = new byte[] { commandCode, commandStatus };

            var result = await sendCommandAuto(commandCode, commandName, commandData);
            if (result)
            {
                isSentOk = await isCommandSent(commandCode, commandName);
                if (isSentOk)
                {
                    log.scanLogWrite(g_address, "OK", "3");

                    btnVibration.BackColor = Color.Yellow;
                    UpdateDeviceStatus(DeviceStatus.DETECT_VIBRATION_OK);
                }
            }

            if (!isSentOk)
            {
                log.scanLogWrite(g_address, "NG", "3");
            }
        }

        //[OK(手動)]ボタンクリック
        private async void sendCommandDetectVibrationFinish()
        {
            bool isSentOk = false;
            byte commandCode = Constant.CommandDetectVibration;
            byte commandStatus = (byte)CommandFlag.FINISH;
            string commandName = "バイブレーション検査終了[0xA2]";

            byte[] commandData = new byte[] { commandCode, commandStatus };

            var result = await sendCommand(commandCode, commandName, commandData);
            if (result)
            {
                isSentOk = await isCommandSent(commandCode, commandName);
                if (isSentOk)
                {
                    log.scanLogWrite(g_address, "OK", "3");

                    int okNum = Convert.ToInt32(lblNumCheckVibOK.Text) + 1;
                    lblNumCheckVibOK.Text = okNum.ToString();

                    btnVibration.BackColor = Color.White; //Reset
                    UpdateDeviceStatus(DeviceStatus.DETECT_VIBRATION_FINISH_OK);
                    sendCommandDetectMikeStart();
                } 
            }

            if (!isSentOk)
            {
                log.scanLogWrite(g_address, "NG", "3");

                int ngNum = Convert.ToInt32(lblNumCheckVibNG.Text) + 1;
                lblNumCheckVibNG.Text = ngNum.ToString();
            }
        }

        private async void sendCommandDetectMikeStart()
        {
            byte commandCode = Constant.CommandDetectMike;
            byte commandStatus = (byte)CommandFlag.START;
            string commandName = "マイク検査開始[0xA3]";

            byte[] commandData = new byte[] { commandCode, commandStatus };

            isDetectStartMike = true; //Important
            var result = await sendCommandAuto(commandCode, commandName, commandData);
            if (!result)
            {
                log.scanLogWrite(g_address, "NG", "4");
            }
        }

        private async void sendCommandDetectMikeFinish()
        {
            byte commandCode = Constant.CommandDetectMike;
            byte commandStatus = (byte)CommandFlag.FINISH;
            string commandName = "マイク検査終了[0xA3]";

            byte[] commandData = new byte[] { commandCode, commandStatus };

            var result = await sendCommandAuto(commandCode, commandName, commandData);
            if (!result)
            {
                UpdateCheckMikeResultOnTable(result);
            }
        }

        private async void sendCommandDetectAcceleSensorStart()
        {
            byte commandCode = Constant.CommandDetectAcceleSensor;
            byte commandStatus = (byte)CommandFlag.START;
            string commandName = "加速度センサー検査開始[0xA4]";

            byte[] commandData = new byte[] { commandCode, commandStatus };

            isDetectStartAccele = true; //Important
            var result = await sendCommandAuto(commandCode, commandName, commandData);
            if (!result)
            {
                log.scanLogWrite(g_address, "NG", "5");
            }
        }

        private async void sendCommandDetectAcceleSensorFinish()
        {
            byte commandCode = Constant.CommandDetectAcceleSensor;
            byte commandStatus = (byte)CommandFlag.FINISH;
            string commandName = "加速度センサー検査終了[0xA4]";

            byte[] commandData = new byte[] { commandCode, commandStatus };

            var result = await sendCommandAuto(commandCode, commandName, commandData);
            if (!result)
            {
                UpdateCheckAclResultOnTable(result);
            }
        }

        private async void sendCommandDetectWearSensorStart()
        {
            byte commandCode = Constant.CommandDetectWearSensor;
            byte commandStatus = (byte)CommandFlag.START;
            string commandName = "装着センサー検査開始[0xA5]";

            byte[] commandData = new byte[] { commandCode, commandStatus };

            isDetectStartWear = true; //Important
            var result = await sendCommandAuto(commandCode, commandName, commandData);
            if (!result)
            {
                log.scanLogWrite(g_address, "NG", "6");
            }
        }

        private async void sendCommandDetectWearSensorFinish()
        {
            byte commandCode = Constant.CommandDetectWearSensor;
            byte commandStatus = (byte)CommandFlag.FINISH;
            string commandName = "装着センサー検査終了[0xA5]";

            byte[] commandData = new byte[] { commandCode, commandStatus };

            var result = await sendCommandAuto(commandCode, commandName, commandData);
            if (!result)
            {
                UpdateCheckWearResultOnTable(result);
            }
        }

        private async void sendCommandDetectEEPROMStart()
        {
            btnEEPROM.BackColor = Color.Yellow;

            byte commandCode = Constant.CommandDetectEEPROM;
            string commandName = "EEPROM検査開始[0xA6]";

            byte[] commandData = new byte[] { commandCode };

            var result = await sendCommandAuto(commandCode, commandName, commandData);
            if (!result)
            {
                log.scanLogWrite(g_address, "NG", "7");
            }
        }

        private async void sendCommandSendPowerSWOff()
        {
            byte commandCode = Constant.CommandSendPowerSWOff;
            string commandName = "電源SW OFF送信[0xF0]";

            byte[] commandData = new byte[] { commandCode };

            var result = await sendCommandAuto(commandCode, commandName, commandData);
            if (!result)
            {
                UpdateCheckEEPROMResultOnTable(result);

                if (isNGClick)
                {
                    lblCheckResult.Text = "NG";
                    lblCheckResult.ForeColor = Color.Red;

                    numNotGood++;
                    updateResultTable();

                    isNGClick = false;
                }
            }
        }

        //マイクグラフに描画
        public bool updateMikeChartArea(List<byte[]> receivedData)
        {
            bool ret = false;
            Debug.WriteLine("Update Mike Chart Area");
            lblStatus.Text = "マイクチャットを更新しました";
            foreach(byte data in receivedData[1])
            {
                if((data << 2) >= breath_min)
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        //加速度センサーグラフに描画
        public bool updateAcceleSensorChartArea(List<byte[]> receivedData)
        {
            bool ret = true;

            int x = 0;
            int y = 0; //加速度センサー（Ｙ）//receiveData[2]
            int z = 0; //加速度センサー（Ｚ）//receiveData[3]

            Debug.WriteLine("Update Accele Sensor Chart Area");
            lblStatus.Text = "加速度センサーチャットを更新しました";


            foreach (byte[] data in receivedData)
            {
                x = Convert.ToInt16(data[1]);
                if((x & 0x80) == 0x80)
                {
                    x = x - 256;
                }
                if (x <= acce_x_min || x >= acce_x_max)
                {
                    ret = false;
                    break;
                }

                y = Convert.ToInt16(data[3]);
                if ((y & 0x80) == 0x80)
                {
                    y = y - 256;
                }
                if (y <= acce_y_min || y >= acce_y_max)
                {
                    ret = false;
                    break;
                }

                z = Convert.ToInt16(data[5]);
                if ((z & 0x80) == 0x80)
                {
                    z = z - 256;
                }
                if (z <= acce_z_min || z >= acce_z_max)
                {
                    ret = false;
                    break;
                }
            }

            return ret;
        }

        //装着センサーグラフに描画
        public bool updateWearSensorChartArea(List<byte[]> receivedData)
        {
            bool ret = false;

            Debug.WriteLine("Update Wear Sensor Chart Area");
            lblStatus.Text = "装着センサーチャットを更新しました";

            foreach (int data in receivedData[1])
            {
                if ((data << 2) >= photo_min)
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        public void updateResultTable()
        {
            int numTotal = numGood + numNotGood;
            lblNumGood.Text  = numGood.ToString();
            lblNumNG.Text    = numNotGood.ToString();
            lblNumTotal.Text = numNotGood.ToString();

            if(numTotal > 0)
            {
                double ngRate = System.Math.Round((double)(numNotGood / numTotal * 100), 1);
                lblRate.Text = $"{ngRate}%";
            } else
            {
                lblRate.Text = "0.0%";
            }
        }

        private async Task<bool> sendCommand(byte commandCode, string commandName, byte[] commandData)
        {
            if (writeCharacteristic != null)
            {
                DialogResult res = MessageBox.Show(commandName + "を実行しますか？", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (res == DialogResult.OK)
                {
                    lblStatus.Text = commandName + "を送信中...";

                    var result = await WriteCommandToDevice(commandData);
                    if (result == CommandResult.SUCCESS)
                    {
                        lblStatus.Text = commandName + "を成功に送信しました";
                        return true;
                    }
                    else
                    {
                        lblStatus.Text = commandName + "を失敗に送信しました";
                    }
                }
                else if (res == DialogResult.Cancel)
                {
                    //Do nothing
                }
            }
            else
            {
                UpdateDeviceStatus(DeviceStatus.NOT_CONNECT, Constant.MSG_SLEEIM_IS_NOT_CONNECTING);
            }

            return false;
        }

        private async Task<bool> sendCommandAuto(byte commandCode, string commandName, byte[] commandData)
        {
            if (writeCharacteristic != null)
            {
                lblStatus.Text = commandName + "を送信中...";

                var result = await WriteCommandToDevice(commandData);
                if (result == CommandResult.SUCCESS)
                {
                    lblStatus.Text = commandName + "を成功に送信しました";
                    return true;
                }
                else
                {
                    lblStatus.Text = commandName + "を失敗に送信しました";
                }
            }
            else
            {
                UpdateDeviceStatus(DeviceStatus.NOT_CONNECT, Constant.MSG_SLEEIM_IS_NOT_CONNECTING);
            }

            return false;
        }

        public async Task<bool> RegisterNotificationWhenValueChanged()
        {
            GattCharacteristicProperties properties = readCharacteristic.CharacteristicProperties;

            var configDesValue = GattClientCharacteristicConfigurationDescriptorValue.None;
            if (properties.HasFlag(GattCharacteristicProperties.Indicate))
            {
                configDesValue = GattClientCharacteristicConfigurationDescriptorValue.Indicate;
            } else if (properties.HasFlag(GattCharacteristicProperties.Notify)) {
                configDesValue = GattClientCharacteristicConfigurationDescriptorValue.Notify;
            }

            try
            {
                // Write the ClientCharacteristicConfigurationDescriptor in order for server to send notifications
                //GattWriteResult status = await chara.WriteClientCharacteristicConfigurationDescriptorWithResultAsync(configDesValue);
                var result = await readCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(configDesValue);
                if (result == GattCommunicationStatus.Success)
                {
                    readCharacteristic.ValueChanged += ReadCharacteristic_ValueChanged;
                    Debug.WriteLine("デバイスからPCにNotification/Indicateの受信コールバックを成功に登録しました");

                    return true;
                }　else
                {
                    Debug.WriteLine("デバイスからPCにNotification/Indicateの受信コールバックを失敗に登録しました");
                }
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        // readCharacteristicのnotification有効化する
        public async Task<bool> EnableNotification()
        {
            var descs = await readCharacteristic.GetDescriptorsAsync();

            foreach (var desc in descs.Descriptors)
            {
                Debug.WriteLine($"Descriptor UUID {desc.Uuid}, Type:{desc.GetType()}");
                if (desc.Uuid.ToString() == Constant.ANDROID_CENTRAL_UUID)
                {
                    byte[] ENABLE_INDICATION_VALUE = { 0x02, 0x00 };
                    DataWriter dataWriter = new DataWriter();
                    dataWriter.WriteBytes(ENABLE_INDICATION_VALUE);
                    IBuffer writeBuffer = dataWriter.DetachBuffer();
                    var result = await desc.WriteValueAsync(writeBuffer);

                    if (result == GattCommunicationStatus.Success)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private async Task<bool> isCommandSent(byte commandCode, string commandName)
        {
            //Wait to reveive CommandResponse from device to PC (Wait readCharacteristic.Value_Changed)
            System.Threading.Thread.Sleep(1000);

            byte[] data = await ReadValue(readCharacteristic);
            if (data != null && data.Length >= 2 && data[0] == commandCode && data[1] == (byte)CommandReturn.SUCCESS) 
            {
                return true;
            }
            else
            {
                lblStatus.Text = commandName + "を失敗に送信しました";
                return false;
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ウインドウを閉じる時に最終結果を書き込み
            log.resultLogWrite(lblCheckPlanNum.Text, lblNumTotal.Text, lblNumGood.Text, lblNumNG.Text);
        }

        private async Task<byte[]> getDataFromDevice(byte receiveCommandCode, string receiveCommandName)
        {
            //Wait to reveive CommandResponse from device to PC (Wait readCharacteristic.Value_Changed)
            System.Threading.Thread.Sleep(1000);

            byte[] receiveData = await ReadValue(readCharacteristic);

            bool receivedFlag = true;
            if(receiveData == null || receiveData.Length < 1)
            {
                receivedFlag = false;
            } else
            {
                if (receiveData[0] != receiveCommandCode)
                {
                    receivedFlag = false;
                }
            }

            if(!receivedFlag)
            {
                lblStatus.Text = "デバイスから" + receiveCommandName + "を失敗に受信しました";
            }

            return receiveData;
        }

        private void UpdateCheckMikeResultOnTable(bool isSentOk)
        {
            log.scanLogWrite(g_address, isSentOk ? "OK" : "NG", "4");
            if (isSentOk && micScanResult)
            {// コマンド応答OK & 判定OK
                int okNum = Convert.ToInt32(lblNumCheckMicOK.Text) + 1;
                lblNumCheckMicOK.Text = okNum.ToString();
            }
            else
            {// コマンド送信失敗 or コマンド応答NG
                int ngNum = Convert.ToInt32(lblNumCheckMicNG.Text) + 1;
                lblNumCheckMicNG.Text = ngNum.ToString();
            }
        }

        private void UpdateCheckAclResultOnTable(bool isSentOk)
        {
            log.scanLogWrite(g_address, isSentOk ? "OK" : "NG", "5");
            if (isSentOk && acceScanResult)
            {
                int okNum = Convert.ToInt32(lblNumCheckAclOK.Text) + 1;
                lblNumCheckAclOK.Text = okNum.ToString();
            }
            else
            {
                int ngNum = Convert.ToInt32(lblNumCheckAclNG.Text) + 1;
                lblNumCheckAclNG.Text = ngNum.ToString();
            }
        }

        private void UpdateCheckWearResultOnTable(bool isSentOk)
        {
            log.scanLogWrite(g_address, isSentOk ? "OK" : "NG", "6");
            if (isSentOk && photoScanResult)
            {
                int okNum = Convert.ToInt32(lblNumCheckPhotoOK.Text) + 1;
                lblNumCheckPhotoOK.Text = okNum.ToString();
            }
            else
            {
                int ngNum = Convert.ToInt32(lblNumCheckPhotoNG.Text) + 1;
                lblNumCheckPhotoNG.Text = ngNum.ToString();
            }
        }

        private void UpdateCheckEEPROMResultOnTable(bool isSentOk)
        {
            log.scanLogWrite(g_address, isSentOk ? "OK" : "NG", "7");
            if (isSentOk)
            {
                int okNum = Convert.ToInt32(lblNumCheckEepOK.Text) + 1;
                lblNumCheckEepOK.Text = okNum.ToString();
            }
            else
            {
                int ngNum = Convert.ToInt32(lblNumCheckEepNG.Text) + 1;
                lblNumCheckEepNG.Text = ngNum.ToString();
            }
        }

        private async void DeviceAdded(DeviceWatcher watcher, DeviceInformation device)
        {
            //Debug.WriteLine("Device added");
        }

        private void DeviceUpdated(DeviceWatcher watcher, DeviceInformationUpdate update)
        {
            //Debug.WriteLine($"Device updated: {update.Id}");
        }

        private void CustomOnPairingRequested(DeviceInformationCustomPairing sender, DevicePairingRequestedEventArgs args)
        {
            args.Accept();
        }
    }
}
