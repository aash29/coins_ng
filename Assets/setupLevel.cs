using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class setupLevel : NetworkBehaviour {

	public GameObject coinPrefab;

	public Dictionary<int, GameObject> coinDict = new Dictionary<int, GameObject>();

	public override void OnStartServer()
	{
        GameObject[] pharray0 = GameObject.FindGameObjectsWithTag("placeholder0");
        GameObject[] pharray1 = GameObject.FindGameObjectsWithTag("placeholder1");


        if (isServer)
        {

			for (int i=0; i<pharray0.Length; i++)
			{
				CmdSpawnCoin (pharray0[i].transform.position,0);
				//CmdRemove(pharray[i]);
			}


         
			
			for (int i=0; i<pharray1.Length; i++)
			{
				CmdSpawnCoin (pharray1[i].transform.position,1);
				//CmdRemove(pharray[i]);
			}

		}

        for (int i = 0; i < pharray0.Length; i++)
        {
            Destroy(pharray0[i]);
        }
        for (int i = 0; i < pharray1.Length; i++)
        {
            Destroy(pharray1[i]);
        }

		initCoins();
		//StartCoroutine(initCoins());



	}

	public override void OnStartClient()
	{
		GameObject[] pharray0 = GameObject.FindGameObjectsWithTag("placeholder0");
		GameObject[] pharray1 = GameObject.FindGameObjectsWithTag("placeholder1");


		for (int i = 0; i < pharray0.Length; i++)
		{
			Destroy(pharray0[i]);
		}
		for (int i = 0; i < pharray1.Length; i++)
		{
			Destroy(pharray1[i]);
		}

		initCoins();


	}

	[Command]
	void CmdSpawnCoin(Vector3 pos, int player)
	{
		GameObject c1 = (GameObject)Instantiate (coinPrefab, pos, Quaternion.identity);
		c1.GetComponent<coin> ().player = player;

		NetworkServer.Spawn (c1);
	}


	[Command]
	void CmdRemove(GameObject c1)
	{
		NetworkServer.Destroy(c1);
	}

	// Use this for initialization
	void Start () {
		ClientScene.RegisterPrefab (coinPrefab);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void initCoins()
	{
		//yield return new WaitForSeconds(1f);
		coinDict.Clear();

			GameObject[] allCoins;
			allCoins = GameObject.FindGameObjectsWithTag("coin");
			for (int i = 0; i < allCoins.Length; i++)
			{
				coinDict.Add(i, allCoins[i]);
			    allCoins[i].GetComponent<coin>().id = i;
			}
	}

}
