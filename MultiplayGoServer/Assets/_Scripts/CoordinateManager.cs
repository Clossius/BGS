using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class CoordinateManager : MonoBehaviour {

	// Point class with information about coordinate point.
	public class Point
	{
		public int x;
		public int z;
		public float xPos;
		public float zPos;
		public string coordinate;

		public Point ()
		{
			
		}
	}

	List<Point> points;

	// Initalize coordinate list.
	public void LoadCoordinateSettings ()
	{
		points = new List<Point> ();
	}

	// Destroy coordinates in memory.
	public void DestroyCoordinates ()
	{
		points.Clear ();
	}

	// Determine coordinate positions.
	// Creates the list of coordinate points.
	// Requires the distance between points and the board size.

	public void CreatePoints (float dis, int size)
	{
		LoadCoordinateSettings ();

		float startPoint = ((size - 1) / 2) * dis;

		for (int x=0; x<size; x++ )
		{
			for (int z=0; z<size; z++)
			{
				Point newPoint = new Point();
				newPoint.xPos = (startPoint*(-1)) + (x * dis);
				newPoint.zPos = (startPoint*(-1)) + (z * dis);
				newPoint.x = x + 1;
				newPoint.z = z + 1;
				newPoint.coordinate = CoordinateToString (x, z);

				points.Add (newPoint);

				//Vector3 newStone = new Vector3 (newPoint.xPos, 0.0f, newPoint.zPos);
			}
		}
	}

	// Translate click pos into coordinate
	public string ClickToCoordinate ( Vector3 pos )
	{
		int x = -1;
		int z = -1;

		float distance = GameObject.Find ("_GobanManager").GetComponent<DrawLineScript> ().distance;

		for( int i=0;i<points.Count;i++ )
		{
			float localDistanceX = pos.x - points[i].xPos;
			if (Mathf.Abs(localDistanceX) < (distance/2)) { x = points[i].x; }

			float localDistanceZ = pos.z - points[i].zPos;
			if (Mathf.Abs(localDistanceZ) < (distance/2)) { z = points[i].z; }

		}

		string newPos = "";
		if (x != -1 && z != -1) {
			newPos = CoordinateToString (x-1, z-1);
		}

		return newPos;
	}

	public string GetLetter (int num)
	{
		List<string> letters = GetLetters ();

		string letter = "";

		if (num < letters.Count && num >= 0) {
			letter = letters [num];
		} else if (num < (letters.Count * 2)) {
			num = num - letters.Count;
			letter = letters [num] + letters[num];
		}

		return letter;
	}

	private List<string> GetLetters ()
	{
		List<string> letters = new List<string> ();
		letters.Add ("A");
		letters.Add ("B");
		letters.Add ("C");
		letters.Add ("D");
		letters.Add ("E");
		letters.Add ("F");
		letters.Add ("G");
		letters.Add ("H");
		letters.Add ("J");
		letters.Add ("K");
		letters.Add ("L");
		letters.Add ("M");
		letters.Add ("N");
		letters.Add ("O");
		letters.Add ("P");
		letters.Add ("Q");
		letters.Add ("R");
		letters.Add ("S");
		letters.Add ("T");
		letters.Add ("U");
		letters.Add ("V");
		letters.Add ("W");
		letters.Add ("X");
		letters.Add ("Y");
		letters.Add ("Z");

		return letters;
	}

	// Translate int coordinate into string coordinate.
	string CoordinateToString ( int x, int z )
	{
		

		string newCoordinate = GetLetter (x);


		newCoordinate = newCoordinate + (z+1).ToString ();


		return newCoordinate;
	}

	// Return Vector 3 from string Coordinate
	public Vector3 CoordinateToVector3 (string pos)
	{
		Point point = new Point ();
		for (int i=0;i<points.Count;i++) 
		{ 
			if (points [i].coordinate == pos) {
				point = points [i];
			} 
		}

		Vector3 newPos = new Vector3 (point.xPos, 0.0f, point.zPos);

		return newPos;
	}


	// Return the x int coordinate.
	public int GetXInt (string coordinate)
	{
		List<string> letters = GetLetters ();
		int index = -1;
		int maxSize = GameObject.Find ("_GobanManager").GetComponent<DrawLineScript> ().GetMaxBoardSize ();

		for (int i=0; i<(letters.Count*2); i++)
		{
			for (int g=0; g<maxSize;g++)
			{
				if (i < letters.Count) {
					string coor = letters [i] + g.ToString ();
					if (coor == coordinate) {
						index = i;
					}
				} else {
					string coor = letters [i-letters.Count] + letters[i-letters.Count] + g.ToString ();
					if (coor == coordinate) {
						index = i;
					}
				}

			}
		}

		if (index == -1){Debug.Log ("ERROD: -1 Could not find coordinate.");}

		return index;
	}

	// Return the z int coordinate.
	public int GetZInt (string coordinate)
	{
		List<string> letters = GetLetters ();
		int index = -1;
		int maxSize = GameObject.Find ("_GobanManager").GetComponent<DrawLineScript> ().GetMaxBoardSize ();

		for (int i=0; i<(letters.Count*2); i++)
		{
			for (int g=0; g<maxSize;g++)
			{
				if (i < letters.Count) {
					string coor = letters [i] + g.ToString ();
					if (coor == coordinate) {
						index = g;
					}
				} else {
					string coor = letters [i-letters.Count] + letters[i-letters.Count] + g.ToString ();
					if (coor == coordinate) {
						index = g;
					}
				}

			}
		}

		if (index == -1){Debug.Log ("ERROD: -1 Could not find coordinate.");}

		return index;
	}
}
