using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
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
        Running
    }

    public class BehaviorException : Exception
    {
        public BehaviorException(string msg) : base(msg)
        { 
        }
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
            switch (Status)
            {
                case Status.Init:
                    Init();
                    break;
                case Status.Failure:
                case Status.Success:
                    throw new BehaviorException(
                        "Do not call finished behavior");
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
                    Status = Status.Success;
                    return Status;
                case Status.Success:
                    Status = Status.Failure;
                    return Status;
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
                case Status.Running:
                    return status;
            }
            Status = Status.Success;
            return Status;
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

    public class RepeatUntilFail : Decorator
    {
        public override Status Update()
        {
            InitIfNeeded();
            var status = child_.Update();
            switch (status)
            {
                case Status.Failure:
                    Status = Status.Success;
                    return Status;
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
        public Behavior[] Children
        {
            get => children_;
            set => children_ = value;
        }

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
                switch (child.Status)
                {
                    case Status.Success:
                        continue;
                }
                var status = child.Update();
                switch (status)
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Failure:
                        Status = status;
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
                switch (child.Status)
                {
                    case Status.Failure:
                        continue;
                }
                var status = child.Update();
                switch (status)
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Success:
                        Status = status;
                        return status;
                }
            }

            return Status.Failure;
        }
    }
}