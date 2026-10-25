using UnityEngine;

public class Capsule : MonoBehaviour
{
    public Vector3 startPos; // Położenie początkowe obiektu
    public Vector3 endPos; // Położenie końcowe obiektu
    public float duration = 3; // Czas trwania ruchu w jednym kierunku
    private float time = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        float t = Mathf.PingPong (time/duration, 1);
        transform.position = Vector3.Lerp(startPos, endPos, t);
    }
}
