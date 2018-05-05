using System.Collections.Generic;
using Stories.Execution;

namespace Stories.Graph.Model
{
    using System.Linq;

    public class Vertex
    {
        private static int count = 0;
        private int key = count++;
        public List<Edge> EdgesOutgoing;

        public List<Edge> EdgesIncoming;

        public AppState State;

        public Vertex(AppState state)
        {
            this.EdgesOutgoing = new List<Edge>();
            this.EdgesIncoming = new List<Edge>();
            this.State = state;
        }
        public override string ToString()
        {
            return this.key.ToString();
        }
    }
}
