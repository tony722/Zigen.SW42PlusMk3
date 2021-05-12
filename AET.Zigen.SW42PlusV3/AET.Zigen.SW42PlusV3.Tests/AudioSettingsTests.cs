using AET.Unity.SimplSharp;
using AET.Unity.SimplSharp.HttpClient;
using AET.Zigen.Sw42PlusV3.ApiObjects;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AET.Zigen.Sw42PlusV3.Tests {
  [TestClass]
  public class AudioSettingsTests {
    private Sw42Plus sw42;
    private AudioSettings api;

    [TestInitialize]
    public void TestInit() {
      ErrorMessage.Clear();
      TestHttpClient.Clear();
      sw42 = Test.Sw42Plus;
      api = sw42.AllAudioSettings[1];
      sw42.SelectedAudioSettings = 2;
    }

    [TestMethod]
    public void Poll_SendsCorrectJson() {
      var api = Test.Sw42Plus.AllAudioSettings[1];
      api.Output.Should().Be(2);
      api.Poll();
      TestHttpClient.Url.Should().Be("http://test/GetAudioSettings");
      TestHttpClient.RequestContents.Should().Be(@"{""output"":1}");
    }

    #region Toggles
    [TestMethod]
    public void Volume_SendsCorrectJson() {
      Test.Sw42Plus.AllAudioSettings[1].Volume = 65535;
      TestHttpClient.Url.Should().Be("http://test/SetAudioSettings");
      TestHttpClient.RequestContents.Should().Be(@"{""output"":1,""volume"":100}");
    }

    [TestMethod]
    public void Mute_HttpAndFeedback_Correct() { TestToggle("Mute"); }

    [TestMethod]
    public void Surround_HttpAndFeedback_Correct() { TestToggle("Surround"); }

    [TestMethod]
    public void BassEnhancement_HttpAndFeedback_Correct() { TestToggle("BassEnhancement", "bass"); }

    [TestMethod]
    public void HighPass_HttpAndFeedback_Correct() { TestToggle("HighPass"); }
    #endregion

    #region Eqs

    [TestMethod]
    public void Band115_HttpAndFeedback_Correct() { TestEq("Band115", "band0"); }
    [TestMethod]
    public void Band330_HttpAndFeedback_Correct() { TestEq("Band330", "band1"); }
    [TestMethod]
    public void Band990_HttpAndFeedback_Correct() { TestEq("Band990", "band2"); }
    [TestMethod]
    public void Band3000_HttpAndFeedback_Correct() { TestEq("Band3000", "band3"); }
    [TestMethod]
    public void Band9900_HttpAndFeedback_Correct() { TestEq("Band9900", "band4"); }
    [TestMethod]
    public void Treble_HttpAndFeedback_Correct() { TestEq("Treble", "treble"); }
    [TestMethod]
    public void Bass_HttpAndFeedback_Correct() { TestEq("Bass", "basstone"); }
    #endregion

    [TestMethod]
    public void TuneMode_HttpAndFeedback_Correct() {
      TestStrings(new[] { "TuneModeDisabled", "TuneModePresets", "TuneModeEqualizer", "TuneModeToneControl" }, "tune mode", new[] { "disabled", "presets", "equalizer", "tonecontrol" });
    }

    [TestMethod]
    public void Presets_HttpAndFeedback_Correct() {
      TestStrings(new[] { "PresetFlat", "PresetRock", "PresetClassical", "PresetDance", "PresetAcoustic" }, "preset", new[] { "flat", "rock", "classical", "dance", "acoustic" });
    }

    [TestMethod]
    public void BassFreq_HttpAndFeedback_Correct() {
      TestInts(new[] { "BassCutFreq80", "BassCutFreq100", "BassCutFreq125", "BassCutFreq150", "BassCutFreq175", "BassCutFreq200", "BassCutFreq225" }, "bassfreq", new[] { 80, 100, 125, 150, 175, 200, 225 });
    }

    [TestMethod]
    public void Volume_HttpAndFeedback_Correct() {
      TestScaledValue("Volume","volume",65535, 100);
      TestScaledValue("Volume", "volume", 32768, 50);
      TestScaledValue("Volume", "volume", 0, 0);
    }

    [TestMethod]
    public void BassLevel_HttpAndFeedback_Correct() {
      TestScaledValue("BassLevel", "basslevel", 65535, 127);
      TestScaledValue("BassLevel", "basslevel", 0, 0);
    }

    [TestMethod]
    public void SurroundLevel_HttpAndFeedback_Correct() {
      TestScaledValue("SurroundLevel", "surrlevel", 65535, 7);
      TestScaledValue("SurroundLevel", "surrlevel", 0, 0);
    }

    #region Test Helpers
    private void TestScaledValue(string name, string jsonName, ushort output, ushort outputScaled) {
      ushort state = 999;
      ushort PropValue() => api.GetPropertyValue<ushort>(name);
      api.SetPropertyValue<SetUshortOutputArrayDelegate>("Set" + name + "F", (index, value) => state = value);
      api.SetPropertyValue<ushort>(name, output);
      PropValue().Should().Be(output, "because '{0}' was set to {1}", name, output);
      TestHttpClient.RequestContents.Should().Be(String.Format(@"{{""output"":1,""{0}"":{1}}}", jsonName, outputScaled));
    }

    private void TestToggle(string name) {
      TestToggle(name, name.ToLower());
    }

    private void TestToggle(string name, string jsonName) {
      ushort state = 999;
      ushort PropValue() => api.GetPropertyValue<ushort>(name);
      using (new AssertionScope()) {
        api.SetPropertyValue<SetUshortOutputArrayDelegate>("Set" + name + "F", (index, value) => state = value);
        api.SetPropertyValue<ushort>(name, 1);
        PropValue().Should().Be(1, "because '{0}' was set to 1", name);
        TestHttpClient.RequestContents.Should().Be(String.Format(@"{{""output"":1,""{0}"":true}}", jsonName));

        api.InvokeMethod(name + "Toggle");
        PropValue().Should().Be(0, "because '{0}' was toggled from 1 to 0.", name);
        TestHttpClient.RequestContents.Should().Be(String.Format(@"{{""output"":1,""{0}"":false}}", jsonName));
      }
    }

    private void TestEq(string name, string jsonName) {
      short state = 999, sw42State = 999;
      string text = "", sw42Text = "";
      var prop = api.GetPropertyValue<EqSetting>(name);
      prop.FeedbackDelegate = (index, value) => state = value;
      prop.TextFeedbackDelegate = (index, value) => text = value.ToString();
      sw42.SetPropertyValue<SetShortOutputDelegate>("Set" + name + "F", v => sw42State = v);
      sw42.SetPropertyValue<SetStringOutputDelegate>("Set" + name + "Text", v => sw42Text = v.ToString());

      using (new AssertionScope()) {
        prop.Value = 30;
        prop.Value.Should().Be(30, "because '{0}' was set to 30", name);
        state.Should().Be(30);
        text.Should().Be("3");
        sw42State.Should().Be(state);
        sw42Text.Should().Be(text);
        TestHttpClient.RequestContents.Should().Be(String.Format(@"{{""output"":1,""{0}"":3}}", jsonName));

        prop.Value = -60;
        prop.Value.Should().Be(-60, "because '{0}' was set to -60", name);
        state.Should().Be(-60);
        text.Should().Be("-6");
        sw42State.Should().Be(state);
        sw42Text.Should().Be(text);
        TestHttpClient.RequestContents.Should().Be(String.Format(@"{{""output"":1,""{0}"":-6}}", jsonName));
      }
    }

    private void TestString(string name, string jsonName, string jsonValue) {
      ushort state = 999;
      api.SetPropertyValue<SetUshortOutputArrayDelegate>("Set" + name + "F", (index, value) => state = value);
      api.InvokeMethod(name);
      state.Should().Be(1);
      TestHttpClient.RequestContents.Should().Be(string.Format(@"{{""output"":1,""{0}"":""{1}""}}", jsonName, jsonValue));
    }

    private void TestInts(string[] names, string jsonName, int[] jsonValues) {
      TestValues(names, jsonName, jsonValues.Select(a => (object)a).ToArray(), @"{{""output"":1,""{0}"":{1}}}");
    }
    private void TestStrings(string[] names, string jsonName, string[] jsonValues) {
      TestValues(names, jsonName, jsonValues, @"{{""output"":1,""{0}"":""{1}""}}");
    }
    private void TestValues(string[] names, string jsonName, object[] jsonValues, string jsonOutputFormat) {
      ushort[] states = names.Select(c => (ushort)999).ToArray();
      for (var a = 0; a < names.Length; a++) {
        var pos = a;
        api.SetPropertyValue<SetUshortOutputArrayDelegate>("Set" + names[a] + "F", (index, value) => { states[pos] = value; });
      }

      for (var i = 0; i < names.Length; i++) {
        api.InvokeMethod(names[i]);
        for (var j = 0; j < names.Length; j++) states[j].Should().Be((i == j).ToUshort());
        TestHttpClient.RequestContents.Should().Be(string.Format(jsonOutputFormat, jsonName, jsonValues[i]));
      }
    }

    #endregion


    [TestMethod]
    public void Deserialize_ValidData_ReturnsCorrectlyPopulatedObject() {
      var responseString =
        @"{""matrix"":[3,2,1,0,3,2,1,0],""status"":""success"",""audioInfo"":{""audiosel"":""local"",""mute"":true,""volume"":50,""tune mode"":""presets"",""presets"":""flat"",""band0"":5,""band1"":6,""band2"":7,""band3"":8,""band4"":9,""basstone"":10,""treble"":11,""surround"":true,""surrlevel"":1,""basslevel"":31,""bass"":true,""bassfreq"":100,""highpass"":true}}";
      TestHttpClient.ResponseContents = responseString;
      api.Poll();
      using (new AssertionScope()) {
        api.Volume.Should().Be(32767);
        api.Mute.Should().Be(1);
        api.TuneMode.Should().Be("presets");
        api.Preset.Should().Be("flat");
        api.Band115.Value.Should().Be(50);
        api.Band330.Value.Should().Be(60);
        api.Band990.Value.Should().Be(70);
        api.Band3000.Value.Should().Be(80);
        api.Band9900.Value.Should().Be(90);
        api.Bass.Value.Should().Be(100);
        api.Treble.Value.Should().Be(110);
        api.Surround.Should().Be(1);
        api.SurroundLevel.Should().Be(9362);
        api.BassEnhancement.Should().Be(1);
        api.BassCutoff.Should().Be(100);
        api.BassLevel.Should().Be(15996);
        api.HighPass.Should().Be(1);
      }
    }
  }
}
