using UnityEngine;
using System.Collections;

public class CameraUpdate : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
            Debug.Log("camera moves");
            //FindCycles();

            if (Input.GetKey(KeyCode.W))
            {
                Camera.main.transform.Translate(new Vector3(0, 0.2f, 0.0f));
            }
            if (Input.GetKey(KeyCode.A))
            {
                Camera.main.transform.Translate(new Vector3(-0.2f, 0.0f, 0.0f));
            }
            if (Input.GetKey(KeyCode.S))
            {
                Camera.main.transform.Translate(new Vector3(0.0f, -0.2f, 0.0f));
            }
            if (Input.GetKey(KeyCode.D))
            {
                Camera.main.transform.Translate(new Vector3(0.2f, 0.0f, 0.0f));
            }


            if (Input.GetKey(KeyCode.Q))
            {
                //Camera.main.transform.Translate (new Vector3 (0.0f, 0.0f, -0.2f));
                Camera.main.orthographicSize += 0.2f;
            }
            if (Input.GetKey(KeyCode.E))
            {
                //Camera.main.transform.Translate (new Vector3 (0.0f, 0.0f, 0.2f));
                Camera.main.orthographicSize -= 0.2f;
            }


 

        }
}
