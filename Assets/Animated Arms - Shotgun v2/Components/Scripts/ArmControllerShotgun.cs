using UnityEngine;
using System.Collections;

public class ArmControllerShotgun : MonoBehaviour {
	
	Animator anim;
	
	bool isReloading;
	bool outOfAmmo;
	
	bool isShooting;
	bool isAimShooting;
	bool isAiming;
	bool isRunning;
	bool isJumping;
	
	//Ammo left
	public int currentAmmo;
	
	[System.Serializable]
	public class ammoSettings
	{  
		[Header("Ammo")]
		//Total ammo
		public int ammo;
	}
	public ammoSettings AmmoSettings;

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
		public ParticleSystem sparkParticles;
	}
	public components Components;

	[System.Serializable]
	public class prefabs
	{  
		[Header("Casing Prefab")]
		public Transform casingPrefab;
	}
	public prefabs Prefabs;
	
	[System.Serializable]
	public class spawnpoints
	{  
		[Header("Spawnpoints")]
		//Spawnpoint for the casing
		public Transform casingSpawnPoint;
	}
	public spawnpoints Spawnpoints;
	
	[System.Serializable]
	public class audioClips
	{  
		[Header("Audio Source")]
		
		public AudioSource mainAudioSource;
		
		[Header("Audio Clips")]
		
		//All audio clips
		public AudioClip shootSound;
		public AudioClip reloadSound;
	}
	public audioClips AudioClips;
	
	void Awake () {
		
		//Set the animator component
		anim = GetComponent<Animator>();
		
		//Set the ammo count
		RefillAmmo ();

		//Start with muzzleflashes hidden
		Components.sideMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
		Components.topMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
		Components.frontMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
		//Start with the light flash disabled
		Components.lightFlash.GetComponent<Light> ().enabled = false;
	}
	
	void Update () {
		
		//Check which animation 
		//is currently playing
		AnimationCheck ();
		
		//Left click to shoot
		if (Input.GetMouseButtonDown (0) 
		    //Disable shooting while running and jumping
			&& !isReloading && !outOfAmmo && !isShooting && !isAimShooting && !isRunning && !isJumping) {
			//Shoot
			Shoot();
		}

		//Right click hold to aim
		if (Input.GetMouseButton (1)) {
			anim.SetBool ("isAiming", true);
		} else {
			anim.SetBool ("isAiming", false);
		}
		
		//R key to reload
		if (Input.GetKeyDown (KeyCode.R) && !isReloading) {
			Reload ();
		}
		
		//Run when holding down left shift and moving
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0) {
			anim.SetFloat("Run", 0.2f);
		} else {
			//Stop running
			anim.SetFloat("Run", 0.0f);
		}
		
		//Space key to jump
		//Disable jumping while reloading
		if (Input.GetKeyDown (KeyCode.Space) && !isReloading) {
			//Play jump animation
			anim.Play("Jump");
		}
		
		//If out of ammo
		if (currentAmmo == 0) {
			outOfAmmo = true;
			//if ammo is higher than 0
		} else if (currentAmmo > 0) {
			outOfAmmo = false;
		}
	}

	//Muzzleflash
	IEnumerator MuzzleFlash () {
	
		//Show a random muzzleflash from the array
		Components.sideMuzzle.GetComponent<SpriteRenderer> ().sprite = Components.muzzleflashSideSprites 
			[Random.Range (0, Components.muzzleflashSideSprites.Length)];
		Components.topMuzzle.GetComponent<SpriteRenderer> ().sprite = Components.muzzleflashSideSprites 
			[Random.Range (0, Components.muzzleflashSideSprites.Length)];
			
		//Show the muzzleflashes
		Components.sideMuzzle.GetComponent<SpriteRenderer> ().enabled = true;
		Components.topMuzzle.GetComponent<SpriteRenderer> ().enabled = true;
		Components.frontMuzzle.GetComponent<SpriteRenderer> ().enabled = true;

		//Enable the light flash
		Components.lightFlash.GetComponent<Light> ().enabled = true;
		//Play the smoke particles
		Components.smokeParticles.Play ();
		//Play the spark particles
		Components.sparkParticles.Play();

		//Show the muzzleflash for 0.02 seconds
		yield return new WaitForSeconds (0.02f);

		//Hide the muzzleflashes
		Components.sideMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
		Components.topMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
		Components.frontMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
		//Disable light flash
		Components.lightFlash.GetComponent<Light> ().enabled = false;
	}
	
	//Shoot
	void Shoot() {
		
		//Play shoot animation
		if (!anim.GetBool ("isAiming")) {
			anim.Play ("Fire");
		} else {
			anim.SetTrigger("Shoot");
		}
		
		//Remove 1 bullet
		currentAmmo -= 1;

		//Play shoot sound
		AudioClips.mainAudioSource.clip = AudioClips.shootSound;
		AudioClips.mainAudioSource.Play();

		//Start casing instantiate
		StartCoroutine (CasingDelay ());
		//Show the muzzleflash
		StartCoroutine (MuzzleFlash ());
	}
	
	//Refill ammo
	void RefillAmmo () {
		
		currentAmmo = AmmoSettings.ammo;
	}
	
	//Reload
	void Reload () {
		
		//Play reload animation
		anim.Play ("Reload");

		//Play reload sound
		AudioClips.mainAudioSource.clip = AudioClips.reloadSound;
		AudioClips.mainAudioSource.Play();
	}
	
	IEnumerator CasingDelay () {
		
		//Wait for 0.3 seconds before spawning casing
		yield return new WaitForSeconds (0.3f);

		//Spawn a casing at the spawnpoint
		Instantiate (Prefabs.casingPrefab, 
			Spawnpoints.casingSpawnPoint.transform.position, 
			Spawnpoints.casingSpawnPoint.transform.rotation);	
	}
	
	//Check current animation playing
	void AnimationCheck () {
		
		//Check if shooting
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Fire")) {
			isShooting = true;
		} else {
			isShooting = false;
		}

		//Check if shooting while aiming down sights
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Aim Fire")) {
			isAimShooting = true;
		} else {
			isAimShooting = false;
		}

		//Check if running
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Run")) {
			isRunning = true;
		} else {
			isRunning = false;
		}
		
		//Check if jumping
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Jump")) {
			isJumping = true;
		} else {
			isJumping = false;
		}

		//Check if finished rleoading (when returning to idle animation)
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Idle")) {
			isReloading = false;
		}
		
		//Check if reloading
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Reload")) {
			// If reloading
			isReloading = true;
			//Refill ammo
			RefillAmmo();
		}
	}
}