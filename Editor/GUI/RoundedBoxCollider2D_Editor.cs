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

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoundedBoxCollider2D))]
public class RoundedBoxCollider_Editor : Editor
{
    private RoundedBoxCollider2D rb;
    private PolygonCollider2D polyCollider;
    private Vector2 off;

    private void OnEnable()
    {
        this.rb = (RoundedBoxCollider2D)this.target;

        this.polyCollider = this.rb.GetComponent<PolygonCollider2D>();

        if (this.polyCollider == null)
        {
            this.polyCollider = this.rb.gameObject.AddComponent<PolygonCollider2D>();
        }

        Vector2[] pts = this.rb.getPoints();

        if (pts != null)
        {
            this.polyCollider.points = pts;
        }
    }

    public override void OnInspectorGUI()
    {
        GUI.changed = false;
        this.DrawDefaultInspector();

        // automatically adjust the radius according to width and height
        float lesser = (this.rb.Width > this.rb.Height) ? this.rb.Height : this.rb.Width;
        lesser /= 2f;
        lesser = Mathf.Round(lesser * 100f) / 100f;
        this.rb.Radius = EditorGUILayout.Slider("Radius", this.rb.Radius, 0f, lesser);
        this.rb.Radius = Mathf.Clamp(this.rb.Radius, 0f, lesser);

        this.rb.advanced = EditorGUILayout.Toggle("Advanced", this.rb.advanced);

        if (this.rb.advanced)
        {
            this.rb.Height = EditorGUILayout.FloatField("Height", this.rb.Height);
            this.rb.Width = EditorGUILayout.FloatField("Width", this.rb.Width);
        }
        else
        {
            this.rb.Height = EditorGUILayout.Slider("Height", this.rb.Height, 1, 25);
            this.rb.Width = EditorGUILayout.Slider("Width", this.rb.Width, 1, 25);
        }

        if (GUILayout.Button("Reset"))
        {
            this.rb.Smoothness = 15;
            this.rb.Width = 2;
            this.rb.Height = 2;
            this.rb.Trapezoid = 0.5f;
            this.rb.Radius = 0.5f;
            this.polyCollider.offset = Vector2.zero;
        }

        if (GUI.changed || !this.off.Equals(this.polyCollider.offset))
        {
            Vector2[] pts = this.rb.getPoints();
            if (pts != null)
            {
                this.polyCollider.points = pts;
            }
        }

        this.off = this.polyCollider.offset;
    }

}
