# artnet-unifi-led
control unifi access point LEDs via ArtNet. Takes data via ArtNet ArtDmx and controls lights of the access points via SSH. See config.example.json for configuration sample.

Each device has two dmx channels: first is for the white LED, second is for the blue LED.

# Warning:
This is just a proof of concept, you probably shouldn't have your SSH credentials in some config file. Please do not use this in a productive environment.