using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public abstract class CustomCollider2D : MonoBehaviour
{
    [Range(10, 90), SerializeField]
    protected int smoothness = 24;
    public int Smoothness { get => this.smoothness; set { this.smoothness = value; this.updateCollider(); } }
    public Vector2 offset = new Vector2();
    public abstract Vector2[] getPoints();
    protected void updateCollider() => this.GetComponent<PolygonCollider2D>().points = this.getPoints();
}
