using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.WebSockets;
using Newtonsoft.Json;

namespace Clairvoyance.server;

public class Server : IDisposable
{
    private readonly Dictionary<string, MapEntity> _activeMaps = new();
    private WebServer _webServer;
    private RealTimeModule _realTimeModule;


    private DateTime _lastUpdateTime = DateTime.MinValue;
    private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(0.005);

    public Server()
    {
        // Initialize the WebSocket module
        _realTimeModule = new RealTimeModule("/realtime");

        // Initialize and configure the web server
        _webServer = new WebServer(o => o
                .WithUrlPrefix("http://*:9696")
                .WithMode(HttpListenerMode.EmbedIO))
            .WithCors()
            .WithModule(_realTimeModule) // Add the WebSocket module
            .WithModule(new ActionModule("/map/{mapId}", HttpVerbs.Get, async ctx =>
            {
                // Extracting the mapId from the URL
                var urlSegments = ctx.Request.Url.Segments;
                var mapId = urlSegments.Length > 2 ? urlSegments[2].TrimEnd('/') : string.Empty;

                if (!string.IsNullOrEmpty(mapId) && _activeMaps.TryGetValue(mapId, out var mapEntity))
                {
                    await ctx.SendDataAsync(mapEntity.PlayerEntities.Values); // Send PlayerEntities as JSON
                }
                else
                {
                    ctx.Response.StatusCode = 404;
                    await ctx.SendStringAsync("Map not found", "text/plain", Encoding.UTF8);
                }
            }))
            .WithModule(new ActionModule("/", HttpVerbs.Any,
                ctx => ctx.SendDataAsync(new { Message = "Hello World" })));

        // Start the WebServer
        _webServer.Start();
    }

    public void Update(string mapId, string playerName, int playerHomeWorldId, float posX, float posY, float posZ)
    {
        // Function to be called frequently on updates if needs be

        // [1] - Check if we currently have the map within our dictionary
        if (!_activeMaps.ContainsKey(mapId))
        {
            _activeMaps.Add(mapId, new MapEntity(mapId));
        }

        // [2] Pass through the parameters to update to the specific map
        _activeMaps[mapId].Update(playerName, playerHomeWorldId, posX, posY, posZ);

        var activeMapsJson = JsonConvert.SerializeObject(_activeMaps.Values);

        if (DateTime.Now - _lastUpdateTime < _updateInterval)
        {
            // Not enough time has passed since the last update, so just return.
            return;
        }

        SendRealTimeUpdate(activeMapsJson);

        // Update the last update time.
        _lastUpdateTime = DateTime.Now;

        // SendRealTimeUpdateAsync(activeMapsJson);
    }

    public void SendRealTimeUpdate(string message)
    {
        _realTimeModule?.SendToAllAsync(message);
    }

    public async Task SendRealTimeUpdateAsync(string message)
    {
        await _realTimeModule.SendToAllAsync(message);
    }

    public void AddNewActiveMap(string mapId)
    {
        _activeMaps.Add(mapId, new MapEntity(mapId));
    }

    public MapEntity GetActiveMap(string mapId)
    {
        return _activeMaps[mapId];
    }

    public void ClearMapData()
    {
        foreach (var dictEntry in _activeMaps)
        {
            dictEntry.Value.PlayerEntities.Clear();
            var activeMapsJson = JsonConvert.SerializeObject(_activeMaps.Values);

            if (DateTime.Now - _lastUpdateTime < _updateInterval)
            {
                // Not enough time has passed since the last update, so just return.
                return;
            }

            SendRealTimeUpdate(activeMapsJson);

            // Update the last update time.
            _lastUpdateTime = DateTime.Now;
            // SendRealTimeUpdateAsync(activeMapsJson);
        }
    }

    public void Dispose()
    {
        _webServer?.Dispose();
    }
}