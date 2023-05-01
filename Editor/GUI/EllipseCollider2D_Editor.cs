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

[CustomEditor(typeof(EllipseCollider2D))]
public class EllipseCollider_Editor : Editor
{
    private EllipseCollider2D ec;
    private PolygonCollider2D polyCollider;
    private Vector2 off;

    private void OnEnable()
    {
        this.ec = (EllipseCollider2D)this.target;

        this.polyCollider = this.ec.GetComponent<PolygonCollider2D>();

        if (this.polyCollider == null)
        {
            this.polyCollider = this.ec.gameObject.AddComponent<PolygonCollider2D>();
        }

        this.polyCollider.points = this.ec.getPoints();
    }

    public override void OnInspectorGUI()
    {
        GUI.changed = false;
        this.DrawDefaultInspector();

        this.ec.advanced = EditorGUILayout.Toggle("Advanced", this.ec.advanced);

        if (this.ec.advanced)
        {
            this.ec.RadiusX = EditorGUILayout.FloatField("RadiusX", this.ec.RadiusX);
            this.ec.RadiusY = EditorGUILayout.FloatField("RadiusY", this.ec.RadiusY);
        }
        else
        {
            this.ec.RadiusX = EditorGUILayout.Slider("RadiusX", this.ec.RadiusX, 1, 25);
            this.ec.RadiusY = EditorGUILayout.Slider("RadiusY", this.ec.RadiusY, 1, 25);
        }

        if (GUI.changed || !this.off.Equals(this.polyCollider.offset))
        {
            this.polyCollider.points = this.ec.getPoints();
        }

        this.off = this.polyCollider.offset;
    }

}
