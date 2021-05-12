using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AET.Unity.RestClient;
using AET.Unity.SimplSharp;
using AET.Unity.SimplSharp.HttpClient;
using AET.Zigen.Sw42PlusV3.ApiObjects;

namespace AET.Zigen.Sw42PlusV3 {
  public delegate void SetAudioSettingsDelegate(AudioSettings[] audioSettingsArray);

  public class Sw42Plus : RestClient {
    private ushort selectedAudioSettings;

    public Sw42Plus() : base(new CrestronHttpClient(5)) {
    }

    public Sw42Plus(IHttpClient httpClient) : base(httpClient) {
      AddEmptyDelegatesToSplusOutputs();
    }

    public void Initialize() {
      VideoMatrix = new VideoMatrix { Sw42PlusV3 = this };
      AllAudioSettings = InitAudioSettingsArray(2);
    }

    private AudioSettings[] InitAudioSettingsArray(int size) {
      var audioSettings = new AudioSettings[size];
      for (ushort i = 0; i < size; i++) {
        audioSettings[i] = new AudioSettings(this) { Output = (ushort)(i + 1)};
        audioSettings[i].Initialize();
      }
      return audioSettings;
    }

    public void SwitchVideoInputToOutput(ushort input, ushort output) {
      VideoMatrix.SwitchInputToOutput(input, output);
      SetVideoOutF(output, input);
    }

    public ushort Debug {
      set { base.HttpClient.Debug = value; }
    }

    public VideoMatrix VideoMatrix { get; private set; }

    public AudioSettings CurrentAudioSettings { get; private set; }
    internal AudioSettings[] AllAudioSettings;

    public ushort SelectedAudioSettings {
      get { return selectedAudioSettings; }
      set {
        selectedAudioSettings = value;
        CurrentAudioSettings = AllAudioSettings[value - 1];
        CurrentAudioSettings.Poll();
      }
    }

    public void GetAudioSettingsArray() {
      SetAudioSettingsArray(AllAudioSettings);
    }

    public void Poll() {
      VideoMatrix.Poll();
      foreach (var audioSetting in AllAudioSettings) audioSetting.Poll();
    }

    #region Splus Feedback Delegates
    public void AddEmptyDelegatesToSplusOutputs() {
      SetAudioSettingsArray = delegate { };
      
      SetAudioOutF = delegate { };
      SetVideoOutF = delegate { };

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
      SetBand115F = delegate { };
      SetBand330F = delegate { };
      SetBand990F = delegate { };
      SetBand3000F = delegate { };
      SetBand9900F = delegate { };
      SetBand115Text = delegate { };
      SetBand330Text = delegate { };
      SetBand990Text = delegate { };
      SetBand3000Text = delegate { };
      SetBand9900Text = delegate { };
      SetBassF = delegate { };
      SetTrebleF = delegate { };
      SetBassText = delegate { };
      SetTrebleText = delegate { };
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

    public SetUshortOutputArrayDelegate SetAudioOutF { get; set; }
    public SetUshortOutputArrayDelegate SetVideoOutF { get; set; }
    public SetAudioSettingsDelegate SetAudioSettingsArray { get; set; }
    
    public SetUshortOutputDelegate SetTuneModeDisabledF { get; set; }
    public SetUshortOutputDelegate SetTuneModePresetsF { get; set; }
    public SetUshortOutputDelegate SetTuneModeEqualizerF { get; set; }
    public SetUshortOutputDelegate SetTuneModeToneControlF { get; set; }
    public SetUshortOutputDelegate SetPresetFlatF { get; set; }
    public SetUshortOutputDelegate SetPresetRockF { get; set; }
    public SetUshortOutputDelegate SetPresetClassicalF { get; set; }
    public SetUshortOutputDelegate SetPresetDanceF { get; set; }
    public SetUshortOutputDelegate SetPresetAcousticF { get; set; }
    public SetShortOutputDelegate SetBand115F { get; set; }
    public SetShortOutputDelegate SetBand330F { get; set; }
    public SetShortOutputDelegate SetBand990F { get; set; }
    public SetShortOutputDelegate SetBand3000F { get; set; }
    public SetShortOutputDelegate SetBand9900F { get; set; }
    public SetShortOutputDelegate SetBassF { get; set; }
    public SetShortOutputDelegate SetTrebleF { get; set; }
    public SetStringOutputDelegate SetBand115Text { get; set; }
    public SetStringOutputDelegate SetBand330Text { get; set; }
    public SetStringOutputDelegate SetBand990Text { get; set; }
    public SetStringOutputDelegate SetBand3000Text { get; set; }
    public SetStringOutputDelegate SetBand9900Text { get; set; }
    public SetStringOutputDelegate SetBassText { get; set; }
    public SetStringOutputDelegate SetTrebleText { get; set; }
    public SetUshortOutputDelegate SetSurroundF { get; set; }
    public SetUshortOutputDelegate SetSurroundLevelF { get; set; }
    public SetUshortOutputDelegate SetBassEnhancementF { get; set; }
    public SetUshortOutputDelegate SetBassLevelF { get; set; }
    public SetUshortOutputDelegate SetBassCutFreq80F { get; set; }
    public SetUshortOutputDelegate SetBassCutFreq100F { get; set; }
    public SetUshortOutputDelegate SetBassCutFreq125F { get; set; }
    public SetUshortOutputDelegate SetBassCutFreq150F { get; set; }
    public SetUshortOutputDelegate SetBassCutFreq175F { get; set; }
    public SetUshortOutputDelegate SetBassCutFreq200F { get; set; }
    public SetUshortOutputDelegate SetBassCutFreq225F { get; set; }
    public SetUshortOutputDelegate SetHighPassF { get; set; }
    public SetUshortOutputDelegate SetMuteF { get; set; }
    public SetUshortOutputDelegate SetVolumeF { get; set; }
    #endregion

  }
}
