namespace ZeroRegex
{
  internal readonly struct Method
  {
    public readonly string Name;
    public readonly string Content;

    public Method(string name, string content)
    {
      Name = name;
      Content = content;
    }

    public override int GetHashCode()
    {
      return Content.GetHashCode();
    }
  }
}