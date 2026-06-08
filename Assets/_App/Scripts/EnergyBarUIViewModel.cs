using UniRx;

public class EnergyBarUIViewModel 
{ 
    private readonly IEnergyService _energy;
    
    public IReadOnlyReactiveProperty<int> Current => _energy.Current; 
    public IReadOnlyReactiveProperty<float> SecondsToNext => _energy.SecondsToNext; 
    public int Max => _energy.Max;
    
    public EnergyBarUIViewModel(IEnergyService energy) 
    { 
        _energy = energy;
    }
    
    public bool TrySpend(int amount) => _energy.TrySpend(amount);
}
