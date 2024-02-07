using Bw.ObjectStorage.Minio.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Result;
using Minio.Exceptions;
using System.Globalization;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Bw.ObjectStorage.Minio;

public interface IMinioBlobStorage
{
    public IMinioClient MinioClient { get; set; }
    public Task<List<string>?> GetBucketsAsStringAsync();
    public Task<ListAllMyBucketsResult> GetBucketsAsync();
    Task<bool> IsExistBucket(string bucketName);
    Task MakeBucketAsync(string bucketName);
    Task RemoveBucketAsync(string bucketName);
    Task UploadFile(Action<MinioFileOptions> options);
    Task UploadFiles(IList<IFormFile> files);
    Task<ICollection<string>> GetObjectsAsync(string bucketName);
    Task<GetObjectModel> GetObjectAsync(string bucket, string objectName);
    Task<string> PutObjectAsync(PutObjectModel model);
}

public class MinioBlobStorage : IMinioBlobStorage
{
    private readonly ILogger<MinioBlobStorage> _logger;
    private readonly IMinioClient _minioClient;
    public IMinioClient MinioClient { get; set; }

    public MinioBlobStorage(IMinioClientFactory minioClientFactory, ILogger<MinioBlobStorage> logger)
    {
        _logger = logger;
        _minioClient = minioClientFactory.CreateClient(nameof(MinioOptions)).Build();
        MinioClient = _minioClient;
        SetCulture();
    }

    private void SetCulture()
    {
        var current = new CultureInfo("fa-IR");
        current.DateTimeFormat = new DateTimeFormatInfo();
        current.DateTimeFormat.Calendar = new GregorianCalendar();
        Thread.CurrentThread.CurrentCulture = current;
    }


    public async Task<List<string>?> GetBucketsAsStringAsync()
    {
        List<string>? buckets = null;
        try
        {
            // List buckets that have read access.
            var response = await _minioClient.ListBucketsAsync();
            buckets = response.Buckets.Select(x => x.Name).ToList();
        }
        catch (MinioException e)
        {
            Console.WriteLine("Error occurred: " + e);
        }

        return buckets;
    }

    public async Task<ListAllMyBucketsResult> GetBucketsAsync()
    {
        return await _minioClient.ListBucketsAsync();
    }

    public async Task<bool> IsExistBucket(string bucketName)
    {
        var beArgs = new BucketExistsArgs()
            .WithBucket(bucketName);
        return await _minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
    }


    public async Task MakeBucketAsync(string bucketName)
    {
        try
        {
            // Create bucket if it doesn't exist.
            bool found = await IsExistBucket(bucketName);
            if (found)
            {
                Console.WriteLine($"{bucketName} already exists");
            }
            else
            {
                var mbArgs = new MakeBucketArgs()
                    .WithBucket(bucketName);
                await _minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                Console.WriteLine($"{bucketName} is created successfully");
            }
        }
        catch (MinioException e)
        {
            Console.WriteLine("Error occurred: " + e);
        }

    }

    public async Task RemoveBucketAsync(string bucketName)
    {
        // Create bucket if it doesn't exist.
        bool found = await IsExistBucket(bucketName);
        if (!found)
        {
            Console.WriteLine($"{bucketName} dont have exists");
        }
        var mbArgs = new RemoveBucketArgs()
            .WithBucket(bucketName);
        await _minioClient.RemoveBucketAsync(mbArgs).ConfigureAwait(false);
    }

    public async Task UploadFile(Action<MinioFileOptions> options)
    {
        MinioFileOptions source = new();
        options(source);

        // Upload a file to bucket.
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(source.BucketName)
            .WithObject(source.ObjectName)
            .WithStreamData(source.FileStream)
            .WithObjectSize(source.FileStream.Length)
            .WithContentType(source.ContentType);
        await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

        // confirm upload 
        StatObjectArgs statObjectArgs = new StatObjectArgs()
            .WithBucket(source.BucketName)
            .WithObject(source.ObjectName);
        ObjectStat objectStat = await _minioClient.StatObjectAsync(statObjectArgs);
        Console.WriteLine("Successfully uploaded " + source.ObjectName);

    }

    public Task UploadFiles(IList<IFormFile> files)
    {
        throw new NotImplementedException();
    }

    public async Task<string> PutObjectAsync(PutObjectModel model)
    {
        var bucketName = model.Bucket;
        // Check Exists bucket
        bool found = await IsExistBucket(bucketName);
        if (!found)
        {
            // if bucket not Exists,make bucket
            await MakeBucketAsync(bucketName);
        }
        MemoryStream filestream = new MemoryStream(model.Data);
        var filename = Guid.NewGuid();
        // upload object
        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(bucketName).WithFileName(filename.ToString())
            .WithStreamData(filestream).WithObjectSize(filestream.Length)
        );

        return await Task.FromResult(filename.ToString());
    }

    public async Task<GetObjectModel> GetObjectAsync(string bucket, string objectName)
    {
        MemoryStream destination = new MemoryStream();
        // Check Exists object
        var objectStateReply = await _minioClient.StatObjectAsync(new StatObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectName)
        );
        if (objectStateReply == null || objectStateReply.DeleteMarker)
            throw new Exception("object not found or Deleted");
        // Get object
        await _minioClient.GetObjectAsync(new GetObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectName)
            .WithCallbackStream((stream) =>
                {
                    stream.CopyTo(destination);
                }
            )
        );
        return await Task.FromResult(new GetObjectModel()
        {
            Data = destination.ToArray(),
            ObjectStat = objectStateReply
        });
    }
    public async Task<ICollection<string>> GetObjectsAsync(string bucketName)
    {
        var files = new List<string>();
        try
        {
            bool found = await IsExistBucket(bucketName: bucketName);
            if (!found)
                throw new Exception("Bucket Name is Not Exist");

            ListObjectsArgs args = new ListObjectsArgs()
                .WithBucket(bucketName)
                // .WithPrefix("prefix")
                .WithRecursive(true);
            var observable = _minioClient.ListObjectsAsync(args);
            IDisposable subscription = observable.Subscribe(
                item =>
                {
                    files.Add(item.Key);
                    Console.WriteLine("OnNext: {0}", item.Key);
                },
                ex => Console.WriteLine("OnError: {0}", ex.Message),
                () => Console.WriteLine("OnComplete: {0}"));
            await observable.ToTask();
            // observable.Wait();
            Console.WriteLine($"{bucketName} does not exist");
        }
        catch (MinioException e)
        {
            Console.WriteLine("Error occurred: " + e);
        }
        return files.ToList();
    }

    public async Task<string> UploadFile(string bucketName, IFormFile file)
    {
        var key = string.Empty;
        try
        {
            key = Guid.NewGuid().ToString();
            var stream = file.OpenReadStream();
            var putObjectArgs = new PutObjectArgs();

            putObjectArgs.WithBucket(bucketName);
            putObjectArgs.WithStreamData(stream);
            putObjectArgs.WithContentType(file.ContentType);
            await _minioClient.PutObjectAsync(putObjectArgs);
        }
        catch (Exception e)
        {
            _logger.LogError("Error ocurred In UploadFileAsync", e);
        }
        return key;
    }
}
