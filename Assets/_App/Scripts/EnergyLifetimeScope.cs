using UnityEngine;
using VContainer;
using VContainer.Unity;

public class EnergyLifetimeScope : LifetimeScope
{
    [SerializeField] private EnergySettings _energySettings;
    [SerializeField] private EnergyBarUIView _energyBarView;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(_energySettings);
        
        builder.RegisterEntryPoint<EnergyService>(Lifetime.Singleton)
            .As<IEnergyService>()
            .AsSelf();
        
        builder.Register<EnergyBarUIViewModel>(Lifetime.Transient);
        
        if (_energyBarView != null)
            builder.RegisterComponent(_energyBarView);
    }
}
