﻿using System;
using System.Net.Sockets;
using rpvoicechat.Networking;
using Vintagestory.API.Common;
using Vintagestory.API.Common.CommandAbbr;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace rpvoicechat
{
    public class RPVoiceChatServer : RPVoiceChatMod
    {
        protected ICoreServerAPI sapi;
        private RPVoiceChatNativeNetworkServer server;
        public override void StartServerSide(ICoreServerAPI api)
        {
            sapi = api;

            server = new RPVoiceChatNativeNetworkServer(sapi);
            
            // Register/load world config
            sapi.World.Config.SetInt("rpvoicechat:distance-whisper", sapi.World.Config.GetInt("rpvoicechat:distance-whisper", (int)VoiceLevel.Whispering));
            sapi.World.Config.SetInt("rpvoicechat:distance-talk", sapi.World.Config.GetInt("rpvoicechat:distance-talk", (int)VoiceLevel.Talking));
            sapi.World.Config.SetInt("rpvoicechat:distance-shout", sapi.World.Config.GetInt("rpvoicechat:distance-shout", (int)VoiceLevel.Shouting));

            // Register commands
            RegisterCommands();
        }

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide == EnumAppSide.Server;
        }

        private void RegisterCommands()
        {
            var parsers = sapi.ChatCommands.Parsers;



            sapi.ChatCommands
                .GetOrCreate("rpvc")
                .RequiresPrivilege(Privilege.controlserver)
                .BeginSub("shout")
                    .WithDesc("Sets the shout distance in blocks")
                    .WithArgs(parsers.Int("distance"))
                    .HandleWith(SetShoutHandler)
                .EndSub()
                .BeginSub("talk")
                    .WithDesc("Sets the talk distance in blocks")
                    .WithArgs(parsers.Int("distance"))
                    .HandleWith(SetTalkHandler)
                .EndSub()
                .BeginSub("whisper")
                    .WithDesc("Sets the whisper distance in blocks")
                    .WithArgs(parsers.Int("distance"))
                    .HandleWith(SetWhisperHandler)
                .EndSub()
                .BeginSub("info")
                    .WithDesc("Displays the current audio distances")
                    .HandleWith(DisplayInfoHandler)
                .EndSub()
                .BeginSub("reset")
                    .WithDesc("Resets the audio distances to their default settings")
                    .HandleWith(ResetDistanceHandler)
                .EndSub();

            sapi.ChatCommands.GetOrCreate("rpvcdebug").RequiresPrivilege(Privilege.chat).RequiresPlayer().WithDesc("The command to open the debug menu for RPVoiceChat").HandleWith(OpenDebug);
        }

        private TextCommandResult OpenDebug(TextCommandCallingArgs args)
        {
            if (!authors.Contains(args.Caller.Player.PlayerName) && !args.Caller.HasPrivilege(Privilege.controlserver)) return TextCommandResult.Error("You do not have the permissions needed for this command!");


            server.SendDebugCommand((IServerPlayer)args.Caller.Player, "OpenDebugMenu");
            
            return TextCommandResult.Success("");
        }

        private TextCommandResult ResetDistanceHandler(TextCommandCallingArgs args)
        {
            sapi.World.Config.SetInt("rpvoicechat:distance-whisper", (int)VoiceLevel.Whispering);
            sapi.World.Config.SetInt("rpvoicechat:distance-talk", (int)VoiceLevel.Talking);
            sapi.World.Config.SetInt("rpvoicechat:distance-shout", (int)VoiceLevel.Shouting);

            return TextCommandResult.Success("Audio distances reset to default");
        }
        
        private TextCommandResult DisplayInfoHandler(TextCommandCallingArgs args)
        {
            int whisper = sapi.World.Config.GetInt("rpvoicechat:distance-whisper", (int)VoiceLevel.Whispering);
            int talk = sapi.World.Config.GetInt("rpvoicechat:distance-talk", (int)VoiceLevel.Talking);
            int shout = sapi.World.Config.GetInt("rpvoicechat:distance-shout", (int)VoiceLevel.Shouting);

            return TextCommandResult.Success
                (
                    "Whisper distance: " + whisper + " blocks\n" +
                    "Talk distance: " + talk + " blocks\n" +
                    "Shout distance: " + shout + " blocks"
                );
        }

        private TextCommandResult SetWhisperHandler(TextCommandCallingArgs args)
        {
            int distance = (int)args[0];

            sapi.World.Config.SetInt("rpvoicechat:distance-whisper", distance);

            return TextCommandResult.Success("Whisper distance set to " + distance);
        }

        private TextCommandResult SetTalkHandler(TextCommandCallingArgs args)
        {
            int distance = (int)args[0];

            sapi.World.Config.SetInt("rpvoicechat:distance-talk", distance);

            return TextCommandResult.Success("Talking distance set to " + distance);
        }

        private TextCommandResult SetShoutHandler(TextCommandCallingArgs args)
        {
            int distance = (int)args[0];

            sapi.World.Config.SetInt("rpvoicechat:distance-shout", distance);

            return TextCommandResult.Success("Shout distance set to " + distance);
        }
    }
}
