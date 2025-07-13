using Microsoft.Xna.Framework;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria.ModLoader;
using System;
using System.Reflection;

namespace SubworldToolkit;

public class ExtendedSubworldSystem : ModSystem
{
	#region ---------- Variables ----------
	public static ExtendedSubworld Current => current;

	internal static ExtendedSubworld current;

	public static List<Subworld> Subworlds;
	public static Dictionary<string, Subworld> SubworldFromName;
	public static Dictionary<Subworld, int> SubworldToID;
	public static Dictionary<Type, int> SubworldTypeToID;
	#endregion

	public override void Load()
	{
		Subworlds = [];
		SubworldFromName = [];
		SubworldToID = [];
		SubworldTypeToID = [];

		List<Subworld> subworlds = typeof(SubworldSystem).GetField("subworlds", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as List<Subworld>;
		for (int i = 0; i < subworlds.Count; i++)
		{
			Subworlds.Add(subworlds[i]);
			SubworldFromName.Add(subworlds[i].FullName, subworlds[i]);
			SubworldToID.Add(subworlds[i], i);
			SubworldTypeToID.Add(subworlds[i].GetType(), i);
		}
	}

	public override void Unload()
	{
		Subworlds?.Clear();
		Subworlds = null;

		SubworldFromName?.Clear();
		SubworldFromName = null;

		SubworldToID?.Clear();
		SubworldToID = null;

		SubworldTypeToID?.Clear();
		SubworldTypeToID = null;
	}

	#region ---------- ModSystem => ExtendedSubworld ----------
	public override void ClearWorld() => Current?.ClearWorld();

	public override void ModifyLightingBrightness(ref float scale) => Current?.ModifyLightingBrightness(ref scale);

	public override void ModifyTimeRate(ref double timeRate, ref double tileUpdateRate, ref double eventUpdateRate) => Current?.ModifyTimeRate(ref timeRate, ref tileUpdateRate, ref eventUpdateRate);

	public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor) => Current?.ModifySunLightColor(ref tileColor, ref backgroundColor);

	public override void PreUpdateWorld() => Current?.PreUpdateWorld();

	public override void PostUpdateWorld() => current?.PostUpdateWorld();

	public override void PreUpdateTime() => Current?.PreUpdateTime();

	public override void PostUpdateTime() => Current?.PostUpdateTime();

	public override void PreUpdateDusts() => Current?.PreUpdateDusts();

	public override void PostUpdateDusts() => current?.PostUpdateDusts();

	public override void PreUpdateEntities() => Current?.PreUpdateEntities();

	public override void PreUpdateGores() => Current?.PreUpdateGores();

	public override void PostUpdateGores() => current?.PostUpdateGores();

	public override void PreUpdateInvasions() => Current?.PreUpdateInvasions();

	public override void PostUpdateInvasions() => current?.PostUpdateInvasions();

	public override void PreUpdateItems() => Current?.PreUpdateItems();

	public override void PostUpdateItems() => current?.PostUpdateItems();

	public override void PreUpdateNPCs() => Current?.PreUpdateNPCs();

	public override void PostUpdateNPCs() => current?.PostUpdateNPCs();

	public override void PreUpdatePlayers() => Current?.PreUpdatePlayers();

	public override void PostUpdatePlayers() => current?.PostUpdatePlayers();

	public override void PreUpdateProjectiles() => Current?.PreUpdateProjectiles();

	public override void PostUpdateProjectiles() => current?.PostUpdateProjectiles();

	public override void PostUpdateEverything() => Current?.PostUpdateEverything();

	public override void UpdateUI(GameTime gameTime) => Current?.UpdateUI(gameTime);
	#endregion
}