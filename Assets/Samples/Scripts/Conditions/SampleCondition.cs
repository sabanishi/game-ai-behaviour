using System;
using GameAiBehaviour;

/// <summary>
/// サンプル用の条件
/// </summary>
[BehaviourTreeCondition("サンプル/サンプル用条件")]
public class SampleCondition : HandleableCondition {
    [Serializable]
    public struct Hoge {
        public int foo;
        public float fuga;
    }
    
    public int test;
    public float test2;
    public Hoge hoge;
}