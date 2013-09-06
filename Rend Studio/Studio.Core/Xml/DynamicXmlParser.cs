using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Studio.Core.Xml
{
    public class DynamicXmlParser : DynamicObject
    {
        XmlNode _node;

        public DynamicXmlParser(XmlNode node)
        {
            _node = node;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_node == null)
            {
                result = null;
                return false;
            }

            var subNode = _node.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name == binder.Name);

            if (subNode == null)
            {
                result = null;
                return false;
            }
            else
            {
                result = new DynamicXmlParser(subNode);
                return true;
            }
        }

        public override string ToString()
        {
            if (_node != null)
            {
                return _node.InnerText;
            }
            else
            {
                return string.Empty;
            }
        }

        public string this[string attr]
        {
            get
            {
                if (_node == null)
                {
                    return string.Empty;
                }

                return _node.Attributes[attr].Value;
            }
        }

        public static implicit operator string(DynamicXmlParser xml)
        {
            return xml.ToString();
        }

        public static implicit operator List<XmlNode>(DynamicXmlParser xml)
        {
            if (xml._node == null)
                return new List<XmlNode>();

            return xml._node.ChildNodes.Cast<XmlNode>().ToList();
        }
    }
}
