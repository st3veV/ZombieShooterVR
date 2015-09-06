using UnityEngine;

public class InternalTimer {

    private float _timer = 0f;
    private float _lastSet = 0f;
	
    public bool Update (float f) {
        _timer -= f;
        return IsTimeUp();
	}

    public bool Update()
    {
        return Update(Time.deltaTime * 1000f);
    }

    public void Set(float f)
    {
        _lastSet = f;
        Reset();
    }

    public bool IsTimeUp()
    {
        return _timer <= 0;
    }

    public void Reset()
    {
        _timer = _lastSet;
    }
}
