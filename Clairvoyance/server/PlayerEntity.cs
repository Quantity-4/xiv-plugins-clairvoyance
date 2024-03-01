namespace Clairvoyance.server;

public interface IPlayerPosition
{
    public string MapId { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float PositionZ { get; set; }
}

public class PlayerPosition : IPlayerPosition
{
    public string MapId { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float PositionZ { get; set; }

    public PlayerPosition(string mapId, float startX, float startY, float startZ)
    {
        MapId = mapId;
        this.PositionX = startX;
        this.PositionY = startY;
        this.PositionZ = startZ;
    }
}

public interface IPlayerEntity
{
    public string PlayerUniqueIdentifier { get; set; }
    public string PlayerName { get; set; }
    public int PlayerHomeWorldId { get; set; }
    public IPlayerPosition PlayerPosition { get; set; }
}

public class PlayerEntity(
    string playerName,
    int playerWorldId,
    string mapId,
    float startPosX,
    float startPosY,
    float startPosZ)
    : IPlayerEntity
{
    public string PlayerUniqueIdentifier { get; set; } = $"{playerName}{playerWorldId}";
    public string PlayerName { get; set; } = playerName;
    public int PlayerHomeWorldId { get; set; } = playerWorldId;
    public IPlayerPosition PlayerPosition { get; set; } = new PlayerPosition(mapId, startPosX, startPosY, startPosZ);
}

