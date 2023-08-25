using ProtoBuf;

namespace RPVoiceChat
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class DebugCommand
    {
        public string Command { get; set; }
    }
}
