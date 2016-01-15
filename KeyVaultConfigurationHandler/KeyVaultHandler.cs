using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyVaultConfigurationHandler
{
    using System.Configuration;
    using System.Xml;

    public class KeyVaultHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            XmlNode newNode = section.OwnerDocument.CreateNode(XmlNodeType.Element, "appSettings", section.NamespaceURI);
            // create an instance of the original handler and call it's create method returning the result.
            NameValueFileSectionHandler handler = new NameValueFileSectionHandler();
            return handler.Create(parent, configContext, newNode);
        }
    }
}
