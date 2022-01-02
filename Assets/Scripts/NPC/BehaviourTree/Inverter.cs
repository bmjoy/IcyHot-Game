namespace BehaviourTree {
    public class Inverter : Node {
        public Inverter(Node child) {
            this.child = child;
        }

        public override State Evaluate() {
            State childState = child.Evaluate();
            if (childState == State.SUCCESS) {
                return State.FAILURE;
            } else if (childState == State.FAILURE) {
                return State.SUCCESS;
            } else {
                return State.RUNNING;
            }
        }

        public override string ToString() {
            return "Inverter containing: " + child.ToString();
        }
    }
}
