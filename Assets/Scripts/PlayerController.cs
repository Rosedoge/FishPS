using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
namespace player
{
    public class PlayerController : MonoBehaviour
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
            [Header("Casing Prefab")]
            public Transform casingPrefab;

            [Header("Grenade Prefab")]
            public Transform grenadePrefab;
        }
        public prefabs Prefabs;

        [System.Serializable]
        public class spawnpoints
        {
            [Header("Spawnpoint")]
            //The position where the casings spawns from
            public Transform casingSpawnPoint;

            [Header("Grenade Spawnpoint")]
            //The position where the casings spawns from
            public Transform grenadeSpawnPoint;
        }
        public spawnpoints Spawnpoints;
        //All Components
        [System.Serializable]
        public class components
        {
            [Header("Muzzleflash Holders")]
            public GameObject sideMuzzle;
            public GameObject topMuzzle;
            public GameObject frontMuzzle;
            //Array of muzzleflash sprites
            public Sprite[] muzzleflashSideSprites;

            [Header("Light Front")]
            public Light lightFlash;
            [Header("Particle System")]
            public ParticleSystem smokeParticles;

            [Header("Bullet In Mag")]
            public Transform bulletInMag;
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

            //Dont show the muzzleflash at start
            Components.sideMuzzle.GetComponent<SpriteRenderer>().enabled = false;
            Components.topMuzzle.GetComponent<SpriteRenderer>().enabled = false;
            Components.frontMuzzle.GetComponent<SpriteRenderer>().enabled = false;



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

            //Right click hold to aim
            if (Input.GetMouseButton(1))
            {
                anim.SetBool("isAiming", true);
            }
            else
            {
                anim.SetBool("isAiming", false);
            }

            //R key to reload
            if (Input.GetKeyDown(KeyCode.R) && !isReloading)
            {
                Reload();
            }

            if (Input.GetKeyDown(KeyCode.G) && !isThrowing)
            {
                ThrowGrenade();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (GetComponent<AbilityController>().abilities[0].UltimateReady)
                {
                    GetComponent<AbilityController>().abilities[0].Activate();
                }
            }

            //Run when holding down left shift and moving
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0)
            {
                anim.SetFloat("Run", 0.2f);
            }
            else
            {
                //Stop running
                anim.SetFloat("Run", 0.0f);
            }

            //Space key to jump
            //Disable jumping animation while reloading
            if (Input.GetKeyDown(KeyCode.Space) && !isReloading)
            {
                //Play jump animation
                anim.Play("Jump");
            }


            //If out of ammo
            if (currentAmmo == 0)
            {
                outOfAmmo = true;
                //if ammo is higher than 0
            }
            else if (currentAmmo > 0)
            {
                outOfAmmo = false;
            }
        }

        //Muzzleflash
        IEnumerator MuzzleFlash()
        {

            //Show muzzleflash if useMuzzleFlash is true
            //Show a random muzzleflash from the array
            Components.sideMuzzle.GetComponent<SpriteRenderer>().sprite = Components.muzzleflashSideSprites
                [Random.Range(0, Components.muzzleflashSideSprites.Length)];
            Components.topMuzzle.GetComponent<SpriteRenderer>().sprite = Components.muzzleflashSideSprites
                [Random.Range(0, Components.muzzleflashSideSprites.Length)];

            //Show the muzzleflashes
            Components.sideMuzzle.GetComponent<SpriteRenderer>().enabled = true;
            Components.topMuzzle.GetComponent<SpriteRenderer>().enabled = true;
            Components.frontMuzzle.GetComponent<SpriteRenderer>().enabled = true;

            //Enable the light flash
            // Components.lightFlash.GetComponent<Light>().enabled = true;

            //Play smoke particles 
            //Components.smokeParticles.Play();

            //Show the muzzleflash for 0.02 seconds
            yield return new WaitForSeconds(0.02f);

            //Hide the muzzleflashes
            Components.sideMuzzle.GetComponent<SpriteRenderer>().enabled = false;
            Components.topMuzzle.GetComponent<SpriteRenderer>().enabled = false;
            Components.frontMuzzle.GetComponent<SpriteRenderer>().enabled = false;

            //Disable the light flash if true
            //Components.lightFlash.GetComponent<Light>().enabled = false;
        }

        public void ThrowGrenade()
        {

            //Play reload animation
            anim.Play("mGrenadeThrow");
            //Start the "show bullet" timer
            StartCoroutine(IEThrowGrenade());
        }


        IEnumerator IEThrowGrenade()
        {
            //Wait for set amount of time 
            yield return new WaitForSeconds(0.25f);
            //Show the bullet inside the mag
            Instantiate(Prefabs.grenadePrefab.gameObject,
               Spawnpoints.grenadeSpawnPoint.transform.position,
               Spawnpoints.grenadeSpawnPoint.transform.rotation);
        }
        void Shoot()
        {

            //Play shoot animation
            if (!anim.GetBool("isAiming"))
            {
                anim.Play("mFire");
            }
            else
            {
                anim.SetTrigger("Shoot");
            }

            //Remove 1 bullet
            currentAmmo -= 1;

            Instantiate(Prefabs.casingPrefab,
                Spawnpoints.casingSpawnPoint.transform.position,
                Spawnpoints.casingSpawnPoint.transform.rotation);
            //Show the muzzleflash
            StartCoroutine(MuzzleFlash());

            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.forward, out hit))
            {
                if (hit.transform.gameObject.tag.Contains("Enemy"))
                {
                        hit.transform.gameObject.GetComponent<Enemy>().TakeDamage(10);
                    
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
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("mFire"))
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
