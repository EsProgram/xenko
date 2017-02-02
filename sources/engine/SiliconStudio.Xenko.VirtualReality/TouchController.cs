﻿using System;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Games;

namespace SiliconStudio.Xenko.VirtualReality
{
    public abstract class TouchController : IDisposable
    {
        public abstract Vector3 Position { get; }

        public abstract Quaternion Rotation { get; }

        public abstract Vector3 LinearVelocity { get; }

        public abstract Vector3 AngularVelocity { get; }

        public abstract DeviceState State { get; }

        public virtual void Update(GameTime time)
        {           
        }

        public abstract float Trigger { get; }

        public abstract float Grip { get; }

        public abstract bool IndexPointing { get; }

        public abstract bool IndexResting { get; }

        public abstract bool ThumbUp { get; }

        public abstract bool ThumbResting { get; }

        /// <summary>
        /// Returns true if in this frame the button switched to pressed state
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public abstract bool IsPressedDown(TouchControllerButton button);

        /// <summary>
        /// Returns true if button switched is in the pressed state
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public abstract bool IsPressed(TouchControllerButton button);

        /// <summary>
        /// Returns true if in this frame the button was released
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public abstract bool IsPressReleased(TouchControllerButton button);

        public virtual void Dispose()
        {          
        }
    }
}
