using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Monster : MonoBehaviour {
	//pang global variable
	public GameObject panel;

	//player/ main character
	public GameObject player;

	//sounds
	public AudioClip[] footsounds;
	private AudioSource sound;
	public AudioSource growl;
	public AudioClip playerDeath;
	public GameObject bgm;
	//variable ng search state
	private float wait = 0f;

	//animation
	private Animator anim;

	//variables pang walk or navigation
	private NavMeshAgent nav;
	private string state = "Idle";
	private bool alive = true;
	private bool highAlert = false;
	private float alertness = 20f;

	//vision ng monster
	public Transform eyes;

	//deathcam
	public GameObject deathCam;
	public Transform camPos; 

	// Use this for initialization in tagalog pang declare
	void Start () {
		nav = GetComponent<NavMeshAgent> ();
		sound = GetComponent<AudioSource> ();
		anim = GetComponent<Animator> ();
		nav.speed = 1.2f;
		anim.speed = 1.2f;
	}
	//sounds ng footstep ng monster
	public void footstep(int _num){
		sound.clip = footsounds [_num];
		sound.Play ();
	}
	public void playerDeathSound(){
		sound.clip = playerDeath;
		sound.Play();
	}

	//pang check kung makikita yung player
	public void CheckSight(){
		if (alive) {
			RaycastHit rayHit;

			if (Physics.Linecast (eyes.position, player.transform.position, out rayHit)) {
				
				//kapag nakita na yung player
				if(rayHit.collider.gameObject.name == "player"){
					if (state != "kill") {
						//pang pabilis ng takbo nang monster
						state = "Chase";
						nav.speed = 3.5f;
						anim.speed = 3.5f;
						//pang play ng sounds yung pitch pang lakas lang ng sounds
						growl.pitch = 1.2f;
						growl.Play();
					}
				}
			}
		}
	}

	//animation pag mamatay yung monster
	public void death(){
		anim.SetTrigger("dead");
		anim.speed = 1f;
		alive = false;

		nav.isStopped = true;
	}


	// Update is called once per frame
	void Update () {
		//for testing purpose
		Debug.DrawLine(eyes.position , player.transform.position , Color.green);


		//pang navigate
		if (alive) {
			//nav.SetDestination (player.transform.position); //wala to cinomment ko lang

			//animation
			anim.SetFloat ("velocity", nav.velocity.magnitude);

			//idle state//
			if(state == "Idle"){
				
				//pick a random place to walkthrough
				//use to find random spot in a alertness radius area//
				Vector3 randomPos = Random.insideUnitSphere * alertness;
				NavMeshHit navHit;
				NavMesh.SamplePosition (transform.position + randomPos , out navHit , 20f,NavMesh.AllAreas);
				//punta malapet sa player
				if (highAlert) {
					NavMesh.SamplePosition (player.transform.position + randomPos , out navHit , 20f,NavMesh.AllAreas);
					//tataas yung alertness kapag nawawala sa mata ng monster yung player
					alertness += 5f;
					//kapag dikatalga niya makita ulet babalik siya sa normal state
					if (alertness > 20f) {
						highAlert = false;
						nav.speed = 1.2f;
						anim.speed = 1.2f;
					}
				}

				nav.SetDestination (navHit.position);
				state = "Walk";
			
			}
			//walking state//
			if(state == "Walk"){
				
				//para makapunta sa destination
				if (nav.remainingDistance <= nav.stoppingDistance && !nav.pathPending) {
					state = "Search";
					wait = 5f;
				
				}
			}

			//search state//
			if(state == "Search"){
				//5 sec maghahanap
				if (wait > 0f) {
					wait -= Time.deltaTime;
					transform.Rotate (0f, 120f * Time.deltaTime, 0f);

				}
				//pagtapos ng 5sec maghanap suko na siya
				else {
					state = "Idle";
				}
			}
			//chase state//
			if(state == "Chase"){
				nav.destination = player.transform.position;
				//pag nawala sa paningin yung player
				float distance = Vector3.Distance(transform.position,player.transform.position);
				//pag distance ay masmalake sa radius of 10
				if (distance > 10f) {
					state = "Hunt";
				}
				//pag papatayin yung player
				else if (nav.remainingDistance <= nav.stoppingDistance + 1f &&  !nav.pathPending){
					if (player.GetComponent<Player> ().alive) {
						state = "Kill";
						player.GetComponent<Player> ().alive = false;
						player.GetComponent<FirstPersonController> ().enabled = false;
						deathCam.SetActive (true);
						deathCam.transform.position = Camera.main.transform.position;
						deathCam.transform.rotation = Camera.main.transform.rotation;
						Camera.main.gameObject.SetActive(false);
						growl.pitch = 0.7f;
						growl.Play ();
						Invoke ("reset", 1f);
					}
				}
			}

			//hunt state//
			if (state == "Hunt") {
				if (nav.remainingDistance <= nav.stoppingDistance && !nav.pathPending) {
					state = "Search";
					wait = 5f;
					highAlert = true;
					alertness = 5f;
					//kailangan para pagnakatago ka sa wall di ka makikita
					CheckSight ();
				}
			}
			//kill state//
			if(state == "Kill"){
				deathCam.transform.position = Vector3.Slerp (deathCam.transform.position, camPos.position , 10f * Time.deltaTime);
				deathCam.transform.rotation = Quaternion.Slerp (deathCam.transform.rotation, camPos.rotation , 10f * Time.deltaTime);
				anim.speed = 1f;
				nav.SetDestination (deathCam.transform.position);
			}
		}
	}

	//pang reset ng game
	void reset(){
		panel.SetActive (true);
		bgm.SetActive (false);
		playerDeathSound ();
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;


	}
}
