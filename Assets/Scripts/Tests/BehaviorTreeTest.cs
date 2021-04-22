using System.Collections;
using System.Collections.Generic;
using bt;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace bt
{
    class SuccessBehavior : Behavior
    {
        public override void Init()
        {
            base.Init();
            Status = Status.Success;
        }

        public override Status Update()
        {
            InitIfNeeded();
            return Status;
        }
    }
    class FailureBehavior : Behavior
    {
        public override void Init()
        {
            base.Init();
            Status = Status.Failure;
        }

        public override Status Update()
        {
            InitIfNeeded();
            return Status;
        }
    }

    class RunningBehavior : Behavior
    {
        public override Status Update()
        {
            InitIfNeeded();
            return Status;
        }
    }
}

public class BehaviorTreeTest
{
    [Test]
    public void InverterTest()
    {
        var inverter1 = new Inverter {Child = new SuccessBehavior()};

        Assert.AreEqual(Status.Failure, inverter1.Update());

        var inverter2 = new Inverter {Child = new FailureBehavior()};
        
        Assert.AreEqual(Status.Success, inverter2.Update());

        var inverter3 = new Inverter {Child = new RunningBehavior()};
        Assert.AreEqual(Status.Running, inverter3.Update());
    }

    [Test]
    public void SucceederTest()
    {
        var succeeder1 = new Succeeder {Child = new SuccessBehavior()};

        Assert.AreEqual(Status.Success, succeeder1.Update());

        var succeeder2 = new Succeeder {Child = new FailureBehavior()};
        
        Assert.AreEqual(Status.Success, succeeder2.Update());

        var succeeder3 = new Succeeder {Child = new RunningBehavior()};
        Assert.AreEqual(Status.Running, succeeder3.Update());
    }

}
