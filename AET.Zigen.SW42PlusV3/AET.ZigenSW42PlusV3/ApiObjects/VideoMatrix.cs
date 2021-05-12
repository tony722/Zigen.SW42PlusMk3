using System;
using AET.Unity.RestClient;
using AET.Unity.SimplSharp;
using Newtonsoft.Json.Linq;

namespace AET.Zigen.Sw42PlusV3.ApiObjects {

  public class VideoMatrix : MatrixObject {
    public VideoMatrix() : base("/SetMatrix", "/GetMatrix") { }
    internal override int OutputCount { get { return 2; } }
    internal override int InputCount { get { return 4; } }

    public void Poll() {
      var response = Sw42PlusV3.HttpGet(GetUrl);
      ParseMatrix(response, Sw42PlusV3.SetVideoOutF);
    }
  }
}

