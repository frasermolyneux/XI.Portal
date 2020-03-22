using System;
using System.IO;
using Newtonsoft.Json;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.Library.DemoManager.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface IDemo
    {
        [JsonProperty]
        GameType Version { get; }

        [JsonProperty]
        string Name { get; }

        [JsonProperty]
        DateTime Date { get; }

        [JsonProperty]
        string Map { get; }

        [JsonProperty]
        string Mod { get; }

        [JsonProperty]
        string GameType { get; }

        [JsonProperty]
        string Server { get; }

        [JsonProperty]
        long Size { get; }

        Stream Open();
    }
}