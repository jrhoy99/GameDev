using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour {
	public Monster monsters;
	public bool alive = true;

	void OnTriggerEnter(Collider other){
		if (other.gameObject.name == "eyes") {
			other.transform.parent.GetComponent<Monster> ().CheckSight ();
		} else if (other.gameObject.name == "lostPage") {
			Destroy (other.gameObject);
			gameplayCanvas.instance.pickedUpSounds ();
			gameplayCanvas.instance.findPage ();
		} else if (other.gameObject.name == "Door") {
			if (gameplayCanvas.instance.pagesFound >= gameplayCanvas.instance.pagesTotal) {
				
				Cursor.visible = true;
				gameplayCanvas.instance.Escaped ();
			} else {
				print ("WEW");
			}
		}
	}


}
