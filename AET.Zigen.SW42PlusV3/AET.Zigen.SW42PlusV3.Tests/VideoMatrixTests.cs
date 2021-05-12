using System;
using AET.Unity.SimplSharp;
using AET.Unity.SimplSharp.HttpClient;
using AET.Zigen.Sw42PlusV3.ApiObjects;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AET.Zigen.Sw42PlusV3.Tests {
  [TestClass]
  public class VideoMatrixTests {
    
    private VideoMatrix api;
    private Sw42Plus sw42;

    [TestInitialize]
    public void TestInit() {
      ErrorMessage.Clear();
      TestHttpClient.Clear();

      sw42 = Test.Sw42Plus;
      api = sw42.VideoMatrix;
    }

    [TestMethod]
    public void SwitchInputToOutput_OutputOutOfBounds_GeneratesError() {
      api.SwitchInputToOutput(1, 0);
      ErrorMessage.LastErrorMessage.Should().Be("Sw42PlusV3.VideoMatrix.Output(0): Must be between 1 and 2");
      api.SwitchInputToOutput(1, 9);
      ErrorMessage.LastErrorMessage.Should().Be("Sw42PlusV3.VideoMatrix.Output(9): Must be between 1 and 2");
    }


    [TestMethod]
    public void SwitchInputToOutput_InputOutOfBounds_GeneratesError() {
      api.SwitchInputToOutput(0, 1);
      ErrorMessage.LastErrorMessage.Should().Be("Sw42PlusV3.VideoMatrix.Input(0): Must be between 1 and 4");
      api.SwitchInputToOutput(9, 1);
      ErrorMessage.LastErrorMessage.Should().Be("Sw42PlusV3.VideoMatrix.Input(9): Must be between 1 and 4");
    }


    
    [TestMethod]
    public void Execute_GeneratesValidHttpRequest() {
      api.SwitchInputToOutput(2,2);
      TestHttpClient.Url.Should().Be("http://test/SetMatrix");
      TestHttpClient.RequestContents.Should().Be(@"{""switch"":{""input"":1,""output"":1}}");
    }

    [TestMethod]
    public void Poll_PopulatesVideoMatrixCorrectly() {
      var outputs = new int[2];
      sw42.SetVideoOutF = (index, value) => outputs[index - 1] = value;
      TestHttpClient.ResponseContents = @"{""matrix"": [0,1]}";
      api.Poll();
      TestHttpClient.Url.Should().Be("http://test/GetMatrix");
      outputs.Should().BeEquivalentTo(new int[] {1, 2});
    }

    [TestMethod]
    public void SwitchInputToOutput_SplusDelegateIsTriggered() {
      int[] outputs = new int[8];
      sw42.SetVideoOutF = (idx, value) => outputs[idx] = value;
      sw42.SwitchVideoInputToOutput(2, 3);
      outputs[3].Should().Be(2);
    }
  }
}
