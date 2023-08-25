using Cairo;
using Cairo.Util;
using rpvoicechat;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace RPVoiceChat
{
    public class DebugSourceListDialog : GuiDialog
    {
        private ICoreClientAPI api;
        private AudioOutputManager audioOutputManager;

        private long gameTick;

        private int btnHeight = 20;
        private int btnWidth = 200;

        public DebugSourceListDialog(ICoreClientAPI client, AudioOutputManager audioOutputManager) : base(client) 
        {
            this.api = client;
            this.audioOutputManager = audioOutputManager;
            
        }


        public override bool TryOpen()
        {

            SetupDialog();

            var success = base.TryOpen();

            if (!success) return false;

            return true;
        }

        private void SetupDialog()
        {
            Composers["rpvc"] = Compose(audioOutputManager.PlayerSources.ToArray()).Compose();
        }

        private GuiComposer Compose(KeyValuePair<string, PlayerAudioSource>[] keyValuePairs)
        {

            // General window
            GuiComposer composer = api.Gui
                .CreateCompo("rpvc:debugmenu", new()
                {
                    Alignment = EnumDialogArea.LeftTop,
                    BothSizing = ElementSizing.FitToChildren,
                    fixedOffsetX = 20,
                    fixedOffsetY = 20
                })
                .AddShadedDialogBG(ElementBounds.Fill, true)
                .AddDialogTitleBar("Nearby Audio Sources", CloseDialog)
                .BeginChildElements();
            
            // Prep for list
            int i = 0;

            // Setup local player
            PlayerAudioSource localSource = audioOutputManager.LocalPlayerAudioSource;
            composer.AddButton(localSource.Player.PlayerName, () => { api.ShowChatMessage("This is " + localSource.Player.PlayerName + " and they are " + Enum.GetName(typeof(VoiceLevel), localSource.VoiceLevel)); return true; }, ElementBounds.Fixed(0, i, btnWidth, btnHeight));
            i += btnHeight;
            
            // Setup each player audio source beside local player
            foreach (var keyValuePair in keyValuePairs)
            {
                composer.AddButton(keyValuePair.Value.Player.PlayerName, () => { api.ShowChatMessage("This is " + keyValuePair.Value.Player.PlayerName + " and they are " + Enum.GetName(typeof(VoiceLevel), keyValuePair.Value.VoiceLevel).ToLower()); return true; }, ElementBounds.Fixed(0, i, btnWidth, btnHeight));
                i += btnHeight;
            }

            // Create custom source
            i += 2 * btnHeight;
            composer.AddButton("Create source", () => { api.ShowChatMessage("Created custom audio source at current location"); return true; }, ElementBounds.Fixed(0, i, btnWidth, btnHeight));

            return composer.EndChildElements();
        }

        private void CloseDialog()
        {
            TryClose();
        }

        public override void OnGuiClosed()
        {
            base.OnGuiClosed();

            api.Event.UnregisterGameTickListener(gameTick);
        }

        public override void OnGuiOpened()
        {
            base.OnGuiOpened();

            gameTick = api.Event.RegisterGameTickListener(UpdateDialog, 500);
        }

        private void RefreshVisuals()
        {
            // Called whenever the dialog is opened
            // 
        }
        
        private void UpdateDialog(float obj)
        {
            // Update the dialog visual here
            // Something like rechecking for audiosources around the player
            // Likely something we should be storing in the AudioOutputManager

        }

        public override string ToggleKeyCombinationCode => null;
    }
}
