using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stl2Blueprint
{
    public partial class Main : Form
    {

        const string header = @"<?xml version=""1.0""?>
<Definitions xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <ShipBlueprints>
    <ShipBlueprint xsi:type=""MyObjectBuilder_ShipBlueprintDefinition"">
      <Id Type=""MyObjectBuilder_ShipBlueprintDefinition"" Subtype=""TestBlueprint""/>
      <DisplayName>avaness</DisplayName>
      <CubeGrids>
        <CubeGrid>
          <SubtypeName/>
          <EntityId>118773863687514751</EntityId>
          <PersistentFlags>CastShadows InScene</PersistentFlags>
          <PositionAndOrientation>
            <Position x =""0"" y=""0"" z=""0"" />
            <Forward x=""0"" y=""0"" z=""0"" />
            <Up x=""0"" y=""0"" z=""0"" />
            <Orientation >
              <X>0</X>
              <Y>0</Y>
              <Z>0</Z>
              <W>0</W>
            </Orientation>
          </PositionAndOrientation>
          <GridSizeEnum>Large</GridSizeEnum>
          <CubeBlocks>";

        const string footer = @"
            </CubeBlocks>
          <DisplayName>{name}</DisplayName>
          <DestructibleBlocks>true</DestructibleBlocks>
          <IsRespawnGrid>false</IsRespawnGrid>
          <LocalCoordSys>0</LocalCoordSys>
          <TargetingTargets />
        </CubeGrid>
      </CubeGrids>
      <WorkshopId>0</WorkshopId>
      <OwnerSteamId>76561198082681546</OwnerSteamId>
      <Points>0</Points>
    </ShipBlueprint>
  </ShipBlueprints>
</Definitions>";

        const string cube = @"
            <MyObjectBuilder_CubeBlock xsi:type=""MyObjectBuilder_CubeBlock"">
              <SubtypeName>LargeBlockArmorBlock</SubtypeName>
              <Min x = ""{x}"" y=""{y}"" z=""{z}"" />
              <BlockOrientation Forward = ""Down"" Up=""Forward"" />
              <ColorMaskHSV x=""{h}"" y=""{s}"" z=""{v}"" />
            </MyObjectBuilder_CubeBlock>";

        StreamWriter text;
        float res = 0.01f;
        bool started = false;
        CancellationTokenSource tokenSource;
        Mesh m;
        Counter blockCount = new Counter();

        public Main ()
        {
            InitializeComponent();
        }

        private void Main_Load (object sender, EventArgs e)
        {
            lblFile.Text = "";
            lblTris.Text = "";
            lblBlockCount.Text = "";
            folderDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + "\\SpaceEngineers\\Blueprints\\local";
            lblOutput.Text = folderDialog.SelectedPath;
        }

        // Keen: MyColorPickerConstants.cs
        public Vector3 HSVToHSVOffset (Vector3 hsv)
        {
            return new Vector3(hsv.x, hsv.y - 0.8f, hsv.z - 0.55f + 0.1f);
        }

        // Keen: ColorExtensions.cs
        public Vector3 ColorToHSV (Color color)
        {
            double max = Math.Max(color.R, Math.Max(color.G, color.B));
            double min = Math.Min(color.R, Math.Min(color.G, color.B));
            double hue = color.GetHue() / 360.0;
            double saturation = max == 0 ? 0.0f : (1.0 - 1.0 * min / max);
            double value = max / byte.MaxValue;
            return new Vector3((float)hue, (float)saturation, (float)value);
        }

        void Process (Mesh m, float c, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;
            string name = GetName();
            string tempFile = Path.GetTempFileName();
            text = File.CreateText(tempFile);

            text.Write(header.Replace("{name}", name + " (Import)"));
            string footer = Main.footer.Replace("{name}", name + " (Import)");
            Vector3 hsv = HSVToHSVOffset(ColorToHSV(colorDialog.Color));
            string cube = Main.cube.Replace("{h}", hsv.x.ToString())
                .Replace("{s}", hsv.y.ToString())
                .Replace("{v}", hsv.z.ToString());

            Vector3 min = m.Min;
            Vector3 max = m.Max;
            Vector3 p = min;
            int x = 0;
            float totalX = (int)((m.Size.x / c) + 1);
            blockCount.Value = 0;
            lblBlockCount.Text = "0";
            for (p.x = min.x; p.x < max.x; p.x += c)
            {
                if (token.IsCancellationRequested)
                {
                    text.Close();
                    File.Delete(tempFile);
                    return;
                }
                float percent = Math.Min(x / totalX, 1);
                progressBar.Value = (int)(percent * 99);
                if(percent > 0)
                {
                    long estBlocks = (long)(blockCount.Value / (double)percent);
                    lblBlockCount.Text = $"Blocks: {blockCount.Value} - {estBlocks}";
                }
                else
                {
                    lblBlockCount.Text = "Blocks: 0";
                }
                int y = 0;
                for (p.y = min.y; p.y < max.y; p.y += c)
                {
                    if (token.IsCancellationRequested)
                        break;
                    int loops = (int)((m.Size.z / c) + 1);
                    float minZ = m.Min.z;
                    Parallel.For(0, loops, (i) => {
                        if (m.ContainsPoint(new Vector3(p.x, p.y, minZ + (c * i))))
                            Write(cube.Replace("{x}", x.ToString()).Replace("{y}", y.ToString()).Replace("{z}", i.ToString()));
                    });
                    y++;
                }
                x++;
            }

            lblBlockCount.Text = "Blocks: " + blockCount.Value.ToString();
            text.Write(footer);
            text.Close();

            string dir = GetDirectory(name);
            Directory.CreateDirectory(dir);
            File.Delete(dir + "\\bp.sbc");
            File.Move(tempFile, dir + "\\bp.sbc");
            if (File.Exists(dir + "\\bp.sbcB5"))
                File.Delete(dir + "\\bp.sbcB5");
            progressBar.Value = 100;
            MessageBox.Show("'" + name + "' blueprint complete!");
        }

        private bool FileReady ()
        {
            return !started && m != null && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath) && Directory.Exists(folderDialog.SelectedPath);
        }

        private string GetDirectory (string name)
        {
            string path = folderDialog.SelectedPath;
            if (!path.EndsWith("\\"))
                path += "\\";
            return path + name;
        }

        private string GetName ()
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in txtBlueprintName.Text)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ')
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private void Write (string s)
        {
            lock (text)
                text.Write(s);
            lock (blockCount)
                blockCount.Value++;
        }

        private void OnFileOpened (object sender, CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                lblFile.Text = fileDialog.SafeFileName;
                txtBlueprintName.ForeColor = Color.Black;
                txtBlueprintName.Text = Path.GetFileNameWithoutExtension(fileDialog.SafeFileName);
                m = Mesh.ParseStl(fileDialog.FileName);
                lblTris.Text = $"Triangles: {m.Triangles.Length} Edges: {m.Edges.Length} Verticies: {m.Verticies.Length}";
                txtSizeX.Enabled = true;
                txtSizeY.Enabled = true;
                txtSizeZ.Enabled = true;
                txtResolution.Enabled = true;
                txtSizeX.Text = "100";
                OnSizeXChanged(null, null);
            }
        }

        private void OnOpenFileClicked (object sender, EventArgs e)
        {
            fileDialog.ShowDialog();
        }

        private void OnFolderOpened (object sender, CancelEventArgs e)
        {
            if (!e.Cancel)
                lblOutput.Text = folderDialog.SelectedPath;
        }

        private void OnOpenFolderClicked (object sender, EventArgs e)
        {
            folderDialog.ShowDialog();
        }

        private async void OnStartClicked (object sender, EventArgs e)
        {
            if (started)
            {
                tokenSource.Cancel();
                started = false;
            }
            else if (FileReady() && !string.IsNullOrWhiteSpace(GetName()))
            {
                started = true;
                btnStart.Text = "Cancel";
                tokenSource = new CancellationTokenSource();
                CancellationToken token = tokenSource.Token;
                await Task.Run(() => Start(token), token);
                if (token.IsCancellationRequested)
                    progressBar.Value = 0;
                else
                    progressBar.Value = 100;
                
                btnStart.Text = "Start";
                started = false;
            }
        }

        private void Start (CancellationToken token)
        {
            Process(m, res, token);
        }

        private void DigitKeyFilter (object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)8 && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private bool txtHandlers = true;
        private void OnSizeXChanged (object sender, EventArgs e)
        {
            if (!txtHandlers || m == null || string.IsNullOrWhiteSpace(txtSizeX.Text))
                return;
            txtHandlers = false;
            Vector3 size = Vector3.Floor(m.Size) + 1;
            if (int.TryParse(txtSizeX.Text, out int n))
            {
                res = size.x / Math.Max(n, 1);
                txtSizeY.Text = Math.Round(size.y / res).ToString();
                txtSizeZ.Text = Math.Round(size.z / res).ToString();
                txtResolution.Text = (1 / res).ToString();
            }
            else
            {
                txtSizeX.Text = Math.Round(size.x / res).ToString();
            }
            txtHandlers = true;

        }

        private void OnSizeYChanged (object sender, EventArgs e)
        {
            if (!txtHandlers || m == null || string.IsNullOrWhiteSpace(txtSizeY.Text))
                return;
            Vector3 size = Vector3.Floor(m.Size) + 1;
            txtHandlers = false;
            if (int.TryParse(txtSizeY.Text, out int n))
            {
                res = size.y / Math.Max(n, 1);
                txtSizeX.Text = Math.Round(size.x / res).ToString();
                txtSizeZ.Text = Math.Round(size.z / res).ToString();
                txtResolution.Text = (1 / res).ToString();
            }
            else
            {
                txtSizeY.Text = Math.Round(size.y / res).ToString();
            }
            txtHandlers = true;
        }

        private void OnSizeZChanged (object sender, EventArgs e)
        {
            if (!txtHandlers || m == null || string.IsNullOrWhiteSpace(txtSizeZ.Text))
                return;
            Vector3 size = Vector3.Floor(m.Size) + 1;
            txtHandlers = false;
            if (int.TryParse(txtSizeZ.Text, out int n))
            {
                res = size.z / Math.Max(n, 1);
                txtSizeX.Text = Math.Round(size.x / res).ToString();
                txtSizeY.Text = Math.Round(size.y / res).ToString();
                txtResolution.Text = (1 / res).ToString();
            }
            else
            {
                txtSizeZ.Text = Math.Round(size.z / res).ToString();
            }
            txtHandlers = true;
        }

        private class Counter
        {
            public int Value = 0;
        }

        private void NumberKeyFilter (object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)8 && e.KeyChar != '.' && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void OnResolutionChanged (object sender, EventArgs e)
        {
            if (!txtHandlers || m == null || string.IsNullOrWhiteSpace(txtResolution.Text))
                return;
            txtHandlers = false;
            if (float.TryParse(txtResolution.Text, out float n))
            {
                float min = 1 / m.Size.Min();
                if (n < min)
                    n = min;
                res = 1 / n;
                Vector3 size = Vector3.Floor(m.Size) + 1;
                txtSizeX.Text = Math.Round(size.x * n).ToString();
                txtSizeY.Text = Math.Round(size.y * n).ToString();
                txtSizeZ.Text = Math.Round(size.z * n).ToString();
            }
            else
            {
                txtResolution.Text = (1 / res).ToString();
            }
            txtHandlers = true;
        }

        private void OnColorClicked (object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
                btnColor.BackColor = colorDialog.Color;
        }
    }
}
