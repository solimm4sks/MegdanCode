using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingArrows : Spawnable
{
    [SerializeField] private int turnsTillFall = 1;
    [SerializeField] private int damage = 3;
    [SerializeField] private bool isWeaponAtk = true;

    [PunRPC]
    public override void SetSpawner(int id)
    {
        base.SetSpawner(id);
        spawnerPlayer.TurnStarted += OnStartTurn;
    }

    private void OnStartTurn(PlayerManager pm) {
        turnsTillFall--;
        if(turnsTillFall <= 0) {
            DamageOntop();
            spawnerPlayer.TurnStarted -= OnStartTurn;
        }
    }

    private void DamageOntop() {
        List<RaycastHit2D> hits = GridHelper.RaycastTile(Coords());
        foreach (var hit in hits) {
            IAttackable target = hit.transform.parent.GetComponent<IAttackable>();
            if (target == null) {
                PhotonNetwork.Destroy(gameObject);
                return;
            }

            spawnerPlayer.DamageTarget(damage, target, isWeaponAtk); //should i really be doing this??
        }
        PhotonNetwork.Destroy(gameObject);
    }
}
