using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class forceBar : NetworkBehaviour {
	[SyncVar]
	public float barDisplay; //current progress

	[SyncVar]
	public float forceLeft; //current progress

	public Texture2D emptyTex;
	public Texture2D fullTex;

	void Start()
	{
	
    }

	void OnGUI() {
		if (!isLocalPlayer)
			return;

		Vector2 size = new Vector2(fullTex.width, fullTex.height);
		Vector2 pos = new Vector2(Screen.width-size.x,100);

		GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));


		GUI.DrawTextureWithTexCoords(new Rect(0, size.y * (1.0f - forceLeft), size.x, size.y * forceLeft), emptyTex, new Rect(0, 0.0f, 1.0f, forceLeft),true);
		GUI.DrawTextureWithTexCoords(new Rect(0, size.y * (1.0f - barDisplay), size.x, size.y* barDisplay), fullTex, new Rect(0, 0.0f, 1.0f, barDisplay ));

	
		GUI.EndGroup();
		
	}
	
	void Update()
	{
		forceLeft= GetComponentInParent<Player>().force;
	}
}
