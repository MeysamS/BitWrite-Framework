namespace Bw.ObjectStorage.Minio.Models;

public class MinioFileOptions
{
    public string BucketName { get; set; } = string.Empty;
    public string ObjectName { get; set; } = string.Empty;
    public Stream FileStream { get; set; }
    public string ContentType { get; set; } = string.Empty;
}