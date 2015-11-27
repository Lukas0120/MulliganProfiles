using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartBot.Plugins;
using SmartBot.Plugins.API;


namespace RopeToVictory
{
    [Serializable]
    public class RopeToVictoryContainer : PluginDataContainer
    {
        //Init vars
        public RopeToVictoryContainer()
        {
            Name = "RopeToVictory";
        }
        
    }

    public class RopeToVictory : Plugin
    {
        public override async void OnTurnBegin()
        {
            Bot.SuspendBot();
            await Task.Delay(48000);
            Bot.ResumeBot();
        }

    }
}
