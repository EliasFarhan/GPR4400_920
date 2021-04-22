using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.PlayerLoop;

namespace bt
{
    public enum Status
    {
        Init,
        Success,
        Failure,
        Running,
        Error
    }
    public abstract class Behavior
    {
        public Status Status { get; set; } = Status.Init;

        public virtual void Init()
        {
            Status = Status.Running;
        }

        protected void InitIfNeeded()
        {
            if (Status == Status.Init)
            {
                Init();
            }
        }
        /**
         * <summary>Update the Behavior.
         * Be sure to call InitIfNeeded at the start of the update.</summary>
         */
        public abstract Status Update();
    }

    public abstract class Decorator : Behavior
    {
        protected Behavior child_;
        public Behavior Child
        {
            get => child_;
            set => child_ = value;
        }

        public override void Init()
        {
            base.Init();
            child_.Status = Status.Init;
        }
    }

    public class Inverter : Decorator
    {
        public override Status Update()
        {
            InitIfNeeded();
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
            InitIfNeeded();
            var status = child_.Update();
            switch (status)
            {
                case Status.Error:
                case Status.Running:
                    return status;
            }
            return Status.Success;
        }
    }

    public class Repeater : Decorator
    {
        public override Status Update()
        {
            InitIfNeeded();
            var status = child_.Update();
            switch (status)
            {
                case Status.Failure:
                case Status.Success:
                    Status = Status.Init;
                    break;
            }
            return status;
        }
    }
    
    public abstract class Composite : Behavior
    {
        protected Behavior[] children_;
        public Behavior[] Children => children_;

        public override void Init()
        {
            base.Init();
            foreach (var child in children_)
            {
                child.Status = Status.Init;
            }
        }
    }
    
    public class Sequence : Composite
    {
        public override Status Update()
        {
            InitIfNeeded();
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
            InitIfNeeded();
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