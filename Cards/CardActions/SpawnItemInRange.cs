using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Pun;

[CreateAssetMenu(menuName = "CardAction/SpawnItemInRange")]
public class SpawnItemInRange : CardAction
{
    public string itemName;
    public int range;
    public bool checkCanStandOn = false;
    public bool checkOccupied = false;
    public bool straight;
    public bool piercing;
    public bool basicRange;
    private List<Vector3Int> tiles;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction)
    {
        tiles = GridHelper.TilesInRange(pm.Coords(), new RangeDefined(range, straight, piercing, basicRange, null, StandableChecker));

        foreach (var tile in tiles) 
            GridHelper.groundTilemap.SetColor(tile, GridHelper.spawnTileColor);

        bool spawned = false;

        while(true) {
            if(Input.GetMouseButtonDown(0)) {
                Vector3Int mousePos = pm.MouseCoords();
                //add raycast here if u want, and check in if
                if(GridHelper.TileInList(mousePos, tiles)) {
                    Spawnable acItem = PhotonNetwork.Instantiate("Prefabs/Items/" + itemName, GridHelper.grid.CellToWorld(mousePos), Quaternion.identity).GetComponent<Spawnable>();
                    acItem.photonView.RPC("SetSpawner", RpcTarget.All, pm.photonView.ViewID);
                    spawned = true;
                    break;
                }
                else {
                    spawned = false;
                    break;
                }
                
            }
            yield return null;
        }

        foreach(var tile in tiles)
            GridHelper.groundTilemap.SetColor(tile, Color.white);
        cae.cardPlayState[currentAction] = spawned ? 1 : 2;
    }

    public bool StandableChecker(Vector3Int tile) {
        bool ok = true;

        if (checkCanStandOn)
            ok &= GridHelper.CanStandOn(tile);
        if (checkOccupied)
            ok &= !GridHelper.TileOccupied(tile);

        List<RaycastHit2D> hits = GridHelper.RaycastTile(tile); //check item already on tile
        foreach(var hit in hits)
            ok &= !(hit.transform.parent.name == itemName + "(Clone)" || hit.transform.name == itemName + "(Clone)");

        return ok;
    }
}
