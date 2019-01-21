using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIScript : MonoBehaviour {

    // This script is to be used to help the AI decide
    // where to play.
    // This AI plays capture Go.

    // Gets a move to play for the AI.
	public string PlayAMove(int boardSize, int color, int botLevel)
    {
        string move = "";
		List<Stone> stones = GameObject.Find ("_StoneManager").GetComponent<StoneManagerScript> ().GetStones ();
		if (stones.Count > 0) {
			move = GetMove (boardSize, color, botLevel);
		} else {
			int x = (boardSize + 1) / 2;
			int z = (boardSize + 1) / 2;
			move = GameObject.Find ("_GobanManager").GetComponent<CoordinateManager>().CoordinateToString(x-1, z-1);
		}

		if (move == "") {
			Debug.Log ("ERROR: Couldn't find move.");
		}

        return move;
    }

    // Will play a move
    // Level 0 will play randomly
    // Level 1 can see atari
    // Level 2 will take an opponent's liberty
    // Level 3 will play on the group with the least amount of liberties.
	string GetMove (int boardSize, int color, int botLvl)
    {
		string move = "";
		List<string> moves = new List<string>();

		// Level 0
		// Play random move.
		move = RandomMove (color, boardSize);
		moves.Add (move);


		// Level 2
		// Make or Take liberties.
		if( botLvl >=2 )
		{
			List<string> lowestLibertyMoves = MakeTakeLiberty( color, boardSize );
			moves = AddMoves( moves, lowestLibertyMoves );
		}

		// Level 3
		// Play on the group with the lowest liberties.
		// Also sorts the liberties by weight.
		// Weight is determind by distance from the center.
		// The closer to the center, the higher the weight.
		if( botLvl >= 3 )
		{
			List<string> lowestLibertyMoves = SortedStonesByLiberties (color, boardSize);
			// Get the liberties of each stone and sort them by weight.
			List<string> sortedLiberties = GetSortedLibertiesByWeight(lowestLibertyMoves, boardSize);
			moves = AddMoves( moves, sortedLiberties );
		}

		// Level 1
		// Check for Atari.
		if( botLvl >= 1 )
		{
			List<string> atariCheck = CheckAtari( color, boardSize );
			moves = AddMoves( moves, atariCheck );		
		}

        // Check for Legal moves.
        // Don't play self atari unless capturing or saving atari
        for( int i=0; i<moves.Count; i++ )
        {
			if (move != "Resign")
			{
				bool legal = CheckForLegalMove( moves[i], color, boardSize );

	        	if( legal )
	        	{  
	        		move=moves[i];
					Debug.Log (move);
	        	}
			}
        }

		// If no legal move exist, then resign.
		if(moves.Count == 0)
		{
			// Resign.
			move = "Resign";
		}

       	return move;
    }

    // Gain or Take Liberties of lowest liberty group.
	List<string> MakeTakeLiberty ( int color, int boardSize )
    {
		List<Stone> stones = GameObject.Find ("_StoneManager").GetComponent<StoneManagerScript> ().GetStones ();
		List<string> sortedStonesByLiberties = SortedStonesByLiberties(color, boardSize);
		List<string> movesToAdd = new List<string>();

		for( int i=0; i<sortedStonesByLiberties.Count; i++ )
		{
			List<string> liberties = GameObject.Find ("_StoneManager")
				.GetComponent<LibertyManager>().GetLibertyCoordinates(sortedStonesByLiberties[i], stones, boardSize);

			for (int g=0; g<liberties.Count; g++)
			{
				if((bool)CheckForLegalMove(liberties[g], color, boardSize))
				{
					movesToAdd.Add( liberties[g] );
				}
			}
		}

		return movesToAdd;
    }

    // Add Moves
    List<string> AddMoves ( List<string> moves, List<string> movesToAdd )
    {
    	for( int i=0; i<movesToAdd.Count; i++ )
    	{
    		moves.Add( movesToAdd[i] ); 
    	}

    	return moves;
    }

    // Check for Atari
	List<string> CheckAtari ( int color, int boardSize )
    {
    	List<string> moves = new List<string>();

		// Check if player bot can capture.
		// Return moves that capture something.
		List<int> colors = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>().playerColors;
		List<string> placesToCapture = new List<string> ();

		for (int i=0; i<colors.Count; i++)
		{
			if(colors[i] != color)
			{
				placesToCapture = CheckForCaptures( colors[i], boardSize );
			}
		}

		// Add the moves.
	    if( placesToCapture.Count > 0 )
	    {
	        for( int i=0;i<placesToCapture.Count;i++ ){ moves.Add( placesToCapture[i] ); }
	    }

		// Save Atari
		// Find the groups in atari and returns the move
		// to gain a liberty.
		List<string> placesToSave = CheckForCaptures( color, boardSize );

	    if( placesToSave.Count > 0 )
	    {
			for( int i=0;i<placesToSave.Count;i++ ){ moves.Add( placesToSave[i] ); }
	    }

	    return moves;
    }

	// Check if the move provided is legal.
	bool CheckForLegalMove ( string move, int color, int boardSize )
	{
		bool legal = false;
		GameObject stoneManager = GameObject.Find ("_StoneManager");
		List<Stone> stones = stoneManager.GetComponent<StoneManagerScript> ().GetStones();

		bool moveExist = false;

		for (int i=0; i<stones.Count; i++)
		{
			if (stones[i].coordinate == move){moveExist = true;}
		}

		if (!moveExist) 
		{
			stoneManager.GetComponent<StoneManagerScript> ().CreateStone (move, color);

			int liberties = stoneManager.GetComponent<LibertyManager> ().GetLiberties (move, stones, boardSize);
			if (liberties > 0) {
				legal = true;
			} else {
				bool capturing = stoneManager.GetComponent<LibertyManager> ().CheckForCapturing (move, stones, boardSize);
				if (capturing) {
					legal = true;
				}
			}

			stoneManager.GetComponent<StoneManagerScript> ().RemoveMove (move);

		}

		return legal;
	}

	// Is a capture possible?
	// If so return the positions where you can capture
	List<string> CheckForCaptures ( int color, int boardSize )
	{
		List<string> movesInAtari = GetStonesInAtari( color, boardSize );
		List<string> moves = GetLibertiesOfMoves( movesInAtari, boardSize );

		return moves;
	}

	// Find all the moves with 1 liberty
	List<string> GetStonesInAtari ( int color, int boardSize )
	{
		List<Stone> stones = GameObject.Find("_StoneManager").GetComponent<StoneManagerScript>().GetStones();
		List<string> movesInAtari = new List<string>();

		for( int i=0; i<stones.Count; i++ )
		{
			if( stones[i].color == color )
			{
				int liberties = GameObject.Find("_StoneManager")
						.GetComponent<LibertyManager>().GetLiberties( stones[i].coordinate, stones,  boardSize );

				if( liberties == 1 ){ movesInAtari.Add( stones[i].coordinate ); }
			}
		}

		return movesInAtari;
	}

	// Finds all the liberty coordinates of all the moves provided.
	List<string> GetLibertiesOfMoves ( List<string> movesToCheck, int boardSize )
	{
		List<string> moves = new List<string>();
		List<Stone> stones = GameObject.Find("_StoneManager").GetComponent<StoneManagerScript>().GetStones();

		for( int i=0; i <movesToCheck.Count; i++ )
		{
			moves = GetLibertiesCoordinatesOfMove( movesToCheck[i], stones, boardSize );
		}

		return moves;
	}

	// Get liberties coordinates of the move provided.
	List<string> GetLibertiesCoordinatesOfMove ( string move, List<Stone> stones, int boardSize )
	{
		List<string> moves = new List<string> ();
		moves = GameObject.Find ("_StoneManager").GetComponent<LibertyManager> ().GetLibertyCoordinates (move, stones, boardSize);
		return moves;
	}

	// Gain liberties
	/*
	List<string> MovesToGainLiberties ( List<string> liberties, string color, int curLib )
	{
		Hashtable moves = new Hashtable();

		for( int i=0; i<liberties.Count; i++ )
		{
			if( !moves.ContainsKey(liberties[i]) )
			{
				int lib = this.GetComponent<_StoneManager>().GetLiberties( liberties[i], color );

				if( lib >= curLib )
				{
					moves.Add( liberties[i], lib );
				}
			}
		}

		List<string> orderedMoves = OrderedString( liberties, moves );
		List<string> reOrdered = new List<string>();

		for( int i=(orderedMoves.Count-1); i>=0; i--  ){ reOrdered.Add( orderedMoves[i] ); }

		return reOrdered;
	}*/



	// Get list of stones that are the given color in the list provided
	List<string> GetColoredStonesInList ( List<string> moves, int color )
	{
		List<string> sortedMoves = new List<string>();

		for( int i=0; i<moves.Count; i++ )
		{
			Stone stone = GameObject.Find ("_StoneManager").GetComponent<StoneManagerScript>().GetStone( moves[i] );
			if( stone.color == color ){ sortedMoves.Add( moves[i] ); }
		}

		return sortedMoves;
	}

    // If a group has two liberties, try to capture.

    // If I have two liberties, save myself.

    // Check for net.

    // Check for ladder.

    // Gets the liberties of the color given
    /*List<string> GetLiberties ( string color )
    {
    	List<string> moves = new List<string>();

    	List<Stone> stones = StonesOfColor( color );

    	List<string> pos = new List<string>();
		for(int i=0;i<stones.Count;i++){ pos.Add( stones[i].pos ); }

		moves = GetLiberties( pos );

    	return moves;
    }*/

    // Sort stones by number of liberties
	List<string> SortedStonesByLiberties (int Color, int boardSize)
    {
		List<Stone> stones = GameObject.Find("_StoneManager").GetComponent<StoneManagerScript>().GetStones();

    	Hashtable liberties = new Hashtable();
    	for( int i=0; i<stones.Count; i++ )
    	{
			if( !liberties.ContainsKey(stones[i].coordinate) )
    		{
				int lib = GameObject.Find("_StoneManager").GetComponent<LibertyManager>().GetLiberties(stones[i].coordinate, stones, boardSize);
				liberties.Add( stones[i].coordinate, lib );
    		}
    	}

    	List<string> stonePositions = new List<string>();

		for( int i=0; i<stones.Count;i++ ){ stonePositions.Add( stones[i].coordinate ); }

    	List<string> sortedStones = OrderedString( stonePositions, liberties );

    	return sortedStones;
    }
		

	/// <summary>
	/// Sorts the liberties by weight.
	/// Weight is determined by distance from Tengen.
	/// The closer a move is to tengen, the greater the weight.
	/// </summary>
	/// <returns>The liberties by weight.</returns>
	/// <param name="liberties">List of coordinates that are a liberty.</param>
	/// <param name="boardSize">Board size.</param>
	List<string> GetSortedLibertiesByWeight ( List<string> moves, int boardSize )
    {
		List<string> sortedLibs = new List<string> ();

		for( int i=0; i<moves.Count; i++ )
		{
			GameObject stoneManager = GameObject.Find ("_StoneManager");
			List<Stone> stones = stoneManager.GetComponent<StoneManagerScript> ().GetStones ();

			Hashtable weights = new Hashtable();
			List<string> liberties = new List<string> ();

			liberties = GameObject.Find ("_StoneManager").GetComponent<LibertyManager> ()
				.GetLibertyCoordinates (moves [i], stones, boardSize);
			Debug.Log (liberties.Count);
			for (int g=0; g<liberties.Count; g++)
			{
				if( !weights.ContainsKey(liberties[g]) )
				{ 
					int weight = GetWeightOfLiberty(liberties[g], boardSize );
					weights.Add( liberties[g], weight );
				}
			}
			List<string> sortedLiberties = OrderedString (liberties, weights);
			// sortedLiberties = 
			for (int g=0; g<sortedLibs.Count; g++)
			{
				sortedLibs.Add (sortedLiberties[g]);
			}
		}
			
		Debug.Log ("Test5");
    	return sortedLibs;
    }

	/// <summary>
	/// Gets the weight of liberty.
	/// Weight it determind by distance away from tenge.
	/// The closer to tengen, the greater the value.
	/// </summary>
	/// <returns>The weight of liberty.</returns>
	/// <param name="move">Coordinate of move is string format.</param>
	/// <param name="boardSize">Board size.</param>
    // Gets the weight of the given liberty.
	int GetWeightOfLiberty ( string move, int boardSize )
    {
		int x = WeightX(move, boardSize);
		int z = WeightZ(move, boardSize);

		int weight = 0;

		if( x > z ) { weight = x; }
		else if ( z > x ) { weight = z; }
		else { weight = x+1; }

		return weight;
    }

    // Get weight of x
	int WeightX ( string move, int boardSize )
    {
		int x = GameObject.Find ("_GobanManager").GetComponent<CoordinateManager> ().GetXInt (move, boardSize);
		int weight = GetWeight ((x+1), boardSize);


		return weight;
    }
    // Get weight of z
	int WeightZ ( string move, int boardSize )
    {
		int z = GameObject.Find ("_GobanManager").GetComponent<CoordinateManager> ().GetZInt (move, boardSize);
		int weight = GetWeight(z, boardSize);

		return weight;
    }

	/// <Summary>
	/// Get the weight of a given number.
	/// 0 is the highest weight.
	/// Weight is determined by distance from Tengen (Central Point).
	/// Distance is negative.
	///</Summary>
	private int GetWeight (int num, int boardSize)
	{
		int tengen = (boardSize + 1) / 2;
		int weight = 0;

		if (num == tengen) {
			weight = 0;
		} else if (num < tengen) {
			weight = num;
		} else if (num > tengen) {
			weight = -1 * (num - tengen);
		}

		return weight;
	}
	
    /// Play a random move.
	///<Summary>
	/// Picks a random legal move on the board.
	/// If no legal move can be found, it returns Resign.
	/// </Summaru>
	// TODO: Return Pass first and then resign.
	string RandomMove (int color, int boardSize)
    {
    	string move = "";

		List<string> coordinates = GameObject.Find ("_GobanManager").GetComponent<CoordinateManager>().GetAllCoordinates();
    	
		for(int i=0; i<coordinates.Count; i++)
		{
			bool legal = CheckForLegalMove (coordinates [i], color, boardSize);
			if (!legal) {
				coordinates.RemoveAt (i);
				i = 0;
			}
		}

		if (coordinates.Count == 0) {
			move = "Resign";
		} else {
			int ran = GameObject.Find ("_Scripts").GetComponent<RandomScript> ().RandInt (0, coordinates.Count);
			move = coordinates [ran];
		}


    	return move;
    }

    // Orders string by given values lowest to highest.
    List<string> OrderedString ( List<string> moves, Hashtable pairs )
    {
    	List<string> orderedString = new List<string>();

		int g = 0;

    	for( int i=0; i<=g; i++ )
    	{
			for( int j=0; j<moves.Count; j++ )
    		{
				if( pairs.ContainsKey( moves[j] ) )
				{
					if( (int)pairs[moves[j]] > g )
					{ 
						g = (int)pairs[moves[j]];
					}

					if( (int)pairs[moves[j]] == i )
		    		{
						orderedString.Add(moves[j]);
		    		}
	    		}
    		}
    	}

    	return orderedString;
    }
}
