using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AntSpawner : MonoBehaviour
{
    public float antRate = 200;
    public TMP_Text antCounter, leafCounter;
    public enum antTypes { Default, Jumping, Flying, Queen };

    private GameObject antPrefab;
    [HideInInspector]
    public int antCount, leafCount;
    private int lastAntCount, lastLeafCount;
    [HideInInspector]
    public bool doesHaveQueen;
    private GameObject multi;

    private void Start()
    {
        antPrefab = Resources.Load<GameObject>("Ant");
        multi = GameObject.FindGameObjectWithTag("Multiplyer");
        multi.SetActive(false);
    }

    private void Update()
    {
        if (lastAntCount != antCount)
        {
            antCounter.text = "Ant count: " + antCount;
            StartCoroutine(UIPulse(antCounter.gameObject));

            lastAntCount = antCount;
        }

        if (lastLeafCount != leafCount)
        {
            leafCounter.text = "Leaf count: " + leafCount;
            StartCoroutine(UIPulse(leafCounter.gameObject));

            lastLeafCount = leafCount;
        }

        if (antRate > 0 && Random.Range(0, antRate) <= 1)
        {
            spawnAnt("Default");
        }
    }

    public void spawnAnt(string typeName)
    {
        antTypes antType = antTypes.Default;

        if (typeName.Equals("Jumping"))
        {
            antType = antTypes.Jumping;
        }
        else if (typeName.Equals("Flying"))
        {
            antType = antTypes.Flying;
        }
        else if (typeName.Equals("Queen"))
        {
            antType = antTypes.Queen;
            doesHaveQueen = true;
        }

        if (
            antType.Equals(antTypes.Default) ||
            (antType.Equals(antTypes.Jumping) && leafCount >= 5) ||
            (antType.Equals(antTypes.Flying) && leafCount >= 15) ||
            (antType.Equals(antTypes.Queen) && leafCount >= 30))
        {
            if (antType.Equals(antTypes.Queen))
            {
                Destroy(GameObject.FindGameObjectWithTag("QueenButton"));
                multi.SetActive(true);
                FindObjectOfType<DecisionManager>().hasQueen = true;

                GameObject queen = Instantiate(Resources.Load<GameObject>("Queen"), transform);
                queen.transform.SetParent(transform.parent);
                queen.transform.localPosition = Vector3.up * 1.9f;

                antCount++;
                leafCount -= 30;
            }
            else
            {
                GameObject newAnt = Instantiate(antPrefab, transform.position, getRandomQuaternion());
                newAnt.GetComponent<AntWiggle>().antType = antType;
                newAnt.transform.SetParent(transform.parent);
                newAnt.GetComponent<Rigidbody>().velocity += Vector3.up;

                antCount++;

                if (antType.Equals(antTypes.Jumping))
                {
                    leafCount -= 5;
                }
                else if (antType.Equals(antTypes.Flying))
                {
                    leafCount -= 15;
                }
            }
        }
    }

    private Quaternion getRandomQuaternion()
    {
        return Quaternion.Euler(
            Random.Range(0, 360),
            Random.Range(0, 360),
            Random.Range(0, 360)
        );
    }

    private IEnumerator UIPulse(GameObject target)
    {
        for (float i = 0; i < Mathf.PI; i += 0.2f)
        {
            target.transform.localScale = Vector3.one * (0.15f * Mathf.Sin(i) + 1);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
