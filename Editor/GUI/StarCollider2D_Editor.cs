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

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StarCollider2D))]
public class StarCollider_Editor : Editor
{
    private StarCollider2D sc;
    private PolygonCollider2D polyCollider;
    private Vector2 off;

    private void OnEnable()
    {
        this.sc = (StarCollider2D)this.target;

        this.polyCollider = this.sc.GetComponent<PolygonCollider2D>();

        if (this.polyCollider == null)
        {
            this.polyCollider = this.sc.gameObject.AddComponent<PolygonCollider2D>();
        }

        this.polyCollider.points = this.sc.getPoints();
    }

    public override void OnInspectorGUI()
    {
        GUI.changed = false;
        this.DrawDefaultInspector();

        this.sc.Rotation = EditorGUILayout.IntSlider("Rotation", this.sc.Rotation, 0, 360 / this.sc.Points);

        this.sc.advanced = EditorGUILayout.Toggle("Advanced", this.sc.advanced);

        if (this.sc.advanced)
        {
            this.sc.RadiusA = EditorGUILayout.FloatField("RadiusA", this.sc.RadiusA);
            this.sc.RadiusB = EditorGUILayout.FloatField("RadiusB", this.sc.RadiusB);
        }
        else
        {
            this.sc.RadiusA = EditorGUILayout.Slider("RadiusA", this.sc.RadiusA, 1, 25);
            this.sc.RadiusB = EditorGUILayout.Slider("RadiusB", this.sc.RadiusB, 1, 25);
        }

        if (GUI.changed || !this.off.Equals(this.polyCollider.offset))
        {
            this.polyCollider.points = this.sc.getPoints();
        }

        this.off = this.polyCollider.offset;
    }

}
#endif
