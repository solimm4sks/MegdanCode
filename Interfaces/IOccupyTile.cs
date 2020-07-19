using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOccupyTile
{
    PhotonView photonView { get; }
    bool ignoreIfPlayerNotOwner { get; }
}
