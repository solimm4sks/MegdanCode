using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StateMachine : MonoBehaviourPun
{
    public State state;
    public void SetState(State st) {
        state = st;
        StartCoroutine(state.Start());
    }
}
