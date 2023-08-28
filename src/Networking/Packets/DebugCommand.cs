using ProtoBuf;

namespace RPVoiceChat
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class DebugCommand
    {
        // Command itself
        public string Command { get; set; }

        // Parameters to be sent back and forth and potentially used by command
        public string ParameterString { get; set; }
        public int ParameterInt { get; set; }
        public float ParameterFloat { get; set; }
        public double ParameterDouble { get; set; }
        public bool ParameterBool { get; set; }
    }
}
