using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrainingHerbs : Spawnable
{
    public int damage = 1;

    private PlayerManager player1;
    private PlayerManager player2;

    protected override void Start()
    {
        base.Start();
        player1 = GameObject.Find("Player1").GetComponent<PlayerManager>();
        player2 = GameObject.Find("Player2").GetComponent<PlayerManager>();
        player1.TurnStarted += P1TurnStarted;
        player2.TurnStarted += P2TurnStarted;
    }

    [PunRPC]
    public void DealP1DamageRPC() {
        player1.GetDamaged(damage);
    }

    [PunRPC]
    public void DealP2DamageRPC()
    {
        player2.GetDamaged(damage);
    }

    public void P1TurnStarted(PlayerManager p1) {
        if (p1.Coords() == Coords()) {
            Debug.Log("" + (GameInfo.ResolveOnTurn((int)p1.gameManager.turnCount) - 1));
            p1.UpdateResolve(Mathf.Max(0, GameInfo.ResolveOnTurn((int)p1.gameManager.turnCount) - 1));
            photonView.RPC("DealP1DamageRPC", RpcTarget.All);
        }
    }

    public void P2TurnStarted(PlayerManager p2)
    {
        if (p2.Coords() == Coords()) {
            p2.UpdateResolve(Mathf.Max(0, GameInfo.ResolveOnTurn((int)p2.gameManager.turnCount) - 1));
            photonView.RPC("DealP2DamageRPC", RpcTarget.All);
        }
    }


    private void OnDisable()
    {
        player1.TurnStarted -= P1TurnStarted;    
        player2.TurnStarted -= P2TurnStarted;    
    }
}
