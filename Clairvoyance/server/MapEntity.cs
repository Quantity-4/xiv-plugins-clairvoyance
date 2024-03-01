using System.Collections.Generic;

namespace Clairvoyance.server;

public interface IMapEntity
{
    public string MapId { get; }

    public Dictionary<string, PlayerEntity> PlayerEntities { get; set; }
    public int PlayerCount { get; set; }
}

public class MapEntity(string mapId) : IMapEntity
{
    public string MapId { get; } = mapId;

    public Dictionary<string, PlayerEntity> PlayerEntities { get; set; } = new Dictionary<string, PlayerEntity>();
    public int PlayerCount { get; set; } = 0;

    /*
     * Add a new player to the current map object
     */
    public void AddNewPlayer(string playerName, int homeWorldId, float posX, float posY, float posZ)
    {
        // If this player is already on the map - we just go back
        if (CurrentlyInMap(GetUniquePlayerIdentifier(playerName, homeWorldId))) return;

        // If they're a new player, we add them to the map
        PlayerEntity newPlayer = new PlayerEntity(playerName, homeWorldId, this.MapId, posX, posY, posZ);
        this.PlayerEntities.Add(GetUniquePlayerIdentifier(playerName, homeWorldId), newPlayer);
    }

    /*
     * Update function, takes in all player information and appropriate 'thing'
     */
    public void Update(string playerName, int playerHomeWorldId, float posX, float posY, float posZ)
    {
        string uniqueIdentifier = GetUniquePlayerIdentifier(playerName, playerHomeWorldId);

        // Check if the player already exists
        if (!PlayerEntities.ContainsKey(uniqueIdentifier))
        {
            AddNewPlayer(playerName, playerHomeWorldId, posX, posY, posZ);
        }
        else
        {
            // Update an existing player if the said player entity already exists
            var playerToUpdate = PlayerEntities[uniqueIdentifier];
            playerToUpdate.PlayerPosition.PositionX = posX;
            playerToUpdate.PlayerPosition.PositionY = posY;
            playerToUpdate.PlayerPosition.PositionZ = posZ;
        }
    }

    /*
     * Update position for a given player
     */
    public void UpdatePlayerPosition(string playerUniqueIdentifier, float posX, float posY, float posZ)
    {
        var playerToUpdate = GetPlayerEntityFromIdentifier(playerUniqueIdentifier);
        // If it is not found [for some reason] - just exit
        if (playerToUpdate == null) return;

        // Else update the attributes for the entity
        playerToUpdate.PlayerPosition.PositionX = posX;
        playerToUpdate.PlayerPosition.PositionY = posY;
        playerToUpdate.PlayerPosition.PositionZ = posZ;
    }

    /*
     * Check if a player id is in the current map
     * True if there is a match
     */
    public bool CurrentlyInMap(string uniqueIdentifier)
    {
        return PlayerEntities.ContainsKey(uniqueIdentifier);
    }

    /*
     * Returns a PlayerEntity from the uniqueIdentifier given if it exists
     * If the uniqueIdentifier is not found - returns null
     */
    public PlayerEntity? GetPlayerEntityFromIdentifier(string uniqueIdentifier)
    {
        return !CurrentlyInMap(uniqueIdentifier) ? null : this.PlayerEntities[uniqueIdentifier];
    }

    /*
     * Gets the UniqueIdentifier when given a player
     */
    static string GetUniquePlayerIdentifier(PlayerEntity selectedPlayer)
    {
        return $"{selectedPlayer.PlayerName}{selectedPlayer.PlayerHomeWorldId}";
    }

    /*
     * Gets the UniqueIdentifier when given a name, and a home world [overload]
     */
    static string GetUniquePlayerIdentifier(string playerName, int homeWorldId)
    {
        return $"{playerName}{homeWorldId}";
    }
}