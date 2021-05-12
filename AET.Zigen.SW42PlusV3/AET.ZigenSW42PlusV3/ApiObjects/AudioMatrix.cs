using System;

namespace AET.Zigen.Sw42PlusV3.ApiObjects {
  public class AudioMatrix : MatrixObject {

    public AudioMatrix() : base("/SetAudioMatrix", "/GetAudioSettings") { }

    internal override int OutputCount { get { return 2;  } }
    internal override int InputCount { get { return 4;}}

    public void Poll() {
      var json = @"{""output"":""none""}";
      var response = Sw42PlusV3.HttpPost(GetUrl, json);
      ParseMatrix(response, Sw42PlusV3.SetAudioOutF);
    }
  }
}

