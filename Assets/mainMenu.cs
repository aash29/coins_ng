using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;



public class mainMenu : MonoBehaviour {

	public int selectedLevel=0;

	public Font f;
	public Canvas levelMenu; 
	public Canvas networkMenu; 

	public string ip = "127.0.0.1";
	public int port = 25008;

	// Use this for initialization
	void Start () {
		levelMenu = levelMenu.GetComponent<Canvas> ();
		levelMenu.enabled = false;

		//networkMenu = networkMenu.GetComponent<Canvas> ();
		//networkMenu.enabled = false;

		/*
		var IPfield = gameObject.GetComponent<InputField>();

		var se= new InputField.SubmitEvent();
		se.AddListener(SubmitIP);
		IPfield.onEndEdit = se;
*/
	}

	void OnGUI () {
        /*
        GameObject inputFieldGo = GameObject.Find("IPfield");
		InputField inputFieldCo = inputFieldGo.GetComponent<InputField>();
		ip = inputFieldCo.text;

		GameObject portFieldGo = GameObject.Find("portField");
		InputField portFieldCo = portFieldGo.GetComponent<InputField>();

		int port_num = port;
		if( int.TryParse( portFieldCo.text, out port_num ) )
			port = port_num;
            */
		//Debug.Log(inputFieldCo.text);

	}

	public void showLevelMenu()
	{
		levelMenu.enabled = true;
	}

	public void showNetworkMenu()
	{
		//networkMenu.enabled = true;
		levelMenu.enabled = true;
	}

	public void joinGame()
	{
		//Network.Connect( ip, port );
		//NetworkLevelLoader.Instance.LoadLevel("game");
		//Application.LoadLevel(1);
	}

    /*
	public void hostGame()
	{
		Network.InitializeServer (1, port, true);
	}

    
	void OnPlayerConnected(NetworkPlayer player) 
	{
		Application.LoadLevel(1);
	}


	public void go()
	{
		Application.LoadLevel(selectedLevel);
	}
    
	*/
	public void loadLevel(string level)
	{
		//selectedLevel = level;
        GetComponent<NetworkLobbyManager>().playScene = level;
		
		//Application.LoadLevel(level);
	}
	
	public void colorButton(Button sender)
	{
		
		foreach(GameObject fooObj in GameObject.FindGameObjectsWithTag("levelButton"))
		{
			Button b1 = fooObj.GetComponent<Button>();
			ColorBlock cb=b1.colors;
			cb.highlightedColor = Color.white;
			b1.colors=cb;
		}
		
		
		ColorBlock cb1 = sender.colors; 
		cb1.highlightedColor = Color.red;
		sender.colors=cb1;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
