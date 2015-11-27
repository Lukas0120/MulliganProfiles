using System;
using SmartBot.Plugins;
using SmartBot.Plugins.API;

namespace SmartBotPlugins_art
{
    [Serializable]
    public class EndAtLegendContainer : PluginDataContainer
    {

        //Init vars
        public EndAtLegendContainer()
        {
            Name = "EndAtLegend";
        }
    }

    public class EndAtLegend : Plugin
    {

        public override void OnGameEnd()
        {
            if (Bot.CurrentMode() != Bot.Mode.Ranked || Bot.GetPlayerDatas().GetRank() !=0) return;
            Bot.Log(
                "You are legend, so pat yourself on the back for... uhm, resilience to convince your real life friends that you are not a bot. ");
            Bot.StopBot();
        }
    }
}
