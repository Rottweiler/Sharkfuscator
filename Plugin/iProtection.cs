using System.IO;

namespace Plugin
{
    public interface iProtection
    {
        string name { get;  }
        string description { get;  }
        string author { get; }
        string init_message { get; }
        char command_short { get; }
        string command_long { get; }
        bool enabled { get; set; }
        bool enabled_default { get; }
        bool Protect(ProtectorState state, string output_filename, Stream stream);
    }
}
