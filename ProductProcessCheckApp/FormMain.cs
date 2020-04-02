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

namespace ProductProcessCheckApp
{
    public partial class FormMain : Form
    {
        public BluetoothLEAdvertisementWatcher watcher;
        public DeviceWatcher deviceWatcher;
        public GattDeviceService gattService;
        public GattCharacteristic readCharacteristic, writeCharacteristic;

        //private String serviceUUID = BluetoothManager.UUID_SERVICE_DECLARATION; //サービスUUID
        public Dictionary<string, BluetoothLEDevice> deviceList = new Dictionary<string, BluetoothLEDevice>();

        private Timer connectTimer = new Timer();
        private Boolean startedFlag = false;

        private String appName = "生産工程検査ソフト";

        private IniFile ini;

        public FormMain()
        {
            InitializeComponent();

            LoadIniFile();

            this.Text = appName;
            this.lblCurrentDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            DisconnectDevice();

            SetupSearchTimeOut();
            SetupBluetooth();

            lblStatus.Text = "Sleeimを検索しています...";
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (watcher != null && deviceWatcher != null)
            {
                StopScanning();
            }

            var message = gattService == null ? "Sleeimのデバイスを接続していません" : "Sleeimを切断完了しました";
            lblStatus.Text = message;

            DisconnectDevice();
            
            MessageBox.Show(message, appName);
        }

        private void btnNG_Click(object sender, EventArgs e)
        {

        }

        private async void sendData_Click(object sender, EventArgs e)
        {
            if (writeCharacteristic != null)
            {
                DialogResult res = MessageBox.Show("生産工程検査に状態変更コマンド[0xB0]を実行しますか？", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (res == DialogResult.OK)
                {
                    byte commandCode = 0xC7;
                    //状態 (0:待機状態, 3:GET状態, 4:SET状態(デバッグ機能), 5:プログラム更新状態(G1D), 6:生産工程検査)
                    int commandStatus = 2;

                    byte[] tmp = new byte[] { commandCode, (byte)commandStatus };
                    await WriteCommandToDevice(tmp);
                }
                else if (res == DialogResult.Cancel)
                {
                    //MessageBox.Show("You have clicked Cancel Button");
                }
            }
            else
            {
                var message = "Sleeimのデバイスを接続していません";
                lblStatus.Text = message;
                MessageBox.Show(message, appName);
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

                    message = "Sleeim[" + address + "]を成功に接続しました";
                    lblStatus.Text = message;
                    lblAddress.Text = "BDアドレス[" + address + "]";
                    MessageBox.Show(message, appName);
                }
                else
                {
                    message = "Sleeim[" + address + "]を失敗に接続しました";
                    lblStatus.Text = message;
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
            foreach (var tmp in gatt.Services)
            {
                Debug.WriteLine($"Uuid {tmp.Uuid}, DeviceId {tmp.DeviceId}");
            }

            if (gatt.Status == GattCommunicationStatus.Success)
            {
                //this.gattService = device.GetGattService(new Guid(BluetoothManager.UUID_SERVICE_DECLARATION));
                var gattServiceResult = await device.GetGattServicesForUuidAsync(new Guid(BluetoothManager.UUID_SERVICE_DECLARATION));
                this.gattService = gattServiceResult.Services.FirstOrDefault();

                if (this.gattService != null)
                {
                    GattCharacteristicsResult readCharacteristicResult = await gattService.GetCharacteristicsForUuidAsync(new Guid(BluetoothManager.UUID_READ_DECLARATION));
                    GattCharacteristicsResult writeCharacteristicResult = await gattService.GetCharacteristicsForUuidAsync(new Guid(BluetoothManager.UUID_WRITE_DECLARATION));

                    readCharacteristic = readCharacteristicResult.Characteristics.FirstOrDefault(servchar => Utility.GetUUIDString(servchar.Uuid) == BluetoothManager.UUID_READ_DECLARATION);
                    writeCharacteristic = writeCharacteristicResult.Characteristics.FirstOrDefault(servchar => Utility.GetUUIDString(servchar.Uuid) == BluetoothManager.UUID_WRITE_DECLARATION);

                    if (readCharacteristic != null)
                    {
                        Console.WriteLine($"Read Characteristic Selected: {Utility.GetUUIDString(readCharacteristic.Uuid)}");
                        readCharacteristic.ValueChanged += ReadCharacteristic_ValueChanged;

                        /*
                        var charResult = await readCharacteristic.WriteClientCharacteristicConfigurationDescriptorWithResultAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);        
                        if (readCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read)) {
                            //This characteristic supports reading from it
                        }
                        if (readCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Write)) {
                            //This characteristic supports writing to it
                        }
                        if (readCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify)) {
                            //This characteristic supports subscribing to notifications
                        }*/
                    }

                    if (writeCharacteristic != null)
                    {
                        Console.WriteLine($"Write Characteristic Selected: {Utility.GetUUIDString(writeCharacteristic.Uuid)}");
                        //writeCharacteristic.ValueChanged += WriteCharacteristic_ValueChanged;

                        return true;
                    }

                    /* var serviceAllCharacteristics = await gattService.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
                    if (serviceAllCharacteristics.Status == GattCommunicationStatus.Success)
                    {
                        readCharacteristic = serviceAllCharacteristics.Characteristics.FirstOrDefault(servchar => Utility.GetUUIDString(servchar.Uuid) == BluetoothManager.UUID_READ_DECLARATION);
                        writeCharacteristic = serviceAllCharacteristics.Characteristics.FirstOrDefault(servchar => Utility.GetUUIDString(servchar.Uuid) == BluetoothManager.UUID_WRITE_DECLARATION);
                    } */
                }
            }

            return false;
        }

        private void SetupSearchTimeOut()
        {
            startedFlag = true;
            connectTimer.Interval = BluetoothManager.MAX_SEARCH_TIME; //秒で計算
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

                var message = "Sleeimのデバイスが見つかりません";
                lblStatus.Text = message;
                MessageBox.Show(message, appName);
            }
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
                    GattCommunicationStatus result = await writeCharacteristic.WriteValueAsync(writeBuffer);

                    Console.WriteLine($"WriteCharacterValue result:{result}");
                    //await selectedReadCharacter.KeepRunnig(selectedWriteCharacter, sndbuf);
                    if (result == GattCommunicationStatus.Unreachable)
                    {
                        MessageBox.Show("Command Write Failed (Unreachable)");
                        return 1;
                    }
                    else if (result == GattCommunicationStatus.ProtocolError)
                    {
                        MessageBox.Show("Command Write Failed (ProtocolError)");
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
                var message = "Sleeimのデバイスを接続していません";
                lblStatus.Text = message;
                MessageBox.Show(message, appName);
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
                var gattCharacteristic = gattService.GetCharacteristics(new Guid(BluetoothManager.UUID_WRITE_DECLARATION)).First();
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

        private async void ConnectDeviceByDeviceInfo(DeviceInformation deviceInfo)
        {
            // Note: BluetoothLEDevice.FromIdAsync must be called from a UI thread because it may prompt for consent
            BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
        }
    }
}
