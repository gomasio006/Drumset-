using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WebSocket4Net;
using System.IO;
using Microsoft.Kinect;

namespace sender_csharp
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        //[要変更]ここにWebsocket プロキシサーバのURLをセットします。
        private string serverURL = "ws://white.cs.inf.shizuoka.ac.jp:10808/";
        //[要変更]ここにチャンネル文字列（半角英数字・ブラウザ側と同じ文字列）をセットします
        private string channel = "aeiou";

        private WebSocket websocket;
        private bool ready = false;



        private KinectSensor sensor;
        private Tracks tracksLeft = new Tracks(new BeatGesture());
        private Tracks tracksRight = new Tracks(new BeatGesture());
        private Drumset drumset;



        public MainWindow()
        {
            InitializeComponent();
            if (serverURL == "")
            {
                textBox1.Text = "URL不明！";
            }
            else
            {
                textBox1.Text = channel;
                websocket = new WebSocket(serverURL);
                websocket.Closed += new EventHandler(websocket_Closed);
                websocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(websocket_Error);
                websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
                websocket.Opened += new EventHandler(websocket_Opened);
                websocket.Open();



                drumset = new Drumset();
                drumset.SetElements(
                    new CircleTarget("RideCymbal", new Position3D(0.4, 0.3, 2.0), 0.3),
                    new CircleTarget("HiHat", new Position3D(-0.7, 0.1, 2.4), 0.3),
                    new CircleTarget("Drum1", new Position3D(-0.2, 0.0, 2.2), 0.3),
                    new CircleTarget("Drum2", new Position3D(0.5, 0.1, 2.6), 0.3)
                    );

                System.Console.WriteLine("Loaded");

                foreach (var potentialSensor in KinectSensor.KinectSensors)
                {
                    if (potentialSensor.Status == KinectStatus.Connected)
                    {
                        this.sensor = potentialSensor;
                        break;
                    }
                }

                if (null != this.sensor)
                {
                    // Turn on the skeleton stream to receive skeleton frames
                    this.sensor.SkeletonStream.Enable();

                    // Add an event handler to be called whenever there is new color frame data
                    this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                    // Start the sensor!
                    try
                    {
                        this.sensor.Start();
                    }
                    catch (IOException)
                    {
                        this.sensor = null;
                    }
                }

                this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
    
            
            }
        }



        static int cnt1 = 0;
        static int cnt2 = 0;

        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            Position3D right_hand_pos = null;
            Position3D right_wrist_pos = null;
            Position3D left_hand_pos = null;
            Position3D left_wrist_pos = null;
            foreach (Skeleton skeleton in skeletons)
            {
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    //余裕があれば書き変えたい
                    foreach (Joint joint in skeleton.Joints)
                    {
                        if (joint.JointType == JointType.HandRight)
                        {
                            right_hand_pos = new Position3D(joint.Position.X, joint.Position.Y, joint.Position.Z);
                        }
                        else if (joint.JointType == JointType.WristRight)
                        {
                            right_wrist_pos = new Position3D(joint.Position.X, joint.Position.Y, joint.Position.Z);
                        }
                        else if (joint.JointType == JointType.HandLeft)
                        {
                            left_hand_pos = new Position3D(joint.Position.X, joint.Position.Y, joint.Position.Z);
                        }
                        else if (joint.JointType == JointType.WristLeft)
                        {
                            left_wrist_pos = new Position3D(joint.Position.X, joint.Position.Y, joint.Position.Z);
                        }
                    }
                }
            }

            Position3D right_stick;
            Position3D left_stick;



            if (right_hand_pos != null && right_wrist_pos != null)
            {
                right_stick = Stick.GetPositionOfEndOfStickRight(right_hand_pos, right_wrist_pos);
                tracksRight.Trail(right_stick);

                Drumset.DetectResult result = drumset.Detect(tracksRight);

                if (result != null)
                {
                    System.Console.WriteLine(result);
                }

                if (cnt1 > 3)
                {
                    PositionRatio r1 = new PositionRatio(right_hand_pos);
                    PositionRatio r2 = new PositionRatio(right_stick);
                    //System.Console.WriteLine(r1.ToJSON() + "\n" + r2.ToJSON());
                    sendMessage("stick", r1.ToJSON() + "&" + r2.ToJSON() + "&R");
                    cnt1 = 0;
                }
                cnt1++;
            }
            if (left_hand_pos != null && left_wrist_pos != null)
            {
                left_stick = Stick.GetPositionOfEndOfStickLeft(left_hand_pos, left_wrist_pos);
                tracksLeft.Trail(left_stick);
                Drumset.DetectResult result = drumset.Detect(tracksLeft);

                if (result != null)
                {
                    System.Console.WriteLine(result);
                }

                if (cnt2 > 3)
                {
                    PositionRatio l1 = new PositionRatio(left_hand_pos);
                    PositionRatio l2 = new PositionRatio(left_stick);
                    //System.Consol2 =riteLine(r1.ToJSON() + "\n" + r2.ToJSON());
                    sendMessage("stick", l1.ToJSON() + "&" + l2.ToJSON() + "&L");
                    cnt2 = 0;
                }
                cnt2++;

            }

        }







        private void sendMessage(string cmd, string msg)
        {
            if (ready)
            {
                //channelを先頭に付けて送信
                websocket.Send(channel + ":" + cmd + "," + msg);
            }
        }
     
        private void websocket_Opened(object sender, EventArgs e)
        {
            //以下のブロックはスレッドセーフにGUIを扱うおまじない
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                //ここにGUI関連の処理を書く。
                button1.IsEnabled = true;
                textBlock2.Text = "接続完了";
            }));

        }

        private void websocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            ready = false;
            //以下のブロックはスレッドセーフにGUIを扱うおまじない
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                //ここにGUI関連の処理を書く。
                textBlock2.Text = "未接続";
                textBlock3.Text = "[error] " + e.Exception.Message + "\n";
                button1.IsEnabled = false;
            }));
            MessageBox.Show("予期しないエラーで通信が途絶しました。再接続には起動しなおしてください。");
        }

        private void websocket_Closed(object sender, EventArgs e)
        {
            ready = false;
            //以下のブロックはスレッドセーフにGUIを扱うおまじない
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                //ここにGUI関連の処理を書く。
                textBlock2.Text = "未接続";
                textBlock3.Text = "[closed]\n";
                button1.IsEnabled = false;
            }));
            MessageBox.Show("サーバがコネクションを切断しました。");
        }

        private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            //  データ受信(サーバで当該チャンネルのモノのみ送る処理をしているが一応チェック)
            if (e.Message.IndexOf(channel+":") == 0) 
            {
                //チャンネル名などをメッセージから削除
                string msg = e.Message.Substring(e.Message.IndexOf(":")+1);
                //カンマ区切りを配列にする方法は↓
                string[] fields = msg.Split(new char[] { ',' });
                reply(fields);

                this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    //ここにGUI関連の処理を書く。
                    //配列をループで回してスラッシュを付けて表示
                    textBlock3.Text = "";
                    foreach (string field in fields) {
                        textBlock3.Text += field + "/";
                    }
                }));
              
            }
            else if (e.Message == channel + ";") 
            {
                //ハンドシェイク完了の受信
                ready = true;
                this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    textBlock2.Text = "ハンドシェイク完了";
                    button1.IsEnabled = false;
                }));
            }
        }

        private void reply(string[] massages)
        {
            string cmd = massages[0];
            switch (cmd)
            {
                case "getProp":
                    sendProperty(); return;
            }
        }

        private void sendProperty()
        {
            string prop = "";
            foreach(Target t in drumset) {
                prop += t.ToJSON() + "&";
                System.Console.WriteLine(t.ToJSON());
            }
            prop = prop.Remove(prop.Length - 1);
            sendMessage("prop", prop);
        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox1.Text, @"^[a-zA-Z0-9]+$"))
            {
                button1.IsEnabled = false;
                channel = textBox1.Text;
                //ハンドシェイクを送信
                websocket.Send(channel + ":");
            }
            else {
                MessageBox.Show("チャンネルは半角英数字のみ！");
            }
        }
         
    }
}
