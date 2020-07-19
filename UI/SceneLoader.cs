using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(int x) {
        SceneManager.LoadScene(x);
    }

    public void LoadScene(string x) {
        SceneManager.LoadScene(x);
    }
}
