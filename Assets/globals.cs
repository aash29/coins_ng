using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class globals : NetworkBehaviour {
	[SyncVar(hook = "OnCurPlayerChange")]
	public int curPlayer=0;

	private int numPlayers=-1;


	//public int playerID=0;

	//[SyncListInt]
	public SyncListInt score = new SyncListInt();
	//static public float force=1.0f;
	static int turnNumber = 1;
	//[SyncVar]
	//static public Dictionary<int, GameObject> coinDict = new Dictionary<int, GameObject>();
	// Use this for initialization
	void Start () {
		score.Add (30);
		score.Add (30);
		//score [0] = 30;
		//score [1] = 30;


	}

	public int getNumPlayers()
	{
		return numPlayers;
	}

	public void setNumPlayers(int value)
	{
		numPlayers = value;
	}


	void OnCurPlayerChange(int cp)
	{
		curPlayer = cp;
	}
		

	

	// Update is called once per frame
	void Update () {
		//score [0] = 0;
		//score [1] = 0;



		globals g1 = GetComponentInParent<globals>();

		//int np1 = g1.getNumPlayers();
		g1.setNumPlayers(GameObject.FindGameObjectsWithTag("Player").Length);
		//g1.setNumPlayers(NetworkServer.connections.Count + 1);
	}







}
