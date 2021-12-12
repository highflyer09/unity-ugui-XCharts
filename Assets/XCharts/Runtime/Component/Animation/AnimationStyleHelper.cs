/******************************************/
/*                                        */
/*     Copyright (c) 2021 monitor1394     */
/*     https://github.com/monitor1394     */
/*                                        */
/******************************************/
using UnityEngine;
using XUGL;

namespace XCharts
{
    public static class AnimationStyleHelper
    {
        public static float CheckDataAnimation(BaseChart chart, Serie serie, int dataIndex, float destProgress)
        {
            if (!serie.animation.IsItemAnimation())
            {
                serie.animation.context.isAllItemAnimationEnd = false;
                return destProgress;
            }
            if (serie.animation.IsFinish())
            {
                serie.animation.context.isAllItemAnimationEnd = false;
                return destProgress;
            }
            var isDataAnimationEnd = true;
            float currHig = serie.animation.CheckItemProgress(dataIndex, destProgress, ref isDataAnimationEnd);
            if (!isDataAnimationEnd)
            {
                serie.animation.context.isAllItemAnimationEnd = false;
            }
            return currHig;
        }

        public static void UpdateSerieAnimation(Serie serie)
        {
            var serieType = serie.GetType();
            var animationType = AnimationType.LeftToRight;
            if (serieType.IsDefined(typeof(DefaultAnimationAttribute), false))
            {
                animationType = serieType.GetAttribute<DefaultAnimationAttribute>().type;
            }
            UpdateAnimationType(serie.animation, animationType);
            serie.animation.context.isAllItemAnimationEnd = true;
        }

        public static void UpdateAnimationType(AnimationStyle animation, AnimationType defaultType)
        {
            animation.context.type = animation.type == AnimationType.Default
                ? defaultType
                : animation.type;
        }

        public static bool GetAnimationPosition(AnimationStyle animation, bool isY, Vector3 lp, Vector3 cp, float progress, ref Vector3 ip)
        {
            if (animation.context.type == AnimationType.AlongPath)
            {
                var dist = Vector3.Distance(lp, cp);
                var rate = (dist - animation.context.currentPathDistance + animation.GetCurrDetail()) / dist;
                ip = Vector3.Lerp(lp, cp, rate);
                return true;
            }
            else
            {
                var startPos = isY ? new Vector3(-10000, progress) : new Vector3(progress, -10000);
                var endPos = isY ? new Vector3(10000, progress) : new Vector3(progress, 10000);

                return UGLHelper.GetIntersection(lp, cp, startPos, endPos, ref ip);
            }
        }
    }
}