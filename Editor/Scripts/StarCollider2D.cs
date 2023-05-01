/*
The MIT License (MIT)

Modified work Copyright (c) 2016 Richard Kopelow
Original work Copyright (c) 2016 GuyQuad

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

You can contact me by email at guyquad27@gmail.com or on Reddit at https://www.reddit.com/user/GuyQuad
*/

using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Physics 2D/Star Collider 2D")]

[RequireComponent(typeof(PolygonCollider2D))]
public class StarCollider2D : CustomCollider2D
{

    [Range(1, 25), HideInInspector, SerializeField]
    private float radiusA = 1;
    public float RadiusA { get => this.radiusA; set { this.radiusA = value; this.updateCollider(); } }

    [Range(1, 25), HideInInspector, SerializeField]
    private float radiusB = 2;
    public float RadiusB { get => this.radiusB; set { this.radiusB = value; this.updateCollider(); } }

    [Range(3, 36), SerializeField]
    private int points = 5;
    public int Points { get => this.points; set { this.points = value; this.updateCollider(); } }

    [HideInInspector, SerializeField]
    private int rotation = 0;
    public int Rotation { get => this.rotation; set { this.rotation = value; this.updateCollider(); } }

    [HideInInspector]
    public bool advanced = false;
    private Vector2 center;

    public override Vector2[] getPoints()
    {
        List<Vector2> pts = new List<Vector2>();

        float ang = this.rotation;

        for (int i = 0; i <= this.points * 2; i++)
        {
            float radius = (i % 2 == 0) ? this.radiusA : this.radiusB;
            float x = radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            float y = radius * Mathf.Sin(ang * Mathf.Deg2Rad);

            pts.Add(new Vector2(x, y));
            ang += 360f / (this.points * 2f);
        }

        return pts.ToArray();
    }
}
