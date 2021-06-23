using System.Collections.Generic;
using Newtonsoft.Json;

namespace Machine
{
    public enum OsType
    {
        Linux,
        Darwin, // == MacOS
        Windows,
        Unknown
    }
    
    /// <summary>
    /// Information about the Machine of the scan. Used for fingerprinting the machine and maybe scan for associated exploit.
    /// </summary>
    public class MachineInfo
    {

        public MachineInfo(OsType osType, string versionNumber, List<string> installedPrograms, List<string> suidPrograms)
        {
            Type = osType;
            VersionNumber = versionNumber;
            InstalledPrograms = installedPrograms;
            SuidPrograms = suidPrograms;
        }
        
        /// <summary>
        /// The version of the OS the Machine is running
        /// </summary>
        [JsonProperty("ostype")] 
        public OsType Type { get; set; }
        
        /// <summary>
        /// The version number of the OS.
        /// </summary>
        [JsonProperty("versionnumber")] 
        public string VersionNumber { get; set; }
        
        /// <summary>
        /// List of all installed programs on the machine, i.e. all files in /bin and /usr/bin/
        /// </summary>
        [JsonProperty("programs")] 
        public List<string> InstalledPrograms { get; set; }

        /// <summary>
        /// List of all suid programs. A subset of InstalledPrograms
        /// </summary>
        [JsonProperty("version")]
        public List<string> SuidPrograms { get; set; }
    }
}