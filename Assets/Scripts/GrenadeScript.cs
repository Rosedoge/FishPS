using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{

    public GameObject explosionPrefab;
    public float explodeTime = 3f;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * 750);
    }

    // Update is called once per frame
    void Update()
    {
        explodeTime -= Time.deltaTime;
        if (explodeTime <= 0)
        {
            GameObject temp = Instantiate(explosionPrefab, transform.position, transform.rotation);
            Destroy(temp, 2f);
            Destroy(this.gameObject);
        }
    }
}
