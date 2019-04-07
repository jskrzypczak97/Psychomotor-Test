using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;

namespace Psychomotor_Test
{
    public partial class Complex_Test : Window
    {
        private const string path_to_rules_file = "C:/Users/jskrz/Desktop/ASK/ASK PROJEKT 5/Psychomotor Test/Psychomotor Test/Rules/ComplexRules.txt";
        private const string path_to_analysis_file = "C:/Users/jskrz/Desktop/ASK/ASK PROJEKT 5/Psychomotor Test/Psychomotor Test/Analysis/Anal.txt";
        private const int SESSION_ATTEMPTS = 10;                                        // Number of attempts in single session
        private int ATTEMPTS_LEFT;                                                      // Attempts left to the end of the session
        private int ROW_ID = 0;                                                         // Row containing correct button in single attempt
        private int COLUMN_ID = 0;                                                      // Column containing correct button in single attempt
        private Dictionary<int, Ellipse> Lights;                                        // Used to determinate which lights have to be turned on
        private DispatcherTimer Timer = new DispatcherTimer();
        private Stopwatch Stopwatch = new Stopwatch();
        List<TimeSpan> Analysis = new List<TimeSpan>();                    // to collect data
        private bool isTraining;                                                        // training or test

        public Complex_Test()
        {
            InitializeComponent();
            Lights = new Dictionary<int, Ellipse>()
            {
                { 01, L_0x1 },{ 02, L_0x2 },{ 03, L_0x3 },{ 04, L_0x4 },{ 05, L_0x5 },{ 06, L_0x6 },{ 07, L_0x7 },{ 08, L_0x8 },
                { 10, L_1x0 },{ 20, L_2x0 },{ 30, L_3x0 },{ 40, L_4x0 },{ 50, L_5x0 },{ 60, L_6x0 },{ 70, L_7x0 },{ 80, L_8x0 }
            };
        }

        private void Begin_Action(object sender, RoutedEventArgs e)
        {
            if(((sender as Button).Content).ToString() == "Start test")
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
            if(attempts_left > 0)
            {
                Random rand = new Random();                                             // Get random row and column
                ROW_ID = rand.Next(1, 9);
                COLUMN_ID = rand.Next(1, 9);

                Lights_turn_on(ROW_ID, COLUMN_ID);

                Timer.Tick += Timer_Tick;
                Timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
                Timer.Start();
                Stopwatch.Start();

                ATTEMPTS_LEFT--;
            }
            else
            {
                Stopwatch.Reset();
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
                TB_Time.Text = String.Format("{0:0}:{1:000}", timespan.Seconds, timespan.Milliseconds);
            }
        }

        private void Panel_Action(object sender, RoutedEventArgs e)
        {
            Button selected_button = sender as Button;                                                              // Check which button was pressed
            string sender_name = selected_button.Name;
            int sender_row_id = Int32.Parse(sender_name[sender_name.Length - 1].ToString());
            int sender_column_id = Int32.Parse(sender_name[sender_name.Length - 3].ToString());

            if(sender_row_id == ROW_ID && sender_column_id == COLUMN_ID)                                            // Is it the right button?
            {
                TimeSpan timespan = Stopwatch.Elapsed;

                if (!isTraining)
                {
                    GetResults(timespan);                                                                             // Save data if it is the test
                }
                else
                {
                    ATTEMPTS_LEFT++;                                                                                // If training juts restart Stopwatch and make it infinit
                    Stopwatch.Restart();
                }

                Lights_turn_off(sender_row_id, sender_column_id);
                Attempt(ATTEMPTS_LEFT);
            }
        }

        private void GetResults(TimeSpan timespan)
        {
            Stopwatch.Stop();                                                 

            TextBlock TB = Analysis.Count() < 15 ?                                                                     // Going through every result label to print it in the right place
                (TextBlock)VisualTreeHelper.GetChild(SP_Results_1, Analysis.Count()) :                                 // Takes parent's specified child and edits it
                (TextBlock)VisualTreeHelper.GetChild(SP_Results_2, Analysis.Count() % 15);
   
            TB.Text = String.Format("{0:0}:{1:000}", timespan.Seconds, timespan.Milliseconds);
            TB.Visibility = Visibility.Visible;

            Analysis.Add(timespan);

            Stopwatch.Restart();
        }

        private void Lights_turn_on(int row, int column)
        {
            RadialGradientBrush turn_on = new RadialGradientBrush();

            GradientStop outer = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FFAE0909"),
                Offset = 1.0
            };

            GradientStop middle = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FFF92727"),
                Offset = 0.467
            };

            GradientStop inner = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FFFF5858"),
                Offset = 0.179
            };

            turn_on.GradientStops.Add(outer);
            turn_on.GradientStops.Add(middle);
            turn_on.GradientStops.Add(inner);

            Lights[row].Fill = turn_on;
            Lights[column * 10].Fill = turn_on;
        }

        private void Lights_turn_off(int row, int column)
        {
            RadialGradientBrush turn_off = new RadialGradientBrush();

            GradientStop outer = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FF6E0707"),
                Offset = 0.834
            };

            GradientStop middle = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FF972020"),
                Offset = 0.326
            };

            GradientStop inner = new GradientStop()
            {
                Color = (Color)ColorConverter.ConvertFromString("#FFA22E2E"),
                Offset = 0.0
            };

            turn_off.GradientStops.Add(outer);
            turn_off.GradientStops.Add(middle);
            turn_off.GradientStops.Add(inner);

            Lights[row].Fill = turn_off;
            Lights[column * 10].Fill = turn_off;
        }

        private void Stop_Action(object sender, RoutedEventArgs e)
        {
            B_Start.IsEnabled = true;
            B_Stop.IsEnabled = true;
            B_Training.IsEnabled = true;
            B_Rules.IsEnabled = true;
            B_Reset.Visibility = Visibility.Hidden;
            B_Analise.Visibility = Visibility.Hidden;
            Lights_turn_off(ROW_ID, COLUMN_ID);
            Timer.Stop();
            Stopwatch.Reset();
            TB_Time.Text = "0:000";
            Analysis.Clear();
            TextBlock TB;
            for(int i = 0; i < VisualTreeHelper.GetChildrenCount(SP_Results_1); i++)
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

        private void Rules_Action(object sender, RoutedEventArgs e)
        {
            Test_Rules TestRules = new Test_Rules();
            TestRules.IsEnabled = true;
            TestRules.TB_Rules.Text = File.ReadAllText(path_to_rules_file);
            TestRules.Show();
        }

        private void Finish_Action(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Analise_Action(object sender, RoutedEventArgs e)
        {
            StreamWriter file = new StreamWriter(path_to_analysis_file);
            foreach (TimeSpan time in Analysis) file.WriteLine(time.ToString(@"ss\.fff"));
            file.Close();
            run_cmd();
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
