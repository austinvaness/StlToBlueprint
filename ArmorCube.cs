using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stl2Blueprint
{
    public class ArmorCube
    {
        const string header = "\n            <MyObjectBuilder_CubeBlock xsi:type=\"MyObjectBuilder_CubeBlock\">\n";
        const string footer =   "            </MyObjectBuilder_CubeBlock>\n";
        const string space =    "              ";

        public ArmorCube()
        {
            BlockSkin = null;
            Large = true;
            BlockType = Type.Block;
            BlockColor = Color.White;
            BlockPosition = new Vector3I();
        }

        public ArmorCube (string blockSkin, bool large, Type blockType, Color blockColor, Vector3I blockPosition)
        {
            BlockSkin = blockSkin;
            Large = large;
            BlockType = blockType;
            BlockColor = blockColor;
            BlockPosition = blockPosition;
        }

        public ArmorCube(ArmorCube cube)
        {

            BlockSkin = cube.BlockSkin;
            Large = cube.Large;
            type = cube.type;
            typeS = cube.typeS;
            color = cube.color;
            colorS = cube.colorS;
            pos = cube.pos;
            posS = cube.posS;
        }

        public ArmorCube(ArmorCube cube, Vector3I newPos)
        {

            BlockSkin = cube.BlockSkin;
            Large = cube.Large;
            type = cube.type;
            typeS = cube.typeS;
            color = cube.color;
            colorS = cube.colorS;
            BlockPosition = newPos;
        }

        public string BlockSkin = null;

        public bool Large = true;

        private Type type;
        private string typeS;
        public Type BlockType
        {
            get
            {
                return type;
            }
            set
            {
                switch (value)
                {
                    case Type.Block:
                        typeS = "BlockArmorBlock";
                        break;
                    case Type.Slope:
                        typeS = "BlockArmorSlope";
                        break;
                    case Type.Corner:
                        typeS = "BlockArmorCorner";
                        break;
                    case Type.InvCorner:
                        typeS = "BlockArmorCornerInv";
                        break;
                    case Type.Half:
                        typeS = "HalfArmorBlock";
                        break;
                    case Type.HalfSlope:
                        typeS = "HalfSlopeArmorBlock";
                        break;
                }
                type = value;
            }
        }

        private Color color;
        private string colorS;
        public Color BlockColor
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                Vector3 hsv = HSVToHSVOffset(ColorToHSV(value));
                colorS = $"<ColorMaskHSV x=\"" + hsv.x.ToString(CultureInfo.InvariantCulture) +
                    "\" y=\"" + hsv.y.ToString(CultureInfo.InvariantCulture) +
                    "\" z=\"" + hsv.z.ToString(CultureInfo.InvariantCulture) + "\" />\n";
            }
        }

        private Vector3I pos;
        private string posS;
        public Vector3I BlockPosition
        {
            get
            {
                return pos;
            }
            set
            {
                pos = value;
                posS = $"<Min x = \"{value.x}\" y=\"{value.y}\" z=\"{value.z}\" />\n";
            }
        }

        public Direction BlockForward = Direction.Forward;
        public Direction BlockUp = Direction.Up;

        public override string ToString ()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(header);

            sb.Append(space).Append("<SubtypeName>");
            if (Large)
                sb.Append("Large");
            else if(type != Type.HalfSlope && type != Type.Half)
                sb.Append("Small");
            sb.Append(typeS).Append("</SubtypeName>\n");

            sb.Append(space).Append(posS);

            sb.Append(space).Append("<BlockOrientation Forward=\"").Append(BlockForward).Append("\" Up=\"").Append(BlockUp).Append("\" />\n");

            if (!string.IsNullOrWhiteSpace(BlockSkin))
                sb.Append(space).Append("<SkinSubtypeId>").Append(BlockSkin).Append("</SkinSubtypeId>\n");

            sb.Append(space).Append(colorS);

            sb.Append(footer);
            return sb.ToString();
        }

        public enum Type
        {
            Block, Slope, Corner, InvCorner, Half, HalfSlope
        }

        public enum Direction
        {
            Forward, Backward, Up, Down, Left, Right
        }

        // Keen: MyColorPickerConstants.cs
        private Vector3 HSVToHSVOffset (Vector3 hsv)
        {
            return new Vector3(hsv.x, hsv.y - 0.8f, hsv.z - 0.55f + 0.1f);
        }

        // Keen: ColorExtensions.cs
        private Vector3 ColorToHSV (Color color)
        {
            double max = Math.Max(color.R, Math.Max(color.G, color.B));
            double min = Math.Min(color.R, Math.Min(color.G, color.B));
            double hue = color.GetHue() / 360.0;
            double saturation = max == 0 ? 0.0f : (1.0 - 1.0 * min / max);
            double value = max / byte.MaxValue;
            return new Vector3((float)hue, (float)saturation, (float)value);
        }
    }
}
