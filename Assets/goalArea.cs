using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class goalArea : NetworkBehaviour {
	int[] coinsInside={0,0};
	public int modifier=1;
	// Use this for initialization
	void Start () {

	}

	[Command]
	public void CmdResetCoin(int id)
	{
		coin c1 = GameObject.Find("root1").GetComponent<setupLevel> ().coinDict [id].GetComponent<coin>();
		c1.counted = 1;
	}

	[Command]
	public void CmdDecreaseScore(int cp)
	{
		GameObject.Find("root1").GetComponent<globals> ().score [(cp+1)%2] -= 1;
	}


	void OnTriggerStay(Collider other) {
		coin oc = other.GetComponentInParent<coin> ();
		int cp = other.GetComponentInParent<coin> ().player;
		if (oc.counted==0) {
			coinsInside [cp]++;
			oc.counted = 1;
			CmdResetCoin(oc.id);
			CmdDecreaseScore (cp);
			//GameObject.Find("root1").GetComponent<globals> ().score [(cp+1)%2] -= 1;
		}
	}

	/*
	void OnTriggerEnter(Collider other) {

		coinsInside[other.GetComponentInParent<coin>().player]++;
	}

	void OnTriggerExit(Collider other) {
		coinsInside[other.GetComponentInParent<coin>().player]--;
	}
	*/

	void Update()
	{

		
	}

	void LateUpdate() {
		//globals.score [0] = globals.score [0] - coinsInside[1] * modifier;
		//globals.score [1] = globals.score [1] - coinsInside[0] * modifier;

		//coinsInside [0] = 0;
		//coinsInside [1] = 0;
	}

}
