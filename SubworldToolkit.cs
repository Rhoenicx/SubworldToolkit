using MonoMod.Cil;
using SubworldLibrary;
using System;
using System.Linq;
using System.Reflection;
using Terraria.GameContent.Events;
using Terraria;
using Terraria.ModLoader;
using Mono.Cecil.Cil;
using Terraria.ID;
using System.Collections.Generic;
using static Terraria.ModLoader.ModContent;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.IO;

namespace SubworldToolkit;

public class SubworldToolkit : Mod
{
	public static SubworldToolkit Instance;
	private static FieldInfo main;
	private static FieldInfo current;
	private static List<int> npcPriorityOrder;

	public static Guid MainWorldUniqueID => SubworldSystem.AnyActive() ? ((WorldFileData)main.GetValue(null)).UniqueId : Main.ActiveWorldFileData.UniqueId;

	#region ---------- Load ----------
	public override void Load()
	{
		#region ----- Instance -----
		Instance = this;
		#endregion

		#region ----- main -----
		main = typeof(SubworldSystem).GetField("main", BindingFlags.Static | BindingFlags.NonPublic);
		#endregion

		#region ----- Patch Current -----
		Type[] sublibTypes = GetInstance<SubworldLibrary.SubworldLibrary>().GetType().Assembly.GetTypes();
		Type SubworldSystemType = null;

		if (sublibTypes.Any(t => t.Name == "SubworldSystem"))
		{
			SubworldSystemType = sublibTypes.First(t => t.Name == "SubworldSystem");
		}

		if (SubworldSystemType == null)
		{
			Instance.Logger.Warn("FAILED: Type 'SubworldSystem' could not be found!");
			return;
		}

		MethodInfo beginEntering = SubworldSystemType.GetMethod("BeginEntering", BindingFlags.Static | BindingFlags.NonPublic);
		if (beginEntering != null)
		{
			MonoModHooks.Modify(beginEntering, IL_Current_Subworld);
		}
		else { Instance.Logger.Warn("FAILED: Method 'BeginEntering' could not be found!"); }


		MethodInfo loadIntoSubworld = SubworldSystemType.GetMethod("LoadIntoSubworld", BindingFlags.Static | BindingFlags.NonPublic);
		if (loadIntoSubworld != null)
		{
			MonoModHooks.Modify(loadIntoSubworld, IL_Current_Subworld);
		}
		else { Instance.Logger.Warn("FAILED: Method 'LoadIntoSubworld' could not be found!"); }


		MethodInfo onDisconnect = SubworldSystemType.GetMethod("OnDisconnect", BindingFlags.Static | BindingFlags.NonPublic);
		if (onDisconnect != null)
		{
			MonoModHooks.Modify(onDisconnect, IL_Current_Subworld);
		}
		else { Instance.Logger.Warn("FAILED: Method 'OnDisconnect' could not be found!"); }

		MethodInfo handlePacket = typeof(SubworldLibrary.SubworldLibrary).GetMethod("HandlePacket", BindingFlags.Instance | BindingFlags.Public);
		if (handlePacket != null)
		{
			MonoModHooks.Modify(handlePacket, IL_Current_Subworld);
		}
		else { Instance.Logger.Warn("FAILED: Method 'HandlePacket' could not be found!"); }
		#endregion

		#region ----- IL Patches -----
		// Get the field
		current = typeof(ExtendedSubworldSystem).GetField("current", BindingFlags.Static | BindingFlags.NonPublic);

		// IL Patches and Detours
		IL_Main.DoUpdateInWorld += (ILContext il) => 
		{
			ILCursor c = new(il);

			if (c.TryGotoNext(i => i.MatchCall(typeof(SystemLoader), "PreUpdateTime")))
			{
				ILLabel skip = il.DefineLabel();

				c.Emit(OpCodes.Ldsfld, current);
				c.Emit(OpCodes.Brfalse, skip);
				c.Emit(OpCodes.Ldsfld, current);
				c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("VerifyTime", BindingFlags.Instance | BindingFlags.Public));

				c.MarkLabel(skip);
			}
			else
			{
				Instance.Logger.Warn("FAILED: ");
			}
		};

		IL_Main.UpdateTimeRate += (ILContext il) =>
		{
			ILCursor c = new(il);

			if (c.TryGotoNext(MoveType.After, i => i.MatchCall(typeof(SystemLoader), "ModifyTimeRate")))
			{
				ILLabel skip = il.DefineLabel();

				c.Emit(OpCodes.Ldsfld, current);
				c.Emit(OpCodes.Brfalse, skip);
				c.Emit(OpCodes.Ldsfld, current);
				c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_DayNightCycleEnabled", BindingFlags.Instance | BindingFlags.Public));
				c.Emit(OpCodes.Brtrue, skip);

				c.Emit(OpCodes.Ldc_R8, 0.0);
				c.Emit(OpCodes.Stsfld, typeof(Main).GetField("dayRate", BindingFlags.Static | BindingFlags.Public));

				c.MarkLabel(skip);
			}
			else
			{
				Instance.Logger.Warn("FAILED: ");
			}
		};

		IL_Main.SkipToTime += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_SkipTimeEnabled", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_Main.Sundialing += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_SundialEnabled", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);

			if (!c.TryGotoNext(MoveType.After, i => i.MatchStsfld<Main>("sundialCooldown")))
			{
				Instance.Logger.Warn("FAILED: ");
				return;
			}

			ILLabel skip2 = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip2);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("ModifySundialing", BindingFlags.Instance | BindingFlags.Public));

			c.MarkLabel(skip2);
		};

		IL_Main.Moondialing += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_MoondialEnabled", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);

			if (!c.TryGotoNext(MoveType.After, i => i.MatchStsfld<Main>("moondialCooldown")))
			{
				Instance.Logger.Warn("FAILED: ");
				return;
			}

			ILLabel skip2 = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip2);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("ModifyMoondialing", BindingFlags.Instance | BindingFlags.Public));

			c.MarkLabel(skip2);
		};

		IL_Main.CanStartInvasion += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Call, typeof(ExtendedSubworld).GetMethod("CanStartInvasion", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ldc_I4_0);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_Main.StartInvasion += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Call, typeof(ExtendedSubworld).GetMethod("CanStartInvasion", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_Main.startPumpkinMoon += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_CanStartPumpkinMoon", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_Main.startSnowMoon += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_CanStartSnowMoon", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		On_LanternNight.LanternsCanStart += (On_LanternNight.orig_LanternsCanStart orig) =>
		{ 
			return (ExtendedSubworldSystem.current?.CanStartLanternNight) != false && orig();
		};

		IL_LanternNight.NaturalAttempt += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_CanStartLanternNight", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_LanternNight.ToggleManualLanterns += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_CanStartLanternNight", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_Main.StartRain += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_CanStartRain", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_Main.StartSlimeRain += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_CanStartSlimeRain", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_WorldGen.SpawnTravelNPC += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_CanSpawnTravellingMerchant", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_WorldGen.SpawnTownNPC += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_CanSpawnTownNPCs", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ldc_I4_0);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_Main.UpdateTime_SpawnTownNPCs += (ILContext il) =>
		{
			ILCursor c = new(il);

			int local = -1;
			int local2 = -1;

			if (c.TryGotoNext(MoveType.After,
				i => i.MatchLdsfld<WorldGen>("prioritizedTownNPCType"),
				i => i.MatchStloc(out local)))
			{
				int type = -1;

				while (c.TryGotoNext(
					i => i.MatchLdcI4(out type),
					i => i.MatchStloc(local)))
				{
					npcPriorityOrder ??= [];
					npcPriorityOrder.Add(type);
				}
			}
			else
			{
				Instance.Logger.Warn("FAILED: ");
			}

			if (c.TryGotoNext(
				i => i.MatchLdloc(out local2),
				i => i.MatchCall(typeof(NPCLoader), "CanTownNPCSpawn")))
			{
				c.Index += 2;
				c.Emit(OpCodes.Ldloc, local2);
				c.Emit(OpCodes.Call, typeof(SubworldToolkit).GetMethod("CanSpawnTownNPC", BindingFlags.Static | BindingFlags.NonPublic));
			}
			else
			{
				Instance.Logger.Warn("FAILED: ");
			}
		};

		IL_BirthdayParty.NaturalAttempt += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_CanStartBirthdayParty", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_BirthdayParty.ToggleManualParty += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_CanStartBirthdayParty", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_CultistRitual.UpdateTime += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_CanStartCultistRitual", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_CultistRitual.CheckRitual += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_CanStartCultistRitual", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ldc_I4_0);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_Sandstorm.StartSandstorm += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_CanStartSandstorm", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_DD2Event.StartInvasion += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_CanStartDD2Event", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_Main.UpdateTime_StartNight += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("StartNight", BindingFlags.Instance | BindingFlags.Public));

			c.MarkLabel(skip);

			if (c.TryGotoNext(
				i => i.MatchLdcI4(1),
				i => i.MatchStsfld<Main>("bloodMoon")))
			{
				ILLabel skip2 = il.DefineLabel();
				ILLabel exit = il.DefineLabel();

				c.Emit(OpCodes.Ldsfld, current);
				c.Emit(OpCodes.Brfalse, skip2);
				c.Emit(OpCodes.Ldsfld, current);
				c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_CanStartBloodMoon", BindingFlags.Instance | BindingFlags.Public));
				c.Emit(OpCodes.Brfalse, exit);

				c.MarkLabel(skip2);

				c.Index += 2;

				c.MarkLabel(exit);
			}
			else
			{
				Instance.Logger.Warn("FAILED: ");
			}
		};

		IL_Main.UpdateTime_StartDay += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("StartDay", BindingFlags.Instance | BindingFlags.Public));

			c.MarkLabel(skip);

			ILLabel exit = il.DefineLabel();

			if (c.TryGotoNext(
				i => i.MatchLdcI4(1),
				i => i.MatchStsfld<Main>("eclipse"))
				&& c.TryGotoPrev(
				i => i.MatchBrtrue(out exit)))
			{
				c.Index++;

				ILLabel skip2 = il.DefineLabel();

				c.Emit(OpCodes.Ldsfld, current);
				c.Emit(OpCodes.Brfalse, skip2);
				c.Emit(OpCodes.Ldsfld, current);
				c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_CanStartSolarEclipse", BindingFlags.Instance | BindingFlags.Public));
				c.Emit(OpCodes.Brfalse, exit);

				c.MarkLabel(skip2);
			}
			else
			{
				Instance.Logger.Warn("FAILED: ");
			}
		};

		IL_Main.ShouldNormalEventsBeAbleToStart += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_CanNaturalSpawnBosses", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ldc_I4_1);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_WorldGen.TriggerLunarApocalypse += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_CanStartLunarEvents", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_Main.HandleMeteorFall += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_CanDropMeteors", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_WorldGen.dropMeteor += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_CanDropMeteors", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_WorldGen.meteor += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_CanDropMeteors", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ldc_I4_0);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_Main.UpdateWindyDayState += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_WindEnabled", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);

			c.Emit(OpCodes.Ldc_R4, 0f);
			c.Emit(OpCodes.Stsfld, typeof(Main).GetField("windSpeedTarget", BindingFlags.Static | BindingFlags.Public));
			c.Emit(OpCodes.Ldc_R4, 0f);
			c.Emit(OpCodes.Stsfld, typeof(Main).GetField("windSpeedCurrent", BindingFlags.Static | BindingFlags.Public));
			c.Emit(OpCodes.Ldc_I4_0);
			c.Emit(OpCodes.Stsfld, typeof(Main).GetField("_shouldUseWindyDayMusic", BindingFlags.Static | BindingFlags.Public));
			c.Emit(OpCodes.Ldc_I4_0);
			c.Emit(OpCodes.Stsfld, typeof(Main).GetField("_shouldUseStormMusic", BindingFlags.Static | BindingFlags.Public));

			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);

			// ---------------------------------
			ILLabel skip2 = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip2);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_WindyDayEnabled", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip2);

			c.Emit(OpCodes.Ldc_I4_0);
			c.Emit(OpCodes.Stsfld, typeof(Main).GetField("_shouldUseWindyDayMusic", BindingFlags.Static | BindingFlags.Public));

			c.MarkLabel(skip2);

			// ---------------------------------
			ILLabel skip3 = il.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip3);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_StormEnabled", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip3);

			c.Emit(OpCodes.Ldc_I4_0);
			c.Emit(OpCodes.Stsfld, typeof(Main).GetField("_shouldUseStormMusic", BindingFlags.Static | BindingFlags.Public));

			c.MarkLabel(skip3);

			// ---------------------------------
			while (c.Next != null)
			{
				if (c.Next.MatchLdcI4(1))
				{
					if (c.Next.Next.MatchStsfld<Main>("_shouldUseWindyDayMusic"))
					{
						ILLabel skip4 = il.DefineLabel();
						ILLabel exit = il.DefineLabel();

						c.Emit(OpCodes.Ldsfld, current);
						c.Emit(OpCodes.Brfalse, skip4);
						c.Emit(OpCodes.Ldsfld, current);
						c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_WindyDayEnabled", BindingFlags.Instance | BindingFlags.Public));
						c.Emit(OpCodes.Brfalse, exit);

						c.MarkLabel(skip4);

						c.Index += 2;

						c.MarkLabel(exit);

					}
					else if (c.Next.Next.MatchStsfld<Main>("_shouldUseStormMusic"))
					{
						ILLabel skip4 = il.DefineLabel();
						ILLabel exit = il.DefineLabel();

						c.Emit(OpCodes.Ldsfld, current);
						c.Emit(OpCodes.Brfalse, skip4);
						c.Emit(OpCodes.Ldsfld, current);
						c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_StormEnabled", BindingFlags.Instance | BindingFlags.Public));
						c.Emit(OpCodes.Brfalse, exit);

						c.MarkLabel(skip4);

						c.Index += 2;

						c.MarkLabel(exit);
					}
					else
					{
						c.Index++;
					}
				}
				else
				{
					c.Index++;
				}
			}
		};

		IL_WorldGen.UpdateWorld_Inner += (ILContext il) =>
		{
			// ---------- World Updates ----------

			ILCursor c = new(il);

			ILLabel skip = c.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_WorldUpdatesEnabled", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);

			// ---------- Infection spread ----------

			if (c.TryGotoNext(MoveType.After, i => i.MatchStsfld<WorldGen>("AllowedToSpreadInfections")))
			{
				ILLabel skip2 = c.DefineLabel();

				c.Emit(OpCodes.Ldsfld, current);
				c.Emit(OpCodes.Brfalse, skip2);
				c.Emit(OpCodes.Ldsfld, current);
				c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_InfectionSpreadEnabled", BindingFlags.Instance | BindingFlags.Public));
				c.Emit(OpCodes.Brtrue, skip2);
				c.Emit(OpCodes.Ldc_I4_0);
				c.Emit(OpCodes.Stsfld, typeof(WorldGen).GetField("AllowedToSpreadInfections", BindingFlags.Static | BindingFlags.Public));

				c.MarkLabel(skip2);
			}
			else
			{
				Instance.Logger.Debug("FAILED: ");
			}

			// ---------- Wiring ----------

			if (c.TryGotoNext(i => i.MatchCall(typeof(Wiring), "UpdateMech")))
			{
				ILLabel skip2 = c.DefineLabel();
				ILLabel exit = c.DefineLabel();

				c.Emit(OpCodes.Ldsfld, current);
				c.Emit(OpCodes.Brfalse, skip2);
				c.Emit(OpCodes.Ldsfld, current);
				c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_WiringEnabled", BindingFlags.Instance | BindingFlags.Public));
				c.Emit(OpCodes.Brtrue, skip2);
				c.Emit(OpCodes.Br, exit);

				c.MarkLabel(skip2);

				c.Index++;

				c.MarkLabel(exit);
			}
			else
			{
				Instance.Logger.Debug("FAILED: ");
			}

			// ---------- Tile Entities ----------

			ILCursor c2;
			if (c.TryGotoNext(MoveType.After, i => i.MatchCall<TileEntity>("UpdateEnd"))
				&& (c2 = c.Clone()).TryGotoPrev(i => i.MatchCall<TileEntity>("UpdateStart")))
			{
				ILLabel skip2 = c2.DefineLabel();
				ILLabel exit = c.DefineLabel();

				c2.Emit(OpCodes.Ldsfld, current);
				c2.Emit(OpCodes.Brfalse, skip2);
				c2.Emit(OpCodes.Ldsfld, current);
				c2.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_TileEntitiesEnabled", BindingFlags.Instance | BindingFlags.Public));
				c2.Emit(OpCodes.Brtrue, skip2);
				c2.Emit(OpCodes.Br, exit);

				c2.MarkLabel(skip2);

				c.MarkLabel(exit);
			}
			else
			{
				Instance.Logger.Debug("FAILED: ");
			}

			// ---------- Falling Stars ----------

			if (c.TryGotoNext(i => i.MatchLdsfld<Main>("dayTime")))
			{
				ILLabel skip2 = c.DefineLabel();

				c.Emit(OpCodes.Ldsfld, current);
				c.Emit(OpCodes.Brfalse, skip2);
				c.Emit(OpCodes.Ldsfld, current);
				c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_FallingStarsEnabled", BindingFlags.Instance | BindingFlags.Public));
				c.Emit(OpCodes.Brtrue, skip2);
				c.Emit(OpCodes.Ret);

				c.MarkLabel(skip2);
			}
			else
			{
				Instance.Logger.Debug("FAILED: ");
			}
		};

		IL_WorldGen.PlantAlch += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = c.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_SpawnAlchemyPlantsEnabled", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_WorldGen.UpdateWorld_OvergroundTile += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = c.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_OvergroundTileUpdateEnabled", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_WorldGen.UpdateWorld_UndergroundTile += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = c.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_UndergroundTileUpdateEnabled", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_Liquid.UpdateLiquid += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = c.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_LiquidUpdateEnabled", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_Mount.SetMount += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = c.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Ldarg_2);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("CanUseMounts", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		On_Mount.CanMount += (On_Mount.orig_CanMount orig, Mount self, int m, Player mountingPlayer) =>
		{
			if (ExtendedSubworldSystem.Current?.CanUseMounts(mountingPlayer) == false)
			{
				return false;
			}

			return orig(self, m, mountingPlayer);
		};

		IL_Player.Teleport += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = c.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Ldarga, 1);
			c.Emit(OpCodes.Ldflda, typeof(Vector2).GetField("X"));
			c.Emit(OpCodes.Ldarga, 1);
			c.Emit(OpCodes.Ldflda, typeof(Vector2).GetField("Y"));
			c.Emit(OpCodes.Ldarg_2);
			c.Emit(OpCodes.Ldarg_3);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("Teleport", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);
		};

		IL_Player.Spawn += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel skip = c.DefineLabel();
			ILLabel skip2 = c.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip2);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Ldarg_1);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("Spawn", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip);

			c.Emit(OpCodes.Ldarg_1);
			c.Emit(OpCodes.Ldc_I4, (int)PlayerSpawnContext.RecallFromItem);
			c.Emit(OpCodes.Bne_Un, skip2);

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("CanUseRecall", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, skip2);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(skip2);
			
			// ---------- ModifySpawn -----------
			
			ILLabel exit = c.DefineLabel();
			
			if (!c.TryGotoNext(
				i => i.MatchCall(typeof(Player), "CheckSpawn"),
				i => i.MatchBrtrue(out exit))
				|| !c.TryGotoPrev(
				i => i.MatchLdarg(0),
				i => i.MatchCall(typeof(Player), "FindSpawn")))
			{
				Instance.Logger.Debug("FAILED: ");
				return;
			}

			ILLabel skip3 = c.DefineLabel();

			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Brfalse, skip3);
			c.Emit(OpCodes.Ldsfld, current);
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Ldarg_1);
			c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("ModifySpawn", BindingFlags.Instance | BindingFlags.Public));
			c.Emit(OpCodes.Brtrue, exit);

			c.MarkLabel(skip3);
		};

		IL_Liquid.Update += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel exit = c.DefineLabel();
			ILLabel skip = c.DefineLabel();

			if (c.TryGotoNext(
				i => i.MatchLdarg(0),
				i => i.MatchLdfld(typeof(Liquid), "y"),
				i => i.MatchCall(typeof(Main), "get_UnderworldLayer"),
				i => i.MatchBle(out exit)))
			{
				c.Emit(OpCodes.Ldsfld, current);
				c.Emit(OpCodes.Brfalse, skip);
				c.Emit(OpCodes.Ldsfld, current);
				c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_WaterVaporizeEnabled", BindingFlags.Instance | BindingFlags.Public));
				c.Emit(OpCodes.Brfalse, exit);

				c.MarkLabel(skip);
			}
			else
			{
				Instance.Logger.Debug("FAILED: ");
			}
		};

		IL_Player.Update += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel bypass = c.DefineLabel();

			if (c.TryGotoNext(MoveType.After,
				i => i.MatchLdsfld<SubworldSystem>("current"),
				i => i.MatchCallvirt<Subworld>("get_NormalUpdates"),
				i => i.MatchBrtrue(out _)))
			{
				c.MarkLabel(bypass);

				c.Index -= 3;

				ILLabel skip = c.DefineLabel();

				c.Emit(OpCodes.Ldsfld, current);
				c.Emit(OpCodes.Brfalse, skip);
				c.Emit(OpCodes.Ldsfld, current);
				c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_GetGravityEnabled", BindingFlags.Instance | BindingFlags.Public));
				c.Emit(OpCodes.Brtrue, bypass);

				c.MarkLabel(skip);
			}
			else
			{
				Instance.Logger.Debug("FAILED: ");
			}
		};

		IL_NPC.UpdateNPC_UpdateGravity += (ILContext il) =>
		{
			ILCursor c = new(il);

			ILLabel bypass = c.DefineLabel();

			if (c.TryGotoNext(MoveType.After,
				i => i.MatchLdsfld<SubworldSystem>("current"),
				i => i.MatchCallvirt<Subworld>("get_NormalUpdates"),
				i => i.MatchBrtrue(out _)))
			{
				c.MarkLabel(bypass);

				c.Index -= 3;

				ILLabel skip = c.DefineLabel();

				c.Emit(OpCodes.Ldsfld, current);
				c.Emit(OpCodes.Brfalse, skip);
				c.Emit(OpCodes.Ldsfld, current);
				c.Emit(OpCodes.Callvirt, typeof(ExtendedSubworld).GetMethod("get_GetGravityEnabled", BindingFlags.Instance | BindingFlags.Public));
				c.Emit(OpCodes.Brtrue, bypass);

				c.MarkLabel(skip);
			}
			else
			{
				Instance.Logger.Debug("FAILED: ");
			}
		};
		#endregion
	}
	#endregion

	#region ---------- IL Helpers ----------
	private static void IL_Current_Subworld(ILContext il)
	{
		ILCursor c = new(il);

		while (c.TryGotoNext(moveType: MoveType.After, i => i.MatchStsfld<SubworldSystem>("current")))
		{
			c.EmitDelegate(IsSubworldEx);
		}
	}

	private static void IsSubworldEx()
	{
		ExtendedSubworldSystem.current = SubworldSystem.Current is ExtendedSubworld extendedSubworld ? extendedSubworld : null;
	}

	private static void CanSpawnTownNPC(int numTownNPCs)
	{
		// Do not run on non-subworlds
		if (ExtendedSubworldSystem.current == null)
		{
			return;
		}

		// Keep track of a new prioritized NPC
		int priority = NPCID.None;
		int lowest = int.MaxValue;

		// Loop over all the NPCs, this array contains all
		// the NPCs that can spawn.
		for (int i = 0; i < Main.townNPCCanSpawn.Length; i++)
		{
			// This NPC has not been enabled for spawning
			if (!Main.townNPCCanSpawn[i])
			{
				continue;
			}

			// Check if this NPC is allowed to spawn in the subworld
			if (ExtendedSubworldSystem.current.CanSpawnTownNPC(i, numTownNPCs))
			{
				// For vanilla NPCs determine the priority based on 
				// the saved List during the IL-Patching.
				if (i < NPCID.Count)
				{
					// Create variable to hold the current index
					int index = -1;

					// Get the index of the current npc inside the list
					if (npcPriorityOrder != null && npcPriorityOrder.Count > 0)
					{
						index = npcPriorityOrder.IndexOf(i);
					}

					// Check if the index is valid and lower than
					// the index we already have.
					if (index > -1 && index < lowest)
					{
						lowest = index;
						priority = i;
					}
				}

				// The NPC may spawn, save its type if needed
				else if (i >= NPCID.Count && priority == NPCID.None)
				{
					priority = i;
				}

				continue;
			}

			// The NPC is not allowed to spawn, remove it
			// from the spawn pool.
			Main.townNPCCanSpawn[i] = false;

			// If this NPC had priority, clear it as well.
			if (WorldGen.prioritizedTownNPCType == i)
			{
				WorldGen.prioritizedTownNPCType = 0;
			}
		}

		// Set the new prioritized npc
		if (WorldGen.prioritizedTownNPCType == 0 && priority != NPCID.None)
		{
			WorldGen.prioritizedTownNPCType = priority;
		}
	}
	#endregion
}