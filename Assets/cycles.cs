using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

using System.Linq;


using akCyclesInUndirectedGraphs;
using EPPZGeometry;

public class cycles : MonoBehaviour {

	String logName;

	public float proximityRadius=4.0f;

	public bool calculatedCyclesThisPush=true;

	StreamWriter sw;
	
	static Material lineMaterial;

	List<List<int>>[] cyclesList = new List<List<int>>[2];

	List<List<Vector3>>[] coinCycles = new List<List<Vector3>>[2];

	List<List<Vector3>>[] edges = new List<List<Vector3>>[2];

	List<Vector3>[] allVertices = new List<Vector3>[2];

	static void CreateLineMaterial ()
	{
		if (!lineMaterial)
		{
			// Unity has a built-in shader that is useful for drawing
			// simple colored things.
			 lineMaterial = new Material ("Shader \"Lines/Colored Blended\" {" +
            "SubShader { Pass { " +
            "    Blend SrcAlpha OneMinusSrcAlpha " +
            "    ZWrite Off Cull Off Fog { Mode Off } " +
            "    BindChannels {" +
            "      Bind \"vertex\", vertex Bind \"color\", color }" +
            "} } }");
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;

		}
	}


	void Start()
	{
		/*
		logName = "count_fox" + (100 * UnityEngine.Random.value).ToString () + ".txt";
		sw = new StreamWriter(logName);
		sw.WriteLine ("lalala");
		sw.Flush ();
		*/
	}




	public bool IsPointInPolygon( Vector3 p, Vector3[] polygon )
	    {
	        double minX = polygon[ 0 ].x;
	        double maxX = polygon[ 0 ].x;
	        double minY = polygon[ 0 ].z;
	        double maxY = polygon[ 0 ].z;
	        for ( int i = 1 ; i < polygon.Length ; i++ )
	        {
	            Vector3 q = polygon[ i ];
	            minX = Math.Min( q.x, minX );
	            maxX = Math.Max( q.x, maxX );
	            minY = Math.Min( q.z, minY );
	            maxY = Math.Max( q.z, maxY );
	        }

	        if ( p.x < minX || p.x > maxX || p.z < minY || p.z > maxY )
	        {
	            return false;
	        }

	        // http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
	        bool inside = false;
	        for ( int i = 0, j = polygon.Length - 1 ; i < polygon.Length ; j = i++ )
	        {
	            if ( ( polygon[ i ].z > p.z ) != ( polygon[ j ].z > p.z ) &&
	                 p.x < ( polygon[ j ].x - polygon[ i ].x ) * ( p.z - polygon[ i ].z ) / ( polygon[ j ].z - polygon[ i ].z ) + polygon[ i ].x )
	            {
	                inside = !inside;
	            }
	        }

	        return inside;
	    }


	public bool IsPolygonInPolygon( Vector3[] p1, Vector3[] p2 )
	{


		for (int i =0; i<p1.Length;i++)
			if (!IsPointInPolygon(p1[i],p2))
			    return false;

		return true;
	}


	void deleteTrappedCoins(int player, List<List<Vector3>> cCycles)
	{
		var coins = GameObject.FindGameObjectsWithTag("coin");

		List<int> protectedCycles = new List<int>();

		/*

		for (int j = 0; j < coinCycles[player % 2].Count; j++)
		{
			for (int k = 0; k < coinCycles[player % 2].Count; k++)
			{
				if (IsPolygonInPolygon(coinCycles[player % 2][j].ToArray(), coinCycles[player % 2][k].ToArray()))
                    {
						protectedCycles.Add(j);
					}
            }

		}

		*/

		

		for ( int j = 0;  j<coinCycles[player%2].Count; j++)
		{
			for (int k = 0; k<coinCycles[(player+1)%2].Count; k++)
			{
				if (IsPolygonInPolygon(coinCycles[player%2][j].ToArray(), coinCycles[(player+1)%2][k].ToArray()) && !(protectedCycles.Contains(j)) )
				{

					foreach (int i in cyclesList[player%2][j].Distinct())
					{
						GameObject.Find("root1").GetComponent<setupLevel>().coinDict[i].GetComponent<Multitag> ().TagsSet.Add("dead");
						//GetComponentInParent<Player>().coinDict.Remove(i);
					}
				}
				
			}

		}

		foreach (var c0 in coins)
		{

			int cp1 = c0.GetComponent<coin>().player;
			if (coinCycles[(player) % 2] != null)
			{
				foreach (var cc in coinCycles[(player) % 2].ToArray())
				{
					if (IsPointInPolygon(c0.transform.position, cc.ToArray()))
					{
						if ((cp1 == player) )
						{
							c0.GetComponent<Multitag>().TagsSet.Add("connected");
						}

					}
				}
			}

		}
	

		
		foreach (var c0 in coins) 
		{
			int cp1 = c0.GetComponent<coin> ().player;
			if (cCycles!=null)
			{
				foreach (var cc in cCycles)
				{
					if (IsPointInPolygon(c0.transform.position, cc.ToArray()))
					{
							if ((cp1==player) && !c0.GetComponent<Multitag>().TagsSet.Contains("connected"))
							{
								c0.GetComponent<Multitag> ().TagsSet.Add("dead");
							}

					}
				}
			}

		}
	}


	public void cleanup()
	{
		List<GameObject> deadCoins = Multitag.FindGameObjectsWithTag ("dead").ToList ();

		for (int i=deadCoins.Count-1; i>-1; i--) 
		{
			var c1 = deadCoins [i];
			int deadKey = GameObject.Find("root1").GetComponent<setupLevel>().coinDict.FirstOrDefault(x => x.Value == c1).Key;
			GameObject.Find("root1").GetComponent<setupLevel>().coinDict.Remove(deadKey);
			c1.GetComponent<Multitag> ().TagsSet.Clear ();
			Destroy (c1);
		}
	}

	public void Update()
	{
		//sw.Close ();
		//sw = new StreamWriter(logName);


		GameObject.Find("root1").GetComponent<setupLevel>().initCoins();
        bool atRest=true;
        /*
		foreach (var c1 in (GameObject.FindGameObjectsWithTag ("coin")))
		{
			if (c1.transform.position.y<-1.0f)
				c1.GetComponent<Multitag> ().TagsSet.Add("dead");

			if (!c1.GetComponent<Rigidbody>().IsSleeping())
			{
				atRest=false;
				calculatedCyclesThisPush = false;
			}
		}

        */
		//if ((atRest) && (!calculatedCyclesThisPush)) {
		FindCycles (0, ref coinCycles [0], ref cyclesList[0]);
		FindCycles (1, ref coinCycles [1], ref cyclesList[1]);



		deleteTrappedCoins (0, coinCycles [1]);
		deleteTrappedCoins (1, coinCycles [0]);

		calculatedCyclesThisPush = true;
		//		}
		cleanup ();

		//sw.Flush ();

	}


	public List<List<Vector3>> FindCycles(int player, ref List<List<Vector3>> polygons, ref List<List<int>> cyclesOut)
	{
				//GameObject[] allCoins;
				Dictionary<int, GameObject> allCoins;
				List<GameObject> coins = new List<GameObject> ();
				List<int> subst = new List<int>();

		//allCoins = GameObject.FindGameObjectsWithTag ("coin");

				setupLevel sl1 = GameObject.Find("root1").GetComponent<setupLevel>();
                allCoins = sl1.coinDict;

				List<List<int>> g2 = new List<List<int>> ();

				List <Vector3> v2 = new List <Vector3> ();

		

				foreach(KeyValuePair<int, GameObject> entry in allCoins)
				{
				//for (int i = 0; i < allCoins.Count(); i++) {

						int cp1 = entry.Value.GetComponent<coin> ().player;
						if (cp1 == player) {
								v2.Add (entry.Value.transform.position);

								//sw.WriteLine ("Vertex number[" + (v2.Count - 1) + "]" + entry.Value.transform.position);

								coins.Add (entry.Value); 
								subst.Add (entry.Key);
								//Array.Remove(coins,c0);
						}
				}

				//List<List<Vector3>> polygons;
		
				List<List<Vector3>> edge = new List<List<Vector3>> ();
		
		
				int edgeNum = 0;
		
				for (int i = 0; i<coins.Count; i++) {
						coins [i].GetComponent<Multitag> ().TagsSet.Remove ("connected");
						for (int j = i+1; j<coins.Count(); j++) {
								GameObject c1 = coins [i];
								GameObject c2 = coins [j];
								Vector3 v1 = c1.transform.position - c2.transform.position;
				
								if (v1.magnitude < proximityRadius) {
										g2.Add (new List<int> {i,j});

										//sw.WriteLine ("Edge (" + i + "," + j + ") added");

										edge.Add (new List<Vector3> {c1.transform.position, c2.transform.position});
										edgeNum++;
								}
				
						}
			
				}




				List<int> ed0;
				for (int i=0; i<v2.Count; i++) {
						for (int j=0; j<g2.Count; j++) {
								//edH0.First();

								ed0 = g2 [j];
								Segment s1 = new Segment ();
								s1 = Segment.SegmentWithPoints (new Vector2 (v2 [ed0 [0]].x, v2 [ed0 [0]].z), new Vector2 (v2 [ed0 [1]].x, v2 [ed0 [1]].z));

								if ((i != ed0 [0]) && (i != ed0 [1]) && (s1.ContainsPoint (new Vector2 (v2 [i].x, v2 [i].z), 1e-4f))) {
										//List<int> ed01 = new List<int> {ed0[0],i};
										List<int> ed01 = new List<int> {ed0[0],i};
										//if (!g2.Contains(ed01))
										g2.Add (ed01);
										//List<int> ed02 = new List<int> {i,ed0[1]};
										List<int> ed02 = new List<int> {i,ed0[1]};
										//if (!g2.Contains(ed02))
										g2.Add (ed02);

										g2.Remove (ed0);

										//sw.WriteLine ("Edge (" + ed0 [0] + "," + ed0 [1] + ") divided to (" + ed0 [0] + "," + i + ") and (" + i + "," + ed0 [1] + ")");
								}
						}

				}

				removeDuplicates (ref g2, v2);


				Dictionary<int, List<int>> subdivs = new Dictionary<int, List<int>>();
				List<int> edge1,edge2;
				
				for (int i = 0; i<g2.Count; i++) 
				{
					subdivs.Add(i,new List<int> {g2[i][0],g2[i][1]});
				}

				for (int i = 0; i<g2.Count; i++)
						for (int j = i+1; j<g2.Count; j++) 
						{
							edge1=g2[i];
							edge2=g2[j];

							Segment s1 = new Segment();
							s1 = Segment.SegmentWithPoints(v2[edge1[0]].xz(), v2[edge1[1]].xz());
							
							Segment s2 = new Segment();
							s2 = Segment.SegmentWithPoints(v2[edge2[0]].xz(), v2[edge2[1]].xz());

							Vector2 vi;
			
							if (s1.IntersectionWithSegmentWithAccuracy(s2, 1e-6f, out vi))
							{
								int cn = addOrGetExisting(vi,ref v2);

								subdivs[i].Add(cn);
								subdivs[j].Add(cn);
							}

						
						}
		for (int i=0; i< subdivs.Keys.Count;i++) {
			int div = subdivs.Keys.ElementAt(i);
			subdivs[div] = subdivs [div].Distinct ().ToList();
			subdivs[div]=subdivs[div].OrderBy(item => ((v2[g2[div][0]]-v2[item]).magnitude)).ToList();
		}		


		List<List<int>> g3 = new List<List<int>> ();
		foreach (KeyValuePair<int, List<int>> kvp in subdivs) 
		{
			for (int i=0; i< kvp.Value.Count-1; i++)
			{
				g3.Add(new List<int> {kvp.Value[i], kvp.Value[i+1]} );
				//sw.WriteLine("g3 edge added (" + kvp.Value[i] +","  + kvp.Value[i+1] + ")");
			}
		}

		g2 = g3;


		removeDuplicates (ref g2, v2);
	
		//v2 = v2.Distinct ().ToList ();

		edges[player] = edge;
		allVertices [player] = v2;


		CGraph cg1 = new CGraph(g2, v2, sw);

		List<List<int>> cycles = cg1.findCycles2();
		//cyclesList = cycles;



		int cp=0;

		polygons = null;


		
		List<List<int>> onlyCoinCycles=new List<List<int>>();
		int ci=0;	
		foreach (List<int> cy in cycles) 
		{
			onlyCoinCycles.Add (new List<int>());

			for (int i = 0; i < cy.Count; i++) 
			{
				if (cy[i]< coins.Count)
					onlyCoinCycles[ci].Add(subst[cy[i]]);
			}
			ci++;
		}
		
		if (cycles != null) {
		

		polygons = new List<List<Vector3>>();

						foreach (List<int> cy in cycles) {
								string s = "" + cy [0];
								polygons.Add (new List<Vector3> ());
								polygons [cp].Add (v2 [cy [0]]);
								if (cy[0]< coins.Count)
								{
									coins [cy [0]].GetComponent<Multitag> ().TagsSet.Add ("connected");
									coins [cy [0]].GetComponent<Multitag> ().TagsSet.Add (cp.ToString());
								}
								for (int i = 1; i < cy.Count; i++) {
										s += "," + cy [i];
					
										Vector3 pos = v2 [cy [i]];

										polygons [cp].Add (pos);
										if (cy[i]< coins.Count)
											{
												coins [cy [i]].GetComponent<Multitag> ().TagsSet.Add ("connected");
												coins [cy [i]].GetComponent<Multitag> ().TagsSet.Add (cp.ToString());
											}
										//linerenderer.SetPosition (i, pos);
					
								}

								//polygons[cp].Add(coins[cy[0]].transform.position);

								cp++;
						}
		

		
		}

		cyclesOut = onlyCoinCycles;

		return polygons;
		
	}


	// Will be called after all regular rendering is done
	public void OnRenderObject ()
	{
		CreateLineMaterial ();
		// Apply the line material
		lineMaterial.SetPass (0);
		
		//GL.PushMatrix ();
		// Set transformation matrix for drawing to
		// match our transform
		//GL.MultMatrix (transform.localToWorldMatrix);
		
		// Draw lines


		try {
			/*
		foreach (var e1 in edges[0])
		{
			GL.Begin (GL.LINES);
			for (int i = 0; i < e1.Count; ++i)
			{
				// Vertex colors change from red to green
				GL.Color (new Color (0, 0.1f, 0, 0.8F));
				// One vertex at transform position
				//GL.Vertex3 (0, 0, 0);
				// Another vertex at edge of circle
				GL.Vertex3 (e1[i].x, 0.0f, e1[i].z);
			}
			GL.End ();
		}
		*/

		foreach (var v1 in allVertices[0])
		{
			GL.Begin (GL.QUADS);
				GL.Color (new Color (0, 0.1f, 0, 0.8F));
				GL.Vertex3 (v1.x-0.05f, 0.2f, v1.z-0.05f);
				GL.Vertex3 (v1.x-0.05f, 0.2f, v1.z+0.05f);
				GL.Vertex3 (v1.x+0.05f, 0.2f, v1.z+0.05f);
				GL.Vertex3 (v1.x+0.05f, 0.2f, v1.z-0.05f);
			GL.End ();
		}


			/*
		
		foreach (var e1 in edges[1])
		{
			GL.Begin (GL.LINES);
			for (int i = 0; i < e1.Count; ++i)
			{
				// Vertex colors change from red to green
				GL.Color (new Color (0.5f, 0.5f, 0, 0.8F));
				// One vertex at transform position
				//GL.Vertex3 (0, 0, 0);
				// Another vertex at edge of circle
				GL.Vertex3 (e1[i].x, 0.0f, e1[i].z);
			}
			GL.End ();

		}
		*/
		}
		catch {};

		Color[] colorArray = new Color[3];
		colorArray [0] = new Color (1.0f, 0.0f, 0.0f, 1f);
		colorArray [1] = new Color (0.0f, 1.0f, 0.0f, 1f);
		colorArray [2] = new Color (0.0f, 0.0f, 1.0f, 1f);
		int ccolor = 0;
		if (coinCycles [0] != null) {
						foreach (var cc in coinCycles[0]) {

								GL.Begin (GL.LINES);
								for (int i = 1; i < cc.Count; i++) {
										// Vertex colors change from red to green
										GL.Color (colorArray[ccolor%3]);

										GL.Vertex3 (cc [i - 1].x, 0.2f, cc [i - 1].z);
										GL.Vertex3 (cc [i].x, 0.2f, cc [i].z);
								}
								GL.End ();
								ccolor++;
						}
				}

		if (coinCycles [1] != null) {
						foreach (var cc in coinCycles[1]) {
				
								GL.Begin (GL.LINES);
								for (int i = 1; i < cc.Count; i++) {
										// Vertex colors change from red to green
										GL.Color (new Color (0.0f, 0.1f, 1.0f, 0.8F));
										// One vertex at transform position
										//GL.Vertex3 (0, 0, 0);
										// Another vertex at edge of circle
										GL.Vertex3 (cc[i-1].x, 0.2f, cc[i-1].z);
										GL.Vertex3 (cc[i].x, 0.2f, cc[i].z);
								}
								GL.End ();
						}
				}


		//GL.PopMatrix ();
	}

	public void removeDuplicates( ref List<List<int>> g2 , List<Vector3> v2)
	{
		List<int> edge1, edge2;
		
		for (int i =0; i< g2.Count; i++)
		for (int j=i+1; j < g2.Count; j++) {
			if (i!=j) 
			{
				edge1 = g2 [i];
				edge2 = g2 [j];
				
				if ( ( ((v2 [edge1 [0]] - v2 [edge2 [0]]).magnitude < 1e-4f) && ((v2 [edge1 [1]] - v2 [edge2 [1]]).magnitude < 1e-4f)) ||
				    ( ((v2 [edge1 [0]] - v2 [edge2 [1]]).magnitude < 1e-4f) && ((v2 [edge1 [1]] - v2 [edge2 [0]]).magnitude < 1e-4f)) )
				{
					g2.RemoveAt(j);
					
					//sw.WriteLine("Edges (" + edge1[0] +","  + edge1[1] + ") and (" + edge2[0] +","  + edge2[1] + ") have same coords, (" + edge2[0] +","  + edge2[1] + ") removed. ##" + i+ "," + j);
					
				}
			}
		}

	}
	public int addOrGetExisting(Vector3 vi,  ref List<Vector3> v2)
	{
				int cn;
				int nn = -1;
				int i = 0;
				foreach (var cv in v2) {
						float mn = (cv.xz () - vi.xz ()).magnitude;
						if (mn < 0.01f) {
								nn = i;
								break;
						}
						i++;
				}
		
				if (nn >= 0) {
						cn = nn;
				} else {
						cn = v2.Count;
						v2.Add (new Vector3 (vi.x, 0.1f, vi.y));
				}
		return cn;
		}

		public int findNodeInCycle(Vector3 vi,  List<Vector3> v2)
		{
			int cn;
			int nn = -1;
			int i = 0;
			foreach (var cv in v2) {
				float mn = (cv.xz () - vi.xz ()).magnitude;
				if (mn < 0.01f) {
					nn = i;
					break;
					}
					i++;
				}

			return nn;
		}
	/*
		public List<List<Vector3>> cycles2Polygons(List<List<int>> cycles)
		{
		List<List<Vector3>> polygons;

		int cp = 0;
		polygons = null;
		
		if (cycles != null) {
			
			polygons = new List<List<Vector3>>();
			
			foreach (List<int> cy in cycles) {
				string s = "" + cy [0];
				polygons.Add (new List<Vector3> ());
				polygons [cp].Add (v2 [cy [0]]);
				if (cy[0]< globals.coinDict.Count)
				{
					globals.coinDict[cy [0]].GetComponent<Multitag> ().TagsSet.Add ("connected");
					globals.coinDict [cy [0]].GetComponent<Multitag> ().TagsSet.Add (cp.ToString());
				}
				for (int i = 1; i < cy.Count; i++) {
					s += "," + cy [i];
					
					Vector3 pos = v2 [cy [i]];
					
					polygons [cp].Add (pos);
					if (cy[i]< globals.coinDict.Count)
					{
						globals.coinDict [cy [i]].GetComponent<Multitag> ().TagsSet.Add ("connected");
						globals.coinDict [cy [i]].GetComponent<Multitag> ().TagsSet.Add (cp.ToString());
					}
					//linerenderer.SetPosition (i, pos);
					
				}
				
				//polygons[cp].Add(coins[cy[0]].transform.position);
				
				cp++;
			}
		}
		
		return polygons;

		}
*/

}