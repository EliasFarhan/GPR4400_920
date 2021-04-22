using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.PlayerLoop;

namespace bt
{
    public enum Status
    {
        Success,
        Failure,
        Running,
        Error
    }
    public abstract class Behavior
    {
        public abstract Status Update();
    }

    public abstract class Decorator : Behavior
    {
        protected Behavior child_;
        public Behavior Child => child_;

    }

    public class Inverter : Decorator
    {
        public override Status Update()
        {
            var status = child_.Update();
            switch (status)
            {
                case Status.Failure:
                    return Status.Success;
                case Status.Success:
                    return Status.Failure;
            }
            return status;
        }
    }

    public class Succeeder : Decorator
    {
        public override Status Update()
        {
            var status = child_.Update();
            switch (status)
            {
                case Status.Error:
                    return status;
            }
            return Status.Success;
        }
    }
    
    public abstract class Composite : Behavior
    {
        protected Behavior[] children_;
        public Behavior[] Children => children_;
    }
    
    public class Sequence : Composite
    {
        public override Status Update()
        {
            foreach (var child in children_)
            {
                var status = child.Update();
                switch (status)
                {
                    case Status.Running:
                    case Status.Error:
                    case Status.Failure:
                        return status;
                }
            }

            return Status.Success;
        }
    }

    public class Selector : Composite
    {
        public override Status Update()
        {
            foreach (var child in children_)
            {
                var status = child.Update();
                switch (status)
                {
                    case Status.Running:
                    case Status.Error:
                    case Status.Success:
                        return status;
                }
            }

            return Status.Failure;
        }
    }
}