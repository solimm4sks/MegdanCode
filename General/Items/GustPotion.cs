using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class GustPotion : Spawnable, IAttackable, IOccupyTile
{
#pragma warning disable 649

    [SerializeField] private GameObject outline;
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

    public void SetAttackableOutline(bool x)
    {
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

    public void MoveTo(Vector3Int tile)
    {
        photonView.RPC("MoveToRPC", RpcTarget.All, tile.x, tile.y, tile.z);
    }

    public void GetDamaged(int x)
    {
        Explode();
    }

    private void Explode()
    {
        Vector3Int currCords = Coords();

        for (int i = 0; i < 6; ++i) {
            Vector3Int adjTile = GridHelper.GetTileInDirection(currCords, i);
            List<RaycastHit2D> hits = GridHelper.RaycastTile(adjTile);
            foreach(var hit in hits){
                IAttackable target = hit.transform.parent.GetComponent<IAttackable>();
                if (target != null) {
                    Vector3Int newTile = GridHelper.GetTileInDirection(adjTile, i);
                    if (GridHelper.CanStandOn(newTile)) {
                        target.MoveTo(newTile);
                    }
                }
            }
        }

        if (photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
