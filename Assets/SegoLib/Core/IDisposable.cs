namespace SegoLib
{
    public interface IDisposable : System.IDisposable
    {
        /// <summary>
        /// Is this object disposed?
        /// </summary>
        public bool IsDisposed { get; }
        /// <summary>
        /// Raised when this object gets disposed.
        /// </summary>
        public event System.Action<IDisposable> disposed;
    } 
}
