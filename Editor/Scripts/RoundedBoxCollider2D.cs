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

[AddComponentMenu("Physics 2D/Rounded Box Collider 2D")]

[RequireComponent(typeof(PolygonCollider2D))]
public class RoundedBoxCollider2D : CustomCollider2D
{

    [Range(.2f, 25), HideInInspector, SerializeField]
    private float height = 2;
    public float Height { get => this.height; set { this.height = value; this.updateCollider(); } }

    [Range(.2f, 25), HideInInspector, SerializeField]
    private float width = 2;
    public float Width { get => this.width; set { this.width = value; this.updateCollider(); } }

    [HideInInspector, SerializeField]
    private float radius = .5f, wt, wb;
    public float Radius { get => this.radius; set { this.radius = value; this.updateCollider(); } }

    [Range(0.05f, .95f), SerializeField]
    private float trapezoid = .5f;
    public float Trapezoid { get => this.trapezoid; set { this.trapezoid = value; this.updateCollider(); } }

    [HideInInspector]
    public bool advanced = false;

    private Vector2 center1, center2, center3, center4;
    private float ang = 0;
    private List<Vector2> points;

    public override Vector2[] getPoints()
    {
        this.points = new List<Vector2>();
        this.points.Clear();

        this.wt = this.width + this.width - ((this.width + this.width) * this.trapezoid);   // width top
        this.wb = (this.width + this.width) * this.trapezoid;                       // width bottom

        // vertices
        Vector2 vTR = new Vector2(this.wt / 2f, +(this.height / 2f)); // top right vertex
        Vector2 vTL = new Vector2(this.wt / -2f, +(this.height / 2f)); // top left vertex
        Vector2 vBL = new Vector2(this.wb / -2f, -(this.height / 2f)); // bottom left vertex
        Vector2 vBR = new Vector2(this.wb / 2f, -(this.height / 2f)); // bottom right vertex

        Vector2 dir = vBL - vTL;
        float hypAngleTL = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // hypertenuse angle top left corner
        hypAngleTL = (hypAngleTL + 360) % 360; // get it between 0-360 range
        hypAngleTL = 360 - hypAngleTL; // use the inside angle
        hypAngleTL /= 2f; // got our adjacent angle
        ///        
        ///    adj TL (Top Left)
        ///    _____
        ///    \    |
        ///     \   |
        ///   h  \  | opp = radius
        ///       \ |
        ///        \|
        /// 
        float adjTL = this.radius / Mathf.Tan(hypAngleTL * Mathf.Deg2Rad);
        this.center1 = new Vector3(vTR.x - adjTL, vTR.y - this.radius, 0);
        this.center2 = new Vector3(vTL.x + adjTL, vTL.y - this.radius, 0);


        float hypAngleBL = (180 - (hypAngleTL * 2f)) / 2f; // hypertenuse angle bottom left corner
        /// 
        ///        /|
        ///       / |
        ///   h  /  | opp = radius
        ///     /   |
        ///    /____|
        /// 
        ///     adj BL (Bottom Left)
        /// 
        float adjBL = this.radius / Mathf.Tan(hypAngleBL * Mathf.Deg2Rad);
        this.center3 = new Vector3(vBL.x + adjBL, vBL.y + this.radius, 0);
        this.center4 = new Vector3(vBR.x - adjBL, vBR.y + this.radius, 0);

        // prevent overlapping of the corners
        this.center1.x = Mathf.Max(0, this.center1.x);
        this.center2.x = Mathf.Min(0, this.center2.x);
        this.center3.x = Mathf.Min(0, this.center3.x);
        this.center4.x = Mathf.Max(0, this.center4.x);

        // curveTOP angles
        Vector2 tmpDir = vBR - vTR;
        float tmpAng = Mathf.Atan2(tmpDir.y, tmpDir.x) * Mathf.Rad2Deg;
        tmpAng = (tmpAng + 360) % 360;
        float x = vTR.x + (adjTL * Mathf.Cos(tmpAng * Mathf.Deg2Rad));
        float y = vTR.y + (adjTL * Mathf.Sin(tmpAng * Mathf.Deg2Rad));
        Vector2 startPos = new Vector2(x, y);

        bool canPlot = Vector2.Distance(startPos, this.center1) >= this.radius * .85f;

        if (!canPlot)
        {
            return null;
        }

        tmpDir = startPos - this.center1;
        tmpAng = Mathf.Atan2(tmpDir.y, tmpDir.x) * Mathf.Rad2Deg;
        tmpAng = (tmpAng + 360) % 360;

        float t = (tmpAng > 180) ? tmpAng - 360 : tmpAng;

        this.ang = tmpAng;
        float totalAngle = (t < 0) ? 90f - t : 90f - tmpAng;
        this.calcPoints(this.center1, totalAngle);
        this.calcPoints(this.center2, totalAngle);


        // curveBottom angles
        tmpDir = vTL - vBL;
        tmpAng = Mathf.Atan2(tmpDir.y, tmpDir.x) * Mathf.Rad2Deg;
        tmpAng = (tmpAng + 360) % 360;
        x = vBL.x + (adjBL * Mathf.Cos(tmpAng * Mathf.Deg2Rad));
        y = vBL.y + (adjBL * Mathf.Sin(tmpAng * Mathf.Deg2Rad));
        startPos = new Vector2(x, y);

        canPlot = Vector2.Distance(startPos, this.center3) >= this.radius * .9f;

        if (!canPlot)
        {
            return null;
        }

        tmpDir = startPos - this.center3;
        tmpAng = Mathf.Atan2(tmpDir.y, tmpDir.x) * Mathf.Rad2Deg;
        tmpAng = (tmpAng + 360) % 360;

        this.ang = tmpAng;
        totalAngle = 270 - tmpAng;
        this.calcPoints(this.center3, totalAngle);
        this.calcPoints(this.center4, totalAngle);

        return this.points.ToArray();
    }

    private void calcPoints(Vector2 ctr, float totAngle)
    {
        for (int i = 0; i <= this.smoothness; i++)
        {
            float a = this.ang * Mathf.Deg2Rad;
            float x = ctr.x + (this.radius * Mathf.Cos(a));
            float y = ctr.y + (this.radius * Mathf.Sin(a));

            this.points.Add(new Vector2(x, y));
            this.ang += totAngle / this.smoothness;
        }

        this.ang -= 90f / this.smoothness;
    }
}
