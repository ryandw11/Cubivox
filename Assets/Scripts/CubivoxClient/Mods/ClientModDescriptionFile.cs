using CubivoxCore.Mods;
using System;

namespace CubivoxClient.Mods
{
    // This only exists since Unity cannot use System.Text.Json easily currently.
    [Serializable]
    public struct ClientModDescriptionFile
    {
        public string ModName;

        public string MainClass;

        public string Description;

        public string Version;

        public string[] Authors;
    }

    internal sealed class ModDescriptionUtils
    {
        public static ModDescriptionFile ClientToCoreDescriptionFile(ClientModDescriptionFile cDescFile)
        {
            return new ModDescriptionFile
            {
                ModName = cDescFile.ModName,
                MainClass = cDescFile.MainClass,
                Description = cDescFile.Description,
                Version = cDescFile.Version,
                Authors = cDescFile.Authors
            };
        }
    }
}
