using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthVisualizer : MonoBehaviour
{

    public LifetimeComponent ObjectToMonitor;
    public Slider SliderVisual;
    

	// Use this for initialization
	void Start ()
	{
	    if (ObjectToMonitor != null)
	    {
	        ObjectToMonitor.OnDie += ObjectToMonitor_OnDie;
	        ObjectToMonitor.OnDamage += ObjectToMonitor_OnDamage;
	    }
	    if (SliderVisual != null)
	    {
	        SliderVisual.minValue = 0;
	        SliderVisual.maxValue = 1;
	    }
	}

    private void ObjectToMonitor_OnDamage(float damage)
    {
        if (SliderVisual != null)
        {
            SliderVisual.normalizedValue = ObjectToMonitor.CurrentHealthPercentage;
        }
    }

    private void ObjectToMonitor_OnDie()
    {
        removeListeners();
        ObjectToMonitor = null;
    }

    private void removeListeners()
    {
        ObjectToMonitor.OnDamage -= ObjectToMonitor_OnDamage;
        ObjectToMonitor.OnDie -= ObjectToMonitor_OnDie;
    }

    // Update is called once per frame
	void Update () {
	
	}
}
