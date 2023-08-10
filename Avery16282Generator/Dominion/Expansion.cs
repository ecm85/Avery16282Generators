using System;

namespace Avery16282Generator.Dominion
{
	public enum Expansion
	{
		Adventures,
		Alchemy,
		Allies,
		Animals,
		Base,
		Cornucopia,
		Dark_Ages,
		Dominion_1st_Edition,
		Dominion_2nd_Edition,
		Dominion_2nd_Edition_Upgrade,
		Empires,
		Guilds,
		Hinterlands_1st_Edition,
		Hinterlands_2nd_Edition,
		Hinterlands_2nd_Edition_Upgrade,
		Intrigue_1st_Edition,
		Intrigue_2nd_Edition,
		Intrigue_2nd_Edition_Upgrade,
		Menagerie,
		Nocturne,
		Plunder,
		Promo,
		Prosperity_1st_Edition,
		Prosperity_2nd_Edition,
		Prosperity_2nd_Edition_Upgrade,
		Seaside_1st_Edition,
		Seaside_2nd_Edition,
		Seaside_2nd_Edition_Upgrade,
		Renaissance
	}

	public static class ExpansionExtensions
	{
		public static string GetExpansionName(this Expansion expansion)
		{
			switch (expansion)
			{
				case Expansion.Adventures:
					return "Adventures";
				case Expansion.Alchemy:
					return "Alchemy";
				case Expansion.Allies:
					return "Allies";
				case Expansion.Animals:
					return "Animals";
				case Expansion.Base:
					return "Base";
				case Expansion.Cornucopia:
					return "Cornucopia";
				case Expansion.Dark_Ages:
					return "Dark Ages";
				case Expansion.Dominion_1st_Edition:
					return "Dominion 1st Edition";
				case Expansion.Dominion_2nd_Edition:
					return "Dominion 2nd Edition";
				case Expansion.Dominion_2nd_Edition_Upgrade:
					return "Dominion 2nd Edition Upgrade";
				case Expansion.Empires:
					return "Empires";
				case Expansion.Guilds:
					return "Guilds";
				case Expansion.Hinterlands_1st_Edition:
					return "Hinterlands 1st Edition";
				case Expansion.Hinterlands_2nd_Edition:
					return "Hinterlands 2nd Edition";
				case Expansion.Hinterlands_2nd_Edition_Upgrade:
					return "Hinterlands 2nd Edition Upgrade";
				case Expansion.Intrigue_1st_Edition:
					return "Intrigue 1st Edition";
				case Expansion.Intrigue_2nd_Edition:
					return "Intrigue 2nd Edition";
				case Expansion.Intrigue_2nd_Edition_Upgrade:
					return "Intrigue 2nd Edition Upgrade";
				case Expansion.Menagerie:
					return "Menagerie";
				case Expansion.Nocturne:
					return "Nocturne";
				case Expansion.Plunder:
					return "Plunder";
				case Expansion.Promo:
					return "Promo";
				case Expansion.Prosperity_1st_Edition:
					return "Prosperity 1st Edition";
				case Expansion.Prosperity_2nd_Edition:
					return "Prosperity 2nd Edition";
				case Expansion.Prosperity_2nd_Edition_Upgrade:
					return "Prosperity 2nd Edition Upgrade";
				case Expansion.Seaside_1st_Edition:
					return "Seaside 1st Edition";
				case Expansion.Seaside_2nd_Edition:
					return "Seaside 2nd Edition";
				case Expansion.Seaside_2nd_Edition_Upgrade:
					return "Seaside 2nd Edition Upgrade";
				case Expansion.Renaissance:
					return "Renaissance";
				default:
					throw new InvalidOperationException();
			}
		}
	}
}
