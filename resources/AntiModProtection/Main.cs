using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using System.Collections.Generic;
using System.Linq;

namespace AntiModProtection
{
    public class Main : Script
    {
		public bool AllowOpenIV = false;
		public bool AllowCustomAsis = false;
		public Dictionary<string, string> AllowedAsis = new Dictionary<string, string>();

		public Main()
		{
			API.onPlayerConnected += OnPlayerConnectedHandler;
			API.onResourceStart += OnScriptStart;
		}

		public void OnScriptStart()
		{
			if (API.hasSetting("AllowOpenIV")) AllowOpenIV = API.getSetting<bool>("AllowOpenIV");
			if (API.hasSetting("AllowCustomAsis")) AllowCustomAsis = API.getSetting<bool>("AllowCustomAsis");

			// Add Whitelisted asi Files here "AllowedAsis.Add("AsiFileName.asi", "FileHash");"
			AllowedAsis.Add("SkipIntro.asi", "CF0F68109888CE43AEE861066A853E7A"); // Whitelist Default SkipIntro.asi
		}

		private void OnPlayerConnectedHandler(Client player)
		{
			if (!AllowOpenIV)
			{
				if (API.isPlayerOpenIVActive(player))
				{
					API.kickPlayer(player, "OpenIV is not allowed!");
				}
			}
			if (!AllowCustomAsis)
			{
				bool foundWrongAsi = false;
				foreach (var asi in player.activeAsis)
				{
					if(AllowedAsis.FirstOrDefault(x => x.Key == asi.Key).Key == null)
					{
						// Found not Whitelisted asi File
						foundWrongAsi = true;
					}
					else
					{
						if(AllowedAsis.FirstOrDefault(x => x.Key == asi.Key).Value != asi.Value)
						{ 
							// Wrong Hash of Whitelisted asi found
							foundWrongAsi = true;
						}
						
					}
					
				}
				if (foundWrongAsi) API.kickPlayer(player, "Usage of Custom Asi Files not allowed!");
			}
		}
	}
}
