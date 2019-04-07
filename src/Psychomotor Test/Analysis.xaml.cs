using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;


namespace Psychomotor_Test
{
    /// <summary>
    /// Interaction logic for Analysis.xaml
    /// </summary>
    public partial class Analysis : Window
    {
        public Analysis()
        {
            InitializeComponent();
        }

        private static void doPython()
        {
            ScriptEngine engine = Python.CreateEngine();
            engine.ExecuteFile("C:/Users/jskrz/Desktop/ASK/ASK PROJEKT 5/Psychomotor Test/Psychomotor Test/test.py");
        }

        private void B_Click(object sender, RoutedEventArgs e)
        {
            doPython();
        }
    }
}
