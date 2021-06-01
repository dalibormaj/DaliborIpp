namespace Sks365.Ippica.Common.Config.Abstraction
{
    /// <summary>
    /// Application settings interface
    /// </summary>
    public interface IAppSettings
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        ConnectionStringSection ConnectionStrings { get; }
        /// <summary>
        /// Gets the integration API urls.
        /// </summary>
        /// <value>
        /// The integration API urls.
        /// </value>
        IntegrationApiUrlSection IntegrationApiUrls { get; }
        OperationRecorderSection OperationRecorder { get; }
        EmailSenderSection EmailSender { get; }
    }
}