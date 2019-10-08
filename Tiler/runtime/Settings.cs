using System.Runtime.Serialization;

namespace Tiler.runtime
{
    [DataContract]
    public class Settings
    {
        [DataMember] internal bool TaskIconClickShowsSettings;
    }
}