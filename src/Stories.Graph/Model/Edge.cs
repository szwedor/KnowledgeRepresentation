namespace Stories.Graph.Model
{
    public class Edge
    {
        public Vertex From;

        public Vertex To;

        public bool IsTypical;

        public string Action;

        public Edge(Vertex from, Vertex to, bool isTypical, string action)
        {
            this.From = from;
            this.To = to;
            this.IsTypical = isTypical;
            this.Action = action;
        }
    }
}
