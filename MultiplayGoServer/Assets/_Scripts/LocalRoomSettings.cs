using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalSettings
{
	public int boardSize;
	public int ruleSet;
	public int maxPlayers;
	public bool roomVisible;
	public bool roomJoinable;
	public int botLevel;
	public int campaignLevel;
}

/// <summary>
/// Local room settings.
/// To be used to store the settings for the room localy.
/// These settings will be used when searching for and creating rooms.
/// </summary>

public class LocalRoomSettings : MonoBehaviour
{
	public LocalSettings localSettings;
    // Start is called before the first frame update
    void Start()
    {
		localSettings = new LocalSettings ();
		LoadCaptureGoSettings (); // Capture Go loaded by default.
    }

	public void LoadCaptureGoSettings ()
	{
		localSettings.boardSize = 9;
		localSettings.ruleSet = 0;
		localSettings.maxPlayers = 2;
		localSettings.roomVisible = true;
		localSettings.roomJoinable = true;
		localSettings.botLevel = 3;
		localSettings.campaignLevel = 0;
	}

	public void LoadCampaignSettings ()
	{
		localSettings.boardSize = 9;
		localSettings.ruleSet = 2;
		localSettings.maxPlayers = 2;
		localSettings.roomVisible = false;
		localSettings.roomJoinable = false;
		localSettings.botLevel = 0;
		localSettings.campaignLevel = 0;
	}

	public void LoadBotSettings (int botLevel)
	{
		localSettings.boardSize = 9;
		localSettings.ruleSet = 1;
		localSettings.maxPlayers = 2;
		localSettings.roomVisible = false;
		localSettings.roomJoinable = false;
		localSettings.botLevel = botLevel;
		localSettings.campaignLevel = 0;
	}

	public LocalSettings GetLocalRoomSettings ()
	{
		return localSettings;
	}

	public void UpdateRuleSet (int ruleSet)
	{
		localSettings.ruleSet = ruleSet;
	}

	public void UpdateMaxPlayers (int maxPlayers)
	{
		localSettings.maxPlayers = maxPlayers;
	}

	public void UpdateRoomVisible (bool visible)
	{
		localSettings.roomVisible = visible;
	}

	public void UpdateRoomJoinable (bool joinable)
	{
		localSettings.roomJoinable = joinable;
	}

	public void UpdateBoardSize (int boardSize)
	{
		localSettings.boardSize = boardSize;
	}

	public void UpdateBotSettings (int level)
	{
		localSettings.botLevel = level;
	}

	public void LoadSettingsByRuleSet (int ruleSet)
	{
		if (ruleSet == 0)
		{
			LoadCaptureGoSettings ();
		}

		else if (ruleSet == 1)
		{
			LoadBotSettings (3);
		}

		else if (ruleSet == 2)
		{
			LoadCampaignSettings ();
		}
	}
}
