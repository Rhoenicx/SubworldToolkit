using Terraria;
using Terraria.ModLoader;

namespace SubworldToolkit;

public class ExtendedSubworldGlobalProjectile : GlobalProjectile
{
	public override bool? CanUseGrapple(int type, Player player)
	{
		return ExtendedSubworldSystem.Current?.CanUseGrapple(type, player);
	}

	public override bool? GrappleCanLatchOnTo(Projectile projectile, Player player, int x, int y)
	{
		return ExtendedSubworldSystem.Current?.GrappleCanLatchOnTo(projectile, player, x, y);
	}

	public override bool? CanCutTiles(Projectile projectile)
	{
		return ExtendedSubworldSystem.Current?.CanCutTiles(projectile);
	}
}
