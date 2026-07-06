using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace FreelancerModStudio
{
    internal partial class frmPathDialog : Form
    {
        readonly TextBox _routeTextBox = new TextBox();
        readonly NumericUpDown _legNumber = new NumericUpDown();
        readonly ComboBox _usageComboBox = new ComboBox();
        readonly NumericUpDown _radius = new NumericUpDown();
        readonly TextBox _startTextBox = new TextBox();
        readonly TextBox _endTextBox = new TextBox();
        readonly CheckBox _usePoints = new CheckBox();
        readonly Button _okButton = new Button();
        readonly Button _cancelButton = new Button();

        public string RouteName { get; private set; }
        public int LegNumber { get; private set; }
        public string Usage { get; private set; }
        public int Radius { get; private set; }
        public bool HasPoints { get; private set; }
        public PathPoint StartPoint { get; private set; }
        public PathPoint EndPoint { get; private set; }

        public frmPathDialog(string defaultRouteName, int defaultLegNumber)
        {
            InitializeComponent();
            Helper.UI.ApplyFont(this);

            _routeTextBox.Text = defaultRouteName;
            _legNumber.Value = Math.Max(1, defaultLegNumber);
            _usageComboBox.SelectedItem = "patrol";
            _radius.Value = 750;
        }

        void InitializeComponent()
        {
            SuspendLayout();

            Text = "Add Path";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(360, 245);

            Label routeLabel = CreateLabel("Route name", 12, 15);
            _routeTextBox.SetBounds(120, 12, 220, 22);

            Label legLabel = CreateLabel("Leg", 12, 45);
            _legNumber.SetBounds(120, 42, 80, 22);
            _legNumber.Minimum = 1;
            _legNumber.Maximum = 999;

            Label usageLabel = CreateLabel("Usage", 12, 75);
            _usageComboBox.SetBounds(120, 72, 120, 22);
            _usageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _usageComboBox.Items.AddRange(new object[] { "patrol", "trade" });

            Label radiusLabel = CreateLabel("Radius", 12, 105);
            _radius.SetBounds(120, 102, 80, 22);
            _radius.Minimum = 1;
            _radius.Maximum = 100000;

            _usePoints.SetBounds(12, 132, 320, 22);
            _usePoints.Text = "Use start and end points";
            _usePoints.CheckedChanged += usePoints_CheckedChanged;

            Label startLabel = CreateLabel("Start", 12, 162);
            _startTextBox.SetBounds(120, 159, 220, 22);
            _startTextBox.Enabled = false;
            _startTextBox.Text = "0, 0, 0";

            Label endLabel = CreateLabel("End", 12, 192);
            _endTextBox.SetBounds(120, 189, 220, 22);
            _endTextBox.Enabled = false;
            _endTextBox.Text = "0, 0, 10000";

            _okButton.SetBounds(184, 218, 75, 23);
            _okButton.Text = "OK";
            _okButton.DialogResult = DialogResult.OK;
            _okButton.Click += okButton_Click;

            _cancelButton.SetBounds(265, 218, 75, 23);
            _cancelButton.Text = "Cancel";
            _cancelButton.DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[]
                {
                    routeLabel, _routeTextBox, legLabel, _legNumber, usageLabel, _usageComboBox,
                    radiusLabel, _radius, _usePoints, startLabel, _startTextBox, endLabel,
                    _endTextBox, _okButton, _cancelButton
                });

            AcceptButton = _okButton;
            CancelButton = _cancelButton;
            ResumeLayout(false);
        }

        static Label CreateLabel(string text, int x, int y)
        {
            Label label = new Label();
            label.SetBounds(x, y, 100, 18);
            label.Text = text;
            return label;
        }

        void usePoints_CheckedChanged(object sender, EventArgs e)
        {
            _startTextBox.Enabled = _usePoints.Checked;
            _endTextBox.Enabled = _usePoints.Checked;
        }

        void okButton_Click(object sender, EventArgs e)
        {
            RouteName = _routeTextBox.Text.Trim();
            if (RouteName.Length == 0)
            {
                MessageBox.Show("Route name is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            LegNumber = (int)_legNumber.Value;
            Usage = _usageComboBox.SelectedItem.ToString();
            Radius = (int)_radius.Value;
            HasPoints = _usePoints.Checked;

            if (!HasPoints)
            {
                return;
            }

            PathPoint start;
            PathPoint end;
            if (!TryParsePoint(_startTextBox.Text, out start) || !TryParsePoint(_endTextBox.Text, out end))
            {
                MessageBox.Show("Start and end points must use X, Y, Z format.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            if (start.DistanceTo(end) <= 0)
            {
                MessageBox.Show("Start and end points must not be identical.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            StartPoint = start;
            EndPoint = end;
        }

        static bool TryParsePoint(string text, out PathPoint point)
        {
            point = new PathPoint();
            string[] parts = text.Split(',');
            if (parts.Length != 3)
            {
                return false;
            }

            double x;
            double y;
            double z;
            if (!double.TryParse(parts[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out x) ||
                !double.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out y) ||
                !double.TryParse(parts[2].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out z))
            {
                return false;
            }

            point = new PathPoint(x, y, z);
            return true;
        }
    }

    internal struct PathPoint
    {
        public readonly double X;
        public readonly double Y;
        public readonly double Z;

        public PathPoint(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double DistanceTo(PathPoint other)
        {
            double x = other.X - X;
            double y = other.Y - Y;
            double z = other.Z - Z;
            return Math.Sqrt(x * x + y * y + z * z);
        }
    }
}
