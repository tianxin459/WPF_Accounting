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
        public static string AppFileName = "App.init";
        public static string DataFile { get { return FolderName + "\\" + FileName; } }

        public DataStorage() {
            //log4net.Config.XmlConfigurator.Configure();
            //log4net.ILog log = log4net.LogManager.GetLogger("AppLogger2");
            //log.Error("test");
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

        public static string LoadAppData(string path)
        {
            try
            {
                var text = File.ReadAllText(path);
                return text;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static List<Member> LoadData()
        {
            try
            {
                EnsureDataPath();
                var encryptedData = File.ReadAllBytes(DataFile);
                var jsonData = EncryptionUtil.AESDecryptToString(encryptedData);
                //var jsonData = File.ReadAllText(DataFile);
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
                var encryptedData = EncryptionUtil.AESEncrypt(jsonData);
                var fs = File.OpenWrite(DataFile);
                fs.Write(encryptedData, 0, encryptedData.Length);
                //byte[] buffer = Encoding.UTF8.GetBytes(jsonData);
                //fs.Write(buffer, 0, buffer.Length);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //config file export
        public static void ExportText(string path, string text)
        {
            try
            {
                FileStream fs;
                if (File.Exists(path))
                {
                    FileStream fsTruncate = new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite);
                    fsTruncate.Close();
                    fs = File.OpenWrite(path);
                }
                else
                {
                    fs = File.Create(path);
                }
                
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                fs.Write(buffer, 0, buffer.Length);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //restore data
        public static List<Member> ReadData(string path)
        {
            try
            {
                //var jsonData = File.ReadAllText(path);

                var encryptedData = File.ReadAllBytes(path);
                var jsonData = EncryptionUtil.AESDecryptToString(encryptedData);

                List<Member> data = JsonConvert.DeserializeObject<List<Member>>(jsonData);
                return data;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        //export file
        public static void ExportData(string path,List<Member> data)
        {
            try
            {
                FileStream fs;
                if(File.Exists(path))
                {
                    FileStream fsTruncate = new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite);
                    fsTruncate.Close();
                    fs = File.OpenWrite(path);
                }
                else {
                    fs = File.Create(path);
                }

                //var jsonData = JsonConvert.SerializeObject(data);
                //byte[] buffer = Encoding.UTF8.GetBytes(jsonData);
                //fs.Write(buffer, 0, buffer.Length);

                var jsonData = JsonConvert.SerializeObject(data);
                var encryptedData = EncryptionUtil.AESEncrypt(jsonData);
                fs.Write(encryptedData, 0, encryptedData.Length);

                fs.Flush();
                fs.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void ExportPrintFile(string path, string text)
        {
            try
            {
                FileStream fs;
                if (File.Exists(path))
                {
                    FileStream fsTruncate = new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite);
                    fsTruncate.Close();
                    fs = File.OpenWrite(path);
                }
                else
                {
                    fs = File.Create(path);
                }

                //var jsonData = JsonConvert.SerializeObject(data);
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                fs.Write(buffer, 0, buffer.Length);
                fs.Flush();
                fs.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally {

            }
        }
    }
}
