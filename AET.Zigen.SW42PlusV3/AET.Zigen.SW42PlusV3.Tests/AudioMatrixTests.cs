using System;
using System.CodeDom;
using System.ComponentModel.Design.Serialization;
using AET.Unity.SimplSharp;
using AET.Unity.SimplSharp.HttpClient;
using AET.Zigen.Sw42PlusV3.ApiObjects;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AET.Zigen.Sw42PlusV3.Tests {
  [TestClass]
  public class AudioMatrixTests {
    private AudioMatrix api;
    private Sw42PlusV3 sw42;

    [TestInitialize]
    public void TestInit() {
      ErrorMessage.Clear();
      TestHttpClient.Clear();

      sw42 = Test.Sw42PlusV3;
      api = sw42.AudioMatrix;
    }

    [DataTestMethod]
    [DataRow(1,1)]
    [DataRow(2,1)]
    [DataRow(3, 1)]
    [DataRow(4, 1)]
    [DataRow(1, 2)]
    [DataRow(2, 2)]
    [DataRow(3, 2)]
    [DataRow(4, 2)]
    public void Matrix_SetValues_GetsSameValues(int input, int output) {
      api.SwitchInputToOutput(input, output);
      ErrorMessage.LastErrorMessage.Should().BeNullOrEmpty();
      TestHttpClient.RequestContents.Should().Be($"{{\"switch\":{{\"input\":{input-1},\"output\":{output-1}}}}}");
    }
    
    [TestMethod]
    public void SwitchInputToOutput_OutputOutOfBounds_GeneratesError() {
      sw42.SwitchAudioInputToOutput(1, 0);
      ErrorMessage.LastErrorMessage.Should().Be("Sw42PlusV3.AudioMatrix.Output(0): Must be between 1 and 8");
      api.SwitchInputToOutput(1, 9);
      ErrorMessage.LastErrorMessage.Should().Be("Sw42PlusV3.AudioMatrix.Output(9): Must be between 1 and 8");
    }


    [TestMethod]
    public void SwitchInputToOutput_InputOutOfBounds_GeneratesError() {
      api.SwitchInputToOutput(0, 1);
      ErrorMessage.LastErrorMessage.Should().Be("Sw42PlusV3.AudioMatrix.Input(0): Must be between 1 and 4");
      api.SwitchInputToOutput(5, 1);
      ErrorMessage.LastErrorMessage.Should().Be("Sw42PlusV3.AudioMatrix.Input(5): Must be between 1 and 4");
    }

    [TestMethod]
    public void SwitchInputToOutput_ValuesUnchanged_Resends() {
      api.SwitchInputToOutput(2, 2);
      TestHttpClient.Url.Should().Be("http://test/SetAudioMatrix");
      TestHttpClient.RequestContents.Should().Be(@"{""switch"":{""input"":1,""output"":1}}");
      TestHttpClient.RequestContents = null;
      api.SwitchInputToOutput(2, 2);
      TestHttpClient.RequestContents.Should().Be(@"{""switch"":{""input"":1,""output"":1}}");
    }

    [TestMethod]
    public void Poll_PopulatesAudioMatrixCorrectly() {
      var outputs = new int[2];
      sw42.SetAudioOutF = (index, value) => outputs[index - 1] = value;
      TestHttpClient.ResponseContents = @"{""matrix"": [1,0]}";
      api.Poll();
      TestHttpClient.Url.Should().Be("http://test/GetAudioSettings");
      outputs.Should().BeEquivalentTo(new int[] { 2, 1 });
    }

    [TestMethod]
    public void SwitchInputToOutput_SplusDelegateIsTriggered() {
      int[] outputs = new int[8];
      sw42.SetAudioOutF = (idx, value) => outputs[idx] = value;
      sw42.SwitchAudioInputToOutput(4,5);
      outputs[5].Should().Be(4);
    }
  }
}