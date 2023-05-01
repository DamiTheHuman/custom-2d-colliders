/*
The MIT License (MIT)

Copyright (c) 2016 GuyQuad

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

[AddComponentMenu("Physics 2D/Bezier Curve Collider 2D")]

[RequireComponent(typeof(EdgeCollider2D))]
public class BezierCurveCollider2D : MonoBehaviour
{

    public List<Vector2> controlPoints, handlerPoints;
    private float spacingOnCreation = 32f;

    [Range(3, 36)]
    public int smoothness = 15;
    private Vector2 origin, center;

    [HideInInspector]
    public bool initialized;

    public bool continous = true;

    [HideInInspector]
    public EdgeCollider2D edge;
    [HideInInspector]
    public PolygonCollider2D polygon;
    private List<Vector2> pts;

    public void Init()
    {
        if (this.initialized)
        {
            return;
        }

        this.initialized = true;
        this.continous = true;
        this.smoothness = 15;

        this.controlPoints = new List<Vector2>();
        this.handlerPoints = new List<Vector2>();

        this.controlPoints.Clear();
        this.handlerPoints.Clear();

        Vector2 pos = this.transform.localPosition;
        this.controlPoints.Add(pos);

        pos.x += this.spacingOnCreation;
        this.controlPoints.Add(pos);

        pos.x -= this.spacingOnCreation;
        pos.y += this.spacingOnCreation;
        this.handlerPoints.Add(pos);

        pos.x += this.spacingOnCreation;
        this.handlerPoints.Add(pos);

        this.drawCurve();
    }

    public void drawCurve()
    {
        this.pts = new List<Vector2>();
        this.pts.Clear();

        this.edge = this.GetComponent<EdgeCollider2D>();

        if (this.edge == null)
        {
            this.gameObject.AddComponent<EdgeCollider2D>();
        }

        if (this.controlPoints.Count == 2)
        {
            this.drawSegment(this.controlPoints[0] - (Vector2)this.transform.localPosition, this.controlPoints[1] - (Vector2)this.transform.localPosition, this.handlerPoints[0] - (Vector2)this.transform.localPosition, this.handlerPoints[1] - (Vector2)this.transform.localPosition);
        }
        else if (this.controlPoints.Count > 2)
        {
            int h = 0;
            for (int i = 0; i < this.controlPoints.Count - 1; i++)
            {
                this.drawSegment(this.controlPoints[i] - (Vector2)this.transform.localPosition, this.controlPoints[i + 1] - (Vector2)this.transform.localPosition, this.handlerPoints[h] - (Vector2)this.transform.localPosition, this.handlerPoints[h + 1] - (Vector2)this.transform.localPosition);
                h += 2;
            }
        }

        this.edge.points = this.pts.ToArray();
        //GetComponent<PolygonCollider2D>().points = pts.ToArray(); 
    }

    private void drawSegment(Vector3 cPt1, Vector3 cPt2, Vector3 hPt1, Vector3 hPt2)
    {
        this.pts.Add(cPt1);

        for (int i = 1; i < this.smoothness; i++)
        {
            this.pts.Add(this.CalculateBezierPoint(1f / this.smoothness * i, cPt1, hPt1, hPt2, cPt2));
        }

        this.pts.Add(cPt2);
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 controlP0, Vector3 handlerP0, Vector3 handlerP1, Vector3 controlP1)
    {
        //http://devmag.org.za/2011/04/05/bzier-curves-a-tutorial/
        Vector3 p = (Mathf.Pow(1.0f - t, 3) * controlP0) + (3 * Mathf.Pow(1 - t, 2) * t * handlerP0) + (3 * (1.0f - t) * Mathf.Pow(t, 2) * handlerP1) + (Mathf.Pow(t, 3) * controlP1);

        return p;
    }

    public void addControlPoint()
    {
        Vector2 pos = this.controlPoints[^1];
        float hPosY = this.handlerPoints[^1].y;

        float mul = (hPosY > pos.y) ? -1 : 1; // check if the handler point was below or top of the control point and use that info to make sure that the next handler point is in the opposite direction

        this.handlerPoints.Add(new Vector2(pos.x, pos.y + (4 * mul)));

        pos.x += 4;
        this.controlPoints.Add(pos);

        pos.y += 4 * mul;
        this.handlerPoints.Add(pos);

        this.drawCurve();
    }

    public void removeControlPoint()
    {
        if (this.controlPoints.Count > 2)
        {
            this.controlPoints.RemoveAt(this.controlPoints.Count - 1);
            this.handlerPoints.RemoveAt(this.handlerPoints.Count - 1);
            this.handlerPoints.RemoveAt(this.handlerPoints.Count - 1);
        }
    }

    public void convertToPolygonCollider()
    {
        if (this.polygon == null)
        {
            //Check for polygon
            if (this.GetComponent<PolygonCollider2D>() != null)
            {
                this.polygon = this.GetComponent<PolygonCollider2D>();
            }
            else
            {
                this.gameObject.AddComponent<PolygonCollider2D>();
                this.polygon = this.GetComponent<PolygonCollider2D>();
            }

        }

        this.polygon.points = this.edge.points;
    }
}
