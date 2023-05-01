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

[AddComponentMenu("Physics 2D/CustomCapsule Collider 2D")]
public class CustomCapsuleCollider2D : CustomCollider2D
{

    [HideInInspector, SerializeField]
    private bool bullet = false, flip = false;
    public bool Bullet { get => this.bullet; set { this.bullet = value; this.updateCollider(); } }
    public bool Flip { get => this.flip; set { this.flip = value; this.updateCollider(); } }
    [HideInInspector]
    [Range(.5f, 25), SerializeField]
    private float radius = 1;
    public float Radius { get => this.radius; set { this.radius = value; this.updateCollider(); } }
    [Range(1, 25), SerializeField]
    private float height = 4;
    public float Height { get => this.height; set { this.height = value; this.updateCollider(); } }
    [Range(0, 180), SerializeField]
    private float rotation = 0;
    public float Rotation { get => this.rotation; set { this.rotation = value; this.updateCollider(); } }

    [HideInInspector]
    public bool advanced = false;
    private Vector2 center, center1, center2;
    private List<Vector2> points;
    private float ang = 0;

    public override Vector2[] getPoints()
    {
        this.points = new List<Vector2>();

        float r = (this.height / 2f) - this.radius;

        if (this.bullet && this.flip)
        {
            r += this.radius;
        }

        this.center1.x = r * Mathf.Sin(this.rotation * Mathf.Deg2Rad);
        this.center1.y = r * Mathf.Cos(this.rotation * Mathf.Deg2Rad);

        if (this.bullet)
        {
            if (!this.flip)
            {
                r += this.radius;
            }
            else
            {
                r -= this.radius;
            }
        }

        this.center2.x = r * Mathf.Sin((this.rotation + 180f) * Mathf.Deg2Rad);
        this.center2.y = r * Mathf.Cos((this.rotation + 180f) * Mathf.Deg2Rad);



        this.ang = 360f - this.rotation;
        this.ang %= 360;

        // top semi circle
        for (int i = 0; i <= this.smoothness; i++)
        {
            if (this.bullet && this.flip)
            {
                this.calcPointLocation(this.radius, this.center1);
                this.ang += 180f;
                this.calcPointLocation(this.radius, this.center1);
                i = this.smoothness + 1;
            }
            else
            {
                this.calcPointLocation(this.radius, this.center1);
                this.ang += 180f / this.smoothness;
            }
        }

        this.ang -= 180f / this.smoothness;
        this.ang %= 360;

        // bottom semi circle
        for (int i = 0; i <= this.smoothness; i++)
        {
            if (this.bullet && !this.flip)
            {
                this.calcPointLocation(this.radius, this.center2);
                this.ang += 180f;
                this.calcPointLocation(this.radius, this.center2);
                i = this.smoothness + 1;
            }
            else
            {
                this.calcPointLocation(this.radius, this.center2);
                this.ang += 180f / this.smoothness;
            }
        }

        return this.points.ToArray();
    }

    private void calcPointLocation(float r, Vector2 centerPt)
    {
        float a = this.ang * Mathf.Deg2Rad;
        float x = centerPt.x + (r * Mathf.Cos(a));
        float y = centerPt.y + (r * Mathf.Sin(a));

        this.points.Add(new Vector2(x, y));
    }
}
