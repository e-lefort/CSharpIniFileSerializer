using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CSharpIniFileSerializer;

namespace CSharpIniFileSerializerWinFormSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //IniSettings.Load();
            PropertyGridSimpleDemoClass p = PropertyGridSimpleDemoClass.Load();
            propertyGrid1.SelectedObject = p;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //IniSettings.Save(new IniSettings());
            PropertyGridSimpleDemoClass.Save(propertyGrid1.SelectedObject as PropertyGridSimpleDemoClass);
        }
    }
}
