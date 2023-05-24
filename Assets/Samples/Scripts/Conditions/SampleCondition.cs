using System;
using GameAiBehaviour;

/// <summary>
/// サンプル用の条件
/// </summary>
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