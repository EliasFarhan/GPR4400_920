using System;
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

    class InitCounterBehavior : Behavior
    {
        private int counter_ = 0;
        public int Counter => counter_;
        public override void Init()
        {
            base.Init();
            counter_++;
        }

        public override Status Update()
        {
            InitIfNeeded();
            return Status.Success;
        }
    }
    
    class StatusAfterCountBehavior : Behavior
    {
        private int counter_ = 0;
        private int period_ = 0;
        private Status returnReturnStatus_ = Status.Failure;

        public override void Init()
        {
            base.Init();
            counter_ = 0;
        }

        public StatusAfterCountBehavior(Status returnStatus, int period)
        {
            period_ = period;
            returnReturnStatus_ = returnStatus;
        }
        
        public override Status Update()
        {
            InitIfNeeded();
            counter_++;
            if (counter_ >= period_)
            {
                Status = returnReturnStatus_;
            }
            return counter_ < period_ ? Status.Running : returnReturnStatus_;
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
        try
        {
            inverter1.Update();
            Assert.Fail("Calling decorator when success should throw!");
        }
        catch (BehaviorException e)
        {
            Assert.Pass("Calling decorator when success should throw!");
        }
        var inverter2 = new Inverter {Child = new FailureBehavior()};
        
        Assert.AreEqual(Status.Success, inverter2.Update());
        try
        {
            inverter2.Update();
            Assert.Fail("Calling decorator when success should throw!");
        }
        catch (BehaviorException e)
        {
            Assert.Pass("Calling decorator when success should throw!");
        }
        var inverter3 = new Inverter {Child = new RunningBehavior()};
        Assert.AreEqual(Status.Running, inverter3.Update());        
        try
        {
            inverter3.Update();
            Assert.Pass("Calling decorator while running should not throw!");
        }
        catch (BehaviorException e)
        {
            Assert.Fail("Calling decorator while running should not throw!");
        }
    }

    [Test]
    public void SucceederTest()
    {
        var succeeder1 = new Succeeder {Child = new SuccessBehavior()};

        Assert.AreEqual(Status.Success, succeeder1.Update());
        try
        {
            succeeder1.Update();
            Assert.Fail("Calling decorator when success should throw!");
        }
        catch (BehaviorException e)
        {
            Assert.Pass("Calling decorator when success should throw!");
        }
        var succeeder2 = new Succeeder {Child = new FailureBehavior()};
        
        Assert.AreEqual(Status.Success, succeeder2.Update());
        try
        {
            succeeder2.Update();
            Assert.Fail("Calling decorator when success should throw!");
        }
        catch (BehaviorException e)
        {
            Assert.Pass("Calling decorator when success should throw!");
        }
        var succeeder3 = new Succeeder {Child = new RunningBehavior()};
        Assert.AreEqual(Status.Running, succeeder3.Update());
        try
        {
            succeeder3.Update();
            Assert.Pass("Calling decorator while running should not throw!");
        }
        catch (BehaviorException e)
        {
            Assert.Fail("Calling decorator while running should not throw!");
        }
    }

    [Test]
    public void RepeaterTest()
    {
        var counter = new InitCounterBehavior();
        var repeater1 = new Repeater {Child = counter};
        repeater1.Update();
        repeater1.Update();
        repeater1.Update();
        Assert.AreEqual(counter.Counter, 3);
    }
    
    [Test]
    public void RepeaterUntilFailTest()
    {

        
        var counter1 = new StatusAfterCountBehavior(Status.Success, 3);
        var repeater1 = new RepeatUntilFail() {Child = counter1};
        var status = repeater1.Update();
        Assert.AreEqual(status, Status.Running);
        status = repeater1.Update();
        Assert.AreEqual(status, Status.Running);
        status = repeater1.Update();
        Assert.AreEqual(status, Status.Success);
        status = repeater1.Update();
        Assert.AreEqual(status, Status.Running);
        
        var counter2 = new StatusAfterCountBehavior(Status.Failure, 3);
        var repeater2 = new RepeatUntilFail() {Child = counter2};
        var status2 = repeater2.Update();
        Assert.AreEqual(status2, Status.Running);
        status2 = repeater2.Update();
        Assert.AreEqual(status2, Status.Running);
        status2 = repeater2.Update();
        Assert.AreEqual( Status.Success, status2);
        try
        {
            status2 = repeater2.Update();
            Assert.Fail();
        }
        catch (BehaviorException e)
        {
            Assert.Pass();   
        }
        
    }

    [Test]
    public void SequenceTest()
    {
        var sequence1 = new Sequence
        {
            Children = new Behavior[]
            {
                new SuccessBehavior(), new SuccessBehavior()
            }
        };
        Assert.AreEqual(Status.Success, sequence1.Update());
        try
        {
            sequence1.Update();
        }
        catch (BehaviorException e)
        {
            Assert.Pass("Calling sequence when succeded should throw");
        }
        var sequence2 = new Sequence
        {
            Children = new Behavior[]
            {
                new SuccessBehavior(), new RunningBehavior()
            }
        };
        Assert.AreEqual(Status.Running, sequence2.Update());
        Assert.AreEqual(Status.Running, sequence2.Update());
        
        var sequence3 = new Sequence
        {
            Children = new Behavior[]
            {
                new SuccessBehavior(), new FailureBehavior()
            }
        };
        Assert.AreEqual(Status.Failure, sequence3.Update());
        try
        {
            sequence3.Update();
        }
        catch (BehaviorException e)
        {
            Assert.Pass("Calling sequence when failed should throw");
        }
    }

}
