using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AntSpawner : MonoBehaviour
{
    public float antRate = 200;
    public TMP_Text antCounter, leafCounter;
    public enum antTypes { Default, Jumping, Flying };

    private GameObject antPrefab;
    [HideInInspector]
    public int antCount, leafCount;

    private void Start()
    {
        antPrefab = Resources.Load<GameObject>("Ant");
        leafCount = 0;
    }

    private void Update()
    {
        antCounter.text = "Ant count: " + antCount;
        leafCounter.text = "Leaf count: " + leafCount;

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

        if (
            antType.Equals(antTypes.Default) ||
            (antType.Equals(antTypes.Jumping) && leafCount >= 5) ||
            (antType.Equals(antTypes.Flying) && leafCount >= 15))
        {
            GameObject newAnt = Instantiate(antPrefab, transform.position, getRandomQuaternion());
            newAnt.GetComponent<AntWiggle>().antType = antType;
            newAnt.transform.SetParent(transform.parent);
            newAnt.GetComponent<Rigidbody>().velocity += Vector3.up;

            antCount++;

            if (antType.Equals(antTypes.Jumping))
            {
                //newAnt.transform.Translate(Vector3.up);

                leafCount -= 5;
            }
            else if (antType.Equals(antTypes.Flying))
            {
                //newAnt.transform.Translate(Vector3.up);

                leafCount -= 15;
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
}
