using UniRx;
using UnityEngine;


public abstract class UIView<TViewModel> : MonoBehaviour 
{ 
    private readonly CompositeDisposable _bindings = new CompositeDisposable();
    
    protected TViewModel ViewModel { get; private set; }
    
    public void Bind(TViewModel viewModel) 
    { 
        Release(); 
        ViewModel = viewModel; 
        OnBind(_bindings);
    }
    
    protected abstract void OnBind(CompositeDisposable bindings);
    
    public void Release() 
    { 
        _bindings.Clear(); 
        OnRelease(); 
        ViewModel = default;
    }
    
    protected virtual void OnRelease() { }
    
    private void OnDestroy() => Release();
}