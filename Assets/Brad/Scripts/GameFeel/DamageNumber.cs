using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class DamageNumber : MonoBehaviour
{
    [Header("Motion")]
    [SerializeField] private float lifetime = 0.8f;
    [SerializeField] private float riseSpeed = 1.5f;
    [SerializeField] private float horizontalJitter = 0.3f;

    [Header("Crit Styling")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color critColor = Color.yellow;
    [SerializeField] private float critScaleMultiplier = 1.5f;

    private TextMeshPro _text;
    private float _elapsed;
    private Vector3 _moveDirection;
    private Color _startColor;

    private void Awake()
    {
        _text = GetComponent<TextMeshPro>();
    }

    public void Initialize(float damage, bool isCrit)
    {
        _text.text = Mathf.RoundToInt(damage).ToString();
        _text.color = isCrit ? critColor : normalColor;

        if (isCrit)
            transform.localScale *= critScaleMultiplier;

        _startColor = _text.color;
        
        float jitter = Random.Range(-horizontalJitter, horizontalJitter);
        _moveDirection = new Vector3(jitter, riseSpeed, 0f);
    }

    private void Update()
    {
        _elapsed += Time.deltaTime;

        transform.position += _moveDirection * Time.deltaTime;

        float t = _elapsed / lifetime;
        _text.color = new Color(_startColor.r, _startColor.g, _startColor.b, Mathf.Lerp(1f, 0f, t));

        if (_elapsed >= lifetime)
            Destroy(gameObject);
    }
}
