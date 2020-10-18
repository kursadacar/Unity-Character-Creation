using System;
using UnityEditor;
using UnityEngine;

namespace EnhancedHierarchy {
    public static class After {

        public static void Condition(Func<bool> condition, Action callback, double timeoutMs = -1) {
            var update = new EditorApplication.CallbackFunction(() => { });
            var timeoutsAt = EditorApplication.timeSinceStartup + (timeoutMs / 1000d);

            update = () => {
                if (timeoutMs > 0d && EditorApplication.timeSinceStartup >= timeoutsAt) {
                    EditorApplication.update -= update;
                    Debug.LogError("After.Condition timed out");
                    return;
                }

                if (condition()) {
                    EditorApplication.update -= update;
                    callback();
                }
            };

            EditorApplication.update += update;
        }

        public static void Frames(int frames, Action callback) {
            var f = 0;
            Condition(() => f++ >= frames, callback);
        }

        public static void Milliseconds(double milliseconds, Action callback) {
            var end = EditorApplication.timeSinceStartup + (milliseconds / 1000f);
            Condition(() => EditorApplication.timeSinceStartup >= end, callback);
        }

    }
}