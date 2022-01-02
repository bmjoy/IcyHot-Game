namespace BehaviourTree {
    public class Selector : Node {
        public Node[] nodes;
        public int    current = 0;

        public Selector(Node[] nodes) {
            this.nodes = nodes;
        }

        public void AppendNode(Node n) {
            var ns = this.nodes;
            this.nodes = new Node[this.nodes.Length + 1];
            ns.CopyTo(this.nodes, 0);
            this.nodes[this.nodes.Length - 1] = n;
            // for (int i = 0; i < this.nodes.Length; i++) {
            //     Debug.Log(this.nodes[i].ToString());
            // }
        }

        public override State Evaluate() {
            if (nodes.Length < 1) {
                return State.FAILURE;
            }
            
            State childState = nodes[current].Evaluate(); 

            // if the current child Returns SUCCESS, report SUCCESSful
            // and return to start
            if (childState == State.SUCCESS) {
                current = 0;
                return State.SUCCESS;

                // if the current child is RUNNING, pass RUNNING and reset
            } else if(childState == State.RUNNING) {
                current = 0;
                return State.RUNNING;

                // if the current child has failed, evaluate the next child
            } else {
                current++;
                // if all children have failed, return a FAILURE and reset
                if (current >= nodes.Length) {
                    current = 0;
                    return State.FAILURE;
                } else {
                    return Evaluate();
                }
            }
        }
    }
}
