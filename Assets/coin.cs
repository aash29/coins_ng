using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class coin : NetworkBehaviour {
	[SyncVar]
	public int player;
	private float currentSpeed;
	public float limit_speed;
	public int counted = 0;
	// the current direction of travel
	private Vector2 currentDir;
	public int id;

	// Use this for initialization
	void Start () {

		if (player==0){
			GetComponent<Renderer>().material = Resources.Load("coin_color_p0") as Material;
		}
		
		if (player==1){
			GetComponent<Renderer>().material = Resources.Load("coin_color_p1") as Material;
		}

	
	}

	void OnDrawGizmos() {
		//UnityEditor.Handles.Label(transform.position, id.ToString());
	}

	
	// Update is called once per frame
	void Update () {

	}


}


