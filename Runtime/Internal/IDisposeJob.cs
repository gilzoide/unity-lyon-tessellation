using System;
using Unity.Jobs;

namespace Gilzoide.LyonTesselation
{
    public readonly struct DisposeJob<TDisposable> : IJob
        where TDisposable : struct, IDisposable
    {
        private readonly TDisposable _disposable;

        public DisposeJob(TDisposable disposable)
        {
            _disposable = disposable;
        }

        public void Execute()
        {
            _disposable.Dispose();
        }
    }

    public static class IDisposableExtensions
    {
        public static JobHandle ScheduleDisposeJob<TDisposable>(this TDisposable disposable, JobHandle dependencies = default)
            where TDisposable : struct, IDisposable
        {
            return new DisposeJob<TDisposable>(disposable).Schedule(dependencies);
        }
    }
}
