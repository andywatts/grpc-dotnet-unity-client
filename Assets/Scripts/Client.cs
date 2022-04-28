using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Routeguide;
using UnityEngine;

public class Client : MonoBehaviour
{
    private int i = 0;

    private RouteGuide.RouteGuideClient client;
    CancellationTokenSource cancellationTokenSource;

    async void Start()
    {
        Debug.Log("Awake");
        CreateGrpcClient();

        DoUnary();
        // DoUnaryAsync();
        // DoServerStream();
    

    private async Task DoServerStream()
    { 
        cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        
        using var call = client.ListFeatures(new Rectangle{Lo = new Point {Latitude = -744026814, Longitude = -744026814}, Hi = new Point { Latitude = 412346009, Longitude = 412346009 }});
            
        while (await call.ResponseStream.MoveNext(token))
        {
            var item = call.ResponseStream.Current;
            Debug.Log("Success! " + item.Name);
        }
    }

    private void CreateGrpcClient()
    {
        // Backport of SocketsHttpHandler to .NET Standard 2.0
        System.AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        var channel = GrpcChannel.ForAddress("http://localhost:50051", new GrpcChannelOptions
        {
            HttpHandler = new StandardSocketsHttpHandler()
        });
        client = new RouteGuide.RouteGuideClient(channel);
    }

    private void DoUnary()
    {
        if (client == null) return;
        Debug.Log("GetFeature start");
        var response = client.GetFeature(new Point { Latitude = 402948455, Longitude = -747903913 });
        Debug.Log(response.Name);
    }
    
    private async Task DoUnaryAsync()
    {
        if (client == null) return;
        Debug.Log("GetFeatureAsync start");
        var response = await client.GetFeatureAsync(new Point { Latitude = 402948455, Longitude = -747903913 });
        Debug.Log(response.Name);
    }

    private void OnDestroy()
    {
        if (cancellationTokenSource != null)
            cancellationTokenSource.Cancel();
    }
}