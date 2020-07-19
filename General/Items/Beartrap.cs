using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beartrap : Spawnable, IOccupyTile
{
    [SerializeField] private int damage = 2;
    public bool ignoreIfPlayerNotOwner { get { return true; }  }

    private void FixedUpdate()
    {
        TryToDamage();
    }

    [PunRPC]
    public override void SetSpawner(int id)
    {
        base.SetSpawner(id);
        photonView.RPC("HideTrapRPC", RpcTarget.Others);
    }

    [PunRPC]
    public void HideTrapRPC() {
        if(!photonView.IsMine)
            GetComponent<SpriteRenderer>().enabled = false;
    }

    private void TryToDamage() {
        if (!photonView.IsMine)
            return;

        List<RaycastHit2D> hits = GridHelper.RaycastTile(Coords());
        bool landedHit = false;
        
        if (hits.Count == 0)
            return;

        foreach (var hit in hits) {
            IAttackable target = hit.transform.parent.GetComponent<IAttackable>();
            if (target == null) {
                continue;
            }

            if (target as Object == spawnerPlayer)
                continue;

            landedHit = true;
            spawnerPlayer.DamageTarget(damage, target); //SHOULD I REALLY BE DOING THIS?????????????????
        }

        if(landedHit)
            PhotonNetwork.Destroy(gameObject);
    }
}
