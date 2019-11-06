using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace TalUtils
{
    public class SerializeHelper
    {
        #region For future use

        private Type _type;
        private string _defaultFile;

        public SerializeHelper(Type type, string defaultFile = "")
        {
            _type = type;
            _defaultFile = defaultFile;
        }

        #endregion
        

        public static bool Save(object data, string strFile)
        {
            if (data != null)
            {
                try
                {
                    FileStream fs = null;
                    XmlSerializer xs = new XmlSerializer(data.GetType());
                    fs = new FileStream(strFile, FileMode.Create, FileAccess.Write);
                    xs.Serialize(fs, data);
                    fs.Close();
                    fs = null;

                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                Exception ex = new Exception("Data object is not initialized");
                ex.Source = "SerializeHelper.Save()";
                throw ex;
            }
        }


        ///WebClient client = new WebClient();
        //Stream stream = client.OpenRead("https://github.com/tbener/PicPick/raw/picpick/PicPickWpf/Versioning/PicPickVersionInfo.xml");
        //StreamReader reader = new StreamReader(stream);
        //String content = reader.ReadToEnd();
        /// 
        ///


        public static object LoadFromWeb(Type type, string strFile)
        {

            FileStream fs = null;
            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead("https://github.com/tbener/PicPick/raw/picpick/PicPickWpf/Versioning/PicPickVersionInfo.xml");
                //fs = new FileStream(strFile, FileMode.Open, FileAccess.Read);
                XmlSerializer xs = new XmlSerializer(type);
                return xs.Deserialize(stream);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                try
                {
                    fs.Close();
                }
                catch { }
                finally { fs = null; }
            }
        }

        public static object Load(Type type, string strFile)
        {

            FileStream fs = null;
            try
            {
                fs = new FileStream(strFile, FileMode.Open, FileAccess.Read);
                XmlSerializer xs = new XmlSerializer(type);
                return xs.Deserialize(fs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                try
                {
                    fs.Close();
                }
                catch { }
                finally { fs = null; }
            }
        }

    }
}
