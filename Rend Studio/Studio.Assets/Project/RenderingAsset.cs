using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Studio.Core.Xml;

namespace Studio.Assets.Project
{
    public class RenderingAsset : Asset
    {
        internal override string Extension { get { return "png"; } }
        internal override string FileDescription { get { return "Rendering"; } }
        internal override bool CanGenerateFileName { get { return ParentAsset != null; } }
        protected override string SerializationName { get { return "Rendering"; } }

        public Bitmap Image { get; private set; }

        /// <summary>
        /// Creates a new rendering asset for serialization/deserialization
        /// purposes.
        /// </summary>
        public RenderingAsset()
        {

        }

        public RenderingAsset(Bitmap image)
        {
            Image = image;
        }

        protected override string GenerateDefaultTitle()
        {
            return DateTime.Now.Ticks.ToString();
        }

        protected override string GenerateFilePath()
        {
            return GenerateRelativeFilePathInSubFolder("Renderings - " + ParentAsset.Title);
        }

        internal override void Save(string filePath)
        {
            EnsureSaveDirectoryExists("Renderings - " + ParentAsset.Title);
            base.Save(filePath);
        }

        protected override void Serialize()
        {
            Image.Save(AbsoluteFilePath, ImageFormat.Png);
        }

        internal override XmlDocument SerializeAsXml()
        {
            var root = CreateBaseXmlDocument();
            var xml = root.OwnerDocument;

            root.SetAttribute("FilePath", FilePath);

            return xml;
        }

        internal override void DeserializeFromXml(XmlNode xml)
        {
            if (xml.Name != SerializationName)
                throw new ArgumentException();

            dynamic xmlObject = new DynamicXmlParser(xml);
            InitializeAsSaved(xmlObject["FilePath"]);
            Image = new Bitmap(AbsoluteFilePath);
        }
    }
}
