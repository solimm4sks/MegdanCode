using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    PhotonView photonView { get; }

    void GetDamaged(int dmg);
    void SetAttackableOutline(bool x);
    void MoveTo(Vector3Int tile);
    void MoveToRPC(int x, int y, int z);
    Vector3Int Coords();
}
