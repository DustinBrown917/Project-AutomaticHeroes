namespace SegoLib
{
    /// <summary>
    /// Objects implementing this can be disposed, but not by consumers of this interface. Data is exposed to notify if the object becomes disposed.
    /// </summary>
    public interface IPrivatelyDisposable
    {
        /// <summary>
        /// Is this object disposed?
        /// </summary>
        public bool IsDisposed { get; }
        /// <summary>
        /// Raised when this object gets disposed.
        /// </summary>
        public event System.Action<IPrivatelyDisposable> Disposed;
    }

    public interface IDisposable : IPrivatelyDisposable, System.IDisposable
    {

    } 
}
