# StreamBitrateSelector

### A [ResoniteModloader](https://github.com/resonite-modding-group/ResoniteModLoader) Mod for [Resonite](https://resonite.com/) used to easily set `UserAudioStream` bitrates to common values.

I needed an excuse to learn how to write mods for Resonite and the minor inconvienence of the `UserAudioStream` bitrate slider being a raw float value from `2.5 - 500` was just enough to motivate me!

This mod will replace the modal that's normally created for setting up a `UserAudioStream` with a new one. It's built *mostly* the same with a small difference: The slider no longer directly drives the bitrate float value. Instead it drives the index on a `ValueMultiplexer<float>` with a collection of common bitrate values. Which is, in turn, used to drive the actual bitrate.

The set of values was chosen based on [OBS-Studio](https://github.com/obsproject/obs-studio)'s set of audio stream bitrate values. The available bitrate settings are as follows (in Kbps): `32, 64, 96, 128, 160, 192, 224, 256, 288, 320`

Thanks to @RileyGuy and @Earthmark for their assistance in writing this mod!

#### *Built using the [ResoniteModTemplate](https://github.com/RileyGuy/ResoniteModTemplate) by @RileyGuy*