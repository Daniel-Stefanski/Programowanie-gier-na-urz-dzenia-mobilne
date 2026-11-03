using UnityEngine;

public enum EggType
{
    Zwykle,
    Zlote,
    Zgnile
}

public class FallingEgg : MonoBehaviour
{
    [Header("Rodzaj jajka - ustaw na prefabie")]
    [SerializeField] private EggType eggType = EggType.Zwykle;

    private float fallSpeed = 2f;
    private float groundY = -4.2f;
    private bool handled = false;

    public EggType Type => eggType;

    public void Initialize(float speed, float groundLevel)
    {
        fallSpeed = speed;
        groundY = groundLevel;
    }

    private void Update()
    {
        if (handled || GameManager.Instance == null || GameManager.Instance.IsGameOver)
            return;

        transform.position += Vector3.down * (fallSpeed * Time.deltaTime);

        if (transform.position.y <= groundY)
        {
            handled = true;
            GameManager.Instance.EggMissed(this);
            Destroy(gameObject);
        }
    }

    public void Collect()
    {
        if (handled || GameManager.Instance == null || GameManager.Instance.IsGameOver)
            return;

        handled = true;
        GameManager.Instance.EggCaught(this);
        Destroy(gameObject);
    }
}
