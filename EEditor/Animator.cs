﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PlayerIOClient;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;
namespace EEditor
{
    class Animator
    {
        Connection conn;

        List<Frame> frames;
        int uploaded = 0;
        System.Timers.Timer timer = new System.Timers.Timer();
        List<string[]> firstFrame = new List<string[]>();
        bool b = false;
        private static Semaphore locker;
        Frame remoteFrame;
        List<Frame> remoteFrame_;
        string levelPassword;
        bool shuffle;
        bool reversed;
        bool vertical;
        static int Gtotal;
        static int Gcurrent;
        static int Gcurrent1;
        static int Max1000 = 0;
        static int maxBlocks = 0;
        bool owner = false;
        int botid = 0;
        DateTime epochStartTime;
        System.Threading.Timer passTimer;
        DateTime start;
        object[] param;
        public static System.Windows.Forms.ProgressBar pb; //Make AnimateForm.cs' progressbar work with this upload status
        public static System.Windows.Forms.Label time;
        public static IntPtr afHandle; //Make TaskbarProgress.cs' progressbar work with this upload status
        string[] ignoreMessages = new string[] { "m", "updatemeta", "show", "hide", "k", "init2", "add", "left", "b" };
        public Animator(List<Frame> frames, Connection conn, string levelPassword, bool shuffle, bool reversed, bool vertical)
        {
            locker = new Semaphore(0, 1);
            this.frames = frames;
            this.remoteFrame_ = frames;
            this.conn = conn;
            this.levelPassword = levelPassword;
            this.shuffle = shuffle;
            this.reversed = reversed;
            this.vertical = vertical;
            conn.OnMessage += OnMessage;
            Control.CheckForIllegalCrossThreadCalls = false;

        }


        public event EventHandler<StatusChangedArgs> StatusChanged;

        protected void OnStatusChanged(string msg, DateTime epoch, bool done = false, int totallines = 0, int countedlines = 0)
        {
            if (StatusChanged != null) StatusChanged(this, new StatusChangedArgs(msg, epoch, done, totallines, countedlines));
        }

        public void Shuffle(List<string[]> l)
        {
            Random r = new Random();
            for (int i = 0; i < l.Count; ++i)
            {
                int j = r.Next(l.Count);
                string[] t = l[i];
                l[i] = l[j];
                l[j] = t;
            }
        }
        public void reverse(List<string[]> l)
        {
            l.Reverse();
        }

        public void Run()
        {

            conn.Send("init");
            locker.WaitOne();
            owner = false;
            Gcurrent1 = 0;
            //var inca = 0;
            //if (frames.Count == 1) inca = 0;
            //else inca = 1;
            List<string[]> diff = null;
            //restart:
            //for (int i = inca; i < frames.Count; i++)
            //{
            start = DateTime.Now;

            firstFrame = frames[0].Diff(remoteFrame);
            ModifyProgressBarColor.SetState(pb, 1);
            TaskbarProgress.SetState(afHandle, TaskbarProgress.TaskbarStates.Normal);
            var dt = System.DateTime.UtcNow;
            if (pb.InvokeRequired) pb.Invoke((MethodInvoker)delegate { pb.Maximum = firstFrame.Count; }); 
            else pb.Maximum = firstFrame.Count;
            epochStartTime = dt;

            stopit:
            if (frames.Count == 1)
            {
                diff = frames[0].Diff(remoteFrame);
            }
            else
            {
                //diff = frames[i].Diff(remoteFrame);
            }
            if (MainForm.userdata.drawMixed) Shuffle(diff);
            if (!MainForm.userdata.drawMixed && MainForm.userdata.reverse) diff.Reverse();
            if (!MainForm.userdata.drawMixed && MainForm.userdata.random)
            {
                diff.Sort((a, b) => (a.ToString()[0].CompareTo(b.ToString()[0])));
            }
            Gtotal = diff.Count;
            Gcurrent = 0;
            maxBlocks = 0;
            int total = diff.Count;
            //OnStatusChanged("Uploading blocks to level. (Total: " + Gcurrent1 + "/" + Gtotal + ")", dt, false, Gtotal, Gcurrent);
            TaskbarProgress.SetValue(afHandle, Gcurrent, Gtotal); //Set TaskbarProgress.cs' values
            Queue<string[]> queue = new Queue<string[]>(diff);
            List<string[]> blocks1 = new List<string[]>(diff);
            List<object[]> blocks = new List<object[]>();

            //Set AnimateForm.cs' progressbar max value
            while (queue.Count > 0)
            {
                string[] cur = queue.Dequeue();
                int x;
                int y;
                if (cur[0] != null)
                {
                    x = Convert.ToInt32(cur[0]);
                    y = Convert.ToInt32(cur[1]);
                    OnStatusChanged("", epochStartTime, false, firstFrame.Count, Gcurrent1);
                    int type = Convert.ToInt32(cur[2]);
                    int at = Convert.ToInt32(cur[3]);
                    uploaded += 1;
                    //Console.WriteLine(lastY + " " + y);
                    if (at == 0)
                    {
                        if (remoteFrame.Foreground[y, x] != type)
                        {
                            if (bdata.morphable.Contains(type))
                            {
                                b = true;
                            }
                            else if (bdata.goal.Contains(type) && type != 83 && type != 77)
                            {
                                if (cur.Length == 5)
                                {
                                    b = true;
                                }
                            }
                            else if (bdata.rotate.Contains(type) && type != 385 && type != 374)
                            {
                                if (cur.Length == 5)
                                {
                                    b = true;
                                }
                            }
                            else if (type == 385)
                            {
                                if (cur.Length == 6)
                                {
                                    b = true;
                                }
                            }
                            else if (type == 374)
                            {
                                if (cur.Length == 5)
                                {
                                    b = true;
                                }
                            }
                            else if (type == 83 || type == 77)
                            {
                                if (cur.Length == 5)
                                {
                                    b = true;
                                }
                            }
                            else if (bdata.portals.Contains(type))
                            {
                                if (cur.Length == 7)
                                {
                                    b = true;
                                }
                            }
                            else
                            {
                                b = true;
                            }
                        }
                        else if (remoteFrame.Foreground[y, x] == type)
                        {
                            if (bdata.morphable.Contains(type))
                            {
                                if (cur.Length == 5)
                                {
                                    if (remoteFrame.BlockData[y, x] != Convert.ToInt32(cur[4]))
                                    {
                                        b = true;
                                    }
                                }
                            }
                            else if (bdata.goal.Contains(type) && type != 83 && type != 77)
                            {
                                if (cur.Length == 5)
                                {
                                    if (remoteFrame.BlockData[y, x] != Convert.ToInt32(cur[4]))
                                    {
                                        b = true;
                                    }
                                }
                            }
                            else if (bdata.rotate.Contains(type) && type != 385 && type != 374)
                            {
                                if (cur.Length == 5)
                                {
                                    if (remoteFrame.BlockData[y, x] != Convert.ToInt32(cur[4]))
                                    {
                                        b = true;
                                    }
                                }
                            }
                            else if (type == 385)
                            {
                                if (cur.Length == 6)
                                {
                                    if (remoteFrame.BlockData3[y, x] != cur[5] || remoteFrame.BlockData[y, x] != Convert.ToInt32(cur[4]))
                                    {
                                        b = true;
                                    }
                                }
                            }
                            else if (type == 374)
                            {
                                if (cur.Length == 5)
                                {
                                    if (remoteFrame.BlockData3[y, x] != cur[5])
                                    {
                                        b = true;
                                    }
                                }
                            }
                            else if (type == 83 || type == 77)
                            {
                                if (cur.Length == 5)
                                {
                                    if (remoteFrame.BlockData[y, x] != Convert.ToInt32(cur[4]))
                                    {
                                        b = true;
                                    }
                                }
                            }
                            else if (bdata.portals.Contains(type))
                            {
                                if (cur.Length == 7)
                                {
                                    if (remoteFrame.BlockData[y, x] != Convert.ToInt32(cur[4]) || remoteFrame.BlockData1[y, x] != Convert.ToInt32(cur[5]) || remoteFrame.BlockData2[y, x] != Convert.ToInt32(cur[6]))
                                    {
                                        b = true;
                                    }
                                }
                            }
                        }
                    }
                    if (at == 1)
                    {
                        if (remoteFrame.Background[y, x] != type)
                        {
                            b = true;
                        }
                    }
                    if (b)
                    {
                        if (bdata.morphable.Contains(type) && type != 385)
                        {
                            param = new object[] { at, x, y, type, Convert.ToInt32(cur[4]) };
                        }
                        else if (bdata.goal.Contains(type))
                        {
                            if (cur.Length == 5)
                            {
                                param = new object[] { at, x, y, type, Convert.ToInt32(cur[4]) };
                            }
                        }
                        else if (bdata.rotate.Contains(type) && type != 385 && type != 374)
                        {
                            if (cur.Length == 5)
                            {
                                param = new object[] { at, x, y, type, Convert.ToInt32(cur[4]) };
                            }
                        }
                        else if (type == 385)
                        {
                            if (cur.Length == 6)
                            {
                                param = new object[] { at, x, y, type, cur[5], Convert.ToInt32(cur[4]) };
                            }

                        }
                        else if (type == 374)
                        {
                            if (cur.Length == 5)
                            {
                                param = new object[] { at, x, y, type, cur[4] };
                            }

                        }
                        else if (type == 77 || type == 83)
                        {
                            if (cur.Length == 5)
                            {
                                param = new object[] { at, x, y, type, int.Parse(cur[4]) };
                            }

                        }
                        else if (bdata.portals.Contains(type))
                        {
                            if (cur.Length == 7)
                            {
                                param = new object[] { at, x, y, type, Convert.ToInt32(cur[4]), Convert.ToInt32(cur[5]), Convert.ToInt32(cur[6]) };
                            }
                        }
                        else
                        {
                            if (MainForm.userdata.level.StartsWith("OW") && MainForm.userdata.levelPass.Length > 0)
                            {
                                param = new object[] { at, x, y, type };
                            }
                            else if (MainForm.userdata.level.StartsWith("OW") && MainForm.userdata.levelPass.Length == 0)
                            {
                                if (y > 4)
                                {
                                    param = new object[] { at, x, y, type };
                                }
                            }
                            else
                            {
                                param = new object[] { at, x, y, type };
                            }
                        }
                        if (conn == null)
                        {
                            OnStatusChanged("Lost connection!", DateTime.MinValue, true, Gtotal, Gcurrent);
                            return;
                        }
                        maxBlocks += 1;
                        Max1000 += 1;
                        if (MainForm.userdata.saveWorldCrew)
                        {
                            if (AnimateForm.saveRights)
                            {
                                if (Max1000 == 1000 && Max1000 < Gtotal)
                                {

                                    conn.Send("save");
                                    Max1000 = 0;
                                }
                            }

                        }
                        int progress = (int)Math.Round((double)(100 * Gcurrent) / Gtotal);
                        if (progress == 50) goto stopit;
                        else if (progress == 90) goto stopit;

                        conn.Send("b", param);
                        Thread.Sleep(MainForm.userdata.uploadDelay);




                        if (Gcurrent1 >= firstFrame.Count)
                        {
                            break;
                        }
                        else
                        {

                            queue.Enqueue(cur);
                            //if (bdata.morphable.Contains(type) || bdata.rotate.Contains(type)) Gcurrent++;
                        }

                        //queue.Enqueue(cur);
                    }

                }
                else
                {
                    if (Gcurrent1 >= firstFrame.Count)
                    {
                        break;
                    }
                    else
                    {
                        queue.Enqueue(cur);
                    }
                    OnStatusChanged("Uploading blocks to level. (Total: " + firstFrame.Count + "/" + Gcurrent + ")", epochStartTime, false, Gtotal, Gcurrent);
                    if (pb.InvokeRequired) pb.Invoke((MethodInvoker)delegate { pb.Maximum = firstFrame.Count; });
                    TaskbarProgress.SetValue(afHandle, Gcurrent, firstFrame.Count);
                    blocks.Clear();
                    break;
                }

            }

            //}
            //if (frames.Count > 1) goto restart;
            OnStatusChanged("Level upload complete!", DateTime.MinValue, true, firstFrame.Count, Gcurrent1);
            Gcurrent1 = 0;
            TaskbarProgress.SetValue(afHandle, Gcurrent1, firstFrame.Count);
            if (MainForm.userdata.saveWorldCrew)
            {
                if (AnimateForm.saveRights)
                {
                    conn.Send("save");
                }
            }
            blocks.Clear();

        }


        void OnMessage(object sender, PlayerIOClient.Message e)
        {
            if (e.Type == "b")
            {
                int layer = e.GetInt(0), x = e.GetInt(1), y = e.GetInt(2), id = e.GetInt(3);
                if (layer == 0) { remoteFrame.Foreground[y, x] = id; } else { remoteFrame.Background[y, x] = id; }
                ++Gcurrent;
                ++Gcurrent1;
                int value = Gcurrent1;
                OnStatusChanged("Uploading blocks to level. (Total: " + value + "/" + Gtotal + ")", DateTime.MinValue, false, Gtotal, Gcurrent);
                if (Convert.ToDouble(Gcurrent1) <= pb.Maximum && Convert.ToDouble(Gcurrent1) >= pb.Minimum) { if (pb.InvokeRequired) pb.Invoke((MethodInvoker)delegate { pb.Value = Gcurrent1; }); TaskbarProgress.SetValue(afHandle, Gcurrent1, firstFrame.Count); }
            }
            else if (e.Type == "br")
            {
                int x = e.GetInt(0), y = e.GetInt(1);
                remoteFrame.Foreground[y, x] = e.GetInt(2);
                remoteFrame.BlockData[y, x] = e.GetInt(3);
                ++Gcurrent;
                ++Gcurrent1;
                int value = Gcurrent1;
                OnStatusChanged("Uploading rotatable blocks to level. (Total: " + value + "/" + firstFrame.Count + ")", DateTime.MinValue, false, Gtotal, Gcurrent);
                if (Convert.ToDouble(Gcurrent1) <= pb.Maximum && Convert.ToDouble(Gcurrent1) >= pb.Minimum) { if (pb.InvokeRequired) pb.Invoke((MethodInvoker)delegate { pb.Value = Gcurrent1; }); TaskbarProgress.SetValue(afHandle, Gcurrent1, firstFrame.Count); }
            }
            else if (e.Type == "wp")
            {
                int x = e.GetInt(0), y = e.GetInt(1);
                remoteFrame.Foreground[y, x] = e.GetInt(2);
                remoteFrame.BlockData3[y, x] = e.GetString(3);
                ++Gcurrent;
                ++Gcurrent1;
                int value = Gcurrent1;
                OnStatusChanged("Uploading world portals to level. (Total: " + value + "/" + firstFrame.Count + ")", DateTime.MinValue, false, Gtotal, Gcurrent);
                if (Convert.ToDouble(Gcurrent1) <= pb.Maximum && Convert.ToDouble(Gcurrent1) >= pb.Minimum) { if (pb.InvokeRequired) pb.Invoke((MethodInvoker)delegate { pb.Value = Gcurrent1; }); TaskbarProgress.SetValue(afHandle, Gcurrent1, firstFrame.Count); }
            }
            else if (e.Type == "ts")
            {
                int x = e.GetInt(0), y = e.GetInt(1);
                remoteFrame.Foreground[y, x] = e.GetInt(2);
                remoteFrame.BlockData3[y, x] = e.GetString(3);
                remoteFrame.BlockData[y, x] = e.GetInt(4);
                ++Gcurrent;
                ++Gcurrent1;
                int value = Gcurrent1;
                OnStatusChanged("Uploading signs to level. (Total: " + value + "/" + firstFrame.Count + ")", DateTime.MinValue, false, Gtotal, Gcurrent);
                if (Convert.ToDouble(Gcurrent1) <= pb.Maximum && Convert.ToDouble(Gcurrent1) >= pb.Minimum) { if (pb.InvokeRequired) pb.Invoke((MethodInvoker)delegate { pb.Value = Gcurrent1; }); TaskbarProgress.SetValue(afHandle, Gcurrent1, firstFrame.Count); }
            }
            else if (e.Type == "bc")
            {

                int x = e.GetInt(0), y = e.GetInt(1);
                remoteFrame.Foreground[y, x] = e.GetInt(2);
                remoteFrame.BlockData[y, x] = e.GetInt(3);
                ++Gcurrent;
                ++Gcurrent1;
                int value = Gcurrent1;
                OnStatusChanged("Uploading numbered action blocks to level. (Total: " + value + "/" + firstFrame.Count + ")", DateTime.MinValue, false, Gtotal, Gcurrent);
                if (Convert.ToDouble(Gcurrent1) <= pb.Maximum && Convert.ToDouble(Gcurrent1) >= pb.Minimum) { if (pb.InvokeRequired) pb.Invoke((MethodInvoker)delegate { pb.Value = Gcurrent1; }); TaskbarProgress.SetValue(afHandle, Gcurrent1, firstFrame.Count); }
            }
            else if (e.Type == "pt")
            {
                int x = e.GetInt(0);
                int y = e.GetInt(1);
                remoteFrame.Foreground[y, x] = e.GetInt(2);
                remoteFrame.BlockData[y, x] = e.GetInt(3);
                remoteFrame.BlockData1[y, x] = e.GetInt(4);
                remoteFrame.BlockData2[y, x] = e.GetInt(5);
                ++Gcurrent;
                ++Gcurrent1;
                int value = Gcurrent1;
                OnStatusChanged("Uploading portals to level. (Total: " + value + "/" + firstFrame.Count + ")", DateTime.MinValue, false, Gtotal, Gcurrent);
                if (Convert.ToDouble(Gcurrent1) <= pb.Maximum && Convert.ToDouble(Gcurrent1) >= pb.Minimum) { if (pb.InvokeRequired) pb.Invoke((MethodInvoker)delegate { pb.Value = Gcurrent1; }); TaskbarProgress.SetValue(afHandle, Gcurrent1, firstFrame.Count); }
            }
            else if (e.Type == "bs")
            {
                int x = e.GetInt(0);
                int y = e.GetInt(1);
                remoteFrame.Foreground[y, x] = e.GetInt(2);
                remoteFrame.BlockData[y, x] = e.GetInt(3);
                ++Gcurrent;
                ++Gcurrent1;
                int value = Gcurrent1;
                OnStatusChanged("Uploading noteblocks to level. (Total: " + value + "/" + firstFrame.Count + ")", DateTime.MinValue, false, Gtotal, Gcurrent);
                if (Convert.ToDouble(Gcurrent1) <= pb.Maximum && Convert.ToDouble(Gcurrent1) >= pb.Minimum) { if (pb.InvokeRequired) pb.Invoke((MethodInvoker)delegate { pb.Value = Gcurrent1; }); TaskbarProgress.SetValue(afHandle, Gcurrent1, firstFrame.Count); }
            }
            else if (e.Type == "access")
            {
                passTimer.Dispose();
                locker.Release();
            }
            else if (e.Type == "init")
            {
                botid = e.GetInt(5);
                AnimateForm.editRights = false;
                AnimateForm.saveRights = false;
                AnimateForm.crewEdit = false;
                AnimateForm.crewWorld = false;
                remoteFrame = Frame.FromMessage(e, false);
                conn.Send("init2");
                if (frames[0].Width <= remoteFrame.Width && frames[0].Height <= remoteFrame.Height)
                {
                }
                else
                {
                    Gtotal = Gcurrent = pb.Maximum = pb.Value = 1;
                    ModifyProgressBarColor.SetState(pb, 2);
                    TaskbarProgress.SetValue(afHandle, Gcurrent, Gtotal);
                    TaskbarProgress.SetState(afHandle, TaskbarProgress.TaskbarStates.Error);
                    OnStatusChanged("Wrong level size. Please create a level with the size of " + remoteFrame.Width + "x" + remoteFrame.Height + ".", DateTime.MinValue, true, Gtotal, Gcurrent);
                    return;
                }
                if (e.GetBoolean(34))
                {

                    AnimateForm.crewWorld = true;
                    if (e.GetBoolean(14))
                    {
                        AnimateForm.crewEdit = true;
                        if (e.GetBoolean(31)) AnimateForm.saveRights = true;
                        locker.Release();
                    }
                    else
                    {
                        OnStatusChanged("Crew: You doesn't have edit rights in this world", DateTime.MinValue, true, Gtotal, Gcurrent);
                        return;
                    }
                }

                else if (e.GetString(0) != e.GetString(13))
                {
                    owner = false;
                    if (MainForm.userdata.level.StartsWith("OW") && levelPassword.Length == 0)
                    {
                        locker.Release();
                    }
                    if (MainForm.userdata.level.StartsWith("OW") && levelPassword.Length > 0)
                    {
                        conn.Send("access", levelPassword);
                        passTimer = new System.Threading.Timer(x => OnStatusChanged("Wrong level code. Please enter the right one and retry.", DateTime.MinValue, true, Gtotal, Gcurrent), null, 5000, Timeout.Infinite);
                    }
                    else if (e.GetBoolean(14))
                    {
                        locker.Release();
                    }
                    else
                    {
                        if (MainForm.userdata.saveWorldCrew)
                        {
                            OnStatusChanged("You are not the owner of this world. You can't save.", DateTime.MinValue, true, Gtotal, Gcurrent);
                            return;
                        }
                        else
                        {
                            Gtotal = Gcurrent = pb.Maximum = pb.Value = 1;
                            ModifyProgressBarColor.SetState(pb, 3);
                            TaskbarProgress.SetValue(afHandle, Gcurrent, Gtotal);
                            TaskbarProgress.SetState(afHandle, TaskbarProgress.TaskbarStates.Paused);
                            conn.Send("access", levelPassword);
                            passTimer = new System.Threading.Timer(x => OnStatusChanged("Wrong level code. Please enter the right one and retry.", DateTime.MinValue, true, Gtotal, Gcurrent), null, 5000, Timeout.Infinite);
                        }
                    }

                }
                else if (e.GetString(0) == e.GetString(13))
                {
                    if (MainForm.userdata.useColor)
                    {
                        if (MainForm.userdata.thisColor != Color.Transparent)
                        {
                            var hex = ColorTranslator.ToHtml(MainForm.userdata.thisColor);
                            conn.Send("say", "/bgcolor " + hex);
                        }
                        else
                        {
                            conn.Send("say", "/bgcolor none");
                        }
                    }
                    AnimateForm.editRights = true;
                    AnimateForm.saveRights = true;
                    owner = true;
                    locker.Release();
                }
                else if (e.GetBoolean(14))
                {
                    AnimateForm.editRights = true;
                    locker.Release();
                }

            }
            else
            {
                //locker.Release();
            }
        }
    }
    public static class upl
    {
        public static object[] blockdata { get; set; }
    }
}
