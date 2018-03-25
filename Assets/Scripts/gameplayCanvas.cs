using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
public class gameplayCanvas : MonoBehaviour {
	public static gameplayCanvas instance;

	public GameObject player;
	public GameObject directionalLight;
	public GameObject escapedPanel;
	public Monster[] monsters;
	public Text txtPages;
	public string pageString;
	public int pagesTotal = 4;
	public int pagesFound = 0;
	public AudioClip pickedUp;
	private AudioSource sounds;
	public AudioClip victory;
	public GameObject bgm;
	// Use this for initialization
	void Start () {
		instance = this;
		sounds = GetComponent<AudioSource> ();
		updateCanvas ();
	}

	public void updateCanvas(){
		pageString = "Pages " + pagesFound.ToString () + "/" + pagesTotal.ToString ();
		txtPages.text = pageString;
	}

	public void findPage(){
		pagesFound++;
		updateCanvas ();
		//pang panalo
		if(pagesFound >= pagesTotal){
			directionalLight.SetActive (true);
			for (int n = 0; n < monsters.Length; n++) {
				monsters [n].death ();
			}
		}
	}

	public void pickedUpSounds(){
		sounds.clip = pickedUp;
		sounds.Play ();
	}
	public void VictorySounds(){
		sounds.clip = victory;
		sounds.Play ();
	}

	public void Escaped(){
		player.GetComponent<Player> ().alive = false;
		player.GetComponent<FirstPersonController> ().enabled = false;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		bgm.SetActive (false);
		escapedPanel.SetActive (true);
		VictorySounds ();


	}
		
	public void reset(){
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}
}
