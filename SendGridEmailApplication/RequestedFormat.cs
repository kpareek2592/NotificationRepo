namespace SendGridEmailApplication
{
    /// <summary>
    /// Requested format for response, can be passed in query string
    /// </summary>
    public enum RequestedFormat
    {
        /// <summary>
        /// Data will be serialized in JSON
        /// </summary>
        Json = 1,
        /// <summary>
        /// Data will be serialized in XML
        /// </summary>
        Xml = 2,
        /// <summary>
        /// Data will be serialized in CSV
        /// </summary>
        Csv = 3
    }
}
