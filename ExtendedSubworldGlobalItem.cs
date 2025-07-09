using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SubworldToolkit;

public class ExtendedSubworldGlobalItem : GlobalItem
{
	public override bool CanUseItem(Item item, Player player)
	{
		if (ExtendedSubworldSystem.Current?.CanUseItem(item, player) == false)
		{ 
			return false;
		}

		switch (item.type)
		{
			case ItemID.GoblinBattleStandard:
				{
					if (ExtendedSubworldSystem.Current?.GoblinArmyEnabled == false)
					{
						return false;
					}
				}
				break;
			
			case ItemID.SnowGlobe:
				{
					if (ExtendedSubworldSystem.Current?.FrostLegionEnabled == false)
					{
						return false;
					}
				}
				break;
			
			case ItemID.PirateMap:
				{
					if (ExtendedSubworldSystem.Current?.PirateInvasionEnabled == false)
					{
						return false;
					}
				}
				break;

			case ItemID.PumpkinMoonMedallion:
				{
					if (ExtendedSubworldSystem.Current?.PumpkinMoonEnabled == false)
					{
						return false;
					}
				}
				break;

			case ItemID.NaughtyPresent:
				{
					if (ExtendedSubworldSystem.Current?.SnowMoonEnabled == false)
					{
						return false;
					}
				}
				break;

			case ItemID.BloodMoonStarter:
				{
					if (ExtendedSubworldSystem.Current?.BloodMoonEnabled == false)
					{
						return false;
					}
				}
				break;

			case ItemID.SolarTablet:
				{
					if (ExtendedSubworldSystem.Current?.SolarEclipseEnabled == false)
					{
						return false;
					}
				}
				break;

			case ItemID.TeleportationPotion:
				{
					if (ExtendedSubworldSystem.Current?.CanUseTeleportationPotion(player) == false)
					{
						return false;
					}
				}
				break;

			case ItemID.PotionOfReturn:
				{
					if (ExtendedSubworldSystem.Current?.CanUsePotionOfReturn(player) == false)
					{
						return false;
					}
				}
				break;

			case ItemID.MagicMirror:
				{
					if (ExtendedSubworldSystem.Current?.CanUseRecall(player) == false)
					{
						return false;
					}
				}
				break;

			case ItemID.MagicConch:
				{
					if (ExtendedSubworldSystem.Current?.CanUseMagicConch(player) == false)
					{
						return false;
					}
				}
				break;

			case ItemID.DemonConch:
				{
					if (ExtendedSubworldSystem.Current?.CanUseDemonConch(player) == false)
					{
						return false;
					}
				}
				break;

			case ItemID.Shellphone:
				{
					if (ExtendedSubworldSystem.Current?.CanUseShellphone(player) == false)
					{
						return false;
					}
				}
				break;

			case ItemID.RodofDiscord:
			case ItemID.RodOfHarmony:
				{
					if (ExtendedSubworldSystem.Current?.CanUseTeleportRod(player) == false)
					{
						return false;
					}
				}
				break;

			case ItemID.PortalGun:
				{
					if (ExtendedSubworldSystem.Current?.CanUsePortalGun(player) == false)
					{
						return false;
					}
				}
				break;
		}
		
		return base.CanUseItem(item, player);
	}

	public override bool? CanCatchNPC(Item item, NPC target, Player player)
	{
		return ExtendedSubworldSystem.Current?.CanCatchNPC(item, target, player);
	}

	public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
	{
		ExtendedSubworldSystem.Current?.HorizontalWingSpeeds(item, player, ref speed, ref acceleration);
	}

	public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
	{
		ExtendedSubworldSystem.Current?.VerticalWingSpeeds(item, player, ref ascentWhenFalling, ref ascentWhenRising, ref maxCanAscendMultiplier, ref maxAscentMultiplier, ref constantAscend);
	}
}