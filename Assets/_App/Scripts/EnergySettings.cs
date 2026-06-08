using UnityEngine;

[CreateAssetMenu(menuName = "App/Energy Settings", fileName = "EnergySettings")]
public class EnergySettings : ScriptableObject 
{
    [SerializeField, Min(1)] private int _maxEnergy = 100;
    
    [SerializeField, Min(0.01f)] private float _regenSeconds = 60f;
    
    public int MaxEnergy => _maxEnergy; 
    public float RegenSeconds => _regenSeconds;
}
    