using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using flowgear.Sdk;
using S3ToAzureBlob.Config;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace S3ToAzureBlob
{
    [Node("EventBooking.FlowgearNodes.S3ToFromAzureBlob", "S3 to/from Azure Blob", NodeType.Connector, "icon.png",
        RunFrom.DropPointUnrestrictedOnly)]
    public class S3ToAzureBlob
    {
        [Property(FlowDirection.Input, ExtendedType.ConnectionProfile)]
        public S3ToAzureBlobConnection Connection { get; set; }

        [Property(FlowDirection.Input)] public TransferDirection Direction { get; set; }

        [Property(FlowDirection.Input)] public string S3BucketName { get; set; }

        [Property(FlowDirection.Input)] public string S3ObjectKey { get; set; }

        [Property(FlowDirection.Input)] public string AzureBlobName { get; set; }

        [Property(FlowDirection.Input)] public string AzureBlobContentType { get; set; }

        [Configuration] public Dictionary<string, object> Configuration { get; set; }

        [Invoke]
        public InvokeResult Invoke()
        {
            ValidateInput();

            if (Direction == TransferDirection.S3ToAzure)
            {
                var client = GetS3Client();
                var request = new GetObjectRequest
                {
                    BucketName = S3BucketName,
                    Key = S3ObjectKey
                };
                var stream = client.GetObject(request).ResponseStream;

                var containerClient = GetAzureClient();
                var blobClient = containerClient.GetBlobClient(AzureBlobName);
                Task blobUploadTask = blobClient.UploadAsync(stream, new BlobHttpHeaders
                {
                    ContentType = AzureBlobContentType
                });

                blobUploadTask.GetAwaiter().GetResult();
            }

            return new InvokeResult();
        }

        private AmazonS3Client GetS3Client()
        {
            var credentials = new BasicAWSCredentials(Connection.S3AccessKey, Connection.S3SecretKey);
            var region = RegionEndpoint.GetBySystemName(Connection.S3RegionName);
            var client = new AmazonS3Client(credentials, region);
            return client;
        }

        private BlobContainerClient GetAzureClient()
        {
            var connectionString = "DefaultEndpointsProtocol=https;" +
                                   $"AccountName={Connection.AzureAccountName};"
                                   + (Connection.AzureAuthMethod == AzureAuthMethod.AccountKey
                                       ? $"AccountKey={Connection.AzureAuthCredential}"
                                       : $"SharedAccessSignature={Connection.AzureAuthCredential}") +
                                   ";EndpointSuffix=core.windows.net";
            var serviceClient = new BlobServiceClient(connectionString);
            var containerClient = serviceClient.GetBlobContainerClient(Connection.AzureContainer);
            return containerClient;
        }

        private void ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(Connection.S3AccessKey))
                throw new ArgumentNullException(nameof(Connection.S3AccessKey));
            if (string.IsNullOrWhiteSpace(Connection.S3SecretKey))
                throw new ArgumentNullException(nameof(Connection.S3SecretKey));
            if (string.IsNullOrWhiteSpace(Connection.S3RegionName))
                throw new ArgumentNullException(nameof(Connection.S3RegionName));

            if (string.IsNullOrWhiteSpace(Connection.AzureAccountName))
                throw new ArgumentNullException(nameof(Connection.AzureAccountName));
            if (string.IsNullOrWhiteSpace(Connection.AzureAuthCredential))
                throw new ArgumentNullException(nameof(Connection.AzureAuthCredential));
            if (string.IsNullOrWhiteSpace(Connection.AzureContainer))
                throw new ArgumentNullException(nameof(Connection.AzureContainer));

            if (string.IsNullOrWhiteSpace(S3BucketName))
                throw new ArgumentNullException(nameof(S3BucketName));
            if (string.IsNullOrWhiteSpace(S3BucketName))
                throw new ArgumentNullException(nameof(S3ObjectKey));

            if (Direction == TransferDirection.AzureToS3)
                throw new Exception("Currently only supporting S3-to-Azure");
        }
    }
}