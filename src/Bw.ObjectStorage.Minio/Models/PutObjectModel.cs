namespace Bw.ObjectStorage.Minio.Models;

public class PutObjectModel
{
    public string Bucket { get; set; }
    public byte[] Data { get; set; }
}