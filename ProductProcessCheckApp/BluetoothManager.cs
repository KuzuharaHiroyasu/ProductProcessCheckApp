using System;

namespace ProductProcessCheckApp
{
    class BluetoothManager
    {
        public static readonly byte CommandStatusChange       = 0xB0; //状態変更コマンド ([Start]ボタンクリック)
        public static readonly byte CommandDetectBattery      = 0xA0; //充電検査
        public static readonly byte CommandDetectLED          = 0xA1; //LED検査
        public static readonly byte CommandDetectVibration    = 0xA2; //バイブレーション検査
        public static readonly byte CommandDetectMike         = 0xA3; //マイク検査
        public static readonly byte CommandDetectAcceleSensor = 0xA4; //加速度センサー検査
        public static readonly byte CommandDetectWearSensor   = 0xA5; //装着センサー検査
        public static readonly byte CommandDetectEEPROM       = 0xA6; //EEPROM
        public static readonly byte CommandSendBreadVolume    = 0xA7; //呼吸音送信
        public static readonly byte CommandSendWearSensor     = 0xA8; //装着センサー値送信
        public static readonly byte CommandSendPowerSWOff     = 0xF0; //電源SW OFF送信

        //UUID：Declaration
        public static readonly String UUID_SERVICE_DECLARATION = "d68c0001-a21b-11e5-8cb8-0002a5d5c51b"; //characteristic: Indicate
        public static readonly String UUID_READ_DECLARATION    = "d68c0002-a21b-11e5-8cb8-0002a5d5c51b"; //characteristic: Indicate（スマホ←機器）
        public static readonly String UUID_WRITE_DECLARATION   = "d68c0003-a21b-11e5-8cb8-0002a5d5c51b"; //characteristic: Write（スマホ→機器）

        public static readonly String UUID_SERVICE_FWUP            = "01010000-0000-0000-0000-000000000080"; //FWアップデート用サービスUUID
        public static readonly String UUID_CHAR_FWUP_WRITE_CONTROL = "02010000-0000-0000-0000-000000000080"; //FWアップデートWrite Characteristic Declaration (制御コマンド通信用キャラクタリスティックUUID)
        public static readonly String UUID_CHAR_FWUP_WRITE_DATA    = "03010000-0000-0000-0000-000000000080"; //FWアップデートWrite Characteristic Declaration (FW更新データ通信用キャラクタリスティックUUID)
        public static readonly String ANDROID_CENTRAL_UUID         = "00001800-0000-1000-8000-00805f9b34fb"; //固定値 (Android: 00002902, PC: 00001800)

        public static readonly int MAX_SEARCH_TIME = 40 * 1000; //40秒
    }
}
