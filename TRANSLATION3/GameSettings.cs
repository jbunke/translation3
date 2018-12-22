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
        private WindowMode windowMode;
        private HUDStatus hudStatus;

        public enum ControlMode
        {
            WASD,
            NUMPAD
        }

        public enum WindowMode
        {
            WINDOWED,
            FULLSCREEN
        }

        public enum HUDStatus
        {
            SHOW,
            HIDE
        }

        private GameSettings(Camera.FollowMode followMode, int period, 
            ControlMode controlMode, WindowMode windowMode, HUDStatus hudStatus)
        {
            this.followMode = followMode;
            this.period = period;
            this.controlMode = controlMode;
            this.windowMode = windowMode;
            this.hudStatus = hudStatus;
        }

        public static GameSettings defaultSettings()
        {
            return new GameSettings(Camera.FollowMode.STEADY,
                20, ControlMode.WASD, WindowMode.FULLSCREEN, HUDStatus.HIDE);
        }

        public int getPeriod() { return period; }

        public String gameSpeed()
        {
            switch (period)
            {
                case 10:
                    return "FASTEST";
                case 15:
                    return "FAST";
                case 20:
                    return "NORMAL";
                case 25:
                default:
                    return "SLOW";
            }
        }

        public WindowMode getWindowMode()
        {
            return windowMode;
        }

        public HUDStatus getHUDStatus()
        {
            return hudStatus;
        }

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
            controlMode = 1 - controlMode;
        }

        public void switchWindowMode()
        {
            windowMode = 1 - windowMode;
        }

        public void switchHUDStatus()
        {
            hudStatus = 1 - hudStatus;
        }

        public void switchPeriod(int change)
        {
            if (change == 1 && period != 25)
            {
                period += 5;
            } else if (change == -1 && period != 10)
            {
                period -= 5;
            }
        }
    }
}
