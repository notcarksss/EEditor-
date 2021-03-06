﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using PlayerIOClient;
using System.Threading;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Drawing.Text;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace EEditor
{
    public partial class MainForm : Form
    {
        public static bool debug = false;
        public static string selectedAcc = "guest";
        public static userData userdata = new userData();
        private string pathSettings = Directory.GetCurrentDirectory() + @"\settings.json";
        private Dictionary<int, Bitmap> sblocks = new Dictionary<int, Bitmap>();
        private Dictionary<int, Bitmap> sblocks1 = new Dictionary<int, Bitmap>();
        private int lastblocks = 0;
        private int[] greyColor = new int[] { 255, 77, 83, 157, 311, 312, 313, 314, 315, 316, 317, 318 };
        public static Dictionary<string, accounts> accs = new Dictionary<string, accounts>();
        public static Dictionary<string, ToolStrip> tps = new Dictionary<string, ToolStrip>();
        public ToolStripComboBox cb { get { return accountsComboBox; } set { accountsComboBox = value; } }
        private int[] blocks = new int[3000];
        private int[] misc = new int[3000];
        private int[] decos = new int[3000];
        private int[] bgs = new int[3000];
        private System.Timers.Timer timer = new System.Timers.Timer(1000);
        public static int[] foregroundBMI = new int[3000];
        public static int[] miscBMI = new int[3000];
        public static int[] decosBMI = new int[3000];
        public static int[] backgroundBMI = new int[3000];
        public int pressed = 0;
        private Color backgroundColor = Color.FromArgb(71, 71, 71);
        public static List<unknownBlock> unknown = new List<unknownBlock>();
        // public static List<unknownBlock> unknown = new List<unknownBlock>();
        public static string loadData = null;
        public static int loadBid = -1;
        public ToolStripLabel fg { get { return foregroundLabel; } set { foregroundLabel = value; } }
        public ToolStripLabel bg { get { return backgroundLabel; } set { backgroundLabel = value; } }
        public ToolStripLabel pos { get { return positionLabel; } set { positionLabel = value; } }
        public ToolStripLabel rot { get { return rotationLabel; } set { rotationLabel = value; } }
        public ToolStripLabel idtarget { get { return idtargetLabel; } set { idtargetLabel = value; } }
        public ToolStripLabel txt { get { return textLabel; } set { textLabel = value; } }
        public ToolStripComboBox tsc { get { return frameSelector; } set { frameSelector = value; } }
        public static bool selectionTool = false;
        private int[,] blockInit;
        private int[,] miscInit;
        private int[,] decorInit;
        private int[,] bgInit;
        private bool starting = false;
        private bool starting1 = false;
        public static List<string> ihavethese = new List<string>();
        public static Dictionary<int, int[]> ihavetheseBid = new Dictionary<int, int[]>();
        public static List<ownedBlocks> ownedb = new List<ownedBlocks>();
        public static Dictionary<int, int> totalblocks = new Dictionary<int, int>();
        public static BrickButton selectedBrick = null;
        public static EditArea editArea;
        public static Bitmap foregroundBMD = new Bitmap(1024 * 16, 16);
        public static Bitmap miscBMD = new Bitmap(Properties.Resources.misc.Width, 16);
        public static Bitmap decosBMD = new Bitmap(Properties.Resources.BLOCKS_deco.Width, 16);
        public static Bitmap backgroundBMD = new Bitmap(Properties.Resources.BLOCKS_back.Width, 16);
        public static System.Windows.Forms.NotifyIcon notification = new System.Windows.Forms.NotifyIcon();
        ToolStripTextBox tsb = new ToolStripTextBox();
        public static bool soundsErrorShown = false;
        private bool decrease = false;
        private string searched = null;
        public static bool resethotkeys = false;
        Minimap minimap;

        public MainForm()
        {
            InitializeComponent();
            string accss = "\\accounts.json";
            if (File.Exists(pathSettings))
            {
                //var output = JObject.Parse(File.ReadAllText(pathSettings));
                userdata = JsonConvert.DeserializeObject<userData>(File.ReadAllText(pathSettings));
                if (userdata.themeBlock.IsEmpty) userdata.themeBlock = SystemColors.Window;
                if (userdata.themeBlocks.IsEmpty) userdata.themeBlocks = SystemColors.Control;
                if (userdata.themeIcons.IsEmpty) userdata.themeIcons = Color.Black;
                if (userdata.themeIconsBg.IsEmpty) userdata.themeIconsBg = Color.Transparent;
                if (userdata.themeToolbarBg.IsEmpty) userdata.themeToolbarBg = SystemColors.Control;
                if (userdata.themeToolbarText.IsEmpty) userdata.themeToolbarText = SystemColors.ControlText;
                if (userdata.themeBlockBG.IsEmpty) userdata.themeBlockBG = Color.Transparent;
                if (userdata.themeBorder.ToString() == null) userdata.themeBorder = false;
                if (userdata.drawMixed.ToString() == null) userdata.drawMixed = false;
                if (userdata.imageBackgrounds.ToString() == null) userdata.imageBackgrounds = true;
                if (userdata.imageBlocks.ToString() == null) userdata.imageBlocks = true;
                if (userdata.imageSpecialblocksMorph.ToString() == null) userdata.imageSpecialblocksMorph = false;
                if (userdata.imageSpecialblocksAction.ToString() == null) userdata.imageSpecialblocksAction = false;
                if (userdata.random.ToString() == null) userdata.random = false;
                if (userdata.reverse.ToString() == null) userdata.reverse = false;
            }
            else
            {
                userdata = new userData()
                {
                    username = "guest",
                    newestBlocks = new List<JToken>() { },
                    uploadDelay = 5,
                    brickHotkeys = " ",
                    sprayr = 5,
                    sprayp = 10,
                    confirmClose = true,
                    updateChecker = true,
                    uploadOption = 0,
                    themeBlock = SystemColors.Window,
                    themeBlocks = SystemColors.Control,
                    themeBlockBG = Color.Transparent,
                    themeIcons = Color.Black,
                    themeIconsBg = Color.Transparent,
                    themeToolbarBg = SystemColors.Control,
                    themeToolbarText = SystemColors.ControlText,
                    themeBorder = false,
                    themeClean = false,
                    imageBackgrounds = true,
                    imageBlocks = true,
                    imageSpecialblocksMorph = false,
                    imageSpecialblocksAction = false,
                    random = false,
                    reverse = false,



                };
                File.WriteAllText(pathSettings, JsonConvert.SerializeObject(userdata, Newtonsoft.Json.Formatting.Indented));
            }
            userdata.openWorld = false;
            userdata.openCodeWorld = false;
            userdata.useColor = false;
            userdata.thisColor = Color.Transparent;
            starting = true;
            starting1 = true;
            levelTextbox.Text = userdata.level;
            codeTextbox.Text = userdata.levelPass;

            if (File.Exists(Directory.GetCurrentDirectory() + accss) && File.ReadAllLines(Directory.GetCurrentDirectory() + accss).Length > 0)
            {

                var output = JObject.Parse(File.ReadAllText(Directory.GetCurrentDirectory() + accss));

                foreach (var property in output)
                {
                    if (!accs.ContainsKey(property.Key))
                    {
                        accs.Add(property.Key, new accounts() { login = property.Value["login"].ToString(), password = property.Value["password"].ToString(), loginMethod = (int)property.Value["loginMethod"], payvault = property.Value["payvault"].ToList<JToken>() });
                        accountsComboBox.Items.Add(property.Key);
                    }
                }
                if (!accs.ContainsKey("guest"))
                {
                    accs.Add("guest", new accounts() { login = "guest", password = "guest", loginMethod = 0, payvault = new List<JToken>() });
                    accountsComboBox.Items.Add("guest");
                    File.WriteAllText(Directory.GetCurrentDirectory() + accss, JsonConvert.SerializeObject(accs, Formatting.Indented));
                }
            }
            else
            {
                if (!accs.ContainsKey("guest"))
                {
                    accs.Add("guest", new accounts() { login = "guest", password = "guest", loginMethod = 0, payvault = new List<JToken>() });
                    accountsComboBox.Items.Add("guest");
                    File.WriteAllText(Directory.GetCurrentDirectory() + accss, JsonConvert.SerializeObject(accs, Formatting.Indented));
                }

            }
            accountsComboBox.Items.Add("---------");
            accountsComboBox.Items.Add("Edit accounts...");

            subButton.Enabled = false;
            frameSelector.Enabled = false;
            addButton.Enabled = false;
            unknownToolStrip.Visible = false;

            editArea = new EditArea(this);
            editArea.Dock = DockStyle.Fill;
            editArea.AutoScroll = true;
            //editArea.Size = new Size(1280,1024);
            panel1.AutoScroll = true;
            panel1.Controls.Add(editArea);
            panel1.Dock = DockStyle.Fill;
            minimap = new Minimap();
            minimap.Size = new Size(25, 25);
            minimap.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            editArea.Minimap = minimap;

            panel1.Controls.Add(minimap);
            minimap.BringToFront();

            userdata.lastSelectedBlockbar = 0;


            //Should be set to current acc index actually

            //================Background==========================
            //Load blocks with image position. blockid,imageposition
            //If there comes new backgrounds you need to add them to the list.
            #region Background

            bgInit = new int[,] {
{ 500,0}, { 501,1}, { 502,2}, { 503,3}, { 504,4}, { 505,5}, { 506,6}, { 507,7}, { 508,8}, { 509,9}, { 510,10},
{ 511,11}, { 512,12}, { 513,13}, { 514,14}, { 515,15}, { 516,16}, { 517,17}, { 518,18}, { 519,19}, { 520,20},
{ 521,21}, { 522,22}, { 523,23}, { 524,24}, { 525,25}, { 526,26}, { 527,27}, { 528,28}, { 529,29}, { 530,30},
{ 531,31}, { 532,32}, { 533,33}, { 534,34}, { 535,35}, { 536,36}, { 537,37}, { 538,38}, { 539,39}, { 540,40},
{ 541,41}, { 542,42}, { 543,43}, { 544,44}, { 545,45}, { 546,46}, { 547,47}, { 548,48}, { 549,49}, { 550,50},
{ 551,51}, { 552,52}, { 553,53}, { 554,54}, { 555,55}, { 556,56}, { 557,57}, { 558,58}, { 559,59}, { 560,60},
{ 561,61}, { 562,62}, { 563,63}, { 564,64}, { 565,65}, { 566,66}, { 567,67}, { 568,68}, { 569,69}, { 570,70},
{ 571,71}, { 572,72}, { 573,73}, { 574,74}, { 575,75}, { 576,76}, { 577,77}, { 578,78}, { 579,79}, { 580,80},
{ 581,81}, { 582,82}, { 583,83}, { 584,84}, { 585,85}, { 586,86}, { 587,87}, { 588,88}, { 589,89}, { 590,90},
{ 591,91}, { 592,92}, { 593,93}, { 594,94}, { 595,95}, { 596,96}, { 597,97}, { 598,98}, { 599,99}, { 600,100},
{ 601,101}, { 602,102}, { 603,103}, { 604,104}, { 605,105}, { 606,106}, { 607,107}, { 608,108}, { 609,109}, { 610,110},
{ 611,111}, { 612,112}, { 613,113}, { 614,114}, { 615,115}, { 616,116}, { 617,117}, { 618,118}, { 619,119}, { 620,120},
{ 621,121}, { 622,122}, { 623,123}, { 624,124}, { 625,125}, { 626,126}, { 627,127}, { 628,128}, { 629,129}, { 630,130},
{ 637,131}, { 638,132}, { 639,133}, { 640,134}, { 641,135}, { 642,136}, { 643,137}, { 644,138}, { 645,139}, { 646,140},
{ 647,141}, { 648,142}, { 649,143}, { 650,144}, { 651,145}, { 652,146}, { 653,147}, { 654,148}, { 655,149}, { 656,150},
{ 657,151}, { 658,152}, { 659,153}, { 660,154}, { 661,155}, { 662,156}, { 663,157}, { 664,158}, { 665,159}, { 666,160},
{ 667,161}, { 668,162}, { 669,163}, { 670,164}, { 671,165}, { 672,166}, { 673,167}, { 674,168}, { 675,169}, { 676,170},
{ 677, 171 },{ 539, 39 }, { 540,40 },{ 637,131 },{ 550,50 }, { 551,51 }, { 552,52 }, { 553,53 },
{ 554,54 }, { 555,55 }, { 559,59 }, { 560,60 },{ 561,61 }, { 562,62 }, { 688,182 }, { 689,183 }, { 690,184 }, { 691,185 }, { 692,186 }, { 693,187 },
{ 564,64 }, { 565,65 }, { 566,66 }, { 567,67 }, { 667,161 }, { 668,162 }, { 669,163 }, { 670,164 },
{ 568,68 }, { 569,69 }, { 570,70 }, { 571,71 }, { 572,72 }, { 573,73 },{ 574,74 }, { 575,75 }, { 576,76 }, { 577,77 }, { 578,78 },
{ 579,79 }, { 580,80 }, { 581,81 }, { 582,82 }, { 583,83 }, { 584,84 },{ 585,85 }, { 586,86 }, { 587,87 }, { 588,88 }, { 589,89 },
{ 594,94 }, { 595,95 }, { 596,96 }, { 597,97 }, { 598,98 },{ 599,99 }, { 600,100 }, { 590,90 }, { 591,91 }, { 592,92 }, { 556,56 }, { 593,93 },
{ 601,101 }, { 602,102 }, { 603,103 }, { 604,104 },{ 605, 105 }, { 673, 167 }, { 674, 168 }, { 675, 169 },
{ 608,108 }, { 609,109 }, { 663,157 }, { 664,158 }, { 665,159 }, { 666,160 },
{ 617,117 }, { 618,118 }, { 619,119 }, { 620,120 }, { 621,121 }, { 622,122 }, { 623,123 },
{ 624,124 }, { 625,125 }, { 626,126 },{ 627,127 }, { 628,128 }, { 629,129 },
{ 557,57 }, { 630,130 },{ 638,132 }, { 639,133 }, { 640,134 },{ 641,135 }, { 642,136 }, { 643,137 },
{ 655,149 }, { 656,150 }, { 657,151 }, { 658,152 }, { 659,153 }, { 660,154 }, { 661,155 }, { 662,156 }, { 663,157 }, { 664,158 }, { 665,159 }, { 666,160 },
{ 678,172 }, { 679,173 }, { 680,174 }, { 681,175 }, { 682,176 }, { 683, 177 }, { 684, 178 }, {685, 179}, {686, 180}, { 687, 181 },
{ 694,188 }, { 695,189 }, { 696,190 }, { 697, 191 }, { 698, 192}, { 699, 193}, { 700, 194 }, { 701, 195 }, { 702, 196 }, { 703, 197 }, { 709, 198 }, { 710, 199 }, { 711, 200 },
{ 704, 201 }, { 705, 202 }, { 706, 203 }, { 707, 204 }, { 708, 205 }, { 712, 206 }, { 713, 207 }, { 714, 208 },
 { 715, 209 },{ 716, 210 },{ 717, 211 },{ 718, 212 },{ 719, 213 }, { 720, 219 },
 { 721, 214 }, { 722, 215 },{ 723, 216 },{ 724, 217 },{ 725, 218 }, { 726, 220 }, { 727, 221 },
 { 728, 222 },{ 729, 223 },{ 730, 224 },{ 731, 225 },{ 732, 226 },{ 733, 227 },{ 734, 228 },
 { 735, 229 },{ 736, 230 },{ 737, 231 },{ 738, 232 },{ 739, 233 },{ 740, 234 },{ 741, 235 },{ 742, 236 },


            };
            for (int i = 0; i < bgInit.Length / 2; i++)
            {
                bgs[bgInit[i, 1]] = bgInit[i, 0]; //Add imageid and blockid (bgs[imageID] blockID)
                backgroundBMI[bgInit[i, 0]] = bgInit[i, 1]; //Add blockid and imageid (backgroundBMI[blockID] imageID)
            }
            #endregion


            //================Decorations==========================
            //Load blocks with image position. blockid,imageposition
            //If there comes new decorations you need to add them to the list.
            #region Decorations
            decorInit = new int[,] {
                { 255, 38 }, {424, 177}, { 249, 32 }, { 250, 33}, { 251, 34}, { 252, 35}, { 253, 36}, { 254, 37},
                { 244, 27 }, { 245, 28 }, { 246, 29 }, { 247, 30 }, { 248, 31 }, { 223, 6 },
                { 233, 16 }, { 234, 17 }, { 235, 18 }, { 236, 19 }, { 237, 20 }, { 238, 21 }, { 239, 22 }, { 240, 23 },
                { 256, 39 }, { 257, 40 }, { 258, 41 }, { 259, 42 }, { 260, 43 }, { 227, 10 }, { 431, 184 },  { 432, 185 }, { 433, 186 }, { 434, 187 },
                { 228 , 11 }, { 229 , 12 }, { 230 , 13 }, { 231 , 14 }, { 232 , 15 }, {  224, 7 }, {  225, 8 }, {  226, 9 }, { 218, 1 }, { 219, 2 }, { 220, 3 }, { 221, 4 }, { 222, 5 },
                { 261, 44 }, { 262, 45}, { 263, 46}, { 264, 47}, { 265, 48}, { 266, 49}, { 267, 50},  { 268, 51}, { 269, 52}, { 270, 53},
                { 271, 54}, { 272, 55}, { 435, 188}, { 436, 189}, { 276, 59 }, { 277, 60 }, { 278, 61 },  { 279, 62 }, { 280, 63 }, { 281, 64 }, { 282, 65 },  { 283, 66 },  { 284, 67 },
                { 285, 68},{ 286, 69 },{ 287, 70 },{ 288, 71 },{ 289, 72 }, { 290, 73 },{ 291, 74 },{ 292, 75 },{ 293, 76 },{ 294, 77 },{ 295, 78 }, { 296, 79 },{ 297, 80 },{ 298, 81 },{ 299, 82 },
                { 301, 83 },{ 302, 84 },{ 303, 85 },{ 304, 86 },{ 305, 87 },{ 306, 88 },{307, 89},{308, 90},{309, 91},{310, 92},
                { 311, 93 },{ 312, 94 },{ 313, 95 },{ 314, 96 },{315,  97 },{ 316, 98 },{ 317, 99 },{ 318,  100}, { 319, 101 },{ 320, 102 },{ 321, 103 },{ 322, 104 },{ 323, 105 },{ 324, 106 },
                {325, 107}, { 326, 108}, { 437, 190 }, { 330, 112 }, { 332, 114 }, { 333, 115 }, { 334, 116 }, { 335, 117 }, {428, 181 }, { 429, 182 }, { 430, 183 }, { 331, 113 },
                { 336, 118}, { 425,178 }, {426, 179 }, { 427,180 },
                { 274,57 }, { 341,122 }, { 342,123 },
                { 343,124 }, { 344,125 }, { 345,126 }, { 346, 127 }, { 347, 128 }, { 348, 129 }, { 349, 130 }, { 350, 131 }, { 351,132 },
                { 352, 133}, { 353, 134}, { 354, 135}, { 355, 136}, { 356, 137},
                { 357, 138},{ 358, 139},{ 359, 140},
                { 362, 141},{ 363, 142},{ 364, 143},{ 365, 144},{ 366, 145},{ 367, 146},
                { 398,165 },{ 399,166 },{ 400,167 },{ 401,168 },{ 402,169 },{ 403,170 },{ 404,171 },
                { 405,172 },{ 406,173 },{ 407,174 },
                { 415,175 },
                { 371, 147}, { 372, 148}, { 373, 149 },
                {382, 150}, {383, 151}, {384, 152},
                {386, 154}, {387, 155}, {388, 156}, {389, 157},
                {390, 158}, {391, 159}, {392, 160}, {393, 161}, {394, 162}, {395, 163}, {396, 164},
                {441, 191}, {442, 192}, {443, 193}, {444, 194}, {445, 195},
                { 446,196 },
                {454, 197}, {455, 198},
                { 466, 199 }, { 462, 200 }, { 463, 201 }, { 468, 202 }, { 469, 203}, { 470 , 204 },
                { 473, 205 }, { 474, 206 }, { 478, 209 }, { 479, 208 }, { 480, 207 }, { 495, 218 }, { 496, 219 },
                { 487, 213 }, { 488, 214 },{ 489, 215 },{ 490, 216 },{ 491, 217 }, { 1501, 220 },
                { 484, 212 }, { 485, 211 }, { 486, 210 }, { 1503,221  }, { 1504,222  }, { 1505,223  },
                { 1508,224 }, { 1509, 225 }, { 1511, 226 }, { 1512, 227 }, { 1513, 228 }, { 1514, 229 }, { 1515, 230 }


            };
            for (int i = 0; i < decorInit.Length / 2; i++)
            {
                decos[decorInit[i, 1]] = decorInit[i, 0]; //Add imageid and blockid (decos[imageID] blockID)
                decosBMI[decorInit[i, 0]] = decorInit[i, 1]; //Add blockid and imageid (decosBMI[blockID] imageID)
            }
            #endregion


            //================Special Blocks==========================
            //Load blocks with image position. blockid,imageposition
            //If there comes new special blocks you need to add them to the list.
            #region Misc
            miscInit = new int[,] {

                { 119, 0 },{ 300, 1 },{ 337, 2 },{ 113, 3 },{ 185, 4 },{ 184, 5 },{ 157, 6 },{ 156, 7 },{ 121, 8 },{ 50, 9 },{ 243, 10 }, { 136, 16 },
                { 201, 12 },{ 200, 13 },{ 361, 24 }, { 360, 27 }, { 368, 28 }, { 369, 29 }, { 370, 30 }, { 207, 31 }, { 206, 32 }, { 397, 53 }, { 411, 70 }, { 412, 71 },  { 413, 72 }, { 414, 73 }, { 416, 107 },
                { 100, 174 }, { 101, 175 }, { 1001, 55 }, { 1002, 63}, { 1003, 59 }, { 1004, 67 }, { 417, 74}, { 418, 75}, {419, 76}, { 420, 77}, { 421, 78}, { 422, 79 }, { 423, 80 }, { 1028, 100}, { 1027, 93 },
                { 374, 33}, { 381, 112 }, { 242, 108 }, { 385, 255}, { 241, 221 }, { 453, 176 }, { 375, 35 }, { 376, 39 }, { 377, 41 }, { 378, 45 }, { 379, 47 } , { 380, 51 }, { 438, 161 }, { 439, 167 },
                { 300, 1}, { 440, 169 }, { 275 , 117 }, { 329 , 129 }, { 273 , 125 }, { 328 , 157 }, { 327 , 121 },
                { 338,119 }, {339,120 }, { 327,121 }, { 370,30 }, { 456, 215 }, { 457,217 }, { 458,219 }, { 338,137}, { 339,133}, { 340 ,153 },
                { 370, 30}, {1041, 203}, {1042, 207}, {1043, 211 }, { 456, 215} ,{ 457, 217},{ 458, 219}, {447, 179},{ 448, 183},{449, 187}, {450, 191},{451, 195},{ 452, 199}, { 464, 201 }, { 465, 202 }, { 460, 222 },
                { 461, 253}, { 1064, 251}, {1052, 224 }, {1053, 228 }, { 1054 ,232 }, { 1055, 236 }, { 1056, 240 }, { 464, 244 }, { 465, 248 },
                { 467, 259} , {1080, 261}, { 1079, 262 }, { 1081, 263 }, { 1075, 264 }, { 1076, 268}, { 1077, 272}, { 1078, 276 }, { 471, 279 },
                { 475, 283 }, { 476, 286 }, { 477, 289 }, { 481, 292}, { 482, 298 }, { 483, 304}, { 1092, 308 },
                { 497, 311 }, { 498, 317 },
                { 492, 319 }, { 493, 323 }, { 494, 327 }, { 499, 331 }, { 1500, 335 }, { 1502, 338 },
                { 1094, 341 }, { 1095, 340 }, { 1506, 348 }, { 1507, 343 }, { 1510, 352 },


            };
            for (int i = 0; i < miscInit.Length / 2; i++)
            {
                misc[miscInit[i, 1]] = miscInit[i, 0]; //Add imageid and blockid (misc[imageID] blockID)
                miscBMI[miscInit[i, 0]] = miscInit[i, 1]; //Add blockid and imageid (miscBMI[blockID] imageID)
            }
            #endregion

            //================Foreground==========================
            //Load blocks with image position. blockid,imageposition
            //If there comes new foreground you need to add them to the list.
            #region Blocks
            blockInit = new int[,] {

                { 0, 0 },{ 1, 1 },{ 2, 2 },{ 3, 3 },{ 4, 4 },{ 5, 5 },{ 6, 6 },{ 7, 7 },{ 8, 8 },{ 9, 9 },{ 10, 10 },
                { 11, 11 },{ 12, 12 },{ 13, 13 },{ 14, 14 },{ 15, 15 },{ 16, 16 },{ 17, 17 },{ 18, 18 },{ 19, 19 },
                { 20, 20 },{ 21, 21 },{ 22, 22 },{ 23, 23 },{ 24, 24 },{ 25, 25 },{ 26, 26 },{ 27, 27 },{ 28, 28 },
                { 29, 29 },{ 30, 30 },{ 31, 31 },{ 32, 32 },{ 33, 33 },{ 34, 34 },{ 35, 35 },{ 36, 36 },{ 37, 37 },
                { 38, 38 },{ 39, 39 },{ 40, 40 },{ 41, 41 },{ 42, 42 },{ 43, 43 },{ 44, 44 },{ 45, 45 },{ 46, 46 },
                { 47, 47 },{ 48, 48 },{ 49, 49 },{ 51, 51 },{ 52, 52 },{ 53, 53 },{ 54, 54 },{ 55, 55 },
                { 56, 56 },{ 57, 57 },{ 58, 58 },{ 59, 59 },{ 60, 60 },{ 61, 61 },{ 62, 62 },{ 63, 63 },{ 64, 64 },
                { 65, 65 },{ 66, 66 },{ 67, 67 },{ 68, 68 },{ 69, 69 },{ 70, 70 },{ 71, 71 },{ 72, 72 },{ 73, 73 },
                { 74, 74 },{ 75, 75 },{ 76, 76 },{ 77, 77 },{ 78, 78 },{ 79, 79 },{ 80, 80 },{ 81, 81 },{ 82, 82 },
                { 83, 83 },{ 84, 84 },{ 85, 85 },{ 86, 86 },{ 87, 87 },{ 88, 88 },{ 89, 89 },{ 90, 90 },{ 91, 91 },
                { 92, 92 },{ 93, 93 },{ 94, 94 },{ 95, 95 },{ 96, 96 },{ 97, 97 },
                { 120, 98}, { 122, 99}, { 123, 100}, { 124, 101}, { 125, 102}, { 126, 103}, { 127, 104}, { 128, 105},
                { 129, 106}, { 130, 107}, { 131, 108}, { 132, 109}, { 133, 110}, { 134, 111}, { 135, 112}, { 137, 114},
                { 138, 115}, { 139, 116}, { 140, 117}, { 141, 118}, { 142, 119}, { 143, 120}, { 144, 121}, { 145, 122},
                { 146, 123}, { 147, 124}, { 148, 125}, { 149, 126}, { 150, 127}, { 151, 128}, { 152, 129}, { 153, 130},
                { 154, 131}, { 158, 132}, { 159, 133}, { 160, 134}, { 118, 135}, { 162, 136}, { 163, 137}, { 165, 139},
                { 166, 140}, { 167, 141}, { 168, 142}, { 169, 143}, { 170, 144}, { 171, 145}, { 172, 146}, { 173, 147},
                { 174, 148}, { 175, 149}, { 176, 150}, { 177, 151}, { 178, 152}, { 179, 153}, { 180, 154}, { 181, 155},
                { 182, 156}, { 114, 157}, { 115, 158}, { 116, 159}, { 117, 160}, { 186, 161}, { 187, 162}, { 188, 163},
                { 189, 164}, { 190, 165}, { 191, 166}, { 192, 167}, { 193, 168}, { 194, 169}, { 195, 170}, { 196, 171},
                { 197, 172}, { 198, 173}, { 98, 174}, { 99, 175}, { 199, 176}, { 202, 177}, { 203, 178}, { 204, 179},
                { 208, 180}, { 209, 181}, { 210, 182}, { 211, 183}, { 212, 184}, { 213, 185 }, { 214, 186}, {215, 187}, {216, 188},
                {408, 189}, {409, 190}, {410, 191}, {1005, 192}, {1006, 193}, {1007, 194}, {1008, 195}, {1009, 196}, {1010, 197},
                {1012, 198}, {1011, 199}, {1013, 200}, {1014, 201}, {1015, 202}, {1016, 203}, {1017, 204},
                {1018, 205}, {1019, 206}, {1020, 207}, {1021, 208}, {1022, 209}, {1023, 210}, {1024, 211}, {1025, 212}, {1026, 213}, {1029, 214},
                {1030, 215}, {1031, 216}, {1032, 217}, {1033, 218},  {1034, 219}, { 1044, 226 }, { 1045, 227 }, { 1046, 228 },
                { 1035, 220 }, { 1036, 221 }, { 1037, 222 }, { 1038, 223 }, { 1039, 224 }, { 1040, 225 }, { 1047, 229 }, { 1048, 230 }, { 1049, 231 }, { 1050, 232 },
                { 1051, 233 }, { 1057, 234 }, { 1058, 235 }, { 1054, 236 }, { 1055, 237 }, { 1056, 238 }, { 1057, 239 }, { 1058, 240 },
                { 1059, 237 }, { 1060, 238 }, { 1061, 239 }, { 1062, 240 }, { 1063, 241 }, { 459, 233 }, { 1051, 234 }, { 1057, 235 }, { 1058, 236 },
                { 1065, 242 }, { 1066, 243 }, { 1067, 244 }, { 1068, 245 }, { 1069, 246 }, { 1070, 247 }, { 1071, 248 }, { 1072, 249 }, { 1073, 250 }, { 1074, 251 },
                { 472, 252 }, { 1081, 253 }, { 1082, 254}, { 1083, 255}, { 1084, 256}, { 1085, 257}, { 1086, 258}, { 1087, 259},
                { 1088, 260 }, { 1089, 261 }, { 1090, 262 }, { 1091, 263 }, { 1093, 264 }, { 1096, 265 }, { 1097, 266 }, { 1098, 267 }, { 1099, 268 }, { 1100, 269 },
                { 1101, 270 },{ 1102, 271 },{ 1103, 272 },{ 1104, 273 },{ 1105, 274 },{ 1106, 275 },{ 1107, 276 },{ 1108, 277 },{ 1109, 278 },{ 1110, 279 },
                { 1111, 280 },{ 1112, 281 },{ 1113, 282 },{ 1114, 283 },{ 1115, 284 }

            };
            for (int i = 0; i < blockInit.Length / 2; i++)
            {
                blocks[blockInit[i, 1]] = blockInit[i, 0]; //Add imageid and blockid (blocks[imageID] blockID)
                foregroundBMI[blockInit[i, 0]] = blockInit[i, 1]; //Add blockid and imageid (foregroundBMI[blockID] imageID)
            }
            #endregion


            //SetupBricks();
            SetupImages();
            DetectBlocks();

            delay = new List<int>();
            editArea.Init(25, 25);
            frameSelector.SelectedIndex = 0;

            this.Text = "EEditor " + this.ProductVersion;

            timer.Elapsed += timer_Elapsed;
            timer.Start();

            // Extract topbar images to tile
            topbar2tile(false);
            Tool.PenSize = 1;

            filterTextBox.KeyDown += filterTextBox_KeyDown;
            penButton.Checked = true;
            starting1 = false;
            accountsComboBox.SelectedItem = userdata.username;

            if (!userdata.themeBorder || userdata.themeBorder)
            {
                var datta = userdata.themeBorder ? BorderStyle.Fixed3D : BorderStyle.None;
                flowLayoutPanel2.BorderStyle = datta;
                flowLayoutPanel3.BorderStyle = datta;
                flowLayoutPanel4.BorderStyle = datta;
                flowLayoutPanel5.BorderStyle = datta;
                flowLayoutPanel6.BorderStyle = datta;
            }
            flowLayoutPanel2.BackColor = userdata.themeBlock;
            flowLayoutPanel3.BackColor = userdata.themeBlock;
            flowLayoutPanel4.BackColor = userdata.themeBlock;
            flowLayoutPanel5.BackColor = userdata.themeBlock;
            flowLayoutPanel6.BackColor = userdata.themeBlock;
            updateImageColor();
        }




        #region image colors
        public void updateImageColor()
        {
            topFlowLayoutPanel.BackColor = userdata.themeToolbarBg;
            bottomFlowLayoutPanel.BackColor = userdata.themeToolbarBg;
            //Image colors
            var incr2 = 0;
            for (int i = 0; i < topFlowLayoutPanel.Controls.Count; i++)
            {
                var control = topFlowLayoutPanel.Controls[i];
                var items = ((ToolStrip)control).Items;
                control.BackColor = userdata.themeToolbarBg;
                ((ToolStrip)control).Renderer = new removeBadRenderer();
                if (((ToolStrip)control).Name != "lastUsedToolStrip")
                {
                    if (items.Count > 0)
                    {
                        for (int o = 0; o < items.Count; o++)
                        {
                            if (items[o].Image != null)
                            {
                                if (items[o].Text != null)
                                {
                                    items[o].ForeColor = userdata.themeToolbarText;
                                }
                                Bitmap bmp = (Bitmap)items[o].Image;
                                if (!sblocks.ContainsKey(incr2)) sblocks.Add(incr2, bmp);
                                else if (sblocks.ContainsKey(incr2))
                                {
                                    bmp = sblocks[incr2];
                                }
                                Bitmap bmp1 = new Bitmap(items[o].Image.Width, items[o].Image.Height);
                                for (int x = 0; x < items[o].Image.Width; x++)
                                {
                                    for (int y = 0; y < items[o].Image.Height; y++)
                                    {
                                        if (bmp.GetPixel(x, y).A > 80)
                                        {
                                            bmp1.SetPixel(x, y, userdata.themeIcons);
                                        }
                                        else
                                        {
                                            bmp1.SetPixel(x, y, userdata.themeIconsBg);
                                        }
                                    }
                                }
                                items[o].Image = bmp1;
                                incr2 += 1;
                            }
                            else
                            {
                                if (items[o].Name.Contains("Text"))
                                {
                                    ((ToolStripTextBox)items[o]).BorderStyle = BorderStyle.FixedSingle;
                                    items[o].BackColor = userdata.themeToolbarBg;
                                    items[o].ForeColor = userdata.themeToolbarText;
                                }
                                else if (items[o].Name.Contains("Combo"))
                                {
                                    items[o].BackColor = userdata.themeToolbarBg;
                                    items[o].ForeColor = userdata.themeToolbarText;

                                    //((ToolStripComboBox)items[o]) = new removeBadRenderer();
                                }
                                else if (items[o].Name.Contains("Label"))
                                {
                                    items[o].ForeColor = userdata.themeToolbarText;
                                }
                            }
                        }
                    }
                }
            }
            var incr3 = 0;
            for (int ii = 0; ii < bottomFlowLayoutPanel.Controls.Count; ii++)
            {
                var control = bottomFlowLayoutPanel.Controls[ii];
                var items = ((ToolStrip)control).Items;
                control.BackColor = userdata.themeToolbarBg;
                ((ToolStrip)control).Renderer = new removeBadRenderer();
                if (((ToolStrip)control).Name != "lastUsedToolStrip")
                {
                    if (items.Count > 0)
                    {
                        for (int oo = 0; oo < items.Count; oo++)
                        {
                            if (items[oo].Image != null)
                            {
                                if (items[oo].Text != null)
                                {
                                    items[oo].ForeColor = userdata.themeToolbarText;
                                }
                                Bitmap bmp = (Bitmap)items[oo].Image;
                                if (!sblocks1.ContainsKey(incr3)) sblocks1.Add(incr3, bmp);
                                else if (sblocks1.ContainsKey(incr3))
                                {
                                    bmp = sblocks1[incr3];
                                }
                                Bitmap bmp1 = new Bitmap(items[oo].Image.Width, items[oo].Image.Height);
                                for (int xx = 0; xx < bmp.Width; xx++)
                                {
                                    for (int yy = 0; yy < bmp.Height; yy++)
                                    {

                                        if (bmp.GetPixel(xx, yy).R + bmp.GetPixel(xx, yy).G + bmp.GetPixel(xx, yy).B == 0 && bmp.GetPixel(xx, yy).A > 15)
                                        {

                                            bmp1.SetPixel(xx, yy, userdata.themeIcons);
                                        }
                                        else
                                        {
                                            bmp1.SetPixel(xx, yy, userdata.themeIconsBg);
                                        }
                                    }
                                }
                                items[oo].Image = bmp1;
                                incr3 += 1;
                            }
                            else
                            {
                                if (items[oo].Name.Contains("Text"))
                                {
                                    ((ToolStripTextBox)items[oo]).BorderStyle = BorderStyle.FixedSingle;
                                    items[oo].BackColor = userdata.themeToolbarBg;
                                    items[oo].ForeColor = userdata.themeToolbarText;
                                }
                                else if (items[oo].Name.Contains("Combo"))
                                {
                                    items[oo].BackColor = userdata.themeToolbarBg;
                                    items[oo].ForeColor = userdata.themeToolbarText;
                                }
                                else if (items[oo].Name.Contains("Label"))
                                {
                                    items[oo].ForeColor = userdata.themeToolbarText;
                                }
                            }

                        }
                    }
                }
            }

        }
        #endregion

        #region Generate topbar images to tile
        private void topbar2tile(bool convert)
        {
            if (convert)
            {
                var width = 0;
                Bitmap img3 = new Bitmap(1024, 24);
                Bitmap img = new Bitmap(1024, 24);
                Graphics g = Graphics.FromImage(img3);
                for (int i = 0; i < topFlowLayoutPanel.Controls.Count; i++)
                {
                    var control = topFlowLayoutPanel.Controls[i];
                    var items = ((ToolStrip)control).Items;
                    if (items.Count > 0)
                    {
                        for (int o = 0; o < items.Count; o++)
                        {
                            if (items[o].Image != null && items[o].Name != "subButton" && items[o].Name != "addButton")
                            {
                                if (items[o].Image.Width == 16 && items[o].Image.Height == 16)
                                {
                                    img = new Bitmap(items[o].Image, new Size(24, 24));
                                    Graphics gg = Graphics.FromImage(img3);
                                    gg.DrawImage(img, new Point(width, 0));
                                    width += 24;
                                }
                                else
                                {
                                    g.DrawImage(items[o].Image, new Point(width, 0));
                                    width += 24;
                                }
                            }
                        }
                    }
                }
                img3.Save("output.png");
            }
        }
        #endregion

        #region  Undo, redo, history updater
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (historyToolStrip.InvokeRequired)
            {
                try
                {
                    if (ToolPen.undolist.Count >= 1) this.Invoke((MethodInvoker)delegate { undoButton.Enabled = true; });
                    if (ToolPen.redolist.Count >= 1) this.Invoke((MethodInvoker)delegate { redoButton.Enabled = true; });
                    if (ToolPen.undolist.Count == 0) this.Invoke((MethodInvoker)delegate { undoButton.Enabled = false; });
                    if (ToolPen.redolist.Count == 0) this.Invoke((MethodInvoker)delegate { redoButton.Enabled = false; });
                    if (ToolPen.undolist.Count >= 1 || ToolPen.redolist.Count >= 1) this.Invoke((MethodInvoker)delegate { historyButton.Enabled = true; });
                    if (ToolPen.undolist.Count == 0 && ToolPen.redolist.Count == 0) this.Invoke((MethodInvoker)delegate { historyButton.Enabled = false; });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
        #endregion

        #region Rebuild ToolStripContainers
        public void rebuildGUI(bool loadunknown)
        {
            tps.Clear();
            ownedb.Clear();
            if (resethotkeys)
            {
                userdata.brickHotkeys = null;
                for (int i = 0; i < 10; i++)
                {
                    if (shortCutButtons[i] != null)
                    {
                        shortCutButtons[i] = null;
                    }
                }
                resethotkeys = false;
            }
            else
            {
                string s = "";
                for (int i = 0; i < 10; i++)
                {
                    if (i != 0) s += ",";
                    s += shortCutButtons[i] == null ? -1 : shortCutButtons[i].ID;
                }
                userdata.brickHotkeys = s;
            }
            if (!userdata.themeBorder || userdata.themeBorder)
            {
                var datta = userdata.themeBorder ? BorderStyle.Fixed3D : BorderStyle.None;
                flowLayoutPanel2.BorderStyle = datta;
                flowLayoutPanel3.BorderStyle = datta;
                flowLayoutPanel4.BorderStyle = datta;
                flowLayoutPanel5.BorderStyle = datta;
                flowLayoutPanel6.BorderStyle = datta;
            }
            flowLayoutPanel2.BackColor = userdata.themeBlock;
            flowLayoutPanel3.BackColor = userdata.themeBlock;
            flowLayoutPanel4.BackColor = userdata.themeBlock;
            flowLayoutPanel5.BackColor = userdata.themeBlock;
            flowLayoutPanel6.BackColor = userdata.themeBlock;
            //toolStripContainer1.TopToolStripPanel.Controls.Clear();
            resetLastBlocks();
            if (flowLayoutPanel2.InvokeRequired) { this.Invoke((MethodInvoker)delegate { flowLayoutPanel2.Controls.Clear(); }); }
            if (flowLayoutPanel3.InvokeRequired) { this.Invoke((MethodInvoker)delegate { flowLayoutPanel3.Controls.Clear(); }); }
            if (flowLayoutPanel4.InvokeRequired) { this.Invoke((MethodInvoker)delegate { flowLayoutPanel4.Controls.Clear(); }); }
            if (flowLayoutPanel5.InvokeRequired) { this.Invoke((MethodInvoker)delegate { flowLayoutPanel5.Controls.Clear(); }); }
            if (flowLayoutPanel6.InvokeRequired) { this.Invoke((MethodInvoker)delegate { flowLayoutPanel6.Controls.Clear(); }); }
            else
            {
                flowLayoutPanel2.Controls.Clear();
                flowLayoutPanel3.Controls.Clear();
                flowLayoutPanel4.Controls.Clear();
                flowLayoutPanel5.Controls.Clear();
                flowLayoutPanel6.Controls.Clear();
            }
            //Toolstrip Container 3 - Recreat it.
            //this.toolStripContainer2.ContentPanel.Controls.Add(this.toolStripContainer3);
            //Toolstrip Container 3 - Recreat it.
            //this.toolStripContainer3.ContentPanel.Controls.Add(this.toolStripContainer4);
            DetectBlocks();
            if (hideBlocksButton.Checked == false)
            {
                if (flowLayoutPanel2.InvokeRequired) { this.Invoke((MethodInvoker)delegate { flowLayoutPanel2.Visible = false; }); }
                if (flowLayoutPanel3.InvokeRequired) { this.Invoke((MethodInvoker)delegate { flowLayoutPanel3.Visible = false; }); }
                if (flowLayoutPanel4.InvokeRequired) { this.Invoke((MethodInvoker)delegate { flowLayoutPanel4.Visible = false; }); }
                if (flowLayoutPanel5.InvokeRequired) { this.Invoke((MethodInvoker)delegate { flowLayoutPanel5.Visible = false; }); }
                if (flowLayoutPanel6.InvokeRequired) { this.Invoke((MethodInvoker)delegate { flowLayoutPanel6.Visible = false; }); }
                else
                {
                    flowLayoutPanel2.Visible = false; //Should be false
                    flowLayoutPanel3.Visible = false;
                    flowLayoutPanel4.Visible = false;
                    flowLayoutPanel5.Visible = false;
                    flowLayoutPanel6.Visible = false;
                }
            }
            else
            {
                if (!loadunknown)
                {

                    if (userdata.lastSelectedBlockbar == 0)
                    {
                        showBlocksButton.PerformClick();
                    }
                    else if (userdata.lastSelectedBlockbar == 1)
                    {
                        showActionsButton.PerformClick();
                    }
                    else if (userdata.lastSelectedBlockbar == 2)
                    {
                        showDecorationsButton.PerformClick();
                    }
                    else if (userdata.lastSelectedBlockbar == 3)
                    {
                        showBackgroundsButton.PerformClick();
                    }
                    else if (userdata.lastSelectedBlockbar == 4)
                    {
                        unknownButton.PerformClick();
                    }
                }
                else
                {
                    userdata.lastSelectedBlockbar = 4;
                    flowLayoutPanel2.Visible = false;
                    flowLayoutPanel3.Visible = false;
                    flowLayoutPanel4.Visible = false;
                    flowLayoutPanel5.Visible = false;
                    flowLayoutPanel6.Visible = true;

                    showBlocksButton.Checked = false;
                    showActionsButton.Checked = false;
                    showDecorationsButton.Checked = false;
                    showBackgroundsButton.Checked = false;
                    unknownButton.Checked = true;
                }
            }
        }
        #endregion;

        #region Detect blocks from the user, payvault or saved payvault
        private void DetectBlocks()
        {
            SetupBricks(false);
        }
        #endregion;

        #region Setup the images
        protected void SetupImages()
        {

            editArea.unknowBricks = EEditor.Properties.Resources.bullets;
            editArea.misc = new Bitmap(Properties.Resources.misc, new System.Drawing.Size(Properties.Resources.misc.Width, 16));



            //Foreground
            Graphics g = Graphics.FromImage(foregroundBMD);
            g.DrawImage(Properties.Resources.BLOCKS_front, new Rectangle(0, 0, Properties.Resources.BLOCKS_front.Width, 16));


            //Misc
            Graphics g1 = Graphics.FromImage(miscBMD);
            g1.DrawImage(Properties.Resources.misc, new Rectangle(0, 0, Properties.Resources.misc.Width, 16));


            //Decorations

            Graphics g2 = Graphics.FromImage(decosBMD);
            g2.DrawImage(Properties.Resources.BLOCKS_deco, new Rectangle(0, 0, Properties.Resources.BLOCKS_deco.Width, 16));


            //Backgrounds
            Graphics g3 = Graphics.FromImage(backgroundBMD);
            g3.DrawImage(Properties.Resources.BLOCKS_back, new Rectangle(0, 0, Properties.Resources.BLOCKS_back.Width, 16));
            resetLastBlocks();


        }
        #endregion
        private void resetLastBlocks()
        {
            Bitmap img3 = foregroundBMD.Clone(new Rectangle(0 * 16, 0, 16, 16), foregroundBMD.PixelFormat);
            lastUsedBlockButton0.Name = "0";
            lastUsedBlockButton0.Image = img3;
            lastUsedBlockButton1.Name = "0";
            lastUsedBlockButton1.Image = img3;
            lastUsedBlockButton2.Name = "0";
            lastUsedBlockButton2.Image = img3;
            lastUsedBlockButton3.Name = "0";
            lastUsedBlockButton3.Image = img3;
            lastUsedBlockButton4.Name = "0";
            lastUsedBlockButton4.Image = img3;
        }
        #region Block stuff
        protected void SetupBricks(bool fromclient)
        {

            #region Foreground
            //Foreground 1
                AddToolStrip(foregroundBMD, 0, new int[] { 260, 156, 9, 10, 11, 12, 13, 14, 15, 205 }, new uint[] { 0xB1B1B1, 0x282828, 0x6E6E6E, 0x3552A8, 0x9735A7, 0xA83554, 0x93A835, 0x42A836, 0x359EA6, 0xB24521 }, true, "Basic", 0, 0, true);
                if (ihavethese.Contains("beta") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 261, 37, 38, 39, 40, 41, 42, 206, 207, 208 }, new uint[] { 0xE5E5E5, 0xCE62CF, 0x4AC882, 0x4D84C6, 0xCF6650, 0xD2A945, 0x999999, 0x49C2C6, 0xCE7E50, 0x474747 }, false, "Beta", 0, 0, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 261, 37, 38, 39, 40, 41, 42, 206, 207, 208 }, new uint[] { 0xE5E5E5, 0xCE62CF, 0x4AC882, 0x4D84C6, 0xCF6650, 0xD2A945, 0x999999, 0x49C2C6, 0xCE7E50, 0x474747 }, false, "Beta", 0, 0, false); }
                AddToolStrip(foregroundBMD, 0, new int[] { 262, 16, 17, 18, 19, 20, 21, 209, 210, 211 }, new uint[] { 0x888888, 0x8B3E09, 0x246F4D, 0x4E246F, 0x438310, 0x6F2429, 0x6F5D24, 0x4C4C4C, 0x092164, 0x181818 }, false, "Brick", 0, 0, true);
                AddToolStrip(foregroundBMD, 0, new int[] { 29, 30, 31, }, new uint[] { 0xA1A3A5, 0xDF7A41, 0xF0A927 }, false, "Metal", 0, 0, true);
                AddToolStrip(foregroundBMD, 0, new int[] { 34, 35, 36 }, new uint[] { 0x456313, 0x456313, 0x456313 }, false, "Grass", 0, 0, true);
                AddToolStrip(foregroundBMD, 0, new int[] { 22, 235, 32, 236, 33 }, new uint[] { 0x895B12, 0xD19322, 0xCF9022, 0x523B0F, 0x000000 }, false, "Generic", 0, 0, true);
                AddToolStrip(foregroundBMD, 0, new int[] { 45, 46, 47, 48, 49 }, new uint[] { 0x72614B, 0x6E6B60, 0x8E734F, 0x7F4F2B, 0x757575 }, false, "Factory", 0, 0, true);
                AddToolStrip(foregroundBMD, 0, new int[] { 44 }, new uint[] { 0x000000 }, false, "Secrets", 0, 0, true);
                if (!userdata.openWorld) AddToolStrip(miscBMD, 1, new int[] { 9, 10, 16 }, null, false, "Secrets", 0, 0, true);
                AddToolStrip(foregroundBMD, 0, new int[] { 51, 52, 53, 54, 55, 56, 57, 58 }, new uint[] { 0xF89299, 0xE98BF6, 0xA789F6, 0x7E99F6, 0x95DCF6, 0x92FBAA, 0xF8DA8C, 0xF6BA94 }, false, "Glass", 0, 0, true);
                if (ihavethese.Contains("brickminiral") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 70, 71, 72, 73, 74, 75, 76 }, new uint[] { 0xEE0000, 0xEE00EE, 0x0000EE, 0x00EEEE, 0x00EE00, 0xEEEE00, 0xEE7700 }, false, "Minerals", 0, 0, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 70, 71, 72, 73, 74, 75, 76 }, new uint[] { 0xEE0000, 0xEE00EE, 0x0000EE, 0x00EEEE, 0x00EE00, 0xEEEE00, 0xEE7700 }, false, "Minerals", 0, 0, false); }
                if (ihavethese.Contains("brickxmas2011") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 78, 79, 80, 81, 82 }, new uint[] { 0x7EBE14, 0xC38884, 0x99440F, 0x56197E, 0x568515 }, false, "Christmas 2011", 0, 0, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 78, 79, 80, 81, 82 }, new uint[] { 0x7EBE14, 0xC38884, 0x99440F, 0x56197E, 0x568515 }, false, "Christmas 2011", 0, 0, false); }
                if (ihavethese.Contains("brickcandy") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 60, 61, 62, 63, 64, 65, 66, 67 }, new uint[] { 0xFB93B4, 0x772C6C, 0x711620, 0x315364, 0x134913, 0xC27474, 0xA46951, 0x8D3111 }, false, "Candy", 0, 0, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 60, 61, 62, 63, 64, 65, 66, 67 }, new uint[] { 0xFB93B4, 0x772C6C, 0x711620, 0x315364, 0x134913, 0xC27474, 0xA46951, 0x8D3111 }, false, "Candy", 0, 0, false); }
                if (ihavethese.Contains("bricksummer2011") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 59 }, new uint[] { 0xD9BB86 }, false, "Summer 2011", 0, 0, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 59 }, new uint[] { 0xD9BB86 }, false, "Summer 2011", 0, 0, false); }
                if (ihavethese.Contains("brickhw2011") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 68, 69 }, new uint[] { 0x685454, 0x5E6E74 }, false, "Halloween 2011", 0, 0, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 68, 69 }, new uint[] { 0x685454, 0x5E6E74 }, false, "Halloween 2011", 0, 0, false); }
                if (ihavethese.Contains("brickscifi") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 84, 85, 86, 87, 88, 89, 90, 91, 234 }, new uint[] { 0x9F4340, 0x3B729D, 0x868686, 0xFFFFFF, 0x6C4F2C, 0xBA6971, 0x6977BA, 0x64B66E, 0xBD8453 }, false, "Sci-Fi", 0, 1, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 84, 85, 86, 87, 88, 89, 90, 91, 234 }, new uint[] { 0x9F4340, 0x3B729D, 0x868686, 0xFFFFFF, 0x6C4F2C, 0xBA6971, 0x6977BA, 0x64B66E, 0xBD8453 }, false, "Sci-Fi", 0, 1, false); }
                if (ihavethese.Contains("brickprison") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 92 }, new uint[] { 0x808080 }, false, "Prison", 0, 1, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 92 }, new uint[] { 0x808080 }, false, "Prison", 0, 1, false); }
                if (ihavethese.Contains("brickpirate") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 93, 94, 131 }, new uint[] { 0xB09364, 0xA89455, 0 }, false, "Pirate", 0, 2, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 93, 94, 131 }, new uint[] { 0xB09364, 0xA89455, 0 }, false, "Pirate", 0, 2, false); }
                if (ihavethese.Contains("brickstone") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 95, 226, 227, 228 }, new uint[] { 0x5E6267, 0x547064, 0x695137, 0x4D5772 }, false, "Stone", 0, 2, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 95, 226, 227, 228 }, new uint[] { 0x5E6267, 0x547064, 0x695137, 0x4D5772 }, false, "Stone", 0, 2, false); }
                if (ihavethese.Contains("brickninja") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 96, 97 }, null, false, "Dojo", 0, 2, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 96, 97 }, null, false, "Dojo", 0, 2, false); }
                if (ihavethese.Contains("brickcowboy") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 99, 100, 101, 102, 103, 104 }, null, false, "Wild West", 0, 2, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 99, 100, 101, 102, 103, 104 }, null, false, "Wild West", 0, 2, false); }
                if (ihavethese.Contains("brickplastic") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 105, 106, 107, 108, 109, 110, 111, 112 }, new uint[] { 0x93EB10, 0xD53725, 0xDFCF19, 0x72C5EB, 0x2B43CF, 0xDA28D8, 0x2DAC10, 0xE5821F }, false, "Plastic", 0, 1, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 105, 106, 107, 108, 109, 110, 111, 112 }, new uint[] { 0x93EB10, 0xD53725, 0xDFCF19, 0x72C5EB, 0x2B43CF, 0xDA28D8, 0x2DAC10, 0xE5821F }, false, "Plastic", 0, 1, false); }
                if (ihavethese.Contains("bricksand") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 114, 115, 116, 117, 118, 119 }, new uint[] { 0xE0D5B1, 0xA29D88, 0xE4D98D, 0xD8B65A, 0xAF9468, 0x795A35 }, false, "Sand", 0, 1, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 114, 115, 116, 117, 118, 119 }, new uint[] { 0xE0D5B1, 0xA29D88, 0xE4D98D, 0xD8B65A, 0xAF9468, 0x795A35 }, false, "Sand", 0, 1, false); }
                AddToolStrip(foregroundBMD, 0, new int[] { 120 }, new uint[] { 0xF6FCFF }, false, "Cloud", 0, 1, true);
                if (ihavethese.Contains("brickindustrial") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 121, 122, 123, 124, 125, 126, 127, 128, 129, 130 }, new uint[] { 0x7E7E7E, 0x595B5D, 0, 0, 0, 0x7B7A7A, 0x6D6C6C, 0x595858, 0x5E5D5D, 0x6D6C6C }, false, "Industrial", 0, 1, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 121, 122, 123, 124, 125, 126, 127, 128, 129, 130 }, new uint[] { 0x7E7E7E, 0x595B5D, 0, 0, 0, 0x7B7A7A, 0x6D6C6C, 0x595858, 0x5E5D5D, 0x6D6C6C }, false, "Industrial", 0, 1, false); }
                if (ihavethese.Contains("brickmedieval") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 132, 133, 134, 136, 137 }, new uint[] { 0, 0x545F68, 0x4D565E, 0, 0 }, false, "Medieval", 0, 1, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 132, 133, 134, 136, 137 }, new uint[] { 0, 0x545F68, 0x4D565E, 0, 0 }, false, "Medieval", 0, 1, false); }
                AddToolStrip(foregroundBMD, 0, new int[] { 140, 141, 142, 143, 144, 145 }, new uint[] { 0xCC730C, 0xDE7A0D, 0xCF740D, 0xD57402, 0xEB7D02, 0xD57402 }, false, "Pipes", 0, 1, true);
                AddToolStrip(foregroundBMD, 0, new int[] { 146, 147, 148, 149, 150, 214 }, new uint[] { 0xD7D6DD, 0x639AFB, 0x58D30A, 0xE2456D, 0xFFAB44, 0x7F7F7F }, false, "Outer Space", 0, 1, true);
                AddToolStrip(foregroundBMD, 0, new int[] { 151, 152, 153, 154, 155 }, new uint[] { 0xDD943B, 0xC68534, 0x916127, 0xB67B31, 0xB47B33 }, false, "Desert", 0, 1, true);
                AddToolStrip(foregroundBMD, 0, new int[] { 263, 161, 162, 163, 164, 165, 166, 167, 212, 213 }, new uint[] { 0xBFBFBF, 0x6B6B6B, 0x2F5391, 0x803D91, 0xA8193F, 0xABB333, 0x45A337, 0x3CB2AC, 0xA15531, 0x272727 }, false, "Checker", 0, 1, true);
                AddToolStrip(foregroundBMD, 0, new int[] { 168, 169, 170, 171, 172, 173, 176 }, new uint[] { 0, 0, 0x99997A, 0xAC7061, 0x62889A, 0x878441, 0 }, false, "Jungle", 0, 1, true);
                if (ihavethese.Contains("bricklava") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 177, 178, 179 }, new uint[] { 0xFFCE3E, 0xFA970E, 0xFF5F00 }, false, "Lava", 0, 1, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 177, 178, 179 }, new uint[] { 0xFFCE3E, 0xFA970E, 0xFF5F00 }, false, "Lava", 0, 1, false); }
                if (ihavethese.Contains("bricksparta") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 180, 181, 182, 183 }, new uint[] { 0xCDD1D3, 0xC1DCB9, 0xE5C6CF, 0 }, false, "Marble", 0, 1, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 180, 181, 182, 183 }, new uint[] { 0xCDD1D3, 0xC1DCB9, 0xE5C6CF, 0 }, false, "Marble", 0, 1, false); }
                if (ihavethese.Contains("brickfarm") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 184 }, new uint[] { 0xCCBE75 }, false, "Farm", 0, 1, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 184 }, new uint[] { 0xCCBE75 }, false, "Farm", 0, 1, false); }
                if (ihavethese.Contains("brickchristmas2014") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 187, 188 }, new uint[] { 0xB2BCE1, 0x385862 }, false, "Christmas 2014", 0, 1, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 187, 188 }, new uint[] { 0xB2BCE1, 0x385862 }, false, "Christmas 2014", 0, 1, false); }
                if (ihavethese.Contains("brickoneway") || debug) { AddToolStrip(miscBMD, 1, new int[] { 308, 55, 63, 59, 67, 224, 228, 232, 236, 240 }, new uint[] { 0x4A4A4A, 0x023032, 0x441602, 0x3C2D01, 0x3E0241, 0x232323, 0x021D33, 0x44020A, 0x0B2F0C, 0x0D0D0D }, false, "One-Way", 0, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 308, 55, 63, 59, 67, 224, 228, 232, 236, 240 }, new uint[] { 0, 0x023032, 0x441602, 0x3C2D01, 0x3E0241, 0x232323, 0x021D33, 0x44020A, 0x0B2F0C, 0x0D0D0D }, false, "One-Way", 0, 2, false); }
                if (ihavethese.Contains("brickmagic") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 200 }, new uint[] { 0x2D4F16 }, false, "Magic blocks", 0, 2, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 200 }, new uint[] { 0x2D4F16 }, false, "Magic blocks", 0, 2, false); }
                if (ihavethese.Contains("brickmagic2") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 201 }, new uint[] { 0x4A1471 }, false, "Magic blocks", 0, 2, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 201 }, new uint[] { 0x4A1471 }, false, "Magic blocks", 0, 2, false); }
                if (ihavethese.Contains("brickmagic3") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 202 }, new uint[] { 0x9D611A }, false, "Magic blocks", 0, 2, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 202 }, new uint[] { 0x9D611A }, false, "Magic blocks", 0, 2, false); }
                if (ihavethese.Contains("brickmagic4") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 203 }, new uint[] { 0x324B7C }, false, "Magic blocks", 0, 2, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 203 }, new uint[] { 0x324B7C }, false, "Magic blocks", 0, 2, false); }
                if (ihavethese.Contains("brickmagic5") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 204 }, new uint[] { 0xAC2531 }, false, "Magic blocks", 0, 2, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 204 }, new uint[] { 0xAC2531 }, false, "Magic blocks", 0, 2, false); }
                AddToolStrip(foregroundBMD, 0, new int[] { 215, 216, 217, 218, 219 }, new uint[] { 0x814224, 0x639012, 0xD3A558, 0x666C7E, 0x7F4413 }, false, "Enviroment", 0, 2, true);
                if (ihavethese.Contains("brickdomestic") || debug)
                {
                    AddToolStrip(foregroundBMD, 0, new int[] { 220, 221, 222, 223, 224, 225 }, new uint[] { 0xAE8358, 0x8D4E07, 0xA11616, 0x165CA1, 0x298820, 0xACA09C }, false, "Domestic", 0, 2, true);
                    AddToolStrip(miscBMD, 1, new int[] { 203, 207, 211 }, new uint[] { 0x544321, 0x462807, 0x55504E }, false, "Domestic", 0, 2, true);
                }
                else
                {
                    AddToolStrip(foregroundBMD, 0, new int[] { 220, 221, 222, 223, 224, 225 }, new uint[] { 0xAE8358, 0x8D4E07, 0xA11616, 0x165CA1, 0x298820, 0xACA09C }, false, "Domestic", 0, 2, false);
                    AddToolStrip(miscBMD, 1, new int[] { 203, 207, 211 }, new uint[] { 0x544321, 0x462807, 0x55504E }, false, "Domestic", 0, 2, false);
                }
                if (ihavethese.Contains("brickhalloween2015") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 229, 230, 231, 232 }, new uint[] { 0x50642F, 0x85897F, 0x3D443C, 0 }, false, "Halloween 2015", 0, 2, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 229, 230, 231, 232 }, new uint[] { 0x50642F, 0x85897F, 0x3D443C, 0 }, false, "Halloween 2015", 0, 2, false); }
                AddToolStrip(foregroundBMD, 0, new int[] { 237, 238, 239, 240, 241 }, new uint[] { 0x6E8CC7, 0xA2B1CD, 0x8B98CB, 0x8B98CC, 0x8C9ACC }, false, "Arctic", 0, 2, true);
                if (ihavethese.Contains("goldmember") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 242, 243, 244, 245, 246 }, new uint[] { 0xC7A546, 0xBF9426, 0xBF9D42, 0xB99334, 0 }, false, "Gold Membership", 0, 2, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 242, 243, 244, 245, 246 }, new uint[] { 0xC7A546, 0xBF9426, 0xBF9D42, 0xB99334, 0 }, false, "Gold Membership", 0, 2, false); }
                if (ihavethese.Contains("brickfairytale") || debug)
                {
                    AddToolStrip(foregroundBMD, 0, new int[] { 247, 248, 249, 250, 251 }, new uint[] { 0x98A093, 0xCB7D15, 0x3CB517, 0x7DB2DB, 0xC83D3F, 0xACA09C }, false, "Fairytale", 0, 2, true);
                    AddToolStrip(miscBMD, 1, new int[] { 264, 268, 272, 276 }, new uint[] { 0x5F3827, 0x3B5A30, 0x26485F, 0x61364C }, false, "Fairytale", 0, 2, true);
                }
                else
                {
                    AddToolStrip(foregroundBMD, 0, new int[] { 247, 248, 249, 250, 251 }, new uint[] { 0x98A093, 0xCB7D15, 0x3CB517, 0x7DB2DB, 0xC83D3F, 0xACA09C }, false, "Fairytale", 0, 2, false);
                    AddToolStrip(miscBMD, 1, new int[] { 264, 268, 272, 276 }, new uint[] { 0x5F3827, 0x3B5A30, 0x26485F, 0x61364C }, false, "Fairytale", 0, 2, false);
                }
                if (ihavethese.Contains("brickspring2016") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 253, 254 }, new uint[] { 0x932A0D, 0x165701 }, false, "Spring 2016", 0, 2, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 253, 254 }, new uint[] { 0x932A0D, 0x165701 }, false, "Spring 2016", 0, 2, false); }
                if (ihavethese.Contains("bricksummer2016") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 255, 256, 257, 258, 259 }, new uint[] { 0xB59358, 0x9A6093, 0xC5A228, 0x56A56E, 0 }, false, "Summer 2016", 0, 2, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 255, 256, 257, 258, 259 }, new uint[] { 0xB59358, 0x9A6093, 0xC5A228, 0x56A56E, 0 }, false, "Summer 2016", 0, 2, false); }

                if (ihavethese.Contains("brickmine") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 264 }, new uint[] { 0x9B5A3F }, false, "Mine", 0, 2, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 264 }, new uint[] { 0x9B5A3F }, false, "Mine", 0, 2, false); }
                AddToolStrip(foregroundBMD, 0, new int[] { 265, 266, 267, 268, 269 }, new uint[] { 0xC79B68, 0x797979, 0xC9B190, 0xAA4937, 0xA94836 }, false, "Construction", 0, 2, true);
                if (ihavethese.Contains("brickchristmas2016") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 270, 271, 272, 273, 274 }, new uint[] { 0x6E2B0B, 0x443C0B, 0x7D4E49, 0x253461, 0x86410D }, false, "Christmas 2016", 0, 2, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 270, 271, 272, 273, 274 }, new uint[] { 0x6E2B0B, 0x443C0B, 0x7D4E49, 0x253461, 0x86410D }, false, "Christmas 2016", 0, 2, false); }

                if (ihavethese.Contains("bricktiles") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 275, 276, 277, 278, 279, 280, 281, 282, 283, 284 }, new uint[] { 0xB3B09B, 0x959386, 0x716F60, 0xAD7373, 0xA97C67, 0xA59069, 0x7E9575, 0x7EA194, 0x7C8B9D, 0x857A99 }, false, "Tiles", 0, 2, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 275, 276, 277, 278, 279, 280, 281, 282, 283, 284 }, new uint[] { 0xB3B09B, 0x959386, 0x716F60, 0xAD7373, 0xA97C67, 0xA59069, 0x7E9575, 0x7EA194, 0x7C8B9D, 0x857A99 }, false, "Tiles", 0, 2, false); }

            #endregion

            #region Decoration
            //Decorations 1
      
                if (ihavethese.Contains("brickchristmas2010") || debug) { AddToolStrip(decosBMD, 2, new int[] { 32, 33, 34, 35, 36, 37 }, null, false, "Christmas 2010", 2, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 32, 33, 34, 35, 36, 37 }, null, false, "Christmas 2010", 2, 0, false); }
                if (ihavethese.Contains("mixednewyear2010") || debug) { AddToolStrip(decosBMD, 2, new int[] { 27, 28, 29, 30, 31 }, null, false, "New Year 2010", 2, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 27, 28, 29, 30, 31 }, null, false, "New Year 2010", 2, 0, false); }
                if (ihavethese.Contains("brickspring2011") || debug) { AddToolStrip(decosBMD, 2, new int[] { 16, 17, 18, 19, 20, 21, 22, 23 }, null, false, "Spring 2011", 2, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 16, 17, 18, 19, 20, 21, 22, 23 }, null, false, "Spring 2011", 2, 0, false); }
                if (ihavethese.Contains("brickhwtrophy") || debug) { AddToolStrip(decosBMD, 2, new int[] { 6 }, null, false, "Prizes", 2, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 6 }, null, false, "Prizes", 2, 0, false); }


                if (ihavethese.Contains("brickspringtrophybronze") || debug) { AddToolStrip(decosBMD, 2, new int[] { 209 }, null, false, "Prizes", 2, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 209 }, null, false, "Prizes", 2, 0, false); }

                if (ihavethese.Contains("brickspringtrophysilver") || debug) { AddToolStrip(decosBMD, 2, new int[] { 208 }, null, false, "Prizes", 2, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 208 }, null, false, "Prizes", 2, 0, false); }


                if (ihavethese.Contains("brickspringtrophygold") || debug) { AddToolStrip(decosBMD, 2, new int[] { 207 }, null, false, "Prizes", 2, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 207 }, null, false, "Prizes", 2, 0, false); }

                if (ihavethese.Contains("bricksummertrophybronze") || debug) { AddToolStrip(decosBMD, 2, new int[] { 212 }, null, false, "Prizes", 2, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 212 }, null, false, "Prizes", 2, 0, false); }
                if (ihavethese.Contains("bricksummertrophysilver") || debug) { AddToolStrip(decosBMD, 2, new int[] { 211 }, null, false, "Prizes", 2, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 211 }, null, false, "Prizes", 2, 0, false); }
                if (ihavethese.Contains("bricksummertrophygold") || debug) { AddToolStrip(decosBMD, 2, new int[] { 210 }, null, false, "Prizes", 2, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 210 }, null, false, "Prizes", 2, 0, false); }




                if (ihavethese.Contains("brickeaster2012") || debug) { AddToolStrip(decosBMD, 2, new int[] { 39, 40, 41, 42, 43 }, null, false, "Easter 2012", 2, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 39, 40, 41, 42, 43 }, null, false, "Easter 2012", 2, 0, false); }
                if (ihavethese.Contains("brickcandy") || debug) { AddToolStrip(decosBMD, 2, new int[] { 10, 184, 185, 186, 187 }, null, false, "Candy", 2, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 10, 184, 185, 186, 187 }, null, false, "Candy", 2, 0, false); }
                if (ihavethese.Contains("bricksummer2011") || debug) { AddToolStrip(decosBMD, 2, new int[] { 11, 12, 13, 14, 15 }, null, false, "Summer 2011", 2, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 11, 12, 13, 14, 15 }, null, false, "Summer 2011", 2, 0, false); }
                if (ihavethese.Contains("brickhw2011") || debug) { AddToolStrip(decosBMD, 2, new int[] { 7, 8, 9 }, null, false, "Halloween 2011", 2, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 7, 8, 9 }, null, false, "Halloween 2011", 2, 0, false); }
                if (ihavethese.Contains("brickxmas2011") || debug) { AddToolStrip(decosBMD, 2, new int[] { 1, 2, 3, 4, 5 }, null, false, "Christmas 2011", 2, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 1, 2, 3, 4, 5 }, null, false, "Christmas 2011", 2, 0, false); }
                if (ihavethese.Contains("brickscifi") || debug) { AddToolStrip(miscBMD, 1, new int[] { 35, 39, 41, 45, 47, 51, 161, 167 }, null, false, "Sci-Fi", 2, 0, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 35, 39, 41, 45, 47, 51, 161, 167 }, null, false, "Sci-Fi", 2, 0, false); }
                if (ihavethese.Contains("brickprison") || debug) { AddToolStrip(decosBMD, 2, new int[] { 44 }, null, false, "Prison", 2, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 44 }, null, false, "Prison", 2, 0, false); }
                AddToolStrip(decosBMD, 2, new int[] { 45, 46, 47, 48, 49, 50, 51, 52, 53 }, null, false, "Windows", 2, 0, true);
                if (ihavethese.Contains("brickpirate") || debug) { AddToolStrip(decosBMD, 2, new int[] { 54, 55, 188, 189 }, null, false, "Pirate", 2, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 54, 55, 188, 189 }, null, false, "Pirate", 2, 0, false); }
                if (ihavethese.Contains("brickninja") || debug) { AddToolStrip(decosBMD, 2, new int[] { 59, 60, 61, 62, 63, 64, 65, 66, 67 }, null, false, "Dojo", 2, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 59, 60, 61, 62, 63, 64, 65, 66, 67 }, null, false, "Dojo", 2, 0, false); }
                if (ihavethese.Contains("brickcowboy") || debug) { AddToolStrip(decosBMD, 2, new int[] { 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82 }, null, false, "Wild West", 2, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82 }, null, false, "Wild West", 2, 0, false); }
                AddToolStrip(miscBMD, 1, new int[] { 1 }, null, false, "Water", 2, 0, true);
                if (ihavethese.Contains("bricksand") || debug) { AddToolStrip(decosBMD, 2, new int[] { 83, 84, 85, 86, 87, 88 }, null, false, "Sand", 2, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 83, 84, 85, 86, 87, 88 }, null, false, "Sand", 2, 0, false); }
                if (ihavethese.Contains("bricksummer2012") || debug) { AddToolStrip(decosBMD, 2, new int[] { 89, 90, 91, 92 }, null, false, "Summer 2012", 2, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 89, 90, 91, 92 }, null, false, "Summer 2012", 2, 0, false); }
                AddToolStrip(decosBMD, 2, new int[] { 93, 94, 95, 96, 97, 98, 99, 100 }, null, false, "Cloud", 2, 0, true);
                if (ihavethese.Contains("brickindustrial") || debug) { AddToolStrip(decosBMD, 2, new int[] { 101, 102, 103, 104, 105, 106 }, null, false, "Industrial", 2, 1, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 101, 102, 103, 104, 105, 106 }, null, false, "Industrial", 2, 1, false); }
                if (ihavethese.Contains("brickmedieval") || debug)
                {
                    AddToolStrip(decosBMD, 2, new int[] { 107, 108, 190 }, null, false, "Medieval", 2, 1, true);
                    AddToolStrip(miscBMD, 1, new int[] { 169 }, null, false, "Medieval", 2, 1, true);
                    AddToolStrip(decosBMD, 2, new int[] { 112 }, null, false, "Medieval", 2, 1, true);
                    AddToolStrip(miscBMD, 1, new int[] { 117, 129, 125, 157, 121 }, null, false, "Medieval", 2, 1, true);
                }
                else
                {
                    AddToolStrip(decosBMD, 2, new int[] { 107, 108, 190 }, null, false, "Medieval", 2, 1, false);
                    AddToolStrip(miscBMD, 1, new int[] { 169 }, null, false, "Medieval", 2, 1, false);
                    AddToolStrip(decosBMD, 2, new int[] { 112 }, null, false, "Medieval", 2, 1, false);
                    AddToolStrip(miscBMD, 1, new int[] { 117, 129, 125, 157, 121 }, null, false, "Medieval", 2, 1, false);
                }
                AddToolStrip(decosBMD, 2, new int[] { 114, 115, 116, 117, 181, 182, 183, 113 }, null, false, "Outer Space", 2, 1, true);
                AddToolStrip(decosBMD, 2, new int[] { 118, 178, 179, 180 }, null, false, "Desert", 2, 1, true);
                if (ihavethese.Contains("brickmonster") || debug)
                {
                    AddToolStrip(miscBMD, 1, new int[] { 137, 133, 153 }, null, false, "Monster", 2, 1, true);
                    AddToolStrip(decosBMD, 2, new int[] { 57, 122, 123 }, null, false, "Monster", 2, 1, true);
                }
                else
                {
                    AddToolStrip(miscBMD, 1, new int[] { 137, 133, 153 }, null, false, "Monster", 2, 1, false);
                    AddToolStrip(decosBMD, 2, new int[] { 57, 122, 123 }, null, false, "Monster", 2, 1, false);
                }
                if (ihavethese.Contains("brickfog") || debug) { AddToolStrip(decosBMD, 2, new int[] { 124, 125, 126, 127, 128, 129, 130, 131, 132 }, null, false, "Fog", 2, 1, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 124, 125, 126, 127, 128, 129, 130, 131, 132 }, null, false, "Fog", 2, 1, false); }
                if (ihavethese.Contains("brickhw2012") || debug) { AddToolStrip(decosBMD, 2, new int[] { 133, 134, 135, 136, 137 }, null, false, "Halloween 2012", 2, 1, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 133, 134, 135, 136, 137 }, null, false, "Halloween 2012", 2, 1, false); }
                AddToolStrip(decosBMD, 2, new int[] { 138, 139, 140 }, null, false, "Jungle", 2, 1, true);
                if (ihavethese.Contains("brickxmas2012") || debug) { AddToolStrip(decosBMD, 2, new int[] { 141, 142, 143, 144, 145, 146 }, null, false, "Christmas 2012", 2, 1, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 141, 142, 143, 144, 145, 146 }, null, false, "Christmas 2012", 2, 1, false); }
                if (ihavethese.Contains("bricklava") || debug) { AddToolStrip(decosBMD, 2, new int[] { 175 }, null, false, "Lava", 2, 1, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 175 }, null, false, "Lava", 2, 1, false); }
                if (ihavethese.Contains("brickswamp") || debug)
                {
                    AddToolStrip(miscBMD, 1, new int[] { 30 }, null, false, "Swamp", 2, 1, true);
                    AddToolStrip(decosBMD, 2, new int[] { 147, 148, 149 }, null, false, "Swamp", 2, 1, true);
                }
                else
                {
                    AddToolStrip(miscBMD, 1, new int[] { 30 }, null, false, "Swamp", 2, 1, false);
                    AddToolStrip(decosBMD, 2, new int[] { 147, 148, 149 }, null, false, "Swamp", 2, 1, false);
                }
                if (ihavethese.Contains("bricksparta") || debug) { AddToolStrip(decosBMD, 2, new int[] { 150, 151, 152 }, null, false, "Marble", 2, 1, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 150, 151, 152 }, null, false, "Marble", 2, 1, false); }
                if (ihavethese.Contains("brickfarm") || debug) { AddToolStrip(decosBMD, 2, new int[] { 154, 155, 156, 157 }, null, false, "Farm", 2, 2, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 154, 155, 156, 157 }, null, false, "Farm", 2, 2, false); }
                if (ihavethese.Contains("brickautumn2014") || debug) { AddToolStrip(decosBMD, 2, new int[] { 158, 159, 160, 161, 162, 163, 164 }, null, false, "Autumn 2014", 2, 2, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 158, 159, 160, 161, 162, 163, 164 }, null, false, "Autumn 2014", 2, 2, false); }
                if (ihavethese.Contains("brickchristmas2014") || debug) { AddToolStrip(decosBMD, 2, new int[] { 165, 166, 167, 168, 169, 170, 171 }, null, false, "Christmas 2014", 2, 2, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 165, 166, 167, 168, 169, 170, 171 }, null, false, "Christmas 2014", 2, 2, false); }
                if (ihavethese.Contains("brickvalentines2015") || debug) { AddToolStrip(decosBMD, 2, new int[] { 172, 173, 174 }, null, false, "Valentines 2015", 2, 2, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 172, 173, 174 }, null, false, "Valentines 2015", 2, 2, false); }
                if (ihavethese.Contains("bricksummer2015") || debug) { AddToolStrip(decosBMD, 2, new int[] { 191, 192, 193, 194, 195 }, null, false, "Summer 2015", 2, 2, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 191, 192, 193, 194, 195 }, null, false, "Summer 2015", 2, 2, false); }

                if (ihavethese.Contains("brickdomestic") || debug)
                {
                    AddToolStrip(decosBMD, 2, new int[] { 196 }, null, false, "Domestic", 2, 2, true);
                    AddToolStrip(miscBMD, 1, new int[] { 179, 183, 187, 191, 195, 199 }, null, false, "Domestic", 2, 2, true);
                }
                else
                {
                    AddToolStrip(decosBMD, 2, new int[] { 196 }, null, false, "Domestic", 2, 2, false);
                    AddToolStrip(miscBMD, 1, new int[] { 179, 183, 187, 191, 195, 199 }, null, false, "Domestic", 2, 2, false);
                }
                if (ihavethese.Contains("brickhalloween2015") || debug)
                {
                    AddToolStrip(decosBMD, 2, new int[] { 197, 198 }, null, false, "Halloween 2015", 2, 2, true);
                    AddToolStrip(miscBMD, 1, new int[] { 215, 217, 219 }, null, false, "Halloween 2015", 2, 2, true);
                }
                else
                {
                    AddToolStrip(decosBMD, 2, new int[] { 197, 198 }, null, false, "Halloween 2015", 2, 2, false);
                    AddToolStrip(miscBMD, 1, new int[] { 215, 217, 219 }, null, false, "Halloween 2015", 2, 2, false);
                }
                if (ihavethese.Contains("bricknewyear2015") || debug)
                {
                    AddToolStrip(decosBMD, 2, new int[] { 200, 201 }, null, false, "New Year 2015", 2, 2, true);
                    AddToolStrip(miscBMD, 1, new int[] { 244, 248 }, null, false, "New Year 2015", 2, 2, true);
                }
                else
                {
                    AddToolStrip(decosBMD, 2, new int[] { 200, 201 }, null, false, "New Year 2015", 2, 2, false);
                    AddToolStrip(miscBMD, 1, new int[] { 244, 248 }, null, false, "New Year 2015", 2, 2, false);
                }
                if (ihavethese.Contains("brickfairytale") || debug)
                {
                    AddToolStrip(decosBMD, 2, new int[] { 202, 203, 204 }, null, false, "Fairytale", 2, 2, true);
                    AddToolStrip(miscBMD, 1, new int[] { 279 }, null, false, "Fairytale", 2, 2, true);
                }
                else
                {
                    AddToolStrip(decosBMD, 2, new int[] { 202, 203, 204 }, null, false, "Fairytale", 2, 2, false);
                    AddToolStrip(miscBMD, 1, new int[] { 279 }, null, false, "Fairytale", 2, 2, false);
                }
                if (ihavethese.Contains("brickspring2016") || debug)
                {
                    AddToolStrip(decosBMD, 2, new int[] { 205, 206 }, null, false, "Spring 2016", 2, 2, true);
                    AddToolStrip(miscBMD, 1, new int[] { 283, 286, 289 }, null, false, "Spring 2016", 2, 2, true);
                }
                else
                {
                    AddToolStrip(decosBMD, 2, new int[] { 205, 206 }, null, false, "Spring 2016", 2, 2, false);
                    AddToolStrip(miscBMD, 1, new int[] { 283, 286, 289 }, null, false, "Spring 2016", 2, 2, false);
                }


                if (ihavethese.Contains("bricksummer2016") || debug)
                {
                    AddToolStrip(miscBMD, 1, new int[] { 292, 298, 304 }, null, false, "Summer 2016", 2, 2, true);
                }
                else
                {
                    AddToolStrip(miscBMD, 1, new int[] { 292, 298, 304 }, null, false, "Summer 2016", 2, 2, false);
                }
                if (ihavethese.Contains("brickmine") || debug) { AddToolStrip(decosBMD, 2, new int[] { 218, 219 }, null, false, "Mine", 2, 2, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 218, 219 }, null, false, "Mine", 2, 2, false); }
                if (ihavethese.Contains("brickmine") || debug) { AddToolStrip(miscBMD, 1, new int[] { 311, 317 }, null, false, "Mine", 2, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 311, 317 }, null, false, "Mine", 2, 2, false); }
                if (ihavethese.Contains("brickrestaurant") || debug) { AddToolStrip(decosBMD, 2, new int[] { 213, 214, 215, 216, 217 }, null, false, "Restaurant", 2, 2, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 213, 214, 215, 216, 217 }, null, false, "Restaurant", 2, 2, false); }
                if (ihavethese.Contains("brickrestaurant") || debug) { AddToolStrip(miscBMD, 1, new int[] { 319, 323, 327 }, null, false, "Restaurant", 2, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 319, 323, 327 }, null, false, "Restaurant", 2, 2, false); }
                if (ihavethese.Contains("brickhalloween2016") || debug) { AddToolStrip(decosBMD, 2, new int[] { 220 }, null, false, "Halloween 2016", 2, 2, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 220 }, null, false, "Halloween 2016", 2, 2, false); }
                if (ihavethese.Contains("brickhalloween2016") || debug) { AddToolStrip(miscBMD, 1, new int[] { 331, 335, 338 }, null, false, "Halloween 2016", 2, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 331, 335, 338 }, null, false, "Halloween 2016", 2, 2, false); }
                AddToolStrip(decosBMD, 2, new int[] { 221, 222, 223 }, null, false, "Construction", 2, 2, true);
                if (ihavethese.Contains("brickchristmas2016") || debug)
                {

                    AddToolStrip(miscBMD, 1, new int[] { 343, 348 }, null, false, "Christmas 2016", 2, 2, true);
                    AddToolStrip(decosBMD, 2, new int[] { 224, 225 }, null, false, "Christmas 2016", 2, 2, true);
                    AddToolStrip(miscBMD, 1, new int[] { 352 }, null, false, "Christmas 2016", 2, 2, true);
                }
                else
                {

                    AddToolStrip(miscBMD, 1, new int[] { 343, 348 }, null, false, "Christmas 2016", 2, 2, false);
                    AddToolStrip(decosBMD, 2, new int[] { 224, 225 }, null, false, "Christmas 2016", 2, 2, false);
                    AddToolStrip(miscBMD, 1, new int[] { 352 }, null, false, "Christmas 2016", 2, 2, false);
                }
            if (ihavethese.Contains("brickstpatricks2017") || debug) { AddToolStrip(decosBMD, 2, new int[] { 226, 227, 228, 229, 230 }, null, false, "St. Patricks 2017", 2, 2, true); }
            else { AddToolStrip(decosBMD, 2, new int[] { 226,227,228,229,230 }, null, false, "St. Patricks 2017", 2, 2, false); }
            //Decorations 3








            #endregion

            #region Action
            //Action 1

            //Everyone have these
            if (!userdata.openWorld)
            {
                AddToolStrip(foregroundBMD, 0, new int[] { 0 }, null, true, "Empty", 1, 0, true);
                AddToolStrip(foregroundBMD, 0, new int[] { 1, 2, 3, 4, 233 }, null, false, "Gravity", 1, 0, true);
                AddToolStrip(miscBMD, 1, new int[] { 70, 71, 72, 73, 222 }, null, false, "Invisible Gravity", 1, 0, true);
                AddToolStrip(foregroundBMD, 0, new int[] { 6, 7, 8, 189, 190, 191 }, new uint[] { 0x2C1A1A, 0x1A2C1A, 0x1A1A2C, 0x0C2D3D, 0x400C40, 0x2C330A }, false, "Keys", 1, 0, true);
                AddToolStrip(foregroundBMD, 0, new int[] { 26, 27, 28, 195, 196, 197 }, new uint[] { 0x9C2D46, 0x379C30, 0x2D449C, 0x2D8D99, 0x912D99, 0x97922D }, false, "Gates", 1, 0, true);
                AddToolStrip(foregroundBMD, 0, new int[] { 23, 24, 25, 192, 193, 194 }, new uint[] { 0x9C2D46, 0x379C30, 0x2D449C, 0x2D8D99, 0x912D99, 0x97922D }, false, "Doors", 1, 0, true);
                AddToolStrip(miscBMD, 1, new int[] { 174, 175 }, null, false, "Coins", 1, 0, true);

                AddToolStrip(foregroundBMD, 0, new int[] { 139, 43 }, new uint[] { 0xB88E15, 0xB88E15 }, false, "Yellow coin doors/gates", 1, 0, true);
                AddToolStrip(foregroundBMD, 0, new int[] { 186, 185 }, new uint[] { 0x1C60F4, 0x1C60F4 }, false, "Blue coin doors/gates", 1, 0, true);
                AddToolStrip(decosBMD, 2, new int[] { 38 }, null, false, "Tools", 1, 0, true);
                AddToolStrip(miscBMD, 1, new int[] { 27 }, null, false, "Tools", 1, 0, true);
                AddToolStrip(miscBMD, 1, new int[] { 8 }, null, false, "Tools", 1, 0, true);
                AddToolStrip(decosBMD, 2, new int[] { 199 }, null, false, "Tools", 1, 0, true);
                AddToolStrip(foregroundBMD, 0, new int[] { 5 }, new uint[] { 0x43391F }, false, "Crown", 1, 0, true);
                if (ihavethese.Contains("brickcrowndoor") || debug) { AddToolStrip(miscBMD, 1, new int[] { 341, 340 }, null, false, "Crown Doors", 1, 0, true); }
                else AddToolStrip(miscBMD, 1, new int[] { 341, 340 }, null, false, "Crown Doors", 1, 0, false);
                if (ihavethese.Contains("brickboost") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 157, 158, 159, 160 }, null, false, "Boost", 1, 0, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 157, 158, 159, 160 }, null, false, "Boost", 1, 0, false); }
                if (ihavethese.Contains("brickmedieval") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 98 }, null, false, "Climbable", 1, 0, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 98 }, null, false, "Climbable", 1, 0, false); }
                if (ihavethese.Contains("brickninja") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 135 }, null, false, "Climbable", 1, 0, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 135 }, null, false, "Climbable", 1, 0, false); }
                AddToolStrip(foregroundBMD, 0, new int[] { 174, 175 }, null, false, "Climbable", 1, 0, true);
                if (ihavethese.Contains("brickcowboy") || debug) { AddToolStrip(decosBMD, 2, new int[] { 177 }, null, false, "Climbable", 1, 0, true); }
                else { AddToolStrip(decosBMD, 2, new int[] { 177 }, null, false, "Climbable", 1, 0, false); }
                if (ihavethese.Contains("brickfairytale") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 252 }, null, false, "Climbable", 1, 0, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 252 }, null, false, "Climbable", 1, 0, false); }
                if (ihavethese.Contains("brickswitchpurple") || debug) { AddToolStrip(miscBMD, 1, new int[] { 3, 4, 5 }, null, false, "Purple Switches", 1, 0, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 3, 4, 5 }, null, false, "Purple Switches", 1, 0, false); }
                if (ihavethese.Contains("brickswitchorange") || debug) { AddToolStrip(miscBMD, 1, new int[] { 259, 261, 262 }, null, false, "Orange Switches", 1, 0, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 259, 261, 262 }, null, false, "Orange Switches", 1, 0, false); }
                if (ihavethese.Contains("brickdeathdoor") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 198, 199 }, new uint[] { 0xA9A9A9, 0xA9A9A9 }, false, "Death", 1, 0, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 198, 199 }, new uint[] { 0xA9A9A9, 0xA9A9A9 }, false, "Death", 1, 0, false); }
                if (ihavethese.Contains("brickeffectzombie") || debug) { AddToolStrip(miscBMD, 1, new int[] { 79, 32, 31 }, null, false, "Zombie", 1, 0, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 79, 32, 31 }, null, false, "Zombie", 1, 0, false); }
                if (ihavethese.Contains("brickeffectteam") || debug) { AddToolStrip(miscBMD, 1, new int[] { 80, 100, 93 }, null, false, "Teams", 1, 0, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 80, 100, 93 }, null, false, "Teams", 1, 0, false); }
                if (ihavethese.Contains("bricktimeddoor") || debug) { AddToolStrip(miscBMD, 1, new int[] { 6, 7 }, null, false, "Timed Doors", 1, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 6, 7 }, null, false, "Timed Doors", 1, 2, false); }
                if (ihavethese.Contains("brickdrums") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 83 }, null, false, "Music", 1, 2, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 83 }, null, false, "Music", 1, 2, false); }
                if (ihavethese.Contains("bricknode") || debug) { AddToolStrip(foregroundBMD, 0, new int[] { 77 }, null, false, "Music", 1, 2, true); }
                else { AddToolStrip(foregroundBMD, 0, new int[] { 77 }, null, false, "Music", 1, 2, false); }
                if (ihavethese.Contains("brickfire") || debug) { AddToolStrip(miscBMD, 1, new int[] { 28 }, null, false, "Hazards", 1, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 28 }, null, false, "Hazards", 1, 2, false); }
                if (ihavethese.Contains("brickspike") || debug) { AddToolStrip(miscBMD, 1, new int[] { 24 }, null, false, "Hazards", 1, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 24 }, null, false, "Hazards", 1, 2, false); }
                AddToolStrip(miscBMD, 1, new int[] { 0 }, null, false, "Liquids", 1, 2, true);

                if (ihavethese.Contains("bricklava") || debug) { AddToolStrip(miscBMD, 1, new int[] { 107 }, null, false, "Liquids", 1, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 107 }, null, false, "Liquids", 1, 2, false); }
                if (ihavethese.Contains("brickswamp") || debug) { AddToolStrip(miscBMD, 1, new int[] { 29 }, null, false, "Liquids", 1, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 29 }, null, false, "Liquids", 1, 2, false); }
                if (ihavethese.Contains("brickinvisibleportal") || debug) { AddToolStrip(miscBMD, 1, new int[] { 112 }, null, false, "Portals", 1, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 112 }, null, false, "Portals", 1, 2, false); }
                if (ihavethese.Contains("brickportal") || debug) { AddToolStrip(miscBMD, 1, new int[] { 108 }, null, false, "Portals", 1, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 108 }, null, false, "Portals", 1, 2, false); }
                if (ihavethese.Contains("brickworldportal") || debug) { AddToolStrip(miscBMD, 1, new int[] { 33 }, null, false, "Portals", 1, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 33 }, null, false, "Portals", 1, 2, false); }
                if (ihavethese.Contains("brickdiamond") || debug) { AddToolStrip(miscBMD, 1, new int[] { 221 }, null, false, "Diamond", 1, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 221 }, null, false, "Diamond", 1, 2, false); }
                if (ihavethese.Contains("brickcake") || debug) { AddToolStrip(miscBMD, 1, new int[] { 2 }, null, false, "Cake", 1, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 2 }, null, false, "Cake", 1, 2, false); }
                if (ihavethese.Contains("brickhologram") || debug) { AddToolStrip(miscBMD, 1, new int[] { 53 }, null, false, "Hologram", 1, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 53 }, null, false, "Hologram", 1, 2, false); }
                if (ihavethese.Contains("bricksign") || debug) { AddToolStrip(miscBMD, 1, new int[] { 255 }, null, false, "Sign", 1, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 255 }, null, false, "Sign", 1, 2, false); }
                if (ihavethese.Contains("brickeffectjump") || debug) { AddToolStrip(miscBMD, 1, new int[] { 74 }, null, false, "Effects", 1, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 74 }, null, false, "Effects", 1, 2, false); }
                if (ihavethese.Contains("brickeffectfly") || debug) { AddToolStrip(miscBMD, 1, new int[] { 75 }, null, false, "Effects", 1, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 75 }, null, false, "Effects", 1, 2, false); }
                if (ihavethese.Contains("brickeffectspeed") || debug) { AddToolStrip(miscBMD, 1, new int[] { 76 }, null, false, "Effects", 1, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 76 }, null, false, "Effects", 1, 2, false); }
                if (ihavethese.Contains("brickeffectlowgravity") || debug) { AddToolStrip(miscBMD, 1, new int[] { 176 }, null, false, "Effects", 1, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 176 }, null, false, "Effects", 1, 2, false); }
                if (ihavethese.Contains("brickeffectprotection") || debug) { AddToolStrip(miscBMD, 1, new int[] { 77 }, null, false, "Effects", 1, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 77 }, null, false, "Effects", 1, 2, false); }
                if (ihavethese.Contains("brickeffectcurse") || debug) { AddToolStrip(miscBMD, 1, new int[] { 78 }, null, false, "Effects", 1, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 78 }, null, false, "Effects", 1, 2, false); }
                if (ihavethese.Contains("brickeffectmultijump") || debug) { AddToolStrip(miscBMD, 1, new int[] { 253 }, null, false, "Effects", 1, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 253 }, null, false, "Effects", 1, 2, false); }
                if (ihavethese.Contains("goldmember") || debug) { AddToolStrip(miscBMD, 1, new int[] { 12, 13 }, new uint[] { 0x281C00, 0xBA983B }, false, "Gold Membership", 1, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 12, 13 }, new uint[] { 0x281C00, 0xBA983B }, false, "Gold Membership", 1, 2, false); }
                if (ihavethese.Contains("brickice2") || debug) { AddToolStrip(miscBMD, 1, new int[] { 251 }, new uint[] { 0x409EB1 }, false, "Ice", 1, 2, true); }
                else { AddToolStrip(miscBMD, 1, new int[] { 251 }, new uint[] { 0x409EB1 }, false, "Ice", 1, 2, false); }
            }
            else
            {
                AddToolStrip(foregroundBMD, 0, new int[] { 0 }, null, true, "Empty", 1, 0, true);
                AddToolStrip(foregroundBMD, 0, new int[] { 1, 2, 3, 4, }, null, false, "Gravity", 1, 0, true);
                AddToolStrip(miscBMD, 1, new int[] { 70, 71, 72, 73, }, null, false, "Invisible Gravity", 1, 0, true);
                AddToolStrip(foregroundBMD, 0, new int[] { 5 }, new uint[] { 0x43391F }, false, "Tools", 1, 0, true);
            }
            #endregion

            #region Background
            //Backgrounds
            if (!userdata.openWorld)
            {
                AddToolStrip(backgroundBMD, 3, new int[] { 209, 0, 1, 2, 3, 4, 5, 6, 138, 139 }, new uint[] { 0x707070, 0x343434, 0x1A2955, 0x4A1751, 0x551A2A, 0x465217, 0x1E5218, 0x174F53, 0x6F370B, 0x050505 }, false, "Basic", 3, 0, true);
                AddToolStrip(backgroundBMD, 3, new int[] { 210, 8, 9, 10, 11, 12, 140, 141, 142, 7 }, new uint[] { 0x5B5B5B, 0x113726, 0x251136, 0x214108, 0x371214, 0x372E12, 0x282828, 0x051132, 0x0F0F0F, 0x441D04 }, false, "Brick", 3, 0, true);
                AddToolStrip(backgroundBMD, 3, new int[] { 212, 13, 14, 15, 16, 17, 18, 19, 143, 144 }, new uint[] { 0x6B6B6B, 0x3C3C3C, 0x1F365F, 0x552860, 0x5E0E23, 0x525A1D, 0x25591E, 0x236764, 0x834A1A, 0x191919 }, false, "Checker", 3, 0, true);
                AddToolStrip(backgroundBMD, 3, new int[] { 213, 20, 21, 22, 23, 24, 25, 26, 145, 146 }, new uint[] { 0x636363, 0x353535, 0x1C325D, 0x4C1853, 4283501598, 0x485318, 0x1D5318, 0x1C5D5B, 0x7A4111, 0x121212 }, false, "Dark", 3, 0, true);
                AddToolStrip(backgroundBMD, 3, new int[] { 211, 110, 111, 112, 113, 114, 115, 116, 147, 148 }, new uint[] { 0x747474, 0x434343, 0x233A61, 0x5E386E, 0x6C1029, 0x5D6123, 0x2E5F24, 0x2B716E, 0x8C5323, 0x202020 }, false, "Normal", 3, 0, true);
                AddToolStrip(backgroundBMD, 3, new int[] { 27, 28, 29, 30, 31, 32, 170, 171 }, new uint[] { 0xFCECA8, 0xB0FCA8, 0xD8FCA8, 0xA8FBFC, 0xA8C0FC, 0xFF9B9B, 0xFFC19B, 0xD2A4FF }, false, "Pastel", 3, 0, true);
                if (ihavethese.Contains("brickbgcanvas") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 33, 34, 35, 36, 37, 38, 106, 165, 166 }, new uint[] { 0x8A3C20, 0x696040, 0x866D25, 0x648B20, 0x20648B, 0x3C4048, 0x0E2C86, 0x94181C, 0x62217E }, false, "Canvas", 3, 0, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 33, 34, 35, 36, 37, 38, 106, 165, 166 }, new uint[] { 0x8A3C20, 0x696040, 0x866D25, 0x648B20, 0x20648B, 0x3C4048, 0x0E2C86, 0x94181C, 0x62217E }, false, "Canvas", 3, 0, false); }
                if (ihavethese.Contains("brickbgcarnival") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 45, 46, 47, 48, 49, 58, 63, 107 }, new uint[] { 0xC9763C, 0x2E1B70, 0xA54880, 0x5D5D5D, 0x039119, 0x958634, 0xD08C85, 0x051578 }, false, "Carnival", 3, 0, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 45, 46, 47, 48, 49, 58, 63, 107 }, new uint[] { 0xC9763C, 0x2E1B70, 0xA54880, 0x5D5D5D, 0x039119, 0x958634, 0xD08C85, 0x051578 }, false, "Carnival", 3, 0, false); }
                if (ihavethese.Contains("brickcandy") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 39, 40 }, new uint[] { 0xFEA8C2, 0xC7D9FF }, false, "Candy", 3, 0, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 39, 40 }, new uint[] { 0xFEA8C2, 0xC7D9FF }, false, "Candy", 3, 0, false); }
                if (ihavethese.Contains("brickhw2011") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 41, 42, 43, 44 }, new uint[] { 0x454545, 0x293134, 0x252C2E, 0x262C2F }, false, "Halloween 2011", 3, 0, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 41, 42, 43, 44 }, new uint[] { 0x454545, 0x293134, 0x252C2E, 0x262C2F }, false, "Halloween 2011", 3, 0, false); }
                if (ihavethese.Contains("brickscifi") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 131 }, new uint[] { 0x737D81 }, false, "Sci-Fi", 3, 0, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 131 }, new uint[] { 0x737D81 }, false, "Sci-Fi", 3, 0, false); }
                if (ihavethese.Contains("brickprison") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 50, 51, 52, 53 }, new uint[] { 0x5A5A5A, 0x8A796D, 0x747A88, 0x4A4A4A }, false, "Prison", 3, 0, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 50, 51, 52, 53 }, new uint[] { 0x5A5A5A, 0x8A796D, 0x747A88, 0x4A4A4A }, false, "Prison", 3, 0, false); }
                if (ihavethese.Contains("brickpirate") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 54, 55, 59, 60 }, new uint[] { 0x664F34, 0x836642, 0x4B3A25, 0x474747 }, false, "Pirate", 3, 0, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 54, 55, 59, 60 }, new uint[] { 0x664F34, 0x836642, 0x4B3A25, 0x474747 }, false, "Pirate", 3, 0, false); }
                if (ihavethese.Contains("brickstone") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 61, 62, 182, 183, 184, 185, 186, 187 }, new uint[] { 0x3B3F44, 0x3B3F44, 0x2F4B3E, 0x2F4B3E, 0x4C341A, 0x4C341A, 0x2F3954, 0x2F3954 }, false, "Stone", 3, 0, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 61, 62, 182, 183, 184, 185, 186, 187 }, new uint[] { 0x3B3F44, 0x3B3F44, 0x2F4B3E, 0x2F4B3E, 0x4C341A, 0x4C341A, 0x2F3954, 0x2F3954 }, false, "Stone", 3, 0, false); }
                if (ihavethese.Contains("brickninja") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 64, 65, 66, 67, 161, 162, 163, 164 }, new uint[] { 0xEFEEE9, 0x93928E, 0x525A70, 0x303541, 0x7B3C43, 0x482327, 0x587052, 0x334130 }, false, "Dojo", 3, 0, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 64, 65, 66, 67, 161, 162, 163, 164 }, new uint[] { 0xEFEEE9, 0x93928E, 0x525A70, 0x303541, 0x7B3C43, 0x482327, 0x587052, 0x334130 }, false, "Dojo", 3, 0, false); }
                if (ihavethese.Contains("brickcowboy") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 68, 69, 70, 71, 72, 73 }, new uint[] { 0x93674D, 0x70513F, 0xA9312E, 0x822F2C, 0x57769D, 0x495A74 }, false, "Wild West", 3, 1, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 68, 69, 70, 71, 72, 73 }, new uint[] { 0x93674D, 0x70513F, 0xA9312E, 0x822F2C, 0x57769D, 0x495A74 }, false, "Wild West", 3, 1, false); }
                AddToolStrip(backgroundBMD, 3, new int[] { 74, 75, 76, 77, 78 }, new uint[] { 0x75DAE7, 0x75DAE7, 0x75DAE7, 0x75DAE7, 0x75DAE7 }, false, "Water", 3, 1, true);
                if (ihavethese.Contains("bricksand") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 79, 80, 81, 82, 83, 84 }, new uint[] { 0xD0C49C, 0xAFA78A, 0xD8CA64, 0xD1A338, 0xCEAD7D, 0x7C5E3C }, false, "Sand", 3, 1, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 79, 80, 81, 82, 83, 84 }, new uint[] { 0xD0C49C, 0xAFA78A, 0xD8CA64, 0xD1A338, 0xCEAD7D, 0x7C5E3C }, false, "Sand", 3, 1, false); }
                if (ihavethese.Contains("brickindustrial") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 85, 86, 87, 88, 89 }, new uint[] { 0x333333, 0x575757, 0x296381, 0x4F6639, 0x695319 }, false, "Industrial", 3, 1, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 85, 86, 87, 88, 89 }, new uint[] { 0x333333, 0x575757, 0x296381, 0x4F6639, 0x695319 }, false, "Industrial", 3, 1, false); }
                AddToolStrip(backgroundBMD, 3, new int[] { 94, 95, 96, 97, 98 }, new uint[] { 0xCFCBB8, 0xAFA898, 0xA8A18F, 0xA8A18E, 0x93866E }, false, "Clay", 3, 1, true);
                if (ihavethese.Contains("brickmedieval") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 99, 100, 90, 91, 92, 56, 93 }, new uint[] { 0x2D353B, 0x836C49, 0xD0BE83, 0x71331D, 0x556D61, 0x684B2C, 0xC7BFA6 }, false, "Medieval", 3, 1, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 99, 100, 90, 91, 92, 56, 93 }, new uint[] { 0x2D353B, 0x836C49, 0xD0BE83, 0x71331D, 0x556D61, 0x684B2C, 0xC7BFA6 }, false, "Medieval", 3, 1, false); }
                AddToolStrip(backgroundBMD, 3, new int[] { 101, 102, 103, 104 }, new uint[] { 0x807F86, 0x2852DE, 0x2E8500, 0x80253E }, false, "Outer Space", 3, 1, true);
                AddToolStrip(backgroundBMD, 3, new int[] { 193, 194, 195 }, new uint[] { 0x824500, 0x824700, 0x7F4602 }, false, "Desert", 3, 1, true);
                if (ihavethese.Contains("brickneon") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 105, 167, 168, 169, 191, 192 }, new uint[] { 0x041E75, 0xAA3E1C, 0x3C6B00, 0xAB003E, 0xA8A800, 0x0DA874 }, false, "Neon", 3, 1, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 105, 167, 168, 169, 191, 192 }, new uint[] { 0x041E75, 0xAA3E1C, 0x3C6B00, 0xAB003E, 0xA8A800, 0x0DA874 }, false, "Neon", 3, 1, false); }
                if (ihavethese.Contains("brickmonster") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 108, 109, 157, 158, 159, 160 }, new uint[] { 0xA0A061, 0x707044, 0x80353C, 0x631C25, 0x65236B, 0x421746 }, false, "Monster", 3, 1, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 108, 109, 157, 158, 159, 160 }, new uint[] { 0xA0A061, 0x707044, 0x80353C, 0x631C25, 0x65236B, 0x421746 }, false, "Monster", 3, 1, false); }
                AddToolStrip(backgroundBMD, 3, new int[] { 117, 118, 119, 120, 121, 122, 123 }, new uint[] { 0x666651, 0x774E44, 0x415A66, 0x6B6834, 0x688403, 0x587003, 0x425402 }, false, "Jungle", 3, 2, true);
                if (ihavethese.Contains("brickxmas2012") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 124, 125, 126 }, new uint[] { 0xD88A19, 0x54840D, 0x1F39D8 }, false, "Christmas 2012", 3, 2, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 124, 125, 126 }, new uint[] { 0xD88A19, 0x54840D, 0x1F39D8 }, false, "Christmas 2012", 3, 2, false); }
                if (ihavethese.Contains("bricklava") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 127, 128, 129 }, new uint[] { 0xCCA333, 0xC6750B, 0xB73A00 }, false, "Lava", 3, 2, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 127, 128, 129 }, new uint[] { 0xCCA333, 0xC6750B, 0xB73A00 }, false, "Lava", 3, 2, false); }
                if (ihavethese.Contains("brickswamp") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 57, 130 }, new uint[] { 0x7B5641, 0x605A24 }, false, "Swamp", 3, 2, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 57, 130 }, new uint[] { 0x7B5641, 0x605A24 }, false, "Swamp", 3, 2, false); }
                if (ihavethese.Contains("bricksparta") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 132, 133, 134 }, new uint[] { 0x777B7D, 0x70816F, 0x83767B }, false, "Marble", 3, 2, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 132, 133, 134 }, new uint[] { 0x777B7D, 0x70816F, 0x83767B }, false, "Marble", 3, 2, false); }
                if (ihavethese.Contains("brickautumn2014") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 135, 136, 137 }, new uint[] { 0x695102, 0x692602, 0x690503 }, false, "Autumn 2014", 3, 2, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 135, 136, 137 }, new uint[] { 0x695102, 0x692602, 0x690503 }, false, "Autumn 2014", 3, 2, false); }
                if (ihavethese.Contains("brickcave") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 149, 150, 151, 152, 153, 154, 155, 156 }, new uint[] { 0x200426, 0x041E20, 0x030C1F, 0x2C051A, 0x081602, 0x240D05, 0x321A08, 0x330909 }, false, "Cave", 3, 2, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 149, 150, 151, 152, 153, 154, 155, 156 }, new uint[] { 0x200426, 0x041E20, 0x030C1F, 0x2C051A, 0x081602, 0x240D05, 0x321A08, 0x330909 }, false, "Cave", 3, 2, false); }
                AddToolStrip(backgroundBMD, 3, new int[] { 172, 173, 174, 175, 176 }, new uint[] { 0x571802, 0x245100, 0x754705, 0x2C3244, 0x551A08 }, false, "Environment", 3, 2, true);
                if (ihavethese.Contains("brickdomestic") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 177, 178, 179, 180, 181 }, new uint[] { 0x624616, 0x371A0D, 0x4A0C07, 0x11304E, 0x063F14 }, false, "Domestic", 3, 2, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 177, 178, 179, 180, 181 }, new uint[] { 0x624616, 0x371A0D, 0x4A0C07, 0x11304E, 0x063F14 }, false, "Domestic", 3, 2, false); }
                if (ihavethese.Contains("brickhalloween2015") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 188, 189, 190 }, new uint[] { 0x1D310C, 0x3B3F35, 0x161D14 }, false, "Halloween 2015", 3, 2, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 188, 189, 190 }, new uint[] { 0x1D310C, 0x3B3F35, 0x161D14 }, false, "Halloween 2015", 3, 2, false); }
                AddToolStrip(backgroundBMD, 3, new int[] { 196, 197 }, new uint[] { 0x1E3C77, 0x3E4D6A }, false, "Arctic", 3, 2, true);
                if (debug) { AddToolStrip(backgroundBMD, 3, new int[] { 198, 199, 200 }, new uint[] { 0x82600E, 0x825707, 0x7E5C10 }, false, "Gold Membership", 3, 2, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 198, 199, 200 }, new uint[] { 0x82600E, 0x825707, 0x7E5C10 }, false, "Gold Membership", 3, 2, false); }
                if (ihavethese.Contains("brickfairytale") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 201, 202, 203, 204 }, new uint[] { 0xD68E64, 0x7EB26E, 0x5DA8BE, 0xD46EB0 }, false, "Fairytale", 3, 2, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 201, 202, 203, 204 }, new uint[] { 0xD68E64, 0x7EB26E, 0x5DA8BE, 0xD46EB0 }, false, "Fairytale", 3, 2, false); }
                if (ihavethese.Contains("bricksummer2016") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 205, 206, 207, 208 }, new uint[] { 0x7C5826, 0x682D63, 0x886200, 0x206B34 }, false, "Summer 2016", 3, 2, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 205, 206, 207, 208 }, new uint[] { 0x7C5826, 0x682D63, 0x886200, 0x206B34 }, false, "Summer 2016", 3, 2, false); }
                if (ihavethese.Contains("brickmine") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 219 }, new uint[] { 0x511000 }, false, "Mine", 3, 2, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 219 }, new uint[] { 0x511000 }, false, "Mine", 3, 2, false); }
                if (ihavethese.Contains("bricktextile") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 214, 215, 216, 217, 218 }, new uint[] { 0x87A884, 0x7D92A7, 0xB395AC, 0xA29E72, 0xA47E7E }, false, "Textile", 3, 2, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 214, 215, 216, 217, 218 }, new uint[] { 0x87A884, 0x7D92A7, 0xB395AC, 0xA29E72, 0xA47E7E }, false, "Textile", 3, 2, false); }

                if (ihavethese.Contains("brickhalloween2016") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 220, 221 }, new uint[] { 0x201E1A, 0x2E1A37 }, false, "Halloween 2016", 3, 2, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 220, 221 }, new uint[] { 0x201E1A, 0x2E1A37 }, false, "Halloween 2016", 3, 2, false); }
                AddToolStrip(backgroundBMD, 3, new int[] { 222, 223, 224, 225, 226 }, new uint[] { 0x8A5E2B, 0x515151, 0x7C6241, 0x852310, 0x852210 }, false, "Construction", 3, 2, true);

                if (ihavethese.Contains("bricktiles") || debug) { AddToolStrip(backgroundBMD, 3, new int[] { 227, 228, 229, 230, 231, 232, 233, 234, 235, 236 }, new uint[] { 0x646049, 0x575448, 0x4B493A, 0x783E3E, 0x734530, 0x634E27, 0x455C3C, 0x3E6054, 0x445365, 0x534766 }, false, "Tiles", 3, 2, true); }
                else { AddToolStrip(backgroundBMD, 3, new int[] { 227, 228, 229, 230, 231, 232, 233, 234, 235, 236 }, new uint[] { 0x646049, 0x575448, 0x4B493A, 0x783E3E, 0x734530, 0x634E27, 0x455C3C, 0x3E6054, 0x445365, 0x534766 }, false, "Tiles", 3, 2, false); }
            }
            #endregion

            if (userdata.newestBlocks.Count >= 1)
            {
                try
                {
                    foreach (JToken lst in userdata.newestBlocks)
                    {
                        if (lst != null)
                        {
                            AddToolStrip(foregroundBMD, 0, new int[] { Convert.ToInt32(lst) }, null, false, "Unknown", 0, 4, true, true);
                        }
                    }
                }
                catch { }
            }
            if (fromclient)
            {

                if (flowLayoutPanel2.InvokeRequired) { this.Invoke((MethodInvoker)delegate { flowLayoutPanel2.Visible = true; }); }
                if (flowLayoutPanel3.InvokeRequired) { this.Invoke((MethodInvoker)delegate { flowLayoutPanel3.Visible = false; }); }
                if (flowLayoutPanel4.InvokeRequired) { this.Invoke((MethodInvoker)delegate { flowLayoutPanel4.Visible = false; }); }
                if (flowLayoutPanel5.InvokeRequired) { this.Invoke((MethodInvoker)delegate { flowLayoutPanel5.Visible = false; }); }
                if (flowLayoutPanel6.InvokeRequired) { this.Invoke((MethodInvoker)delegate { flowLayoutPanel6.Visible = false; }); }
                if (starting)
                {
                    starting = false;
                    this.Invoke((MethodInvoker)delegate { editArea.Init(25, 25); });

                }
            }
            else
            {

                if (flowLayoutPanel2.InvokeRequired) { this.Invoke((MethodInvoker)delegate { flowLayoutPanel2.Visible = true; }); }
                if (flowLayoutPanel3.InvokeRequired) { this.Invoke((MethodInvoker)delegate { flowLayoutPanel3.Visible = false; }); }
                if (flowLayoutPanel4.InvokeRequired) { this.Invoke((MethodInvoker)delegate { flowLayoutPanel4.Visible = false; }); }
                if (flowLayoutPanel5.InvokeRequired) { this.Invoke((MethodInvoker)delegate { flowLayoutPanel5.Visible = false; }); }
                if (flowLayoutPanel6.InvokeRequired) { this.Invoke((MethodInvoker)delegate { flowLayoutPanel6.Visible = false; }); }
                else
                {
                    flowLayoutPanel2.Visible = true;
                    flowLayoutPanel3.Visible = false;
                    flowLayoutPanel4.Visible = false;
                    flowLayoutPanel5.Visible = false;
                    flowLayoutPanel6.Visible = false;
                }
                starting = false;
            }


            //passTimer = new System.Threading.Timer(x => editArea.Init(25, 25), null, 5000, Timeout.Infinite);
        }

        private delegate void AddToolStripCallback(Image target, int mode, int[] ids, uint[] colors = null, bool setFirst = false, string desc = "Unknown", int toolstrip = 0, int row = 0, bool doihave = false, bool unknown = false);
        protected void AddToolStrip(Image target, int mode, int[] ids, uint[] colors = null, bool setFirst = false, string desc = "Unknown", int toolstrip = 0, int row = 0, bool doihave = false, bool unknown = false)
        {
            if (this.InvokeRequired)
            {
                AddToolStripCallback method = new AddToolStripCallback(AddToolStrip);
                base.Invoke(method, new object[] { target, mode, ids, colors, setFirst, desc, toolstrip, row, doihave, unknown });
            }
            else
            {
                if (!unknown)
                {
                    if (doihave && !debug)
                    {

                        int[] values = new int[ids.Length];
                        for (int i = 0; i < ids.Length; i++)
                        {
                            if (mode == 0) values[i] += blocks[ids[i]];
                            if (mode == 1) values[i] += misc[ids[i]];
                            if (mode == 2) values[i] += decos[ids[i]];
                            if (mode == 3) values[i] += bgs[ids[i]];
                        }
                        ownedb.Add(new ownedBlocks() { mode = mode, blocks = values, name = desc });
                    }

                    int length = ids.Length;
                    Bitmap bitmap = new Bitmap(target);
                    int n = bitmap.Width / 16;
                    var bid = 0;
                    BrickButton[] items = new BrickButton[length];


                    for (int j = 0; j < length; ++j)
                    {

                        bid = ids[j];
                        Bitmap brick = bitmap.Clone(new Rectangle(16 * ids[j], 0, 16, 16), System.Drawing.Imaging.PixelFormat.DontCare);
                        if (mode == 0)
                        {
                            ids[j] = blocks[ids[j]];
                        }
                        else if (mode == 1)
                        {
                            ids[j] = misc[ids[j]];
                        }
                        else if (mode == 2)
                        {
                            ids[j] = decos[ids[j]];
                        }
                        else if (mode == 3)
                        {
                            ids[j] = bgs[ids[j]];
                        }
                        int i = ids[j];
                        if (userdata.newestBlocks.Count >= 1)
                        {
                            for (int a = 0; a < userdata.newestBlocks.Count; a++)
                            {
                                if (ids[j].ToString() == userdata.newestBlocks[a].ToString())
                                {
                                    JToken jt = userdata.newestBlocks[a];
                                    userdata.newestBlocks.Remove(jt);
                                }
                            }
                        }

                        editArea.Bricks[ids[j]] = brick;

                        editArea.BricksFade[ids[j]] = Fade(brick);

                        items[j] = new BrickButton(brick, this, SetBrick, ids[j], bid, true, mode, desc);
                        //else items[j] = new BrickButton(brick, this, SetBrick, ids[j], bid, false, mode, desc);
                        items[j].MainForm = this;
                        if (ids[j] == 9 && setFirst)
                        {
                            selectedBrick = items[j];
                            selectedBrick.Checked = true;
                            editArea.Tool.PenID = i;
                        }
                        if (colors != null)
                        {
                            Minimap.Colors[ids[j]] = (0xffu << 24) | colors[j];
                            Minimap.ImageColor[ids[j]] = true;
                            if (doihave)
                            {
                                if (i < 500 || i >= 1001)
                                {
                                    if (bdata.morphable.Contains(ids[j]))
                                    {
                                        InsertImageForm.SpecialMorph.Add(i);
                                    }
                                    else if (bdata.goal.Contains(ids[j]))
                                    {
                                        InsertImageForm.SpecialAction.Add(i);
                                    }
                                    else if (!bdata.rotate.Contains(ids[j]) && !bdata.portals.Contains(ids[j]))
                                    {
                                        InsertImageForm.Blocks.Add(i);
                                    }

                                }
                                else if (i >= 500 && i <= 999)
                                {
                                    InsertImageForm.Background.Add(i);
                                }
                            }
                        }
                    }


                    if (doihave)
                    {
                        ToolStrip strip = new ToolStrip();
                        if (searched != null && items[0].blockInfo.ToLower().Contains(searched) || filterTextBox.Text == string.Empty)
                        {
                            strip = new ToolStrip(items);
                        }
                        if (strip.Items.Count > 0)
                        {
                            foreach (int id in ids)
                            {
                                if (!tps.ContainsKey(id.ToString()))
                                {
                                    tps.Add(id.ToString(), strip);
                                }
                            }
                            strip.AutoSize = true;
                            strip.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
                            strip.Renderer = new removeBadRenderer();
                            strip.GripStyle = ToolStripGripStyle.Hidden;
                            strip.BackColor = MainForm.userdata.themeBlocks;
                            ToolTip tip = new ToolTip();
                            tip.SetToolTip(strip, desc);

                            switch (toolstrip)
                            {
                                case 0:
                                    flowLayoutPanel2.Controls.Add(strip);
                                    break;
                                case 1:
                                    flowLayoutPanel3.Controls.Add(strip);
                                    break;
                                case 2:
                                    flowLayoutPanel4.Controls.Add(strip);
                                    break;
                                case 3:
                                    flowLayoutPanel5.Controls.Add(strip);
                                    break;
                            }
                        }
                    }
                }
                else
                {

                    Bitmap bitmap = new Bitmap(Properties.Resources.bullets);
                    Bitmap brick = new Bitmap(16, 16);
                    if (ids[0] < 500 || ids[0] >= 1001)
                    {
                        brick = bitmap.Clone(new Rectangle(16 * 7, 0, 16, 16), System.Drawing.Imaging.PixelFormat.DontCare);
                    }
                    else if (ids[0] >= 500 && ids[0] <= 999)
                    {
                        brick = bitmap.Clone(new Rectangle(16 * 5, 0, 16, 16), System.Drawing.Imaging.PixelFormat.DontCare);
                    }
                    BrickButton items = new BrickButton(brick, this, SetBrick, ids[0], 0, true, mode, "no");
                    //editArea.Bricks[9] = brick;
                    ToolStrip strip = new ToolStrip(items);
                    strip.AutoSize = true;
                    strip.GripStyle = ToolStripGripStyle.Hidden;
                    strip.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
                    strip.Renderer = new removeBadRenderer();
                    strip.BackColor = MainForm.userdata.themeBlocks;
                    foreach (int id in ids)
                    {
                        if (!tps.ContainsKey(id.ToString()))
                        {
                            tps.Add(id.ToString(), strip);
                        }
                    }
                    //ToolTip tip = new ToolTip();
                    //tip.SetToolTip(strip, desc);
                    if (flowLayoutPanel6.InvokeRequired) { this.Invoke((MethodInvoker)delegate { flowLayoutPanel6.Controls.Add(strip); }); }
                    else { flowLayoutPanel6.Controls.Add(strip); }
                }
            }

        }

        public class BrickButton : ToolStripButton
        {
            public MainForm MainForm { get; set; }
            public int ID { get; set; }
            public string blockInfo { get; set; }
            public int ShortCutID { get; set; }
            public int X { get; set; }
            public int mode { get; set; }
            public BrickButton(Image image, MainForm mainForm, EventHandler onClick, int id, int picid, bool grey, int mode, string blockdata)
                : base("", image, onClick)
            {
                MainForm = mainForm;
                this.BackColor = MainForm.userdata.themeBlockBG;
                this.ID = id;
                this.blockInfo = blockdata;
                this.AutoSize = false;
                this.Name = id.ToString();
                this.mode = mode;
                this.ImageScaling = ToolStripItemImageScaling.None;
                this.Size = new Size(20, 20);
                this.CheckOnClick = true;
                this.MouseDown += BrickButton_MouseDown;
                if (debug)
                {
                    var layer = "Foreground";
                    switch (mode)
                    {
                        case 0:
                            layer = "Foreground";
                            break;
                        case 1:
                            layer = "Misc";
                            break;
                        case 2:
                            layer = "Decorations";
                            break;
                        case 3:
                            layer = "Backgrounds";
                            break;
                    }
                    this.ToolTipText = "Layer: " + layer + "\nBlockID: " + id.ToString() + "\nPicID: " + picid;
                }
                ShortCutID = -1;
                if (userdata.brickHotkeys != null && userdata.brickHotkeys.Contains(','))
                {
                    List<int> values = userdata.brickHotkeys.Split(',').ToList().ConvertAll(int.Parse);
                    for (int i = 0; i < values.Count; i++)
                        if (values[i] == id)
                        {
                            ShortCutID = i;
                            MainForm.SetBrickShortCut(ShortCutID, this);
                        }
                }
            }

            #region Popup from blocks in blockbar
            private void BrickButton_MouseDown(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Right)
                {
                    //Find me
                    BrickButton cur = (BrickButton)sender;
                    var bid = cur.ID;
                    loadBid = cur.ID;
                    ContextMenuStrip cm = new ContextMenuStrip();
                    cm.Name = cur.ID.ToString();
                    cm.Items.Add("Copy BlockID", Properties.Resources.copy);
                    (cm.Items[0] as ToolStripMenuItem).Click += BrickButton_Click;
                    cm.Show(tps[bid.ToString()], cur.Bounds.Location);
                    if (bid == 423 || bid == 1027 || bid == 1028)
                    {
                        //Team door, gate, colors
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "None", 0, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Red", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Blue", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Green", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Cyan", 4, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Magenta", 5, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Yellow", 6, true));
                    }
                    else if (bid == 461 || bid == 417 || bid == 418 || bid == 419 || bid == 420 || bid == 453)
                    {
                        if (bid == 461)
                        {
                            // Double jump effect
                            cm.Items.Add(new ToolStripSeparator());
                            cm.Items.Add(toolStripMenuCreator(cur.ID, "No Jumping", 0, true));
                            cm.Items.Add(toolStripMenuCreator(cur.ID, "Normal Jumping", 1, true));
                            cm.Items.Add(toolStripMenuCreator(cur.ID, "Double Jumping", 2, true));
                        }
                        else
                        {
                            //Rest of the effects
                            cm.Items.Add(new ToolStripSeparator());
                            cm.Items.Add(toolStripMenuCreator(cur.ID, "Disabled", 0, true));
                            cm.Items.Add(toolStripMenuCreator(cur.ID, "Enabled", 1, true));
                        }
                    }
                    else if (bid == 385)
                    {
                        // Signs
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Normal", 0, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Silver", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Copper", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Gold", 3, true));
                    }
                    else if (bid == 464 || bid == 465)
                    {
                        //New year 2015 - String and Balloon
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Green", 0, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Orange", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Red", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Purple", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Blue", 4, true));
                    }
                    else if (bid == 456 || bid == 457 || bid == 458)
                    {
                        //halloween 2015
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Light", 0, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Dark", 1, true));
                    }
                    else if (bid == 447)
                    {
                        //Domestic
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Upper, turned on", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Upper, turned off", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Lower, turned on", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Lower, turned off", 0, true));
                    }
                    else if (bid == 448)
                    {
                        //Domestic
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Right to down", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Left to down", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Up to left", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Up to right", 0, true));
                    }
                    else if (bid == 449)
                    {
                        //Domestic
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Daylight", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Northern lights", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Sunset", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Night", 0, true));
                    }
                    else if (bid == 450)
                    {
                        //Domestic
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Green-yellow", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Green-red", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Green-pink", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Green-blue", 0, true));
                    }
                    else if (bid == 451)
                    {
                        //Domestic
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Standby", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "No signal", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Sea doc", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Wildlife", 0, true));
                    }
                    else if (bid == 452)
                    {
                        //Domestic
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Lights off", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Blue light", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Pink light", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Yellow light", 0, true));
                    }
                    else if (bid == 361)
                    {
                        //Spikes
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Up", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Down", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Right", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Left", 0, true));
                    }
                    else if (bid == 273 || bid == 328 || bid == 327)
                    {
                        //Colored Flags from medieval
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Blue", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Green", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Yellow", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Red", 0, true));
                    }
                    else if (bid == 329)
                    {
                        //Sword from medieval
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Down Right", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Down Left", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Up Left", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Up Right", 0, true));
                    }
                    else if (bid == 275)
                    {
                        //Axe from medieval
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Up Right", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Down Right", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Down Left", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Up Left", 0, true));
                    }
                    else if (bid == 440)
                    {
                        //wood from medieval
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "''''", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "T", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "|", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "\\'", 4, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "7", 5, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Ṽ", 0, true));
                    }
                    else if (bid == 280 || bid == 279 || bid == 277 || bid == 276)
                    {
                        //Dojo
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Blue", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Green", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Red", 0, true));
                    }
                    else if (bid == 439 || bid == 380 || bid == 378 || bid == 376)
                    {
                        //Scifi
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "‒", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "|", 0, true));
                    }
                    else if (bid == 438 || bid == 379 || bid == 375 || bid == 377)
                    {
                        //Scifi
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Right /", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Right \\", 0, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Left \\", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Left /", 3, true));

                    }
                    else if (bid == 1001 || bid == 1002 || bid == 1003 || bid == 1004 || bid == 1052 || bid == 1053 || bid == 1054 || bid == 1055 || bid == 1056 || bid == 1092)
                    {
                        //One-Way
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Up", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Down", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Right", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Left", 0, true));

                    }
                    else if (bid == 1041 || bid == 1042 || bid == 1043)
                    {
                        //Domestic halfblocks
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Down", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Up", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Left", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Right", 0, true));

                    }
                    else if (bid == 1075 || bid == 1076 || bid == 1077 || bid == 1078)
                    {
                        //Fairytale halfblocks
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Down", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Up", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Left", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Right", 0, true));

                    }
                    else if (bid == 471)
                    {
                        //Fairytale Flowers
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Pink", 0, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Blue", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Orange", 2, true));
                    }
                    else if (bid == 475)
                    {
                        //Spring 2016
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "White", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Blue", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Purple", 0, true));
                    }
                    else if (bid == 476)
                    {
                        //Spring 2016
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Red", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Yellow", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Purple", 0, true));
                    }
                    else if (bid == 477)
                    {
                        //Spring 2016
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Yellow", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "White", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Orange", 0, true));
                    }
                    else if (bid == 481 || bid == 482)
                    {
                        //Summer 2016
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Purple", 0, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Red", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Orange", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Green", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Cyan", 4, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Blue", 5, true));
                    }
                    else if (bid == 483)
                    {
                        //Summer 2016
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Green", 0, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Yellow", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Brown", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Red", 3, true));
                    }
                    else if (bid == 497)
                    {
                        //Mine
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Red", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Yellow", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Green", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Cyan", 4, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Blue", 5, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Purple", 0, true));
                    }
                    else if (bid == 492 || bid == 493 || bid == 494)
                    {
                        //Halloween 2016 
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "", 0, true));
                    }
                    else if (bid == 499)
                    {
                        //Halloween 2016 tree
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Right Down", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Left Down", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Left Up", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Right Up", 0, true));
                    }
                    else if (bid == 1500)
                    {
                        //Halloween 2016 pumpkin
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Dark", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Light", 0, true));
                    }
                    else if (bid == 1502)
                    {
                        //Halloween 2016 pumpkin
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Red", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Purple", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Green", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Yellow", 0, true));
                    }
                    else if (bid == 1506 || bid == 1507)
                    {
                        cm.Items.Add(new ToolStripSeparator());
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Red", 1, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Yellow", 2, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Green", 3, true));
                        cm.Items.Add(toolStripMenuCreator(cur.ID, "Purple", 0, true));
                    }
                    cm.ItemClicked += cm_ItemClicked;
                }
                else
                {

                    BrickButton cur = (BrickButton)sender;
                    MainForm.pressed = 0;
                    cur.MouseUp += delegate (object sender1, MouseEventArgs msa)
                    {
                        if (msa.Button == MouseButtons.Left)
                        {
                            if (MainForm.pressed == 0)
                            {
                                lastSelectedBlocksUpdate(cur);

                            }

                        }
                        MainForm.pressed += 1;

                    };

                }
            }
            #endregion
            private void lastSelectedBlocksUpdate(BrickButton bb)
            {
                BrickButton cur = bb;
                var bid = cur.ID;
                var derp = cur.Name;
                Console.WriteLine(cur.mode);
                if (MainForm.lastUsedBlockButton0.Name != cur.ID.ToString() && MainForm.lastUsedBlockButton1.Name != cur.ID.ToString() && MainForm.lastUsedBlockButton2.Name != cur.ID.ToString() && MainForm.lastUsedBlockButton3.Name != cur.ID.ToString() && MainForm.lastUsedBlockButton4.Name != cur.ID.ToString())
                {
                    Bitmap img4 = new Bitmap(16, 16); ;
                    if (cur.ID < 500 || cur.ID >= 1001)
                    {
                        if (cur.mode == 0 && foregroundBMI[bid] != 0)
                        {
                            img4 = foregroundBMD.Clone(new Rectangle(foregroundBMI[cur.ID] * 16, 0, 16, 16), foregroundBMD.PixelFormat);
                        }
                        else if (cur.mode == 2 && decosBMI[bid] != 0)
                        {
                            img4 = decosBMD.Clone(new Rectangle(decosBMI[cur.ID] * 16, 0, 16, 16), decosBMD.PixelFormat);
                        }
                        else if (cur.mode == 1 && miscBMI[bid] != 0)
                        {
                            img4 = miscBMD.Clone(new Rectangle(miscBMI[cur.ID] * 16, 0, 16, 16), miscBMD.PixelFormat);
                        }
                    }
                    else if (cur.ID >= 500 && cur.ID <= 999 && cur.mode == 3)
                    {
                        img4 = backgroundBMD.Clone(new Rectangle(backgroundBMI[cur.ID] * 16, 0, 16, 16), backgroundBMD.PixelFormat);
                    }
                    MainForm.lastUsedBlockButton4.Image = MainForm.lastUsedBlockButton3.Image;
                    MainForm.lastUsedBlockButton4.Name = MainForm.lastUsedBlockButton3.Name;

                    MainForm.lastUsedBlockButton3.Image = MainForm.lastUsedBlockButton2.Image;
                    MainForm.lastUsedBlockButton3.Name = MainForm.lastUsedBlockButton2.Name;

                    MainForm.lastUsedBlockButton2.Image = MainForm.lastUsedBlockButton1.Image;
                    MainForm.lastUsedBlockButton2.Name = MainForm.lastUsedBlockButton1.Name;

                    MainForm.lastUsedBlockButton1.Image = MainForm.lastUsedBlockButton0.Image;
                    MainForm.lastUsedBlockButton1.Name = MainForm.lastUsedBlockButton0.Name;

                    MainForm.lastUsedBlockButton0.Image = img4;
                    MainForm.lastUsedBlockButton0.Name = cur.ID.ToString();
                }
                else
                {
                    //if (MainForm.lastblocks == 5) MainForm.lastblocks = 0;
                }
            }
            private void BrickButton_Click(object sender, EventArgs e)
            {
                Clipboard.SetText(loadBid.ToString());
            }


            void cm_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
            {

                if (e.ClickedItem.Name.Contains(':'))
                {
                    string[] values = e.ClickedItem.Name.Split(':');
                    if (values.Length == 2)
                    {
                        int bid = Convert.ToInt32(values[0]);
                        int rotation = Convert.ToInt16(values[1]);
                        selectedBrick.Checked = false;
                        editArea.Tool.PenID = bid;
                        //selectedBrick = bid;
                        if (ToolPen.rotation.ContainsKey(bid)) { ToolPen.rotation[bid] = rotation; }
                        else { ToolPen.rotation.Add(bid, rotation); }
                    }
                }
            }
            private ToolStripMenuItem toolStripMenuCreator(int bid, string text, int rotation, bool userotation)
            {
                ToolStripMenuItem tsp = new ToolStripMenuItem();
                tsp.Name = bid.ToString() + ":" + rotation.ToString();
                tsp.Text = text;
                tsp.Image = id2block(bid, rotation, userotation);
                return tsp;
            }
            private Bitmap id2block(int id, int rotation, bool userotation)
            {
                Bitmap img1;
                if (id < 500 || id >= 1001)
                {
                    if (decosBMI[id] != 0)
                    {
                        if (!userotation) img1 = decosBMD.Clone(new Rectangle(decosBMI[id] * 16, 0, 16, 16), decosBMD.PixelFormat);
                        else { img1 = bdata.getRotation(id, rotation); }
                        return img1;
                    }
                    else if (miscBMI[id] != 0 || id == 119)
                    {
                        if (!userotation) img1 = miscBMD.Clone(new Rectangle(miscBMI[id] * 16, 0, 16, 16), miscBMD.PixelFormat);
                        else { img1 = bdata.getRotation(id, rotation); }
                        return img1;
                    }
                    else
                    {
                        if (!userotation) img1 = foregroundBMD.Clone(new Rectangle(foregroundBMI[id] * 16, 0, 16, 16), foregroundBMD.PixelFormat);
                        else { img1 = bdata.getRotation(id, rotation); }
                        return img1;
                    }

                }
                if (id >= 500 && id <= 999)
                {
                    Bitmap img10 = backgroundBMD.Clone(new Rectangle(backgroundBMI[id] * 16, 0, 16, 16), backgroundBMD.PixelFormat);
                    return img10;
                }
                else
                {
                    return null;
                }
            }


            static Rectangle Rect = new Rectangle(11, 9, 6, 8);
            static Font StringFont = new Font("Courier", 6);

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                if (ShortCutID >= 0)
                {
                    Graphics g = e.Graphics;
                    g.FillRectangle(Brushes.White, Rect);
                    g.DrawString(ShortCutID.ToString(), StringFont, Brushes.Black, new PointF(11, 9));
                }
            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                this.Checked = true;
                for (int key = (int)Keys.D0; key <= (int)Keys.D9; ++key)
                {
                    if (EditArea.IsKeyDown(key))
                    {
                        Console.WriteLine(key);
                        ShortCutID = key - (int)Keys.D0;
                        MainForm.SetBrickShortCut(ShortCutID, this);
                        return;
                    }
                }
            }
        }

        BrickButton[] shortCutButtons = new BrickButton[10];

        public void SetBrickShortCut(int id, BrickButton button)
        {
            if (shortCutButtons[id] != null && shortCutButtons[id] != button)
            {
                shortCutButtons[id].ShortCutID = -1;
                shortCutButtons[id].Invalidate();
            }
            shortCutButtons[id] = button;
            shortCutButtons[id].Invalidate();
        }

        public void SetActiveBrick(int id)
        {
            if (shortCutButtons[id] != null)
            {
                BrickButton cur = shortCutButtons[id];
                selectedBrick.Checked = false;
                editArea.Tool.PenID = cur.ID;
                selectedBrick = cur;
                cur.Checked = true;
            }
        }

        protected Bitmap Fade(Bitmap org)
        {
            Bitmap img = (Bitmap)org.Clone();
            Graphics graphics = Graphics.FromImage(img);
            Pen p = new Pen(Color.FromArgb(140, 0, 0, 0), img.Width * 2);
            graphics.DrawLine(p, -1, -1, img.Width, img.Height);
            graphics.Save();
            graphics.Dispose();
            return img;
        }

        void SetBrick(object sender, EventArgs e)
        {
            BrickButton cur = (BrickButton)sender;
            if (cur.ID == 83 || cur.ID == 77)
            {
                MessageBox.Show("EEditor doesn't support the newest Piano or Drums yet.", "Sorry!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                selectedBrick.Checked = false;
                editArea.Tool.PenID = cur.ID;
                selectedBrick = cur;
                if (userdata.usePenTool) SetPenTool();
            }
            else
            {
                selectedBrick.Checked = false;
                editArea.Tool.PenID = cur.ID;
                selectedBrick = cur;
                if (userdata.usePenTool) SetPenTool();
            }
        }

        public void setBrick(int id, bool color2id)
        {
            int found = 0;
            if (color2id)
            {
                foreach (Control ctrl in flowLayoutPanel2.Controls)
                {
                    foreach (BrickButton item in ((ToolStrip)ctrl).Items)
                    {
                        if (item.ID == id)
                        {
                            showBlocksButton.PerformClick();
                            editArea.Tool.PenID = item.ID;
                            selectedBrick.Checked = false;
                            item.Checked = true;
                            Rectangle rec = item.Bounds;
                            ToolTip tip = new ToolTip();
                            tip.Show("V", ctrl, rec.X, rec.Y - 20, 2000);
                            selectedBrick = item;
                            found = 1;
                            break;
                        }

                    }
                }
                if (found != 1)
                {
                    foreach (Control ctrl in flowLayoutPanel5.Controls)
                    {
                        foreach (BrickButton item in ((ToolStrip)ctrl).Items)
                        {
                            if (item.ID == id)
                            {
                                showBackgroundsButton.PerformClick();
                                BrickButton cur = item;
                                selectedBrick.Checked = false;
                                Rectangle rec = item.Bounds;
                                ToolTip tip = new ToolTip();
                                tip.Show("V", ctrl, rec.X, rec.Y - 20, 2000);
                                editArea.Tool.PenID = cur.ID;
                                selectedBrick = cur;
                                cur.Checked = true;
                                found = 2;
                                break;
                            }

                        }
                    }
                }
            }
            else
            {
                foreach (Control ctrl in flowLayoutPanel2.Controls)
                {
                    foreach (BrickButton item in ((ToolStrip)ctrl).Items)
                    {
                        if (item.ID == id)
                        {
                            showBlocksButton.PerformClick();
                            editArea.Tool.PenID = item.ID;
                            selectedBrick.Checked = false;
                            item.Checked = true;
                            selectedBrick = item;
                            found = 1;
                            break;
                        }

                    }
                }
                if (found != 1)
                {
                    foreach (Control ctrl in flowLayoutPanel5.Controls)
                    {
                        foreach (BrickButton item in ((ToolStrip)ctrl).Items)
                        {
                            if (item.ID == id)
                            {
                                showBackgroundsButton.PerformClick();
                                editArea.Tool.PenID = item.ID;
                                selectedBrick.Checked = false;
                                item.Checked = true;
                                selectedBrick = item;
                                found = 2;
                                break;
                            }

                        }
                    }
                }
                if (found != 2)
                {
                    foreach (Control ctrl in flowLayoutPanel4.Controls)
                    {
                        foreach (BrickButton item in ((ToolStrip)ctrl).Items)
                        {
                            if (item.ID == id)
                            {
                                showDecorationsButton.PerformClick();
                                editArea.Tool.PenID = item.ID;
                                selectedBrick.Checked = false;
                                item.Checked = true;
                                selectedBrick = item;
                                found = 3;
                                break;
                            }

                        }
                    }
                }
                if (found != 3)
                {
                    foreach (Control ctrl in flowLayoutPanel3.Controls)
                    {
                        foreach (BrickButton item in ((ToolStrip)ctrl).Items)
                        {
                            if (item.ID == id)
                            {
                                showActionsButton.PerformClick();
                                editArea.Tool.PenID = item.ID;
                                selectedBrick.Checked = false;
                                item.Checked = true;
                                selectedBrick = item;
                                found = 4;
                                break;
                            }

                        }
                    }
                }

            }
        }
        #endregion

        //topFlowLayoutPanel
        #region fileToolStrip
        //New
        private void newWorldButton_Click(object sender, EventArgs e)
        {
            NewDialogForm form = new NewDialogForm(this);
            if (form.ShowDialog() == DialogResult.OK)
            {
                levelTextbox.Text = userdata.level;
                codeTextbox.Text = userdata.levelPass;
                InsertImageForm.Background.Clear();
                InsertImageForm.Blocks.Clear();
                MainForm.editArea.Back = null;
                MainForm.editArea.Back1 = null;
                rebuildGUI(true);
                if (form.MapFrame != null)
                {
                    if (form.NeedsInit)
                    {
                        editArea.Init(form.SizeWidth, form.SizeHeight);
                        //editArea.Init(form.SizeWidth, form.SizeHeight);
                    }
                    else
                    {
                        editArea.Init(form.MapFrame, false);
                    }
                }
                else
                {
                    if (form.NeedsInit)
                    {
                        editArea.Init(form.SizeWidth, form.SizeHeight);
                    }
                }
            }
            else
            {
                if (form.notsaved) MessageBox.Show("The world is not saved.", "World not saved", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                form.notsaved = false;
            }
        }
        //Load
        private void new33ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetDummy();
            try
            {
                MainForm.editArea.Back = null;
                MainForm.editArea.Back1 = null;
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Select a level to load from";
                ofd.DefaultExt = "eelevel";
                ofd.Filter = "EverybodyEdits level (*.eelevel)|*.eelevel";
                ofd.FilterIndex = 1;
                ofd.AddExtension = true;
                ofd.RestoreDirectory = true;
                ofd.CheckFileExists = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string path = ofd.FileName;
                    FileStream fs = new FileStream(path, FileMode.Open);
                    BinaryReader reader = new BinaryReader(fs);
                    //Frame frame = Frame.Load(reader, 4,path);
                    Frame frame = Frame.Load(reader, 4);
                    reader.Close();
                    fs.Close();
                    if (frame != null)
                    {
                        this.Text = Path.GetFileName(ofd.FileName) + " by " + "test" + " (" + frame.Width + "x" + frame.Height + ") - EEditor " + this.ProductVersion;
                        editArea.Init(frame, false);
                    }
                    else MessageBox.Show("The loaded file is broken.", "File is broken", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void loadNewMenuItem_Click(object sender, EventArgs e)
        {
            SetDummy();
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Select a level to load from";
                ofd.DefaultExt = ".eelevel";
                ofd.Filter = "EverybodyEdits level (*.eelevel)|*.eelevel";
                ofd.FilterIndex = 1;
                ofd.AddExtension = true;
                ofd.RestoreDirectory = true;
                ofd.CheckFileExists = true;
                if (ofd.ShowDialog() != DialogResult.OK) return;
                string path = ofd.FileName;
                FileStream fs = new FileStream(path, FileMode.Open);
                BinaryReader reader = new BinaryReader(fs);
                Frame frame = Frame.Load(reader, 2);
                reader.Close();
                fs.Close();
                if (frame != null)
                {
                    this.Text = Path.GetFileName(ofd.FileName) + " by " + "test" + " (" + frame.Width + "x" + frame.Height + ") - EEditor " + this.ProductVersion;
                    editArea.Init(frame, false);
                }
                else MessageBox.Show("The loaded file is broken.", "File is broken", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void loadOldMenuItem_Click(object sender, EventArgs e)
        {
            SetDummy();
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Select a level to load from";
                ofd.DefaultExt = "eelevel";
                ofd.Filter = "EverybodyEdits level (*.eelevel)|*.eelevel";
                ofd.AddExtension = true;
                ofd.RestoreDirectory = true;
                if (ofd.ShowDialog() != DialogResult.OK) return;
                string path = ofd.FileName;
                FileStream fs = new FileStream(path, FileMode.Open);
                BinaryReader reader = new BinaryReader(fs);
                Frame frame = Frame.Load(reader, 1);
                reader.Close();
                fs.Close();
                if (frame != null)
                {
                    this.Text = Path.GetFileName(ofd.FileName) + " by " + "test" + " (" + frame.Width + "x" + frame.Height + ") - EEditor " + this.ProductVersion;
                    editArea.Init(frame, false);
                }
                else MessageBox.Show("The loaded file is broken.", "File is broken", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void loadOldestMenuItem_Click(object sender, EventArgs e)
        {
            SetDummy();
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Select a level to load from";
                ofd.DefaultExt = "eelevel";
                ofd.Filter = "EverybodyEdits level (*.eelevel)|*.eelevel";
                ofd.AddExtension = true;
                ofd.RestoreDirectory = true;
                if (ofd.ShowDialog() != DialogResult.OK) return;
                string path = ofd.FileName;
                FileStream fs = new FileStream(path, FileMode.Open);
                BinaryReader reader = new BinaryReader(fs);
                Frame frame = Frame.Load(reader, 0);
                reader.Close();
                fs.Close();
                if (frame != null)
                {
                    this.Text = Path.GetFileName(ofd.FileName) + " by " + "test" + " (" + frame.Width + "x" + frame.Height + ") - EEditor " + this.ProductVersion;
                    editArea.Init(frame, false);
                }
                else MessageBox.Show("The loaded file is broken.", "File is broken", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void savToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetDummy();
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Select a level to load from";
                ofd.DefaultExt = ".sav";
                ofd.Filter = "EEAnimator (*.sav)|*.sav";
                ofd.FilterIndex = 1;
                ofd.AddExtension = true;
                ofd.RestoreDirectory = true;
                ofd.CheckFileExists = true;
                if (ofd.ShowDialog() != DialogResult.OK) return;
                string path = ofd.FileName;
                FileStream fs = new FileStream(path, FileMode.Open);
                BinaryReader reader = new BinaryReader(fs);
                Frame frame = Frame.Load(reader, 3);
                reader.Close();
                fs.Close();
                if (frame != null)
                {
                    this.Text = Path.GetFileName(ofd.FileName) + " by " + "test" + " (" + frame.Width + "x" + frame.Height + ") - EEditor " + this.ProductVersion;
                    editArea.Init(frame, false);
                }
                else MessageBox.Show("The loaded file was made by an unknown EEAnimator version.", "Unknown version", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void eEBuilderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetDummy();
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Select a level to load from";
                ofd.DefaultExt = ".aub1";
                ofd.Filter = "EEBuilder (*.aub1)|*.aub1";
                ofd.FilterIndex = 1;
                ofd.AddExtension = true;
                ofd.RestoreDirectory = true;
                ofd.CheckFileExists = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string path = ofd.FileName;
                    Frame.LoadEEBuilder(path, 5);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Save
        private void saveWorldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetDummy();
            try
            {
                SaveFileDialog ofd = new SaveFileDialog();
                ofd.Title = "Select a file to save to";
                ofd.DefaultExt = "eelevel";
                ofd.Filter = "EverybodyEdits level (*.eelevel)|*.eelevel";
                ofd.AddExtension = true;
                ofd.RestoreDirectory = true;
                if (ofd.ShowDialog() != DialogResult.OK) return;
                string path = ofd.FileName;

                FileStream fs = new FileStream(path, FileMode.Create);
                BinaryWriter writer = new BinaryWriter(fs);
                editArea.CurFrame.Save(writer);
                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void worldAsImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editArea.Back.Save(Directory.GetCurrentDirectory() + "\\Saved_World_" + userdata.level + ".png");
        }
        private void minimapAsImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Minimap.Bitmap.Save(Directory.GetCurrentDirectory() + "\\Saved_Minimap_" + userdata.level + ".png");
        }
        #endregion

        #region toolToolStrip
        //Draw
        public void SetDummy()
        {
            fillButton.Checked = false;
            markButton.Checked = false;
            rectangleButton.Checked = false;
            filledRectangleButton.Checked = false;
            circleButton.Checked = false;
            lineButton.Checked = false;
            sprayButton.Checked = false;
            penButton.Checked = false;
            editArea.Tool.CleanUp(false);
            editArea.Tool = new ToolDummy(editArea);
            editArea.Tool.PenID = selectedBrick.ID;
            selectionTool = false;
        }
        public void SetPenTool()
        {
            fillButton.Checked = false;
            markButton.Checked = false;
            rectangleButton.Checked = false;
            filledRectangleButton.Checked = false;
            circleButton.Checked = false;
            lineButton.Checked = false;
            sprayButton.Checked = false;
            penButton.Checked = true;
            //filledCircleButton.Checked = false;
            editArea.Tool.CleanUp(false);
            editArea.Tool = new ToolPen(editArea);
            editArea.Tool.PenID = selectedBrick.ID;
            selectionTool = false;
        }
        private void penButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Brush frm = new Brush();
                frm.BrushSize.Value = Tool.PenSize;
                frm.ShowDialog();
            }
        }
        private void penButton_Click(object sender, EventArgs e) { SetPenTool(); }
        //Fill
        public void SetFillTool()
        {
            fillButton.Checked = true;
            markButton.Checked = false;
            rectangleButton.Checked = false;
            filledRectangleButton.Checked = false;
            circleButton.Checked = false;
            lineButton.Checked = false;
            sprayButton.Checked = false;
            penButton.Checked = false;
            //filledCircleButton.Checked = false;
            editArea.Tool.CleanUp(false);
            editArea.Tool = new ToolFill(editArea);
            editArea.Tool.PenID = selectedBrick.ID;
            selectionTool = false;
        }
        private void fillButton_Click(object sender, EventArgs e) { SetFillTool(); }
        //Spray
        public void SetSprayTool()
        {
            fillButton.Checked = false;
            markButton.Checked = false;
            rectangleButton.Checked = false;
            filledRectangleButton.Checked = false;
            circleButton.Checked = false;
            lineButton.Checked = false;
            sprayButton.Checked = true;
            penButton.Checked = false;
            //filledCircleButton.Checked = false;
            editArea.Tool.CleanUp(false);
            editArea.Tool = new ToolSpray(editArea);
            editArea.Tool.PenID = selectedBrick.ID;
            selectionTool = false;
        }
        private void sprayButton_Click(object sender, EventArgs e) { SetSprayTool(); }
        private void sprayButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Form frm = new SprayCan();
                if (frm.ShowDialog() == DialogResult.OK)
                {

                }
            }
        }
        //Selection
        public void SetMarkTool()
        {
            fillButton.Checked = false;
            markButton.Checked = true;
            rectangleButton.Checked = false;
            filledRectangleButton.Checked = false;
            circleButton.Checked = false;
            lineButton.Checked = false;
            sprayButton.Checked = false;
            penButton.Checked = false;
            //filledCircleButton.Checked = false;
            editArea.Tool.CleanUp(false);
            editArea.Tool = new ToolMark(editArea);
            editArea.Tool.PenID = selectedBrick.ID;
            SetTransFormToolStrip(true);
            selectionTool = true;
        }
        private void markButton_Click(object sender, EventArgs e) { SetMarkTool(); }
        public void SetTransFormToolStrip(bool value)
        {
            morphToolStrip.Visible = value;
            markButton.Checked = value;
            historyToolStrip.Visible = !value; //Hide history toolbar while it doesn't work with selection tool
        }

        // ---
        //Shapes
        public void SetRectTool()
        {
            fillButton.Checked = false;
            markButton.Checked = false;
            rectangleButton.Checked = true;
            filledRectangleButton.Checked = false;
            circleButton.Checked = false;
            lineButton.Checked = false;
            sprayButton.Checked = false;
            penButton.Checked = false;
            //filledCircleButton.Checked = false;
            editArea.Tool.CleanUp(false);
            editArea.Tool = new ToolRect(editArea);
            editArea.Tool.PenID = selectedBrick.ID;
            selectionTool = false;
        }
        private void rectangleButton_Click(object sender, EventArgs e) { SetRectTool(); rectangleButton.Checked = false; }

        public void SetFilledRectTool()
        {
            fillButton.Checked = false;
            markButton.Checked = false;
            rectangleButton.Checked = false;
            filledRectangleButton.Checked = true;
            circleButton.Checked = false;
            lineButton.Checked = false;
            sprayButton.Checked = false;
            penButton.Checked = false;
            //filledCircleButton.Checked = false;
            editArea.Tool.CleanUp(false);
            editArea.Tool = new ToolRectFill(editArea);
            editArea.Tool.PenID = selectedBrick.ID;
            selectionTool = false;
        }
        private void filledRectangleButton_Click(object sender, EventArgs e) { SetFilledRectTool(); filledRectangleButton.Checked = false; }

        public void SetCircleTool()
        {
            fillButton.Checked = false;
            markButton.Checked = false;
            rectangleButton.Checked = false;
            sprayButton.Checked = false;
            penButton.Checked = false;
            filledRectangleButton.Checked = false;
            circleButton.Checked = true;
            lineButton.Checked = false;
            //filledCircleButton.Checked = false;
            editArea.Tool.CleanUp(false);
            editArea.Tool = new ToolCircle(editArea);
            editArea.Tool.PenID = selectedBrick.ID;
            selectionTool = false;
        }
        private void circleButton_Click(object sender, EventArgs e) { SetCircleTool(); circleButton.Checked = false; }

        public void SetFilledCircleTool()
        {
            fillButton.Checked = false;
            markButton.Checked = false;
            rectangleButton.Checked = false;
            sprayButton.Checked = false;
            penButton.Checked = true;
            filledRectangleButton.Checked = false;
            circleButton.Checked = false;
            lineButton.Checked = false;
            filledCircleButton.Checked = false;
            editArea.Tool.CleanUp(false);
            editArea.Tool = new ToolCircleFill(editArea);
            editArea.Tool.PenID = selectedBrick.ID;
            selectionTool = false;
        }
        private void filledCircleButton_Click(object sender, EventArgs e) { SetFilledCircleTool(); filledCircleButton.Checked = false; }

        public void SetLineTool()
        {
            fillButton.Checked = false;
            sprayButton.Checked = false;
            penButton.Checked = false;
            markButton.Checked = false;
            rectangleButton.Checked = false;
            filledRectangleButton.Checked = false;
            circleButton.Checked = false;
            lineButton.Checked = true;
            // filledCircleButton.Checked = false;
            editArea.Tool.CleanUp(false);
            editArea.Tool = new ToolLine(editArea);
            editArea.Tool.PenID = selectedBrick.ID;
            selectionTool = false;
        }
        private void lineButton_Click(object sender, EventArgs e) { SetLineTool(); lineButton.Checked = false; }
        //Insert
        private void imageButton_Click(object sender, EventArgs e)
        {
            new InsertImageForm().ShowDialog();
        }
        private void textButton_Click(object sender, EventArgs e)
        {
            SetMarkTool();
            Form frm = new fontadding();
            frm.ShowDialog();

        }
        // ---
        // Find&replace
        private void replaceButton_Click(object sender, EventArgs e)
        {
            string incfg = null;

            using (Replacer rp = new Replacer(this))
            {
                try
                {
                    SetDummy();
                    rp.ShowDialog();

                }
                catch { }
                if (incfg != null) ToolPen.undolist.Push(incfg);
            }
        }
        #endregion

        #region morphToolStrip
        //Mirror
        private void mirrorButton_Click(object sender, EventArgs e)
        {
            editArea.Mirror();
        }
        //Flip
        private void flipButton_Click(object sender, EventArgs e)
        {
            editArea.Flip();
        }
        //Rotate left
        private void rotateLeftButton_Click(object sender, EventArgs e)
        {
            editArea.Rotate90();
            editArea.Rotate90();
            editArea.Rotate90();
        }
        //Rotate right
        private void rotateRightButton_Click(object sender, EventArgs e)
        {
            editArea.Rotate90();
        }
        #endregion

        #region historyToolStrip
        //Undo
        private void undoButton_Click(object sender, EventArgs e)
        {
            if (!markButton.Checked)
            {
                if (ToolPen.undolist.Count > 0)
                {
                    string vara = ToolPen.undolist.Pop();
                    ToolPen.redolist.Push(vara);
                    string[] var = vara.Split(':');

                    if (var.Length == 4)
                    {
                        int bidAfter = Convert.ToInt32(var[0]);
                        int bidBefore = Convert.ToInt32(var[1]);
                        int xx = Convert.ToInt32(var[2]);
                        int yy = Convert.ToInt32(var[3]);
                        if (Convert.ToInt32(var[0]) >= 500 && Convert.ToInt32(var[0]) <= 999)
                        {

                            editArea.Frames[0].Background[yy, xx] = bidBefore;
                            Point p = new Point(xx * 16 - Math.Abs(editArea.AutoScrollPosition.X), yy * 16 - Math.Abs(editArea.AutoScrollPosition.Y));
                            Graphics g = Graphics.FromImage(editArea.Back);
                            editArea.Draw(xx, yy, g, userdata.thisColor);
                            g.Save();
                            editArea.Invalidate(new Rectangle(p, new Size(16, 16)));
                        }
                        else if (Convert.ToInt32(var[0]) < 500 || Convert.ToInt32(var[0]) >= 1001)
                        {
                            editArea.Frames[0].Foreground[yy, xx] = bidBefore;
                            Point p = new Point(xx * 16 - Math.Abs(editArea.AutoScrollPosition.X), yy * 16 - Math.Abs(editArea.AutoScrollPosition.Y));
                            Graphics g = Graphics.FromImage(editArea.Back);
                            editArea.Draw(xx, yy, g, userdata.thisColor);
                            g.Save();
                            editArea.Invalidate(new Rectangle(p, new Size(16, 16)));
                        }
                    }
                    else
                    {
                        int bidAfter = 0;
                        int bidBefore = 0;
                        int xx = 0;
                        int yy = 0;
                        int incr = 0;
                        for (int i = 0; i < var.Length; i++)
                        {
                            if (var[i] != "")
                            {
                                if (incr == 0)
                                {
                                    bidAfter = Convert.ToInt32(var[i]);
                                    incr += 1;
                                }
                                else if (incr == 1)
                                {
                                    bidBefore = Convert.ToInt32(var[i]);
                                    incr += 1;
                                }
                                else if (incr == 2)
                                {
                                    xx = Convert.ToInt32(var[i]);
                                    incr += 1;
                                }
                                else if (incr == 3)
                                {
                                    yy = Convert.ToInt32(var[i]);
                                    if (bidAfter >= 500 && bidAfter <= 999 && bidBefore == 0)
                                    {
                                        editArea.Frames[0].Background[yy, xx] = bidBefore;
                                    }
                                    else if (bidBefore >= 500 && bidBefore <= 999)
                                    {
                                        editArea.Frames[0].Background[yy, xx] = bidBefore;
                                    }
                                    else
                                    {
                                        editArea.Frames[0].Foreground[yy, xx] = bidBefore;
                                    }
                                    Point p = new Point(xx * 16 - Math.Abs(editArea.AutoScrollPosition.X), yy * 16 - Math.Abs(editArea.AutoScrollPosition.Y));
                                    Graphics g = Graphics.FromImage(editArea.Back);
                                    editArea.Draw(xx, yy, g, userdata.thisColor);
                                    g.Save();
                                    editArea.Invalidate(new Rectangle(p, new Size(16, 16)));
                                    bidAfter = 0;
                                    bidBefore = 0;
                                    xx = 0;
                                    yy = 0;
                                    incr = 0;
                                }
                            }
                        }
                    }

                }
            }
            else
            {
                MessageBox.Show("Can't undo/redo when selection tool is used.", "Can't use history", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        //Redo
        private void redoButton_Click(object sender, EventArgs e)
        {
            if (!markButton.Checked)
            {
                if (ToolPen.redolist.Count > 0)
                {
                    string vara = ToolPen.redolist.Pop();
                    string[] var = vara.Split(':');
                    ToolPen.undolist.Push(vara);
                    if (var.Length == 4)
                    {
                        int bidAfter = Convert.ToInt32(var[0]);
                        int bidBefore = Convert.ToInt32(var[1]);
                        int xx = Convert.ToInt32(var[2]);
                        int yy = Convert.ToInt32(var[3]);
                        if (Convert.ToInt32(var[0]) >= 500 && Convert.ToInt32(var[0]) <= 999)
                        {

                            editArea.Frames[0].Background[yy, xx] = bidAfter;
                            Point p = new Point(xx * 16 - Math.Abs(editArea.AutoScrollPosition.X), yy * 16 - Math.Abs(editArea.AutoScrollPosition.Y));
                            Graphics g = Graphics.FromImage(editArea.Back);
                            editArea.Draw(xx, yy, g, userdata.thisColor);
                            g.Save();
                            editArea.Invalidate(new Rectangle(p, new Size(16, 16)));
                        }
                        else if (Convert.ToInt32(var[0]) < 500 || Convert.ToInt32(var[0]) >= 1001)
                        {
                            editArea.Frames[0].Foreground[yy, xx] = bidAfter;
                            Point p = new Point(xx * 16 - Math.Abs(editArea.AutoScrollPosition.X), yy * 16 - Math.Abs(editArea.AutoScrollPosition.Y));
                            Graphics g = Graphics.FromImage(editArea.Back);
                            editArea.Draw(xx, yy, g, userdata.thisColor);
                            g.Save();
                            editArea.Invalidate(new Rectangle(p, new Size(16, 16)));
                        }
                    }
                    else
                    {
                        int bidAfter = 0;
                        int bidBefore = 0;
                        int xx = 0;
                        int yy = 0;
                        int incr = 0;
                        for (int i = 0; i < var.Length; i++)
                        {
                            if (var[i] != "")
                            {
                                if (incr == 0)
                                {
                                    bidAfter = Convert.ToInt32(var[i]);
                                    incr += 1;
                                }
                                else if (incr == 1)
                                {
                                    bidBefore = Convert.ToInt32(var[i]);
                                    incr += 1;
                                }
                                else if (incr == 2)
                                {
                                    xx = Convert.ToInt32(var[i]);
                                    incr += 1;
                                }
                                else if (incr == 3)
                                {
                                    yy = Convert.ToInt32(var[i]);
                                    if (bidAfter >= 500 && bidAfter <= 999)
                                    {
                                        editArea.Frames[0].Background[yy, xx] = bidAfter;
                                    }
                                    else
                                    {
                                        editArea.Frames[0].Foreground[yy, xx] = bidAfter;
                                    }
                                    Point p = new Point(xx * 16 - Math.Abs(editArea.AutoScrollPosition.X), yy * 16 - Math.Abs(editArea.AutoScrollPosition.Y));
                                    Graphics g = Graphics.FromImage(editArea.Back);
                                    editArea.Draw(xx, yy, g, userdata.thisColor);
                                    g.Save();
                                    editArea.Invalidate(new Rectangle(p, new Size(16, 16)));
                                    bidAfter = 0;
                                    bidBefore = 0;
                                    xx = 0;
                                    yy = 0;
                                    incr = 0;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Can't undo/redo when selection tool is used.", "Can't use history", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //History
        private void historyButton_Click(object sender, EventArgs e)
        {
            Form frm = new History();
            if (frm.ShowDialog() == DialogResult.OK)
            {

            }
        }
        #endregion

        #region uploadToolStrip
        //Level ID textbox
        private void levelTextbox_TextChanged(object sender, EventArgs e)
        {
            if (!starting)
            {
                //Make the refresh button show as download button if you change world id
                Bitmap bmp1 = new Bitmap(Properties.Resources.download.Width, Properties.Resources.download.Height);
                Bitmap bmp = new Bitmap(Properties.Resources.download);
                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        if (bmp.GetPixel(x, y).A > 80)
                        {
                            bmp1.SetPixel(x, y, userdata.themeIcons);
                        }
                        else
                        {
                            bmp1.SetPixel(x, y, userdata.themeIconsBg);
                        }
                    }
                }
                refreshButton.Image = bmp1;
            }
        }
        private void levelTextbox_Leave(object sender, EventArgs e)
        {
            userdata.level = levelTextbox.Text;
        }
        //Level code textbox
        private void codeTextbox_Leave(object sender, EventArgs e)
        {
            userdata.levelPass = codeTextbox.Text;
        }
        //Load/reload
        private void refreshButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (levelTextbox.Text.StartsWith("PW") || levelTextbox.Text.StartsWith("BW") || levelTextbox.Text.StartsWith("OW") || levelTextbox.Text == "tutorialWorld")
                {
                    SetDummy();
                    userdata.thisColor = Color.Transparent;
                    userdata.useColor = false;
                    if (levelTextbox.Text.StartsWith("OW"))
                    {
                        userdata.openWorld = true;
                        InsertImageForm.Background.Clear();
                        InsertImageForm.Blocks.Clear();
                        rebuildGUI(false);
                    }
                    else
                    {
                        if (userdata.openWorld)
                        {
                            userdata.openWorld = false;
                            rebuildGUI(false);
                        }

                    }
                    userdata.level = levelTextbox.Text.StartsWith("OW") ? levelTextbox.Text.Replace("-", " ") : levelTextbox.Text;
                    loaddata(0);
                    
                }
                else
                {
                    MessageBox.Show("You need to insert a world ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (levelTextbox.Text.StartsWith("PW") || levelTextbox.Text.StartsWith("BW") || levelTextbox.Text.StartsWith("OW"))
                {
                    SetDummy();
                    userdata.thisColor = Color.Transparent;
                    userdata.useColor = false;
                    if (levelTextbox.Text.StartsWith("OW"))
                    {
                        userdata.openWorld = true;
                        InsertImageForm.Background.Clear();
                        InsertImageForm.Blocks.Clear();
                        rebuildGUI(false);
                    }
                    else
                    {
                        if (userdata.openWorld)
                        {
                            userdata.openWorld = false;
                            rebuildGUI(false);
                        }


                    }
                    userdata.level = levelTextbox.Text.StartsWith("OW") ? levelTextbox.Text.Replace("-", " ") : levelTextbox.Text;
                    levelTextbox.Text = userdata.level;
                    loaddata(1);

                }
                else
                {
                    MessageBox.Show("You need to insert a world ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


        }
        private void loaddata(int frm)
        {
            
            NewDialogForm form = new NewDialogForm(this);
            if (frm == 0) form.LoadFromLevel(userdata.level, 0);
            if (frm == 1) form.LoadFromLevel(userdata.level, 1);
            if (form.DialogResult == DialogResult.OK)
            {
                editArea.Back = null;
                editArea.Back1 = null;
                if (form.MapFrame != null)
                {
                    editArea.Init(form.MapFrame, false);
                    Bitmap bmp1 = new Bitmap(Properties.Resources.refresh.Width, Properties.Resources.refresh.Height);
                    Bitmap bmp = new Bitmap(Properties.Resources.refresh);
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        for (int y = 0; y < bmp.Height; y++)
                        {
                            if (bmp.GetPixel(x, y).A > 80)
                            {
                                bmp1.SetPixel(x, y, userdata.themeIcons);
                            }
                            else
                            {
                                bmp1.SetPixel(x, y, userdata.themeIconsBg);
                            }
                        }
                    }
                    refreshButton.Image = bmp1;
                    //updateImageColor();
                }
            }
        }
        //Upload
        private void uploadButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(codeTextbox.Text)) userdata.levelPass = codeTextbox.Text;
            if (!string.IsNullOrWhiteSpace(levelTextbox.Text)) userdata.level = levelTextbox.Text;
            editArea.Tool.CleanUp(false);
            AnimateForm form = new AnimateForm(editArea.Frames);
            form.ShowDialog();
        }
        #endregion

        #region settingsToolStrip
        //Accounts
        private void accountsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!starting1)
            {
                if (accountsComboBox.SelectedItem != null)
                {
                    if (accountsComboBox.SelectedItem.ToString().Contains("-"))
                    {
                        accountsComboBox.SelectedIndex = 0;
                        editArea.Focus();
                    }
                    else if (accountsComboBox.SelectedIndex == (accountsComboBox.Items.Count - 1))
                    {
                        Accounts ac = new Accounts();
                        ac.Show();
                    }
                    else
                    {
                        selectedAcc = accountsComboBox.Text;
                        if (accs.ContainsKey(selectedAcc))
                        {
                            userdata.username = accountsComboBox.Text;
                            ihavethese.Clear();
                            if (accs[selectedAcc].payvault.Count > 0)
                            {
                                foreach (string derp in accs[selectedAcc].payvault)
                                {
                                    ihavethese.Add(derp);
                                }
                            }
                            //SetupImages();
                            rebuildGUI(false);
                            editArea.Focus();

                        }
                        //editArea.Focus();
                    }

                }

            }

        }
        //Settings
        private void settingsButton_Click(object sender, EventArgs e)
        {
            labeld:
            SettingsForm sf = new SettingsForm();
            if (sf.ShowDialog() == DialogResult.OK)
            {
                if (SettingsForm.reset)
                {
                    rebuildGUI(false);
                    SettingsForm.reset = false;
                    goto labeld;

                }
                else
                {
                    rebuildGUI(false);
                }
            }
        }
        //About
        private void aboutButton_Click(object sender, EventArgs e)
        {
            new About(this).ShowDialog();
        }
        #endregion

        //bottomFlowLayoutPanel
        #region statusToolStrip
        //No actions defined here - see EditArea_MouseMove on EditArea.cs for Mainform.*.Text values.
        #endregion

        #region findToolStrip
        //Filtering textbox
        void filterTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Return)
            {
                searched = filterTextBox.Text;
                rebuildGUI(false);
            }
        }
        //Block by color
        private void pickerButton_Click(object sender, EventArgs e) { SetColorPicker(); }
        public void SetColorPicker()
        {
            editArea.Tool.CleanUp(false);
            editArea.Tool = new ToolPicker(editArea);
            editArea.Tool.PenID = selectedBrick.ID;
            SetPenTool();
        }
        #endregion

        #region viewToolStrip
        //View tabs
        public void SetView()
        {
            var nextprev = userdata.lastSelectedBlockbar;
            if (decrease)
            {
                if (nextprev <= 4 && nextprev > 0)
                {
                    nextprev -= 1;
                }
                if (nextprev == 0) decrease = false;
            }
            else
            {
                if (nextprev >= 0 && nextprev < 4)
                {
                    nextprev += 1;
                }
                if (nextprev == 4) decrease = true;
            }
            switch (nextprev)
            {
                case 0:
                    showBlocksButton.PerformClick();
                    break;
                case 1:
                    showActionsButton.PerformClick();
                    break;
                case 2:
                    showDecorationsButton.PerformClick();
                    break;
                case 3:
                    showBackgroundsButton.PerformClick();
                    break;
                case 4:
                    unknownButton.PerformClick();
                    nextprev = 0;
                    break;
            }
        }
        private void showBlocksButton_Click(object sender, EventArgs e)
        {
            userdata.lastSelectedBlockbar = 0;
            flowLayoutPanel2.Visible = true;
            flowLayoutPanel3.Visible = false;
            flowLayoutPanel4.Visible = false;
            flowLayoutPanel5.Visible = false;
            flowLayoutPanel6.Visible = false;

            showBlocksButton.Checked = true;
            showActionsButton.Checked = false;
            showDecorationsButton.Checked = false;
            showBackgroundsButton.Checked = false;
            unknownButton.Checked = false;

        }
        private void showActionsButton_Click(object sender, EventArgs e)
        {
            userdata.lastSelectedBlockbar = 1;
            flowLayoutPanel2.Visible = false;
            flowLayoutPanel3.Visible = true;
            flowLayoutPanel4.Visible = false;
            flowLayoutPanel5.Visible = false;
            flowLayoutPanel6.Visible = false;

            showBlocksButton.Checked = false;
            showActionsButton.Checked = true;
            showDecorationsButton.Checked = false;
            showBackgroundsButton.Checked = false;
            unknownButton.Checked = false;
        }
        private void showDecorationsButton_Click(object sender, EventArgs e)
        {
            userdata.lastSelectedBlockbar = 2;
            flowLayoutPanel2.Visible = false;
            flowLayoutPanel3.Visible = false;
            flowLayoutPanel4.Visible = true;
            flowLayoutPanel5.Visible = false;
            flowLayoutPanel6.Visible = false;

            showBlocksButton.Checked = false;
            showActionsButton.Checked = false;
            showDecorationsButton.Checked = true;
            showBackgroundsButton.Checked = false;
            unknownButton.Checked = false;
        }
        private void showBackgroundsButton_Click(object sender, EventArgs e)
        {
            userdata.lastSelectedBlockbar = 3;
            flowLayoutPanel2.Visible = false;
            flowLayoutPanel3.Visible = false;
            flowLayoutPanel4.Visible = false;
            flowLayoutPanel5.Visible = true;
            flowLayoutPanel6.Visible = false;

            showBlocksButton.Checked = false;
            showActionsButton.Checked = false;
            showDecorationsButton.Checked = false;
            showBackgroundsButton.Checked = true;
            unknownButton.Checked = false;
        }
        private void unknownButton_Click(object sender, EventArgs e)
        {
            userdata.lastSelectedBlockbar = 4;
            flowLayoutPanel2.Visible = false;
            flowLayoutPanel3.Visible = false;
            flowLayoutPanel4.Visible = false;
            flowLayoutPanel5.Visible = false;
            flowLayoutPanel6.Visible = true;

            showBlocksButton.Checked = false;
            showActionsButton.Checked = false;
            showDecorationsButton.Checked = false;
            showBackgroundsButton.Checked = false;
            unknownButton.Checked = true;
            rebuildGUI(true);

        }
        // ---
        //Blockbar toggle
        private void hideBlocksButton_Click(object sender, EventArgs e)
        {
            if (hideBlocksButton.Checked == false)
            {
                flowLayoutPanel2.Visible = false;
                flowLayoutPanel3.Visible = false;
                flowLayoutPanel4.Visible = false;
                flowLayoutPanel5.Visible = false;
                flowLayoutPanel6.Visible = false;

                showBlocksButton.Enabled = false;
                showActionsButton.Enabled = false;
                showDecorationsButton.Enabled = false;
                showBackgroundsButton.Enabled = false;
                unknownButton.Enabled = false;
            }
            else
            {
                if (userdata.lastSelectedBlockbar == 0)
                {
                    flowLayoutPanel2.Visible = true;
                    flowLayoutPanel3.Visible = false;
                    flowLayoutPanel4.Visible = false;
                    flowLayoutPanel5.Visible = false;
                    flowLayoutPanel6.Visible = false;
                }
                else if (userdata.lastSelectedBlockbar == 1)
                {
                    flowLayoutPanel2.Visible = false;
                    flowLayoutPanel3.Visible = true;
                    flowLayoutPanel4.Visible = false;
                    flowLayoutPanel5.Visible = false;
                    flowLayoutPanel6.Visible = false;
                }
                else if (userdata.lastSelectedBlockbar == 2)
                {
                    flowLayoutPanel2.Visible = false;
                    flowLayoutPanel3.Visible = false;
                    flowLayoutPanel4.Visible = true;
                    flowLayoutPanel5.Visible = false;
                    flowLayoutPanel6.Visible = false;
                }
                else if (userdata.lastSelectedBlockbar == 3)
                {
                    flowLayoutPanel2.Visible = false;
                    flowLayoutPanel3.Visible = false;
                    flowLayoutPanel4.Visible = false;
                    flowLayoutPanel5.Visible = true;
                    flowLayoutPanel6.Visible = false;
                }
                else if (userdata.lastSelectedBlockbar == 4)
                {
                    flowLayoutPanel2.Visible = false;
                    flowLayoutPanel3.Visible = false;
                    flowLayoutPanel4.Visible = false;
                    flowLayoutPanel5.Visible = false;
                    flowLayoutPanel6.Visible = true;
                }

                showBlocksButton.Enabled = true;
                showActionsButton.Enabled = true;
                showDecorationsButton.Enabled = true;
                showBackgroundsButton.Enabled = true;
                unknownButton.Enabled = true;

            }
        }
        //Minimap toggle
        private void minimapButton_Click(object sender, EventArgs e)
        {
            if (minimapButton.Checked)
            {
                minimap.BringToFront();
            }
            else
            {
                editArea.BringToFront();
            }
        }
        #endregion

        //Misc
        #region unknownToolStrip
        List<int> delay;

        private void frameSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!starting)
            {
                editArea.changeFrame(frameSelector.SelectedIndex);
                delayTextBox.Enabled = false;
                delayTextBox.Text = "-Delay-";
            }
        }

        private void delayTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (frameSelector.SelectedIndex != 0)
                    delay[frameSelector.SelectedIndex - 1] = int.Parse(delayTextBox.Text);
            }
            catch
            {
                delayTextBox.Text = delay[frameSelector.SelectedIndex - 1].ToString();
                delayTextBox.Invalidate();
            }
        }

        private void delayTextBox_Click(object sender, EventArgs e)
        {

        }

        private void addButton_Click(object sender, EventArgs e)
        {
            int n = frameSelector.Items.Count;
            /*delay.Add(750);
            for (int i = delay.Count - 1; i >= frameSelector.SelectedIndex + 1; --i)
            {
                int t = delay[i];
                delay[i] = delay[i - 1];
                delay[i - 1] = t;
            }
             */
            frameSelector.Items.Add("Frame " + n);
            editArea.createFrame(frameSelector.SelectedIndex + 1);
            frameSelector.SelectedIndex++;
        }

        private void subButton_Click(object sender, EventArgs e)
        {
            if (frameSelector.Items.Count > 0)
            {
                frameSelector.Items.Clear();
            }
        }

        #endregion

        #region Workaround to bind hotkeys to buttons
        public void SetTool(int o)
        {
            switch (o)
            {
                case 0:
                    newWorldButton.PerformClick();
                    break;
                case 1:
                    openWorldDropButton.ShowDropDown();
                    break;
                case 2:
                    saveDropButton.ShowDropDown();
                    break;
                case 3:
                    imageButton.PerformClick();
                    break;
                case 4:
                    textButton.PerformClick();
                    break;
                case 5:
                    replaceButton.PerformClick();
                    break;
                case 6:
                    levelTextbox.Focus();
                    break;
                case 7:
                    refreshButton.PerformClick();
                    break;
                case 8:
                    codeTextbox.Focus();
                    break;
                case 9:
                    uploadButton.PerformClick();
                    break;
                case 10:
                    hideBlocksButton.PerformClick();
                    break;
                case 11:
                    minimapButton.PerformClick();
                    break;
                case 12:
                    settingsButton.PerformClick();
                    break;
                case 13:
                    aboutButton.PerformClick();
                    break;
                case 14:
                    undoButton.PerformClick();
                    break;
                case 15:
                    redoButton.PerformClick();
                    break;
                case 16:
                    historyButton.PerformClick();
                    break;
                case 17:
                    mirrorButton.PerformClick();
                    break;
                case 18:
                    flipButton.PerformClick();
                    break;
                case 19:
                    rotateLeftButton.PerformClick();
                    break;
                case 20:
                    rotateRightButton.PerformClick();
                    break;

            }

        }
        #endregion

        #region Form loading and closing
        private void MainForm_Load(object sender, EventArgs e)
        {
            if (userdata.updateChecker == true)
            {

                About ab = new About(this);
                Thread thread = new Thread(delegate () { ab.checkVersion(true); });
                thread.Start();
            }

        }
        private void MainForm_Activated(object sender, EventArgs e)
        {
            codeTextbox.Text = userdata.levelPass;
            levelTextbox.Text = userdata.level;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MainForm.userdata.confirmClose == true)
            {
                DialogResult dr = MessageBox.Show("Are you sure you want to exit EEditor?", "Quit EEditor?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }

            string s = "";
            for (int i = 0; i < 10; i++)
            {
                if (i != 0) s += ",";
                s += shortCutButtons[i] == null ? -1 : shortCutButtons[i].ID;
            }
            userdata.brickHotkeys = s;
            userdata.openWorld = false;

            File.WriteAllText(pathSettings, JsonConvert.SerializeObject(userdata, Newtonsoft.Json.Formatting.Indented));
            //Clear block rotations
            ToolPen.rotation.Clear();
            ToolPen.text.Clear();
            ToolPen.id.Clear();
            ToolPen.target.Clear();
            try
            {
                timer.Stop();
            }
            catch
            {

            }
        }
        #endregion

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (debug == false) debug = true;
            else debug = false;
            rebuildGUI(false);
        }

        private void lastUsedBlockButton_Click(object sender, EventArgs e)
        {

        }

        private void lastUsedBlockButton(object sender, EventArgs e)
        {
            setBrick(Convert.ToInt32(((ToolStripButton)sender).Name), false);
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {

        }

        private void imageToBlocksButton_Click(object sender, EventArgs e)
        {
            Form frm = new ImageToBlocks();
            frm.ShowDialog();
        }
    }
    public class ownedBlocks
    {
        public int mode { get; set; }
        public int[] blocks { get; set; }
        public string name { get; set; }
    }
    public class accounts
    {
        public int loginMethod { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public List<JToken> payvault { get; set; }
    }
    public class unknownBlock
    {
        public unknownBlock()
        {

        }
        public unknownBlock(int id, int layer, int blockdata, int blockdata1, int blockdata2, string blockdata3)
        {
            this.ID = id;
            this.Layer = layer;
            this.Blockdata = blockdata;
            this.Blockdata1 = blockdata1;
            this.Blockdata2 = blockdata2;
            this.Blockdata3 = blockdata3;
        }
        public int ID { get; set; }
        public int Layer { get; set; }
        public int Blockdata { get; set; }
        public int Blockdata1 { get; set; }
        public int Blockdata2 { get; set; }
        public string Blockdata3 { get; set; }
    }
    public class userData
    {
        public string level { get; set; }
        public bool resetHotkeys { get; set; }
        public string levelPass { get; set; }
        public string brickHotkeys { get; set; }
        public bool showhitboxes { get; set; }
        public bool usePenTool { get; set; }
        public bool useColor { get; set; }
        public bool debugBlocksTooltip { get; set; }
        public bool dontReplaceBlocks { get; set; }
        public int uploadDelay { get; set; }
        public int lastSelectedBlockbar { get; set; }
        public Color thisColor { get; set; }
        public string username { get; set; }
        public int sprayr { get; set; }
        public int sprayp { get; set; }
        public bool updateChecker { get; set; }
        public List<JToken> newestBlocks { get; set; }
        public bool selectAllBorder { get; set; }
        public bool backgroundOnlyShift { get; set; }
        public bool openWorld { get; set; }
        public bool saveWorldCrew { get; set; }
        public bool confirmClose { get; set; }
        public int uploadOption { get; set; }
        public Color themeBlock { get; set; }
        public Color themeBlocks { get; set; }
        public Color themeIcons { get; set; }
        public Color themeIconsBg { get; set; }
        public Color themeToolbarBg { get; set; }
        public Color themeToolbarText { get; set; }
        public Color themeBlockBG { get; set; }
        public bool themeBorder { get; set; }
        public bool themeClean { get; set; }
        public bool drawMixed { get; set; }
        public bool imageBlocks { get; set; }
        public bool imageBackgrounds { get; set; }
        public bool imageSpecialblocksMorph { get; set; }
        public bool imageSpecialblocksAction { get; set; }
        public bool reverse { get; set; }
        public bool random { get; set; }
        public bool openCodeWorld { get; set; }

    }
    public class removeBadRenderer : ToolStripSystemRenderer
    {
        public removeBadRenderer() { }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            //base.OnRenderToolStripBorder(e);
        }
    }
}
