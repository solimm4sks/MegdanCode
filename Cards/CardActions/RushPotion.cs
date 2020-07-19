using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/RushPotion")]
public class RushPotion : CardAction
{
    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction)
    {
        List<Vector3Int> tiles = new List<Vector3Int>(GridHelper.GetAdjacentTiles(pm.Coords()));
        tiles.Add(pm.Coords());

        List<RaycastHit2D> hits = GridHelper.RaycastTilesNoEmptyHits(tiles);
        foreach (var hit in hits) {
            PlayerManager hitPlayer = hit.transform.parent.GetComponent<PlayerManager>();
            if (hitPlayer != null) {
                ApplyEffects(hitPlayer);
            }
        }

        cae.cardPlayState[currentAction] = 1;
        yield break;
    }

    void ApplyEffects(PlayerManager player) {
        player.Hasten(1);
        player.RefreshBasic();
    }
}
