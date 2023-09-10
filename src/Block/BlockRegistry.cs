﻿
using Vintagestory.API.Common;

namespace RPVoiceChat
{
    public class BlockRegistry
    {
        public static void RegisterBlocks(ICoreAPI api)
        {
            api.RegisterBlockClass("soundemittingblock", typeof(SoundEmittingBlock));
        }
    }
}
