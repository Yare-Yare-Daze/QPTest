using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

public abstract class Service : IAsyncStartable, IDisposable 
{ 
    async Awaitable IAsyncStartable.StartAsync(CancellationToken cancellation) 
    { 
        await InitializeAsync(cancellation);
    }

    void IDisposable.Dispose() 
    { 
        ReleaseAsync().Forget();
    }
    
    protected virtual UniTask InitializeAsync(CancellationToken cancellation) => UniTask.CompletedTask;
    
    protected virtual UniTask ReleaseAsync() => UniTask.CompletedTask;
}