using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardVisuals : MonoBehaviour
{
    public TextMeshProUGUI titleObj;
    public TextMeshProUGUI descObj;
    public TextMeshProUGUI flavorObj;
    public TextMeshProUGUI resolveObj;
    public Image artObj;

    private Card card;

    public void LoadCard(Card c) {
        if (c == null) {
            titleObj.text = "";
            descObj.text = "";
            flavorObj.text = "";
            resolveObj.text = "";
            artObj.sprite = Resources.Load<Sprite>("Art/Cards/BlankCardArt");
            return;
        }

        gameObject.SetActive(true);

        card = c;
        titleObj.text = card.title;
        descObj.text = card.desc;
        artObj.sprite = card.art;
        resolveObj.text = card.resolveCost.ToString();
        if (card.art == null) {
            artObj.sprite = Resources.Load<Sprite>("Art/Cards/BlankCardArt");
        }
        if (string.IsNullOrEmpty(card.flavor)) {
            flavorObj.gameObject.SetActive(false);
            flavorObj.text = "";
        }
        else {
            flavorObj.gameObject.SetActive(true);
            flavorObj.text = card.flavor;
        }
    }

    public void LoadCard(string t, string d, string f, int r) {
        titleObj.text = t;
        descObj.text = d;
        flavorObj.text = f;
        resolveObj.text = ""  +r;
        artObj.sprite = Resources.Load<Sprite>("Art/Cards/BlankCardArt");
    }
}
