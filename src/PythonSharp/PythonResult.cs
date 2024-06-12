namespace PythonSharp
{
    using System;
    using System.Collections.Specialized;

    /// <summary>
    /// Python result.
    /// </summary>
    public class PythonResult
    {
        #region Public-Members

        /// <summary>
        /// GUID.
        /// </summary>
        public Guid GUID { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Success or failure result.
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// Data.
        /// </summary>
        public string Data { get; set; } = null;

        /// <summary>
        /// Console output.
        /// </summary>
        public string ConsoleOutput { get; set; } = null;

        /// <summary>
        /// Error output.
        /// </summary>
        public string ErrorOutput { get; set; } = null;

        /// <summary>
        /// Result from executing shell commands.
        /// </summary>
        public int Result { get; set; } = 0;

        /// <summary>
        /// Exception, if any.
        /// </summary>
        public Exception Exception { get; set; } = null;

        #endregion

        #region Private-Members

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate.
        /// </summary>
        public PythonResult()
        {

        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        #endregion
    }
}
