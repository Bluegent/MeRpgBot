using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGClient
{
    using OpenTK;

    public class Animation
    {
        private Texture2D animatedTexture;
        private BezierSpline animationCurve;

        private long animationDuration;

        private float splineDuration;

        private long currentFrame;

        private Vector2 currentLocation;
        public Animation()
        {

        }

        public Animation(Texture2D texture, BezierSpline curve, long duration)
        {
            curve.Calculate();
            animatedTexture = texture;
            animationCurve = curve;
            animationDuration = duration / 1000 * 60;
            splineDuration = (float)animationDuration / curve.ResultCurve.Count;
            currentFrame = 0;
            currentLocation = new Vector2(curve.ResultCurve[0].X, curve.ResultCurve[0].Y);
        }

        public void Update(float delta)
        {
            currentFrame++;
            int currentSplinePoint = (int)(currentFrame / splineDuration);
            float elapsedFrames = currentFrame - currentSplinePoint * splineDuration;
            float weight = Remap(elapsedFrames, 0, splineDuration, 0, 1);
            if (currentSplinePoint < animationCurve.ResultCurve.Count - 1)
                currentLocation = Vector2.Lerp(
                    animationCurve.ResultCurve[currentSplinePoint],
                    animationCurve.ResultCurve[currentSplinePoint + 1], weight);
        }

        private float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public void Render(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawTexture(animatedTexture, currentLocation.X, currentLocation.Y, animatedTexture.Width, animatedTexture.Height);
        }
    }
}
