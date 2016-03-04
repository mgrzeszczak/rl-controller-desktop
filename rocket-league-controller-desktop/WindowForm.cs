using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using rlController;

namespace rlc_gui
{
    public partial class Window : Form
    {
        private bool serverStatus = false;
        private ServerImproved server;
        private Settings settings;

        public Window()
        {
            InitializeComponent();
            MaximizeBox = false;
            init();
        }

        public void updateController(int which, bool active)
        {
            Panel p = which == 0 ? controllerStatus1 : controllerStatus2;
            p.BackColor = active ? Color.Green : Color.Red;
        }

        private void init()
        {
            initComboBoxes();
            settings = Settings.loadSettings();
            idComboBox1.SelectedIndex = settings.controllerIds[0];
            idComboBox2.SelectedIndex = settings.controllerIds[1];
            twoControllersCheckBox.Checked = settings.twoControllers;
            controller2GroupBox.Enabled = settings.twoControllers;
            controllerStatus1.BackColor = Color.Red;
            controllerStatus2.BackColor = Color.Red;
            server = new ServerImproved(this,settings.port);
        }

        private void initComboBoxes()
        {
            var values1 = new List<ComboBoxValue>();
            var values2 = new List<ComboBoxValue>();
            for (int i = 1; i <= 16; i++)
            {
                values1.Add(new ComboBoxValue((uint)i));
                values2.Add(new ComboBoxValue((uint)i));
            }
            idComboBox1.DataSource = values1;
            idComboBox2.DataSource = values2;
            idComboBox1.DisplayMember = "Value";
            idComboBox1.ValueMember = "Value";
            idComboBox2.DisplayMember = "Value";
            idComboBox2.ValueMember = "Value";
        }

        private void toggleComponents(bool b)
        {
            twoControllersCheckBox.Enabled = b;
            idComboBox1.Enabled = b;
            idComboBox2.Enabled = b;
        }

        public void changeServerStatus(bool b)
        {
            serverStatusLabel.Text = b ? "On" : "Off";
        }

        private void err(string errorText)
        {
            MessageBox.Show(errorText, "Error");
        }

        private void onOffButton_Click(object sender, EventArgs e)
        {
            if (twoControllersCheckBox.Checked && (idComboBox1.SelectedValue.Equals(idComboBox2.SelectedValue)))
            {
                Console.WriteLine("test");
                err("Controller ids must be different.");
                return;
            }
            onOffButton.Enabled = false;
            if (!serverStatus)
            {
                try
                {
                    List<uint> uints = new List<uint>();
                    uints.Add((uint)idComboBox1.SelectedValue);
                    if (twoControllersCheckBox.Checked) uints.Add((uint)idComboBox2.SelectedValue);
                    server.start(uints);
                    serverStatusLabel.Text = "On";
                    onOffButton.Text = "Stop";
                    toggleComponents(false);
                    serverStatus = true;
                } catch (Exception ex)
                {
                    err(ex.Message);
                    server.freeControllers();
                }
            } else
            {
                server.stop();
                onOffButton.Text = "Start";
                toggleComponents(true);
                serverStatus = false;
            }
            onOffButton.Enabled = true;
        }

        private void twoControllersCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (twoControllersCheckBox.Checked) controller2GroupBox.Enabled = true;
            else controller2GroupBox.Enabled = false;
            if (settings != null)
            {
                settings.twoControllers = twoControllersCheckBox.Checked;
                settings.saveSettings();
            }
        }

        private void idComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            settings.controllerIds[1] = idComboBox2.SelectedIndex;
            if (settings!=null) settings.saveSettings();
        }

        private void idComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            settings.controllerIds[0] = idComboBox1.SelectedIndex;
            if (settings!=null) settings.saveSettings();
        }
    }

    public class ComboBoxValue
    {
        public ComboBoxValue(uint value)
        {
            Value = value;
        }
        public uint Value { get; set; }
    }
}
