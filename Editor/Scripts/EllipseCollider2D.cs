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

[AddComponentMenu("Physics 2D/Ellipse Collider 2D")]
public class EllipseCollider2D : CustomCollider2D
{

    [Range(1, 25), HideInInspector, SerializeField]
    private float radiusX = 1, radiusY = 2;
    public float RadiusX { get => this.radiusX; set { this.radiusX = value; this.updateCollider(); } }
    public float RadiusY { get => this.radiusY; set { this.radiusY = value; this.updateCollider(); } }

    [Range(0, 180), SerializeField]
    private float rotation = 0;
    public float Rotation { get => this.rotation; set { this.rotation = value; this.updateCollider(); } }

    [HideInInspector]
    public bool advanced = false;
    private Vector2 origin, center;

    public override Vector2[] getPoints()
    {
        List<Vector2> points = new List<Vector2>();

        float ang = 0;
        float o = this.rotation * Mathf.Deg2Rad;

        for (int i = 0; i <= this.smoothness; i++)
        {
            float a = ang * Mathf.Deg2Rad;

            // fan shuriken
            //float radius;
            //float radX = 90 - (Mathf.Abs(ang) % 90);
            //float radY = 90 - radX;
            //radius = ((radiusX * radX / 90f) + (radiusY * radY / 90f)) / 2f;
            //float x = center.x + radius * Mathf.Cos(a);
            //float y = center.y + radius * Mathf.Sin(a);

            // https://www.uwgb.edu/dutchs/Geometry/HTMLCanvas/ObliqueEllipses5a.HTM
            float x = (this.radiusX * Mathf.Cos(a) * Mathf.Cos(o)) - (this.radiusY * Mathf.Sin(a) * Mathf.Sin(o));
            float y = (-this.radiusX * Mathf.Cos(a) * Mathf.Sin(o)) - (this.radiusY * Mathf.Sin(a) * Mathf.Cos(o));

            points.Add(new Vector2(x, y));
            ang += 360f / this.smoothness;
        }

        points.RemoveAt(0);

        return points.ToArray();
    }
}
