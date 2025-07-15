using Terraria;
using Terraria.ID;
using SubworldLibrary;
using Terraria.GameContent.Events;
using System.Collections.Generic;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace SubworldToolkit;

public abstract class ExtendedSubworld : Subworld
{
	#region ---------- Overrides ----------
	/// <summary>
	/// NOTE: <see cref="NormalUpdates"/> is enabled specifically
	/// for the other settings to function. Disable at your
	/// own risk!
	/// <br/> Returns true by default.
	/// </summary>
	public override bool NormalUpdates => true;

	public override void CopyMainWorldData()
	{
		if (SynchronizeTime)
		{
			SubworldSystem.CopyWorldData(nameof(Main.time), (int)Main.time);
			SubworldSystem.CopyWorldData(nameof(Main.dayTime), Main.dayTime);
			SubworldSystem.CopyWorldData(nameof(Main.fastForwardTimeToDawn), Main.fastForwardTimeToDawn);
			SubworldSystem.CopyWorldData(nameof(Main.sundialCooldown), Main.sundialCooldown);
			SubworldSystem.CopyWorldData(nameof(Main.fastForwardTimeToDusk), Main.fastForwardTimeToDusk);
			SubworldSystem.CopyWorldData(nameof(Main.moondialCooldown), Main.moondialCooldown);
		}

		if (SynchronizeMoonPhase)
		{
			SubworldSystem.CopyWorldData(nameof(Main.moonPhase), Main.moonPhase);
		}

		if (SynchronizePumpkinMoon)
		{
			SubworldSystem.CopyWorldData(nameof(Main.pumpkinMoon), Main.pumpkinMoon);
		}

		if (SynchronizeBloodMoon)
		{
			SubworldSystem.CopyWorldData(nameof(Main.bloodMoon), Main.bloodMoon);
		}

		if (SynchronizeSnowMoon)
		{
			SubworldSystem.CopyWorldData(nameof(Main.snowMoon), Main.snowMoon);
		}

		if (SynchronizeSolarEclipse)
		{
			SubworldSystem.CopyWorldData(nameof(Main.eclipse), Main.eclipse);
		}
	}

	public override void ReadCopiedMainWorldData()
	{
		if (SynchronizeTime)
		{
			ExtendedSubworldSystem.SetTime = true;
			ExtendedSubworldSystem.Time = SubworldSystem.ReadCopiedWorldData<int>(nameof(Main.time));
			ExtendedSubworldSystem.DayTime = SubworldSystem.ReadCopiedWorldData<bool>(nameof(Main.dayTime));
			ExtendedSubworldSystem.FastForwardTimeToDawn = SubworldSystem.ReadCopiedWorldData<bool>(nameof(Main.fastForwardTimeToDawn));
			ExtendedSubworldSystem.SundialCooldown = SubworldSystem.ReadCopiedWorldData<int>(nameof(Main.sundialCooldown));
			ExtendedSubworldSystem.FastForwardTimeToDusk = SubworldSystem.ReadCopiedWorldData<bool>(nameof(Main.fastForwardTimeToDusk));
			ExtendedSubworldSystem.MoondialCooldown = SubworldSystem.ReadCopiedWorldData<int>(nameof(Main.moondialCooldown));
		}

		if (SynchronizeMoonPhase)
		{ 
			ExtendedSubworldSystem.SetMoonPhase = true;
			ExtendedSubworldSystem.MoonPhase = SubworldSystem.ReadCopiedWorldData<int>(nameof(Main.moonPhase));
		}

		if (SynchronizePumpkinMoon)
		{
			ExtendedSubworldSystem.SetPumpkinMoon = true;
			ExtendedSubworldSystem.PumpkinMoon = SubworldSystem.ReadCopiedWorldData<bool>(nameof(Main.pumpkinMoon));
		}

		if (SynchronizeBloodMoon)
		{
			ExtendedSubworldSystem.SetBloodMoon = true;
			ExtendedSubworldSystem.BloodMoon = SubworldSystem.ReadCopiedWorldData<bool>(nameof(Main.bloodMoon));
		}

		if (SynchronizeSnowMoon)
		{
			ExtendedSubworldSystem.SetSnowMoon = true;
			ExtendedSubworldSystem.SnowMoon = SubworldSystem.ReadCopiedWorldData<bool>(nameof(Main.snowMoon));
		}

		if (SynchronizeSolarEclipse)
		{
			ExtendedSubworldSystem.SetSolarEclipse = true;
			ExtendedSubworldSystem.SolarEclipse = SubworldSystem.ReadCopiedWorldData<bool>(nameof(Main.eclipse));
		}
	}

	#endregion

	#region ---------- Updates ----------
	/// <summary>
	/// Globally enable or disable all the settings.
	/// Use this as a base settings for your subworld.
	/// <br/> Returns true by default.
	/// </summary>
	public virtual bool VanillaUpdates => true;
	#endregion

	#region ---------- Events ----------
	/// <summary>
	/// Determines if the vanilla goblin army can be
	/// used in the subworld. Prevents the start of the 
	/// invasion during <see cref="Main.StartInvasion(int)"/>
	/// and <see cref="Main.CanStartInvasion(int, bool)"/>,
	/// effectively stopping the natural spawn and 
	/// <see cref="ItemID.GoblinBattleStandard"/>.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool CanStartGoblinArmy => VanillaUpdates;
	public virtual bool AutoStopGoblinArmy => !VanillaUpdates;

	/// <summary>
	/// Determines if the vanilla frost legion can be
	/// used in the subworld. Prevents the start of the 
	/// invasion during <see cref="Main.StartInvasion(int)"/>
	/// and <see cref="Main.CanStartInvasion(int, bool)"/>,
	/// effectively stopping the natural spawn and 
	/// <see cref="ItemID.SnowGlobe"/>.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool CanStartFrostLegion => VanillaUpdates;
	public virtual bool AutoStopFrostLegion => !VanillaUpdates;

	/// <summary>
	/// Determines if the vanilla pirate invasion can be
	/// used in the subworld. Prevents the start of the 
	/// invasion during <see cref="Main.StartInvasion(int)"/>
	/// and <see cref="Main.CanStartInvasion(int, bool)"/>,
	/// effectively stopping the natural spawn and 
	/// <see cref="ItemID.PirateMap"/>.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool CanStartPirateInvasion => VanillaUpdates;
	public virtual bool AutoStopPirateInvasion => !VanillaUpdates;

	/// <summary>
	/// Determines if the martian madness invasion can be
	/// used in the subworld. Prevents the start of the 
	/// invasion during <see cref="Main.StartInvasion(int)"/>
	/// and <see cref="Main.CanStartInvasion(int, bool)"/>,
	/// effectively stopping the natural spawn when the drone
	/// escapes.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool CanStartMartianMadness => VanillaUpdates;
	public virtual bool AutoStopMartianMadness => !VanillaUpdates;

	/// <summary>
	/// Determines if the vanilla pumpkin moon can be
	/// used in the subworld. Prevents the start of the 
	/// event during <see cref="Main.startPumpkinMoon"/>,
	/// effectively stopping the spawn by 
	/// <see cref="ItemID.PumpkinMoonMedallion"/>.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool CanStartPumpkinMoon => VanillaUpdates;
	public virtual bool AutoStopPumpkinMoon => !VanillaUpdates;
	public virtual bool SynchronizePumpkinMoon => false;

	/// <summary>
	/// Determines if the vanilla pumpkin moon can be
	/// used in the subworld. Prevents the start of the 
	/// event during <see cref="Main.startPumpkinMoon"/>,
	/// effectively stopping the spawn by 
	/// <see cref="ItemID.NaughtyPresent"/>.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool CanStartSnowMoon => VanillaUpdates;
	public virtual bool AutoStopSnowMoon => !VanillaUpdates;
	public virtual bool SynchronizeSnowMoon => false;

	/// <summary>
	/// Determines if the latern night can be started
	/// in the subworld. Prevents the automatic start
	/// of the latern night.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool CanStartLanternNight => VanillaUpdates;
	public virtual bool AutoStopLanternNight => !VanillaUpdates;

	/// <summary>
	/// Determines if the rain event can be used in the
	/// subworld. Prevents the start of the event during
	/// <see cref="Main.StartRain"/>, effectively stopping
	/// the natural spawn of the rain.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool CanStartRain => VanillaUpdates;
	public virtual bool AutoStopRain => !VanillaUpdates;

	/// <summary>
	/// Determines if the slime rain event can be used in
	/// the subworld. Prevents the start of the event during
	/// <see cref="Main.StartSlimeRain(bool)"/>, effectively
	/// stopping the natural spawn of the slime rain event.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool CanStartSlimeRain => VanillaUpdates;
	public virtual bool AutoStopSlimeRain => !VanillaUpdates;

	/// <summary>
	/// Determines if a natural party can occur in the subworld.
	/// Checked during BirthdayParty.NaturalAttempt. Parties
	/// can still be started manually using the tile or code.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool CanStartBirthdayParty => VanillaUpdates;
	public virtual bool AutoStopBirthdayParty => !VanillaUpdates;

	/// <summary>
	/// Determines if the Cultist Ritual spawn attempt runs
	/// on the subworld. Prevents the natural spawn during
	/// <see cref="CultistRitual.UpdateTime"/> and the 
	/// ritual check CultistRitual.CheckRitual.
	/// </summary>
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	public virtual bool CanStartCultistRitual => VanillaUpdates;

	/// <summary>
	/// Determines if a sandstorm can occur in the subworld.
	/// This prevents the sandstorm event from starting when
	/// <see cref="Sandstorm.StartSandstorm"/>
	/// is called.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool CanStartSandstorm => VanillaUpdates;
	public virtual bool AutoStopSandstorm => !VanillaUpdates;

	/// <summary>
	/// Determines if the old one's army invasion can start
	/// in the subworld. This prevents the event from starting
	/// when <see cref="DD2Event.StartInvasion(int)"/>
	/// is called.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool CanStartDD2Event => VanillaUpdates;
	public virtual bool AutoStopDD2Event => !VanillaUpdates;

	/// <summary>
	/// Determines if the blood moon event can occur in the 
	/// subworld. This prevents the natural spawn and the
	/// summon item <see cref="ItemID.BloodMoonStarter"/>.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool CanStartBloodMoon => VanillaUpdates;
	public virtual bool AutoStopBloodMoon => !VanillaUpdates;
	public virtual bool SynchronizeBloodMoon => false;

	/// <summary>
	/// Determines if vanilla bosses should naturally spawn in
	/// the subworld. Examples: Eye of Cthulhu, Mech bosses.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool CanNaturalSpawnBosses => VanillaUpdates;

	/// <summary>
	/// Determines if the solar eclipse can happen in the subworld.
	/// This applies to the natural spawn and the spawn item 
	/// <see cref="ItemID.SolarTablet"/>.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool CanStartSolarEclipse => VanillaUpdates;
	public virtual bool AutoStopSolarEclipse => !VanillaUpdates;
	public virtual bool SynchronizeSolarEclipse => false;

	/// <summary>
	/// Determines if the Lunar Events are enabled in the subworld.
	/// This prevents the natural spawn of the pillars after the
	/// Lunatic Cultist is defeated. Prevents the code 
	/// <see cref="WorldGen.TriggerLunarApocalypse"/> from executing.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool CanStartLunarEvents => VanillaUpdates;
	public virtual bool AutoStopLunarEvents => !VanillaUpdates;

	/// <summary>
	/// Determines if meteors can fall in this subworld.
	/// This prevents the meteor fall check during <see cref="Main.HandleMeteorFall"/>
	/// and <see cref="WorldGen.dropMeteor"/>
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool CanDropMeteors => VanillaUpdates;

	/// <summary>
	/// Determines if wind mechanics are enabled in the subworld.
	/// Sets all wind related variabels to zero.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool WindEnabled => VanillaUpdates;

	/// <summary>
	/// Determines if the Windy Day state can be active in the subworld.
	/// <br/>Returns <see cref="WindEnabled"/> by default.
	/// </summary>
	public virtual bool WindyDayEnabled => WindEnabled;

	/// <summary>
	/// Determines if the Windy Day state can be active in the subworld.
	/// <br/>Returns <see cref="WindyDayEnabled"/> by default.
	/// </summary>
	public virtual bool StormEnabled => WindyDayEnabled;
	#endregion

	#region ---------- World ----------
	/// <summary>
	/// Determines if any world updates are enabled on the subworld.
	/// Be aware that this disabled A LOT of vanilla logic:
	/// Wiring, Liquid, Tile Entities, Tile Ticks, Falling Stars and more...
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool WorldUpdatesEnabled => VanillaUpdates;

	/// <summary>
	/// Determines if evil biomes can spread in the subworld.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool InfectionSpreadEnabled => VanillaUpdates;

	/// <summary>
	/// Determines if wiring should be updated in the subworld.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool WiringEnabled => VanillaUpdates;

	/// <summary>
	/// Determines if tile entities should be updated in the subworld.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool TileEntitiesEnabled => VanillaUpdates;

	/// <summary>
	/// Determines if liquids should be updated in the subworld.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool LiquidUpdateEnabled => VanillaUpdates;

	/// <summary>
	/// Determines if new alchemy plants should be spawned in the
	/// subworld.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool SpawnAlchemyPlantsEnabled => VanillaUpdates;

	/// <summary>
	/// Determines if falling stars should be spawned at night when 
	/// inside the subworld. This only works when it is night.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool FallingStarsEnabled => VanillaUpdates;

	/// <summary>
	/// Determines if the random updates for all overground tiles
	/// is enabled. This takes care of spreading grass, biomes, growing
	/// plants, growing veins and more.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool OvergroundTileUpdateEnabled => VanillaUpdates;

	/// <summary>
	/// Determines if the random updates for all overground tiles
	/// is enabled. This takes care of spreading grass, biomes, growing
	/// plants, growing veins and more.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool UndergroundTileUpdateEnabled => VanillaUpdates;

	/// <summary>
	/// Determines if water should vaporize when reaching its Y-position
	/// is greater than <see cref="Main.UnderworldLayer"/>. Return false
	/// to disable water vaporize.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool WaterVaporizeEnabled => VanillaUpdates;

	/// <summary>
	/// When enabled <see cref="Subworld.GetGravity(Player)"/> is
	/// called for the subworld even when <see cref="Subworld.normalUpdates"/>
	/// is enabled (which is the default behavior of <see cref="ExtendedSubworld"/>).
	/// Returns NOT <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool GetGravityEnabled => !VanillaUpdates;
	#endregion

	#region ---------- Time ----------
	/// <summary>
	/// Determines if the day and night cycle is enabled.
	/// This setting is applied as condition to update
	/// <see cref="Main.dayRate"/>, if this is disabled
	/// time will not pass. Time can still be changed 
	/// through other means.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool DayNightCycleEnabled => VanillaUpdates;

	/// <summary>
	/// Determines whether the <see cref="Main.SkipToTime(int, bool)"/>
	/// can be used to skip time in the subworld. This vanilla
	/// method also takes care of handling starting night/day.
	/// <br/>Returns <see cref="DayNightCycleEnabled"/> by default.
	/// </summary>
	public virtual bool SkipTimeEnabled => DayNightCycleEnabled;

	public virtual bool SynchronizeTime => false;

	public virtual bool SynchronizeMoonPhase => false;

	/// <summary>
	/// Determines if players (and modders) can use 
	/// <see cref="Main.Sundialing"/> to start
	/// skipping time to dawn. Only works when
	/// <see cref="DayAndNightCycle"/> is enabled.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool SundialEnabled => VanillaUpdates;

	/// <summary>
	/// Determines if players (and modders) can use
	/// <see cref="Main.Moondialing"/> to start
	/// skipping time to dusk. Only works when
	/// <see cref="DayAndNightCycle"/> is enabled.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool MoondialEnabled => VanillaUpdates;

	/// <summary>
	/// Special hook of SubworldToolkit, this hook will take
	/// care of resetting all the events that are not 
	/// enabled. This method is called on the very start of
	/// Main.DoUpdateInWorld.
	/// </summary>
	public virtual void VerifyTime()
	{
		// Force disable lantern night
		if (!CanStartLanternNight && AutoStopLanternNight)
		{
			LanternNight.GenuineLanterns = false;
			LanternNight.ManualLanterns = false;
			LanternNight.NextNightIsLanternNight = false;
			LanternNight.LanternNightsOnCooldown = 0;
		}

		// Force disable all invasions
		if ((!CanStartGoblinArmy && Main.invasionType == InvasionID.GoblinArmy && AutoStopGoblinArmy)
			|| (!CanStartFrostLegion && Main.invasionType == InvasionID.SnowLegion && AutoStopFrostLegion)
			|| (!CanStartPirateInvasion && Main.invasionType == InvasionID.PirateInvasion && AutoStopPirateInvasion)
			|| (!CanStartMartianMadness && Main.invasionType == InvasionID.MartianMadness && AutoStopMartianMadness))
		{
			Main.invasionDelay = 0;
			Main.invasionType = 0;
			Main.invasionSize = 0;
			Main.invasionWarn = 0;
			Main.invasionX = 0.0;
			Main.invasionSizeStart = 0;
			Main.invasionProgress = 0;
			Main.invasionProgressWave = 0;
		}

		// Force disable pumpkin moon
		if (!CanStartPumpkinMoon && AutoStopPumpkinMoon)
		{
			Main.pumpkinMoon = false;
		}

		// Force disable snow moon
		if (!CanStartSnowMoon && AutoStopSnowMoon)
		{
			Main.snowMoon = false;
		}

		// Force disable bloodmoon
		if (!CanStartBloodMoon && AutoStopBloodMoon)
		{
			Main.bloodMoon = false;
		}

		// Force disable eclipse
		if (!CanStartSolarEclipse && AutoStopSolarEclipse)
		{
			Main.eclipse = false;
		}

		// Force disable rain
		if (!CanStartRain && AutoStopRain)
		{
			Main.StopRain();
		}

		// Force disable slime rain
		if (!CanStartSlimeRain && AutoStopSlimeRain)
		{
			Main.StopSlimeRain(false);
		}

		// Force disable sandstorm
		if (!CanStartSandstorm && AutoStopSandstorm)
		{
			Sandstorm.Happening = false;
			Sandstorm.TimeLeft = 0.0;
		}

		// Force disable birtday party
		if (!CanStartBirthdayParty && AutoStopBirthdayParty && BirthdayParty.PartyIsUp)
		{
			BirthdayParty.ManualParty = false;
			BirthdayParty.GenuineParty = false;
			BirthdayParty.PartyDaysOnCooldown = 0;
			BirthdayParty.CelebratingNPCs.Clear();
		}

		// Force disable DD2 Event
		if (!CanStartDD2Event && AutoStopDD2Event && DD2Event.Ongoing)
		{
			DD2Event.Ongoing = false;
			NPC.totalInvasionPoints = 0.0f;
			NPC.waveKills = 0.0f;
			NPC.waveNumber = 0;
			DD2Event.WipeEntities();
		}

		// Force unspawn the travelling merchant
		if (!CanSpawnTravellingMerchant && AutoDespawnTravellingMerchant && NPC.travelNPC)
		{
			WorldGen.UnspawnTravelNPC();
		}

		// Force stop the lunar events
		if (!CanStartLunarEvents && AutoStopLunarEvents && NPC.LunarApocalypseIsUp)
		{ 
			NPC.LunarApocalypseIsUp = false;
			NPC.TowerActiveStardust = false;
			NPC.TowerActiveNebula = false;
			NPC.TowerActiveVortex = false;
			NPC.TowerActiveSolar = false;
		}

		// Force disable meteor fall
		if (!CanDropMeteors)
		{
			WorldGen.spawnMeteor = false;
		}

		// Force disable wind
		if (!WindEnabled)
		{ 
			Main.windSpeedCurrent = 0.0f;
			Main.windSpeedTarget = 0.0f;
			Main._shouldUseWindyDayMusic = false;
			Main._shouldUseStormMusic = false;
		}

		// Force disable windy day
		if (!WindyDayEnabled)
		{
			Main._shouldUseWindyDayMusic = false;
		}

		// Force disable storm
		if (!StormEnabled)
		{
			Main._shouldUseStormMusic = false;
		}
	}

	/// <summary>
	/// Use to make things happen when the daytime starts. Called
	/// during <see cref="Main.UpdateTime_StartDay(ref bool)"/>
	/// and <see cref="Main.SkipToTime(int, bool)"/>.
	/// This method will only be able to get called when 
	/// <see cref="DayNightCycleEnabled"/> is enabled and/or
	/// <see cref="SkipTimeEnabled"/> is enabled.
	/// </summary>
	public virtual void StartDay(ref bool stopEvents) { }

	/// <summary>
	/// Use to make things happen when the daytime starts. Called
	/// during <see cref="Main.UpdateTime_StartNight(ref bool)"/>
	/// and <see cref="Main.SkipToTime(int, bool)"/>.
	/// This method will only be able to get called when 
	/// <see cref="DayNightCycleEnabled"/> is enabled and/or
	/// <see cref="SkipTimeEnabled"/> is enabled.
	/// </summary>
	public virtual void StartNight(ref bool stopEvents) { }

	/// <summary>
	/// Use this to make things happen when the sundial
	/// is used. Called just before the world data
	/// is synchronized. Use this to set a custom cooldown.
	/// <see cref="SundialEnabled"/> needs to be enabled.
	/// NOTE: the cooldown is converted to a byte on the network!
	/// </summary>
	public virtual void ModifySundialing() { }

	/// <summary>
	/// Use this to make things happen when the moondial
	/// is used. Called just before the world data
	/// is synchronized. Use this to set a custom cooldown.
	/// <see cref="MoondialEnabled"/> needs to be enabled.
	/// NOTE: the cooldown is converted to a byte on the network!
	/// </summary>
	public virtual void ModifyMoondialing() { }
	#endregion

	#region ---------- Invasions ----------
	/// <summary>
	/// Determines if the requested invasion type can be started
	/// in this subworld. See <see cref="InvasionID"/> for the 
	/// IDs of the invasions. By default this method evaluates
	/// the chosen settings of the subworld. In most cases you 
	/// don't need to use this.
	/// </summary>
	public virtual bool CanStartInvasion(int type)
	{
		return type switch
		{
			InvasionID.GoblinArmy => CanStartGoblinArmy,
			InvasionID.SnowLegion => CanStartFrostLegion,
			InvasionID.PirateInvasion => CanStartPirateInvasion,
			InvasionID.MartianMadness => CanStartMartianMadness,
			_ => true
		};
	}
	#endregion

	#region ---------- NPCs ----------
	/// <summary>
	/// Determines if the vanilla travelling merchant should
	/// spawn in the subworld. Prevents the spawn of the
	/// NPC during <see cref="WorldGen.SpawnTravelNPC"/>.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool CanSpawnTravellingMerchant => VanillaUpdates;
	public virtual bool AutoDespawnTravellingMerchant => !VanillaUpdates;

	/// <summary>
	/// Determines if town NPCs can automatically spawn in 
	/// this subworld. This will block any town NPC spawn
	/// during <see cref="WorldGen.SpawnTownNPC(int, int)"/>.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool CanSpawnTownNPCs => VanillaUpdates;

	/// <summary>
	/// Determines whether the NPC of the given type may
	/// spawn in the subworld. When false is returned the
	/// NPC is deselected from <see cref="Main.townNPCCanSpawn"/>
	/// and a new <see cref="WorldGen.prioritizedTownNPCType"/>
	/// is chosen.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool CanSpawnTownNPC(int type, int numTownNPCs) => VanillaUpdates;

	/// <summary>
	/// Use to edit the spawn pool of the subworld. 
	/// Gets called form <see cref="ExtendedSubworldGlobalNPC"/>.
	/// </summary>
	public virtual void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) { }

	/// <summary>
	/// Use to edit the spawn rate of the subworld.
	/// Gets called form <see cref="ExtendedSubworldGlobalNPC"/>.
	/// </summary>
	public virtual void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns) { }

	/// <summary>
	/// Use to edit the spawn range of the subworld.
	/// Gets called form <see cref="ExtendedSubworldGlobalNPC"/>.
	/// </summary>
	public virtual void EditSpawnRange(Player player, ref int spawnRangeX, ref int spawnRangeY, ref int safeRangeX, ref int safeRangeY) { }
	#endregion

	#region ---------- Tiles ----------
	public virtual bool CanDrop(int i, int j, int type) => true;

	public virtual bool CanExplode(int i, int j, int type) => true;

	public virtual bool CanPlace(int i, int j, int type) => true;

	public virtual bool CanPlaceTile(int i, int j, int type, int player, int style) => true;

	public virtual bool CanKillTile(int i, int j, int type, ref bool blockDamaged) => true;

	public virtual bool CanReplace(int i, int j, int type, int tileTypeBeingPlaced) => true;

	public virtual bool Slope(int i, int j, int type) => true;

	public virtual bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak) => true;

	public virtual bool AutoSelect(int i, int j, int type, Item item) => true;

	public virtual void DropCritterChance(int i, int j, int type, ref int wormChance, ref int grassHopperChance, ref int jungleGrubChance) { }

	public virtual bool? IsTileDangerous(int i, int j, int type, Player player) => null;
	#endregion

	#region ---------- System ----------
	public virtual void ClearWorld() { }

	public virtual void ModifyLightingBrightness(ref float scale) { }

	public virtual void ModifyTimeRate(ref double timeRate, ref double tileUpdateRate, ref double eventUpdateRate) { }

	public virtual void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor) { }

	public virtual void PreUpdateEntities() { }

	public virtual void PreUpdatePlayers() { }

	public virtual void PostUpdatePlayers() { }

	public virtual void PreUpdateNPCs() { }

	public virtual void PostUpdateNPCs() { }

	public virtual void PreUpdateGores() { }

	public virtual void PostUpdateGores() { }

	public virtual void PreUpdateProjectiles() { }

	public virtual void PostUpdateProjectiles() { }

	public virtual void PreUpdateItems() { }

	public virtual void PostUpdateItems() { }

	public virtual void PreUpdateDusts() { }

	public virtual void PostUpdateDusts() { }

	public virtual void PreUpdateTime() { }

	public virtual void PostUpdateTime() { }

	public virtual void PreUpdateWorld() { }

	public virtual void PostUpdateWorld() { }

	public virtual void PreUpdateInvasions() { }

	public virtual void PostUpdateInvasions() { }

	public virtual void PostUpdateEverything() { }

	public virtual void UpdateUI(GameTime gameTime) { }
	#endregion

	#region ---------- Items ----------
	/// <summary>
	/// Determines if the <see cref="ItemID.IceRod"/> can
	/// create ice blocks on the given location in the 
	/// subworld.
	/// <br/>Returns <see cref="VanillaUpdates"/> by default.
	/// </summary>
	public virtual bool CanPlaceIceBlock(int i, int j, int player) => VanillaUpdates;

	public virtual bool CanUseItem(Item item, Player player) => true;

	public virtual bool? CanCatchNPC(Item item, NPC target, Player player) => null;

	public virtual void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration) { }

	public virtual void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend) { }
	#endregion

	#region ---------- Players ----------
	/// <summary>
	/// Determines if boots like <see cref="ItemID.FlowerBoots"/>
	/// can place tiles on certain tiles in the subworld.
	/// Returns true by default.
	/// </summary>
	public virtual bool CanDoBootsEffects(Player player) => true;

	/// <summary>
	/// Determines if the player can use buckets in the subworld.
	/// Returns true by default.
	/// </summary>
	public virtual bool CanUseBuckets(Player player) => true;

	/// <summary>
	/// Determines if the player can use the lawn mower to alter
	/// grass tiles in the subworld.
	/// Returns true by default.
	/// </summary>
	public virtual bool CanMowGrassTile(Player player, Vector2 position) => true;

	/// <summary>
	/// Determines if the tile can be affected by <see cref="ItemID.DirtRod"/>
	/// Returns true by default.
	/// </summary>
	public virtual bool CanUseDirtRod(Player player, Tile tile) => true;

	/// <summary>
	/// Determines if the player can use any wiring tools to
	/// edit the wiring on the position <see cref="Player.tileTargetX"/> by
	/// <see cref="Player.tileTargetY"/>.
	/// Returns true by default.
	/// </summary>
	public virtual bool CanDoWireStuffHere(Player player) => true;

	/// <summary>
	/// Determines if the player can use wings to fly in this
	/// subworld. This is used during <see cref="UpdateEquips(Player)"/>.
	/// When disabled it will force the wing time to zero.
	/// Returns true by default.
	/// </summary>
	public virtual bool CanUseWings(Player player) => true;

	/// <summary>
	/// Determines if the player can use rocket boots accessory
	/// to fly in this subworld. This is used during 
	/// <see cref="UpdateEquips(Player)"/>. When disabled it will
	/// force the rocket boots off.
	/// Returns true by default.
	/// </summary>
	public virtual bool CanUseRocketBoots(Player player) => true;

	/// <summary>
	/// Determines if the player can use the flying carpet 
	/// accessory to fly in this subworld. This is used during
	/// <see cref="UpdateEquips(Player)"/>. When disabled it will
	/// force the carpet off.
	/// Returns true by default.
	/// </summary>
	public virtual bool CanUseCarpet(Player player) => true;

	/// <summary>
	/// Determines if mounts can be used in this subworld.
	/// This is used during <see cref="Mount.SetMount(int, Player, bool)"/> 
	/// and <see cref="Mount.CanMount(int, Player)"/>.
	/// Effectively preventing the use of any mount.
	/// Returns true by default.
	/// </summary>
	public virtual bool CanUseMounts(Player player) => true;

	/// <summary>
	/// Determines if a player is able to use accessories
	/// that allow the player to wall jump on tiles.
	/// This disables the use of <see cref="ItemID.ShoeSpikes"/>,
	/// <see cref="ItemID.ClimbingClaws"/> and upgrades.
	/// <br/>Returns true by default.
	/// </summary>
	public virtual bool CanUseWallJumps(Player player) => true;

	/// <summary>
	/// Determines if the player can use teleports. This is the
	/// generic teleport used by <see cref="TileID.Teleporter"/>.
	/// Might be used by other methods and possibly mods.
	/// <br/>Returns true by default.
	/// </summary>
	public virtual bool CanUseTeleport(Player player) => true;

	/// <summary>
	/// Determines if the player can use <see cref="ItemID.TeleportationPotion"/>.
	/// Might be used by other methods and possibly mods.
	/// <br/>Returns true by default.
	/// </summary>
	public virtual bool CanUseTeleportationPotion(Player player) => true;

	/// <summary>
	/// Determines if the player can use <see cref="ItemID.PotionOfReturn"/>.
	/// Might be used by other methods and possibly mods.
	/// <br/>Returns true by default.
	/// </summary>
	public virtual bool CanUsePotionOfReturn(Player player) => true;

	/// <summary>
	/// Determines if the player can use <see cref="ItemID.WormholePotion"/>.
	/// Might be used by other methods and possibly mods.
	/// <br/>Returns true by default.
	/// </summary>
	public virtual bool CanUseWormholePotion(Player player) => true;

	/// <summary>
	/// Determines if the player can use recall items (Magic mirror and similar items).
	/// This blocks <see cref="Player.Spawn(PlayerSpawnContext)"/> from
	/// running when the <see cref="PlayerSpawnContext"/> argument is
	/// equals to <see cref="PlayerSpawnContext.RecallFromItem"/>.
	/// <br/>Returns true by default.
	/// </summary>
	public virtual bool CanUseRecall(Player player) => true;

	/// <summary>
	/// Determines if the player can use <see cref="ItemID.DemonConch"/> and
	/// upgrades to teleport to the underworld.
	/// Might be used by other methods and possibly mods.
	/// <br/>Returns true by default.
	/// </summary>
	public virtual bool CanUseDemonConch(Player player) => true;

	/// <summary>
	/// Determins if the player can use <see cref="ItemID.MagicConch"/> and
	/// upgrades to teleport to the oceans.
	/// Might be used by other methods and possibly mods.
	/// <br/>Returns true by default.
	/// </summary>
	public virtual bool CanUseMagicConch(Player player) => true;

	/// <summary>
	/// Determins if the player can use <see cref="ItemID.Shellphone"/>.
	/// Might be used by other methods and possibly mods.
	/// <br/>Returns true by default.
	/// </summary>
	public virtual bool CanUseShellphone(Player player) => true;

	/// <summary>
	/// Determines if the player can use <see cref="ItemID.RodofDiscord"/>
	/// and alternatives to teleport at will.
	/// <br/>Returns true by default.
	/// </summary>
	public virtual bool CanUseTeleportRod(Player player) => true;

	/// <summary>
	/// Determines if the player can use <see cref="ItemID.PortalGun"/>
	/// and go throught portals created by the portal gun item or tile.
	/// <br/>Returns true by default.
	/// </summary>
	public virtual bool CanUsePortalGun(Player player) => true;

	/// <summary>
	/// Determines if the player can use pylons to teleport.
	/// <br/>Returns true by default.
	/// </summary>
	public virtual bool CanUsePylon(Player player) => true;

	/// <summary>
	/// Determines if the player can use teleport using the
	/// <see cref="ItemID.QueenSlimeHook"/>.
	/// <br/>Returns true by default.
	/// </summary>
	public virtual bool CanUseHookOfDissonance(Player player) => true;

	public virtual void PreUpdate(Player player) { }

	public virtual void PreUpdateBuffs(Player player) { }

	public virtual void PreUpdateMovement(Player player) { }

	public virtual void PostUpdateRunSpeeds(Player player) { }

	public virtual void UpdateEquips(Player player) 
	{
		// Check if wings are enabled in the subworld,
		// if not disable the wings.
		if (!CanUseWings(player))
		{
			player.wingTime = 0f;
			player.wingTimeMax = 0;
		}

		// Check if rocket boots are enabled in the subworld,
		// if not disable the rocket boots.
		if (!CanUseRocketBoots(player))
		{ 
			player.rocketBoots = 0;
			player.vanityRocketBoots = 0;
		}

		// Check if the flying carpet are enabled in the subworld,
		// if not disable the flying carpet.
		if (!CanUseCarpet(player))
        {
			player.carpet = false;
			player.carpetFrame = -1;
			player.canCarpet = false;
        }

		// Check if wings are enabled in the subworld,
		// if not disable the wings.
		if (!CanUseMounts(player) && player.mount?.Active == true)
		{
			player.mount.Dismount(player);
		}

		// Check if extra jumps are enabled in the subworld,
		// if not disable extra jumps.
		if (!CanUseWallJumps(player))
		{
			player.spikedBoots = 0;
		}
    }

	public virtual void PostUpdateEquips(Player player) { }

	public virtual void PostUpdate(Player player) { }

	public virtual bool CanStartExtraJump(Player player, ExtraJump jump) => true;

	public virtual void OnRespawn(Player player) { }

	public virtual void OnEnterWorld(Player player) { }

	/// <summary>
	/// Use to modify the spawn position of the player in this subworld.
	/// Write to player.SpawnX and player.SpawnY to modify the tile position.
	/// Make sure the position is valid (in world and not blocked by tiles).
	/// When successfully modified return true to block vanilla code from running.
	/// Returns false by default.
	/// </summary>
	public virtual bool ModifySpawn(Player player, PlayerSpawnContext context) => false;

	/// <summary>
	/// Use to block the spawn code from happening for the player.
	/// Return false to block the spawn logic.
	/// Returns true by default. 
	/// </summary>
	public virtual bool Spawn(Player player, PlayerSpawnContext context) => true;

	/// <summary>
	/// Use to block and/or modify any teleport. Return false to disallow
	/// the teleport. The position argument is the position where the player 
	/// gets teleported to. Use the style argument to determine which source
	/// teleported the player.<br/>
	/// Returns true by default (allows the teleport).<br/>
	/// <br/>
	/// Style index:<br/>
	/// 0	= Default (teleporter tile)<br/>
	/// 1	= RoD/RoH<br/>
	/// 2	= Teleportation potion<br/>
	/// 3	= Wormhole potion<br/>
	/// 4	= Portal gun<br/>
	/// 5	= Magic conch<br/>
	/// 6	= ???<br/>
	/// 7	= Demon conch<br/>
	/// 8	= Potion of return<br/>
	/// 9	= Pylon<br/>
	/// 10	= Hook of dissonance<br/>
	/// 11	= Shellphone<br/>
	/// 12	= Shimmer unstuck<br/>
	/// </summary>
	public virtual bool Teleport(Player player, ref float x, ref float y, int style, int extraInfo)
	{
		return style switch
		{
			0 => CanUseTeleport(player),
			1 => CanUseTeleportRod(player),
			2 => CanUseTeleportationPotion(player),
			3 => CanUseWormholePotion(player),
			4 => CanUsePortalGun(player),
			5 => CanUseMagicConch(player),
			6 => true,
			7 => CanUseDemonConch(player),
			8 => CanUsePotionOfReturn(player),
			9 => CanUsePylon(player),
			10 => CanUseHookOfDissonance(player),
			11 => CanUseShellphone(player),
			_ => true
		};
	}

	/// <summary>
	/// Whether this subworld should be saved to the player
	/// as a logout subworld. If this is set to true the
	/// player will be send back to the subworld where they
	/// exited the game. The subworld is saved upon player 
	/// saving.
	/// Returns false by default.
	/// </summary>
	public virtual bool SaveLogoutSubworld => false;

	/// <summary>
	/// Whether the position of the player inside this subworld 
	/// should be saved to the player. Only works when
	/// <see cref="SaveLogoutSubworld"/> returns true.
	/// The player will be placed on this position when rejoining
	/// the world. The position is saved upon player saving. 
	/// Returns false by default.
	/// </summary>
	public virtual bool SaveLogoutPosition => false;
	#endregion

	#region ---------- Projectiles ----------
	public virtual bool? CanUseGrapple(int type, Player player) => null;

	public virtual bool? GrappleCanLatchOnTo(Projectile projectile, Player player, int x, int y) => null;

	public virtual bool? CanCutTiles(Projectile projectile) => null;
	#endregion
}