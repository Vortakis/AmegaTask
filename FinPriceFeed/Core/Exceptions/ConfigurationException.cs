namespace FinPriceFeed.Core.Exceptions
{
    public class ConfigurationException : Exception
    {
        public string SettingKey { get; }

        public ConfigurationException(string settingKey, string message, Exception? inner = null) : base(message, inner)
        {
            SettingKey = settingKey;
        }

        public override string ToString()
        {
            var innerExceptionDetails = InnerException != null ? $" Inner Exception: {InnerException.ToString()}" : string.Empty;
            return $"{base.ToString()} (Setting Key: {SettingKey}){innerExceptionDetails}";
        }
    }
}
