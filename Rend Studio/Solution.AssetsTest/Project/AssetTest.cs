using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Moq;
using Studio.Assets.Project;
using Studio.AssetsTest;
using Studio.Core.Testable;
using Xunit;

namespace Solution.AssetsTest.Project
{
    [Trait("Studio.Assets", "Asset")]
    public class AssetTest
    {
        #region Save tests

        [Fact(DisplayName = "An unsaved asset has no file path and no save file")]
        public void T1()
        {
            var asset = new TestAsset();

            Assert.False(asset.HasSaveFile);
            Assert.Null(asset.FilePath);
        }

        [Fact(DisplayName="Saving asset triggers saving of XmlDocument to the specified file path")]
        public void T2()
        {
            var xmlMock = TypeMocks.MockXmlDocumentFactory();
            var asset = new TestAsset();
            var filePath = "c:/fileName.rnd";

            asset.Save(filePath);

            xmlMock.Verify(x => x.Save(filePath), Times.Once());
        }

        [Fact(DisplayName = "Saving asset sets FilePath and save state")]
        public void T3()
        {
            var xmlMock = TypeMocks.MockXmlDocumentFactory();
            var asset = new TestAsset();
            var filePath = "c:/fileName.rnd";

            asset.Save(filePath);

            Assert.True(asset.HasSaveFile);
            Assert.Equal(filePath, asset.FilePath);
        }

        #endregion

        #region AbsoluteFilePath tests

        [Fact(DisplayName = "AbsoluteFilePath is null when FilePath is not set")]
        public void T10()
        {
            var asset = new TestAsset();

            Assert.Null(asset.AbsoluteFilePath);
        }

        [Fact(DisplayName = "AbsoluteFilePath equals FilePath if FilePath is absolute")]
        public void T11()
        {
            TypeMocks.MockXmlDocumentFactory();
            var asset = new TestAsset();
            var absolutePath = "c:/file.rnd";

            asset.Save(absolutePath);

            Assert.Equal(absolutePath, asset.AbsoluteFilePath);
        }

        [Fact(DisplayName = "AbsoluteFilePath is null if FilePath is relative and the asset does not have a parent asset")]
        public void T12()
        {
            TypeMocks.MockXmlDocumentFactory();
            var asset = new TestAsset();
            var relativePath = "file.rnd";

            asset.Save(relativePath);

            Assert.Null(asset.AbsoluteFilePath);
        }

        [Fact(DisplayName = "AbsoluteFilePath is null, when FilePath is relative and the parent asset has AbsoluteFilePath equal null")]
        public void T13()
        {
            TypeMocks.MockXmlDocumentFactory();
            var asset = new TestAsset();
            var parentAsset = new TestAsset();
            parentAsset.AddSubAsset(asset);
            var relativePath = "/folder/file2.rnd";

            asset.Save(relativePath);

            Assert.Null(asset.AbsoluteFilePath);
        }

        [Fact(DisplayName = "AbsoluteFilePath gets the absolute path of the asset, when FilePath is relative and the parent asset has an absolute path")]
        public void T14()
        {
            TypeMocks.MockXmlDocumentFactory();
            var asset = new TestAsset();
            var parentAsset = new TestAsset();
            parentAsset.AddSubAsset(asset);
            var absolutePath = "c:/file1.rnd";
            var relativePath = "/folder/file2.rnd";
            parentAsset.Save(absolutePath);

            asset.Save(relativePath);

            Assert.Equal(@"c:\\folder\file2.rnd", asset.AbsoluteFilePath);
        }

        #endregion

        #region SubAssets tests

        [Fact(DisplayName = "Adding child asset, sets ParentAsset on child asset")]
        public void T20()
        {
            var asset = new TestAsset();
            var parentAsset = new TestAsset();

            parentAsset.AddSubAsset(asset);

            Assert.Equal(parentAsset, asset.ParentAsset);
        }

        [Fact(DisplayName = "Adding child asset, adds the child asset the SubAssets collection")]
        public void T21()
        {
            var asset = new TestAsset();
            var parentAsset = new TestAsset();

            parentAsset.AddSubAsset(asset);

            Assert.True(parentAsset.SubAssets.Contains(asset));
            Assert.Equal(1, parentAsset.SubAssets.Count());
        }

        #endregion

        #region Deserialization tests

        [Fact(DisplayName = "Creating asset instance by type name works for arbitrary types")]
        public void T30()
        {
            var result = Asset.GetAssetInstanceByTypeName("Test");

            Assert.NotNull(result);
            Assert.True(result is TestAsset);
        }

        [Fact(DisplayName = "Creating asset instance by type name throws an exception for invalid types")]
        public void T31()
        {
            Assert.Throws<ArgumentException>(() => Asset.GetAssetInstanceByTypeName("NonExistingType"));
        }

        [Fact(DisplayName = "Creating asset instance by type name throws an exception for types without default constructor")]
        public void T32()
        {
            Assert.Throws<ArgumentException>(() => Asset.GetAssetInstanceByTypeName("Test2"));
        }

        #endregion
    }

    public class TestAsset : Asset
    {
        internal override string Extension { get { throw new NotImplementedException(); } }
        internal override string FileDescription { get { throw new NotImplementedException(); } }
        protected override string SerializationName { get { throw new NotImplementedException(); } }

        protected override string GenerateDefaultTitle()
        {
            return null;
        }

        internal override XmlDocument SerializeAsXml()
        {
            return ObjectFactory.XmlDocumentFactory();
        }

        internal override void DeserializeFromXml(XmlNode xml)
        {

        }
    }

    public class Test2Asset : TestAsset
    {
        public Test2Asset(int arbitraryArgument)
        {

        }
    }
}
