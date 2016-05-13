using System;

namespace INetSales.Objects
{
    public interface IUploader
    {
        /// <summary>
        /// Indica que esta pendente de upload.
        /// </summary>
        bool IsPendingUpload { get; set; }

        /// <summary>
        /// Data em foi feito o ultimo upload.
        /// </summary>
        DateTime? DataLastUpload { get; set; }
    }
}