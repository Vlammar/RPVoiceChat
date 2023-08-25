using ProtoBuf;

namespace RPVoiceChat
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class ConnectionInfo
    {
        public string Address { get; set; }
        public int Port { get; set; }
    }
}
