using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SubworldToolkit;

public class ExtendedSubworldPlayer : ModPlayer
{
	/// <summary>
	/// Dictionary with world specific data indexed by 
	/// the world's Guid.
	/// </summary>
	public Dictionary<Guid, PlayerData> PlayerDataByGuid = [];

	/// <summary>
	/// The unique id saved upon joining a world. Used to access
	/// player data for the current world.
	/// </summary>
	public Guid MainWorldUniqueID;

	/// <summary>
	/// Determines if the logout subworld and position should 
	/// be restored upon entering a world for the first time.
	/// This boolean is set to true during <see cref="Initialize"/>,
	/// when the client opens the player selection menu.
	/// Cleared after joining a world.
	/// </summary>
	public bool RestoreLogout = false;

	public override void SaveData(TagCompound tag)
	{
		List<string> guids = [];

		PlayerDataByGuid ??= [];

		foreach (KeyValuePair<Guid, PlayerData> kvp in PlayerDataByGuid)
		{
			guids.Add(kvp.Key.ToString());

			TagCompound data = [];

			data[nameof(PlayerData.VisitedSubworlds)] = kvp.Value.VisitedSubworlds.ToList();
			data[nameof(PlayerData.LogoutSubworldID)] = kvp.Value.LogoutSubworldID;
			data[nameof(PlayerData.HasLogoutSubworld)] = kvp.Value.HasLogoutSubworld;
			data[nameof(PlayerData.HasLogoutPosition)] = kvp.Value.HasLogoutPosition;
			data[nameof(PlayerData.LogoutPosition)] = kvp.Value.LogoutPosition;

			tag[kvp.Key.ToString()] = data;
		}

		tag[nameof(PlayerDataByGuid)] = guids;
	}

	public override void LoadData(TagCompound tag)
	{
		PlayerDataByGuid = [];

		IList<string> guids = tag.GetList<string>(nameof(PlayerDataByGuid));
        foreach (string guidString in guids)
        {
			if (!tag.ContainsKey(guidString))
			{
				continue;
			}

			TagCompound data = tag.GetCompound(guidString);

			Guid guid = new(guidString);
			PlayerData playerData = new();

			if (data.ContainsKey(nameof(PlayerData.VisitedSubworlds)))
			{
				IList<string> visited = data.GetList<string>(nameof(PlayerData.VisitedSubworlds));
				foreach (string visitedString in visited)
				{
					playerData.VisitedSubworlds.Add(visitedString);
				}
			}

			playerData.HasLogoutSubworld = data.GetBool(nameof(PlayerData.HasLogoutSubworld));
			playerData.LogoutSubworldID = data.GetString(nameof(PlayerData.LogoutSubworldID));
			playerData.HasLogoutPosition = data.GetBool(nameof(PlayerData.HasLogoutPosition));
			playerData.LogoutPosition = data.Get<Vector2>(nameof(PlayerData.LogoutPosition));

			PlayerDataByGuid[guid] = playerData;
		}
    }

	public override void Initialize()
	{
		// Upon loading the player, signal to restore the
		// logged out subworld and position upon entering
		// any world.
		RestoreLogout = true;
	}

	public override void PreUpdate()
	{
		// Run the subworld logic for this player
		ExtendedSubworldSystem.current?.PreUpdate(Player);

		// Check if the current party is the owner of the player
		if (Main.netMode == NetmodeID.Server || Player.whoAmI != Main.myPlayer || Main.gameMenu || RestoreLogout)
		{
			return;
		}

		// Get the player's data for the current world
		PlayerData data = GetPlayerData();

		// Save the logout data
		data.HasLogoutSubworld = ExtendedSubworldSystem.current?.SaveLogoutSubworld == true;
		data.LogoutSubworldID = ExtendedSubworldSystem.current?.FullName ?? "";
		data.HasLogoutPosition = ExtendedSubworldSystem.current?.SaveLogoutPosition == true;
		data.LogoutPosition = Player.Bottom;
	}

	public override void OnEnterWorld()
	{
		// Run the subworld logic for this player
		ExtendedSubworldSystem.current?.OnEnterWorld(Player);

		if (Main.netMode == NetmodeID.Server || Player.whoAmI != Main.myPlayer)
		{
			return;
		}

		// Save the unique id of the main world. Used to access
		// the world specific data.
		if (!SubworldSystem.AnyActive())
		{
			MainWorldUniqueID = Main.ActiveWorldFileData.UniqueId;
		}

		// Get the player's data for the current world
		PlayerData data = GetPlayerData();

		// Entered the main world
		if (!SubworldSystem.AnyActive())
		{
			// Try to resore the saved subworld
			if (RestoreLogout && (!data.HasLogoutSubworld || !SubworldSystem.Enter(data.LogoutSubworldID)))
			{
				RestoreLogout = false;
			}
		}

		// Entered a subworld
		else
		{
			// Add the subworld to the visited list
			data.VisitedSubworlds ??= [];
			data.VisitedSubworlds.Add(SubworldSystem.Current.FullName);

			// Check if there is a logout position set
			if (RestoreLogout && data.LogoutSubworldID == SubworldSystem.Current.FullName && data.HasLogoutPosition)
			{
				Player.Bottom = data.LogoutPosition;
			}

			// Clear the logout restore attempt
			RestoreLogout = false;
		}
	}

	public PlayerData GetPlayerData()
	{
		// Verify the Player data dictionary
		PlayerDataByGuid ??= [];

		// Check if the Player data for the current world exists
		if (!PlayerDataByGuid.TryGetValue(MainWorldUniqueID, out PlayerData data))
		{
			data = new();
			PlayerDataByGuid.Add(MainWorldUniqueID, data);
		}

		return data;
	}

	#region ---------- ModPlayer => ExtendedSubworld ----------
	public override void PreUpdateBuffs() => ExtendedSubworldSystem.Current?.PreUpdateBuffs(Player);

	public override void PreUpdateMovement() => ExtendedSubworldSystem.Current?.PreUpdateMovement(Player);

	public override void PostUpdateRunSpeeds() => ExtendedSubworldSystem.Current?.PostUpdateRunSpeeds(Player);

	public override void UpdateEquips() => ExtendedSubworldSystem.Current?.UpdateEquips(Player);

	public override void PostUpdateEquips() => ExtendedSubworldSystem.Current?.PostUpdateEquips(Player);

	public override void PostUpdate() => ExtendedSubworldSystem.Current?.PostUpdate(Player);

	public override bool CanStartExtraJump(ExtraJump jump) => ExtendedSubworldSystem.Current?.CanStartExtraJump(Player, jump) != false;

	public override void OnRespawn() => ExtendedSubworldSystem.Current?.OnRespawn(Player);
	#endregion
}

public class PlayerData
{
	/// <summary>
	/// The subworlds this player as visited at least
	/// a single time. Saves the filenames of the 
	/// subworld.
	/// </summary>
	public HashSet<string> VisitedSubworlds = [];

	/// <summary>
	/// Determines if the player has a logout subworld set. 
	/// If this is true the player will be send to the world 
	/// where the exited.
	/// </summary>
	public bool HasLogoutSubworld = false;
	
	/// <summary>
	/// The fullname of the subworld the player exited. Used 
	/// to send the player back to their logout subworld upon 
	/// entering the world.
	/// </summary>
	public string LogoutSubworldID = "";

	/// <summary>
	/// Determines if the player has a logout position set.
	/// If this is true the player will be placed back on the
	/// the position <see cref="LogoutPosition"/> upon entering.
	/// </summary>
	public bool HasLogoutPosition = false;
	
	/// <summary>
	/// The loguout position of the player, this is the position
	/// the player will get when entering the logout subworld.
	/// Only used when <see cref="HasLogoutPosition"/> is enabled.
	/// </summary>
	public Vector2 LogoutPosition = Vector2.Zero;
}