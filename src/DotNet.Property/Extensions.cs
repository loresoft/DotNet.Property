using System.Xml.Linq;

namespace DotNet.Property;

/// <summary>
/// Extension methods
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Gets the or create an <see cref="XElement"/>.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="name">The name of the element.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"><paramref name="container"/> or <paramref name="name"/> is <see langword="null"/></exception>
    public static XElement GetOrCreateElement(this XContainer container, string name)
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(name);

        var element = container.Element(name);
        if (element != null)
            return element;

        element = new XElement(name);
        container.Add(element);

        return element;
    }
}
