using rpvoicechat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;

namespace RPVoiceChat
{
    public class DebugMenuDialog : GuiDialog
    {
        private ICoreClientAPI api;
        private AudioOutputManager audioOutputManager;

        private bool isSetup;
        private long gameTick;

        // Menu specific things
        private PlayerAudioSource SelectedSource;

        public DebugMenuDialog(ICoreClientAPI client, AudioOutputManager audioOutputManager) : base(client) 
        {
            this.api = client;
            this.audioOutputManager = audioOutputManager;
        }

        private void SetupDialog()
        {
            // Called when the dialog is opened for the first time.
            // Used to build the dialog for the first time

            var bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            bgBounds.BothSizing = ElementSizing.FitToChildren;

            var audioSourceListBounds = ElementBounds.Fixed(200, 200);


            var composer = api.Gui.CreateCompo("rpvcdebugmenu", ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle))
                .AddShadedDialogBG(bgBounds)
                .AddDialogTitleBar("RP VC Config Menu", CloseDialog)
                .AddContainer(audioSourceListBounds, "rpvcDebugAudioSourceList");

            
        }

        public override bool TryOpen()
        {
            if (!isSetup)
                SetupDialog();

            var success = base.TryOpen();

            if (!success) return false;

            RefreshVisuals();
            return true;
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
