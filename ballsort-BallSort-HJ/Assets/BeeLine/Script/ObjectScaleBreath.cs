using UnityEngine;

public class ObjectScaleBreath : MonoBehaviour
{
    private void Update()
    {
        float oldScale = _currentScale;
        _currentScale = _currentScale + (_isToBig ? 1f : -1f) * _scaleSpeed * Time.deltaTime;

        if (_isToBig)
        {
            //增大
            if (oldScale <= _scaleToBig && _currentScale > _scaleToBig)
            {
                _isToBig = false;
            }
        }
        else
        {
            //减小
            if (oldScale >= _scaleToSmall && _currentScale < _scaleToSmall)
            {
                _isToBig = true;
            }
        }

        _transform.Get(gameObject).transform.localScale = _currentScale * Vector3.one;
    }

    [SerializeField] private float _scaleSpeed = 0.1f;
    [SerializeField] private float _scaleToBig = 1.3f;
    [SerializeField] private float _scaleToSmall = 0.9f;


    private float _currentScale = 1;
    private DelayGetComponent<Transform> _transform = new DelayGetComponent<Transform>();
    private bool _isToBig;
}