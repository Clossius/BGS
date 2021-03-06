﻿using System.Collections;
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

	public List<string> GetAllCoordinates ()
	{
		List<string> coordinates = new List<string> ();

		if (points.Count == 0){Debug.Log ("ERROR: GetAllCoordinates called before points could be initialized.");}

		for (int i=0; i<points.Count; i++)
		{
			coordinates.Add (points[i].coordinate);
		}

		return coordinates;
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

		if (num < 0 || num >= letters.Count*2){Debug.Log ("ERROR: Number provided out of Range. Num: " + num.ToString());}

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
	public string CoordinateToString ( int x, int z )
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
	public int GetXInt (string coordinate, int boardSize)
	{
		List<string> letters = GetLetters ();
		int index = -1;

		for (int i=0; i<(letters.Count*2); i++)
		{
			for (int g=1; g<=boardSize;g++)
			{
				if (i < letters.Count) {
					string coor = letters [i] + (g).ToString ();
					if (coor == coordinate) {
						index = i;
					}
				} else {
					string coor = letters [i-letters.Count] + letters[i-letters.Count] + (g).ToString ();
					if (coor == coordinate) {
						index = i;
					}
				}

			}
		}

		if (index == -1){Debug.Log ("ERROR: -1 Could not find coordinate.");}

		return index;
	}

	// Return the z int coordinate.
	public int GetZInt (string coordinate, int boardSize)
	{
		List<string> letters = GetLetters ();
		int index = -1;

		for (int i=0; i<(letters.Count*2); i++)
		{
			for (int g=1; g<=boardSize;g++)
			{
				if (i < letters.Count) {
					string coor = letters [i] + (g).ToString ();
					if (coor == coordinate) {
						index = g;
					}
				} else {
					string coor = letters [i-letters.Count] + letters[i-letters.Count] + (g).ToString ();
					if (coor == coordinate) {
						index = g;
					}
				}

			}
		}

		if (index == -1){Debug.Log ("ERROR: -1 Could not find coordinate.");}

		return index;
	}
		
	/// <summary>
	/// Convert the given int x and int z to
	/// a coordinate. Returns "" if coordinate does
	/// not exist.
	/// </summary>
	/// <returns>Returns a Coordinate.
	/// Returns "" if coordinate does
	/// not exist.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	/// <param name="boardSize">Board size.</param>
	public string GetCoordinate (int x, int z, int boardSize)
	{
		string move = "";
		string letter = "";
		if (x > -1 && x < boardSize){letter = GetLetter (x);}

		if(z > 0 && z <= boardSize && letter != "")
		{
			move = letter + z.ToString ();
		}

		bool moveExist = CoordinateExist (move);

		if(!moveExist){move = "";}

		return move;
	}

	/// <summary>
	/// Check if the given move exist as a point
	/// on the board.
	/// </summary>
	/// <returns><c>true</c>, if move exist, <c>false</c> otherwise.</returns>
	/// <param name="move">Move.</param>
	public bool CoordinateExist (string move)
	{
		bool exist = false;

		for (int i=0; i<points.Count; i++)
		{
			if(points[i].coordinate == move){exist = true;}
		}

		return exist;
	}
}
