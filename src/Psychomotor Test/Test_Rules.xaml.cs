using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Psychomotor_Test;
using System.IO;

namespace Psychomotor_Test
{
    public partial class Test_Rules : Window
    {
        private MediaPlayer media = new MediaPlayer();
        private bool isPlayed = false;
        public Test_Rules()
        {
            InitializeComponent();
        }

        private void Okay_Action(object sender, RoutedEventArgs e)
        {
            if (isPlayed) media.Stop();
            this.Close();
        }

        private void Sound_Action(object sender, RoutedEventArgs e)
        {
            if (isPlayed) media.Stop();
            Button selected_button = sender as Button;                                                              // Check which button was pressed
            string sender_name = selected_button.Name.Substring(2);

            string[] files = Directory.GetFiles(Sound_Test.path_to_emergency_folder);
            foreach (string file_name in files)
            {
                if (file_name.Contains(sender_name))
                {
                    media.Open(new System.Uri(file_name));
                    break;
                }
            }
            media.Play();
            isPlayed = true;
        }
    }
}
