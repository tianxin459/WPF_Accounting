using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Permissions;
using System.Windows;
using System.Security;
using Accounting.Model;
using Newtonsoft.Json;

namespace Accounting.Util
{
    public class DataStorage
    {
        public static string FolderName = "Data";
        public static string FileName = "Data.data";
        public static string DataFile { get { return FolderName + "\\" + FileName; } }

        public DataStorage() {
            //log4net.Config.XmlConfigurator.Configure();
            //log4net.ILog log = log4net.LogManager.GetLogger("AppLogger2");
            //log.Error("test");
        }


        public void SaveData(string text)
        {
            try
            {
                EnsureDataPath();
                //var file = new FileInfo(DataFile);
                var fs = File.OpenWrite(DataFile);
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                fs.Write(buffer, 0, buffer.Length);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public string ReadData()
        {
            try
            {
                EnsureDataPath();
                var text = File.ReadAllText(DataFile);
                return text;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public static void EnsureDataPath()
        {
            if (!Directory.Exists(FolderName))
            {
                Directory.CreateDirectory(FolderName);
            }
            if (!File.Exists(DataFile))
            {
                File.Create(DataFile);
            }
        }



        public static List<Member> LoadData()
        {
            try
            {
                EnsureDataPath();
                var jsonData = File.ReadAllText(DataFile);
                List<Member> data = JsonConvert.DeserializeObject<List<Member>>(jsonData);
                return data;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public static List<Member> ReadData(string path)
        {
            try
            {
                var jsonData = File.ReadAllText(path);

                List<Member> data = JsonConvert.DeserializeObject<List<Member>>(jsonData);
                return data;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public static void SaveData(List<Member> data)
        {
            try
            {
                EnsureDataPath();

                //clear file
                FileStream fsTruncate = new FileStream(DataFile, FileMode.Truncate, FileAccess.ReadWrite);
                fsTruncate.Close();

                var jsonData = JsonConvert.SerializeObject(data);
                var fs = File.OpenWrite(DataFile);
                byte[] buffer = Encoding.UTF8.GetBytes(jsonData);
                fs.Write(buffer, 0, buffer.Length);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
