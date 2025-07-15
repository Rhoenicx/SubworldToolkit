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
					if (ExtendedSubworldSystem.Current?.CanStartGoblinArmy == false)
					{
						return false;
					}
				}
				break;
			
			case ItemID.SnowGlobe:
				{
					if (ExtendedSubworldSystem.Current?.CanStartFrostLegion == false)
					{
						return false;
					}
				}
				break;
			
			case ItemID.PirateMap:
				{
					if (ExtendedSubworldSystem.Current?.CanStartPirateInvasion == false)
					{
						return false;
					}
				}
				break;

			case ItemID.PumpkinMoonMedallion:
				{
					if (ExtendedSubworldSystem.Current?.CanStartPumpkinMoon == false)
					{
						return false;
					}
				}
				break;

			case ItemID.NaughtyPresent:
				{
					if (ExtendedSubworldSystem.Current?.CanStartSnowMoon == false)
					{
						return false;
					}
				}
				break;

			case ItemID.BloodMoonStarter:
				{
					if (ExtendedSubworldSystem.Current?.CanStartBloodMoon == false)
					{
						return false;
					}
				}
				break;

			case ItemID.SolarTablet:
				{
					if (ExtendedSubworldSystem.Current?.CanStartSolarEclipse == false)
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

			case ItemID.DirtRod:
				{
					int x = (int)((double)Main.mouseX + Main.screenPosition.X) / 16;
					int y = player.gravDir >= 0f
						? (int)((double)Main.mouseY + Main.screenPosition.Y) / 16
						: (int)(Main.screenPosition.Y + (double)Main.screenHeight - (double)Main.mouseY) / 16;

					if (ExtendedSubworldSystem.Current?.CanUseDirtRod(player, Main.tile[x, y]) == false)
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