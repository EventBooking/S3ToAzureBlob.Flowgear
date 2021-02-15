using flowgear.Sdk;

namespace S3ToAzureBlob.Config
{
    public class S3ToAzureBlobConnection
    {
        [Property(FlowDirection.Input)]
        public string S3AccessKey { get; set; }

        [Property(ExtendedType.Secret)]
        public string S3SecretKey { get; set; }

        [Property(FlowDirection.Input)]
        public string S3RegionName { get; set; }

        [Property(FlowDirection.Input)]
        public string AzureAccountName { get; set; }

        [Property(FlowDirection.Input)]
        public AzureAuthMethod AzureAuthMethod { get; set; }

        [Property(FlowDirection.Input)]
        public string AzureAuthCredential { get; set; }

        [Property(FlowDirection.Input)]
        public string AzureContainer { get; set; }
    }
}
