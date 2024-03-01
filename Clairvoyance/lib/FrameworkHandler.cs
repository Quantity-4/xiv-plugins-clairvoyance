using System;
using Clairvoyance.server;
using Dalamud.Game.ClientState.Objects.SubKinds;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using ObjectKind = Dalamud.Game.ClientState.Objects.Enums.ObjectKind;

namespace Clairvoyance.lib;

public class FrameworkHandler : IDisposable
{
    private readonly Server _server;

    public FrameworkHandler()
    {
        // Data Server
        this._server = new Server();
    }

    public unsafe void Update()
    {
        GameObject* localPlayerGameObject = GameObjectManager.GetGameObjectByIndex(0);
        Character* localPlayer = (Character*)localPlayerGameObject;

        for (int i = 1; i != 200; ++i)
        {
            GameObject* gameObject = GameObjectManager.GetGameObjectByIndex(i);
            Character* characterPtr = (Character*)gameObject;


            if (gameObject == null || gameObject == localPlayerGameObject || !gameObject->IsCharacter())
            {
                continue;
            }


            switch ((ObjectKind)characterPtr->GameObject.ObjectKind)
            {
                case ObjectKind.Player:
                    var y = *localPlayerGameObject;
                    this.PlayerHandler(characterPtr, localPlayer);
                    break;
            }
        }
    }

    private unsafe void PlayerHandler(Character* characterPtr, Character* localPlayer)
    {
        // We treat the object ID as a Dalamud Game Object
        Dalamud.Game.ClientState.Objects.Types.GameObject? currentObjectId =
            Helper.ObjectTable.SearchById(characterPtr->GameObject.ObjectID);

        // Validity checks
        if (currentObjectId == null) return;

        // Processing logic [assuming we are within an iteration]
        var playerObject = (PlayerCharacter)currentObjectId;

        // We now have a 'character' as a PlayerCharacter object
        Helper.ChatGui.Print(
            $"Character: {playerObject.Name}\n-- World: {playerObject.HomeWorld.Id}" +
            $"\n-- Position --" +
            $"\n-- X: {playerObject.Position.X}" +
            $"\n-- Y: {playerObject.Position.Y}" +
            $"\n-- Z: {playerObject.Position.Z}"
        );

        // TODO: Implement some sort of map id retrieval system thing... at some point
        AppendPlayerDetailsToServer(
            "limsa", playerObject.Name.TextValue,
            (int)playerObject.HomeWorld.Id,
            playerObject.Position.X,
            playerObject.Position.Y,
            playerObject.Position.Z
        );
    }

    /*
     * Call server update function with appropriate details
     */
    private void AppendPlayerDetailsToServer(string mapId, string playerName, int homeWorldId, float posX, float posY,
        float posZ)
    {
        this._server.Update(mapId, playerName, homeWorldId, posX, posY, posZ);
    }

    private static unsafe bool IsObjectIdInParty(uint objectId)
    {
        GroupManager* groupManager = GroupManager.Instance();
        InfoProxyCrossRealm* infoProxyCrossRealm = InfoProxyCrossRealm.Instance();

        if (groupManager->MemberCount > 0 && groupManager->IsObjectIDInParty(objectId))
        {
            return true;
        }

        if (infoProxyCrossRealm->IsInCrossRealmParty == 0)
        {
            return false;
        }

        foreach (CrossRealmGroup group in infoProxyCrossRealm->CrossRealmGroupArraySpan)
        {
            if (group.GroupMemberCount == 0)
            {
                continue;
            }

            for (int i = 0; i < group.GroupMemberCount; ++i)
            {
                if (group.GroupMembersSpan[i].ObjectId == objectId)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static unsafe bool UnsafeArrayEqual(byte* arr1, byte* arr2, int len)
    {
        ReadOnlySpan<byte> a1 = new(arr1, len);
        ReadOnlySpan<byte> a2 = new(arr2, len);
        return a1.SequenceEqual(a2);
    }

    private static unsafe bool UnsafeArrayEqual(byte[] arr1, byte* arr2, int len)
    {
        fixed (byte* a1 = arr1)
        {
            return UnsafeArrayEqual(a1, arr2, len);
        }
    }


    public void Dispose()
    {
        _server?.Dispose();
    }
}