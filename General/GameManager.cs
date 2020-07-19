using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;
using System.IO;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPun {

#pragma warning disable 649
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Slider currentTurnSlider;

    [HideInInspector] public PlayerManager player1;
    [HideInInspector] public PlayerManager player2;
    [HideInInspector] public float turnCount = 0.5f;
    public TextMeshProUGUI resolveCountText;
    
    private TextMeshProUGUI turnCountText;
    private ChestManager chestManager;


    private void Awake() {
        turnCountText = GameObject.Find("Canvas/TurnCount").GetComponent<TextMeshProUGUI>();
        resolveCountText = GameObject.Find("Canvas/ResolveCount").GetComponent<TextMeshProUGUI>();
        chestManager = FindObjectOfType<ChestManager>();

        GridHelper.grid = FindObjectOfType<Grid>();
        GridHelper.groundTilemap = GameObject.Find("Environment/Grid/GroundTilemap").GetComponent<Tilemap>();
        GridHelper.waterTilemap = GameObject.Find("Environment/Grid/WaterTilemap").GetComponent<Tilemap>();
        GridHelper.SetAllTileFlags();

        turnCount = 0.5f;
    }

    [PunRPC]
    public void SetPlayer1(int id) => player1 = PhotonView.Find(id).GetComponent<PlayerManager>();
    [PunRPC]
    public void SetPlayer2(int id) => player2 = PhotonView.Find(id).GetComponent<PlayerManager>();

    private void Start() {
        StartCoroutine("WaitForPlayers");
    }

    private IEnumerator WaitForPlayers() {
        while (player1 == null || player2 == null)
            yield return null;

        SetupTurnsOnStart();
        chestManager.PlayersLoaded();
    }

    private void SetupTurnsOnStart() {
        player1.otherManager = player2;
        player1.playerUsernameText.text = player1.photonView.Owner.NickName;
        player2.otherManager = player1;
        player2.playerUsernameText.text = player2.photonView.Owner.NickName;

        if (!PhotonNetwork.IsMasterClient) {
            SwitchTurn(); //same as if player2 pressed End Turn

            player2.EnablePlusR1Button();
        }
    }

    public void SwitchTurn() {
        photonView.RPC("EnableEndTurnButton", RpcTarget.Others);
        endTurnButton.interactable = false;
        photonView.RPC("UpdateTurnCount", RpcTarget.All);

        EndClientPlayerTurn(); //end turn: this client, this player
        photonView.RPC("EndNonClientPlayerTurn", RpcTarget.Others); //end turn: other client, this player
        StartNonClientPlayerTurn(); //start turn: this client, other player
        photonView.RPC("StartClientPlayerTurn", RpcTarget.Others); //start turn: other client, other player
    }

    [PunRPC]
    public void UpdateTurnCount() {
        turnCount += 0.5f;
        turnCountText.text = "Turn: " + turnCount;
        currentTurnSlider.value = (int)turnCount;
    }

    [PunRPC]
    public void EnableEndTurnButton() {
        endTurnButton.interactable = true;
    }

    [PunRPC]
    public void EndClientPlayerTurn() {
        ClientPlayer().EndTurn();
    }

    [PunRPC]
    public void EndNonClientPlayerTurn()
    {
        NonClientPlayer().EndTurn();
    }

    [PunRPC]
    public void StartClientPlayerTurn() {
        ClientPlayer().StartTurn();
    }

    [PunRPC]
    public void StartNonClientPlayerTurn()
    {
        NonClientPlayer().StartTurn();
    }

    private PlayerManager ClientPlayer() {
        if (PhotonNetwork.IsMasterClient)
            return player1;
        else
            return player2;
    }

    private PlayerManager NonClientPlayer() {
        if (PhotonNetwork.IsMasterClient)
            return player2;
        else
            return player1;
    }


    public void GivePlayer2ButtonR() {
        player2.GetRFromButton();     
    }
}
