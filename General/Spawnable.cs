using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnable : MonoBehaviourPun
{
    protected Vector3 spawnOffset = new Vector3(0f, 0.136f);
    protected PlayerManager spawnerPlayer;

    protected virtual void Start() {
        FixPosition();
    }

    protected virtual void FixPosition() {
        transform.position += spawnOffset;
    }

    public virtual void SetPosition(Vector3Int tile)
    {
        transform.position = GridHelper.grid.CellToWorld(tile) + spawnOffset;
    }

    [PunRPC]
    public virtual void SetSpawner(int id)
    {
        spawnerPlayer = PhotonView.Find(id).GetComponent<PlayerManager>();
    }

    public Vector3Int Coords() {
        return GridHelper.grid.WorldToCell(transform.position);
    }
}
