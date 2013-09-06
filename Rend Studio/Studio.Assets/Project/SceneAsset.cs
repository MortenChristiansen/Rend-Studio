using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Microsoft.Win32;
using Studio.Core.Xml;

namespace Studio.Assets.Project
{
    public class SceneAsset : Asset
    {
        private static int _newSceneIndex = 1;
        private static HashSet<SceneAsset> _openSceneAssets = new HashSet<SceneAsset>();

        internal override string Extension { get { return "rnd"; } }
        internal override string FileDescription { get { return "Rend scene"; } }
        protected override string SerializationName { get { return "Scene"; } }
        internal override bool CanGenerateFileName { get { return ParentAsset != null; } }

        public string Code { get; set; }
        public IEnumerable<RenderingAsset> Renderings
        {
            get { return SubAssets.Where(a => a is RenderingAsset).Cast<RenderingAsset>(); }
        }

        public SceneAsset()
        {
            Code = string.Empty;

            if (!_openSceneAssets.Contains(this))
                _openSceneAssets.Add(this);
        }

        public void AddRendering(RenderingAsset rendering)
        {
            AddSubAsset(rendering);
        }

        protected override string GenerateDefaultTitle()
        {
            string title;
            do
            {
                title = "New scene " + _newSceneIndex++;
            } while (_openSceneAssets.Any(i => i.Title == title));

            return title;
        }

        internal override void Save(string filePath)
        {
            if (ParentAsset != null)
                EnsureSaveDirectoryExists("Scenes");

            base.Save(filePath);
        }

        internal override XmlDocument SerializeAsXml()
        {
            var root = CreateBaseXmlDocument();
            var xml = root.OwnerDocument;

            root.SetAttribute("Title", Title);

            var code = xml.CreateElement("Code");
            code.InnerText = Code;
            root.AppendChild(code);

            AppendSubAssets(root);

            return xml;
        }

        internal override void DeserializeFromXml(XmlNode xml)
        {
            if (xml.Name != SerializationName)
                throw new ArgumentException();

            dynamic xmlObject = new DynamicXmlParser(xml);
            Title = xmlObject["Title"];
            Code = xmlObject.Code;

            DeserializeSubAssets(xmlObject.SubAssets);
        }

        protected override string GenerateFilePath()
        {
            return GenerateRelativeFilePathInSubFolder("Scenes");
        }
    }
}
