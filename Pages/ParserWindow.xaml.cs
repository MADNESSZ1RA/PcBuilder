using System;
using System.Diagnostics;
using System.Windows;

namespace PcBuilder.Pages
{
    public partial class ParserWindow : Window
    {
        public ParserWindow()
        {
            InitializeComponent();
        }

        public void OpenPage(Uri url)
        {
            WebView.Source = url;
        }
    }
}
