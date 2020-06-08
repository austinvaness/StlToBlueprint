using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
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
        private StandardMesh m;
        private readonly Counter blockCount = new Counter();
        private string output;
        private bool hollow;
        private bool slopes;
        private bool lessAccuracy;
        private bool chunkedProcessing;
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
                + "\\SpaceEngineers";
            if(Directory.Exists(se))
            {
                output = se + "\\Blueprints\\local";
                Directory.CreateDirectory(output);
            }
            folderDialog.SelectedPath = output;
            lblOutput.Text = output;
            comboType.SelectedIndex = 0;
            comboSkin.SelectedIndex = 0;

            background.DoWork += Start;
            background.RunWorkerCompleted += End;
            background.ProgressChanged += During;
            hollow = chkHollow.Checked;
            slopes = chkSlopes.Checked;
            lessAccuracy = chkAccuracy.Checked;
            chunkedProcessing = chkChunkMesh.Checked;
        }

        void Process (Mesh m, float c, bool hollow, bool slopes, bool chunkedProcessing, BackgroundWorker worker, DoWorkEventArgs e)
        {

            Vector3I size = Vector3.Floor(m.Bounds.size / c) + 1;
            BitArray grid = new BitArray(size.x, size.y, size.z);

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            if (chunkedProcessing)
            {
                worker.ReportProgress(0, $"Generating MetaGrid... {0}%");
                m = new ChunkMesh(((StandardMesh)m), new Vector3I(1, size.y, size.z),
                                    m.Bounds);
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
                worker.ReportProgress((int)(percent * GlobalConstants.processSplit), $"Building point cloud... {(int)(percent * 100)}% - Points: {grid.Count}");
                int y = 0;
                for (p.y = min.y; p.y < max.y; p.y += c)
                {
                    if (worker.CancellationPending)
                        break;
                    float minZ = m.Bounds.min.z;
                    Parallel.For(0, size.z, (i) => {
                        if (m.ContainsPoint(new Vector3(p.x, p.y, minZ + (c * i))))
                            WriteCube(grid, x, y, i);
                    });
                    y++;
                }
                x++;
            }

            blockCount.Value = 0;
            worker.ReportProgress(GlobalConstants.processSplit, "Processing points... 0% - Blocks: 0");

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

            Vector3I gridPos = new Vector3I();
            for (; gridPos.x < size.x; gridPos.x++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    text.Close();
                    File.Delete(tempFile);
                    return;
                }
                float percent = gridPos.x / (float)size.x;
                worker.ReportProgress(GlobalConstants.processSplit + (int)(percent * (100 - GlobalConstants.processSplit)), $"Processing points... {(int)(percent * 100)}% - Blocks: {blockCount.Value}");
                for (gridPos.y = 0; gridPos.y < size.y; gridPos.y++)
                {
                    if (worker.CancellationPending)
                        break;
                    Parallel.For(0, size.z, (i) => {
                        Write(grid, gridPos, i, c, hollow, slopes, m);
                    });
                }
            }

            worker.ReportProgress(100, "Done! - Blocks: " + blockCount.Value);

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

        private void Write (BitArray grid, Vector3I gridPos, int z, float c, bool hollow, bool slopes, Mesh m)
        {
            gridPos.z = z;
            if (CheckSlopes(gridPos, c,hollow, slopes, grid, out ArmorCube cube, m))
            {
                string s = cube.ToString();
                lock (text)
                    text.Write(s);
                lock (blockCount)
                    blockCount.Value++;
            }
        }

        private bool CheckSlopes(Vector3I gridPos, float c, bool hollow, bool slopes, BitArray grid, out ArmorCube cube, Mesh m)
        {
            cube = null;

            bool xSafe = gridPos.x < grid.Length(0) - 1;
            bool ySafe = gridPos.y < grid.Length(1) - 1;
            bool zSafe = gridPos.z < grid.Length(2) - 1;

            bool b11 = grid [gridPos.x, gridPos.y, gridPos.z];

            bool b21 = false;
            if (ySafe)
                b21 = grid [gridPos.x, gridPos.y + 1, gridPos.z];

            bool b31 = false;
            if (xSafe)
                b31 = grid [gridPos.x + 1, gridPos.y, gridPos.z];

            bool b41 = false;
            if (xSafe && ySafe)
                b41 = grid [gridPos.x + 1, gridPos.y + 1, gridPos.z];

            bool b12 = false;
            if (zSafe)
                b12 = grid [gridPos.x, gridPos.y, gridPos.z + 1];

            bool b22 = false;
            if (ySafe && zSafe)
                b22 = grid [gridPos.x, gridPos.y + 1, gridPos.z + 1];

            bool b32 = false;
            if (xSafe && zSafe)
                b32 = grid [gridPos.x + 1, gridPos.y, gridPos.z + 1];

            bool b42 = false;
            if (xSafe && ySafe && zSafe)
                b42 = grid [gridPos.x + 1, gridPos.y + 1, gridPos.z + 1];

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
            if (count > 0)
            {
                if (count == 8 && hollow)
                    return false;
            }
            else
            {
                Vector3 realPos = (gridPos * c) + m.Bounds.min;
                if (!lessAccuracy && !m.IntersectsBox(new BoundingBox(realPos, realPos + c)))
                    return false;
                count = 8;
            }

            cube = new ArmorCube(this.cube, gridPos);
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
                else if (count == 3)
                {
                    cube.BlockType = ArmorCube.Type.InvCorner;
                    if (b21 && b22 && (b11 || b41))
                    {
                        cube.BlockForward = ArmorCube.Direction.Right;
                        //cube.BlockUp = ArmorCube.Direction.Up;
                    }
                    else if (b41 && b42 && (b31 || b21))
                    {
                        cube.BlockForward = ArmorCube.Direction.Backward;
                        //cube.BlockUp = ArmorCube.Direction.Up;
                    }
                    else if (b31 && b32 && (b41 || b11))
                    {
                        cube.BlockForward = ArmorCube.Direction.Left;
                        cube.BlockUp = ArmorCube.Direction.Down;
                    }
                    else if (b11 && b12 && (b31 || b21))
                    {
                        cube.BlockForward = ArmorCube.Direction.Backward;
                        cube.BlockUp = ArmorCube.Direction.Down;
                    }
                    else if (b22 && b21 && (b12 || b42))
                    {
                        //cube.BlockForward = ArmorCube.Direction.Forward;
                        //cube.BlockUp = ArmorCube.Direction.Up;
                    }
                    else if (b42 && b41 && (b22 || b32))
                    {
                        cube.BlockForward = ArmorCube.Direction.Left;
                        //cube.BlockUp = ArmorCube.Direction.Up;
                    }
                    else if (b32 && b31 && (b42 || b12))
                    {
                        //cube.BlockForward = ArmorCube.Direction.Forward;
                        cube.BlockUp = ArmorCube.Direction.Down;
                    }
                    else if (b12 && b11 && (b32 || b22))
                    {
                        cube.BlockForward = ArmorCube.Direction.Right;
                        cube.BlockUp = ArmorCube.Direction.Down;
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
            return true;
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
                m = StandardMesh.ParseStl(fileDialog.FileName);
                if (m.Triangles.Length > 0)
                {
                    lblFile.Text = fileDialog.SafeFileName;
                    txtBlueprintName.ForeColor = Color.Black;
                    txtBlueprintName.Text = Path.GetFileNameWithoutExtension(fileDialog.SafeFileName);
                    lblTris.Text = $"Triangles: {m.Triangles.Length} Edges: {m.Edges.Length} Verticies: {m.Verticies.Length}";
                    if (!txtSizeX.Enabled)
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
            Process(m, res, hollow, slopes, chunkedProcessing, background, e);
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

        private void ChkChunkMesh_CheckedChanged(object sender, EventArgs e)
        {
            chunkedProcessing = chkChunkMesh.Checked;
        }

        private void chkHeavyArmor_CheckedChanged(object sender, EventArgs e)
        {
            cube.Heavy = chkHeavyArmor.Checked;
        }
    }
}
