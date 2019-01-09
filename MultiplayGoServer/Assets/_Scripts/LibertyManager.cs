using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class LibertyManager : MonoBehaviour {

	private GameObject gobanManager;

	// Initialize settings
	private void Start ()
	{
		gobanManager = GameObject.Find ("_GobanManager");

		if(gobanManager == null){Debug.Log ("ERROR: Could not find Goban Manager.");}
	}

	// Get String of Moves
	public List<string> GetStringOfMoves (List<string> playedMoves, List<int> moveColors, string move)
	{
		List<string> moveString = new List<string> ();
		moveString.Add (move);

		int color = -1;
		int index = -1;

		for (int i=0;i<playedMoves.Count;i++)
		{
			if(playedMoves[i] == move){index = i;}
		}

		color = moveColors [index];

		if (color == -1 || index == -1){
			Debug.Log ("ERROR: Found -1");
			Debug.Log ("Color: " + color.ToString());
			Debug.Log ("Index: " + index.ToString());
		}

		int count=0;
		while (count < moveString.Count)
		{
			count++;
			int x = gobanManager.GetComponent<CoordinateManager> ().GetXInt (move);
			int z = gobanManager.GetComponent<CoordinateManager> ().GetZInt (move);

			string newCoord;
			bool moveExist = false;

			// Check above
			newCoord = gobanManager.GetComponent<CoordinateManager>().GetLetter(x);
			newCoord = newCoord + (z+1).ToString ();
			moveExist = CheckMoveExist(newCoord, playedMoves);
			if (moveExist)
			{
				bool sameColor = CompareColors (move, newCoord, playedMoves, moveColors);
				if (sameColor)
				{
					moveString.Add (newCoord);
				}
			}


			// Check below
			newCoord = gobanManager.GetComponent<CoordinateManager>().GetLetter(x);
			newCoord = newCoord + (z-1).ToString ();
			moveExist = CheckMoveExist(newCoord, playedMoves);
			if (moveExist)
			{
				bool sameColor = CompareColors (move, newCoord, playedMoves, moveColors);
				if (sameColor)
				{
					moveString.Add (newCoord);
				}
			}

			// Check left
			newCoord = gobanManager.GetComponent<CoordinateManager>().GetLetter(x-1);
			newCoord = newCoord + (z).ToString ();
			moveExist = CheckMoveExist(newCoord, playedMoves);
			if (moveExist)
			{
				bool sameColor = CompareColors (move, newCoord, playedMoves, moveColors);
				if (sameColor)
				{
					moveString.Add (newCoord);
				}
			}

			// Check right
			newCoord = gobanManager.GetComponent<CoordinateManager>().GetLetter(x+1);
			newCoord = newCoord + (z).ToString ();
			moveExist = CheckMoveExist(newCoord, playedMoves);
			if (moveExist)
			{
				bool sameColor = CompareColors (move, newCoord, playedMoves, moveColors);
				if (sameColor)
				{
					moveString.Add (newCoord);
				}
			}

			if (count > 1000){
				Debug.Log("ERROR: Infinite Loop");
				break;
			}
		}

		return moveString;
	}

	// Check if move exist.
	private bool CheckMoveExist (string move, List<string> moves)
	{
		bool exist = false;

		for (int i=0; i<moves.Count; i++)
		{
			if (moves[i] == move){exist = true;}
		}

		return exist;
	}

	// Compare move colors.
	private bool CompareColors (string move, string newMove, List<string> moves, List<int> colors)
	{
		bool sameColor = false;
		int index = -1;
		int newIndex = -1;

		for (int i=0; i<moves.Count; i++)
		{
			if (move == moves[i]){index = i;}
		}

		for (int i=0; i<moves.Count; i++)
		{
			if (newMove == moves[i]){newIndex = i;}
		}

		if (colors[index] == colors [newIndex])
		{
			sameColor = true;
		}


		return sameColor;
	}

	// Get Liberties of String

	public int GetLiberties (string move, List<string> moves, List<int> colors, int boardSize)
	{
		int liberties = -1;

		List<string> movesToCheck = GetStringOfMoves (moves, colors, move);

		if (movesToCheck.Count > 0){liberties = 0;}

		for (int i=0; i<movesToCheck.Count; i++)
		{

			int x = gobanManager.GetComponent<CoordinateManager> ().GetXInt (move);
			int z = gobanManager.GetComponent<CoordinateManager> ().GetZInt (move);

			string newCoord;
			bool moveExist = false;

			// Check above
			newCoord = gobanManager.GetComponent<CoordinateManager>().GetLetter(x);
			newCoord = newCoord + (z+1).ToString ();
			moveExist = CheckMoveExist(newCoord, moves);
			if (!moveExist)
			{
				bool edgeOfBoard = false;
				if (x == boardSize){ edgeOfBoard = true;}
				if (x == 0) { edgeOfBoard = true; }
				if (z == boardSize) { edgeOfBoard = true; }
				if (z == 0) { edgeOfBoard = true; }

				if (!edgeOfBoard)
				{
					liberties++;
				}
			}

			// Check below
			newCoord = gobanManager.GetComponent<CoordinateManager>().GetLetter(x);
			newCoord = newCoord + (z-1).ToString ();
			moveExist = CheckMoveExist(newCoord, moves);
			if (!moveExist)
			{
				bool edgeOfBoard = false;
				if (x == boardSize){ edgeOfBoard = true;}
				if (x == 0) { edgeOfBoard = true; }
				if (z == boardSize) { edgeOfBoard = true; }
				if (z == 0) { edgeOfBoard = true; }

				if (!edgeOfBoard)
				{
					liberties++;
				}
			}

			// Check left
			newCoord = gobanManager.GetComponent<CoordinateManager>().GetLetter(x-1);
			newCoord = newCoord + (z).ToString ();
			moveExist = CheckMoveExist(newCoord, moves);
			if (!moveExist)
			{
				bool edgeOfBoard = false;
				if (x == boardSize){ edgeOfBoard = true;}
				if (x == 0) { edgeOfBoard = true; }
				if (z == boardSize) { edgeOfBoard = true; }
				if (z == 0) { edgeOfBoard = true; }

				if (!edgeOfBoard)
				{
					liberties++;
				}
			}

			// Check right
			newCoord = gobanManager.GetComponent<CoordinateManager>().GetLetter(x+1);
			newCoord = newCoord + (z+1).ToString ();
			moveExist = CheckMoveExist(newCoord, moves);
			if (!moveExist)
			{
				bool edgeOfBoard = false;
				if (x == boardSize){ edgeOfBoard = true;}
				if (x == 0) { edgeOfBoard = true; }
				if (z == boardSize) { edgeOfBoard = true; }
				if (z == 0) { edgeOfBoard = true; }

				if (!edgeOfBoard)
				{
					liberties++;
				}
			}

		}

		if (liberties == -1){Debug.Log ("ERROR: -1 Liberties could not be found.");}

		return liberties;
	}

}
