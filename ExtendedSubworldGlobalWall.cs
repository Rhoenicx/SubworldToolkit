using Terraria;
using Terraria.ModLoader;

namespace SubworldToolkit;

public class ExtendedSubworldGlobalWall : GlobalWall
{
	public override bool CanPlace(int i, int j, int type)
	{
		if (ExtendedSubworldSystem.current?.CanPlaceWall(i, j, type) == false)
		{
			return false;
		}

		return base.CanPlace(i, j, type);
	}

	public override bool CanExplode(int i, int j, int type)
	{
		if (ExtendedSubworldSystem.current?.CanExplodeWall(i, j, type) == false)
		{
			return false;
		}

		return base.CanExplode(i, j, type);
	}

	public override void KillWall(int i, int j, int type, ref bool fail)
	{
		if (ExtendedSubworldSystem.current?.CanKillWall(i, j, type) == false)
		{
			fail = true;
		}
	}

	public override bool CanBeTeleportedTo(int i, int j, int type, Player player, string context)
	{
		if (ExtendedSubworldSystem.current?.CanBeTeleportedTo(i, j, type, player, context) == false)
		{
			return false;
		}

		return base.CanBeTeleportedTo(i, j, type, player, context);
	}

	public override bool Drop(int i, int j, int type, ref int dropType)
	{
		if (ExtendedSubworldSystem.current?.CanDropWall(i, j, type) == false)
		{
			return false;
		}

		return base.Drop(i, j, type, ref dropType);
	}

	public override bool WallFrame(int i, int j, int type, bool randomizeFrame, ref int style, ref int frameNumber)
	{
		if (ExtendedSubworldSystem.current?.WallFrame(i, j, type, randomizeFrame, ref style, ref frameNumber) == false)
		{
			return false;
		}

		return base.WallFrame(i, j, type, randomizeFrame, ref style, ref frameNumber);
	}
}
