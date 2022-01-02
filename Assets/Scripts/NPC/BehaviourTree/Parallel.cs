namespace BehaviourTree {
    public class Parallel : Node {
        
        public Node[] nodes;

        public Parallel(Node[] nodes) {
            this.nodes = nodes;
        }

        public override State Evaluate() {
            State firstNodeState = nodes[0].Evaluate();
            for(int i = 1; i < nodes.Length; i++) {
                nodes[i].Evaluate();
            }
            return firstNodeState;
        }

        public override string ToString() {
            return "Parallel Node";
        }
    }
}
