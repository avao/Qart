using System.Xml.Linq;

namespace Qart.Testing.Execution
{
    public interface IDescriptionWriter
    {
        void AddNote(string name, string value);
        IDescriptionWriter CreateNestedWriter(string scope);
        XDocument GetContent();
    }
}
