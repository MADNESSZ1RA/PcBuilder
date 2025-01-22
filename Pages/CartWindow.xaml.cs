using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PcBuilder.Pages
{
    public partial class CartWindow : Window
    {
        public CartWindow()
        {
            InitializeComponent();
        }

        public void ShowCart(string Content)
        {
            CartTextBox.Text = Content;
        }

        public void check_compatibility(object sender, EventArgs args)
        {
            ChechCpuMotherBoard();
        }

        public void ChechCpuMotherBoard()
        {
            string processor = GetComponentName("Процессор");
            string motherboard = GetComponentName("Материнская плата");
        }

        public string GetComponentName(string componentType)
        {
            string cartContent = CartTextBox.Text;

            string searchPattern = $@"{componentType}:(.*)";

            var match = System.Text.RegularExpressions.Regex.Match(cartContent, searchPattern);

            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }

            return string.Empty;
        }

        public void GetMatchFromDatabase(string NameOne, string NameTwo)
        {

        }
    }
}
