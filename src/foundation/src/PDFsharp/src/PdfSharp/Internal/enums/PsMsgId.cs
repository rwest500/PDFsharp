// PDFsharp - A .NET library for processing PDF
// See the LICENSE file in the solution root for more information.

namespace PdfSharp.Internal
{
    /// <summary>
    /// PDFsharp message ID.
    /// Represents IDs for error and diagnostic messages generated by PDFsharp.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    enum PsMsgId
    {
        None = 0,

        // ----- General Messages ---------------------------------------------------------------------

        /// <summary>
        /// PSMsgID.
        /// </summary>
        SampleMessage1 = MessageIdOffset.Ps,

        /// <summary>
        /// PSMsgID.
        /// </summary>
        SampleMessage2,

        // ----- XGraphics Messages ---------------------------------------------------------------

        // ----- PDF Messages ----------------------------------------------------------------------

        /// <summary>
        /// PSMsgID.
        /// </summary>
        NameMustStartWithSlash,

        /// <summary>
        /// PSMsgID.
        /// </summary>
        UserOrOwnerPasswordRequired,

        // ----- PdfParser Messages -------------------------------------------------------------------

        /// <summary>
        /// PSMsgID.
        /// </summary>
        UnexpectedToken,

        /// <summary>
        /// PSMsgID.
        /// </summary>
        UnknownEncryption,
    }
}
