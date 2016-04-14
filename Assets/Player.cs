using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Player : NetworkBehaviour {

	//[SyncVar]
	public int playerID=0;
	[SyncVar]
	public float force = 1.0f;

	//using System.Collections.Generic;public Dictionary<int, GameObject> coinDict = new Dictionary<int, GameObject>();

	// Use this for initialization
	void Start () {
		//StartCoroutine(initCoins());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void OnStartLocalPlayer()
	{

		//int n1 = GetComponentInParent<globals> ().numPlayers;
		//GameManager.RegisterPlayer(this);
		if (isServer)
		{
			GetComponentInParent<Player>().playerID = 0;
		}
		else if (isLocalPlayer)
		{
			GetComponentInParent<Player>().playerID = 1;
		}

	}



}
