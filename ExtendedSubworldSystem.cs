using Microsoft.Xna.Framework;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria.ModLoader;
using System;
using System.Reflection;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using System.IO;

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

	internal static bool SetTime;
	internal static int Time;
	internal static bool DayTime;
	internal static bool FastForwardTimeToDawn;
	internal static int SundialCooldown;
	internal static bool FastForwardTimeToDusk;
	internal static int MoondialCooldown;

	internal static bool SetMoonPhase;
	internal static int MoonPhase;

	internal static bool SetBloodMoon;
	internal static bool BloodMoon;

	internal static bool SetPumpkinMoon;
	internal static bool PumpkinMoon;

	internal static bool SetSnowMoon;
	internal static bool SnowMoon;

	internal static bool SetSolarEclipse;
	internal static bool SolarEclipse;
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

	public override void PreUpdateTime()
	{
		if (SetTime)
		{
			Main.time = Time;
			Main.dayTime = DayTime;
			Main.fastForwardTimeToDawn = FastForwardTimeToDawn;
			Main.sundialCooldown = SundialCooldown;
			Main.fastForwardTimeToDusk = FastForwardTimeToDusk;
			Main.moondialCooldown = MoondialCooldown;
			SetTime = false;
		}

		if (SetMoonPhase)
		{ 
			Main.moonPhase = MoonPhase;
			SetMoonPhase = false;
		}

		if (SetPumpkinMoon)
		{ 
			Main.pumpkinMoon = PumpkinMoon;
			SetPumpkinMoon = false;
		}

		if (SetBloodMoon)
		{ 
			Main.bloodMoon = BloodMoon;
			SetBloodMoon = false;
		}

		if (SetSnowMoon)
		{ 
			Main.snowMoon = SnowMoon;
			SetSnowMoon = false;
		}

		if (SetSolarEclipse)
		{
			Main.eclipse = SolarEclipse;
			SetSolarEclipse = false;
		}

		Current?.PreUpdateTime();
	}

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

	#region ---------- Network ----------
	public override bool HijackSendData(int whoAmI, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
	{
		if (Main.netMode == NetmodeID.Server && !SubworldSystem.AnyActive() && msgType == MessageID.WorldData)
		{
			List<byte> packet = [];

			packet.Add((byte)SubworldToolkitMessageType.WorldData);

			packet.Add(new BitsByte()
			{
				[0] = Main.dayTime,
				[1] = Main.fastForwardTimeToDawn,
				[2] = Main.fastForwardTimeToDusk,
				[3] = Main.pumpkinMoon,
				[4] = Main.bloodMoon,
				[5] = Main.snowMoon,
				[6] = Main.eclipse,
			});

			packet.AddRange(BitConverter.GetBytes((int)Main.time));
			packet.Add((byte)Main.moonPhase);
			packet.Add((byte)Main.sundialCooldown);
			packet.Add((byte)Main.moondialCooldown);

			for (int i = 0; i < Subworlds.Count; i++)
			{
				if (Subworlds[i] is ExtendedSubworld)
				{
					SubworldSystem.SendToSubserver(i, Mod, [.. packet]);
				}
			}
		}

		return false;
	}

	public override bool HijackGetData(ref byte messageType, ref BinaryReader reader, int playerNumber)
	{
		if (Main.netMode == NetmodeID.Server && messageType == MessageID.MiscDataSync && current?.SynchronizeTime == true)
		{
			long position = reader.BaseStream.Position;

			byte player = reader.ReadByte();
			byte subIndex = reader.ReadByte();

			switch (subIndex)
			{
				case 3:
					{
						SubworldSystem.SendToMainServer(Mod, [(byte)SubworldToolkitMessageType.Sundial]);
					}
					return true;

				case 6:
					{
						SubworldSystem.SendToMainServer(Mod, [(byte)SubworldToolkitMessageType.Moondial]);
					}
					return true;
			}

			reader.BaseStream.Position = position;
		}

		return false;
	}
	#endregion
}