namespace ArtnetUnifiLed
{
    internal class UnifiDevice
    {
        public string Host { get; set; }
        public int Port { get; set; } = 22;
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
