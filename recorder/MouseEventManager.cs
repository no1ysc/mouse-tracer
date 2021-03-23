using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace recorder
{
    public enum MouseEvent
    {
        Click,
        DoubleClick,
        Wheel,
        Move,
    }

    public class MouseEventManager
    {
        //long ms = 0;
        //string eventType;
        //Point position;

        //string prev;
        //string current;
        public MouseEventManager()
        {
            hook = Hook.GlobalEvents();

            hook.MouseClick += GrobalEventMouseClick;
            hook.MouseDoubleClick += GrobalEventMouseDoubleClick;
            hook.MouseWheel += GrobalEventMouseWheel;
            hook.MouseMove += GrobalEventMouseMove;
        }

        ~MouseEventManager()
        {
            Close();
        }

        public int CurrentX { get; private set; } = 0;
        public int CurrentY { get; private set; } = 0;

        public void Start(string filePath)
        {
            streamWriter = new StreamWriter(filePath);
            startTime = DateTime.Now;
            IsRecording = true;
        }

        public void Stop()
        {
            IsRecording = false;
            streamWriter.Close();
        }


        public void Close()
        {
            hook.MouseClick -= GrobalEventMouseClick;
            hook.MouseDoubleClick -= GrobalEventMouseDoubleClick;
            hook.MouseWheel -= GrobalEventMouseWheel;
            hook.MouseMove -= GrobalEventMouseMove;
            streamWriter.Close();
        }

        private StreamWriter streamWriter;
        private IKeyboardMouseEvents hook;
        private DateTime startTime;
        public bool IsRecording { get; private set; } = false;

        private void Logging(MouseEvent me, int x, int y)
        {
            CurrentX = x;
            CurrentY = y;

            if (IsRecording) 
            {
                long deltaAfterStartingMiliseconds = DateTime.Now.Ticks - startTime.Ticks;
                streamWriter.WriteLine($"{deltaAfterStartingMiliseconds}, {me}, {x}, {y}");
            }
        }

        private void GrobalEventMouseMove(object sender, MouseEventArgs e)
        {
            Logging(MouseEvent.Move, e.X, e.Y);
        }

        private void GrobalEventMouseWheel(object sender, MouseEventArgs e)
        {
            Logging(MouseEvent.Wheel, e.X, e.Y);
        }

        private void GrobalEventMouseDoubleClick(object sender, MouseEventArgs e)
        {
            Logging(MouseEvent.DoubleClick, e.X, e.Y);
        }

        private void GrobalEventMouseClick(object sender, MouseEventArgs e)
        {
            Logging(MouseEvent.Click, e.X, e.Y);
        }
    }
}
