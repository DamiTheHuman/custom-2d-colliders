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

[CustomEditor(typeof(CustomCapsuleCollider2D))]
public class CustomCapsuleCollider2D_Editor : Editor
{
    private CustomCapsuleCollider2D capCol;
    private PolygonCollider2D polyCollider;
    private Vector2 off;
    private bool advanced;

    private void OnEnable()
    {
        this.capCol = (CustomCapsuleCollider2D)this.target;

        this.polyCollider = this.capCol.GetComponent<PolygonCollider2D>();

        if (this.polyCollider == null)
        {
            this.polyCollider = this.capCol.gameObject.AddComponent<PolygonCollider2D>();
        }

        this.polyCollider.points = this.capCol.getPoints();
    }

    public override void OnInspectorGUI()
    {
        GUI.changed = false;
        this.DrawDefaultInspector();

        this.capCol.Bullet = EditorGUILayout.Toggle("Bullet", this.capCol.Bullet);

        if (this.capCol.Bullet)
        {
            this.capCol.Flip = EditorGUILayout.Toggle("Flip", this.capCol.Flip);
        }

        this.capCol.advanced = EditorGUILayout.Toggle("Advanced", this.capCol.advanced);

        if (this.capCol.advanced)
        {
            this.capCol.Height = EditorGUILayout.FloatField("Height", this.capCol.Height);
            this.capCol.Radius = Mathf.Clamp(this.capCol.Radius, 0f, this.capCol.Height / 2);
            this.capCol.Radius = EditorGUILayout.FloatField("Radius", this.capCol.Radius);
        }
        else
        {
            this.capCol.Height = EditorGUILayout.Slider("Height", this.capCol.Height, 1, 25);
            this.capCol.Radius = Mathf.Clamp(this.capCol.Radius, 0f, this.capCol.Height / 2);
            this.capCol.Radius = EditorGUILayout.Slider("Radius", this.capCol.Radius, 0.25f, this.capCol.Height / 2f);
        }

        if (GUI.changed || !this.off.Equals(this.polyCollider.offset))
        {
            this.polyCollider.points = this.capCol.getPoints();
        }

        this.off = this.polyCollider.offset;
    }

}
