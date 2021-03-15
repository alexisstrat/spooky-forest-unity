using UnityEngine;

namespace Spooky_Forest.Scripts.Native
{
    public static class AndroidToast
    {
        public static void ShowAndroidToast(string message)
        {
            //create a Toast class object
            AndroidJavaClass toastClass =
                new AndroidJavaClass("android.widget.Toast");

            //create an array and add params to be passed
            object[] toastParams = new object[3];
            AndroidJavaClass unityActivity =
                new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            toastParams[0] =
                unityActivity.GetStatic<AndroidJavaObject>
                    ("currentActivity");
            toastParams[1] = message;
            toastParams[2] = toastClass.GetStatic<int>
                ("LENGTH_LONG");

            //call static function of Toast class, makeText
            AndroidJavaObject toastObject =
                toastClass.CallStatic<AndroidJavaObject>
                    ("makeText", toastParams);

            //show toast
            toastObject.Call("show");
        }
    }
}