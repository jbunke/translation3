using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TRANSLATION3
{
    public class GameSettings
    {
        private Camera.FollowMode followMode;
        private int period;
        private ControlMode controlMode;

        public enum ControlMode
        {
            WASD,
            NUMPAD
        }

        private GameSettings(Camera.FollowMode followMode, int period, 
            ControlMode controlMode)
        {
            this.followMode = followMode;
            this.period = period;
            this.controlMode = controlMode;
        }

        public static GameSettings defaultSettings()
        {
            return new GameSettings(Camera.FollowMode.STEADY, 20, ControlMode.WASD);
        }

        public int getPeriod() { return period; }

        public ControlMode getControlMode()
        {
            return controlMode;
        }

        public Keys[] getControls()
        {
            switch (controlMode)
            {
                case ControlMode.NUMPAD:
                    return new Keys[] {Keys.NumPad8, Keys.NumPad5,
                        Keys.NumPad4, Keys.NumPad6, Keys.NumPad7, Keys.NumPad9,
                        Keys.NumPad0, Keys.NumPad1, Keys.Escape};
                case ControlMode.WASD:
                default:
                    return new Keys[] {Keys.W, Keys.S, Keys.A,
                        Keys.D, Keys.Q, Keys.E,
                        Keys.Space, Keys.Z, Keys.Escape};
            }
        }

        public Camera.FollowMode getFollowMode()
        {
            return followMode;
        }

        public void switchFollowMode(int change)
        {
            int i = (int)followMode + change;
            i %= 3;
            if (i < 0)
                i += 3;

            followMode = (Camera.FollowMode)i;
        }

        public void switchControlMode()
        {
            controlMode = (ControlMode)(1 - (int)controlMode);
        }
    }
}
