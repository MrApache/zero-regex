namespace ZeroRegex.Launcher;

public static class Program
{
  private const string _pattern =
    //@"((\w[^\W]+)[\.\-]?){1,}\@(([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3})|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$";
    @"^[\w\.\-]+@[a-zA-Z0-9\-]+\.[a-zA-Z]{2,}$";

  private const string _input = //"lama.loca.loca123@inca.com";
    "john.doe@my-company.uk";

  private static readonly Pattern _pt = PatternBuilder.Build(_pattern);

  public static void Main()
  {
    //Match matchx = _pt.Match(_input);
    //Foo foo = new Foo();
    //Match match = foo.Match(_input);
    //Console.WriteLine(match);
  }
}

[CompileRegex(@"^[\w\.\-]+@[a-zA-Z0-9\-]+\.[a-zA-Z]{2,}$")]
public readonly ref partial struct Foo;