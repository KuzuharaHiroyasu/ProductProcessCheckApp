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

        private Timer connectTimer = new Timer();
        private Boolean startedFlag = false;
        private DeviceStatus deviceStatus = DeviceStatus.NOT_CONNECT;

        private IniFile ini;

        private int numGood    = 0;
        private int numNotGood = 0;

        private int numGoodCheck1 = 0;

        private const int GraphDataNum = 40 + 1;
        Queue<double> MicDataRespQueue = new Queue<double>();
        Queue<double> AclXDataRespQueue = new Queue<double>();
        Queue<double> AclYDataRespQueue = new Queue<double>();
        Queue<double> AclZDataRespQueue = new Queue<double>();
        Queue<double> PhotoDataRespQueue = new Queue<double>();
        object lockData = new object();

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
            AclXDataRespQueue.Clear();
            AclYDataRespQueue.Clear();
            AclZDataRespQueue.Clear();
            PhotoDataRespQueue.Clear();

            for (int i = 0; i < GraphDataNum; i++)
            {
                MicDataRespQueue.Enqueue(0);
                AclXDataRespQueue.Enqueue(0);
                AclYDataRespQueue.Enqueue(0);
                AclZDataRespQueue.Enqueue(0);
                PhotoDataRespQueue.Enqueue(0);
            }

            GraphUpdate_Apnea();
        }

        private void GraphUpdate_Apnea()
        {
            int cnt = 0;

            lock (lockData)
            {
                Series srs_micresp = chart_mic.Series["マイク"];
                Series srs_aclxresp = chart_acl.Series["X軸"];
                Series srs_aclyresp = chart_acl.Series["Y軸"];
                Series srs_aclzresp = chart_acl.Series["Z軸"];
                Series srs_photoresp = chart_photo.Series["装着センサー"];
                srs_micresp.Points.Clear();
                srs_aclxresp.Points.Clear();
                srs_aclyresp.Points.Clear();
                srs_aclzresp.Points.Clear();
                srs_photoresp.Points.Clear();
                cnt = 0;
                foreach (double data in MicDataRespQueue)
                {
                    srs_micresp.Points.AddXY(cnt, data);
                    srs_aclxresp.Points.AddXY(cnt, data);
                    srs_aclyresp.Points.AddXY(cnt, data);
                    srs_aclzresp.Points.AddXY(cnt, data);
                    srs_photoresp.Points.AddXY(cnt, data);
                    cnt++;
                }
            }
        }
            private async void btnConnect_Click(object sender, EventArgs e)
        {
            if(deviceStatus == DeviceStatus.NOT_CONNECT || deviceStatus == DeviceStatus.CONNECT_FAILED) //接続処理
            {
                DisconnectDevice(false);

                SetupSearchTimeOut();
                SetupBluetooth();

                lblStatus.Text = Constant.MSG_SLEEIM_IS_SEARCHING;
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
                //MessageBox.Show("未対応", Constant.APP_NAME);
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
            lblCheckResult.Text = "NG";
            lblCheckResult.ForeColor = Color.Red;

            //MessageBox.Show("未対応", Constant.APP_NAME);
            numNotGood++;
            updateResultTable();
        }

        private void UpdateDeviceStatus(DeviceStatus status, String statusMessage = "", bool showDialog = true)
        {
            deviceStatus = status;
            btnConnect.Enabled = true;
            if (status == DeviceStatus.CONNECT_SUCCESS)
            {
                btnConnect.Text = "START";
                btnDisconnect.Enabled = true;
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
            }
        }

        private void LoadIniFile()
        {
            ini = new IniFile("Setting.ini");
            //ini.Write("HomePage", "http://www.google.com");

            var modelName = ini.Read("MODEL", "MODEL_NAME");
            var version = ini.Read("BUILD_VER", "VERSION");
            if (modelName != "")
            {
                lblModelName.Text = "機種名：" + modelName;
            }

            if(version != "")
            {
                lblVersion.Text = "Ver：" + version;
            }
        }

        private void CustomGUI()
        {
            var blueColor = ColorTranslator.FromHtml("#65a7e3");
            btnDisconnect.BackColor = blueColor;
            btnConnect.BackColor    = blueColor;
            btnNG.BackColor         = ColorTranslator.FromHtml("#ff842e");

            lblTitleGood.ForeColor = ColorTranslator.FromHtml("#ff842e");
            lblTitleNG.ForeColor   = ColorTranslator.FromHtml("#fe0000");
            lblTitleRate.ForeColor = ColorTranslator.FromHtml("#246794");

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

                        //RegisterNotificationWhenValueChanged(readCharacteristic);
                        RegisterNotificationWhenValueChanged(writeCharacteristic);
                    }
                    else
                    {
                        UpdateDeviceStatus(DeviceStatus.NOT_CONNECT, "Sleeim[" + address + "]を失敗に接続しました", false);
                    }
                } catch(Exception e)
                {
                    UpdateDeviceStatus(DeviceStatus.NOT_CONNECT, "Sleeim[" + address + "]を失敗に接続しました", false);
                }
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
                watcher.Stop();
                deviceWatcher.Stop();
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

        private void CustomOnPairingRequested(DeviceInformationCustomPairing sender, DevicePairingRequestedEventArgs args)
        {
            args.Accept();
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

                        if (readCharacteristic != null)
                        {
                            Console.WriteLine($"Read Characteristic Selected: {Utility.GetUUIDString(readCharacteristic.Uuid)}");
                            var charResult = await readCharacteristic.WriteClientCharacteristicConfigurationDescriptorWithResultAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

                            readCharacteristic.ValueChanged += ReadCharacteristic_ValueChanged;
                        }

                        if (writeCharacteristic != null)
                        {
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

                UpdateDeviceStatus(DeviceStatus.NOT_CONNECT, Constant.MSG_SLEEIM_NOT_FOUND);
            }
        }

        private void DateTimer_Tick(object sender, EventArgs e)
        {
            this.lblCurrentDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        }

        public async Task SendCommandToDevice(string commandInfo)
        {
            byte[] byteData = new byte[commandInfo.Length];
            int commandLength = commandInfo.Length;

            for (int cmdChar = 0; cmdChar < commandInfo.Length; cmdChar++)
            {
                byteData[cmdChar] = Convert.ToByte(commandInfo[cmdChar]);
            }

            byte[] tmp = new byte[commandLength];
            //BLECommon.ByteCopy(byteData, ref tmp, 0, commandLength);

            await WriteCommandToDevice(tmp);
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
                    //await selectedReadCharacter.KeepRunnig(selectedWriteCharacter, sndbuf);
                    if (result == GattCommunicationStatus.Unreachable)
                    {
                        //MessageBox.Show("Command Write Failed (Unreachable)");
                        Debug.WriteLine("Command Write Failed (Unreachable)");
                        return CommandResult.UNREACHABLE;
                    }
                    else if (result == GattCommunicationStatus.ProtocolError)
                    {
                        //MessageBox.Show("Command Write Failed (ProtocolError)");
                        Debug.WriteLine("Command Write Failed (ProtocolError)");
                        return CommandResult.PROTOCOL_ERROR;
                    }
                    else if (result == GattCommunicationStatus.Success)
                    {
                        //MessageBox.Show("Command Write Successfully");
                        return CommandResult.SUCCESS;
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Command Write Failed. Exception: " + ex.Message);
                    Debug.WriteLine("Write, Exception: " + ex.Message);
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
                    var RawBufLen = (int)result.Value.Length;

                    CryptographicBuffer.CopyToByteArray(result.Value, out data);
                }
            }

            return data;
        }

        private async Task SendValues(byte[] toSend)
        {
            IBuffer writer = toSend.AsBuffer();
            try
            {
                // BT_Code: Writes the value from the buffer to the characteristic.  
                var gattCharacteristic = gattService.GetCharacteristics(new Guid(Constant.UUID_CHAR_WRITE)).First();
                var result = await gattCharacteristic.WriteValueAsync(writer);
                if (result == GattCommunicationStatus.Success)
                {
                    //Use for debug or notyfy
                    var dialog = new Windows.UI.Popups.MessageDialog("Succes");
                    await dialog.ShowAsync();
                }
                else
                {
                    var dialog = new Windows.UI.Popups.MessageDialog("Failed");
                    await dialog.ShowAsync();
                }
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80650003 || (uint)ex.HResult == 0x80070005)
            {
                // E_BLUETOOTH_ATT_WRITE_NOT_PERMITTED or E_ACCESSDENIED
                // This usually happens when a device reports that it
                //support writing, but it actually doesn't.
                var dialog = new Windows.UI.Popups.MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }
        }

        private void WriteCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            Debug.WriteLine("WriteCharacteristic_ValueChanged Here");
            byte[] data = new byte[args.CharacteristicValue.Length];
            DataReader.FromBuffer(args.CharacteristicValue).ReadBytes(data);

            //Do something
        }

        private static void ReadCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            Debug.WriteLine("ReadCharacteristic_ValueChanged Here");
            byte[] data = new byte[args.CharacteristicValue.Length];
            DataReader.FromBuffer(args.CharacteristicValue).ReadBytes(data);
        }

        private async void DeviceAdded(DeviceWatcher watcher, DeviceInformation device)
        {
            //Debug.WriteLine("Device added");
        }

        private void DeviceUpdated(DeviceWatcher watcher, DeviceInformationUpdate update)
        {
            Debug.WriteLine($"Device updated: {update.Id}");
        }

        //[START]ボタンクリック
        private async void sendCommandStatusChange()
        {
            int commandStatus = 6; //状態 (0:待機状態, 3:GET状態, 4:SET状態(デバッグ機能), 5:プログラム更新状態(G1D), 6:生産工程検査)
            byte[] commandData = new byte[] { Constant.CommandStatusChange, (byte)commandStatus };

            var result = await sendCommand(Constant.CommandStatusChange, "状態変更コマンド[0xB0]", commandData);
            if(result)
            {
                UpdateDeviceStatus(DeviceStatus.STATUS_CHANGE_OK); //ボタンを[OK(手動)]にする
                sendCommandDetectBatteryStart();
            }
        }

        private async void sendCommandDetectBatteryStart()
        {
            byte[] commandData = new byte[] { Constant.CommandDetectBattery, (byte)CommandFlag.START };

            var result = await sendCommandAuto(Constant.CommandDetectBattery, "充電検査開始[0xA0]", commandData);
            if (result)
            {
                btnBattery.BackColor = Color.Yellow;
                UpdateDeviceStatus(DeviceStatus.DETECT_BATTERY_OK);
            }
        }

        //[OK(手動)]ボタンクリック
        private async void sendCommandDetectBatteryFinish()
        {
            int okNum;
            byte[] commandData = new byte[] { Constant.CommandDetectBattery, (byte)CommandFlag.FINISH };

            var result = await sendCommand(Constant.CommandDetectBattery, "充電検査終了[0xA0]", commandData);
            if (result)
            {
                btnBattery.BackColor = Color.White; //Reset
                sendCommandDetectLEDStart();
            }
            okNum = Convert.ToInt32(lblNumCheckBatteryOK.Text) + 1;
            lblNumCheckBatteryOK.Text = okNum.ToString();
        }

        private async void sendCommandDetectLEDStart()
        {
            byte[] commandData = new byte[] { Constant.CommandDetectLED, (byte)CommandFlag.START };

            var result = await sendCommandAuto(Constant.CommandDetectLED, "LED検査開始[0xA1]", commandData);
            if (result)
            {
                btnLED.BackColor = Color.Yellow;
                UpdateDeviceStatus(DeviceStatus.DETECT_LED_OK);
            }
        }

        //[OK(手動)]ボタンクリック
        private async void sendCommandDetectLEDFinish()
        {
            int okNum;
            byte[] commandData = new byte[] { Constant.CommandDetectLED, (byte)CommandFlag.FINISH };

            var result = await sendCommand(Constant.CommandDetectLED, "LED検査終了[0xA1]", commandData);
            if (result)
            {
                btnLED.BackColor = Color.White; //Reset
                sendCommandDetectVibrationStart();
            }
            okNum = Convert.ToInt32(lblNumCheckLedOK.Text) + 1;
            lblNumCheckLedOK.Text = okNum.ToString();
        }

        private async void sendCommandDetectVibrationStart()
        {
            byte[] commandData = new byte[] { Constant.CommandDetectVibration, (byte)CommandFlag.START };

            var result = await sendCommandAuto(Constant.CommandDetectVibration, "バイブレーション検査開始[0xA2]", commandData);
            if (result)
            {
                btnVibration.BackColor = Color.Yellow;
                UpdateDeviceStatus(DeviceStatus.DETECT_VIBRATION_OK);
            }
        }

        //[OK(手動)]ボタンクリック
        private async void sendCommandDetectVibrationFinish()
        {
            int okNum;
            byte[] commandData = new byte[] { Constant.CommandDetectVibration, (byte)CommandFlag.FINISH };

            var result = await sendCommand(Constant.CommandDetectVibration, "バイブレーション検査終了[0xA2]", commandData);
            if (result)
            {
                btnVibration.BackColor = Color.White; //Reset
                UpdateDeviceStatus(DeviceStatus.DETECT_VIBRATION_FINISH_OK);
                sendCommandDetectMikeStart();
            }
            okNum = Convert.ToInt32(lblNumCheckVibOK.Text) + 1;
            lblNumCheckVibOK.Text = okNum.ToString();
        }

        private async void sendCommandDetectMikeStart()
        {
            byte[] commandData = new byte[] { Constant.CommandDetectMike, (byte)CommandFlag.START };

            var result = await sendCommandAuto(Constant.CommandDetectMike, "マイク検査開始[0xA3]", commandData);
            if (result)
            {
                btnMike.BackColor = Color.Yellow;
                UpdateDeviceStatus(DeviceStatus.DETECT_MIKE_OK);
                sendCommandSendBreathVolume();
            }
        }

        private async void sendCommandSendBreathVolume()
        {
            byte[] commandData = new byte[] { Constant.CommandSendBreathVolume };

            var result = await sendCommandAuto(Constant.CommandSendBreathVolume, "呼吸音送信[0xA7]", commandData);
            if (result)
            {
                UpdateDeviceStatus(DeviceStatus.SEND_BREATH_VOLUME_OK);
                sendCommandDetectMikeFinish();
            }
        }

        private async void sendCommandDetectMikeFinish()
        {
            byte[] commandData = new byte[] { Constant.CommandDetectMike, (byte)CommandFlag.FINISH };

            var result = await sendCommandAuto(Constant.CommandDetectMike, "マイク検査終了[0xA3]", commandData);
            if (result)
            {
                btnMike.BackColor = Color.White; //Reset
                UpdateDeviceStatus(DeviceStatus.DETECT_MIKE_FINISH_OK);
                sendCommandDetectAcceleSensorStart();
            }
        }

        private async void sendCommandDetectAcceleSensorStart()
        {
            byte[] commandData = new byte[] { Constant.CommandDetectAcceleSensor, (byte)CommandFlag.START };

            var result = await sendCommandAuto(Constant.CommandDetectAcceleSensor, "加速度センサー検査開始[0xA4]", commandData);
            if (result)
            {
                btnAcceleSensor.BackColor = Color.Yellow;
                UpdateDeviceStatus(DeviceStatus.DETECT_ACCELE_SENSOR_OK);
                sendCommandSendAcceleSensor();
            }
        }

        private async void sendCommandSendAcceleSensor()
        {
            //Will implement later (TMP)
            byte x = 0; //加速度センサー（Ｘ）
            byte y = 0; //加速度センサー（Ｙ）
            byte z = 0; //加速度センサー（Ｚ）

            byte[] commandData = new byte[] { Constant.CommandSendAcceleSensor, x, y, z };

            var result = await sendCommandAuto(Constant.CommandSendAcceleSensor, "加速度センサー値送信[0xA8]", commandData);
            if (result)
            {
                UpdateDeviceStatus(DeviceStatus.SEND_ACCELE_SENSOR_OK);
                sendCommandDetectAcceleSensorFinish();
            }
        }

        private async void sendCommandDetectAcceleSensorFinish()
        {
            byte[] commandData = new byte[] { Constant.CommandDetectAcceleSensor, (byte)CommandFlag.FINISH };

            var result = await sendCommandAuto(Constant.CommandDetectAcceleSensor, "加速度センサー検査終了[0xA4]", commandData);
            if (result)
            {
                btnAcceleSensor.BackColor = Color.White; //Reset
                UpdateDeviceStatus(DeviceStatus.DETECT_ACCELE_SENSOR_FINISH_OK);
                sendCommandDetectWearSensorStart();
            }
        }

        private async void sendCommandDetectWearSensorStart()
        {
            byte[] commandData = new byte[] { Constant.CommandDetectWearSensor, (byte)CommandFlag.START };

            var result = await sendCommandAuto(Constant.CommandDetectWearSensor, "装着センサー検査開始[0xA5]", commandData);
            if (result)
            {
                btnWearSensor.BackColor = Color.Yellow;
                UpdateDeviceStatus(DeviceStatus.DETECT_WEAR_SENSOR_OK);
                sendCommandSendWearSensor();
            }
        }

        private async void sendCommandSendWearSensor()
        {
            byte[] commandData = new byte[] { Constant.CommandSendWearSensor };

            var result = await sendCommandAuto(Constant.CommandSendWearSensor, "装着センサー値送信[0xA9]", commandData);
            if (result)
            {
                UpdateDeviceStatus(DeviceStatus.SEND_WEAR_SENSOR_OK);
                sendCommandDetectWearSensorFinish();
            }
        }

        private async void sendCommandDetectWearSensorFinish()
        {
            byte[] commandData = new byte[] { Constant.CommandDetectWearSensor, (byte)CommandFlag.FINISH };

            var result = await sendCommandAuto(Constant.CommandDetectWearSensor, "装着センサー検査終了[0xA5]", commandData);
            if (result)
            {
                btnWearSensor.BackColor = Color.White; //Reset
                UpdateDeviceStatus(DeviceStatus.DETECT_WEAR_SENSOR_FINISH_OK);
                sendCommandDetectEEPROMStart();
            }
        }

        private async void sendCommandDetectEEPROMStart()
        {
            btnEEPROM.BackColor = Color.Yellow;
            byte[] commandData = new byte[] { Constant.CommandDetectEEPROM };

            var result = await sendCommandAuto(Constant.CommandDetectEEPROM, "EEPROM検査開始[0xA6]", commandData);
            if (result)
            {
                btnEEPROM.BackColor = Color.White; //Reset
                UpdateDeviceStatus(DeviceStatus.DETECT_EEPROM_OK);
                sendCommandSendPowerSWOff();
            }
        }

        private async void sendCommandSendPowerSWOff()
        {
            byte[] commandData = new byte[] { Constant.CommandSendPowerSWOff };

            var result = await sendCommandAuto(Constant.CommandSendPowerSWOff, "電源SW OFF送信[0xF0]", commandData);
            if (result)
            {
                UpdateDeviceStatus(DeviceStatus.SEND_POWER_OFF_OK);
                DisconnectDevice(true);

                btnFinish.BackColor = Color.Yellow;

                numGood++;
                updateResultTable();
                lblCheckResult.Text = "OK";
                //Write log here
            }
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

        public async void RegisterNotificationWhenValueChanged(GattCharacteristic chara)
        {
            GattCharacteristicProperties properties = chara.CharacteristicProperties;

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
                GattWriteResult status = await chara.WriteClientCharacteristicConfigurationDescriptorWithResultAsync(configDesValue);
                var result = await chara.WriteClientCharacteristicConfigurationDescriptorAsync(configDesValue);
                if (result == GattCommunicationStatus.Success)
                {
                    chara.ValueChanged += WriteCharacteristic_ValueChanged;
                }
            }
            catch (Exception ex)
            {
            }

            var currentDesValue = await chara.ReadClientCharacteristicConfigurationDescriptorAsync();
            if (currentDesValue.ClientCharacteristicConfigurationDescriptor != GattClientCharacteristicConfigurationDescriptorValue.Notify)
            {
                MessageBox.Show($"Characteristic: {Utility.GetUUIDString(chara.Uuid)} Not Support Method ValueChanged", Constant.APP_NAME);
            }
        }
    }
}
