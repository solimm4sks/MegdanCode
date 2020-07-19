using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPun
{
#pragma warning disable 649

    [SerializeField] private GameObject knightPrefab;
    [SerializeField] private GameObject rangerPrefab;

    private Vector3Int startPos1 = new Vector3Int(0, -1, 0);
    private Vector3Int startPos2 = new Vector3Int(2, 3, 0);
    private GameManager gameManager;

    private int called = 0;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        StartCoroutine("DelaySpawn");
    }

    private IEnumerator DelaySpawn() {
        yield return new WaitForFixedUpdate();
        SpawnPlayer();
    }

    private void FixedUpdate()
    {
        if (called == 2) //not optimal but doesnt really matter, will get deleted quickly
            Destroy(gameObject);
    }

    private void SpawnPlayer() {
        string json = File.ReadAllText(Application.dataPath + "/SaveData/CurrentDeck.txt");
        DeckInfo deckInfo = JsonUtility.FromJson<DeckInfo>(json);

        PlayerManager player;
        switch (deckInfo.playerClass) {
            case "Knight":
                player = PhotonNetwork.Instantiate("Prefabs/Classes/" + knightPrefab.name, new Vector3(1000, 1000, 1000), Quaternion.identity).GetComponent<PlayerManager>();
                break;
            case "Ranger":
                player = PhotonNetwork.Instantiate("Prefabs/Classes/" + rangerPrefab.name, new Vector3(1000, 1000, 1000), Quaternion.identity).GetComponent<PlayerManager>();
                break;
            default: //hopefully never actually happens
                player = PhotonNetwork.Instantiate("Prefabs/Classes/" + knightPrefab.name, new Vector3(1000, 1000, 1000), Quaternion.identity).GetComponent<PlayerManager>();
                break;
        }

        int ismaster;
        if (PhotonNetwork.IsMasterClient) {
            ismaster = 1;
        }
        else {
            ismaster = 2;
        }
        photonView.RPC("SetPlayerObjName", RpcTarget.AllBuffered, player.photonView.ViewID, ismaster);//so .name is synced
        SetNameInGameManager(player);
        SetPlayerPosition(player);

        CardManager playerCardManager = player.GetComponent<CardManager>();
        playerCardManager.playerManager = player;
        playerCardManager.GetDeck();
    }

    private void SetNameInGameManager(PlayerManager player) {
        if (PhotonNetwork.IsMasterClient) {
            gameManager.photonView.RPC("SetPlayer1", RpcTarget.AllBuffered, player.photonView.ViewID);
        }
        else {
            gameManager.photonView.RPC("SetPlayer2", RpcTarget.AllBuffered, player.photonView.ViewID);
        }
    }

    [PunRPC]
    private void SetPlayerObjName(int id, int num) {
        GameObject playerObj = PhotonView.Find(id).gameObject;
        if (num == 1) {
            playerObj.name = "Player1";
        }
        else {
            playerObj.name = "Player2";
        }

        called++;
    }

    private void SetPlayerPosition(PlayerManager player) {
        if (player.name == "Player1")
            player.SetPlayerPos(startPos1);
        else
            player.SetPlayerPos(startPos2);
    }
}
