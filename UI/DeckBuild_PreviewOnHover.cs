using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DeckBuild_PreviewOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private TextMeshProUGUI textChild;
    private GameObject previewParent;
    //private DeckBuilder deckBuilder;
    
    public Card card;

    private void Awake() {
        //deckBuilder = FindObjectOfType<DeckBuilder>();
        previewParent = GameObject.Find("Canvas/PlayerDeck/PreviewParent");

        foreach (Transform child in transform)
            if (child.gameObject.name == "CardName") {
                textChild = child.gameObject.GetComponent<TextMeshProUGUI>();
                break;
            }

    }

    private void Update() {
        if (card != null)
            textChild.text = card.title;
        else
            textChild.text = "     /";
    }

    GameObject preview;
    public void OnPointerEnter(PointerEventData pointerData) {
        GameObject curr = Instantiate(Resources.Load<GameObject>("Prefabs/Card"));
        curr.transform.SetParent(previewParent.transform, false);
        curr.transform.localScale = new Vector3(1.6f, 1.6f, 1);
        curr.GetComponent<CardLogic>().enabled = false;
        curr.GetComponent<CardVisuals>().LoadCard(card);
        preview = curr;
    }

    public void OnPointerExit(PointerEventData pointerData) {
        Destroy(preview);
    }
}
