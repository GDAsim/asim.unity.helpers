using System.Collections.Generic;
using UnityEngine;

namespace asim.unity.helpers
{
    /// <summary>
    /// Wrapper/Helper class that helps to draw Simple Lines and Circles, Rect,  in unity's OnGUI
    /// Supports custom origin, and rotation , save and apply GUI Matrix
    /// Will Add basic shapes
    /// </summary>
    public static class UnityOnGUIHelper
    {
        public static int Width => Screen.width;
        public static int Height => Screen.height;
        public static Vector2 Center => new Vector2(Screen.width / 2, Screen.height / 2);

        static Vector2 OriginPos = new Vector2(0, 0);
        static float OriginRotation = 0;
        static Vector2 OriginScale = new Vector2(1, 1);

        static Stack<Vector2> savedorigin = new Stack<Vector2>();
        static Stack<float> savedrotation = new Stack<float>();
        static Stack<Vector2> savedscale = new Stack<Vector2>();

        /// <summary>
        /// Save the current Origin to a stack
        /// </summary>
        public static void SaveOrigin()
        {
            savedorigin.Push(OriginPos);
            savedrotation.Push(OriginRotation);
            savedscale.Push(OriginScale);
        }

        /// <summary>
        /// Load the Origin pop from the stack
        /// </summary>
        public static void LoadOrigin()
        {
            OriginPos = savedorigin.Pop();
            OriginRotation = savedrotation.Pop();
            OriginScale = savedscale.Pop();
        }

        /// <summary>
        /// Reset the Origin to Default, 0 position, 0 rotation, 1 scale
        /// </summary>
        public static void ResetOrigin()
        {
            OriginPos = Vector2.zero;
            OriginRotation = 0;
            OriginScale = Vector2.one;
        }

        public static void SetOriginPosition(Vector2 newPos)
        {
            OriginPos = newPos;
        }
        public static void TranslateOrigin(Vector2 translateAmt)
        {
            OriginPos += translateAmt;
        }
        public static void SetOriginRotattion(float radians)
        {
            OriginRotation = radians;
        }
        public static void RotateOrigin(float radians)
        {
            OriginRotation += radians;
        }
        public static void SetOriginScale(Vector2 newScale)
        {
            OriginScale = newScale;
        }
        public static void Scale(Vector2 scaleAmt)
        {
            OriginScale += scaleAmt;
        }


        #region Drawing

        static Texture2D DefaultTexture = new Texture2D(2, 2);

        /// <summary>
        /// Draw Text adjusted by originscale and position, rotation
        /// </summary>
        public static void DrawText(float f, Vector2 position, Vector2 size)
        {
            DrawText(f.ToString(), position, size);
        }
        public static void DrawText(string text, Vector2 position,Vector2 size)
        {
            Matrix4x4 originalMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(OriginPos, Quaternion.AngleAxis(OriginRotation * Mathf.Rad2Deg, Vector3.right), new Vector3(OriginScale.x, OriginScale.y, 1));

            GUI.Label(new Rect(position, size), text);

            GUI.matrix = originalMatrix;
        }

        /// <summary>
        /// Draw Line adjusted by originscale and position, rotation
        /// </summary>
        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
        {
            // Determine the angle of the line.
            float angle = Vector3.Angle(pointB - pointA, Vector2.right);
            // If pointB is above pointA, then angle needs to be negative.
            if (pointA.y > pointB.y) { angle = -angle; }

            Vector3 scale = new Vector3((pointB - pointA).magnitude + 0.1f, width, 1);

            Matrix4x4 originalMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(OriginPos + pointA, Quaternion.AngleAxis(OriginRotation * Mathf.Rad2Deg + angle, Vector3.back), new Vector3(OriginScale.x * scale.x, OriginScale.y * scale.y, 1 * scale.z));

            GUI.DrawTexture(new Rect(0, 0, 1, 1), DefaultTexture, ScaleMode.ScaleToFit, false, 0, color, width, 0);

            GUI.matrix = originalMatrix;
        }

        /// <summary>
        /// Draw a Dot / Point / Circle adjusted by originscale and position, rotation
        /// </summary>
        public static void DrawDot(Vector2 position, float size, Color32 color, Color32 borderColor, float thickness = 0)
        {
            DrawEllipse(position, new Vector2(size, size), color, borderColor, thickness);
        }

        /// <summary>
        /// Draw a Ellipse adjusted by originscale and position, rotation
        /// </summary>
        public static void DrawEllipse(Vector2 position, Vector2 size, Color32 color, Color32 borderColor, float thickness = 0)
        {
            Matrix4x4 originalMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(OriginPos, Quaternion.AngleAxis(OriginRotation * Mathf.Rad2Deg, Vector3.back), new Vector3(OriginScale.x, OriginScale.y, 1));

            Rect rect = new Rect(position, size);
            GUI.DrawTexture(rect, DefaultTexture, ScaleMode.ScaleToFit, false, 0, color, 0, size.x);
            if (thickness > 0) GUI.DrawTexture(rect, DefaultTexture, ScaleMode.ScaleToFit, false, 0, borderColor, thickness, size.x);

            GUI.matrix = originalMatrix;
        }

        /// <summary>
        /// Draws a Rect OnGUI adjusted by originscale and position, rotation
        /// </summary>
        public static void DrawRect(Vector2 position, Vector2 size, Color32 color, Color32 borderColor, bool IsCenterOrigin = false, float thickness = 0)
        {
            Matrix4x4 originalMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(OriginPos, Quaternion.AngleAxis(OriginRotation * Mathf.Rad2Deg, Vector3.back),new Vector3(OriginScale.x, OriginScale.y,1));

            Rect rect;
            if (IsCenterOrigin) rect = new Rect(position - size / 2f, size);
            else rect = new Rect(position, size);

            GUI.DrawTexture(rect, DefaultTexture, ScaleMode.StretchToFill, false, 0, color, 0, 0);
            if (thickness > 0) GUI.DrawTexture(rect, DefaultTexture, ScaleMode.StretchToFill, false, 0, borderColor, thickness, 0);

            GUI.matrix = originalMatrix;
        }


        #endregion

        //Extras
        /// <summary>
        /// Set Background Color, 
        /// Option to Do Only onces or repeatetly
        /// Option to change Main camera Clear mode to 'dont clear'
        /// </summary>
        static bool HasBGDrawned = false;
        public static void SetBGColor(Color32 bgColor,bool repeat = true, bool setCameraFlag = false)
        {
            if(setCameraFlag) Camera.main.clearFlags = CameraClearFlags.Nothing;

            if (!repeat && !HasBGDrawned)
            {
                GL.Clear(false, true, bgColor, 0);
                HasBGDrawned = true;
            }
            else if(repeat)
            {
                GL.Clear(false, true, bgColor, 0);
                HasBGDrawned = true;
            }
        }

        /// <summary>
        /// Set Target Framerate
        /// </summary>
        public static void SetFrameRate(int rate)
        {
            Application.targetFrameRate = rate;
        }
    }
}