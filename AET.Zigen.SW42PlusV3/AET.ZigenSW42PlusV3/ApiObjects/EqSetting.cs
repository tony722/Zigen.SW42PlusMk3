using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AET.Unity.SimplSharp;
using Newtonsoft.Json.Linq;

namespace AET.Zigen.Sw42PlusV3.ApiObjects {
  public class EqSetting {
    private short currentValue;
    private double currentValueScaled;

    public EqSetting() {
      FeedbackDelegate = delegate { };
      TextFeedbackDelegate = delegate { };
    }

    public EqSetting(AudioSettings audioSettings, string jsonName, SetShortOutputDelegate sw42FeedbackDelegate, SetStringOutputDelegate sw42TextFeedbackDelegate) : this() {
      AudioSettings = audioSettings;
      JsonName = jsonName;
      Sw42FeedbackDelegate = sw42FeedbackDelegate;
      Sw42TextFeedbackDelegate = sw42TextFeedbackDelegate;
    }

    public AudioSettings AudioSettings { get; set; }
    public string JsonName { get; set; }
    
    public SetShortOutputArrayDelegate FeedbackDelegate { get; set; }
    public SetShortOutputDelegate Sw42FeedbackDelegate { get; set; }

    public SetStringOutputArrayDelegate TextFeedbackDelegate { get; set; }
    public SetStringOutputDelegate Sw42TextFeedbackDelegate { get; set; }
    
    public short Value {
      get { return currentValue; }
      set {
        var valueScaled = ConvertEqFrom16Bit(value);
        if (currentValueScaled == valueScaled) return;
        AudioSettings.Post(JsonName, valueScaled);
        UpdateFeedback(value, valueScaled);
      }
    }

    public void UpdateFeedback(JObject json) {
      var valueScaled = json[JsonName].Value<double>();
      var value = ConvertEqTo16Bit(valueScaled);
      UpdateFeedback(value, valueScaled);
    }

    public void UpdateFeedback(short value, double valueScaled) {
      currentValue = value;
      currentValueScaled = valueScaled;

      FeedbackDelegate(AudioSettings.Output, value);
      TextFeedbackDelegate(AudioSettings.Output, valueScaled.ToString());
      if (AudioSettings.Output != AudioSettings.Sw42PlusV3.SelectedAudioSettings) return; 
      Sw42FeedbackDelegate(value);
      Sw42TextFeedbackDelegate(valueScaled.ToString());
    }

    private double ConvertEqFrom16Bit(short value) {
      double o = value;
      o /= 10;
      o = Math.Round(o * 4) / 4;
      return o;
    }
    private short ConvertEqTo16Bit(double? nullableValue) {
      double value = nullableValue ?? 0;
      return (short)(value * 10);
    }
  }
}
