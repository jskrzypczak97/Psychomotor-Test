using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

namespace Psychomotor_Test
{  
    public partial class Simple_Test : Window
    {
        private const string path_to_rules_file = "C:/Users/jskrz/Desktop/ASK/ASK PROJEKT 5/Psychomotor Test/Psychomotor Test/Rules/SimpleRules.txt";
        private const string path_to_analysis_file = "C:/Users/jskrz/Desktop/ASK/ASK PROJEKT 5/Psychomotor Test/Psychomotor Test/Analysis/Anal.txt";
        private const int SESSION_ATTEMPTS = 5;                                        // Number of attempts in single session
        private int ATTEMPTS_LEFT;                                                      // Attempts left to the end of the session
        private DispatcherTimer Timer = new DispatcherTimer();
        private Stopwatch Stopwatch = new Stopwatch();
        List<TimeSpan> Analysis = new List<TimeSpan>();                    // to collect data
        TimeSpan Random_time;
        private bool isAttempt = false;
        private bool isGreen;
        private bool isTraining;
        private bool Gas_pedal = false;
        private bool Brake_pedal = false;

        public Simple_Test()
        {
            InitializeComponent();
            
        }

        private void Begin_Action(object sender, RoutedEventArgs e)
        {
            if (((sender as Button).Content).ToString() == "Start test")
            {
                isTraining = false;
                G_Results.Visibility = Visibility.Visible;
            }
            else
            {
                isTraining = true;
                G_Results.Visibility = Visibility.Hidden;
            }
            B_Start.IsEnabled = false;
            B_Training.IsEnabled = false;
            B_Rules.IsEnabled = false;

            ATTEMPTS_LEFT = SESSION_ATTEMPTS;
            Attempt(ATTEMPTS_LEFT);
        }

        private void Attempt(int attempts_left)
        {
            if (attempts_left > 0)
            {
                Lights_on_orange();
                Random rand = new Random();                                             // Get random row and column
                Random_time = new TimeSpan(0,0,0,rand.Next(1, 2), rand.Next(0, 1000));  // 2.000sec to 6.999sec for the light to change from orange
                isGreen = Convert.ToBoolean(rand.Next(0, 2));

                Timer.Tick += Timer_Tick;
                Timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
                Timer.Start();
                Stopwatch.Start();

                ATTEMPTS_LEFT--;
            }
            else
            {
                Lights_off_green();
                Lights_off_red();
                TB_Time.Text = "0:000";
                B_Stop.IsEnabled = false;
                B_Reset.Visibility = Visibility.Visible;
                B_Analise.Visibility = Visibility.Visible;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Stopwatch.IsRunning)
            {
                TimeSpan timespan = Stopwatch.Elapsed;
                if(isAttempt) TB_Time.Text = String.Format("{0:0}:{1:000}", timespan.Seconds, timespan.Milliseconds);
                if (timespan >= Random_time && !isAttempt)
                {
                    Lights_off_orange();
                    Stopwatch.Reset();
                    isAttempt = true;
                    if (isGreen) Lights_on_green();
                    else Lights_on_red();
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.RightCtrl || e.Key == Key.LeftCtrl)
            {
                TimeSpan timespan = Stopwatch.Elapsed;

                if (e.Key == Key.RightCtrl && !Gas_pedal)
                {
                    Gas.Height = 80;
                    SkewTransform skew = new SkewTransform(-5, 0, 0, 0);
                    Gas.RenderTransform = skew;
                    Gas_pedal = true;
                    if (isAttempt)
                    {
                        if (isGreen) getResults(timespan, true);
                        else getResults(TimeSpan.Zero, false);
                    }
                }

                if (e.Key == Key.LeftCtrl && !Brake_pedal)
                {
                    Brake.Height = 80;
                    SkewTransform skew = new SkewTransform(5, 0, 0, 0);
                    Brake.RenderTransform = skew;
                    Brake_pedal = true;
                    if (isAttempt)
                    {
                        if (!isGreen) getResults(timespan, true);
                        else getResults(TimeSpan.Zero, false);
                    }
                }
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.RightCtrl && Gas_pedal)
            {
                Gas.Height = 120;
                SkewTransform sk = new SkewTransform(0, 0, 0, 0);
                Gas.RenderTransform = sk;
                Gas_pedal = false;
            }
            if (e.Key == Key.LeftCtrl && Brake_pedal)
            {
                Brake.Height = 120;
                SkewTransform sk = new SkewTransform(0, 0, 0, 0);
                Brake.RenderTransform = sk;
                Brake_pedal = false;
            }
        }

        private void getResults(TimeSpan timespan, bool isCorrect)
        {
            isAttempt = false;
            Stopwatch.Reset();

            if (!isTraining)
            {
                TextBlock TB = Analysis.Count() < 15 ?                                                                     // Going through every result label to print it in the right place
                    (TextBlock)VisualTreeHelper.GetChild(SP_Results_1, Analysis.Count()) :                                 // Takes parent's specified child and edits it
                    (TextBlock)VisualTreeHelper.GetChild(SP_Results_2, Analysis.Count() % 15);

                TB.Text = isCorrect ? String.Format("{0:0}:{1:000}", timespan.Seconds, timespan.Milliseconds) : "INCORRECT";
                TB.Visibility = Visibility.Visible;
                Analysis.Add(timespan);
            }
            else
            {
                ATTEMPTS_LEFT++;
            }

            Lights_off_red();
            Lights_off_green();
            TB_Time.Text = String.Format("{0:0}:{1:000}", timespan.Seconds, timespan.Milliseconds);

            Attempt(ATTEMPTS_LEFT);
        }

        private void Lights_on_red()
        {
            RadialGradientBrush turn_on = new RadialGradientBrush();

            GradientStop outer = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FFE53131"),
                Offset = 0.982
            };

            GradientStop middle_1 = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FFBC2929"),
                Offset = 0.754
            };

            GradientStop middle_2 = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FEC50C0C"),
                Offset = 0.427
            };

            GradientStop inner = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FFE82020"),
                Offset = 0.142
            };

            turn_on.GradientStops.Add(outer);
            turn_on.GradientStops.Add(middle_1);
            turn_on.GradientStops.Add(middle_2);
            turn_on.GradientStops.Add(inner);

            E_red.Fill = turn_on;
            Stopwatch.Start();
        }

        private void Lights_on_orange()
        {
            RadialGradientBrush turn_on = new RadialGradientBrush();

            GradientStop outer = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FFFF9300"),
                Offset = 0.818
            };

            GradientStop middle = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FFD06111"),
                Offset = 0.425
            };

            GradientStop inner = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FFCA5F14"),
                Offset = 0.062
            };

            turn_on.GradientStops.Add(outer);
            turn_on.GradientStops.Add(middle);
            turn_on.GradientStops.Add(inner);

            E_orange.Fill = turn_on;
        }

        private void Lights_on_green()
        {
            RadialGradientBrush turn_on = new RadialGradientBrush();

            GradientStop outer = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FF48C210"),
                Offset = 1.0
            };

            GradientStop middle = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FF3BA10B"),
                Offset = 0.514
            };

            GradientStop inner = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FF3F9616"),
                Offset = 0
            };

            turn_on.GradientStops.Add(outer);
            turn_on.GradientStops.Add(middle);
            turn_on.GradientStops.Add(inner);

            E_green.Fill = turn_on;
            Stopwatch.Start();
        }

        private void Lights_off_red()
        {
            RadialGradientBrush turn_off = new RadialGradientBrush();

            GradientStop outer = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FFAD3232"),
                Offset = 1
            };

            GradientStop middle_1 = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FF741212"),
                Offset = 0.983
            };

            GradientStop middle_2 = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FF541717"),
                Offset = 0.524
            };

            GradientStop inner = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FF571010"),
                Offset = 0.201
            };

            turn_off.GradientStops.Add(outer);
            turn_off.GradientStops.Add(middle_1);
            turn_off.GradientStops.Add(middle_2);
            turn_off.GradientStops.Add(inner);

            E_red.Fill = turn_off;
        }

        private void Lights_off_orange()
        {
            RadialGradientBrush turn_off = new RadialGradientBrush();

            GradientStop outer = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FF9D4E13"),
                Offset = 1
            };

            GradientStop middle = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FF944913"),
                Offset = 0.754
            };

            GradientStop inner = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FF662C14"),
                Offset = 0.191
            };

            turn_off.GradientStops.Add(outer);
            turn_off.GradientStops.Add(middle);
            turn_off.GradientStops.Add(inner);

            E_orange.Fill = turn_off;
        }

        private void Lights_off_green()
        {
            RadialGradientBrush turn_off = new RadialGradientBrush();

            GradientStop outer = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FF327A11"),
                Offset = 1.0
            };

            GradientStop middle = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FF28620D"),
                Offset = 0.449
            };

            GradientStop inner = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FF216504"),
                Offset = 0.105
            };

            turn_off.GradientStops.Add(outer);
            turn_off.GradientStops.Add(middle);
            turn_off.GradientStops.Add(inner);

            E_green.Fill = turn_off;
        }

        private void Rules_Action(object sender, RoutedEventArgs e)
        {
            Test_Rules TestRules = new Test_Rules();
            TestRules.IsEnabled = true;
            TestRules.TB_Rules.Text = File.ReadAllText(path_to_rules_file);
            TestRules.Show();
        }

        private void Stop_Action(object sender, RoutedEventArgs e)
        {
            B_Start.IsEnabled = true;
            B_Stop.IsEnabled = true;
            B_Training.IsEnabled = true;
            B_Rules.IsEnabled = true;
            B_Reset.Visibility = Visibility.Hidden;
            Lights_off_red();
            Lights_off_orange();
            Lights_off_green();
            Timer.Stop();
            Stopwatch.Reset();
            TB_Time.Text = "0:000";
            Analysis.Clear();
            TextBlock TB;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(SP_Results_1); i++)
            {
                TB = (TextBlock)VisualTreeHelper.GetChild(SP_Results_1, i);
                TB.Visibility = Visibility.Hidden;
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(SP_Results_2); i++)
            {
                TB = (TextBlock)VisualTreeHelper.GetChild(SP_Results_2, i);
                TB.Visibility = Visibility.Hidden;
            }
        }

        private void Finish_Action(object sender, RoutedEventArgs e)
        {
            Stop_Action(null, null);
            this.Close();
        }

        private void Analise_Action(object sender, RoutedEventArgs e)
        {
            StreamWriter file = new StreamWriter(path_to_analysis_file);
            foreach(TimeSpan time in Analysis) file.WriteLine(time.ToString(@"ss\.fff"));
            file.Close();
            run_cmd();
            Analysis Results = new Analysis();
            Results.IsEnabled = true;

            System.Threading.Thread.Sleep(5000);

            Results.Img_Mid.Source = new BitmapImage(new Uri(@"C:\Users\jskrz\Desktop\ASK\ASK PROJEKT 5\Psychomotor Test\Psychomotor Test\Analysis\plot.png", UriKind.Relative));
            Results.G_LeftImage.Visibility = Visibility.Hidden;
            Results.G_RightImage.Visibility = Visibility.Hidden;
            Results.G_LeftTime.Visibility = Visibility.Hidden;
            Results.G_RightTime.Visibility = Visibility.Hidden;

            Results.Show();
        }

        private void run_cmd()
        {
            string strCmdText = "/C python plot.py";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = strCmdText;
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}
