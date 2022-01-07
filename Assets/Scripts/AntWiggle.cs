using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntWiggle : MonoBehaviour
{
    public float wiggleRate = 500;
    public float wiggleAmount = 1.1f;

    private Rigidbody rb;
    private AntSpawner spawn;
    [HideInInspector]
    public AntSpawner.antTypes antType;
    private bool isJumping, isFlying;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        spawn = FindObjectOfType<AntSpawner>();
    }

    private void Update()
    {
        if (transform.position.y >= 3)
        {
            rb.velocity += Vector3.down;
        }
        else if (transform.position.y <= -3)
        {
            spawn.antCount--;
            Destroy(gameObject);
        }

        if (!isJumping && antType.Equals(AntSpawner.antTypes.Jumping))
        {
            wiggleRate = 30;
            wiggleAmount = 4;

            GetComponentInChildren<MeshRenderer>().material = Resources.Load<Material>("Jumping");

            isJumping = true;
        }
        else if (!isFlying && antType.Equals(AntSpawner.antTypes.Flying))
        {
            wiggleRate = 30;
            wiggleAmount = 4;

            rb.drag /= 2;

            CapsuleCollider[] coll = GetComponents<CapsuleCollider>();
            coll[0].radius = 4;
            coll[0].height = 10;
            coll[1].radius = 4;
            coll[1].height = 10;

            GetComponentInChildren<MeshRenderer>().material = Resources.Load<Material>("Flying");

            isFlying = true;
        }

        if (Random.Range(0, wiggleRate) <= 1)
        {
            rb.velocity += randomVector3();

            if (!antType.Equals(AntSpawner.antTypes.Default))
            {
                rb.velocity = rb.velocity.y * Vector3.up;
            }
        }
    }

    private Vector3 randomVector3()
    {
        return new Vector3(
            Random.Range(-wiggleAmount, wiggleAmount),
            Random.Range(-wiggleAmount, wiggleAmount),
            Random.Range(-wiggleAmount, wiggleAmount)
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Tree"))
        {
            if (antType.Equals(AntSpawner.antTypes.Default))
            {
                spawn.leafCount++;
            }
            else if (antType.Equals(AntSpawner.antTypes.Jumping))
            {
                spawn.leafCount += 5;
            }
            else if (antType.Equals(AntSpawner.antTypes.Flying))
            {
                spawn.leafCount += 10;
            }

            spawn.antCount--;
            Destroy(gameObject);
        }
    }
}
