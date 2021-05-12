using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using AET.Unity.RestClient;
using Crestron.SimplSharp.Net.Http;
using AET.Unity.SimplSharp;

namespace AET.Zigen.Sw42PlusV3.ApiObjects {
  public abstract class Sw42PlusV3Object {
    protected Sw42PlusV3Object (string setUrl, string getUrl) {
      SetUrl = setUrl;
      GetUrl = getUrl;
    }

    protected string GetUrl { get; private set; }
    protected string SetUrl { get; private set; }

    public Sw42Plus Sw42PlusV3 { get; set; }

    internal string Post(string contents) {
      return Sw42PlusV3.HttpPost(SetUrl, contents);
    }

    internal string PostFormatted(string contents, params object[] args) {
      var postContents = string.Format(contents, args);
      return Post(postContents);
    }
  }
}
