using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Xml
{
    public class LocalXmlLoader 
    {
        private string path;

        public bool done
        {
            get { return true; }
        }

        public string name
        {
            get
            {
                return "";
            }
            set
            {

            }
        }

        public string GetData()
        {
            return File.ReadAllText(path);
        }

        public void Load(string path)
        {
            this.path = path;
        }
        public void Destroy()
        {

        }

        public void Start()
        {
            
        }
    }
}
