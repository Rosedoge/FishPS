using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BobScript : MonoBehaviour
{

    float Starttoshoot = 1.5f;
    // Start is called before the first frame update
    void Awake()
    {
        Invoke("ColliderLol", 0.1f);
    }

    bool started = false;
    // Update is called once per frame
    void Update()
    {
        if (Starttoshoot >= 0)
        {
           
            Starttoshoot -= Time.deltaTime;
            return;
        }
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ;
        if(!started)
            InvokeRepeating("Shoot", 0, 0.5f);
        started = true;
    }

    void Shoot()
    {
        transform.RotateAround(transform.position, transform.up, Time.deltaTime * 90f);
        Collider[] cols = Physics.OverlapSphere(transform.position, 10f);
        cols = EnemyCol(cols.OfType<Collider>().ToList());
        foreach (Collider col in cols)
        {
            Debug.Log("Col name " + col.gameObject.name);
            col.gameObject.GetComponent<Enemy>().TakeDamage(25);
        }
    }
    
    /// <summary>
    /// returns only enemy colliders
    /// </summary>
    /// <param name="full"></param>
    /// <returns></returns>
    Collider[] EnemyCol(List<Collider> full)
    {
        int count = 0;
        Collider[] temp = new Collider[20];
        foreach (Collider item in full)
        {
            if (item.gameObject.tag.Contains("Enemy"))
            {
                temp[count] = item;
                count += 1;
            }
        }

        return temp;
    }

    void ColliderLol()
    {
        Debug.Log("colliding");
        GetComponent<CapsuleCollider>().enabled = true;
    }
}
