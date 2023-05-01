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

using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierCurveCollider2D))]
public class BezierCurveCollider_Editor : Editor
{
    private BezierCurveCollider2D bc;
    private float handleSize = 4f;

    private void OnEnable()
    {
        this.bc = (BezierCurveCollider2D)this.target;

        if (!this.bc.initialized)
        {
            this.bc.Init();
        }
    }

    public override void OnInspectorGUI()
    {
        GUI.changed = false;
        this.DrawDefaultInspector();

        if (!this.bc.edge.offset.Equals(Vector2.zero))
        {
            this.bc.edge.offset = Vector2.zero; // prevent changes to offset
        }

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Point"))
        {
            this.bc.addControlPoint();
        }
        if (GUILayout.Button("Use Polygon Collider"))
        {
            this.bc.convertToPolygonCollider();
        }

        if (this.bc.controlPoints.Count > 2) // minimum 2 control points are always required
        {
            if (GUILayout.Button("Remove Point"))
            {
                this.bc.removeControlPoint();
            }
        }

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Reset"))
        {
            this.bc.initialized = false;
            this.bc.Init();
        }

        if (GUI.changed)
        {
            this.bc.drawCurve();
        }
    }

    private void OnSceneGUI()
    {
        GUI.changed = false;
        Handles.color = new Color(1, 1, 1, 0.5f);
        // manage control points
        for (int i = 0; i < this.bc.controlPoints.Count; i++)
        {
            Vector3 start = this.bc.controlPoints[i];
            Vector3 newPos = Handles.FreeMoveHandle(this.bc.controlPoints[i], Quaternion.identity, this.handleSize, Vector3.zero, Handles.ConeHandleCap);
            this.bc.controlPoints[i] = newPos;

            // if the control point was moved.. offset the joining handler points
            if (!start.Equals(newPos))
            {
                Vector2 offset = newPos - start;

                // if there are only 2 control points
                if (this.bc.controlPoints.Count == 2)
                {
                    this.bc.handlerPoints[i] += offset;
                }
                // if there are more than 2 control points
                else if (this.bc.controlPoints.Count > 2)
                {
                    // if you moved the first control point
                    if (i == 0)
                    {
                        this.bc.handlerPoints[0] += offset; // offset the handle
                    }
                    // if you moved the last control point
                    else if (i == this.bc.controlPoints.Count - 1)
                    {
                        this.bc.handlerPoints[^1] += offset; // offset the handle
                    }
                    // if you moved one of the other control points in the middle
                    else
                    {
                        int ind = (i * 2) - 1;
                        this.bc.handlerPoints[ind] += offset; // offset the top handle
                        this.bc.handlerPoints[++ind] += offset; // offset the bottom handle

                    }
                }
            }
        }

        // manage handler points
        // when using continous curves
        if (!this.bc.continous)
        {
            for (int i = 0; i < this.bc.handlerPoints.Count; i++)
            {
                this.bc.handlerPoints[i] = Handles.FreeMoveHandle(this.bc.handlerPoints[i], Quaternion.identity, this.handleSize, Vector3.zero, Handles.ConeHandleCap);
            }
        }
        else
        // when using non-continous curves
        {
            for (int i = 0; i < this.bc.handlerPoints.Count; i++)
            {
                // if there are only 2 control points
                if (this.bc.controlPoints.Count == 2)
                {
                    Handles.color = Color.red;
                    this.bc.handlerPoints[i] = Handles.FreeMoveHandle(this.bc.handlerPoints[i], Quaternion.identity, this.handleSize, Vector3.zero, Handles.ConeHandleCap);
                    Handles.color = Color.yellow;
                }
                // if there are more than 2 control points
                else if (this.bc.controlPoints.Count > 2)
                {
                    // no additional calculations required for the first and last handler points
                    if (i == 0 || i == this.bc.handlerPoints.Count - 1)
                    {
                        Handles.color = Color.red;
                        this.bc.handlerPoints[i] = Handles.FreeMoveHandle(this.bc.handlerPoints[i], Quaternion.identity, this.handleSize, Vector3.zero, Handles.ConeHandleCap);
                        Handles.color = Color.yellow;
                    }
                    else
                    {
                        // changes for the rest of the handler points in the middle
                        Vector3 start = this.bc.handlerPoints[i];
                        Handles.color = Color.red;
                        Vector3 newPos = Handles.FreeMoveHandle(this.bc.handlerPoints[i], Quaternion.identity, this.handleSize, Vector3.zero, Handles.ConeHandleCap);
                        Handles.color = Color.yellow;
                        this.bc.handlerPoints[i] = newPos;

                        if (!start.Equals(newPos))
                        {
                            bool movedTop = i % 2 == 1;

                            // if we are moving the top handle
                            if (movedTop)
                            {
                                int cp = (i + 1) / 2; // get the control point for this handle
                                // calc angle of the top handle
                                Vector2 dir = this.bc.handlerPoints[i] - this.bc.controlPoints[cp];
                                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                                angle = (angle + 360) % 360;

                                // adjust the angle of the bottom handle
                                float magH2 = Vector2.Distance(this.bc.controlPoints[cp], this.bc.handlerPoints[i + 1]);
                                angle = 270 - angle;

                                float x = this.bc.controlPoints[cp].x + (magH2 * Mathf.Sin(angle * Mathf.Deg2Rad));
                                float y = this.bc.controlPoints[cp].y + (magH2 * Mathf.Cos(angle * Mathf.Deg2Rad));

                                this.bc.handlerPoints[i + 1] = new Vector2(x, y);

                            }
                            else
                            //if we are moving the bottom handle
                            {
                                int cp = i / 2; // get the control point for this handle
                                // calc the angle of the bottom angle
                                Vector2 dir = this.bc.controlPoints[cp] - this.bc.handlerPoints[i];
                                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                                angle = (angle + 360) % 360;

                                // adjust the angle of top handle
                                float magH2 = Vector2.Distance(this.bc.controlPoints[cp], this.bc.handlerPoints[i - 1]);
                                angle = 360 - angle + 90;

                                float x = this.bc.controlPoints[cp].x + (magH2 * Mathf.Sin(angle * Mathf.Deg2Rad));
                                float y = this.bc.controlPoints[cp].y + (magH2 * Mathf.Cos(angle * Mathf.Deg2Rad));

                                this.bc.handlerPoints[i - 1] = new Vector2(x, y);
                            }
                        }
                    }
                }
            }
        }


        // draw a line from the control point to handler points
        if (this.bc.handlerPoints.Count == 2)
        {
            Handles.DrawLine(this.bc.handlerPoints[0], this.bc.controlPoints[0]);
            Handles.DrawLine(this.bc.handlerPoints[1], this.bc.controlPoints[1]);
        }
        else
        {
            int c = 0;
            for (int i = 0; i < this.bc.handlerPoints.Count; i += 2)
            {
                Handles.DrawLine(this.bc.handlerPoints[i], this.bc.controlPoints[c]);
                Handles.DrawLine(this.bc.handlerPoints[i + 1], this.bc.controlPoints[c + 1]);
                c++;
            }
        }

        if (GUI.changed)
        {
            Undo.RegisterCompleteObjectUndo(this.target, "�pdate Handle \"" + Regex.Replace("Bezier Curve", "(?!^)([A-Z])", " $1") + "\"");
            this.bc.drawCurve();
        }
    }

}