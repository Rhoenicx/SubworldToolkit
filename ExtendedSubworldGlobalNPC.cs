using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace SubworldToolkit;

public class ExtendedSubworldGlobalNPC : GlobalNPC
{
	public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
	{
		ExtendedSubworldSystem.Current?.EditSpawnPool(pool, spawnInfo);
	}

	public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
	{
		ExtendedSubworldSystem.Current?.EditSpawnRate(player, ref spawnRate, ref maxSpawns);
	}

	public override void EditSpawnRange(Player player, ref int spawnRangeX, ref int spawnRangeY, ref int safeRangeX, ref int safeRangeY)
	{
		ExtendedSubworldSystem.Current?.EditSpawnRange(player, ref spawnRangeX, ref spawnRangeY, ref safeRangeX, ref safeRangeY);
	}
}
