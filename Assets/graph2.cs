//using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using EPPZGeometry;
using System.IO;


namespace akCyclesInUndirectedGraphs
{

 

    class CGraph
    {

		//StreamWriter sw;

        public List<List<int>> graph;

        public List<Vector3> vertices;

		//static List<List<int>> neighbors;
		public Dictionary<int, List<int>> neighbors = new Dictionary<int, List<int>>();
 

        public List<int[]> cycles = new List<int[]>();

		public List<int> protectedCoins = new List<int>();

		public CGraph(List<List<int>> iGraph, List<Vector3> iVertices, StreamWriter sw1)
        {
            graph=iGraph;
            vertices = iVertices;
			//sw = sw1;
			//sw = new StreamWriter("findCycles.txt");


			//neighbors = new List<List<int>> ();

			for (int i = 0; i < graph.Count; i++) 
			{
				if (neighbors.ContainsKey(graph[i][0]))
				{
					if (!neighbors[graph[i][0]].Contains(graph[i][1]))
						neighbors[graph[i][0]].Add(graph[i][1]);
				}
				else
				{
					neighbors.Add(graph[i][0], new List<int>());
					neighbors[graph[i][0]].Add(graph[i][1]);
				}

				if (neighbors.ContainsKey(graph[i][1]))
				{
					if (!neighbors[graph[i][1]].Contains(graph[i][0]))
						neighbors[graph[i][1]].Add(graph[i][0]);
				}
				else
				{
					neighbors.Add(graph[i][1], new List<int>());
					neighbors[graph[i][1]].Add(graph[i][0]);
				}
			}

        }


		public List<List<int>> findCycles2()
		{

			HashSet<int> unprocessedVertices = new HashSet<int>();

			List<List<int>> cyclesList = new List<List<int>> ();

			int currentCycleNum = 0;

			for (int i=0; i<graph.Count; i++) 
			{
				unprocessedVertices.Add(graph[i][0]);
				unprocessedVertices.Add(graph[i][1]);
			}
			int uv = 0;

			while (unprocessedVertices.Count>0) {
				uv++;
				if (uv>100)
					break;

								int leftmost = findLeftVertex (unprocessedVertices);
								if (leftmost == -1) {
										return null;
								}
								//sw.WriteLine("Leftmost vertex [" + leftmost + "]" + vertices[leftmost]);


								int imax = steepestNeighbor (leftmost);

								//sw.WriteLine("Steepest neighbor [" + imax + "]" + vertices[imax]);


			

								List<int> currentCycle = new List<int> ();

								currentCycle.Add (leftmost);
								currentCycle.Add (imax);

								int prevNode = leftmost;
								int currentNode = imax;
								int nextCWNeighbor = getNextCWNeighbor (imax, prevNode);

								//sw.WriteLine("Next vertex [" + nextCWNeighbor + "]" + vertices[nextCWNeighbor]);

								currentCycle.Add (nextCWNeighbor);



								prevNode = currentNode;
								currentNode = nextCWNeighbor;
								int cc = 0;
								

								while (currentNode!=leftmost) {
										cc++;
										if (cc > 100) 
												break;

										//currentCycle.RemoveAt(currentCycle.Count-1);
										nextCWNeighbor = getNextCWNeighbor (currentNode, prevNode);
										//sw.WriteLine("Next vertex [" + nextCWNeighbor + "]" + vertices[nextCWNeighbor]);
										currentCycle.Add (nextCWNeighbor);
										prevNode = currentNode;
										currentNode = nextCWNeighbor;

										//if (currentCycle[currentCycle.Count-1]==currentCycle[currentCycle.Count-3])
								}



								foreach (int n1 in currentCycle)
								{
									unprocessedVertices.Remove(n1);
								}

								currentCycle=cycle_red (currentCycle,0);				
				


								if (currentCycle.Count>0)
								{
								
									cyclesList.Add (currentCycle);

									List<Vector2> p1 = cycleToPolygon2(currentCycle) ;

									Polygon pe = Polygon.PolygonWithPointList(p1);
								
									for (int i=0; i< vertices.Count;i++)
									{
								
									if (pe.ContainsPoint(vertices[i].xz()) || pe.PermiterContainsPoint(vertices[i].xz()) )
										//if ( pe.PermiterContainsPoint(vertices[i].xz()) )
										//if (pe.ContainsPoint(vertices[i].xz()))
											unprocessedVertices.Remove(i);
											//protectedCoins.Add(i);
									}
								}
						}

			return cyclesList;

		}


		public List<int> cycle_red(List<int> cycle, int n)
		{
			for (int i=0;i<cycle.Count; i++) 
			{
				List<int> nc = neighbors[cycle[i]].Intersect(cycle).ToList();
				if (nc.Count<2)
				{
					cycle.RemoveAt(i);
					n=1;
				}
			}
			if (n==0) 
				return cycle;
			else
				return cycle_red (cycle,0);

		}

		public int getNextCWNeighbor(int i,int p)
		{
			List<int> neighArray = new List<int>(neighbors [i]);
			neighArray.Remove (p);

			float minAngle = 100.0f;
			if (neighArray.Count==0)
			{
				return p;
			}

			/*
			int minInd = neighArray.Aggregate((agg, next) => angle(vertices[p],vertices[next],vertices[i]) < angle(vertices[p],vertices[agg],vertices[i]) ? next : agg);

			return minInd;
			*/

			int minInd = neighArray [0];

			foreach (var n1 in neighArray) 
			{
				float a1 = angle(vertices[p],vertices[n1],vertices[i]);
				if (Mathf.Abs(a1-minAngle)<1e-4)				{
					if (Vector3.Distance(vertices[i],vertices[n1])<Vector3.Distance(vertices[i],vertices[minInd]))
					{
						minAngle=a1;
						minInd=n1;
					}
				}
				else
				if (a1<minAngle)
				{
					minAngle=a1;
					minInd=n1;
				}

			}
			return minInd;

		}




		float angle(Vector3 a, Vector3 b, Vector3 center)
		{
			Vector3 ca = a - center;
			Vector3 cb = b - center;

			Vector3 t = Vector3.Normalize (ca);

			float bx = Vector3.Dot (t, cb);

			Vector3 n = ca;
			n.x = ca.z;
			n.z = -ca.x;

			n.Normalize ();

			float by = Vector3.Dot (n, cb);

			float angle = Mathf.Atan2 (by, bx);

			return (angle > 0 ? angle : (2*Mathf.PI + angle));
		}

	


        public int findLeftVertex(HashSet<int> nodes)
        {
            int indMin = -1;
            float xMin = 100000;
            foreach (int n1 in nodes)
			//for (int i=0; i<vertices.Count; i++) 
            {
                if (vertices[n1].x<xMin)
                {
                    xMin=vertices[n1].x;
                    indMin = n1;
                }
            }
            return indMin;
        }


		public int steepestNeighbor(int leftmost)
		{
			float kmax = -100000;
			int imax = neighbors[leftmost][0];
			
			List<int> verticalNeighbors = new List<int>();
			
			foreach (int n1 in neighbors[leftmost]) 
			{
				
				if ((Mathf.Abs(vertices[n1].x-vertices[leftmost].x)<1e-3) && (vertices[n1].z>vertices[leftmost].z) )
				{
					verticalNeighbors.Add(n1);
				}
				else
				{
					
					float k=(vertices[n1].z-vertices[leftmost].z)/(vertices[n1].x-vertices[leftmost].x);
					if (k>kmax)
					{
						kmax=k;
						imax = n1;
					}
				} 
			}
			
			if (verticalNeighbors.Count > 0) 
			{

				imax=verticalNeighbors.Aggregate((agg, next) => (vertices[next].z > vertices[agg].z ? next : agg));
				
			}

			return imax;

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

		public List<Vector2> cycleToPolygon2(List<int> cycle)
		{
			List<Vector2> polygon = new List<Vector2> ();
		

			polygon.Add (vertices[cycle [0]].xz ());
			for (int i = 1; i < cycle.Count; i++) 
				{
			
				Vector2 pos = vertices[cycle [i]].xz ();
					polygon.Add (pos);
				}
			return polygon;
		}

		public List<Vector3> cycleToPolygon(List<int> cycle)
		{
			List<Vector3> polygon = new List<Vector3> ();
			
			
			polygon.Add (vertices[cycle [0]]);
			for (int i = 1; i < cycle.Count; i++) 
			{
				
				Vector3 pos = vertices[cycle [i]];
				polygon.Add (pos);
			}
			return polygon;
		}


    }
}