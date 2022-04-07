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
using Week13Day3Assignment.Model;
using Week13Day3Assignment.Services;

namespace Week13Day3Assignment
{
    /// <summary>
    /// Interaction logic for Form.xaml
    /// </summary>
    public partial class Form : Window
    {
        StudentDbService studentDbService;
        
        public Form()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            Start();
        }

        private void Start()
        {
            studentDbService=new StudentDbService();
            DataContext = studentDbService;
        }

        private void ButtonInsert_Click(object sender, RoutedEventArgs e)
        {
            studentDbService.Insert();
            TextBoxFirstName.Focus();
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            studentDbService.Update();
        }

        private void ButtonFirst_Click(object sender, RoutedEventArgs e)
        {
            studentDbService.First();
        }

        private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
        {
            studentDbService.Previous();
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            studentDbService.Next();
        }

        private void ButtonLast_Click(object sender, RoutedEventArgs e)
        {
            studentDbService.Last();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            studentDbService.Save();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            studentDbService.StopEditing();
        }
    }
}
