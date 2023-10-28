using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace SuperCarter.Model
{
    public static class LoadDataService<T>
    {

        public static List<T> ReadFile(string _path)
        {

            if (System.IO.File.Exists(_path))
            {
                string jsonString = System.IO.File.ReadAllText(_path);
                List<T> userData = JsonConvert.DeserializeObject<List<T>>(jsonString);
                return userData;
            }
            else
            {
                MessageBox.Show("Cannt find " + _path);
                return new List<T>() { };
            }

        }

        public static void Writefile(string _path)
        {
            using (System.IO.StreamWriter filestream = new System.IO.StreamWriter(_path))
            {
                List<T> lstStuModel = new List<T>();

                string JSONStreamdata = JsonConvert.SerializeObject(lstStuModel, Newtonsoft.Json.Formatting.Indented);
                filestream.Write(JSONStreamdata);

            }
        }


    }
}
