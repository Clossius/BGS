using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour
{
	[Header("Objects")]
	public GameObject tutorialMenu;
	public GameObject nextButton;
	public GameObject subMenu;
	public TextMeshProUGUI text;
	public TextMeshProUGUI name;
	public Image image;

	[Header("Sprites")]
	public Sprite character1;
	public Sprite character2;

	// Local Variables
	private int campaignLevel = 0;
	private int commentIndex = 0;
	private int nameIndex = 0;

	// List
	List<string> comments;
	List<Sprite> sprites;
	List<string> names;

    // Start is called before the first frame update
    void Start()
    {
		InitializeSprites ();
		InitializeNames ();
		InitializeComments ();
    }

    // Initialize the comments for the campaign.
	private void InitializeComments ()
	{
		comments = new List<string> () {
			"Oh! Hello there~", // 0
			"Hmm? This? This is Go. It's a fantastic game! Would you like to play?", // 1
			"Great! We are playing Capture Go. The rules are simple. The first one to surround "+
				"and the other players stone or stones win.", // 2
			"Let's try playing a game! I will set up the board.", // 3
			"Normally we play on an empty board, " +
				"but in this case we will give you an advantage. Black goes first.", // 4
			"Game Start!", // 5
			"Sorry, but no worries! You can try again!", // 6
			"Great job! You got it!", // 7
			"Not bad. But let's see how you do without handicap.", // 8
			"Oh, this is " + names[1] + ". He is another beginner like you.", // 9
			"You two should try playing!", // 10
			"Hehe, it won't be as easy as her little test.", // 11
			"Game Start!" // 12
		};
	}

	private void InitializeSprites ()
	{
		sprites = new List<Sprite> () {
			character1,
			character2
		};
	}

	private void InitializeNames ()
	{
		names = new List<string> () {
			"QiWei",
			"Foshy"
		};
	}

	private void LoadComment ()
	{
		nextButton.SetActive (true);

		if (commentIndex < 0 || commentIndex >= comments.Count){Debug.Log ("ERROR: Index out of range."); return;}

		text.text = comments[commentIndex];
		Debug.Log (commentIndex.ToString() + ": " + comments[commentIndex]);

		if (commentIndex == comments.Count)
		{
			nextButton.SetActive (false);
		}

		if (commentIndex == 4)
		{
			GameObject.Find ("_StoneManager").GetComponent<StoneManagerScript> ().LoadSpecificStoneSet (1);

		} else if (commentIndex == 5)
		{
			StartNewGame (1);
		} else if (commentIndex == 6) {
			commentIndex = 2;
		} else if (commentIndex == 8)
		{
			image.sprite = sprites [1];
			name.text = names [1];
		} else if (commentIndex == 9)
		{
			image.sprite = sprites [0];
			name.text = names [0];
		} else if (commentIndex == 11)
		{
			image.sprite = sprites [1];
			name.text = names [1];
		} else if (commentIndex == 12) 
		{
			StartNewGame(0);
		}

		commentIndex++;
		Debug.Log (commentIndex);
	}

	private void StartNewGame (int stoneSettings)
	{
		nextButton.SetActive (false);
		GameObject roomManager = GameObject.FindGameObjectWithTag ("RoomManager");
		//roomManager.GetComponent<RoomManager> ().OnPlayerJoined (names[nameIndex]);

		Debug.Log (nameIndex.ToString() + " : " + names.Count);
		Debug.Log (names[nameIndex]);
		roomManager.GetComponent<RoomManager> ().StartGameForCampaign (names[nameIndex], stoneSettings);
		commentIndex--;
	}

	public void NextButtonClicked ()
	{
		LoadComment ();
	}

	public void LoadCampaign (int level)
	{
		campaignLevel = level;

		if (campaignLevel == 0)
		{
			image.sprite = sprites [nameIndex];
			name.text = names [nameIndex];;
			LoadComment();
			subMenu.SetActive (false);
		}
	}

	/// <summary>
	/// Called when a game is finished during the tutorial.
	/// </summary>
	/// <param name="won">If set to <c>true</c> won.</param>
	public void PlayedGame (bool won)
	{
		commentIndex++;
		if (won) {
			commentIndex++;
		}

		LoadComment ();
	}

	public string GetNPCName ()
	{
		return names[nameIndex];
	}
}
