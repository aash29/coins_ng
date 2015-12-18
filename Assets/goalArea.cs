using UnityEngine;
using System.Collections;

public class goalArea : MonoBehaviour {
	int[] coinsInside={0,0};
	public int modifier=1;
	// Use this for initialization
	void Start () {

	}

	void OnTriggerStay(Collider other) {
				coinsInside [other.GetComponentInParent<coin> ().player]++;
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
		globals.score [0] = globals.score [0] + coinsInside[0] * modifier;
		globals.score [1] = globals.score [1] + coinsInside[1] * modifier;

		coinsInside [0] = 0;
		coinsInside [1] = 0;
	}

}
