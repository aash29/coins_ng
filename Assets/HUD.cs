using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class HUD : NetworkBehaviour {
	public Font f;
	public GameObject root;

	// Use this for initialization

	void OnGUI () {
		// Make a background box
		if (isLocalPlayer) {

			GUIStyle customGuiStyle = new GUIStyle ();

			GUI.skin.font = f;

			if (root==null)
				root = GameObject.Find("root1");

			//root = GameObject.Find("root1");
			globals g1 = root.GetComponent<globals>();
			int cp = g1.curPlayer;
			GUI.Box (new Rect (210, 10, 300, 50), "Current player: " + g1.curPlayer.ToString ()+ " of " + GameObject.Find("root1").GetComponent<globals>().getNumPlayers());

			customGuiStyle.fontSize = 20;
			customGuiStyle.normal.textColor = Color.white;
			int i1 = GetComponentInParent<Player> ().playerID;

			GUI.Box (new Rect (Screen.width - 150, Screen.height - 50, 50, 50), "Player ID:" + GetComponentInParent<Player> ().playerID.ToString (), customGuiStyle);

			int s1 = g1.score [0];
			int s2 = g1.score [1];

			GUI.Box (new Rect (25, Screen.height - 50, 50, 50), s1.ToString () + " : ", customGuiStyle);
			GUI.Box (new Rect (100, Screen.height - 50, 50, 50), s2.ToString (), customGuiStyle);

		}
		//globals.score [0] = 0;
		//globals.score [1] = 0;
		
		//GUI.Box(new Rect(10,200,300,290), Network.player.ToString ());
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
