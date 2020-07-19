using Photon.Pun;
using System;

public class PlayerInfo : MonoBehaviourPun, IPunObservable
{
    public bool moved;
    public bool attacked;
    public int slowed = 0; //for how many of MY turns am i slowed?
    public int shocked = 0;
    public int hasted = 0;
    public int stunned = 0;
    public int evading = 0;
    public int unshaken = 0;
    public int immovable = 0;

    public int cardsPlayed = 0;
    public int turnDistanceTraveled = 0;
    public int stillStandingTurns = 0; //actualy is haveNotBeenAttackedTurns

    public bool nextAtkSlows = false;
    public bool nextAtkShocks = false;
    public bool nextFreeMove = false;
    public bool nextFreeBasic = false;

    public int maxHp = 12;
    public int health = 12;
    public int attack = 2;
    public int block = 0;
    public int resolve;

    public int basicRange = 1;
    public bool basicIsStraight = true; //not true only for wizard?
    public bool basicIsPiercing = false;
    public int addedBasicRange = 0;
    public float basicRangeMultiplier = 1;
    public int setBasicRange = -1;

    public int moveDist = 1;
    public int addedMoveDist = 0;
    public bool moveIsCharge = false;

    public int addedBasicAttack = 0;
    public float basicAttackMultiplier = 1;
    public int setBasicAttack = -1;

    public int addedDamage = 0;
    public float damageMultiplier = 1;
    public int setDamage = -1;

    public int addedWeaponDamage = 0;
    public float weaponDamageMultiplier = 1;

    private PlayerInfo playerInfo;
    private PlayerManager playerManager;

    private void Awake()
    {
        playerInfo = GetComponent<PlayerInfo>();
        playerManager = GetComponent<PlayerManager>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) {
            stream.SendNext(playerInfo.moved);
            stream.SendNext(playerInfo.attacked);
            stream.SendNext(playerInfo.slowed);
            stream.SendNext(playerInfo.shocked);
            stream.SendNext(playerInfo.hasted);
            stream.SendNext(playerInfo.stunned);
            stream.SendNext(playerInfo.evading);
            stream.SendNext(playerInfo.cardsPlayed);
            stream.SendNext(playerInfo.turnDistanceTraveled);
            stream.SendNext(playerInfo.stillStandingTurns);
            stream.SendNext(playerInfo.nextAtkSlows);
            stream.SendNext(playerInfo.nextAtkShocks);
            stream.SendNext(playerInfo.nextFreeMove);
            stream.SendNext(playerInfo.nextFreeBasic);
            //stream.SendNext(playerInfo.maxHp);
            stream.SendNext(playerInfo.health);                 playerManager.UpdateAHBText();
            //stream.SendNext(playerInfo.attack);
            stream.SendNext(playerInfo.block);                  playerManager.UpdateAHBText();
            stream.SendNext(playerInfo.resolve);
            //stream.SendNext(playerInfo.basicRange);
            stream.SendNext(playerInfo.basicIsStraight);
            stream.SendNext(playerInfo.basicIsPiercing);
            stream.SendNext(playerInfo.moveDist);
            stream.SendNext(playerInfo.addedMoveDist);
            stream.SendNext(playerInfo.moveIsCharge);
            stream.SendNext(playerInfo.addedBasicRange);
            stream.SendNext(playerInfo.basicRangeMultiplier);
            stream.SendNext(playerInfo.setBasicRange);
            stream.SendNext(playerInfo.addedBasicAttack);
            stream.SendNext(playerInfo.basicAttackMultiplier);
            stream.SendNext(playerInfo.setBasicAttack);
            stream.SendNext(playerInfo.addedDamage);
            stream.SendNext(playerInfo.damageMultiplier);
            stream.SendNext(playerInfo.setDamage);
            stream.SendNext(playerInfo.addedWeaponDamage);
            stream.SendNext(playerInfo.weaponDamageMultiplier);

        }
        else {
            playerInfo.moved = (bool)stream.ReceiveNext();
            playerInfo.attacked = (bool)stream.ReceiveNext();
            playerInfo.slowed = (int)stream.ReceiveNext();
            playerInfo.shocked = (int)stream.ReceiveNext();
            playerInfo.hasted = (int)stream.ReceiveNext();
            playerInfo.stunned = (int)stream.ReceiveNext();
            playerInfo.evading = (int)stream.ReceiveNext();
            playerInfo.cardsPlayed = (int)stream.ReceiveNext();
            playerInfo.turnDistanceTraveled = (int)stream.ReceiveNext();
            playerInfo.stillStandingTurns = (int)stream.ReceiveNext();
            playerInfo.nextAtkSlows = (bool)stream.ReceiveNext();
            playerInfo.nextAtkShocks = (bool)stream.ReceiveNext();
            playerInfo.nextFreeMove = (bool)stream.ReceiveNext();
            playerInfo.nextFreeBasic = (bool)stream.ReceiveNext();
            //playerInfo.maxHp = (int)stream.ReceiveNext();
            playerInfo.health = (int)stream.ReceiveNext();              playerManager.UpdateAHBText();
            //playerInfo.attack = (int)stream.ReceiveNext();
            playerInfo.block = (int)stream.ReceiveNext();               playerManager.UpdateAHBText();
            playerInfo.resolve = (int)stream.ReceiveNext();
           // playerInfo.basicRange = (int)stream.ReceiveNext();
            playerInfo.basicIsStraight = (bool)stream.ReceiveNext();
            playerInfo.basicIsPiercing = (bool)stream.ReceiveNext();
            playerInfo.moveDist = (int)stream.ReceiveNext();
            playerInfo.addedMoveDist = (int)stream.ReceiveNext();
            playerInfo.moveIsCharge = (bool)stream.ReceiveNext();
            int abr = (int)stream.ReceiveNext();                if (playerInfo.addedBasicRange != abr) { playerInfo.addedBasicRange = abr; playerManager.SetAllAttackableOutlines(); }
            float brm = (float)stream.ReceiveNext();            if (playerInfo.basicRangeMultiplier != brm) { playerInfo.basicRangeMultiplier = brm; playerManager.SetAllAttackableOutlines(); }
            int sbr  = (int)stream.ReceiveNext();               if (playerInfo.setBasicRange != sbr) { playerInfo.setBasicRange = sbr; playerManager.SetAllAttackableOutlines(); }
            playerInfo.addedBasicAttack = (int)stream.ReceiveNext();
            playerInfo.basicAttackMultiplier = (float)stream.ReceiveNext();
            playerInfo.setBasicAttack = (int)stream.ReceiveNext();
            playerInfo.addedDamage = (int)stream.ReceiveNext();
            playerInfo.damageMultiplier = (float)stream.ReceiveNext();
            playerInfo.setDamage = (int)stream.ReceiveNext();
            playerInfo.addedWeaponDamage = (int)stream.ReceiveNext();
            playerInfo.weaponDamageMultiplier = (float)stream.ReceiveNext();
        }
    }
}
