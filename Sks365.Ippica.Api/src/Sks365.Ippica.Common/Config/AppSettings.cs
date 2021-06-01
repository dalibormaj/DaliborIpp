using Sks365.Ippica.Common.Config.Abstraction;

namespace Sks365.Ippica.Common.Config
{
    /// <summary>
    /// Application settings
    /// </summary>
    /// <seealso cref="Sks365.Cashier.Common.Config.IAppSettings" />
    /// <seealso cref="Sks365.Cashier.Common.IAppSettings" />
    public class AppSettings : IAppSettings
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public ConnectionStringSection ConnectionStrings { get; set; }

        /// <summary>
        /// Gets or sets the integration API urls.
        /// </summary>
        /// <value>
        /// The integration API urls.
        /// </value>
        public IntegrationApiUrlSection IntegrationApiUrls { get; set; }
        public OperationRecorderSection OperationRecorder { get; set; }
        public EmailSenderSection EmailSender { get; set; }
    }
}