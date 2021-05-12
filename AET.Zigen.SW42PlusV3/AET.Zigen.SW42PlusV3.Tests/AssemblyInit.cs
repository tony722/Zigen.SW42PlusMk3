using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AET.Unity.RestClient;
using AET.Unity.SimplSharp;
using AET.Unity.SimplSharp.HttpClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AET.Zigen.HxlPlus.Tests {

  [TestClass]
  class AssemblyInit {
    [AssemblyInitialize]
    public static void Init(TestContext _) {
      ErrorMessage.ErrorMessageHandler = new TestErrorMessageHandler();
      Test.HxlPlus.HostName = "http://Test";
      Test.HxlPlus.Initialize();
      //Test.HxlPlus.Timer = Test.Timer;
      //Test.HxlPlus.SpaceBetweenCommands = 0;
      //Test.HxlPlus.Mutex = Test.Mutex;
    }
  }
}
