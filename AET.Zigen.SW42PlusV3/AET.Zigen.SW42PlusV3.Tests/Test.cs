using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AET.Unity.SimplSharp;
using AET.Unity.SimplSharp.HttpClient;
using AET.Unity.SimplSharp.Timer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AET.Zigen.Sw42PlusV3.Tests {
  static class Test {
    static Test() {
      ErrorMessage.ErrorMessageHandler = new TestErrorMessageHandler();
    }
    
    public static TestTimer Timer { get; } = new TestTimer() { ElapseImmediately = true };
    public static TestMutex Mutex { get; } = new TestMutex();

    public static Sw42Plus Sw42Plus {
      get {
        var sw42 = new Sw42Plus(new TestHttpClient()) {HostName = "http://Test" };
        sw42.Initialize();
        return sw42;
      }
    }

    public static T GetPropertyValue<T>(this object obj, string propName) {
      var prop = obj.GetType().GetProperty(propName);
      if (prop == null) Assert.Fail("Property '{0}' does not exist", propName);
      return (T)prop.GetValue(obj, null) ;
    }

    public static void SetPropertyValue<T>(this object obj, string propName, T value) {
      var prop = obj.GetType().GetProperty(propName);
      if(prop == null) Assert.Fail("Property '{0}' does not exist", propName);
      prop.SetValue(obj, value);
    }

    public static void InvokeMethod(this object obj, string methodName) {
      var method = obj.GetType().GetMethod(methodName);
      if (method == null) Assert.Fail("Method '{0}' does not exist", methodName);
      method.Invoke(obj, null);
    }
  }
}
