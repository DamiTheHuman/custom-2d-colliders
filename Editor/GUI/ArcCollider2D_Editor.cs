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

[CustomEditor(typeof(ArcCollider2D))]
public class ArcCollider_Editor : Editor
{
    private ArcCollider2D ac;
    private PolygonCollider2D polyCollider;
    private Vector2 off;

    private void OnEnable()
    {
        this.ac = (ArcCollider2D)this.target;

        this.polyCollider = this.ac.GetComponent<PolygonCollider2D>();

        if (this.polyCollider == null)
        {
            this.ac.gameObject.AddComponent<PolygonCollider2D>();
            this.polyCollider = this.ac.GetComponent<PolygonCollider2D>();
        }

        this.polyCollider.points = this.ac.getPoints();
    }

    public override void OnInspectorGUI()
    {
        GUI.changed = false;
        this.DrawDefaultInspector();

        this.ac.advanced = EditorGUILayout.Toggle("Advanced", this.ac.advanced);

        if (this.ac.advanced)
        {
            this.ac.Radius = EditorGUILayout.FloatField("Radius", this.ac.Radius);
        }
        else
        {
            this.ac.Radius = EditorGUILayout.Slider("Radius", this.ac.Radius, 1, 64);
        }
        if (!this.ac.PizzaSlice)
        {
            if (this.ac.advanced)
            {
                this.ac.Thickness = EditorGUILayout.FloatField("Thickness", this.ac.Thickness);
            }
            else
            {
                this.ac.Thickness = EditorGUILayout.Slider("Thickness", this.ac.Thickness, 1, 25);
            }
        }

        if (GUI.changed || !this.off.Equals(this.polyCollider.offset))
        {
            this.polyCollider.points = this.ac.getPoints();
        }

        this.off = this.polyCollider.offset;
    }

}