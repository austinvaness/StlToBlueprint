using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
      <Id Type=""MyObjectBuilder_ShipBlueprintDefinition"" Subtype=""{name}""/>
      <DisplayName>avaness</DisplayName>
      <CubeGrids>
        <CubeGrid>
          <SubtypeName/>
          <EntityId>{id}</EntityId>
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
          <GridSizeEnum>{type}</GridSizeEnum>
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

        private readonly ArmorCube cube = new ArmorCube();
        private StreamWriter text;
        private float res = 0.01f;
        private Mesh m;
        private readonly Counter blockCount = new Counter();
        private string output;
        private bool hollow;
        private bool slopes;
        private readonly Random rand = new Random();

        public Main ()
        {
            InitializeComponent();
        }

        private void Main_Load (object sender, EventArgs e)
        {
            lblFile.Text = "";
            lblTris.Text = "";
            lblInfo.Text = "";
            output = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string se = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + "\\SpaceEngineers\\Blueprints\\local";
            if(Directory.Exists(se))
                output = se;
            folderDialog.SelectedPath = output;
            lblOutput.Text = output;
            comboType.SelectedIndex = 0;
            comboSkin.SelectedIndex = 0;

            background.DoWork += Start;
            background.RunWorkerCompleted += End;
            background.ProgressChanged += During;
            hollow = chkHollow.Checked;
            slopes = chkSlopes.Checked;
        }

        void Process (Mesh m, float c, bool hollow, bool slopes, BackgroundWorker worker, DoWorkEventArgs e)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Vector3I size = Vector3.Floor(m.Bounds.size / c) + 1;
            BitArray grid = new BitArray(size.x, size.y, size.z);

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            Vector3 min = m.Bounds.min;
            Vector3 max = m.Bounds.max;
            Vector3 p = min;
            int x = 0;
            for (p.x = min.x; p.x < max.x; p.x += c)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                float percent = Math.Min(x / (float)size.x, 1);
                worker.ReportProgress((int)(percent * 70), $"Building point cloud... {(int)(percent * 100)}% - Points: {grid.Count}");
                int y = 0;
                for (p.y = min.y; p.y < max.y; p.y += c)
                {
                    if (worker.CancellationPending)
                        break;
                    float minZ = m.Bounds.min.z;
                    Parallel.For(0, size.z, (i) => {
                        if (m.ContainsPoint(new Vector3(p.x, p.y, minZ + (c * i))))
                            WriteCube(grid, x, y, i);
                            //Write(cube.Replace("{x}", x.ToString()).Replace("{y}", y.ToString()).Replace("{z}", i.ToString()));
                    });
                    y++;
                }
                x++;
            }

            blockCount.Value = 0;
            worker.ReportProgress(70, "Processing points... 0% - Blocks: 0");

            string name = GetName();
            byte [] randId = new byte [8];
            rand.NextBytes(randId);
            string header = Main.header.Replace("{name}", name).Replace("{id}", BitConverter.ToInt64(randId, 0).ToString());
            if (this.cube.Large)
                header = header.Replace("{type}", "Large");
            else
                header = header.Replace("{type}", "Small");
            string tempFile = Path.GetTempFileName();
            text = File.CreateText(tempFile);
            text.Write(header);

            TimeSpan pointsCreatedTimeSpan = stopWatch.Elapsed;
            stopWatch.Restart();

            for (x = 0; x < size.x; x++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    text.Close();
                    File.Delete(tempFile);
                    return;
                }
                float percent = x / (float)size.x;
                worker.ReportProgress(70 + (int)(percent * 30), $"Processing points... {(int)(percent * 100)}% - Blocks: {blockCount.Value}");
                for (int y = 0; y < size.y; y++)
                {
                    if (worker.CancellationPending)
                        break;
                    Parallel.For(0, size.z, (i) => {
                        Write(grid, x, y, i, hollow, slopes, c, m);
                    });
                }
            }
            TimeSpan blocksCreatedTimeSpan = stopWatch.Elapsed;
            stopWatch.Stop();

            string pointsCreatedElapsedString = String.Format("    Point cloud took:{0:00}:{1:00}:{2:00}.{3:00}",
           pointsCreatedTimeSpan.Hours, pointsCreatedTimeSpan.Minutes, pointsCreatedTimeSpan.Seconds,
           pointsCreatedTimeSpan.Milliseconds / 10);
            string blocksCreatedElapsedString = String.Format("    Block placement took:{0:00}:{1:00}:{2:00}.{3:00}",
           blocksCreatedTimeSpan.Hours, blocksCreatedTimeSpan.Minutes, blocksCreatedTimeSpan.Seconds,
           blocksCreatedTimeSpan.Milliseconds / 10);

            worker.ReportProgress(100, "Done! - Blocks: "
                + blockCount.Value
                + pointsCreatedElapsedString
                + blocksCreatedElapsedString);

            text.Write(Main.footer.Replace("{name}", name));
            text.Close();

            string dir = GetDirectory(name);
            Directory.CreateDirectory(dir);
            File.Delete(dir + "\\bp.sbc");
            File.Move(tempFile, dir + "\\bp.sbc");
            if (File.Exists(dir + "\\bp.sbcB5"))
                File.Delete(dir + "\\bp.sbcB5");
            MessageBox.Show("'" + name + "' blueprint complete!");
        }

        private void WriteCube (BitArray array, int x, int y, int z)
        {
            int i = array.GetIndex(x, y, z);
            lock (array)
                array [i] = true;
        }

        private void Write (BitArray grid, int x, int y, int z, bool hollow, bool slopes, float c, Mesh m)
        {
            bool xSafe = x < grid.Length(0) - 1;
            bool ySafe = y < grid.Length(1) - 1;
            bool zSafe = z < grid.Length(2) - 1;

            bool b11 = grid [x, y, z];

            bool b21 = false;
            if (ySafe)
                b21 = grid [x, y + 1, z];

            bool b31 = false;
            if (xSafe)
                b31 = grid [x + 1, y, z];

            bool b41 = false;
            if(xSafe && ySafe)
                b41 = grid [x + 1, y + 1, z];

            bool b12 = false;
            if(zSafe)
                b12 = grid [x, y, z + 1];

            bool b22 = false;
            if(ySafe && zSafe)
                b22 = grid [x, y + 1, z + 1];

            bool b32 = false;
            if(xSafe && zSafe)
                b32 = grid [x + 1, y, z + 1];

            bool b42 = false;
            if(xSafe && ySafe && zSafe)
                b42 = grid [x + 1, y + 1, z + 1];

            byte count = 0;
            if (b11)
                count++;
            if (b21)
                count++;
            if (b31)
                count++;
            if (b41)
                count++;
            if (b12)
                count++;
            if (b22)
                count++;
            if (b32)
                count++;
            if (b42)
                count++;

            Vector3I gridPos = new Vector3I(x, y, z);
            Vector3 realPos = (gridPos * c) + m.Bounds.min;
            if (count > 0)
            {
                if (count == 8 && hollow)
                    return;
            }
            else
            {
                
                if (!m.ContainsBox(new BoundingBox(realPos, realPos + c)))
                    return;
                count = 8;
            }

            ArmorCube cube = new ArmorCube(this.cube, gridPos);
            //cube.BlockColor = m.getColor(realPos + c);
            if (slopes)
            {
                if (count == 1)
                {
                    cube.BlockType = ArmorCube.Type.Corner;
                    if (b11)
                    {
                        cube.BlockForward = ArmorCube.Direction.Left;
                        //cube.BlockUp = ArmorCube.Direction.Up;
                    }
                    else if (b21)
                    {
                        //cube.BlockForward = ArmorCube.Direction.Forward;
                        cube.BlockUp = ArmorCube.Direction.Down;
                    }
                    else if (b31)
                    {
                        //cube.BlockForward = ArmorCube.Direction.Forward;
                        //cube.BlockUp = ArmorCube.Direction.Up;
                    }
                    else if (b41)
                    {
                        cube.BlockForward = ArmorCube.Direction.Right;
                        cube.BlockUp = ArmorCube.Direction.Down;
                    }
                    else if (b12)
                    {
                        cube.BlockForward = ArmorCube.Direction.Backward;
                        //cube.BlockUp = ArmorCube.Direction.Up;
                    }
                    else if (b22)
                    {
                        cube.BlockForward = ArmorCube.Direction.Left;
                        cube.BlockUp = ArmorCube.Direction.Down;
                    }
                    else if (b32)
                    {
                        cube.BlockForward = ArmorCube.Direction.Right;
                        //cube.BlockUp = ArmorCube.Direction.Up;
                    }
                    else
                    {
                        cube.BlockForward = ArmorCube.Direction.Backward;
                        cube.BlockUp = ArmorCube.Direction.Down;
                    }
                }
                else if (count == 2)
                {
                    cube.BlockType = ArmorCube.Type.Slope;
                    if (b11 && b21)
                    {
                        //cube.BlockForward = ArmorCube.Direction.Forward;
                        cube.BlockUp = ArmorCube.Direction.Right;
                    }
                    else if (b21 && b41)
                    {
                        //cube.BlockForward = ArmorCube.Direction.Forward;
                        cube.BlockUp = ArmorCube.Direction.Down;
                    }
                    else if (b41 && b31)
                    {
                        //cube.BlockForward = ArmorCube.Direction.Forward;
                        cube.BlockUp = ArmorCube.Direction.Left;
                    }
                    else if (b31 && b11)
                    {
                        //cube.BlockForward = ArmorCube.Direction.Forward;
                        //cube.BlockUp = ArmorCube.Direction.Up;
                    }
                    else if (b12 && b22)
                    {
                        cube.BlockForward = ArmorCube.Direction.Backward;
                        cube.BlockUp = ArmorCube.Direction.Right;
                    }
                    else if (b22 && b42)
                    {
                        cube.BlockForward = ArmorCube.Direction.Backward;
                        cube.BlockUp = ArmorCube.Direction.Down;
                    }
                    else if (b42 && b32)
                    {
                        cube.BlockForward = ArmorCube.Direction.Backward;
                        cube.BlockUp = ArmorCube.Direction.Left;
                    }
                    else if (b32 && b12)
                    {
                        cube.BlockForward = ArmorCube.Direction.Backward;
                        //cube.BlockUp = ArmorCube.Direction.Up;
                    }
                    else if (b11 && b12)
                    {
                        cube.BlockForward = ArmorCube.Direction.Down;
                        cube.BlockUp = ArmorCube.Direction.Right;
                    }
                    else if (b21 && b22)
                    {
                        cube.BlockForward = ArmorCube.Direction.Left;
                        cube.BlockUp = ArmorCube.Direction.Down;
                    }
                    else if (b31 && b32)
                    {
                        cube.BlockForward = ArmorCube.Direction.Right;
                        //cube.BlockUp = ArmorCube.Direction.Up;
                    }
                    else if (b41 && b42)
                    {
                        cube.BlockForward = ArmorCube.Direction.Up;
                        cube.BlockUp = ArmorCube.Direction.Left;
                    }
                    else
                    {
                        cube.BlockType = ArmorCube.Type.Block;
                    }
                }
                else if (count == 4)
                {
                    cube.BlockType = ArmorCube.Type.InvCorner;
                    if (b11 && b21 && b41 && b22)
                    {
                        cube.BlockForward = ArmorCube.Direction.Right;
                        //cube.BlockUp = ArmorCube.Direction.Up;
                    }
                    else if (b21 && b41 && b31 && b42)
                    {
                        cube.BlockForward = ArmorCube.Direction.Backward;
                        //cube.BlockUp = ArmorCube.Direction.Up;
                    }
                    else if (b41 && b31 && b11 && b32)
                    {
                        cube.BlockForward = ArmorCube.Direction.Left;
                        cube.BlockUp = ArmorCube.Direction.Down;
                    }
                    else if (b31 && b11 && b21 && b12)
                    {
                        cube.BlockForward = ArmorCube.Direction.Backward;
                        cube.BlockUp = ArmorCube.Direction.Down;
                    }
                    else if (b12 && b22 && b42 && b21)
                    {
                        //cube.BlockForward = ArmorCube.Direction.Forward;
                        //cube.BlockUp = ArmorCube.Direction.Up;
                    }
                    else if (b22 && b42 && b32 && b41)
                    {
                        cube.BlockForward = ArmorCube.Direction.Left;
                        //cube.BlockUp = ArmorCube.Direction.Up;
                    }
                    else if (b42 && b32 && b12 && b31)
                    {
                        //cube.BlockForward = ArmorCube.Direction.Forward;
                        cube.BlockUp = ArmorCube.Direction.Down;
                    }
                    else if (b32 && b12 && b22 && b11)
                    {
                        cube.BlockForward = ArmorCube.Direction.Right;
                        cube.BlockUp = ArmorCube.Direction.Down;
                    }
                    else
                    {
                        cube.BlockType = ArmorCube.Type.Block;
                    }
                }
            }

            // TODO: Check all the points to determine appropriate slope vs full block

            string s = cube.ToString();
            lock (text)
                text.Write(s);
            lock (blockCount)
                blockCount.Value++;
        }

        private bool FileReady ()
        {
            return m != null && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath) && Directory.Exists(folderDialog.SelectedPath);
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

        private void OnOpenFileClicked (object sender, EventArgs e)
        {
            if(fileDialog.ShowDialog() == DialogResult.OK)
            {
                lblFile.Text = fileDialog.SafeFileName;
                txtBlueprintName.ForeColor = Color.Black;
                txtBlueprintName.Text = Path.GetFileNameWithoutExtension(fileDialog.SafeFileName);
                m = Mesh.ParseStl(fileDialog.FileName);
                lblTris.Text = $"Triangles: {m.Triangles.Length} Edges: {m.Edges.Length} Verticies: {m.Verticies.Length}";
                if(!txtSizeX.Enabled)
                {
                    txtSizeX.Enabled = true;
                    txtSizeY.Enabled = true;
                    txtSizeZ.Enabled = true;
                    txtResolution.Enabled = true;
                    txtSizeX.Text = "100";
                }
                OnSizeXChanged(null, null);
            }
        }

        private void OnOpenFolderClicked (object sender, EventArgs e)
        {
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                output = folderDialog.SelectedPath;
                lblOutput.Text = output;
            }
        }

        private void OnStartClicked (object sender, EventArgs e)
        {
            if (background.IsBusy)
            {
                btnStart.Enabled = false;
                background.CancelAsync();
            }
            else if (FileReady() && !string.IsNullOrWhiteSpace(GetName()))
            {
                btnStart.Enabled = true;
                btnStart.Text = "Cancel";
                progressBar.Value = 0;
                lblInfo.Text = "Loading...";
                background.RunWorkerAsync();
            }
        }

        private void Start (object sender, DoWorkEventArgs e)
        {
            Process(m, res, hollow, slopes, background, e);
        }

        private void During (object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            if (e.UserState != null && e.UserState is string)
                lblInfo.Text = (string)e.UserState;
        }

        private void End (object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show(e.Error.Message);
            else if (e.Cancelled)
            {
                progressBar.Value = 0;
                lblInfo.Text = "Canceled!";
            }
            else
            {
                progressBar.Value = 100;
            }
            btnStart.Enabled = true;
            btnStart.Text = "Start";
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
            Vector3 realSize = m.Bounds.size;
            Vector3I size = new Vector3I();
            if (int.TryParse(txtSizeX.Text, out size.x))
            {
                size.x = Math.Max(size.x, 1);
                res = realSize.x / size.x;
                size.y = (int)Math.Round(realSize.y / res);
                txtSizeY.Text = size.y.ToString();
                size.z = (int)Math.Round(realSize.z / res);
                txtSizeZ.Text = size.z.ToString();
                UpdateResolutionText();
                UpdateLabelSizeM(size);
            }
            else
            {
                txtSizeX.Text = Math.Round(realSize.x / res).ToString();
            }
            txtHandlers = true;

        }

        private void OnSizeYChanged (object sender, EventArgs e)
        {
            if (!txtHandlers || m == null || string.IsNullOrWhiteSpace(txtSizeY.Text))
                return;
            txtHandlers = false;
            Vector3 realSize = m.Bounds.size;
            Vector3I size = new Vector3I();
            if (int.TryParse(txtSizeY.Text, out size.y))
            {
                size.y = Math.Max(size.y, 1);
                res = realSize.y / size.y;
                size.x = (int)Math.Round(realSize.x / res);
                txtSizeX.Text = size.x.ToString();
                size.z = (int)Math.Round(realSize.z / res);
                txtSizeZ.Text = size.z.ToString();
                UpdateResolutionText();
                UpdateLabelSizeM(size);
            }
            else
            {
                txtSizeY.Text = Math.Round(realSize.y / res).ToString();
            }
            txtHandlers = true;
        }

        private void OnSizeZChanged (object sender, EventArgs e)
        {
            if (!txtHandlers || m == null || string.IsNullOrWhiteSpace(txtSizeZ.Text))
                return;
            txtHandlers = false;
            Vector3 realSize = m.Bounds.size;
            Vector3I size = new Vector3I();
            if (int.TryParse(txtSizeZ.Text, out size.z))
            {
                size.z = Math.Max(size.z, 1);
                res = realSize.z / size.z;
                size.x = (int)Math.Round(realSize.x / res);
                txtSizeX.Text = size.x.ToString();
                size.y = (int)Math.Round(realSize.y / res);
                txtSizeY.Text = size.y.ToString();
                UpdateResolutionText();
                UpdateLabelSizeM(size);
            }
            else
            {
                txtSizeZ.Text = Math.Round(realSize.z / res).ToString();
            }
            txtHandlers = true;
        }

        private void UpdateResolutionText()
        {
            txtResolution.Text = (1 / res).ToString();
        }

        private void UpdateLabelSizeM(Vector3I size)
        {
            if (cube.Large)
                lblSizeMeters.Text = (size * 2.5f).ToString() + "m";
            else
                lblSizeMeters.Text = (size * 0.5f).ToString() + "m";
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
                float min = 1 / m.Bounds.size.Min();
                if (n < min)
                    n = min;
                res = 1 / n;
                Vector3I size = Vector3.Round(m.Bounds.size * n);
                txtSizeX.Text = size.x.ToString();
                txtSizeY.Text = size.y.ToString();
                txtSizeZ.Text = size.z.ToString();
                UpdateLabelSizeM(size);
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
            {
                btnColor.BackColor = colorDialog.Color;
                cube.BlockColor = colorDialog.Color;
            }
        }

        private void OnCubeSizeChanged (object sender, EventArgs e)
        {
            cube.Large = comboType.SelectedIndex == 0;
            if(m != null)
                UpdateLabelSizeM(Vector3.Round(m.Bounds.size / res));
        }

        private void OnSkinTypeChanged (object sender, EventArgs e)
        {
            if (comboSkin.SelectedIndex > 0)
                cube.BlockSkin = comboSkin.SelectedItem.ToString() + "_Armor";
            else
                cube.BlockSkin = null;
        }

        private void OnHollowChanged (object sender, EventArgs e)
        {
            hollow = chkHollow.Checked;
        }

        private void OnUseSlopesChanged (object sender, EventArgs e)
        {
            slopes = chkSlopes.Checked;
        }
    }
}
