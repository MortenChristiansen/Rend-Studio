using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Moq;
using Studio.Core.Testable;

namespace Studio.AssetsTest
{
    static class TypeMocks
    {
        public static XmlDocument MockedXmlDocument { get; private set; }

        public static Mock<XmlDocument> MockXmlDocumentFactory()
        {
            var xmlMock = new Mock<XmlDocument>();
            ObjectFactory.XmlDocumentFactory = () => xmlMock.Object;
            MockedXmlDocument = xmlMock.Object;
            return xmlMock;
        }
    }
}
