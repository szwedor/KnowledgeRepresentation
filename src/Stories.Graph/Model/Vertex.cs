using System.Collections.Generic;
using Stories.Execution;

namespace Stories.Graph.Model
{
    public class Vertex
    {
        public List<Edge> EdgesOutgoing;

        public List<Edge> EdgesIncoming;

        public AppState State;

        public Vertex(AppState state)
        {
            this.EdgesOutgoing = new List<Edge>();
            this.EdgesIncoming = new List<Edge>();
            this.State = state;
        }

    }
}
