using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;
using System.IO;

namespace Psychomotor_Test
{
    public partial class Sound_Test : Window
    {
        public const string path_to_music_folder = "C:/Users/jskrz/Desktop/ASK/ASK PROJEKT 5/Psychomotor Test/Psychomotor Test/Music/";
        public const string path_to_emergency_folder = "C:/Users/jskrz/Desktop/ASK/ASK PROJEKT 5/Psychomotor Test/Psychomotor Test/Emergency/";
        private const string path_to_rules_file = "C:/Users/jskrz/Desktop/ASK/ASK PROJEKT 5/Psychomotor Test/Psychomotor Test/Rules/SoundRules.txt";
        private const string path_to_analysis_file = "C:/Users/jskrz/Desktop/ASK/ASK PROJEKT 5/Psychomotor Test/Psychomotor Test/Analysis/Anal.txt";
        private int SESSION_ATTEMPTS = Directory.GetFiles(path_to_music_folder).Length;                                                                         // Number of attempts in single session
        private int ATTEMPTS_LEFT;                                                                                                                              // Attempts left to the end of the session
        private int CURRENT_ATTEMPT; 
                                                                                       
        private bool isEmergencyPlaying = false;
        private bool isMusicPlaying = false;
        private bool isAttempt = false;
        private bool isTraining;
        private bool Gas_pedal = false;
        private bool Brake_pedal = false;

        private DispatcherTimer Timer = new DispatcherTimer();
        private Stopwatch Stopwatch = new Stopwatch();
        List<Attempt_data> Analysis = new List<Attempt_data>();

        public Sound_Test()
        {
            InitializeComponent();
        }

        private void Begin_Action(object sender, RoutedEventArgs e)
        {
            InitializeTestContent();
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
            Attempt();
        }

        private void InitializeTestContent()
        {
            string[] Music = Directory.GetFiles(path_to_music_folder);

            int indexof_;
            string volume, song_name, file;
            foreach (string path in Music)
            {
                file = path.Substring(path_to_music_folder.Length);
                indexof_ = file.IndexOf('_');
                volume = file.Substring(0, indexof_);
                song_name = file.Substring(indexof_ + 1);
                Attempt_data attempt = new Attempt_data(song_name, volume);
                Analysis.Add(attempt);
            }
            CURRENT_ATTEMPT = Analysis.Count - ATTEMPTS_LEFT;
            Analysis = Analysis.OrderBy(x => Guid.NewGuid()).ToList();      // Shuffling
            TB_Volume.Visibility = Visibility.Visible;
        }

        private void Attempt()
        {
            if (ATTEMPTS_LEFT > 0)
            {
                isAttempt = true;
                CURRENT_ATTEMPT = Analysis.Count - ATTEMPTS_LEFT;
                Analysis[CURRENT_ATTEMPT].Music.Volume = 100;
                Analysis[CURRENT_ATTEMPT].Music.Play();
                isMusicPlaying = true;
                TB_Volume.Text = "VOLUME: " + Analysis[CURRENT_ATTEMPT].MusicVolume;
                Timer.Tick += Timer_Tick;
                Timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
                Timer.Start();
                Stopwatch.Start();

                ATTEMPTS_LEFT--;
            }
            else
            {
                if (!isTraining)
                {
                    TB_Time.Text = "0:000";
                    B_Stop.IsEnabled = false;
                    B_Reset.Visibility = Visibility.Visible;
                    B_Analise.Visibility = Visibility.Visible;
                }
                else
                {
                    ATTEMPTS_LEFT = SESSION_ATTEMPTS;
                    Analysis.Clear();
                    InitializeTestContent();
                    Attempt();
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Stopwatch.IsRunning)
            {
                TimeSpan timespan = Stopwatch.Elapsed;
                if (timespan > Analysis[CURRENT_ATTEMPT].EmergencyStart && !isEmergencyPlaying)
                {
                    Analysis[CURRENT_ATTEMPT].Emergency.Volume = 100;
                    Analysis[CURRENT_ATTEMPT].Emergency.Play();
                    isEmergencyPlaying = true;
                    Stopwatch.Restart();
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.RightCtrl || e.Key == Key.LeftCtrl))
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
                        if (isEmergencyPlaying && Analysis[CURRENT_ATTEMPT].IsReactionCorrect("Gas"))
                        {
                            getResults(timespan, true);
                        } 
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
                        if (isEmergencyPlaying && Analysis[CURRENT_ATTEMPT].IsReactionCorrect("Brake"))
                        {
                            getResults(timespan, true);
                        }
                        else getResults(TimeSpan.Zero, false);
                    }
                }
            }
        }

        private void getResults(TimeSpan timespan, bool isCorrect)
        {
            isAttempt = false;
            Stopwatch.Reset();
            Analysis[CURRENT_ATTEMPT].Music.Stop();
            Analysis[CURRENT_ATTEMPT].Emergency.Stop();
            isEmergencyPlaying = false;
            isMusicPlaying = false;

            if (!isTraining)
            {
                TextBlock TB = CURRENT_ATTEMPT < 15 ?                                                                     // Going through every result label to print it in the right place
                    (TextBlock)VisualTreeHelper.GetChild(SP_Results_1, CURRENT_ATTEMPT) :                                 // Takes parent's specified child and edits it
                    (TextBlock)VisualTreeHelper.GetChild(SP_Results_2, CURRENT_ATTEMPT % 15);

                TB.Text = isCorrect ? String.Format("{0:0}:{1:000}", timespan.Seconds, timespan.Milliseconds) : "INCORRECT";
                TB.Visibility = Visibility.Visible;
                Analysis[CURRENT_ATTEMPT].AttemptTime = timespan;
            }


            TB_Time.Text = String.Format("{0:0}:{1:000}", timespan.Seconds, timespan.Milliseconds);

            Attempt();
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

        private void Stop_Action(object sender, RoutedEventArgs e)
        {
            B_Start.IsEnabled = true;
            B_Stop.IsEnabled = true;
            B_Training.IsEnabled = true;
            B_Rules.IsEnabled = true;
            B_Reset.Visibility = Visibility.Hidden;
            B_Analise.Visibility = Visibility.Hidden;
            TB_Volume.Visibility = Visibility.Hidden;
            Timer.Stop();
            Stopwatch.Reset();
            TB_Time.Text = "0:000";
            Analysis[CURRENT_ATTEMPT].Music.Stop();
            Analysis[CURRENT_ATTEMPT].Emergency.Stop();
            Analysis.Clear();
            isAttempt = false;
            isEmergencyPlaying = false;
            isMusicPlaying = false;
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

        private void Rules_Action(object sender, RoutedEventArgs e)
        {
            Test_Rules TestRules = new Test_Rules();
            TestRules.IsEnabled = true;
            TestRules.B_Horn.Visibility = Visibility.Visible;
            TestRules.B_Siren.Visibility = Visibility.Visible;
            TestRules.TB_Rules.Text = File.ReadAllText(path_to_rules_file);
            
            TestRules.Show();
        }

        private void Finish_Action(object sender, RoutedEventArgs e)
        {
            if(isMusicPlaying || isEmergencyPlaying) Stop_Action(null, null);
            this.Close();
        }

        private void Analise_Action(object sender, RoutedEventArgs e)
        {
            StreamWriter file = new StreamWriter(path_to_analysis_file);
            foreach (Attempt_data attempt in Analysis)
            {
                string line = attempt.AttemptTime.ToString(@"ss\.fff") + ',' + attempt.MusicVolume + ',' + attempt.EmergencyFileName;
                file.WriteLine(line);
            }
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
