using System;

namespace ProductProcessCheckApp
{
    class Constant
    {
        public static readonly byte CommandStatusChange       = 0xB0; //状態変更コマンド ([Start]ボタンクリック)
        public static readonly byte CommandDetectBattery      = 0xA0; //充電検査
        public static readonly byte CommandDetectLED          = 0xA1; //LED検査
        public static readonly byte CommandDetectVibration    = 0xA2; //バイブレーション検査
        public static readonly byte CommandDetectMike         = 0xA3; //マイク検査
        public static readonly byte CommandDetectAcceleSensor = 0xA4; //加速度センサー検査
        public static readonly byte CommandDetectWearSensor   = 0xA5; //装着センサー検査
        public static readonly byte CommandDetectEEPROM       = 0xA6; //EEPROM
        public static readonly byte CommandReceiveBreathVolume= 0xA7; //呼吸音送信 (G1D→PC)
        public static readonly byte CommandReceiveAcceleSensor= 0xA8; //加速度センサー値送信 (G1D→PC)
        public static readonly byte CommandReceiveWearSensor  = 0xA9; //装着センサー値送信 (G1D→PC)
        public static readonly byte CommandSendPowerSWOff     = 0xF0; //電源SW OFF送信

        //Service：Declaration
        public static readonly String UUID_SERVICE_DECLARATION     = "d68c0001-a21b-11e5-8cb8-0002a5d5c51b"; //Service: Indicate
        public static readonly String UUID_CHAR_READ_DECLARATION   = "d68c0002-a21b-11e5-8cb8-0002a5d5c51b"; //Permission: Indicate（スマホ←機器）
        public static readonly String UUID_CHAR_WRITE_DECLARATION  = "d68c0003-a21b-11e5-8cb8-0002a5d5c51b"; //Permission: Write（スマホ→機器）

        public static readonly String UUID_SERVICE_FWUP            = "01010000-0000-0000-0000-000000000080"; //Service: FWアップデート用
        public static readonly String UUID_CHAR_FWUP_WRITE_CONTROL = "02010000-0000-0000-0000-000000000080"; //Permission:Write(FWアップデートWrite, 制御コマンド通信用)
        public static readonly String UUID_CHAR_FWUP_WRITE_DATA    = "03010000-0000-0000-0000-000000000080"; //Permission:WriteWithoutResponse (FWアップデートWrite, FW更新データ通信用)

        public static readonly String UUID_SERVICE_CENTRAL         = "00001800-0000-1000-8000-00805f9b34fb"; //固定値 (PC: 00001800, Android: 00002902)
        public static readonly String UUID_CHAR_READ1              = "00002a04-0000-1000-8000-00805f9b34fb"; //Permission: Read
        public static readonly String UUID_CHAR_READ_WRITE1        = "00002a00-0000-1000-8000-00805f9b34fb"; //Permission: Read, Write
        public static readonly String UUID_CHAR_READ_WRITE2        = "00002a01-0000-1000-8000-00805f9b34fb"; //Permission: Read, Write
        public static readonly String ANDROID_CENTRAL_UUID         = "00002902-0000-1000-8000-00805f9b34fb";


        public static String UUID_SERVICE    = UUID_SERVICE_DECLARATION;
        public static String UUID_CHAR_READ  = UUID_CHAR_READ_DECLARATION;
        public static String UUID_CHAR_WRITE = UUID_CHAR_WRITE_DECLARATION; //Used to send command

        //public static String UUID_SERVICE    = UUID_SERVICE_FWUP;
        //public static String UUID_CHAR_READ  = UUID_CHAR_FWUP_WRITE_CONTROL;
        //public static String UUID_CHAR_WRITE = UUID_CHAR_FWUP_WRITE_DATA; //Used to send command

        //public static String UUID_SERVICE    = UUID_SERVICE_CENTRAL;
        //public static String UUID_CHAR_READ  = UUID_CHAR_READ1;
        //public static String UUID_CHAR_WRITE = UUID_CHAR_READ_WRITE1; //OR UUID_CHAR_READ_WRITE2  //Used to send command


        public static readonly int MAX_SEARCH_TIME = 30 * 1000; //40秒

        public static string APP_NAME                     = "生産工程検査ソフト";
        public static string MSG_SLEEIM_NOT_FOUND         = "Sleeimが見つかりませんでした";
        public static string MSG_SLEEIM_IS_CONNECTING     = "";
        public static string MSG_SLEEIM_IS_NOT_CONNECTING = "Sleeimのデバイスを接続していません";
        public static string MSG_SLEEIM_DISCONNECTED      = "Sleeimを切断完了しました";
        public static string MSG_SLEEIM_CONNECT_FAILED    = "Sleeimが接続できませんでした";
        public static string MSG_SLEEIM_IS_SEARCHING      = "Sleeimを検索しています...";
    }

    public enum DeviceStatus
    {
        NOT_CONNECT                = 0,  //[未接続]ステータス
        CONNECT_FAILED             = 1,  //[接続失敗]ステータス
        CONNECT_SUCCESS            = 2,  //[成功に接続後]ステータス
        STATUS_CHANGE_OK           = 3,  //[状態変更コマンド(0xB0)送信後]ステータス
        DETECT_BATTERY_OK          = 4,  //[充電検査開始コマンド送信後]ステータス
        DETECT_LED_OK              = 5,  //[LED検査開始コマンド送信後]ステータス
        DETECT_VIBRATION_OK        = 6,  //[バイブレーション検査開始コマンド送信後]ステータス
        DETECT_VIBRATION_FINISH_OK = 7,  //[バイブレーション検査終了コマンド送信後]ステータス
        DETECT_MIKE_OK             = 8,  //[マイク検査開始コマンド送信後]ステータス
        RECEIVE_BREATH_VOLUME_OK   = 9,  //デバイスから[呼吸音送信コマンド受信後]ステータス
        DETECT_MIKE_FINISH_OK      = 10, //[マイク検査終了コマンド送信後]ステータス
        DETECT_ACCELE_SENSOR_OK        = 11, //[加速度センサー検査開始コマンド送信後]ステータス
        RECEIVE_ACCELE_SENSOR_OK       = 12, //デバイスから[加速度センサー値送信コマンド受信後]ステータス
        DETECT_ACCELE_SENSOR_FINISH_OK = 13, //[加速度センサー検査終了コマンド送信後]ステータス
        DETECT_WEAR_SENSOR_OK          = 14, //[装着センサー検査開始コマンド送信後]ステータス
        RECEIVE_WEAR_SENSOR_OK         = 15, //デバイスから[装着センサー値送信コマンド受信後]ステータス
        DETECT_WEAR_SENSOR_FINISH_OK   = 16, //[装着センサー検査終了コマンド送信後]ステータス
        DETECT_EEPROM_OK               = 17, //[EEPROM検査開始コマンド送信後]ステータス
        SEND_POWER_OFF_OK              = 18, //["電源SW OFF送信コマンド送信後]ステータス
    }

    public enum CommandResult
    {
        SUCCESS = 0,
        UNREACHABLE = 1,
        PROTOCOL_ERROR = 2,
        NOT_CONNECT = 3,
        UNKNOW_ERROR = -1
    }    

    public enum CommandFlag
    {
        START = 0, //0：開始
        FINISH = 1 //1：終了
    }

    public enum CommandReturn
    {
        SUCCESS = 0, //0：OK(成功)
        NOT_GOOD = 1   //1：NG(失敗)
    }
}
