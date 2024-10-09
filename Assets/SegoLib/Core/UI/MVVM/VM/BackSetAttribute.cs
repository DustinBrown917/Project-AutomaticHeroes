
namespace SegoLib.UI.MVVM
{
    public class BackSetAttribute : System.Attribute
    {
        public readonly string backsetMethodName;

        public BackSetAttribute(string backsetMethodName)
        {
            this.backsetMethodName = backsetMethodName;
        }
    } 
}
