using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameInfo
{
    public static string GameVersion = "0.1";
    public static int sameCardNum = 2;
    public static int cardsPerClass = 14;
    public static int deckSize = 8;
    //private static int[] resolveOnTurn = { 0, 1, 1, 1, 2, 2, 2, 3 }; 
    private static int[] resolveOnTurn = { 0, 1, 2, 2, 2, 3}; 
    public static string[] classes = { "Knight", "Ranger" };

    public static int ResolveOnTurn(int turn) {
        if (turn >= resolveOnTurn.Length)
            return resolveOnTurn[resolveOnTurn.Length - 1];
        else
            return resolveOnTurn[turn];
    }
}
