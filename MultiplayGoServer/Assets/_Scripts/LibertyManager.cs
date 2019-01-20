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

	// Get String of Moves connected to the
	// move provided. The move provided will
	// be the first index.
	public List<string> GetStringOfMoves (string move, List<Stone> stones, int boardSize)
	{
		List<string> moveString = new List<string> ();
		moveString.Add (move);

		int color = -1;
		int index = -1;

		for (int i=0;i<stones.Count;i++)
		{
			if(stones[i].coordinate == move){index = i;}
		}

		if (index == -1){Debug.Log ("ERROR: Index -1");}

		color = stones [index].color;

		if (color == -1)
		{
			Debug.Log ("Color: " + color.ToString());
			Debug.Log ("Index: " + index.ToString());
		}

		// Continue the loop unit the end of moveString
		// The first move is the move provided. 
		// Moves are added if the same color is connected
		// to the given move.
		for (int i=0; i < moveString.Count; i++)
		{
			List<string> touchingCoords = GetTouchingCoordinates (moveString[i], boardSize);

			for (int g=0; g<touchingCoords.Count; g++)
			{
				bool moveExist = CheckMoveExist (touchingCoords[g], stones);

				if (moveExist)
				{
					bool alreadyChecked = AlreadyChecked (moveString, touchingCoords[g]);
					bool sameColor = CompareColors (move, touchingCoords[g], stones);

					if(sameColor && !alreadyChecked){moveString.Add (touchingCoords[g]);}
				}
			}


			if (i > 1000){
				Debug.Log("ERROR: Infinite Loop");
				break;
			}
		}
			
		return moveString;
	}

	// Check if a move exist in the movesChecked array.
	// If so, return true.
	private bool AlreadyChecked (List<string> movesChecked, string move)
	{
		bool found = false;

		for (int i=0; i<movesChecked.Count; i++)
		{
			if(movesChecked[i] == move){found = true;}
		}

		return found;
	}

	// Get a list of moves touching the move provided.
	// Does not include the move provided.
	// Checks above, below, left, and right.
	// If the edge of the board, then it will skip that direction.
	private List<string> GetTouchingCoordinates (string move, int boardSize)
	{
		// Get the int coordinates of x and z
		int x = gobanManager.GetComponent<CoordinateManager> ().GetXInt (move, boardSize);
		int z = gobanManager.GetComponent<CoordinateManager> ().GetZInt (move, boardSize);

		List<string> touchingCoordinate = new List<string> ();

		// Check above
		if(z+1 <= boardSize)
		{
			string newCoord = gobanManager.GetComponent<CoordinateManager> ().GetCoordinate (x, (z+1), boardSize);

			if (newCoord != "")
			{
				touchingCoordinate.Add (newCoord);
			}
		}

		// Check below
		if(z-1 > 0)
		{
			string newCoord = gobanManager.GetComponent<CoordinateManager> ().GetCoordinate (x, (z-1), boardSize);

			if (newCoord != "")
			{
				touchingCoordinate.Add (newCoord);
			}
		}

		// Check left
		if(x-1 >= 0)
		{
			string newCoord = gobanManager.GetComponent<CoordinateManager> ().GetCoordinate ((x-1), z, boardSize);

			if (newCoord != "")
			{
				touchingCoordinate.Add (newCoord);
			}
		}

		// Check right
		if(x+1 != boardSize)
		{
			string newCoord = gobanManager.GetComponent<CoordinateManager> ().GetCoordinate ((x+1), z, boardSize);

			if (newCoord != "")
			{
				touchingCoordinate.Add (newCoord);
			}
		}
			
		return touchingCoordinate;
	}

	// Check if move exist in the stones on the board.
	private bool CheckMoveExist (string move, List<Stone> stones)
	{
		bool exist = false;

		for (int i=0; i<stones.Count; i++)
		{
			if (stones[i].coordinate == move){exist = true;}
		}

		return exist;
	}

	// Compare move colors.
	private bool CompareColors (string move, string newMove, List<Stone> stones)
	{
		bool sameColor = false;
		int index = -1;
		int newIndex = -1;

		for (int i=0; i<stones.Count; i++)
		{
			if (move == stones[i].coordinate){index = i;}
		}

		for (int i=0; i<stones.Count; i++)
		{
			if (newMove == stones[i].coordinate){newIndex = i;}
		}

		if (stones[index].color == stones [newIndex].color)
		{
			sameColor = true;
		}


		return sameColor;
	}
		

	// Get Liberties of a move.
	// Gets the list of stones connected to the move provided that 
	// are the same color and check if there is an empty space
	// next to the stones.
	public int GetLiberties (string move, List<Stone> stones, int boardSize)
	{
		int liberties = -1;

		List<string> movesToCheck = GetStringOfMoves (move, stones, boardSize);

		if (movesToCheck.Count > 0) {
			liberties = 0;
		} else {
			Debug.Log ("ERROR: No moves found.");
		}

		int loop = 0; // Infinite loop counter
		List<string> libertyCoordinates = new List<string>();
		for (int i=0; i<movesToCheck.Count; i++)
		{
			List<string> touchingCoords = GetTouchingCoordinates (movesToCheck [i], boardSize);

			for (int g=0; g<touchingCoords.Count; g++)
			{
				bool exist = CheckMoveExist (touchingCoords [g], stones);
				if(!exist){libertyCoordinates.Add (touchingCoords[g]);}
			}

			if (loop > 1000){Debug.Log ("ERROR: Infinite Loop."); break;}
		}

		libertyCoordinates = RemoveDuplicates (libertyCoordinates);

		liberties = libertyCoordinates.Count;
			
		return liberties;
	}

	// Get the liberties of the move provided.
	// Make a list<string> of moves with the coordinates
	// of the liberties.
	public List<string> GetLibertyCoordinates (string move, List<Stone> stones, int boardSize)
	{
		List<string> libertyCoordinates = new List<string> ();

		List<string> movesToCheck = GetStringOfMoves (move, stones, boardSize);

		int loop = 0; // Infinite loop counter
		for (int i=0; i<movesToCheck.Count; i++)
		{
			List<string> touchingCoords = GetTouchingCoordinates (movesToCheck [i], boardSize);

			for (int g=0; g<touchingCoords.Count; g++)
			{
				bool exist = CheckMoveExist (touchingCoords [g], stones);
				if(!exist){libertyCoordinates.Add (touchingCoords[g]);}
			}

			if (loop > 1000){Debug.Log ("ERROR: Infinite Loop."); break;}
		}

		libertyCoordinates = RemoveDuplicates (libertyCoordinates);

		if(libertyCoordinates.Count == 0){Debug.Log ("ERROR: No liberties found.");}

		return libertyCoordinates;
	}

	// Check for duplicate coordinates in the list<string> provided.
	// When a duplicate is found, the move is removed from the list.
	private List<string> RemoveDuplicates (List<string> moves)
	{
		int loopCounter = 0;
		for (int i=0; i<moves.Count; i++)
		{
			for(int g=0; g<moves.Count; g++)
			{
				loopCounter++;
				if (g != i && moves[i] == moves[g])
				{
					// If the move at i and g are the same coorinate
					// and i and g are not the same number, remove
					// that coordinate and restart the loops.
					moves.RemoveAt(i);
					i = 0;
					g = 0;

					if (loopCounter >= 10000){Debug.Log ("ERROR: Infinite Loop Found."); return moves;}
				}
			}
		}

		return moves;
	}

	// Check surrounding moves of the move played. If the colors are not the same, check the liberties.
	// If any of the surrounding moves have 0 liberties, return true that the move provided
	// captured something.
	public bool CheckForCapturing (string move, List<Stone> stones, int boardSize)
	{
		bool capturing = false;

		List<string> movesToCheck = GetTouchingCoordinates (move, boardSize);
		for (int i=0; i<movesToCheck.Count;i++)
		{
			bool moveExist = CheckMoveExist (movesToCheck[i], stones);
			if(moveExist)
			{
				int liberties = GetLiberties (movesToCheck[i], stones, boardSize);
				if(liberties == 0){ capturing = true;}
			}
		}

		return capturing;
	}
}
