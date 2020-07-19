using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosivePotion : Spawnable, IAttackable, IOccupyTile
{
#pragma warning disable 649

    [SerializeField] private int damage = 2;
    [SerializeField] private GameObject outline;
    private bool exploding = false;
    public bool ignoreIfPlayerNotOwner { get { return false; } }

    private PlayerManager player1;
    private PlayerManager player2;

    private void Awake()
    {
        player1 = GameObject.Find("Player1").GetComponent<PlayerManager>();
        player2 = GameObject.Find("Player2").GetComponent<PlayerManager>();
    }

    private void Update()
    {
        if (player1.Coords() == Coords() || player2.Coords() == Coords())
            Explode();
    }


    public void SetAttackableOutline(bool x) {
        outline.SetActive(x);
    }

    [PunRPC]
    public void MoveToRPC(int x, int y, int z)
    {
        if (!photonView.IsMine)
            return;

        Vector3Int tile = new Vector3Int(x, y, z);
        transform.position = GridHelper.grid.CellToWorld(tile) + spawnOffset;
    }

    public void MoveTo(Vector3Int tile) {
        photonView.RPC("MoveToRPC", RpcTarget.All, tile.x, tile.y, tile.z);
    }

    public void GetDamaged(int x)
    {
        if(!exploding)
            Explode();
    }

    /*
    [PunRPC]
    private void SetExploding() {
        Debug.Log(Identify() + "recieved - Exploding");
        exploding = true;
    }
    */

    private void Explode()
    {
        StartCoroutine("DelayExplosion");
    }

    private IEnumerator DelayExplosion() {
        //photonView.RPC("SetExploding", RpcTarget.All);
        exploding = true;
        //Debug.Log(Identify() + "sending - Exploding");
        //play anim here?
        yield return new WaitForSeconds(0.1f);

        if (!photonView.IsMine)
            yield break;

        List<Vector3Int> tiles = new List<Vector3Int>(GridHelper.GetAdjacentTiles(Coords()));
        tiles.Add(Coords());
        List<RaycastHit2D> hits = GridHelper.RaycastTilesNoEmptyHits(tiles);
        foreach (var hit in hits) {
            IAttackable attackable = hit.transform.parent.GetComponent<IAttackable>();
            if (attackable != null)
                photonView.RPC("DamageEnemyRPC", RpcTarget.All, attackable.photonView.ViewID, damage);
        }

        if (photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    public void DamageEnemyRPC(int id, int dmg) {
        try {
            IAttackable trgt = PhotonView.Find(id).GetComponent<IAttackable>();
            //Debug.Log(Identify() + "sending damage to - " + trgt);
            trgt.GetDamaged(damage);
        }
        catch { 
            
        }
    }

    private string Identify() {
        if (PhotonNetwork.IsMasterClient)
            return "Master(" + gameObject.name + "): ";
        return "Client(" + gameObject.name + "): ";
    }
}
