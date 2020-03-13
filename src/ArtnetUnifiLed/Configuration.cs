using System.Collections.Generic;

namespace ArtnetUnifiLed
{
    internal class Configuration
    {
        public int Universe { get; set; }
        public IList<UnifiDevice> Devices { get; set; }
    }
}
