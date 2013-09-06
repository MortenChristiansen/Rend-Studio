using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Studio.Core.Xml;

namespace Studio.Assets.Project
{
    public class ProjectAsset : Asset
    {
        internal override string Extension
        {
            get { return "rdproj"; }
        }

        internal override string FileDescription
        {
            get { return "Rend project"; }
        }

        protected override string SerializationName
        {
            get { return "Project"; }
        }

        public IEnumerable<SceneAsset> Scenes
        {
            get { return SubAssets.Where(a => a is SceneAsset).Cast<SceneAsset>(); }
        }

        public void AddScene(SceneAsset scene)
        {
            AddSubAsset(scene);
            if (!scene.HasSaveFile)
                scene.Save();
        }

        protected override string GenerateDefaultTitle()
        {
            return "New Project";
        }

        internal override XmlDocument SerializeAsXml()
        {
            var root = CreateBaseXmlDocument();
            var xml = root.OwnerDocument;

            root.SetAttribute("Title", Title);

            AppendSubAssetReferences(root);

            return xml;
        }

        internal override void DeserializeFromXml(XmlNode xml)
        {
            if (xml.Name != SerializationName)
                throw new ArgumentException();

            dynamic xmlObject = new DynamicXmlParser(xml);
            Title = xmlObject["Title"];

            DeserializeReferencedSubAssets(xmlObject.SubAssets);
        }
    }
}
