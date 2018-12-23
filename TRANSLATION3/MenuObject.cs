using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TRANSLATION3
{
    class MenuObject : HasLocation
    {
        private bool canSelect;
        private bool isSelected;
        private Bitmap image;
        private Bitmap selectedImage;
        private Task task;
        private String set;
        private MenuFrame frame;
        private main main;

        public enum Task
        {
            UNPAUSE,
            SET_MODE,
            SET_PAUSE,
            SET_MENU,
            CLOSE,
            SWITCH_FOLLOW,
            SWITCH_CONTROLS,
            SWITCH_WINDOW,
            SWITCH_HUD,
            SWITCH_PERIOD,
            TYPE,
            SAVE_EDITOR,
            NULL
        }

        public MenuObject(bool canSelect, String text, int size,
            Point location, Task task, String set, main main)
        {
            this.canSelect = canSelect;
            this.isSelected = false;
            this.location = location;
            this.main = main;

            image = Font.VIGILANT.print(text, size,
                Color.FromArgb(255, 0, 0));

            if (canSelect)
            {
                selectedImage = Font.VIGILANT.print(
                    text, size, Color.FromArgb(255, 255, 255));
                this.task = task;
                this.set = set;
            } 
        }

        public MenuObject(Point location, Bitmap image, main main)
        {
            this.canSelect = false;
            this.isSelected = false;
            this.location = location;
            this.main = main;
            this.image = image;
        }

        public void keyHandler(KeyEventArgs k)
        {
            switch (k.KeyCode)
            {
                case Keys.Enter:
                    frame.setIsTyping(false);
                    break;
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                case Keys.Shift:
                case Keys.ShiftKey:
                    break;
                case Keys.Back:
                    switch (set)
                    {
                        case "setlevelname":
                            main.getEditor().backspaceName();
                            break;
                        case "setlevelnote":
                            main.getEditor().backspaceNote();
                            break;
                    }
                    break;
                case Keys.Space:
                case Keys.OemPeriod:
                case Keys.Oemcomma:
                case Keys.OemQuestion:
                case Keys.OemQuotes:
                    String code = "";
                    if (!k.Shift)
                    {
                        switch (k.KeyCode)
                        {
                            case Keys.Space:
                                code = " ";
                                break;
                            case Keys.OemPeriod:
                                code = ".";
                                break;
                            case Keys.Oemcomma:
                                code = ",";
                                break;
                            case Keys.OemQuestion:
                                code = "/";
                                break;
                            case Keys.OemQuotes:
                                code = "'";
                                break;
                        }
                    } else
                    {
                        switch (k.KeyCode)
                        {
                            case Keys.Space:
                                code = " ";
                                break;
                            case Keys.OemPeriod:
                                code = ">";
                                break;
                            case Keys.Oemcomma:
                                code = "<";
                                break;
                            case Keys.OemQuestion:
                                code = "?";
                                break;
                            case Keys.OemQuotes:
                                code = "\"";
                                break;
                        }
                    }

                    switch (set)
                    {
                        case "setlevelname":
                            main.getEditor().addToName(code);
                            break;
                        case "setlevelnote":
                            main.getEditor().addToNote(code);
                            break;
                    }
                    break;
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    if (!k.Shift)
                    {
                        switch (set)
                        {
                            case "setlevelname":
                                main.getEditor().addToName(
                                    k.KeyCode.ToString().Substring(1));
                                break;
                            case "setlevelnote":
                                main.getEditor().addToNote(
                                    k.KeyCode.ToString().Substring(1));
                                break;
                        }
                    }
                    break;
                default:
                    String s = k.KeyCode.ToString();
                    if (!k.Shift)
                        s = s.ToLower();
                    // TODO
                    switch (set)
                    {
                        case "setlevelname":
                            main.getEditor().addToName(s);
                            break;
                        case "setlevelnote":
                            main.getEditor().addToNote(s);
                            break;
                    }
                    break;
            }

            if (main.getMode() == Mode.MENU)
            {
                main.refreshMenuFrame();
            } else if (main.getMode() == Mode.PAUSE)
            {
                main.refreshPauseFrame();
            }
        }

        public void doTask()
        {
            if (task != Task.TYPE)
                frame.setIsTyping(false);
            switch (task)
            {
                case Task.UNPAUSE:
                    main.unpause();
                    break;
                case Task.SET_MODE:
                    Mode mode;
                    Enum.TryParse(set, out mode);
                    main.setMode(mode);
                    break;
                case Task.CLOSE:
                    main.Close();
                    break;
                case Task.SET_PAUSE:
                    if (main.getMode() != Mode.PAUSE)
                        main.setMode(Mode.PAUSE);
                    main.setPauseFrame(set);
                    break;
                case Task.SET_MENU:
                    if (main.getMode() != Mode.MENU)
                        main.setMode(Mode.MENU);
                    main.setMenuFrame(set);
                    break;
                case Task.SWITCH_CONTROLS:
                    main.getSettings().switchControlMode();
                    List<Player> players = main.getLevel().getPlayers();
                    players[0].setControls(
                        main.getSettings().getControlMode());

                    if (players.Count > 1)
                    {
                        players[1].setControls(1 - 
                        main.getSettings().getControlMode());
                    }

                    main.refreshPauseFrame();
                    break;
                case Task.SWITCH_FOLLOW:
                    main.getSettings().switchFollowMode(Int32.Parse(set));
                    main.getLevel().setCamera(
                        main.getSettings().getFollowMode());
                    main.refreshPauseFrame();
                    break;
                case Task.SWITCH_PERIOD:
                    main.getSettings().switchPeriod(Int32.Parse(set));
                    main.applySettings();
                    main.refreshPauseFrame();
                    break;
                case Task.SWITCH_WINDOW:
                    main.getSettings().switchWindowMode();
                    main.applySettings();
                    main.refreshPauseFrame();
                    break;
                case Task.SWITCH_HUD:
                    main.getSettings().switchHUDStatus();
                    main.applySettings();
                    main.refreshPauseFrame();
                    break;
                case Task.TYPE:
                    frame.setIsTyping(!frame.getIsTyping());
                    break;
                case Task.SAVE_EDITOR:
                    main.launchSaveEditor();
                    break;
            }
        }
        
        public void setFrame(MenuFrame frame)
        {
            this.frame = frame;
        }

        public bool canBeSelected()
        {
            return canSelect;
        }

        public void setSelect(bool isSelected)
        {
            this.isSelected = isSelected;
        }

        public bool isHovering(Point mousePos)
        {
            if (!canSelect)
                return false;

            int x = selectedImage.Width / 2;
            int y = selectedImage.Height / 2;

            return Math.Abs(mousePos.X - location.X) <= x &&
                Math.Abs(mousePos.Y - location.Y) <= y;
        }

        public Bitmap render()
        {
            if (canSelect && isSelected)
            {
                return selectedImage;
            } else
            {
                return image;
            }
        }
    }
}
