using Minio.DataModel;

namespace Bw.ObjectStorage.Minio.Models;

public class GetObjectModel
{
    public ObjectStat ObjectStat { get; set; }
    public byte[] Data { get; set; }
}