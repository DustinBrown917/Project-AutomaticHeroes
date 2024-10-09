using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace SegoLib.Benchmarking
{
    /// <summary>
    /// Base class for benchmarking. Inherit from this class, create the code that you'd like to benchmark within that class, add it to the Benchmarks list, then RunBenchmark()
    /// <para>Courtesy of https://github.com/Matthew-J-Spencer/Unity-Benchmarks/blob/524f42866d4ea5639a2926d2ba47c267f63a273e/Assets/_Game/_Scripts/Classes/BenchmarkBase.cs#L9 </para>
    /// </summary>
    public abstract class BenchmarkBase : MonoBehaviour
    {
        public string BenchmarkName { get; protected set; }
        public int MaxRecommendedIterations { get; protected set; } = int.MaxValue;
        protected readonly List<BenchmarkData> Benchmarks = new List<BenchmarkData>();

        private void Awake() => Init();

        protected abstract void Init();

        public IEnumerable<BenchmarkData> RunBenchmark(int iterations)
        {
            foreach (var benchmark in Benchmarks) {
                yield return Run(benchmark);
            }

            BenchmarkData Run(BenchmarkData data)
            {
                var watch = Stopwatch.StartNew();
                for (var i = 0; i < iterations; i++) {
                    data.Action();
                }

                watch.Stop();
                data.Result = watch.ElapsedMilliseconds;
                return data;
            }
        }
    }

    public struct BenchmarkData
    {
        public readonly string Name;
        public readonly Action Action;
        public long Result;

        public BenchmarkData(string name, Action action)
        {
            Name = name;
            Action = action;
            Result = 0;
        }

        public override string ToString()
        {
            return Name + ": " + Result + "ms";
        }
    } 
}