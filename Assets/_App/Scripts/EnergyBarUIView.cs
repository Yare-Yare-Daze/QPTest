using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;


public class EnergyBarUIView : UIView<EnergyBarUIViewModel>
{ 
    [SerializeField] private TMP_Text _counterText; 
    [SerializeField] private Image _fillBar; 
    [SerializeField] private Button _spendButton;
    
    private const int SpendAmount = 10;
    
    [Inject] 
    public void Construct(EnergyBarUIViewModel viewModel) => Bind(viewModel);
    
    protected override void OnBind(CompositeDisposable bindings) 
    {
        
        ViewModel.Current
            .Subscribe(current => _counterText.text = $"{current} / {ViewModel.Max}")
            .AddTo(bindings);
        
        ViewModel.SecondsToNext
            .Subscribe(progress => _fillBar.fillAmount = progress)
            .AddTo(bindings);

        
        _spendButton.onClick.AddListener(OnSpendClicked);
    }

    protected override void OnRelease() 
    { 
        _spendButton.onClick.RemoveListener(OnSpendClicked);
    }
    
    private void OnSpendClicked() => ViewModel.TrySpend(SpendAmount);
}