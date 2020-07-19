using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpawnable
{
    PhotonView photonView { get; }

    void SetPosition(Vector3Int tile);
    Vector3Int Coords();

    [PunRPC]
    void SetSpawner(int spawner);
}
