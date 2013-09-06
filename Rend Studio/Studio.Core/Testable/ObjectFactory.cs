using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Studio.Core.Testable
{
    public static class ObjectFactory
    {
        /// <summary>
        /// Gets or sets the factory method used for instantiating
        /// new instances of XmlDocument objects.
        /// </summary>
        public static Func<XmlDocument> XmlDocumentFactory { get; set; }

        static ObjectFactory()
        {
            XmlDocumentFactory = () => new XmlDocument();
        }
    }
}
