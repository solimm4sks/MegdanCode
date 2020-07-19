using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableGameObject : MonoBehaviour
{
    public int lifetime = 1;

    private void OnEnable()
    {
        StartCoroutine("DelayDisable");
    }

    private IEnumerator DelayDisable() {
        yield return new WaitForSeconds(lifetime);
        gameObject.SetActive(false);
    }
}
