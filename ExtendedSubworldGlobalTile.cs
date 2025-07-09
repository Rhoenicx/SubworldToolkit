using Terraria;
using Terraria.ModLoader;

namespace SubworldToolkit;

public class ExtendedSubworldGlobalTile : GlobalTile
{
	public override bool CanDrop(int i, int j, int type)
	{
		if (ExtendedSubworldSystem.Current?.CanDrop(i, j, type) == false)
		{
			return false;
		}

		return base.CanDrop(i,j, type);
	}

	public override bool CanExplode(int i, int j, int type)
	{
		if (ExtendedSubworldSystem.Current?.CanExplode(i, j, type) == false)
		{
			return false;
		}

		return base.CanExplode(i, j, type);
	}

	public override bool CanPlace(int i, int j, int type)
	{
		if (ExtendedSubworldSystem.Current?.CanPlace(i, j, type) == false)
		{
			return false;
		}

		return base.CanPlace(i, j, type);
	}

	public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
	{
		if (ExtendedSubworldSystem.Current?.CanKillTile(i, j, type, ref blockDamaged) == false)
		{
			return false;
		}

		return base.CanKillTile(i, j, type, ref blockDamaged);
	}

	public override bool CanReplace(int i, int j, int type, int tileTypeBeingPlaced)
	{
		if (ExtendedSubworldSystem.Current?.CanReplace(i, j, type, tileTypeBeingPlaced) == false)
		{
			return false;
		}

		return base.CanReplace(i, j, type, tileTypeBeingPlaced);
	}

	public override bool Slope(int i, int j, int type)
	{
		if (ExtendedSubworldSystem.Current?.Slope(i, j, type) == false)
		{
			return false;
		}

		return base.Slope(i, j, type);
	}

	public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak)
	{
		if (ExtendedSubworldSystem.Current?.TileFrame(i, j, type, ref resetFrame, ref noBreak) == false)
		{
			return false;
		}

		return base.TileFrame(i, j, type, ref resetFrame, ref noBreak);
	}

	public override bool AutoSelect(int i, int j, int type, Item item)
	{
		if (ExtendedSubworldSystem.Current?.AutoSelect(i, j, type, item) == false)
		{
			return false;
		}

		return base.AutoSelect(i, j, type, item);
	}

	public override void DropCritterChance(int i, int j, int type, ref int wormChance, ref int grassHopperChance, ref int jungleGrubChance)
	{
		ExtendedSubworldSystem.Current?.DropCritterChance(i, j, type, ref wormChance, ref grassHopperChance, ref jungleGrubChance);
	}

	public override bool? IsTileDangerous(int i, int j, int type, Player player)
	{
		return ExtendedSubworldSystem.Current?.IsTileDangerous(i, j, type, player);
	}
}
