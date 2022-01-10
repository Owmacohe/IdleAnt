using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionManager : MonoBehaviour
{
    private GameObject decision;
    [HideInInspector]
    public bool hasQueen;

    private void Start()
    {
        decision = GameObject.FindGameObjectWithTag("Decision");
        decision.SetActive(false);
    }

    public void answer(bool isYes)
    {
        print(isYes);
    }
}
