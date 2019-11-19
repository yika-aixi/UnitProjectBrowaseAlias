using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CabinIcarus.ProjectBrowserAliasTools
{
    [InitializeOnLoad]
    public static class GUIStyleEx
    {
        private static MethodInfo _getNumCharactersThatFitWithinWidth;
        
        static GUIStyleEx()
        {
            _getNumCharactersThatFitWithinWidth = typeof(GUIStyle).GetMethod("GetNumCharactersThatFitWithinWidth",
                BindingFlags.NonPublic | BindingFlags.Instance);
        }
        
        /// <summary>
        /// UnityEditor.ObjectListArea - GetCroppedLabelText (G:1211)
        /// </summary>
        /// <param name="self"></param>
        /// <param name="text"></param>
        /// <param name="cropWidth"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static string CropText(this GUIStyle self,string text,in float cropWidth,string symbol = "…")
        {
            int thatFitWithinWidth = (int) _getNumCharactersThatFitWithinWidth.Invoke(self,new object[]{text,cropWidth});
            
            int num;
            switch (thatFitWithinWidth)
            {
                case -1:
                    return text;
                case 0:
                case 1:
                    num = 0;
                    break;
                default:
                    num = thatFitWithinWidth != text.Length ? 1 : 0;
                    break;
            }
            text = num == 0 ? text : text.Substring(0, thatFitWithinWidth - 1) + symbol;

            return text;
        }
    }
}