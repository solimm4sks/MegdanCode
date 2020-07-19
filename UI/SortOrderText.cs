using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortOrderText : MonoBehaviour
{
    [SerializeField] string sortingLayerName = "UI";
    [SerializeField] int sortOrder = 1000;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().sortingLayerName = sortingLayerName;
        gameObject.GetComponent<MeshRenderer>().sortingOrder = sortOrder;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
