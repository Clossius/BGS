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
		InitializeComments ();
		InitializeSprites ();
		InitializeNames ();
    }

    // Initialize the comments for the campaign.
	private void InitializeComments ()
	{
		comments = new List<string> () {
			"Oh! Hello there~",
			"Hmm? This? This is Go. It's a fantastic game! Would you like to play?",
			"Great! We are playing Capture Go. The rules are simple. The first one to surround "+
				"and the other players stone or stones win.",
			"Let's try playing a game! I will set up the board.", 
			"Normally we play on an empty board, " +
				"but in this case we will give you an advantage. Black goes first.",
			"Game Start!",
			"Sorry, but no worries! You can try again!",
			"Great job! You got it!"
		};
	}

	private void InitializeSprites ()
	{
		sprites = new List<Sprite> () {
			character1
		};
	}

	private void InitializeNames ()
	{
		names = new List<string> () {
			"QiWei"
		};
	}

	private void LoadComment ()
	{
		nextButton.SetActive (true);

		if (commentIndex < 0 || commentIndex >= comments.Count){Debug.Log ("ERROR: Index out of range."); return;}

		text.text = comments[commentIndex];

		if (commentIndex == comments.Count)
		{
			nextButton.SetActive (false);
		}

		if (commentIndex == 4)
		{
			GameObject.Find ("_StoneManager").GetComponent<StoneManagerScript> ().LoadSpecificStoneSet (0);

		} else if (commentIndex == 5)
		{
			nextButton.SetActive (false);
			GameObject roomManager = GameObject.FindGameObjectWithTag ("RoomManager");
			roomManager.GetComponent<RoomManager> ().OnPlayerJoined (names[nameIndex]);

			roomManager.GetComponent<RoomManager> ().SetPlayerColor (0, 0);
			roomManager.GetComponent<RoomManager> ().SetPlayerColor (1, 1);

			roomManager.GetComponent<RoomManager> ().StartGameForCampaign ();
		} else if (commentIndex == 6) {
			commentIndex = 2;
		}

		commentIndex++;
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
