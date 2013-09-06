using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.Serialization;
using System.Xml;
using Studio.Core.Testable;
using System.Reflection;

namespace Studio.Assets.Project
{
    public abstract class Asset
    {
        private List<Asset> _subAssets;

        /// <summary>
        /// Gets whether the asset has a file on disk to which
        /// it has been saved at some point. It does not consider
        /// whether the file is up to date.
        /// </summary>
        internal bool HasSaveFile { get; private set; }

        /// <summary>
        /// Gets the child assets for the current asset.
        /// </summary>
        internal IEnumerable<Asset> SubAssets { get { return _subAssets; } private set { _subAssets = value.ToList(); } }

        /// <summary>
        /// Gets the parent of the asset. Returns null if the asset has
        /// no logical parents.
        /// </summary>
        internal Asset ParentAsset { get; private set; }

        /// <summary>
        /// Gets the absolute path to the file where the asset
        /// is stored on disk.
        /// </summary>
        internal string FilePath { get; private set; }

        /// <summary>
        /// Gets whether the asset can generate a file name.
        /// </summary>
        internal virtual bool CanGenerateFileName { get { return false; } }

        /// <summary>
        /// Gets the file extension for the asset type.
        /// </summary>
        internal abstract string Extension { get; }

        /// <summary>
        /// Gets the textual descriptor for the asset type in file
        /// dialogs.
        /// </summary>
        internal abstract string FileDescription { get; }

        /// <summary>
        /// Gets the name used to identify the asset type when serialized.
        /// </summary>
        protected abstract string SerializationName { get; }

        /// <summary>
        /// Gets or sets the title of the asset. Is used as a basis for the file name
        /// of the asset and as the asset identifier.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Event that triggers when the asset is serialized to disk.
        /// </summary>
        protected event EventHandler Serialized;

        /// <summary>
        /// Gets the absolute path to the file, either directly from
        /// the asset, or derived from the parent asset. Returns null
        /// if there is not enough information to create the file path.
        /// </summary>
        public string AbsoluteFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FilePath))
                    return null;

                if (FilePath.Contains(Path.VolumeSeparatorChar))
                    return FilePath;

                if (ParentAsset == null)
                    return null;

                var parentPath = ParentAsset.AbsoluteFilePath;
                if (parentPath == null)
                    return null;

                var parentDirectory = Path.GetDirectoryName(parentPath);
                return parentDirectory + FilePath.Replace('/', '\\');
            }
        }

        /// <summary>
        /// Creates a new unsaved asset with a default generated title-
        /// </summary>
        public Asset()
        {
            _subAssets = new List<Asset>();
            HasSaveFile = false;
            Title = GenerateDefaultTitle();
        }

        /// <summary>
        /// Generate a default title for the asset.
        /// </summary>
        /// <returns>A default title.</returns>
        protected abstract string GenerateDefaultTitle();
        /// <summary>
        /// Creates an XML document, describing the asset.
        /// </summary>
        internal abstract XmlDocument SerializeAsXml();
        /// <summary>
        /// Populates the asset instance with data from an
        /// XML node containing the serialized asset.
        /// </summary>
        /// <param name="xml">An XML node representing the serialized asset.</param>
        internal abstract void DeserializeFromXml(XmlNode xml);

        private void OnSerialized()
        {
            if (Serialized != null)
                Serialized(this, EventArgs.Empty);
        }

        /// <summary>
        /// Add an asset to the current asset's SubAssets collection.
        /// </summary>
        /// <param name="asset">The sub asset to add.</param>
        internal void AddSubAsset(Asset asset)
        {
            _subAssets.Add(asset);
            asset.ParentAsset = this;
        }

        /// <summary>
        /// Creates a file path for the asset, based on its state. Returning null
        /// indicates the asset cannot automatically generate a path or is not in
        /// a state that supports it.
        /// </summary>
        /// <returns>A path to the file - or null if no path can be generated.</returns>
        protected virtual string GenerateFilePath()
        {
            return null;
        }

        /// <summary>
        /// Utility method for removing the extension from a file name.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected static string RemoveExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return fileName.Substring(0, fileName.Length - extension.Length);
        }

        /// <summary>
        /// Save the asset to disk. Assumes that the asset has a file name
        /// or can generate one itself.
        /// </summary>
        internal void Save()
        {
            if (!HasSaveFile)
            {
                if (!CanGenerateFileName)
                    throw new ArgumentException("The file has not been saved before. Supply a file path.");

                var path = GenerateFilePath();
                Save(path);
            }
            else
            {
                Serialize();
            }
        }

        /// <summary>
        /// Saves the asset to disk at the given location.
        /// </summary>
        /// <param name="filePath">The full path to the desired save location.</param>
        internal virtual void Save(string filePath)
        {
            FilePath = filePath;
            Serialize();
            HasSaveFile = true;
        }

        /// <summary>
        /// Serializes the asset and saves it to disk.
        /// </summary>
        protected virtual void Serialize()
        {
            //TODO: Error handling
            var xml = SerializeAsXml();
            xml.Save(AbsoluteFilePath);
            OnSerialized();
        }

        /// <summary>
        /// Instantiates an instance of an asset from a serialized
        /// source.
        /// </summary>
        /// <param name="filePath">The path to the file to desiralize the
        /// asset from.</param>
        /// <returns>The asset, populated from the serialization data.</returns>
        internal static TAsset Deserialize<TAsset>(string filePath)
            where TAsset : Asset, new()
        {
            //TODO: Error handling
            var asset = new TAsset();
            var xml = ObjectFactory.XmlDocumentFactory();
            xml.Load(filePath);
            asset.InitializeAsSaved(filePath);
            asset.DeserializeFromXml(xml.LastChild);
            return asset;
        }

        /// <summary>
        /// Initializes the state of the asset as saved.
        /// </summary>
        /// <param name="filePath">The path of the file the asset is saved in.</param>
        protected void InitializeAsSaved(string filePath)
        {
            FilePath = filePath;
            _subAssets = new List<Asset>();
            HasSaveFile = true;
        }

        /// <summary>
        /// Saves the asset to disk.
        /// </summary>
        /// <param name="fileName">The location to save the asset.</param>
        /// <param name="title">The title to assign the asset.</param>
        internal void Save(string fileName, string title)
        {
            Title = title;
            Save(fileName);
        }

        /// <summary>
        /// Instantiates an asset based on its type name.
        /// </summary>
        /// <param name="type">The serialization name of the asset.</param>
        /// <returns>An instance of the asset, created using its
        /// empty constructor. Throws an exception if no valid type was found.</returns>
        internal static Asset GetAssetInstanceByTypeName(string type)
        {
            var typeName = type + "Asset";
            var assetAssembly = Assembly.GetAssembly(typeof(Asset));
            var assetType = GetAssetSubType(assetAssembly, typeName);
            
            if (assetType == null)
            {
                var executingAssembly = Assembly.GetCallingAssembly();
                if (assetAssembly != executingAssembly)
                    assetType = GetAssetSubType(executingAssembly, typeName);
            }

            if (assetType == null)
                throw new ArgumentException("Invalid Asset type: " + typeName);

            return (Asset)Activator.CreateInstance(assetType);
        }

        private static Type GetAssetSubType(Assembly assembly, string typeName)
        {
            var types = assembly.GetTypes();
            return types.FirstOrDefault(t => t.Name == typeName && typeof(Asset).IsAssignableFrom(t) && t.GetConstructors().Any(c => c.GetParameters().Length == 0));
        }

        /// <summary>
        /// Utility method for creating an empty XML document for serialization 
        /// purposes.
        /// </summary>
        /// <returns>The root node of the XML document.</returns>
        protected XmlElement CreateBaseXmlDocument()
        {
            var xml = ObjectFactory.XmlDocumentFactory();
            var declaration = xml.CreateXmlDeclaration("1.0", null, null);
            xml.AppendChild(declaration);

            var root = xml.CreateElement(SerializationName);
            xml.AppendChild(root);

            return root;
        }

        /// <summary>
        /// Appends the sub assets of the current asset to the xml document as xml 
        /// nodes. The XML will include the fully serialized asset.
        /// </summary>
        /// <param name="element">The xml element to append the sub assets to.</param>
        protected void AppendSubAssets(XmlElement element)
        {
            var xml = element.OwnerDocument;
            var assets = xml.CreateElement("SubAssets");
            foreach (var asset in SubAssets)
            {
                var assetXml = asset.SerializeAsXml();
                var importedNode = xml.ImportNode(assetXml.LastChild, deep: true);
                assets.AppendChild(importedNode);
            }
            element.AppendChild(assets);
        }

        /// <summary>
        /// Appends the sub assets of the current asset to the XML document as 
        /// references. The references will include the type of asset and the 
        /// path of the asset file.
        /// </summary>
        /// <param name="element">The xml element to append the sub assets to.</param>
        protected void AppendSubAssetReferences(XmlElement element)
        {
            var xml = element.OwnerDocument;
            var assets = xml.CreateElement("SubAssets");
            foreach (var asset in SubAssets)
            {
                var subAsset = xml.CreateElement(asset.SerializationName);
                subAsset.SetAttribute("FilePath", asset.FilePath);
                assets.AppendChild(subAsset);
            }
            element.AppendChild(assets);
        }

        /// <summary>
        /// Instantiates the assets serialized in the XML and adds them to
        /// the sub assets of the current asset.
        /// </summary>
        /// <param name="subAssets">A number of xml nodes, each containing a
        /// serialized asset.</param>
        protected void DeserializeSubAssets(List<XmlNode> subAssets) 
        {
            foreach (var assetNode in subAssets)
            {
                var asset = Asset.GetAssetInstanceByTypeName(assetNode.Name);
                AddSubAsset(asset);
                asset.DeserializeFromXml(assetNode);
            }
        }

        /// <summary>
        /// Instantiates the assets referenced in the XML and adds them to
        /// the sub assets of the current asset.
        /// </summary>
        /// <param name="subAssets">A number of xml nodes, each containing a
        /// node with the path to the file describing the actual sub asset.</param>
        protected void DeserializeReferencedSubAssets(List<XmlNode> subAssets)
        {
            foreach (var assetNode in subAssets)
            {
                var subAsset = GetAssetInstanceByTypeName(assetNode.Name);
                subAsset.InitializeAsSaved(assetNode.Attributes["FilePath"].Value);
                AddSubAsset(subAsset);
                var xml = ObjectFactory.XmlDocumentFactory();
                xml.Load(subAsset.AbsoluteFilePath);
                subAsset.DeserializeFromXml(xml.LastChild);
            }
        }

        /// <summary>
        /// Gets a file path for the asset to be saved in. The path is based
        /// on the file path of the parent asset and the desired name of
        /// the folder to put it in. If no parnt asset exists, null is returned.
        /// </summary>
        /// <param name="folderName">The name of the folder for the asset to be
        /// located in.</param>
        /// <returns>The path of the file, in the named folder - or null
        /// if no parent asset is defined.</returns>
        protected string GenerateRelativeFilePathInSubFolder(string folderName)
        {
            if (ParentAsset.FilePath == null) return null;

            return string.Format("/{0}/{1}.{2}", folderName, Title, Extension);
        }

        /// <summary>
        /// Gets the absolute path to a folder with the given name.
        /// </summary>
        /// <param name="folderName">The name of the folder</param>
        /// <returns>The path to the folder</returns>
        protected string GetSaveDirectory(string folderName)
        {
            var baseDirectoryPath = Path.GetDirectoryName(ParentAsset.AbsoluteFilePath);
            var renderingDirectoryPath = Path.Combine(baseDirectoryPath, folderName);
            return renderingDirectoryPath;
        }

        /// <summary>
        /// Determines if the save directory for the asset exists and
        /// creates it if it does not.
        /// </summary>
        /// <param name="directoryName">The name of the directory.</param>
        protected void EnsureSaveDirectoryExists(string directoryName)
        {
            var renderingDirectoryPath = GetSaveDirectory(directoryName);
            var directory = new DirectoryInfo(renderingDirectoryPath);
            if (!directory.Exists)
                directory.Create();
        }
    }
}
