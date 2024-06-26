﻿using System.Text;
using System.Xml;

namespace XmlContentTranslator
{
    public static class XmlUtils
    {
        private static readonly StringBuilder Sb = new StringBuilder();

        public static bool ContainsText(XmlNode node)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Text)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsTextNode(XmlNode node)
        {
            return node.ChildNodes.Count == 1 && node.ChildNodes[0].NodeType == XmlNodeType.Text || ContainsText(node);
        }

        public static string BuildNodePath(XmlNode node)
        {
            Sb.Clear();
            Sb.Append(node.Name);
            if (node.NodeType == XmlNodeType.Attribute)
            {
                XmlNode old = node;
                node = (node as XmlAttribute).OwnerElement; // use OwnerElement for attributes as ParentNode is null
                Sb.Insert(0, node.Name + "@" + GetAttributeIndex(node, old));
                //  node = node.ParentNode;
            }
            while (node.ParentNode != null)
            {
                Sb.Insert(0, node.ParentNode.Name + GetNodeIndex(node) + "/");
                node = node.ParentNode;
            }
            return Sb.ToString();
        }

        public static string GetAttributeIndex(XmlNode node, XmlNode child)
        {
            if (node.Attributes == null)
                return string.Empty;

            int nameCount = 0;
            foreach (XmlAttribute x in node.Attributes)
            {
                if (x.Name == child.Name)
                    nameCount++;
            }

            int i = 0;
            foreach (XmlAttribute x in node.Attributes)
            {
                if (x == node)
                    break;
                i++;
            }
            return i == 0 || nameCount < 2 ? string.Empty : string.Format("[{0}]", i);
        }

        public static string GetNodeIndex(XmlNode node)
        {
            int i = 0;
            if (node.NodeType == XmlNodeType.Comment || node.NodeType == XmlNodeType.CDATA)
                return string.Empty;

            if (!string.IsNullOrEmpty(node.NamespaceURI))
            {
                var man = new XmlNamespaceManager(node.OwnerDocument.NameTable);
                man.AddNamespace(node.Prefix, node.NamespaceURI);

                foreach (var x in node.ParentNode.SelectNodes(node.Name, man))
                {
                    if (x == node)
                        break;
                    i++;
                }

            }
            else if (node.NodeType != XmlNodeType.Text)
            {
                foreach (var x in node.ParentNode.SelectNodes(node.Name))
                {
                    if (x == node)
                        break;
                    i++;
                }
            }

            return i == 0 ? string.Empty : string.Format("[{0}]", i);
        }

        public static bool IsParentElement(XmlNode xnode)
        {
            return xnode.ChildNodes.Count > 0 && !IsTextNode(xnode) &&
                xnode.NodeType != XmlNodeType.Comment && xnode.NodeType != XmlNodeType.CDATA;
        }
    }
}
