using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Shrine : MonoBehaviour
{
    /*
    private GameObject shrineInactive;
    private GameObject shrineActive;
    private GameManager gameManager;
    private PlayerManager player1;
    private PlayerManager player2;

    private Vector3Int myCoords;
    private Grid grid;
    private bool collectedp1 = false;
    private bool collectedp2 = false;
    private bool open = false;

    private int shrineOpenTurn = 4;

    void Start()
    {
        grid = FindObjectOfType<Grid>();
        shrineInactive = transform.GetChild(0).gameObject;
        shrineActive = transform.GetChild(1).gameObject;
        gameManager = GameObject.FindObjectOfType<GameManager>();
        player1 = gameManager.player1;
        player2 = gameManager.player2;
        myCoords = grid.WorldToCell(transform.position);

        shrineInactive.SetActive(true);    
        shrineActive.SetActive(false);    
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.turnCount < shrineOpenTurn)
            return;
        else if (!open) {
            open = true;
            shrineInactive.SetActive(false);
            shrineActive.SetActive(true);
        }

        if (player1.state.IsMyTurn() && !collectedp1) {
            if (GridHelper.IsTileAdjacent(player1.Coords(), myCoords)) {
                player1.DrawCards(1);
                collectedp1 = true;

                shrineInactive.SetActive(true);
                shrineActive.SetActive(false);
            }
        }

        if (player1.state.IsMyTurn() && collectedp1) {
            shrineInactive.SetActive(true);
            shrineActive.SetActive(false);
        }

        if (!player1.state.IsMyTurn() && collectedp1) {
            collectedp1 = false;
            shrineInactive.SetActive(false);
            shrineActive.SetActive(true);
        }

        if (player2.state.IsMyTurn() && !collectedp2) {
            if (GridHelper.IsTileAdjacent(player2.Coords(), myCoords)) {
                player2.DrawCards(1);
                collectedp2 = true;

                shrineInactive.SetActive(true);
                shrineActive.SetActive(false);
            }
        }

        if (player2.state.IsMyTurn() && collectedp2) {
            shrineInactive.SetActive(true);
            shrineActive.SetActive(false);
        }

        if (!player2.state.IsMyTurn() && collectedp2) {
            collectedp2 = false;
            shrineInactive.SetActive(false);
            shrineActive.SetActive(true);
        }
    }
    */
}
