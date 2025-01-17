﻿using System.ComponentModel;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace NoMoreTombs
{
	public class Configuration : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		public static Configuration Instance;
		//Label & Tooltip deprecated. Use Keys or just specify as Args: it will be interpreted as a key if beginning starts with "$".
		//https://github.com/tModLoader/tModLoader/pull/3302
		[DefaultValue(true)]
		[LabelArgs("$Disable Tombstones")]
		[TooltipArgs("$Prevents tombstones from being spawned when you die\nDefaults to true")]
		public bool NoTombstones;

		[DefaultValue(false)]
		[LabelArgs("$Disable Death Message")]
		[TooltipArgs("$Prevents death messages from being shown when you die\nDefaults to false")]
		public bool NoDeathMessage;

		[DefaultValue(false)]
		[LabelArgs("$Enable Tombs From Town NPC Deaths")]
		[TooltipArgs("$Allows town NPCs to drop tombstones when they die while the player is on any difficulty\nDefaults to false")]
		public bool TownNPCTombs;

		[DefaultValue(false)]
		[LabelArgs("$Disable Tombs From Town NPC Deaths in Hardcore")]
		[TooltipArgs("$Disables town NPCs dropping tombstones when they die while the player is on hardcore mode\nTakes precedense over the previous setting\nDefaults to false")]
        public bool NoTownNPCTombs;

		private bool OldNoTombstones;
		private bool OldNoDeathMessage;
		private bool OldTownNPCTombs;
		private bool OldNoTownNPCTombs;

		public override void OnLoaded()
		{
			OldNoTombstones = NoTombstones;
			OldNoDeathMessage = NoDeathMessage;
			OldTownNPCTombs = TownNPCTombs;
			OldNoTownNPCTombs = NoTownNPCTombs;
		}

		public override void OnChanged()
		{
			if (OldNoTombstones != NoTombstones)
			{
				Mod.Logger.Info((NoTombstones ? "Disabled" : "Enabled") + " Tombstones");
				OldNoTombstones = NoTombstones;
			}
			if (OldNoDeathMessage != NoDeathMessage)
			{
				Mod.Logger.Info((NoDeathMessage ? "Disabled" : "Enabled") + " Death Messages");
				OldNoDeathMessage = NoDeathMessage;
			}
			if (OldTownNPCTombs != TownNPCTombs)
			{
				Mod.Logger.Info((TownNPCTombs ? "Enabled" : "Disabled") + " Town NPC Tombs");
				OldTownNPCTombs = TownNPCTombs;
			}
			if (OldNoTownNPCTombs != NoTownNPCTombs)
			{
				Mod.Logger.Info((NoTownNPCTombs ? "Disabled" : "Enabled") + " Town NPC Tombs on Hardcore");
				OldNoTownNPCTombs = NoTownNPCTombs;
			}
		}

		public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message)
		{
			if (!IsPlayerLocalServerOwner(whoAmI))
			{
				message = "Sorry, config settings can only be changed by the server owner.";
				return false;
			}
			return true;
		}

		public static bool IsPlayerLocalServerOwner(int whoAmI)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				return Netplay.Connection.Socket.GetRemoteAddress().IsLocalHost();
			}

			for (int i = 0; i < Main.maxPlayers; i++)
			{
				RemoteClient client = Netplay.Clients[i];
				if (client.State == 10 && i == whoAmI && client.Socket.GetRemoteAddress().IsLocalHost())
				{
					return true;
				}
			}
			return false;
		}
	}
}
