using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetrifyingPotion : Spawnable, IAttackable, IOccupyTile
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
        List<Vector3Int> tiles = new List<Vector3Int>(GridHelper.GetAdjacentTiles(Coords()));
        tiles.Add(Coords());
        List<IAttackable> attackables = GridHelper.IAttackablesInTiles(tiles);

        foreach (var atk in attackables)
            if (atk is PlayerManager) {
                PlayerManager player = atk as PlayerManager;
                if (player.photonView.IsMine) {
                    player.Stun(1);
                    player.GainBlock(10);
                }
            }

        if (photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
}

/*
    public int dist;
    public bool self = true;

    //private PlayerManager playerManager;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction)
    {
        //playerManager = pm;
        pm.CardAttack(0, false, DamageFunction, new RangeDefined(dist, false, false, false, self), cae, currentAction);
        yield break;
    }

    public void DamageFunction(int dmg, IAttackable target, bool wpatk) {
        if (target is PlayerManager) {
            PlayerManager enemy = target as PlayerManager;
            enemy.GainBlock(10);
            enemy.Stun(1);
        }
    }
*/
