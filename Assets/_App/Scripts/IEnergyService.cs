using UniRx;


public interface IEnergyService 
{ 
    IReadOnlyReactiveProperty<int> Current { get; }
    
    IReadOnlyReactiveProperty<float> SecondsToNext { get; }
    
    int Max { get; }
    
    bool TrySpend(int amount);
}
