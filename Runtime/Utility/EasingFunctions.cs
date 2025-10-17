/*
 * Project: JuiceTwee
 * https://github.com/Kalmera74/JuiceTwee
 *
 * Author: Kalmera (GitHub: Kalmera74)
 * Copyright (c) 2025 Kalmera
 *
 * Licensed under the MIT License.
 * You may obtain a copy of the License at
 * https://opensource.org/licenses/MIT
 *
 * Version: 1.0.0
 */

using UnityEngine;

namespace JuiceTwee.Runtime.Utility
{
    public static class EasingFunctions
    {
        /// <summary>
        /// Returns an eased value between 0 and 1 based on the given time <paramref name="t"/> 
        /// and the selected <paramref name="curveType"/> easing function.
        /// </summary>
        /// <param name="t">
        /// The normalized time (0 to 1). Values outside this range are clamped.
        /// </param>
        /// <param name="curveType">
        /// The type of easing curve to apply. Supports Linear, EaseIn, EaseOut, and EaseInOut variants,
        /// including Sine, Cubic, Quart, Quint, Expo, Circ, Back, Elastic, and Bounce.
        /// </param>
        /// <returns>
        /// The eased value corresponding to <paramref name="t"/> using the specified <paramref name="curveType"/>.
        /// </returns>
        /// <remarks>
        /// This method uses <see cref="Mathf.Clamp01(float)"/> to ensure <paramref name="t"/> 
        /// is always within the 0–1 range before applying the easing function.
        /// </remarks>
        public static float GetEasedValue(float t, AnimationCurveType curveType)
        {
            t = Mathf.Clamp01(t);

            switch (curveType)
            {
                case AnimationCurveType.Linear:
                    return t;

                // EaseIn
                case AnimationCurveType.EaseIn:
                    return EaseInQuad(t);
                case AnimationCurveType.EaseInSine:
                    return EaseInSine(t);
                case AnimationCurveType.EaseInCubic:
                    return EaseInCubic(t);
                case AnimationCurveType.EaseInQuart:
                    return EaseInQuart(t);
                case AnimationCurveType.EaseInQuint:
                    return EaseInQuint(t);
                case AnimationCurveType.EaseInExpo:
                    return EaseInExpo(t);
                case AnimationCurveType.EaseInCirc:
                    return EaseInCirc(t);
                case AnimationCurveType.EaseInBack:
                    return EaseInBack(t);
                case AnimationCurveType.EaseInElastic:
                    return EaseInElastic(t);
                case AnimationCurveType.EaseInBounce:
                    return EaseInBounce(t);

                // EaseOut
                case AnimationCurveType.EaseOut:
                    return EaseOutQuad(t);
                case AnimationCurveType.EaseOutSine:
                    return EaseOutSine(t);
                case AnimationCurveType.EaseOutCubic:
                    return EaseOutCubic(t);
                case AnimationCurveType.EaseOutQuart:
                    return EaseOutQuart(t);
                case AnimationCurveType.EaseOutQuint:
                    return EaseOutQuint(t);
                case AnimationCurveType.EaseOutExpo:
                    return EaseOutExpo(t);
                case AnimationCurveType.EaseOutCirc:
                    return EaseOutCirc(t);
                case AnimationCurveType.EaseOutBack:
                    return EaseOutBack(t);
                case AnimationCurveType.EaseOutElastic:
                    return EaseOutElastic(t);
                case AnimationCurveType.EaseOutBounce:
                    return EaseOutBounce(t);

                // EaseInOut
                case AnimationCurveType.EaseInOut:
                    return EaseInOutQuad(t);
                case AnimationCurveType.EaseInOutSine:
                    return EaseInOutSine(t);
                case AnimationCurveType.EaseInOutCubic:
                    return EaseInOutCubic(t);
                case AnimationCurveType.EaseInOutQuart:
                    return EaseInOutQuart(t);
                case AnimationCurveType.EaseInOutQuint:
                    return EaseInOutQuint(t);
                case AnimationCurveType.EaseInOutExpo:
                    return EaseInOutExpo(t);
                case AnimationCurveType.EaseInOutCirc:
                    return EaseInOutCirc(t);
                case AnimationCurveType.EaseInOutBack:
                    return EaseInOutBack(t);
                case AnimationCurveType.EaseInOutElastic:
                    return EaseInOutElastic(t);
                case AnimationCurveType.EaseInOutBounce:
                    return EaseInOutBounce(t);

                default:
                    return t;
            }
        }

        // --- EaseIn Functions ---
        public static float EaseInSine(float t) => 1f - Mathf.Cos((t * Mathf.PI) / 2f);
        public static float EaseInQuad(float t) => t * t;
        public static float EaseInCubic(float t) => t * t * t;
        public static float EaseInQuart(float t) => t * t * t * t;
        public static float EaseInQuint(float t) => t * t * t * t * t;
        public static float EaseInExpo(float t) => Mathf.Approximately(t, 0f) ? 0f : Mathf.Pow(2f, 10f * t - 10f);
        public static float EaseInCirc(float t) => -(Mathf.Sqrt(1f - t * t) - 1f);
        public static float EaseInBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return c3 * t * t * t - c1 * t * t;
        }
        public static float EaseInElastic(float t)
        {
            const float c4 = (2f * Mathf.PI) / 3f;
            if (Mathf.Approximately(t, 0f)) return 0f;
            if (Mathf.Approximately(t, 1f)) return 1f;
            return -Mathf.Pow(2f, 10f * t - 10f) * Mathf.Sin((t * 10f - 10.75f) * c4);
        }
        public static float EaseInBounce(float t) => 1f - EaseOutBounce(1f - t); // EaseInBounce is often defined as reverse of EaseOutBounce

        // --- EaseOut Functions ---
        public static float EaseOutSine(float t) => Mathf.Sin((t * Mathf.PI) / 2f);
        public static float EaseOutQuad(float t) => 1f - (1f - t) * (1f - t);
        public static float EaseOutCubic(float t) => 1f - Mathf.Pow(1f - t, 3f);
        public static float EaseOutQuart(float t) => 1f - Mathf.Pow(1f - t, 4f);
        public static float EaseOutQuint(float t) => 1f - Mathf.Pow(1f - t, 5f);
        public static float EaseOutExpo(float t) => Mathf.Approximately(t, 1f) ? 1f : 1f - Mathf.Pow(2f, -10f * t);
        public static float EaseOutCirc(float t) => Mathf.Sqrt(1f - Mathf.Pow(t - 1f, 2f));
        public static float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
        }
        public static float EaseOutElastic(float t)
        {
            const float c4 = (2f * Mathf.PI) / 3f;
            if (Mathf.Approximately(t, 0f)) return 0f;
            if (Mathf.Approximately(t, 1f)) return 1f;
            return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
        }
        public static float EaseOutBounce(float t)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (t < 1 / d1)
            {
                return n1 * t * t;
            }
            else if (t < 2 / d1)
            {
                return n1 * (t -= (1.5f / d1)) * t + 0.75f;
            }
            else if (t < 2.5 / d1)
            {
                return n1 * (t -= (2.25f / d1)) * t + 0.9375f;
            }
            else
            {
                return n1 * (t -= (2.625f / d1)) * t + 0.984375f;
            }
        }

        // --- EaseInOut Functions ---
        public static float EaseInOutSine(float t) => -(Mathf.Cos(Mathf.PI * t) - 1f) / 2f;
        public static float EaseInOutQuad(float t) => t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
        public static float EaseInOutCubic(float t) => t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
        public static float EaseInOutQuart(float t) => t < 0.5f ? 8f * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 4f) / 2f;
        public static float EaseInOutQuint(float t) => t < 0.5f ? 16f * t * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 5f) / 2f;
        public static float EaseInOutExpo(float t)
        {
            if (Mathf.Approximately(t, 0f)) return 0f;
            if (Mathf.Approximately(t, 1f)) return 1f;
            if (t < 0.5f) return Mathf.Pow(2f, 20f * t - 10f) / 2f;
            return (2f - Mathf.Pow(2f, -20f * t + 10f)) / 2f;
        }
        public static float EaseInOutCirc(float t) => t < 0.5f
            ? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * t, 2f))) / 2f
            : (Mathf.Sqrt(1f - Mathf.Pow(-2f * t + 2f, 2f)) + 1f) / 2f;
        public static float EaseInOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c2 = c1 * 1.525f;
            return t < 0.5f
                ? (Mathf.Pow(2f * t, 2f) * ((c2 + 1f) * 2f * t - c2)) / 2f
                : (Mathf.Pow(2f * t - 2f, 2f) * ((c2 + 1f) * (t * 2f - 2f) + c2) + 2f) / 2f;
        }
        public static float EaseInOutElastic(float t)
        {
            const float c5 = (2f * Mathf.PI) / 4.5f; // Often (2 * PI) / 4.5
            if (Mathf.Approximately(t, 0f)) return 0f;
            if (Mathf.Approximately(t, 1f)) return 1f;
            if (t < 0.5f) return (Mathf.Pow(2f, 20f * t - 10f) * Mathf.Sin((20f * t - 11.125f) * c5)) / 2f;
            return (Mathf.Pow(2f, -20f * t + 10f) * Mathf.Sin((20f * t - 11.125f) * c5)) / 2f + 1f;
        }
        public static float EaseInOutBounce(float t) => t < 0.5f
            ? (1f - EaseOutBounce(1f - 2f * t)) / 2f
            : (1f + EaseOutBounce(2f * t - 1f)) / 2f;
    }

    // Your enum definition (as provided in your request)
    public enum AnimationCurveType
    {
        Linear,
        EaseIn,
        EaseInSine,
        EaseInCubic,
        EaseInQuart,
        EaseInQuint,
        EaseInExpo,
        EaseInCirc,
        EaseInBack,
        EaseInElastic,
        EaseInBounce,
        EaseOut,
        EaseOutSine,
        EaseOutCubic,
        EaseOutQuart,
        EaseOutQuint,
        EaseOutExpo,
        EaseOutCirc,
        EaseOutBack,
        EaseOutElastic,
        EaseOutBounce,
        EaseInOut,
        EaseInOutSine,
        EaseInOutCubic,
        EaseInOutQuart,
        EaseInOutQuint,
        EaseInOutExpo,
        EaseInOutCirc,
        EaseInOutBack,
        EaseInOutElastic,
        EaseInOutBounce,
    }

    public enum EasingDirection // This enum is not directly used in GetEasedValue, but good to have
    {
        In,
        Out,
    }
}