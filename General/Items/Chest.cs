using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Chest : Spawnable, IAttackable, ITerrain
{
    #pragma warning disable 649
    [SerializeField] private GameObject myOutline;
    [SerializeField] private Animator animator;

    public int health = 7;
    public int id;
    private SpriteRenderer sprite;
    private ChestManager chestManager;
    private TextMeshPro healthText;
    
    private Color[] colors = {
        new Color(252f / 255, 3f / 255, 44f / 255), //red
        new Color(3f / 255, 123f / 255, 252f / 255), //blue
        new Color(20f / 255, 252f / 255, 3f / 255), //green
        new Color(223f / 255, 252f / 255, 3f / 255), //yellow
        new Color(148f / 255, 3f / 255, 252f / 255)}; //purple

    private void Awake() {
        sprite = GetComponent<SpriteRenderer>();
        chestManager = FindObjectOfType<ChestManager>();
        healthText = GameObject.Find(gameObject.name + "/Text").GetComponent<TextMeshPro>();

        spawnOffset = new Vector3(0, 0.29f, 0);
    }


    [PunRPC]
    public void SetHealth(int x) {
        health = x;
        healthText.text = "" + health;
    }

    [PunRPC]
    public void SetId(int x) {
        id = x;
        sprite.color = colors[id];
    }

    [PunRPC]
    public void StartChestOpeningAnimRPC() {
        animator.SetTrigger("opening");
    }

    [PunRPC]
    public void StartChestHitAnimRPC() {
        animator.SetTrigger("hit");
    }

    public void GetDamaged(int dmg) {
        health -= dmg;
        healthText.text = "" + health;

        if (!PhotonNetwork.IsMasterClient)
            return;

        if (health <= 0) {

            photonView.RPC("StartChestOpeningAnimRPC", RpcTarget.All);
            StartCoroutine("WaitForAnimation");
        }
        else {
            photonView.RPC("StartChestHitAnimRPC", RpcTarget.All);
        }
    }

    private IEnumerator WaitForAnimation() {
        yield return new WaitForEndOfFrame();

        while (animator.GetCurrentAnimatorStateInfo(0).IsName("chestOpenAnimation")) {
            yield return null;
        }
        
        chestManager.ChestDestroyed(id);
        PhotonNetwork.Destroy(gameObject);
    }

    public void SetAttackableOutline(bool x) {
        myOutline.SetActive(x);
    }

    [PunRPC]
    public void MoveToRPC(int x, int y, int z)
    {
        if (!photonView.IsMine)
            return;

        Vector3Int tile = new Vector3Int(x, y, z);
        transform.position = GridHelper.grid.CellToWorld(tile) + spawnOffset;
    }

    public void MoveTo(Vector3Int tile) {
        photonView.RPC("MoveToRPC", RpcTarget.All, tile.x, tile.y, tile.z);
    }
}
