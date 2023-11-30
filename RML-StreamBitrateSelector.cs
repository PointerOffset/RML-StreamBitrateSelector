using HarmonyLib;
using ResoniteModLoader;
using System;
using System.Reflection;
using FrooxEngine;
using Elements.Core;
using FrooxEngine.UIX;

namespace StreamBitrateSelector;

public class StreamBitrateSelector : ResoniteMod
{
    public override string Name => "StreamBitrateSelector";
    public override string Author => "PointerOffset";
    public override string Version => "1.0.0";
    public override string Link => "resonite.com";
    public static ModConfiguration? Config;

    public override void OnEngineInit()
    {
        Harmony harmony = new("net.PointerOffset.StreamBitrateSelector");
        Config = GetConfiguration();
        Config?.Save(true);
        harmony.PatchAll();
    }

    [HarmonyPatch(typeof(AudioStreamSpawner))]
    public static class AudioStreamSpawner_Patcher
    {
        [HarmonyPatch("Pressed")]
        [HarmonyPrefix]
        public static bool Pressed_Patch(AudioStreamSpawner __instance, Sync<string> ____bitrateString, IButton button, ButtonEventData eventData)
        {
            Msg("----------Opening StreamBitrateSelector Modal----------");
            // We create a ValueMultiplexer with a list of common audio stream bitrates.
            // The list is derived from OBS-Studio's list of audio stream bitrate options.
            // We'll then use the slider to change the index of the ValueMultiplexer.
            // The selected index will determine which value "BitrateKbps" is being driven to.
            ValueMultiplexer<float> bitrateValueMultiplexer = __instance.Slot.AttachComponent<ValueMultiplexer<float>>();
            bitrateValueMultiplexer.Values.Add(32f);
            bitrateValueMultiplexer.Values.Add(64f);
            bitrateValueMultiplexer.Values.Add(96f);
            bitrateValueMultiplexer.Values.Add(128f);
            bitrateValueMultiplexer.Values.Add(160f);
            bitrateValueMultiplexer.Values.Add(192f);
            bitrateValueMultiplexer.Values.Add(224f);
            bitrateValueMultiplexer.Values.Add(256f);
            bitrateValueMultiplexer.Values.Add(288f);
            bitrateValueMultiplexer.Values.Add(320f);
            bitrateValueMultiplexer.Target.Target = __instance.BitrateKbps;

            // ConvertableIntDriver will take our slider's float value and turn it into an int to drive our ValueMultiplexer's Index.
            ConvertibleIntDriver<float> sliderConvertibleIntDriver = __instance.Slot.AttachComponent<ConvertibleIntDriver<float>>();
            //ConvertibleFloatDriver<int> indexConverttibleFloatDriver = __instance.Slot.AttachComponent<ConvertibleFloatDriver<int>>();
            //ValueField<float> sliderDriveWriteBack = __instance.Slot.AttachComponent<ValueField<float>>();

            UIBuilder uIBuilder = new UIBuilder(__instance.Slot.OpenModalOverlay(new float2(0.8f, 0.9f), "Tools.StreamAudio.Dialog.Title".AsLocaleKey()));
            RadiantUI_Constants.SetupDefaultStyle(uIBuilder);
            uIBuilder.SplitVertically(0.875f, out var top, out var bottom);
            AudioInputDeviceSelection audioInputDeviceSelection = top.Slot.AttachComponent<AudioInputDeviceSelection>();
            audioInputDeviceSelection.UseFilteredData.Value = false;
            audioInputDeviceSelection.SelectedDeviceIndex.Value = __instance.InputInterface.FindAudioInputIndex(__instance.DeviceName.Value, caseSensitive: false, allowPartialMatch: false);
            __instance.DeviceName.DriveFrom(audioInputDeviceSelection.SelectedDeviceName);
            uIBuilder.NestInto(bottom);
            uIBuilder.SplitHorizontally(0.7f, out var left, out var right);
            uIBuilder.Style.TextAutoSizeMax *= 1.5f;
            uIBuilder.Style.ButtonTextPadding *= 2f;
            uIBuilder.NestInto(left);
            uIBuilder.SplitHorizontally(0.333f, out var left2, out var right2);
            uIBuilder.ForceNext = left2;
            LocaleString text = "Tools.StreamAudio.Bitrate".AsLocaleKey("bitrate", ____bitrateString);
            uIBuilder.Text(in text).RectTransform.AddFixedPadding(8f);
            uIBuilder.NestInto(right2);

            // Setup a slider with the "Integer" field set to true.
            // Ensures only whole integer values come out of the slider.
            // We can't use Slider<int> as clamping prevents us from sliding to the maximum value.
            Slider<float> slider = uIBuilder.Slider(64f, 0f, 0, 9, true);

            // Assign our Source and Target for ConvertableIntDriver to drive ValueMultiplexer's Index
            sliderConvertibleIntDriver.Source.Target = slider.Value;
            sliderConvertibleIntDriver.Target.Target = bitrateValueMultiplexer.Index;

            slider.RectTransform.AddFixedPadding(8f);
            //slider.Value.DriveFrom(sliderDriveWriteBack.Value, writeBack: true);
            uIBuilder.NestOut();
            uIBuilder.ForceNext = right;
            uIBuilder.Button(OfficialAssets.Graphics.Icons.Voice.Broadcast, "Tools.StreamAudio.Start".AsLocaleKey(), __instance.OnStartStreaming).RectTransform.AddFixedPadding(8f);

            return false;
        }
    }

}
