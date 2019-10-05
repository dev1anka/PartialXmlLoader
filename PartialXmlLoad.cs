
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;


namespace Common.Xml
{
    public class PartialXmlLoad
    {
        public XmlImportNode importModel;
        public bool waitingChildsLoad;

        private bool _isReady;
        public LocalXmlLoader loader;
        public List<PartialXmlLoad> childLoaders;
        private FileInfo fileInfo;
        private XmlNode parentNode;
        private string path;

        private int deep;
        private XmlDocument xmlDoc;
        public XmlNode partialNode;
        public string initialText;

        private string rootPath;

        public Action onReady;


        private string _name;

        public static string xmlExtension = ".xml"; 
        public string name
        {

            get
            {

                return _name;
            }
            set
            {
                _name = value;
            }
        }


        //public bool done => throw new NotImplementedException();

        //public string name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public PartialXmlLoad(string path) : this(path, 0)
        {

        }
        public PartialXmlLoad(string path, int deep, string _rootPath = "", XmlNode parentNode = null, XmlImportNode _importNode = null)
        {
            fileInfo = new FileInfo(path);
            this._name = fileInfo.Name;
            this.importModel = _importNode;
            if (deep == 0)
            {
                this.rootPath = fileInfo.DirectoryName;
            }
            else
            {
                this.rootPath = _rootPath;
            }
            _isReady = false;
            this.parentNode = parentNode;
            this.path = path;

            this.deep = deep;


            loader = new LocalXmlLoader();
            waitingChildsLoad = false;


        }
        public void Load()
        {
            loader.Load(this.path);
            XmlReadyAction();

        }

        private void XmlReadyAction()
        {
            var data = loader.GetData();
            //Console.WriteLine(path);
            //Console.WriteLine(data );
            StringReader stringReader = new StringReader(data);
            this.initialText = data;
            xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(stringReader);
            }
            catch (Exception exc)
            {
                Console.WriteLine("exception :   : " + path + " : " + exc.Message);

            }


            partialNode = xmlDoc.LastChild;//SelectSingleNode("/root");

            //  Console.WriteLine("xml parse edildil");
            if (parentNode != null)
            {
                var refNode = parentNode.SelectSingleNode("//reference[@key='" + importModel.key + "']");
                if (refNode == null)
                {
                    Console.WriteLine(importModel.key + " e ait ref no bulunamadı");
                }
                var refParent = refNode.ParentNode;
                var subXml = parentNode.OwnerDocument.ImportNode(partialNode, true);
                refParent.InsertAfter(subXml, refNode);
                refParent.RemoveChild(refNode);
                refNode.RemoveAll();
                this.partialNode = subXml;

            }







            SetXmlImports();
        }
        private void SetXmlImports()
        {
            var imports = partialNode.SelectSingleNode("imports");
            if (imports != null && imports.HasChildNodes)
            {

                childLoaders = new List<PartialXmlLoad>();

                var nodeList = imports.SelectNodes("import");
                var len = nodeList.Count;

                for (int i = 0; i < len; i++)
                {
                    var importNod = new XmlImportNode(nodeList[i]);
                    var childLoader = new PartialXmlLoad(rootPath + "/" + importNod.name+xmlExtension, this.deep + 1, this.rootPath, this.partialNode, importNod);
                    childLoaders.Add(childLoader);

                    childLoader.Load();
                }

                OnChildsReady();
                _isReady = true;

            }
            else
            {
                _isReady = true;
                waitingChildsLoad = false;
            }
        }

        private void OnChildsReady()
        {
            if (onReady != null)
            {
                onReady();
            }
        }
        private void OnQueCompleted()
        {
            Console.WriteLine("que completed !é!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            OnChildsReady();
            waitingChildsLoad = false;
        }
        public XmlNode GetChildXmlNode(string childName)
        {
            var childLoader = this.childLoaders.SingleOrDefault(x => x.importModel.key == childName);
            if (childLoader == null)
            {
                Console.WriteLine(childName + " isminde child loader bulunamadı");
            }
            var childNode = childLoader.partialNode;
            return childNode;
        }
        public bool IsChildsReady()
        {
            //throw new NotImplementedException();


            //return false;

            foreach (var child in this.childLoaders)
            {
                var childRes = child.IsChildsReady();
                if (!childRes)
                {
                    return false;
                }
            }
            return true;
        }
        public void Destroy()
        {
            if (childLoaders != null)
            {
                childLoaders.ForEach(x => { x.Destroy(); });
                childLoaders.Clear();
                childLoaders = null;
            }
            loader.Destroy();
            xmlDoc.RemoveAll();
            xmlDoc = null;
            partialNode = null;
        }

        public void Start()
        {
            this.Load();
        }

        public bool done
        {
            get
            {

                return true;
            }
        }
        public void IteareAction(Action<PartialXmlLoad> func)
        {
            func(this);
            if (this.childLoaders != null)
            {
                childLoaders.ForEach(x => { x.IteareAction(func); });
            }
        }

    }
}
