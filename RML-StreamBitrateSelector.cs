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
            Slider<int> slider = uIBuilder.Slider<int>(64f, bitrateValueMultiplexer.Index, 0, 10);
            slider.RectTransform.AddFixedPadding(8f);
            slider.Value.DriveFrom(bitrateValueMultiplexer.Index, writeBack: true);
            bitrateValueMultiplexer.Index.Value = 3;
            uIBuilder.NestOut();
            uIBuilder.ForceNext = right;
            uIBuilder.Button(OfficialAssets.Graphics.Icons.Voice.Broadcast, "Tools.StreamAudio.Start".AsLocaleKey(), __instance.OnStartStreaming).RectTransform.AddFixedPadding(8f);

            return false;
        }
    }

}
