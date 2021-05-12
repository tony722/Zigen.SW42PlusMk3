using System;
using AET.Unity.RestClient;
using AET.Unity.SimplSharp;
using Crestron.SimplSharp.Net.Http;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace AET.Zigen.Sw42PlusV3.ApiObjects {
  public class AudioSettings : Sw42PlusV3Object {
    private JObject json;

    public AudioSettings(Sw42Plus sw42PlusV3) : this() {
      Sw42PlusV3 = sw42PlusV3;
    }

    public AudioSettings() : base("/SetAudioSettings", "/GetAudioSettings") { }

    internal void Initialize() {
      AddEmptyDelegatesToSplusOutputs();
      Band115 = new EqSetting(this, "band0", (v) => Sw42PlusV3.SetBand115F(v), (v) => Sw42PlusV3.SetBand115Text(v));
      Band330 = new EqSetting(this, "band1", (v) => Sw42PlusV3.SetBand330F(v), (v) => Sw42PlusV3.SetBand330Text(v));
      Band990 = new EqSetting(this, "band2", (v) => Sw42PlusV3.SetBand990F(v), (v) => Sw42PlusV3.SetBand990Text(v));
      Band3000 = new EqSetting(this, "band3", (v) => Sw42PlusV3.SetBand3000F(v), (v) => Sw42PlusV3.SetBand3000Text(v));
      Band9900 = new EqSetting(this, "band4", (v) => Sw42PlusV3.SetBand9900F(v), (v) => Sw42PlusV3.SetBand9900Text(v));
      Treble = new EqSetting(this, "treble", (v) => Sw42PlusV3.SetTrebleF(v), (v) => Sw42PlusV3.SetTrebleText(v));
      Bass = new EqSetting(this, "basstone", (v) => Sw42PlusV3.SetBassF(v), (v) => Sw42PlusV3.SetBassText(v));
    }

    public ushort Output { get; set; }

    #region Mute

    private ushort mute;
    public ushort Mute {
      get { return mute; }
      set {
        if (mute == value) return;
        PostString("mute", value.ToBool());
        MuteF = value;
      }
    }

    public ushort MuteF {
      set {
        mute = value;
        ShowFeedback(value, SetMuteF, Sw42PlusV3.SetMuteF);
      }
    }

    public void MuteToggle() { Mute = (ushort)(Mute == 0 ? 1 : 0); }

    #endregion

    #region Volume

    private ushort volume, volumeScaled;
    public ushort Volume {
      get { return volume; }
      set {
        var valueScaled = value.ConvertFrom16Bit(100);
        if (volumeScaled == valueScaled) return;
        Post("volume", valueScaled);
        UpdateVolumeF(value, valueScaled);
      }
    }

    public void UpdateVolumeF(ushort value, ushort valueScaled) {
      volume = value;
      volumeScaled = valueScaled;
      ShowFeedback(value, SetVolumeF, Sw42PlusV3.SetVolumeF);
    }

    #endregion

    #region TuneMode

    private string tuneMode;
    internal string TuneMode {
      get { return tuneMode; }
      set {
        if (tuneMode == value) return;
        Post("tune mode", value);
        TuneModeF = value;
      }
    }

    internal string TuneModeF {
      set {
        tuneMode = value;
        ShowFeedback(value == "disabled", SetTuneModeDisabledF, Sw42PlusV3.SetTuneModeDisabledF);
        ShowFeedback(value == "presets", SetTuneModePresetsF, Sw42PlusV3.SetTuneModePresetsF);
        ShowFeedback(value == "equalizer", SetTuneModeEqualizerF, Sw42PlusV3.SetTuneModeEqualizerF);
        ShowFeedback(value == "tonecontrol", SetTuneModeToneControlF, Sw42PlusV3.SetTuneModeToneControlF);
      }
    }

    public void TuneModeDisabled() { TuneMode = "disabled"; }
    public void TuneModePresets() { TuneMode = "presets"; }
    public void TuneModeEqualizer() { TuneMode = "equalizer"; }
    public void TuneModeToneControl() { TuneMode = "tonecontrol"; }
    #endregion

    #region Preset

    private string preset;
    internal string Preset {
      get { return preset; }
      set {
        if (preset == value) return;
        Post("preset", value);
        PresetF = value;
      }
    }

    internal string PresetF {
      set {
        preset = value;
        ShowFeedback(value == "flat", SetPresetFlatF, Sw42PlusV3.SetPresetFlatF);
        ShowFeedback(value == "rock", SetPresetRockF, Sw42PlusV3.SetPresetRockF);
        ShowFeedback(value == "classical", SetPresetClassicalF, Sw42PlusV3.SetPresetClassicalF);
        ShowFeedback(value == "dance", SetPresetDanceF, Sw42PlusV3.SetPresetDanceF);
        ShowFeedback(value == "acoustic", SetPresetAcousticF, Sw42PlusV3.SetPresetAcousticF);
      }
    }

    public void PresetFlat() { Preset = "flat"; }
    public void PresetRock() { Preset = "rock"; }
    public void PresetClassical() { Preset = "classical"; }
    public void PresetDance() { Preset = "dance"; }
    public void PresetAcoustic() { Preset = "acoustic"; }
    #endregion

    #region EQ Bands
    public EqSetting Band115 { get; private set; }
    public EqSetting Band330 { get; private set; }
    public EqSetting Band990 { get; private set; }
    public EqSetting Band3000 { get; private set; }
    public EqSetting Band9900 { get; private set; }
    public EqSetting Bass { get; private set; }
    public EqSetting Treble { get; private set; }
    #endregion

    #region Surround

    private ushort surround;
    public ushort Surround {
      get { return surround; }
      set {
        if (surround == value) return;
        PostString("surround", value.ToBool());
        SurroundF = value;
      }
    }

    public ushort SurroundF {
      set {
        surround = value;
        ShowFeedback(value, SetSurroundF, Sw42PlusV3.SetSurroundF);
      }
    }

    public void SurroundToggle() { Surround = (ushort)(Surround == 0 ? 1 : 0); }

    private ushort surroundLevel, surroundLevelScaled;
    public ushort SurroundLevel {
      get { return surroundLevel; }
      set {
        var valueScaled = value.ConvertFrom16Bit(7);
        if (surroundLevelScaled == valueScaled) return;
        Post("surrlevel", valueScaled);
        UpdateSurroundLevelF(value, valueScaled);
      }
    }

    public void UpdateSurroundLevelF(ushort value, ushort valueScaled) {
      surroundLevel = value;
      surroundLevelScaled = valueScaled;
      ShowFeedback(value, SetSurroundLevelF, Sw42PlusV3.SetSurroundLevelF);
    }
    #endregion

    #region BassEnhancement

    private ushort bassEnhancement;
    public ushort BassEnhancement {
      get { return bassEnhancement; }
      set {
        if (bassEnhancement == value) return;
        PostString("bass", value.ToBool());
        BassEnhancementF = value;
      }
    }

    public ushort BassEnhancementF {
      set {
        bassEnhancement = value;
        ShowFeedback(value, SetBassEnhancementF, Sw42PlusV3.SetBassEnhancementF);
      }
    }

    public void BassEnhancementToggle() { BassEnhancement = (ushort)(BassEnhancement == 0 ? 1 : 0); }

    private ushort bassLevel, bassLevelScaled;
    public ushort BassLevel {
      get { return bassLevel; }
      set {
        var valueScaled = value.ConvertFrom16Bit(127);
        if (bassLevelScaled == valueScaled) return;
        Post("basslevel", valueScaled);
        UpdateBassLevelF(value, valueScaled);
      }
    }

    public void UpdateBassLevelF(ushort value, ushort valueScaled) {
      bassLevel = value;
      bassLevelScaled = valueScaled;
      ShowFeedback(value, SetBassLevelF, Sw42PlusV3.SetBassLevelF);
    }

    private ushort bassCutoff;
    internal ushort BassCutoff {
      get { return bassCutoff; }
      set {
        if (bassCutoff == value) return;
        Post("bassfreq", value);
        BassCutoffF = value;
      }
    }

    internal ushort BassCutoffF {
      set {
        bassCutoff = value;
        ShowFeedback(value == 80, SetBassCutFreq80F, Sw42PlusV3.SetBassCutFreq80F);
        ShowFeedback(value == 100, SetBassCutFreq100F, Sw42PlusV3.SetBassCutFreq100F);
        ShowFeedback(value == 125, SetBassCutFreq125F, Sw42PlusV3.SetBassCutFreq125F);
        ShowFeedback(value == 150, SetBassCutFreq150F, Sw42PlusV3.SetBassCutFreq150F);
        ShowFeedback(value == 175, SetBassCutFreq175F, Sw42PlusV3.SetBassCutFreq175F);
        ShowFeedback(value == 200, SetBassCutFreq200F, Sw42PlusV3.SetBassCutFreq200F);
        ShowFeedback(value == 225, SetBassCutFreq225F, Sw42PlusV3.SetBassCutFreq225F);
      }
    }

    public void BassCutFreq80() { BassCutoff = 80; }
    public void BassCutFreq100() { BassCutoff = 100; }
    public void BassCutFreq125() { BassCutoff = 125; }
    public void BassCutFreq150() { BassCutoff = 150; }
    public void BassCutFreq175() { BassCutoff = 175; }
    public void BassCutFreq200() { BassCutoff = 200; }
    public void BassCutFreq225() { BassCutoff = 225; }


    private ushort highPass;
    public ushort HighPass {
      get { return highPass; }
      set {
        if (highPass == value) return;
        PostString("highpass", value.ToBool());
        HighPassF = value;
      }
    }

    public ushort HighPassF {
      set {
        highPass = value;
        ShowFeedback(value, SetHighPassF, Sw42PlusV3.SetHighPassF);
      }
    }

    public void HighPassToggle() { HighPass = (ushort)(HighPass == 0 ? 1 : 0); }
    #endregion

    public void Poll() {
      var postContents = string.Format(@"{{""output"":{0}}}", Output - 1);
      var response = Sw42PlusV3.HttpPost(GetUrl, postContents);
      try {
        json = JObject.Parse(response);
        json = json["audioInfo"] as JObject;
        FillFromJsonObject();
      } catch (Exception ex) {
        ErrorMessage.Error("Sw42PlusV3.AudioSettings.Poll: Error handling Poll() response: {0}", ex.Message);
      }
    }

    private void FillFromJsonObject() {
      MuteF = BoolFromJson("mute");
      ScaledFromJson("volume", 100, UpdateVolumeF);
      TuneModeF = json["tune mode"].Value<string>();
      PresetF = json["presets"].Value<string>();
      Band115.UpdateFeedback(json);
      Band330.UpdateFeedback(json);
      Band990.UpdateFeedback(json);
      Band3000.UpdateFeedback(json);
      Band9900.UpdateFeedback(json);
      Bass.UpdateFeedback(json);
      Treble.UpdateFeedback(json);
      SurroundF = BoolFromJson("surround");
      ScaledFromJson("surrlevel", 7, UpdateSurroundLevelF);
      BassEnhancementF = BoolFromJson("bass");
      ScaledFromJson("basslevel", 127, UpdateBassLevelF);
      BassCutoffF = json["bassfreq"].Value<ushort>();
      HighPassF = BoolFromJson("highpass");
    }

    #region Conversion Routines
    private short ConvertEqTo16Bit(double? nullableValue) {
      double value = nullableValue ?? 0;
      return (short)(value * 10);
    }
    private double ConvertEqFrom16Bit(short value) {
      double o = value;
      o /= 10;
      o = Math.Round(o * 4) / 4;
      return o;
    }

    private void EqFromJson(string key, Action<short, double> updateFeedback) {
      var valueScaled = json[key].Value<double>();
      var value = ConvertEqTo16Bit(valueScaled);
      updateFeedback(value, valueScaled);
    }

    private void ScaledFromJson(string key, int scale, Action<ushort, ushort> updateFeedback) {
      var valueScaled = json[key].Value<int>();
      var value = valueScaled.ConvertTo16Bit(scale);
      updateFeedback(value, (ushort)valueScaled);
    }

    private ushort BoolFromJson(string key) {
      return (ushort)(json[key].Value<bool>() ? 1 : 0);
    }
    #endregion

    #region Json Post Builders
    internal void Post(string key, string value) {
      if (value == null) PostObject(key, "null");
      PostFormatted(@"{{""output"":{0},""{1}"":""{2}""}}", Output - 1, key, value);
    }

    internal void Post(string key, ushort value) {
      PostObject(key, value);
    }

    internal void Post(string key, double value) {
      PostObject(key, value);
    }

    internal void PostString(string key, bool value) {
      PostObject(key, value.ToString().ToLower());
    }

    private void PostObject(string key, object value) {
      PostFormatted(@"{{""output"":{0},""{1}"":{2}}}", Output - 1, key, value);
    }

    #endregion

    #region Feedback routines
    private void ShowFeedback(ushort value, SetUshortOutputArrayDelegate localDelegate, SetUshortOutputDelegate sw42Delegate) {
      localDelegate(Output, value);
      if (Output == Sw42PlusV3.SelectedAudioSettings) sw42Delegate(value);
    }

    private void ShowFeedback(bool value, SetUshortOutputArrayDelegate localDelegate, SetUshortOutputDelegate sw42Delegate) {
      ShowFeedback(value.ToUshort(), localDelegate, sw42Delegate);
    }
    #endregion

    #region SPlus Feedback Delegates
    public void AddEmptyDelegatesToSplusOutputs() {
      SetMuteF = delegate { };
      SetVolumeF = delegate { };
      SetTuneModeDisabledF = delegate { };
      SetTuneModePresetsF = delegate { };
      SetTuneModeEqualizerF = delegate { };
      SetTuneModeToneControlF = delegate { };
      SetPresetFlatF = delegate { };
      SetPresetRockF = delegate { };
      SetPresetClassicalF = delegate { };
      SetPresetDanceF = delegate { };
      SetPresetAcousticF = delegate { };
      SetSurroundF = delegate { };
      SetSurroundLevelF = delegate { };
      SetBassEnhancementF = delegate { };
      SetBassLevelF = delegate { };
      SetBassCutFreq80F = delegate { };
      SetBassCutFreq100F = delegate { };
      SetBassCutFreq125F = delegate { };
      SetBassCutFreq150F = delegate { };
      SetBassCutFreq175F = delegate { };
      SetBassCutFreq200F = delegate { };
      SetBassCutFreq225F = delegate { };
      SetHighPassF = delegate { };
      SetMuteF = delegate { };
      SetVolumeF = delegate { };
    }
    public SetUshortOutputArrayDelegate SetTuneModeDisabledF { get; set; }
    public SetUshortOutputArrayDelegate SetTuneModePresetsF { get; set; }
    public SetUshortOutputArrayDelegate SetTuneModeEqualizerF { get; set; }
    public SetUshortOutputArrayDelegate SetTuneModeToneControlF { get; set; }
    public SetUshortOutputArrayDelegate SetPresetFlatF { get; set; }
    public SetUshortOutputArrayDelegate SetPresetRockF { get; set; }
    public SetUshortOutputArrayDelegate SetPresetClassicalF { get; set; }
    public SetUshortOutputArrayDelegate SetPresetDanceF { get; set; }
    public SetUshortOutputArrayDelegate SetPresetAcousticF { get; set; }
    public SetUshortOutputArrayDelegate SetSurroundF { get; set; }
    public SetUshortOutputArrayDelegate SetSurroundLevelF { get; set; }
    public SetUshortOutputArrayDelegate SetBassEnhancementF { get; set; }
    public SetUshortOutputArrayDelegate SetBassLevelF { get; set; }
    public SetUshortOutputArrayDelegate SetBassCutFreq80F { get; set; }
    public SetUshortOutputArrayDelegate SetBassCutFreq100F { get; set; }
    public SetUshortOutputArrayDelegate SetBassCutFreq125F { get; set; }
    public SetUshortOutputArrayDelegate SetBassCutFreq150F { get; set; }
    public SetUshortOutputArrayDelegate SetBassCutFreq175F { get; set; }
    public SetUshortOutputArrayDelegate SetBassCutFreq200F { get; set; }
    public SetUshortOutputArrayDelegate SetBassCutFreq225F { get; set; }
    public SetUshortOutputArrayDelegate SetHighPassF { get; set; }
    public SetUshortOutputArrayDelegate SetMuteF { get; set; }
    public SetUshortOutputArrayDelegate SetVolumeF { get; set; }
    #endregion
  }
}
