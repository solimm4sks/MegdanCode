using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestManager : MonoBehaviourPun
{
    private Vector3Int startingChestLoc = new Vector3Int(1, 1, 0);
    private Chest chest;
    private PlayerManager player1;
    private PlayerManager player2;
    [SerializeField]private PlayerManager damager; //the one that gets/has the effect
    private GameManager gameManager;

    private int numOfChestTypes = 5;
    private int chestIndex = -1;
    private int maxChests = 10000000;
    private int activeEffect = -1;
    private float turnsTillSpawn = 1.5f;
    private int[] chestHp = new int[3] { 4, 5, 6 };
    private int[] effectTurnDuration; //= new int[3] { 2, 2, 2};
    private bool repeatingColors = false;
    private List<int> avalibleChestIds = new List<int>();

    private void Start() {
        gameManager = FindObjectOfType<GameManager>();
        for(int i = 0; i < numOfChestTypes; ++i) 
            avalibleChestIds.Add(i);
        effectTurnDuration = new int[100];
        for (int i = 0; i < 100; ++i) {
            effectTurnDuration[i] = 2;
        }
    }

    public void PlayersLoaded() {
        player1 = GameObject.Find("Player1").GetComponent<PlayerManager>();
        player2 = GameObject.Find("Player2").GetComponent<PlayerManager>();

        if(PhotonNetwork.IsMasterClient)
            SpawnChest(startingChestLoc);
    }

    private void SpawnChest(Vector3Int location) { //will only be called on the master client
        if (chestIndex == maxChests - 1)
            return;
        chestIndex++;

        PushTargetIfOnTop();
        chest = PhotonNetwork.Instantiate("Prefabs/Items/Chest", GridHelper.grid.CellToWorld(location), Quaternion.identity).GetComponent<Chest>();
        chest.photonView.RPC("SetHealth", RpcTarget.All, chestIndex + 5); ///AUTOMATIC HEALTH
        int newId = RandomId();
        chest.photonView.RPC("SetId", RpcTarget.All, newId);
        avalibleChestIds.Remove(newId);
        StartCoroutine("DelaySAAO");

        player1.DamagedSomeone += DamagerChecker; //if DamageTarget() works on as an RPC, we gucci here
        player2.DamagedSomeone += DamagerChecker;
    }

    private IEnumerator DelaySAAO() {
        yield return new WaitForSeconds(0.1f);
        while (player1.state == null)
            yield return null;

        if (player1.state.IsMyTurnGeneral())
            player1.SetAllAttackableOutlines();
        else
            player2.SetAllAttackableOutlines();
    }

    private int RandomId() {
        if (avalibleChestIds.Count == 0)
            return Random.Range(0, numOfChestTypes);
        return repeatingColors ? Random.Range(0, numOfChestTypes) : avalibleChestIds[Random.Range(0, avalibleChestIds.Count)];
    }

    private void PushTargetIfOnTop() {
        List<RaycastHit2D> hits = GridHelper.RaycastTile(startingChestLoc);
        foreach(var hit in hits){
            IAttackable target = hit.transform.parent.GetComponent<IAttackable>();
            if (target != null) {
                PushTarget(target);
            }
        }
    }

    private void PushTarget(IAttackable target) {
        Vector3Int[] adjs = GridHelper.GetAdjacentTiles(target.Coords());
        List<Vector3Int> standables = new List<Vector3Int>();
        foreach(var tile in adjs) {
            if(GridHelper.CanStandOn(tile))
                standables.Add(tile);
        }

        target.MoveTo(standables[Random.Range(0, standables.Count)]);
    }

    public void ChestDestroyed(int id) {
        if (!PhotonNetwork.IsMasterClient)
            return;

        player1.DamagedSomeone -= DamagerChecker;
        player2.DamagedSomeone -= DamagerChecker;

        if(chestIndex < maxChests - 1 && PhotonNetwork.IsMasterClient)
            StartCoroutine("CountdownSpawn");

        StartCoroutine(DelayStartEffect(id));
    }

    private IEnumerator DelayStartEffect(int id) {
        yield return new WaitForEndOfFrame(); //ranger cant apply on hits, with 1 atk

        StartCoroutine("Effect" + id);
        activeEffect = id;
    }

    private void DamagerChecker(PlayerManager pm) {
        damager = pm;
    }

    private IEnumerator CountdownSpawn() { //could use the turn start event (playermanager) here to optimize
        double startingTurn = gameManager.turnCount;
        while (startingTurn + turnsTillSpawn > gameManager.turnCount) {
            yield return null;
        }

        List<Vector3Int> possibleTiles = GridHelper.AllStandableTiles(player1);
        SpawnChest(startingChestLoc); //can also use possibleTiles
    }

    private IEnumerator Effect0() { //red
        PlayerManager cp = damager;
        cp.AlterDamage(1,1,-1);
        cp.AlterBasicDamage(1, 1, -1);

        double startingTurn = gameManager.turnCount;
        int effectDur = effectTurnDuration[chestIndex];
        while (startingTurn + effectDur > gameManager.turnCount) { //-1 so the current turn is counted
            yield return null;
        }

        cp.AlterDamage(-1, 1, -1);
        cp.AlterBasicDamage(-1, 1, -1);
        activeEffect = -1;
    }

    private IEnumerator Effect1() { //blue
        PlayerManager cp = damager;
        cp.AlterMoveDist(1);

        double startingTurn = gameManager.turnCount;
        int effectDur = effectTurnDuration[chestIndex];
        while (startingTurn + effectDur > gameManager.turnCount) { //-1 so the current turn is counted
            yield return null;
        }

        cp.AlterMoveDist(-1);
        activeEffect = -1;
    }

    private bool damaged = false;
    private bool healedTurn = false;
    private IEnumerator Effect2() { //green
        PlayerManager cp = damager;

        cp.DamagedSomeone += EffectedPlayersDamagedEnemy;
        cp.TurnStarted += EffectedPlayersTurnStart;

        double startingTurn = gameManager.turnCount;
        int effectDur = effectTurnDuration[chestIndex];
        while (startingTurn + effectDur > gameManager.turnCount) { //-1 so the current turn is counted
            if (!healedTurn && damaged) {
                cp.GainHealth(3);
                healedTurn = true;
            }
            damaged = false;
            yield return null;
        }

        cp.DamagedSomeone -= EffectedPlayersDamagedEnemy;
        cp.TurnStarted -= EffectedPlayersTurnStart;
        activeEffect = -1;
    }

    private bool disableEnemy = false;
    private IEnumerator Effect3() { //yellow
        PlayerManager cp = damager;
        cp.DamagedSomeone += EffectedPlayersDamagedEnemy;

        double startingTurn = gameManager.turnCount;
        int effectDur = effectTurnDuration[chestIndex];
        while (startingTurn + effectDur > gameManager.turnCount) { //-1 so the current turn is counted
            if (disableEnemy) {
                damager.otherManager.Slow(1);
                damager.otherManager.Shock(1);
                disableEnemy = false;
            }
            yield return null;
        }

        cp.DamagedSomeone -= EffectedPlayersDamagedEnemy;
        activeEffect = -1;
    }

    private bool incResolve = false;
    private IEnumerator Effect4() { //purple
        PlayerManager cp = damager;

        cp.TurnStarted += EffectedPlayersTurnStart;
        cp.UpdateResolve(damager.playerInfo.resolve + 1);
        cp.DrawCards(1);

        double startingTurn = gameManager.turnCount;
        int effectDur = effectTurnDuration[chestIndex];
        while (startingTurn + effectDur > gameManager.turnCount) { //-1 so the current turn is counted
            if (incResolve) {
                cp.UpdateResolve(damager.playerInfo.resolve + 1);
                cp.DrawCards(1);
                incResolve = false;
            }
            yield return null;
        }

        cp.TurnStarted -= EffectedPlayersTurnStart;
        activeEffect = -1;
    }

    private void EffectedPlayersTurnStart(PlayerManager pm) {
        if(pm != damager)
            return;

        if (activeEffect == 4) {
            incResolve = true;
        }
        else if (activeEffect == 2) {
            healedTurn = false;
        }
    }

    private void EffectedPlayersDamagedEnemy(PlayerManager pm) {
        if(pm != damager)
            return;

        if (activeEffect == 3) {
            disableEnemy = true;
        }
        else if (activeEffect == 2) {
            damaged = true;
        }
    }
}

/*
switch (id) {
    case 0:
        StartCoroutine("Effect0");
        break;
    case 1:
        StartCoroutine("Effect1");
        break;
    case 2:
        StartCoroutine("Effect2");
        break;
    case 3:
        StartCoroutine("Effect3");
        break;
    case 4:
        StartCoroutine("Effect4");
        break;
}
*/
