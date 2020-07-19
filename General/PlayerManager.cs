using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;
using Photon.Pun;
using JetBrains.Annotations;
using Photon.Realtime;
using System.Security.Cryptography;
using UnityEngine.UI;
using System.Linq.Expressions;

abstract public class PlayerManager : StateMachine, IAttackable, ITerrain {

#pragma warning disable 649
    //scripts
    [HideInInspector] public PlayerManager otherManager;
    [HideInInspector] public CardManager cardManager;
    [HideInInspector] public GameManager gameManager;

    [HideInInspector] public Color atkRangeColor = new Color(237f / 255, 109f / 255, 107f / 255, 1f); //color of atk range hightlight
    [HideInInspector] public Vector3 cellOffset = new Vector3(0, -0.2f, 0); //player portrait offsett from cell positions

    //objects
    [HideInInspector] public Camera myCamera;
    private TextMeshPro healthText;
    private TextMeshPro attackText;
    private TextMeshPro blockText;
    private GameObject blockIcon;
    private GameObject portraitOutline;
    [HideInInspector] public TextMeshPro playerUsernameText;
    private Button p2RButton;

    public PlayerInfo playerInfo;

    //events
    public delegate void DamageEnemyDel(PlayerManager pm);
    public event DamageEnemyDel DamagedSomeone;
    public delegate void TurnStartDel(PlayerManager pm);
    public event TurnStartDel TurnStarted;


    private void Awake() {
        cardManager = GetComponent<CardManager>();
        playerInfo = GetComponent<PlayerInfo>();

        gameManager = FindObjectOfType<GameManager>();
        myCamera = FindObjectOfType<Camera>();

        portraitOutline = GameObject.Find(gameObject.name + "/PlayerIcon/portraitOutline");
        healthText = GameObject.Find(gameObject.name + "/PlayerIcon/Health").GetComponent<TextMeshPro>();
        attackText = GameObject.Find(gameObject.name + "/PlayerIcon/Attack").GetComponent<TextMeshPro>();
        blockText = GameObject.Find(gameObject.name + "/PlayerIcon/Block").GetComponent<TextMeshPro>();
        blockIcon = GameObject.Find(gameObject.name + "/PlayerIcon/BlockIcon");
        playerUsernameText = GameObject.Find(gameObject.name + "/PlayerIcon/PlayerName").GetComponent<TextMeshPro>();
        p2RButton = GameObject.Find("Canvas/Plus1RButtonParent/Plus1RButton").GetComponent<Button>();
    }

    protected virtual void Start() {
        SetupAHB();
    }

    protected virtual void Update() {
        if (!photonView.IsMine)
            return;
        BasicMoveDetect();
        ShowAttackDetect();
        Attack();

        if (playerInfo.unshaken > 0) { //really fucking heavy, but idk a better solution tbh
            CleanseDisables();
        }

        try {
            p2RButton.interactable = state.IsMyTurnGeneral();
        }
        catch {
            //p2RButton.interactable = false;
        }
    }

    public void EnablePlusR1Button() {
        p2RButton.gameObject.SetActive(true);
    }

    public void GetRFromButton() { //since only p2 can click the button, there is no need for IsMine check
        UpdateResolve(playerInfo.resolve + 1);
        p2RButton.gameObject.SetActive(false);
    }

    
    //<turnHandling>
    public void StartTurn() {
        SetState(new StartTurn(this));
        if (photonView.IsMine)
            TurnStarted?.Invoke(this);
    }

    public void SetAllAttackableOutlines() {
        bool val = !playerInfo.attacked && (state != null ?  state.IsMyTurn() : true);
        photonView.RPC("SetAllAORPC", RpcTarget.All, val); //ismyturn = !enemyturn, fix this, dont show basic atk outline for card atks
    }

    [PunRPC]
    public void SetAllAORPC(bool val) {
        List<IAttackable> targets = GridHelper.IAttackablesInTiles(GridHelper.AllTiles());
        foreach (var target in targets) {
            target.SetAttackableOutline(val && TargetInBasicRange(target.Coords()));
        }
    }

    public void EndTurn() {
        SetState(new EndTurn(this));
    }
    //</turnHandline>

    //<resolve>-----------
    [PunRPC]
    public void UpdateResolveRPC(int x) {
        if (photonView.IsMine) {
            playerInfo.resolve = x;
        }
        gameManager.resolveCountText.text = "Resolve: " + playerInfo.resolve;
    }

    public void UpdateResolve(int x) {
        photonView.RPC("UpdateResolveRPC", RpcTarget.All, x);
    }
    //</resolve>----------

    #region effects
    //<effects>------------
    public void CleanseDisables() {
        Unslow();
        Unshock();
        Unstun();
    }

    [PunRPC]
    public void AlterMoveDistRPC(int x)
    {
        if (!photonView.IsMine)
            return;

        playerInfo.addedMoveDist += x;
    }

    public void AlterMoveDist(int addedMoveDist)
    {
        photonView.RPC("AlterMoveDistRPC", RpcTarget.All, addedMoveDist);
    }

    [PunRPC]
    public void ImmovableRPC(int x)
    {
        if (!photonView.IsMine)
            return;
        playerInfo.immovable = Mathf.Max(playerInfo.immovable, x);
    }

    public void Immovable(int turns)
    {
        photonView.RPC("ImmovableRPC", RpcTarget.All, turns);
    }

    [PunRPC]
    public void UnimmovableRPC()
    {
        if (!photonView.IsMine)
            return;
        playerInfo.immovable = 0;
    }

    public void Unimmovable()
    {
        photonView.RPC("UnimmovableRPC", RpcTarget.All);
    }

    [PunRPC]
    public void UnshakenRPC(int x)
    {
        if (!photonView.IsMine)
            return;
        playerInfo.unshaken = Mathf.Max(playerInfo.unshaken, x);
    }

    public void Unshaken(int turns)
    {
        photonView.RPC("UnshakenRPC", RpcTarget.All, turns);
    }

    [PunRPC]
    public void DeunshakenRPC()
    {
        if (!photonView.IsMine)
            return;
        playerInfo.unshaken = 0;
    }

    public void Deunshaken()
    {
        photonView.RPC("DeunshakenRPC", RpcTarget.All);
    }


    [PunRPC]
    public void AlterBasicRangeRPC(int x, float y, int z)
    {
        if (!photonView.IsMine)
            return;

        playerInfo.basicRange += x;
        playerInfo.basicRangeMultiplier *= y;
        playerInfo.setBasicRange = z;
    }

    public void AlterBasicRange(int addedBasicRange, float basicRangeMultiplier, int setBasicRange)
    {
        photonView.RPC("AlterBasicRangeRPC", RpcTarget.All, addedBasicRange, basicRangeMultiplier, setBasicRange);
        SetAllAttackableOutlines();
    }

    [PunRPC]
    public void AlterWeaponDamageRPC(int x, float y)
    {
        if (!photonView.IsMine)
            return;
        playerInfo.addedWeaponDamage += x;
        playerInfo.weaponDamageMultiplier *= y;
    }

    public void AlterWeaponDamage(int addedWeaponDamage, float weaponDamageMultiplier)
    {
        photonView.RPC("AlterWeaponDamageRPC", RpcTarget.All, addedWeaponDamage, weaponDamageMultiplier);
    }

    [PunRPC]
    public void AlterDamageRPC(int x, float y, int z)
    {
        if (!photonView.IsMine)
            return;
        playerInfo.addedDamage += x;
        playerInfo.damageMultiplier *= y;
        playerInfo.setDamage = z;
    }

    public void AlterDamage(int addedDamage, float damageMultiplier, int setDamage)
    {
        if (!photonView.IsMine)
            return;
        photonView.RPC("AlterDamageRPC", RpcTarget.All, addedDamage, damageMultiplier, setDamage);
    }

    [PunRPC]
    public void AlterBasicDamageRPC(int x, float y, int z) {
        if (!photonView.IsMine)
            return;
        playerInfo.addedBasicAttack += x;
        playerInfo.basicAttackMultiplier *= y;
        playerInfo.setBasicAttack = z;
    }

    public void AlterBasicDamage(int addedBasicAttack, float basicAttackMultiplier, int setBasicAttack) {
        photonView.RPC("AlterBasicDamageRPC", RpcTarget.All, addedBasicAttack, basicAttackMultiplier, setBasicAttack);
    }

    [PunRPC]
    public void ShockRPC(int x) {
        if (!photonView.IsMine)
            return;
        playerInfo.shocked = Mathf.Max(playerInfo.shocked, x);
    }

    public void Shock(int turns) {
        photonView.RPC("ShockRPC", RpcTarget.All, turns);
    }

    [PunRPC]
    public void UnshockRPC() {
        if (!photonView.IsMine)
            return;
        playerInfo.shocked = 0;
    }

    public void Unshock() {
        photonView.RPC("UnshockRPC", RpcTarget.All);
    }
    
    [PunRPC]
    public void SlowRPC(int x)
    {
        if (!photonView.IsMine)
            return;
        playerInfo.slowed = Mathf.Max(playerInfo.slowed, x);
    }

    public void Slow(int turns) {
        photonView.RPC("SlowRPC", RpcTarget.All, turns);
    }

    [PunRPC]
    public void UnslowRPC()
    {
        if (!photonView.IsMine)
            return;
        playerInfo.slowed = 0;
    }
    public void Unslow() {
        photonView.RPC("UnslowRPC", RpcTarget.All);
    }

    [PunRPC]
    public void HastenRPC(int x)
    {
        if (!photonView.IsMine)
            return;
        playerInfo.hasted = Mathf.Max(playerInfo.hasted, x);
    }

    public void Hasten(int turns) {
        photonView.RPC("HastenRPC", RpcTarget.All, turns);
    }

    [PunRPC]
    public void UnhastenRPC()
    {
        if (!photonView.IsMine)
            return;
        playerInfo.hasted = 0;
    }

    public void Unhasten() {
        photonView.RPC("UnhastenRPC", RpcTarget.All);
    }

    [PunRPC]
    public void StunRPC(int x)
    {
        if (!photonView.IsMine)
            return;
        playerInfo.stunned = Mathf.Max(playerInfo.stunned, x);
    }
    public void Stun(int turns)
    {
        photonView.RPC("StunRPC", RpcTarget.All, turns);
    }

    [PunRPC]
    public void UnstunRPC()
    {
        if (!photonView.IsMine)
            return;
        playerInfo.stunned = 0;
    }
    public void Unstun()
    {
        photonView.RPC("UnstunRPC", RpcTarget.All);
    }

    [PunRPC]
    public void AddEvadeRPC(int x)
    {
        if (!photonView.IsMine)
            return;
        playerInfo.evading = Mathf.Max(playerInfo.evading, x);
    }
    public void AddEvade(int turns) {
        photonView.RPC("AddEvadeRPC", RpcTarget.All, turns);
    }
    
    [PunRPC]
    public void UnevadeRPC()
    {
        if (!photonView.IsMine)
            return;
        playerInfo.evading = 0;
    }
    public void Unevade() {
        photonView.RPC("UnevadeRPC", RpcTarget.All);
    }
    
    [PunRPC]
    public void RefreshMoveRPC()
    {
        if (!photonView.IsMine)
            return;
        playerInfo.moved = false;
    }

    public void RefreshMove() {
        photonView.RPC("RefreshMoveRPC", RpcTarget.All);
    }
    
    [PunRPC]
    public void RefreshBasicRPC()
    {
        if (!photonView.IsMine)
            return;
        playerInfo.attacked = false;
        SetAllAttackableOutlines();
    }

    public void RefreshBasic() {
        photonView.RPC("RefreshBasicRPC", RpcTarget.All);
    }

    [PunRPC]
    public void SetDistanceTraveledRPC(int x) {
        if (!photonView.IsMine)
            return;
        playerInfo.turnDistanceTraveled = x;
    }

    public void SetDistanceTraveled(int dist) {
        photonView.RPC("SetDistanceTraveledRPC", RpcTarget.All, dist);
    }

    [PunRPC]
    public void SetStillStandingTurnsRPC(int x) {
        if (!photonView.IsMine)
            return;

        playerInfo.stillStandingTurns = x;
    }

    public void SetStillStandingTurns(int numTurns) {
        photonView.RPC("SetStillStandingTurnsRPC", RpcTarget.All, numTurns);
    }
    //</effects>---------
    #endregion


    //<cards>-----------
    [PunRPC]
    public void DrawCardsRPC(int x) {
        if (!photonView.IsMine)
            return;
        cardManager.DrawHandCards(x);
    }

    public void DrawCards(int x) {
        photonView.RPC("DrawCardsRPC", RpcTarget.All, x);
    }

    public delegate void SelectTileDel(Vector3Int? selectedTile);

    public void CardSelectTile(RangeDefined range, SelectTileDel SelectTileFunction) {
        CardSelectTile(Coords(), range, SelectTileFunction);
    }

    public void CardSelectTile(Vector3Int start, RangeDefined range, SelectTileDel SelectTileFunction) {
        List<Vector3Int> tiles;
        if (range.basicRange)
            tiles = TilesInBasic(range.allowedDirections);
        else
            tiles = GridHelper.TilesInRange(start, range);

        StartCoroutine(CardSelectTileRoutine(tiles, SelectTileFunction));
    }

    private IEnumerator CardSelectTileRoutine(List<Vector3Int> tiles, SelectTileDel SelectTileFunction) {
        GridHelper.SetGroundTilesColor(tiles, GridHelper.spawnTileColor);

        Vector3Int? selectedTile = null;

        while (true) {
            if (Input.GetMouseButtonDown(0)) {
                Vector3Int mouseCoords = MouseCoords();
                if (!GridHelper.TileInList(mouseCoords, tiles)) {
                    break;
                }

                selectedTile = mouseCoords;
                break;
            }
            yield return null;
        }

        GridHelper.SetGroundTilesColor(tiles, Color.white);
        SelectTileFunction(selectedTile != null && GridHelper.groundTilemap.HasTile(selectedTile.Value) ? selectedTile : null);
    }

    public delegate void DealDamageDel(int dmg, IAttackable target, bool weaponAtk);

    public void CardAttack(int cardDmg, bool weaponAtk, DealDamageDel DamageFunction, RangeDefined range, CardActionExecutor cae, int currentAction) {
        if (range.IsTileValid == null) //this gets done in  TilesInAttack anyway, but whatever
            range.IsTileValid = GridHelper.CanAttackThrough;

        List<Vector3Int> tiles;
        if (range.basicRange) {
            tiles = TilesInBasic(range.allowedDirections);
        }
        else {
            tiles = TilesInAttack(range);
        }

        foreach (var tile in tiles) {
            GridHelper.groundTilemap.SetColor(tile, atkRangeColor);
        }

        List<RaycastHit2D> hits = GridHelper.RaycastTiles(tiles);
        List<IAttackable> targets = new List<IAttackable>();
        foreach (var hit in hits) {
            IAttackable target = hit.collider.gameObject.transform.parent.GetComponent<IAttackable>();
            if (target != null) {
                target.SetAttackableOutline(true);
                targets.Add(target);
            }
        }

        StartCoroutine(CardAttackRoutine(
            tiles, 
            (range.basicRange && playerInfo.basicIsPiercing && playerInfo.basicIsStraight) || 
                (!range.basicRange && range.straight && range.piercing), 
            cae, 
            currentAction, 
            cardDmg, 
            weaponAtk, 
            DamageFunction, 
            targets));
    }

    public bool CanMove() { //at all
        return state.CanMove();
    }

    public void CardAttack(int cardDmg, RangeDefined range, CardActionExecutor cae, int currentAction) {
        CardAttack(cardDmg, false, DamageTarget, range, cae, currentAction);
    }

    public void CardAttack(int cardDmg, bool weaponAtk, RangeDefined range, CardActionExecutor cae, int currentAction)
    {
        CardAttack(cardDmg, weaponAtk, DamageTarget, range, cae, currentAction);
    }

    private IEnumerator CardAttackRoutine(List<Vector3Int> tiles, bool piercing, CardActionExecutor cae, int currentAction, int cardDmg, bool weaponAtk, DealDamageDel DamageFunction, List<IAttackable> targets) {
        while (true) {
            if (Input.GetMouseButtonDown(0)) {
                if (!GridHelper.TileInList(MouseCoords(), tiles)) {
                    CardAttackCancel();
                    break;
                }

                List<RaycastHit2D> hits = GridHelper.RaycastTile(MouseCoords());
                if (hits.Count == 0) {
                    CardAttackCancel();
                    break;
                }

                bool landedHit = false;
                foreach (var hit in hits) {
                    IAttackable target = hit.transform.parent.GetComponent<IAttackable>();
                    if (target == null) {
                        continue;
                    }
                    landedHit = true;

                    //actually hit something
                    if (piercing) {
                        int targetDir = -1;
                        int basicDist = BasicRange();
                        List<Vector3Int>[] tilesInDir = new List<Vector3Int>[6];

                        for (int i = 0; i < 6; ++i) {
                            tilesInDir[i] = GridHelper.TilesInDirection(Coords(), i, basicDist, true, GridHelper.CanAttackThrough);
                            if (GridHelper.TileInList(target.Coords(), tilesInDir[i])) {
                                targetDir = i;
                                break;
                            }
                        }

                        List<RaycastHit2D> hits1 = GridHelper.RaycastTilesNoEmptyHits(tilesInDir[targetDir]);
                        foreach (var dirHit in hits1) {
                            IAttackable piercedTarget = dirHit.transform.parent.GetComponent<IAttackable>();
                            if (piercedTarget != null) {
                                DamageFunction(cardDmg, piercedTarget, weaponAtk);
                                target.SetAttackableOutline(false);
                            }
                        }
                    }
                    else {
                        DamageFunction(cardDmg, target, weaponAtk);
                    }

                    break;
                }

                if (!landedHit) {
                    CardAttackCancel();
                    break;
                }

                CardAttackHide();
                cae.cardPlayState[currentAction] = 1;
                break;

            }
            else if (Input.GetMouseButtonDown(1)) {
                foreach (var tile in tiles) 
                    GridHelper.groundTilemap.SetColor(tile, Color.white);
                cae.cardPlayState[currentAction] = 2;
                break;
            }
            yield return null;
        }
        
        void CardAttackHide() {
            foreach (var tile in tiles)
                GridHelper.groundTilemap.SetColor(tile, Color.white);
            foreach (var target in targets)
                target.SetAttackableOutline(false);
        }

        void CardAttackCancel() {
            CardAttackHide();
            cae.cardPlayState[currentAction] = 2;
        }
    }
    //</cards>----------

    //<basicAtk>-------------
    public int BasicCost() {
        return playerInfo.nextFreeBasic ? 0 : (1 + (playerInfo.shocked > 0 ? 1 : 0) - (playerInfo.hasted > 0 ? 1 : 0));
    }
    
    public void SetAttackableOutline(bool x) {
        portraitOutline.SetActive(x);
    }

    private void ShowAttackDetect()
    {
        if (Input.GetMouseButtonDown(1) && Coords() == MouseCoords()) {
            StartCoroutine(state.ShowAttackRange());
        }
        else if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
            StartCoroutine(state.HideAttack());
        }
    }

    private void Attack() {
        if (playerInfo.attacked)
            return;
        if (playerInfo.resolve < BasicCost())
            return;
        if (Input.GetMouseButtonDown(0) && TargetInBasicRange(MouseCoords())) {
            StartCoroutine(state.Attack());
        }
    }

    public void AttackBasedOnPiercing(IAttackable target) {
        if (!playerInfo.basicIsStraight || !playerInfo.basicIsPiercing) { //non piercing atk
            DamageTarget(BasicDamage(), target, true);
        }
        else { ///this only for piercing atks (so ranger)
            int targetDir = -1;
            int basicDist = BasicRange();
            List<Vector3Int>[] tilesInDir = new List<Vector3Int>[6];

            for (int i = 0; i < 6; ++i) {
                tilesInDir[i] = GridHelper.TilesInDirection(Coords(), i, basicDist, true, GridHelper.CanAttackThrough);
                if (GridHelper.TileInList(target.Coords(), tilesInDir[i])) {
                    targetDir = i;
                    break;
                }
            }

            List<RaycastHit2D> hits = GridHelper.RaycastTilesNoEmptyHits(tilesInDir[targetDir]);
            foreach (var hit1 in hits) {
                IAttackable piercedTarget = hit1.transform.parent.GetComponent<IAttackable>();
                if (piercedTarget != null) {
                    DamageTarget(BasicDamage(), piercedTarget, true);
                }
            }
        }
    }

    /*
    private void Attack() {
        if (Input.GetMouseButtonDown(1) && Coords() == MouseCoords()) {
            StartCoroutine(state.Attack());
        }
        else if (Input.GetMouseButtonDown(1)) {
            StartCoroutine(state.HideAttack());
        }
        else if(Input.GetMouseButtonDown(0)){
            bool hitSomething = false;

            RaycastHit2D hit = GridHelper.RaycastTile(MouseCoords());
            if (hit) {
                IAttackable target = hit.transform.parent.GetComponent<IAttackable>();
                if (target != null && TargetInBasicRange(target.Coords())) {
                    StartCoroutine(state.LandAttack(target));
                    hitSomething = true;
                }
            }

            if (!hitSomething) {
                StartCoroutine(state.HideAttack());
            }
        }
    }
    */

    public void HideAttack() {
        StartCoroutine(state.HideAttack());
    }

    //</basicAtk>-----------------

    //<damage>-------------
    public int BasicDamage() {
        return (int)(playerInfo.setBasicAttack == -1 ? (playerInfo.attack + playerInfo.addedBasicAttack) * playerInfo.basicAttackMultiplier : playerInfo.setBasicAttack);
    }

    [PunRPC]
    public void DamageTargetRPC(int dmg, int viewId, bool weaponAttack) {
        DamagedSomeone?.Invoke(this);

        if (playerInfo.setDamage != -1) {
            dmg = playerInfo.setDamage;
        }
        else {
            if (weaponAttack)
                dmg = (int)((dmg + playerInfo.addedWeaponDamage) * playerInfo.weaponDamageMultiplier);
            dmg = (int)((dmg + playerInfo.addedDamage) * playerInfo.damageMultiplier);
        }
        IAttackable target = PhotonView.Find(viewId).GetComponent<IAttackable>();
        if (target == null) {
            Debug.LogError("Invalid photon view id, does not represent IAttackable.", this);
            return;
        }
        target.GetDamaged(dmg);
    }

    public void DamageTarget(int dmg, IAttackable target, bool weaponAttack) {
        photonView.RPC("DamageTargetRPC", RpcTarget.All, dmg, target.photonView.ViewID, weaponAttack);
    }

    public void DamageTarget(int dmg, IAttackable target) {
        DamageTarget(dmg, target, false);
    }

    public virtual void GetDamaged(int dmg) {
        if (!photonView.IsMine)
            return;

        if (playerInfo.evading > 0) {
            playerInfo.evading--;
            return;
        }

        SetStillStandingTurns(0);

        int leftDmg = dmg - playerInfo.block;
        GainBlock(-dmg);
        if (leftDmg > 0) {
            GainHealth(-leftDmg);
        }

        RemoveBlock();
        UpdateAHBText();
    }
    //</damage>------------

    //<health>--------------
    [PunRPC]
    public void GainHealthRPC(int x) {
        playerInfo.health = Mathf.Min(playerInfo.health + x, playerInfo.maxHp);
        UpdateAHBText();
    }

    public void GainHealth(int x) {
        photonView.RPC("GainHealthRPC", RpcTarget.All, x);
    }

    [PunRPC]
    public void SetHealthRPC(int x) {
        playerInfo.health = x;
        UpdateAHBText();
    }
    public void SetHealth(int x) {
        photonView.RPC("SetHealthRPC", RpcTarget.All, x);
    }
    //</health>---------------

    //<block>-------------
    [PunRPC]
    public void GainBlockRPC(int x) {
        if (!photonView.IsMine)
            return;

        playerInfo.block = Mathf.Max(0, playerInfo.block + x);
    }

    public void GainBlock(int x) {
        photonView.RPC("GainBlockRPC", RpcTarget.All, x);
    }

    private void RemoveBlock() {
        GainBlock(-playerInfo.block);
    }

    public void UpdateHealthText(int hp) {
        healthText.text = "" + hp;
    }

    public void UpdateAttackText(int atk) {
        attackText.text = "" + atk;
    }

    public void UpdateBlockText(int blk) {
        blockText.text = "" + blk;
        if (blk > 0) {
            blockText.gameObject.SetActive(true);
            blockIcon.SetActive(true);
        }
        else {
            blockText.gameObject.SetActive(false);
            blockIcon.SetActive(false);
        }
    }
    //</block>------------

    //<Text>--------------
    private void SetupAHB() {
        SetClassProperties();
        playerInfo.health = playerInfo.maxHp;
        playerInfo.block = 0;

        healthText.gameObject.SetActive(true);
        attackText.gameObject.SetActive(true);
        blockText.gameObject.SetActive(false);
        blockIcon.SetActive(false);

        UpdateAHBText();
    }

    [PunRPC]
    public void UpdateAHBTextRPC() {
        UpdateHealthText(playerInfo.health);
        UpdateAttackText(BasicDamage());
        UpdateBlockText(playerInfo.block);
    }

    public void UpdateAHBText() { //Update AttackHealthBlock Text
        photonView.RPC("UpdateAHBTextRPC", RpcTarget.All);
    }
    //</Text>-------------

    //<movement>--------------
    public int MoveCost() {
        return playerInfo.nextFreeMove ? 0 : (1 + (playerInfo.slowed > 0 ? 1 : 0) - (playerInfo.hasted > 0 ? 1 : 0));
    }

    public int MoveDistance() {
        return playerInfo.moveDist + playerInfo.addedMoveDist;
    }

    private void BasicMoveDetect() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3Int mouseCoords = MouseCoords();

            if (Coords() == mouseCoords) {
                StartCoroutine(state.ShowBasicMove());
            }
            else {
                StartCoroutine(state.BasicMove(mouseCoords));
            }
        }
    }

    public virtual void _BasicMoveShow() {
        List<Vector3Int> tiles = TilesInMove();
        foreach (var tile in tiles) {
            if (GridHelper.CanStandOn(tile))
                GridHelper.groundTilemap.SetColor(tile, GridHelper.standableTileColor);
        }
    }

    public delegate void MoveDelegate();
    public event MoveDelegate Moved;

    public virtual void _BasicMove(Vector3Int destination) {
        List<Vector3Int> tiles = TilesInMove();
        foreach (var tile in tiles) {
            if (GridHelper.CanStandOn(tile)) {
                GridHelper.groundTilemap.SetColor(tile, Color.white);
                if (tile == destination && playerInfo.resolve >= MoveCost()) {
                    MoveTo(destination);
                    UpdateResolve(playerInfo.resolve - MoveCost());
                    playerInfo.moved = true;
                    playerInfo.nextFreeMove = false;
                }
            }
        }
    }

    [PunRPC]
    public void MoveToRPC(int x, int y, int z)
    {
        if (!photonView.IsMine)
            return;

        Vector3Int tile = new Vector3Int(x, y, z);
        if (GridHelper.CanStandOn(tile))
            transform.position = GridHelper.grid.CellToWorld(tile) + cellOffset;
    }

    public void MoveTo(Vector3Int destination) {
        if (!state.IsMyTurnGeneral() && playerInfo.immovable > 0) //so i dont get hit by like heavy barrage
            return;

        int x = GridHelper.TileDistance(Coords(), destination);
        //Debug.Log("DIST: " + x);
        SetDistanceTraveled(playerInfo.turnDistanceTraveled + x);
        photonView.RPC("MoveToRPC", RpcTarget.All, destination.x, destination.y, destination.z);

        Moved?.Invoke();
    }

    //</movement>----------

    #region range
    //<range>---------------
    public int BasicRange() {
        return (int)(playerInfo.setBasicRange == -1 ? (playerInfo.basicRange + playerInfo.addedBasicRange) * playerInfo.basicRangeMultiplier : playerInfo.setBasicRange);
    }

    public bool AttackableInTiles(List<Vector3Int> tiles) {
        List<RaycastHit2D> hits = GridHelper.RaycastTilesNoEmptyHits(tiles);
        foreach (var hit in hits) {
            IAttackable target = hit.transform.parent.GetComponent<IAttackable>();
            if (target != null)
                return true;
        }
        return false;
    }

    public bool AttackableInBasicRange() {
        return AttackableInTiles(TilesInBasic());
    }

    public bool TargetInBasicRange(Vector3Int target) {
        List<Vector3Int> tiles = TilesInBasic();
        foreach (Vector3Int tile in tiles)
            if (tile == target)
                return true;
        return false;
    }

    public bool EnemyInBasicRange() {
        return TargetInBasicRange(EnemyCoords());
    }
    
    public List<Vector3Int> TilesInAttack(RangeDefined range) {
        if(range.IsTileValid == null)
            range.IsTileValid = GridHelper.CanAttackThrough;
        return GridHelper.TilesInRange(Coords(), range);
    }

    public List<Vector3Int> TilesInBasic() {
        return TilesInBasic(null);
    }

    public List<Vector3Int> TilesInBasic(List<int> allowedDirections) {
        return GridHelper.TilesInRange(Coords(), new RangeDefined(BasicRange(), playerInfo.basicIsStraight, playerInfo.basicIsPiercing, false, allowedDirections, GridHelper.CanAttackThrough));
    }

    public List<Vector3Int> TilesInAnyMove(RangeDefined range) {
        range.IsTileValid = GridHelper.CanStandOn;
        return GridHelper.TilesInRange(Coords(), range);
    }

    public List<Vector3Int> TilesInMove() {
        return GridHelper.TilesInRange(Coords(), new RangeDefined(MoveDistance(), playerInfo.moveIsCharge, false, false, null, GridHelper.CanStandOn));
    }

    //</range>-----------
    #endregion

    [PunRPC]
    //<positionCoords>----------
    public void SetPlayerPosRPC(int x, int y, int z) {
        Vector3Int tile = new Vector3Int(x, y, z);
        if (GridHelper.CanStandOn(tile))
            transform.position = GridHelper.grid.CellToWorld(tile) + cellOffset;
    }

    public void SetPlayerPos(Vector3Int tile) {
        photonView.RPC("SetPlayerPosRPC", RpcTarget.All, tile.x, tile.y, tile.z);
    }

    public Vector3Int Coords() { //same thing, cant be bothered to change it in all files
        return GridHelper.grid.WorldToCell(transform.position);
    }

    public Vector3Int EnemyCoords() {
        return otherManager.Coords();
    }

    public Vector3Int MouseCoords() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = GridHelper.grid.transform.position.z - myCamera.transform.position.z;
        Vector3 worldMousePos = myCamera.ScreenToWorldPoint(mousePos);
        Vector3Int mouseCoords = GridHelper.grid.WorldToCell(worldMousePos);
        return mouseCoords;
    }
    //</positionCoords>-------------

    //class overrides
    protected abstract void SetClassProperties();
}