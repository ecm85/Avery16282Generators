using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.IO;

namespace Avery16282Generator.PL
{
    public class S3Service
    {
        private const string bucketName = "avery-16282-generators-temp";
        public static string UploadPdfToS3(byte[] bytes, string filenameWithoutExtension)
        {
            var client = new AmazonS3Client(
                Environment.GetEnvironmentVariable("S3_AWS_ACCESS_KEY_ID"),
                Environment.GetEnvironmentVariable("S3_AWS_SECRET_ACCESS_KEY"),
                RegionEndpoint.USEast2);
            var transferUtility = new TransferUtility(client);
            var filename = $"{filenameWithoutExtension}{Guid.NewGuid()}.pdf";
            using (var memoryStream = new MemoryStream(bytes))
            {
                var request = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    ContentType = "application/pdf",
                    CannedACL = S3CannedACL.PublicRead,
                    InputStream = memoryStream,
                    Key = filename,
                    StorageClass = S3StorageClass.Standard
                };
                transferUtility.Upload(request);
            }
            return $"https://{bucketName}.s3.us-east-2.amazonaws.com/{filename}";
        }
    }
}
