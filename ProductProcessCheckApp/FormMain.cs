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

namespace ProductProcessCheckApp
{
    public partial class FormMain : Form
    {
        public BluetoothLEAdvertisementWatcher watcher;
        public DeviceWatcher deviceWatcher;
        public GattDeviceService gattService;
        public GattCharacteristic readCharacteristic, writeCharacteristic;

        public List<GattCharacteristic> characteristicList = new List<GattCharacteristic>();

        public Dictionary<string, BluetoothLEDevice> deviceList = new Dictionary<string, BluetoothLEDevice>();

        private Timer connectTimer = new Timer();
        private Boolean startedFlag = false;
        private DeviceStatus deviceStatus = DeviceStatus.NOT_CONNECT;

        private IniFile ini;

        public FormMain()
        {
            InitializeComponent();

            CustomGUI();

            LoadIniFile();
        }

        private async　void btnConnect_Click(object sender, EventArgs e)
        {
            if(deviceStatus == DeviceStatus.NOT_CONNECT || deviceStatus == DeviceStatus.CONNECT_FAILED) //接続処理
            {
                DisconnectDevice();

                SetupSearchTimeOut();
                SetupBluetooth();

                lblStatus.Text = Constant.MSG_SLEEIM_IS_SEARCHING;
            } else if (deviceStatus == DeviceStatus.CONNECT_SUCCESS) //状態変更コマンド(0xB0)送信
            {
                sendCommandStatusChange();
            } else if (deviceStatus == DeviceStatus.STARTED) 
            {
                MessageBox.Show("未対応", Constant.APP_NAME);
            } else if (deviceStatus == DeviceStatus.OK_MANUAL_DID)
            {
                MessageBox.Show("未対応", Constant.APP_NAME);
            } else if (deviceStatus == DeviceStatus.AUTO_CHECKED)
            {
                MessageBox.Show("未対応", Constant.APP_NAME);
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

            DisconnectDevice();
            
            MessageBox.Show(message, Constant.APP_NAME);
        }

        private async void btnNG_Click(object sender, EventArgs e)
        {
            MessageBox.Show("未対応", Constant.APP_NAME);
        }

        private void UpdateDeviceStatus(DeviceStatus status, String statusMessage = "", bool showDialog = true)
        {
            deviceStatus = status;
            if(status == DeviceStatus.CONNECT_SUCCESS)
            {
                btnConnect.Text = "START";
            } else if (status == DeviceStatus.STARTED)
            {
                btnConnect.Text = "OK(手動)";
            } else if (status == DeviceStatus.OK_MANUAL_DID)
            {
                btnConnect.Text = "自動検査";
            } else
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

        private void DisconnectDevice()
        {
            if (gattService != null)
            {
                gattService.Dispose();
            }
            gattService = null;
            readCharacteristic = null;
            writeCharacteristic = null;
            deviceList = new Dictionary<string, BluetoothLEDevice>();

            lblAddress.Text = "BDアドレス[-:-:-:-:-:-]";
            UpdateDeviceStatus(DeviceStatus.NOT_CONNECT);
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
            this.lblCurrentDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

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
                var isConnected = await ConnectDevice(device);
                if (isConnected)
                {
                    StopScanning();

                    UpdateDeviceStatus(DeviceStatus.CONNECT_SUCCESS, "Sleeim[" + address + "]を成功に接続しました", false);
                    lblAddress.Text = "BDアドレス[" + address + "]";
                }
                else
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
            DevicePairingResult result = await device.DeviceInformation.Pairing.PairAsync(DevicePairingProtectionLevel.None);
            if (result != null)
            {
                Debug.WriteLine($"Pairing Result: {result.Status}");
                if (result.Status == DevicePairingResultStatus.Paired || result.Status == DevicePairingResultStatus.AlreadyPaired)
                {
                    Debug.WriteLine("Paired OK");
                    return true;
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
                    Debug.WriteLine($"Service: UUID {service.Uuid}, ShareMode {service.SharingMode}, DeviceId {service.DeviceId}");

                    var charactersResult = await service.GetCharacteristicsAsync();
                    foreach (var chara in charactersResult.Characteristics)
                    {
                        characteristicList.Add(chara);
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
                            readCharacteristic.ValueChanged += ReadCharacteristic_ValueChanged;
                        }

                        if (writeCharacteristic != null)
                        {
                            Console.WriteLine($"Write Characteristic Selected: {Utility.GetUUIDString(writeCharacteristic.Uuid)}");
                            writeCharacteristic.ValueChanged += WriteCharacteristic_ValueChanged;

                            return true;
                        }
                    }
                }

                //var gattServiceResult = await device.GetGattServicesForUuidAsync(new Guid(Constant.UUID_SERVICE));
                //this.gattService = gattServiceResult.Services.FirstOrDefault();
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

        public async Task<int> WriteCommandToDevice(byte[] commandData)
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
                        MessageBox.Show("Command Write Failed (Unreachable)");
                        Debug.WriteLine("Command Write Failed (Unreachable)");
                        return 1;
                    }
                    else if (result == GattCommunicationStatus.ProtocolError)
                    {
                        MessageBox.Show("Command Write Failed (ProtocolError)");
                        Debug.WriteLine("Command Write Failed (ProtocolError)");
                        return 2;
                    }
                    else if (result == GattCommunicationStatus.Success)
                    {
                        //readCharacteristic.ValueChanged += ReadCharacteristic_ValueChanged;
                        MessageBox.Show("Command Write Successfully");
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Command Write Failed. Exception: " + ex.Message);
                    Debug.WriteLine("Write, Exception: " + ex.Message);
                }
            }
            else
            {
                UpdateDeviceStatus(DeviceStatus.NOT_CONNECT, Constant.MSG_SLEEIM_IS_NOT_CONNECTING);
            }

            return -1;
        }

        public async Task ReadValue()
        {
            GattReadResult result = await readCharacteristic.ReadValueAsync(BluetoothCacheMode.Uncached);
            if (result.Status == GattCommunicationStatus.Success)
            {
                var RawBufLen = (int)result.Value.Length;
                byte[] data;
                string strData = null;

                CryptographicBuffer.CopyToByteArray(result.Value, out data);
                //byte[] bytes = WindowsRuntimeBufferExtensions.ToArray(result.Value, 0, (int)result.Value.Length);

                for (int i = 0; i < data.Length; i++)
                {
                    if (Convert.ToChar(data[i]) != '\0')
                    {
                        strData = strData + Convert.ToChar(data[i]);
                    }
                }

                Console.WriteLine("Data Received : " + strData);
            }
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
        }

        private static void ReadCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            Debug.WriteLine("ReadCharacteristic_ValueChanged Here");
        }

        private async void DeviceAdded(DeviceWatcher watcher, DeviceInformation device)
        {
            /* Debug.WriteLine($"Device added");
            try
            {
                var service = await GattDeviceService.FromIdAsync(device.Id);
                //Debug.WriteLine("Opened Service!!");
            }
            catch
            {
                //Debug.WriteLine("Failed to open service.");
            }*/
        }

        private void DeviceUpdated(DeviceWatcher watcher, DeviceInformationUpdate update)
        {
            Debug.WriteLine($"Device updated: {update.Id}");
        }

        private async void sendCommandStatusChange()
        {
            if (writeCharacteristic != null)
            {
                DialogResult res = MessageBox.Show("生産工程検査に状態変更コマンド[0xB0]を実行しますか？", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (res == DialogResult.OK)
                {
                    lblStatus.Text = "状態変更コマンド(0xB0)を送信中...";

                    byte commandCode = Constant.CommandStatusChange;
                    //状態 (0:待機状態, 3:GET状態, 4:SET状態(デバッグ機能), 5:プログラム更新状態(G1D), 6:生産工程検査)
                    int commandStatus = 6;

                    byte[] tmp = new byte[] { commandCode, (byte)commandStatus };
                    await WriteCommandToDevice(tmp);
                }
                else if (res == DialogResult.Cancel)
                {

                }
            }
            else
            {
                UpdateDeviceStatus(DeviceStatus.NOT_CONNECT, Constant.MSG_SLEEIM_IS_NOT_CONNECTING);
            }
        }
    }
}
