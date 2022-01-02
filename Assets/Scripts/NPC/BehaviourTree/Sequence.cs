namespace BehaviourTree {
    public class Sequence : Node {
        public Node[] nodes;
        public int    current = 0;
        public Sequence(Node[] nodes) {
            this.nodes = nodes;
        }
        
        public override State Evaluate() {
            State childState = nodes[current].Evaluate();
            // if the current node succeeded, report that SUCCESS upwards
            // and move to the next node.
            if (childState == State.SUCCESS) {
                current++;

                // reset to beginning if we have finished the sequence
                if (current >= nodes.Length) {
                    current = 0;
                    return State.SUCCESS;
                } else {
                    return Evaluate();
                }

                // if the current node is still RUNNING, keep RUNNING
            } else if (childState == State.RUNNING) {
                current = 0;
                return State.RUNNING;

                // if the current node failed, then return a FAILURE and return to the
                // beginning of the sequence
            } else {
                current = 0;
                return State.FAILURE;
            }
        }
    }

}
