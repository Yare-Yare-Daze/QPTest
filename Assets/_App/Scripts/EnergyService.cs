using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;


public class EnergyService : Service, IEnergyService 
{ 
    private EnergySettings _settings;
    
    private ReactiveProperty<int> _current; 
    private ReactiveProperty<float> _secondsToNext;

    private CancellationTokenSource _cts;
    
    private UniTaskCompletionSource _spentSignal;
    
    public IReadOnlyReactiveProperty<int> Current => _current; 
    public IReadOnlyReactiveProperty<float> SecondsToNext => _secondsToNext; 
    public int Max => _settings.MaxEnergy;
    
    public EnergyService(EnergySettings settings) 
    { 
        _settings = settings;
        
        _current = new ReactiveProperty<int>(settings.MaxEnergy); 
        _secondsToNext = new ReactiveProperty<float>(1f);
    }
    
    protected override UniTask InitializeAsync(CancellationToken cancellation) 
    { 
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellation); 
        RegenLoopAsync(_cts.Token).Forget(); 
        return UniTask.CompletedTask;
    }
    
    protected override UniTask ReleaseAsync() 
    { 
        _cts?.Cancel(); 
        _cts?.Dispose(); 
        _cts = null; 
        _spentSignal?.TrySetCanceled(); 
        return UniTask.CompletedTask;
    }
    
    public bool TrySpend(int amount) 
    { 
        if (amount <= 0) 
            return false;

        if (_current.Value < amount) 
            return false;

        bool wasFull = _current.Value >= _settings.MaxEnergy; 
        _current.Value -= amount;
        
        if (wasFull) 
            _secondsToNext.Value = 0f;
        
        _spentSignal?.TrySetResult();
        
        return true;
    }

    private async UniTaskVoid RegenLoopAsync(CancellationToken token) 
    { 
        float regenSeconds = Mathf.Max(0.0001f, _settings.RegenSeconds);
        
        try 
        { 
            while (!token.IsCancellationRequested) 
            { 
                if (_current.Value >= _settings.MaxEnergy) 
                { 
                    _secondsToNext.Value = 1f; 
                    await WaitForSpendAsync(token); 
                    continue;
                }
                
                float elapsed = _secondsToNext.Value * regenSeconds;
                
                while (_current.Value < _settings.MaxEnergy && elapsed < regenSeconds) 
                { 
                    await UniTask.Yield(PlayerLoopTiming.Update, token); 
                    elapsed += Time.deltaTime; 
                    _secondsToNext.Value = Mathf.Clamp01(elapsed / regenSeconds);
                }

                if (_current.Value < _settings.MaxEnergy) 
                { 
                    _current.Value += 1; 
                    _secondsToNext.Value = _current.Value >= _settings.MaxEnergy ? 1f : 0f;
                }
            }
        }
        catch (OperationCanceledException) 
        {
            
        }
    }
    private async UniTask WaitForSpendAsync(CancellationToken token)
    { 
        _spentSignal = new UniTaskCompletionSource(); 
        using (token.Register(static state => ((UniTaskCompletionSource)state).TrySetCanceled(), _spentSignal)) 
        { 
            await _spentSignal.Task;
        }
        
        _spentSignal = null;
    }
}