using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using akCyclesInUndirectedGraphs;
public class select : NetworkBehaviour {

	public float baseforce;
	[SyncVar]
	public GameObject curCoin = null;



	void Start () {

	}

	
	
	public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float y) 
	{
		Ray ray = Camera.main.ScreenPointToRay(screenPosition);
		Plane xz = new Plane(new Vector3(0, 1, 0), new Vector3(0, y, 0));
		float distance;
		xz.Raycast(ray, out distance);
	
		return ray.GetPoint(distance);
	}

	[Command]
	IEnumerator CmdEndTurn()
	{


		if (GameObject.Find("root1").GetComponent<globals>().getNumPlayers()==1) 
		{
			GetComponentInParent<Player>().playerID = (GetComponentInParent<Player>().playerID + 1) % 2;
		}




		//globals.score [0] = globals.score [0] - coinsInside[1] * modifier;
		//globals.score [1] = globals.score [1] - coinsInside[0] * modifier;


		Debug.Log(GetComponentInParent<Player>().playerID);
		selectCoin(false, curCoin);
		curCoin=null;
		GetComponentInParent<Player>().force = 1;
		CmdUncountCoins (0);
		yield return new WaitForSeconds(1f);
		CmdUncountCoins (1);

	}


	[Command]
	public void CmdUncountCoins(int t)
	{
		foreach (var c1 in (GameObject.FindGameObjectsWithTag ("coin"))) {
			c1.GetComponent<coin>().counted=t;
		}
	}


	

	void startTurn()
	{
		selectCoin(false, curCoin);
		curCoin=null;
		//GetComponentInParent<Player>().force = 1;
		GameObject.Find("root1").GetComponent<globals>().curPlayer=(GameObject.Find("root1").GetComponent<globals>().curPlayer+1)%2;
	}

	

	void selectCoin(bool sel, GameObject coin)
	{
		if (coin != null) {
			/*

						Material[] mats = curCoin.GetComponent<Renderer>().materials;
						List<Material> lmat = new List<Material>(mats);

			    	if (sel)
					{
						  lmat.Add(Resources.Load("coin_color_selected") as Material);
					}
					else
					{
						lmat.RemoveAt(lmat.Count-1);

						Vector3 ev = new Vector3 (0.0f, 0.0f, 0.0f);
						LineRenderer linerenderer =  GetComponent<LineRenderer>();
						linerenderer.SetPosition(0,ev);
						linerenderer.SetPosition(1,ev);
					}
					coin.GetComponent<Renderer>().materials = lmat.ToArray();
					
			*/			
		

				}
	}


	[Command]
	public void CmdChangeCurPlayer()
	{
		GameObject.Find("root1").GetComponent<globals> ().curPlayer = (GameObject.Find("root1").GetComponent<globals> ().curPlayer + 1 )% 2;
	}
	



    [Command]
	void CmdHandleClick(Vector3 ev, int button, float force)
	{

        Debug.Log("clicked");
        if (button == 0) {

						if (curCoin == null) {
								RaycastHit hit = new RaycastHit ();
								Ray ray = new Ray (Camera.main.transform.position, ev - Camera.main.transform.position);
								if (Physics.Raycast (ray, out hit)) {
										Debug.Log ("Hit something");
										if (hit.collider.gameObject.tag == "coin") {
												GameObject cc = hit.collider.gameObject;
						
												coin c1 = cc.GetComponent<coin> ();
												int cp = c1.player;
												if (cp == GameObject.Find("root1").GetComponent<globals> ().curPlayer) {
														curCoin = cc;
														selectCoin (true, curCoin);
												}
						
										}
								}
						} else {

								Vector3 hr = (ev - curCoin.transform.position);
								hr.y = 0;
								hr.Normalize ();
								float magn = baseforce * force;
								hr = hr*magn;
								GetComponentInParent<Player>().force-=force;
                                GetComponent<forceBar>().barDisplay-=force;
								//				hr.Normalize();
								/*
								if ((hr).magnitude > maxforce) {
										hr.Normalize ();
										hr = hr * maxforce;
								}
								*/
								Debug.Log("Coin pushed");
                                Debug.Log(hr);
                                curCoin.GetComponent<Rigidbody>().AddForce(hr,ForceMode.VelocityChange);
								selectCoin(false, curCoin);
								curCoin=null;
			}
		}

		if (button == 1) 
		{
			selectCoin(false, curCoin);
			curCoin=null;
		}

	}
	[Command]
	void CmdSetForce(float arg){
		GetComponent<forceBar>().barDisplay+=arg;
		float limspeed = Mathf.Min (curCoin.GetComponent<coin> ().limit_speed, GetComponentInParent<Player>().force);
		if (GetComponent<forceBar> ().barDisplay > limspeed)
						GetComponent<forceBar> ().barDisplay = limspeed;
		if (GetComponent<forceBar>().barDisplay<0.0f)
			GetComponent<forceBar>().barDisplay=0.0f;
		}

	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
            return;


        if (Input.GetKey (KeyCode.W)) {
			Camera.main.transform.Translate (new Vector3 (0, 0.2f, 0.0f));
		}
		if (Input.GetKey (KeyCode.A)) {
			Camera.main.transform.Translate (new Vector3 (-0.2f, 0.0f, 0.0f));
		}
		if (Input.GetKey (KeyCode.S)) {
			Camera.main.transform.Translate (new Vector3 (0.0f, -0.2f, 0.0f));
		}
		if (Input.GetKey (KeyCode.D)) {
			Camera.main.transform.Translate (new Vector3 (0.2f, 0.0f, 0.0f));
		}
		
		
		if (Input.GetKey (KeyCode.Q)) {
			//Camera.main.transform.Translate (new Vector3 (0.0f, 0.0f, -0.2f));
			Camera.main.orthographicSize+=0.2f;
		}
		if (Input.GetKey (KeyCode.E)) {
			//Camera.main.transform.Translate (new Vector3 (0.0f, 0.0f, 0.2f));
			Camera.main.orthographicSize-=0.2f;
		}



		if (Input.GetKey (KeyCode.Z)) {
			if (curCoin !=  null)
				CmdSetForce(0.01f);
		}
		
		if (Input.GetKey (KeyCode.C)) {
			if (curCoin !=  null)
				CmdSetForce(-0.01f);

		}
		


        Vector3 ev = GetWorldPositionOnPlane(Input.mousePosition,0);
						float barForce = GetComponent<forceBar>().barDisplay;
						ev.y = 0.0f;


		if (GameObject.Find("root1").GetComponent<globals>().curPlayer == GetComponentInParent<Player>().playerID)
		{

			if (curCoin != null)
			{
				LineRenderer linerenderer = GetComponent<LineRenderer>();
				linerenderer.SetPosition(0, curCoin.transform.position);
				linerenderer.SetPosition(1, ev);
			}
			else
			{
				LineRenderer linerenderer = GetComponent<LineRenderer>();
				linerenderer.SetPosition(0, new Vector3(0.0f,0.0f,0.0f));
				linerenderer.SetPosition(1, new Vector3(0.0f, 0.0f, 0.0f));

			}


			if (Input.GetMouseButtonDown (0)) {
            	CmdHandleClick(ev, 0, barForce);

			}

			if (Input.GetKeyUp (KeyCode.Return)) {
								StartCoroutine("endTurn");
								CmdChangeCurPlayer();
			}
			if (Input.GetMouseButtonDown (1)) {
				CmdHandleClick(ev, 1, barForce);
			}

			if (Input.GetKeyUp (KeyCode.Return)) {
				CmdEndTurn ();
				CmdChangeCurPlayer();
			}


			if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel"))>0.001f){
				if (curCoin !=  null)
				CmdSetForce(Input.GetAxis("Mouse ScrollWheel")*0.2f);
			}

			




				}
		}
	
	
}