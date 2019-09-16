using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
namespace player
{
    public class TwistedScript : MonoBehaviour
    {

        Animator anim;

        bool isReloading;
        bool outOfAmmo;

        bool isShooting;
        bool isAimShooting;
        bool isAiming;
        bool isRunning;
        bool isJumping;
        bool isThrowing;

        [System.Serializable]
        public class prefabs
        {
            [Header("Card")]
            public GameObject cardPrefab;
        }
        public prefabs Prefabs;

        [System.Serializable]
        public class spawnpoints
        {
            [Header("Spawnpoint")]
            //The position where the casings spawns from
            public Transform cardSpawnPoint;

            [Header("Grenade Spawnpoint")]
            //The position where the casings spawns from
            public Transform grenadeSpawnPoint;
        }
        public spawnpoints Spawnpoints;
        //All Components
        [System.Serializable]
        public class components
        {
            [Header("Cards")]
            public GameObject[] cards;


            

            
        }
        public components Components;

        //Ammo left
        public int currentAmmo;

        //Used for fire rate
        float lastFired;

        public float fireRate;

        // Start is called before the first frame update
        void Start()
        {
            anim = GetComponent<Animator>();

            //Set the ammo count
            RefillAmmo();




        }

        // Update is called once per frame
        void Update()
        {

            //Check which animation 
            //is currently playing
            AnimationCheck();
            //Left click hold to fire
            if (Input.GetMouseButton(0)
                //Disable shooting while running and jumping
                && !isReloading && !outOfAmmo && !isShooting && !isAimShooting && !isRunning && !isJumping)
            {
                //Shoot automatic
                if (Time.time - lastFired > 1 / fireRate)
                {
                    Shoot();
                    lastFired = Time.time;
                }
            }

            ////Right click hold to aim
            //if (Input.GetMouseButton(1))
            //{
            //    anim.SetBool("isAiming", true);
            //}
            //else
            //{
            //    anim.SetBool("isAiming", false);
            //}

            ////R key to reload
            //if (Input.GetKeyDown(KeyCode.R) && !isReloading)
            //{
            //    Reload();
            //}

            //if (Input.GetKeyDown(KeyCode.G) && !isThrowing)
            //{
            //    ThrowGrenade();
            //}
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (GetComponent<AbilityController>().abilities[0].UltimateReady)
                {
                    GetComponent<AbilityController>().abilities[0].Activate();
                    //Destroy(GetComponent<MouseLook>());
                }
            }

            //Run when holding down left shift and moving
            //if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0)
            //{
            //    anim.SetFloat("Run", 0.2f);
            //}
            //else
            //{
            //    //Stop running
            //    anim.SetFloat("Run", 0.0f);
            //}

            ////Space key to jump
            ////Disable jumping animation while reloading
            //if (Input.GetKeyDown(KeyCode.Space) && !isReloading)
            //{
            //    //Play jump animation
            //    anim.Play("Jump");
            //}


            ////If out of ammo
            //if (currentAmmo == 0)
            //{
            //    outOfAmmo = true;
            //    //if ammo is higher than 0
            //}
            //else if (currentAmmo > 0)
            //{
            //    outOfAmmo = false;
            //}
        }


        public void CardEnable()
        {
            foreach (GameObject item in Components.cards)
            {
                item.SetActive(true);

            }

        }

        public void CardHide()
        {
            foreach (GameObject item in Components.cards)
            {
                item.SetActive(false);

            }
        }
        Vector3 Direction;
        public void ThrowCard()
        {
            GameObject Temp = Instantiate(Prefabs.cardPrefab,
                Spawnpoints.cardSpawnPoint.transform.position,
                Spawnpoints.cardSpawnPoint.transform.rotation);
            Temp.GetComponent<Rigidbody>().AddForce(Direction * 1000);
            //Shoot();
        }

        void Shoot()
        {

            anim.Play("ThrowCard");

            Direction = (transform.GetChild(0).forward + Random.insideUnitSphere * 0.035f).normalized;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Direction, out hit))
            {
                if (hit.transform.gameObject.tag.Contains("Enemy"))
                {
                    hit.transform.gameObject.GetComponent<Enemy>().TakeDamage(15);

                }
            }
        }

        //Refill ammo
        public void RefillAmmo()
        {
            //Set the ammo
            currentAmmo = 30;
        }

        //Reload
        void Reload()
        {

            //Play reload animation
            anim.Play("mReload");
            //Start the "show bullet" timer
            StartCoroutine(BulletInMagTimer());
        }
        IEnumerator BulletInMagTimer()
        {
            //Wait for set amount of time 
            yield return new WaitForSeconds(0.5f);
            //Show the bullet inside the mag
            // Components.bulletInMag.GetComponent<MeshRenderer>().enabled = true;
        }


        //Check current animation playing
        void AnimationCheck()
        {

            //Check if shooting
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("ThrowCard"))
            {
                isShooting = true;
            }
            else
            {
                isShooting = false;
            }

            //Check if shooting while aiming down sights
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("mAimFire"))
            {
                isAimShooting = true;
            }
            else
            {
                isAimShooting = false;
            }

            //Check if running
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("mRun"))
            {
                isRunning = true;
            }
            else
            {
                isRunning = false;
            }

            //Check if jumping
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
            {
                isJumping = true;
            }
            else
            {
                isJumping = false;
            }

            //Check if reloading
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("mReload"))
            {
                // If reloading
                isReloading = true;
                //Refill ammo
                RefillAmmo();
            }
            else
            {
                //If not reloading
                isReloading = false;
            }

            //Check if jumping
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("mGrenadeThrow"))
            {
                isThrowing = true;
            }
            else
            {
                isThrowing = false;
            }

        }
    }
}
