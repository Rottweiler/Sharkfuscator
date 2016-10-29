using System.IO;

namespace Sharkfuscator
{
    interface iProtection
    {
        string name { get;  }
        string description { get;  }
        string author { get; }
        string init_message { get; }
        void Protect(Stream stream);
    }
}
