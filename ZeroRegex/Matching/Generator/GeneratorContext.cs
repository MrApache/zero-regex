using System.Collections.Generic;

namespace ZeroRegex
{
  internal readonly ref struct GeneratorContext
  {
    public readonly string TextVariable;
    public readonly string StartIntVariable;
    public readonly string LengthIntVariable;

    public readonly Dictionary<string, Method> MethodDeclarations;
    public readonly List<string> InvokationList;

    public GeneratorContext(string textVariable, string startIntVariable, string lengthIntVariable)
    {
      TextVariable = textVariable;
      StartIntVariable = startIntVariable;
      LengthIntVariable = lengthIntVariable;
      MethodDeclarations = new Dictionary<string, Method>();
      InvokationList = new List<string>();
    }

    public string CreateMethodInvokation(string methodName)
    {
      return $"{methodName}(ref {StartIntVariable}, ref {LengthIntVariable}, ref {TextVariable})";
    }

    public string CreateMethodInvokation(string methodName, string startName, string lengthName)
    {
      return $"{methodName}(ref {startName}, ref {lengthName}, ref {TextVariable})";
    }

    public void RegisterMethod(Method method)
    {
      MethodDeclarations.Add(method.Name, method);
      InvokationList.Add(method.Name);
    }
  }
}