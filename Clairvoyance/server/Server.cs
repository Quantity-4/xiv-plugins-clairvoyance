using System;
using System.Collections.Generic;
using System.Text;
using EmbedIO;
using EmbedIO.Actions;

namespace Clairvoyance.server;

public class Server : IDisposable
{
    private readonly Dictionary<string, MapEntity> _activeMaps = new();
    private readonly WebServer _webServer;


    public Server()
    {
        // Initialize and configure the web server
        _webServer = new WebServer(o => o
                .WithUrlPrefix("http://localhost:9696")
                .WithMode(HttpListenerMode.EmbedIO))
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
    }

    public void AddNewActiveMap(string mapId)
    {
        _activeMaps.Add(mapId, new MapEntity(mapId));
    }

    public MapEntity GetActiveMap(string mapId)
    {
        return _activeMaps[mapId];
    }

    public void Dispose()
    {
        _webServer?.Dispose();
    }
}