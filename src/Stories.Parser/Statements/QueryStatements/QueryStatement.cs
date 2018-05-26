namespace Stories.Parser.Statements
{
    public class QueryStatement : Statement
    {
        public Sufficiency Sufficiency { get; set; }
  }

    public enum Sufficiency
    {
        Necessary,
        Possibly,
        Typically
    }
}
