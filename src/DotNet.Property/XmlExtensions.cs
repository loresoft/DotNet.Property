using System;
using System.Xml.Linq;

namespace DotNet.Property
{
    public static class XmlExtensions
    {
        public static XElement GetOrCreateElement(this XContainer container, string name)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var element = container.Element(name);
            if (element != null)
                return element;

            element = new XElement(name);
            container.Add(element);

            return element;
        }
    }
}