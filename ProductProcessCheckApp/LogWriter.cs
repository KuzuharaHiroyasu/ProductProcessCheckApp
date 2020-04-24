using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductProcessCheckApp
{
    class LogWriter
    {
        private FileStream fs;
        private StreamWriter sw;

        public void logFileCreate(string filePath, string modelName)
        {
            if (filePath == "" || !Directory.Exists(filePath))
            {
                filePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            }

            filePath = System.IO.Path.GetDirectoryName(filePath);

            string fileName = modelName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";

            fs = File.Create(filePath + "\\" + fileName);
            sw = new StreamWriter(fs);
        }

        public void verLogWrite(string ver)
        {
            if (sw != null)
            {
                sw.WriteLine("ver:" + ver);
            }
        }

        public void serialLogWrite(string startNum, string endNum)
        {
            if (sw != null)
            {
                sw.WriteLine("SerialNo:" + startNum + " - " + endNum);
            }
        }

        public void scanLogWrite(string address, string result, string num)
        {
            if (sw != null)
            {
                string time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                sw.WriteLine(time + " [" + Utility.getFormatDeviceAddress(address) + "] " + result + " " + num);
            }
        }

        public void resultLogWrite(string scanPlanNum, string scanNum, string ret_ok, string ret_ng)
        {
            if (sw != null)
            {
                sw.WriteLine("検査予定数：" + scanPlanNum + " 検査数：" + scanNum + " OK：" + ret_ok + " NG：" + ret_ng);
                sw.Close();
                fs.Close();
            }
        }
    }
}
