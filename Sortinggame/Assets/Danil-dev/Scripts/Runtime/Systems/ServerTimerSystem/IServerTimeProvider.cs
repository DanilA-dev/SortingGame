using System;
using UnityEngine;

namespace D_Dev.ServerTimerSystem
{
    public interface IServerTimeProvider
    {
        public Awaitable<DateTime?> RequestUtcTimeAsync();
    }
}
