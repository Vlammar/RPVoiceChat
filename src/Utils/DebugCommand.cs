using ProtoBuf;

namespace rpvoicechat
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class DebugCommand
    {
        public string Command { get; set; }
    }
}
