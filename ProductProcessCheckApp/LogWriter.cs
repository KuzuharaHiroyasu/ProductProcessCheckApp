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
        private StreamWriter sw;

        public void logFileCreate(string filePath, string fileName)
        {
            sw = File.CreateText(filePath + fileName);
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

        public void scanLogWrite(string time, string address, string result, string num)
        {
            if (sw != null)
            {
                sw.WriteLine(time + " [" + address + "] " + result + num);
            }
        }

        public void resultLogWrite(string scanPlanNum, string scanNum, string ret_ok, string ret_ng)
        {
            if (sw != null)
            {
                sw.WriteLine("検査予定数：" + scanPlanNum + " 検査数：" + scanNum + " OK：" + ret_ok + " NG：" + ret_ng);
            }
        }
    }
}
