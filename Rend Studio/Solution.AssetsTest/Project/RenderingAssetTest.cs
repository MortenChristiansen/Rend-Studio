using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Studio.Assets.Project;
using Xunit;

namespace Studio.AssetsTest.Project
{
    [Trait("Studio.Assets", "RenderingAsset")]
    public class RenderingAssetTest
    {
        [Fact(DisplayName = "Deserializing a rendering asset from XML will replace relative path with absolute path")]
        public void T1()
        {
            TypeMocks.MockXmlDocumentFactory();
            //Mock directory actions
            //Mock bitmap factory
            var renderingAsset = new RenderingAsset();
            var sceneAsset = new SceneAsset();
            var absolutePath = "c:/file1.rnd";
            sceneAsset.Save(absolutePath);
            var relativePath = "/renderings/file2.png";
            sceneAsset.AddRendering(renderingAsset);
            var renderingXml = GetRelativePathSerializedRenderingAsset(relativePath);

            renderingAsset.DeserializeFromXml(renderingXml);

            //Assert that a bitmap was created with the absolute path
            Assert.Equal(relativePath, renderingAsset.FilePath);
        }

        private XmlNode GetRelativePathSerializedRenderingAsset(string relativePath)
        {
            var xml = TypeMocks.MockedXmlDocument;
            var node = xml.CreateElement("Rendering");
            node.SetAttribute("FilePath", relativePath);
            return node;
        }

        // Test that deserializing from XML set the proper propert(y/ties)
    }
}
