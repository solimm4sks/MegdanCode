using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "CardAction/ThrowPommel")]
public class ThrowPommel : CardAction
{
    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction) {
        Vector3Int[] enemyAdjs = GridHelper.GetAdjacentTiles(pm.EnemyCoords());
        List<Vector3Int> standableTiles = new List<Vector3Int>();

        for (int i = 0; i < enemyAdjs.Length; ++i) {
            if (GridHelper.CanStandOn(enemyAdjs[i])) {
                standableTiles.Add(enemyAdjs[i]);
            }
        }

        if (standableTiles.Count > 0) {
            int random = Random.Range(0, standableTiles.Count);
            Pommel pommel = PhotonNetwork.Instantiate(
                "Prefabs/Items/Pommel",
                GridHelper.grid.CellToWorld(standableTiles[random]),
                Quaternion.identity).GetComponent<Pommel>();
            pommel.photonView.RPC("SetSpawner", RpcTarget.All, pm.photonView.ViewID);
        }

        cae.cardPlayState[currentAction] = 1;
        yield return null;
    }
}
