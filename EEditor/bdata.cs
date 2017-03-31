﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayerIOClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
namespace EEditor
{
    class bdata
    {
        public static int[] goal = { 77, 83, 43, 165, 213, 214, 417, 418, 419, 420, 421, 422, 423, 1027, 1028, 113, 185, 184, 1011, 1012, 453, 461, 467, 1079, 1080 };
        //public static int[] effects = { 417, 418, 419, 420, 421, 422, 453 };
        public static int[] rotate = { 1001, 1002, 1003, 1004, 1027, 1028, 361, 385, 374, 1052, 1053, 1054, 1055, 1056, 1092};
        public static int[] ignore = { 1001, 1002, 1003, 1004, 361, 417, 418, 419, 420, 1052, 1053, 1054, 1055, 1056, 1092, 461 };
        public static int[] morphable = { 375, 376, 379, 380, 377, 378, 438, 439, 276, 277, 279, 280, 440, 275, 329, 273, 328, 327, 338, 339, 340, 1041, 1042, 1043, 456, 457, 458, 447, 448, 449, 450, 451, 452, 464, 465, 1075, 1076, 1077, 1078, 471, 475, 476, 477, 481, 482, 483, 497, 492, 493, 494, 499, 1500, 1502,1507, 1506,1101,1102,1103,1104,1105 };
        public static int[] portals = { 242, 381 };

        //ToolPen (Increase up to 1, 2, 3, 5)
        public static int[] increase3 = { 1001, 1002, 1003, 1004, 361, 375, 376, 377, 378, 379, 380, 438, 439, 275, 329, 273, 328, 327, 338, 339, 340, 1041, 1042, 1043, 447, 448, 449, 450, 451, 452, 1052, 1053, 1054, 1055, 1056, 1075, 1076, 1077, 1078, 1092, 492, 493, 494, 499, 1502 };
        public static int[] increase2 = { 276, 277, 279, 280, 461, 471, 475, 476, 477, 483, 1500 };
        public static int[] increase1 = { 417, 418, 419, 420, 453, 456, 457, 458 };
        public static int[] increase4 = { 1507, 1506, 464, 465 };
        public static int[] increase5 = { 440, 481, 482, 497 };

        public static bool isBg(int id)
        {
            if (id >= 500 && id <= 999)
            {
                return true;
            }
            else if (id < 500 || id >= 1001)
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        public static bool ParamNumbers(PlayerIOClient.Message m, int message, string type)
        {
            switch (m.Type)
            {
                case "init":
                    if (m[(uint)message].GetType().ToString() == type) { return true; }
                    break;
            }
            return false;
        }

        public static Bitmap getRotation(int fid, int coins)
        {
            if (fid == 385)
            {
                int roted = 15;
                switch (coins)
                {
                    case 0:
                        if (fid == 385) roted = 255;
                        break;
                    case 1:
                        if (fid == 385) roted = 256;
                        break;
                    case 2:
                        if (fid == 385) roted = 257;
                        break;
                    case 3:
                        if (fid == 385) roted = 258;
                        break;
                }

                Bitmap bmp2 = MainForm.miscBMD.Clone(new Rectangle(roted * 16, 0, 16, 16), MainForm.miscBMD.PixelFormat);
                return bmp2;
            }
            else if (bdata.morphable.Contains(fid))
            {
                if (fid >= 1101 && fid <= 1105)
                {
                    Bitmap bmp2 = MainForm.foregroundBMD.Clone(new Rectangle(MainForm.foregroundBMI[fid] * 16, 0, 16, 16), MainForm.foregroundBMD.PixelFormat);
                    return bmp2;
                }
                else if (fid == 276 || fid == 277 || fid == 279 || fid == 280 || fid == 338 || fid == 339 || fid == 340 || fid == 1041 || fid == 1042 || fid == 1043 || fid == 456 || fid == 457 || fid == 458 || fid == 447 || fid == 448 || fid == 449 || fid == 450 || fid == 451 || fid == 452 || fid >= 1075 && fid <= 1078 || fid == 471 || fid == 475 || fid == 476 || fid == 477 || fid == 497 || fid == 492 || fid == 493 || fid == 494 || fid == 499 || fid == 1500 || fid == 1502 || fid == 1506 || fid == 1507)
                {
                    int roted = 15;
                    switch (coins)
                    {
                        case 0:
                            if (fid == 276) roted = 140;
                            if (fid == 277) roted = 143;
                            if (fid == 279) roted = 146;
                            if (fid == 280) roted = 149;
                            if (fid == 338) roted = 136;
                            if (fid == 339) roted = 132;
                            if (fid == 340) roted = 152;
                            if (fid == 1041) roted = 202;
                            if (fid == 1042) roted = 206;
                            if (fid == 1043) roted = 210;
                            if (fid == 456) roted = 214;
                            if (fid == 457) roted = 216;
                            if (fid == 458) roted = 218;
                            if (fid == 447) roted = 178;
                            if (fid == 448) roted = 182;
                            if (fid == 449) roted = 186;
                            if (fid == 450) roted = 190;
                            if (fid == 451) roted = 194;
                            if (fid == 452) roted = 198;
                            if (fid == 1075) roted = 263;
                            if (fid == 1076) roted = 267;
                            if (fid == 1077) roted = 271;
                            if (fid == 1078) roted = 275;
                            if (fid == 471) roted = 279;
                            if (fid == 475) roted = 282;
                            if (fid == 476) roted = 285;
                            if (fid == 477) roted = 288;
                            if (fid == 497) roted = 311;
                            if (fid == 492) roted = 318;
                            if (fid == 493) roted = 322;
                            if (fid == 494) roted = 326;
                            if (fid == 499) roted = 330;
                            if (fid == 1500) roted = 334;
                            if (fid == 1502) roted = 339;
                            if (fid == 1507) roted = 342;
                            if (fid == 1506) roted = 347;
                            break;
                        case 1:
                            if (fid == 276) roted = 141;
                            if (fid == 277) roted = 144;
                            if (fid == 279) roted = 147;
                            if (fid == 280) roted = 150;
                            if (fid == 338) roted = 137;
                            if (fid == 339) roted = 133;
                            if (fid == 340) roted = 153;
                            if (fid == 1041) roted = 203;
                            if (fid == 1042) roted = 207;
                            if (fid == 1043) roted = 211;
                            if (fid == 456) roted = 215;
                            if (fid == 457) roted = 217;
                            if (fid == 458) roted = 219;
                            if (fid == 447) roted = 179;
                            if (fid == 448) roted = 183;
                            if (fid == 449) roted = 187;
                            if (fid == 450) roted = 191;
                            if (fid == 451) roted = 195;
                            if (fid == 452) roted = 199;
                            if (fid == 1075) roted = 264;
                            if (fid == 1076) roted = 268;
                            if (fid == 1077) roted = 272;
                            if (fid == 1078) roted = 276;
                            if (fid == 471) roted = 280;
                            if (fid == 475) roted = 283;
                            if (fid == 476) roted = 286;
                            if (fid == 477) roted = 289;
                            if (fid == 497) roted = 312;
                            if (fid == 492) roted = 319;
                            if (fid == 493) roted = 323;
                            if (fid == 494) roted = 327;
                            if (fid == 499) roted = 331;
                            if (fid == 1500) roted = 335;
                            if (fid == 1502) roted = 338;
                            if (fid == 1507) roted = 343;
                            if (fid == 1506) roted = 348;
                            break;
                        case 2:
                            if (fid == 276) roted = 142;
                            if (fid == 277) roted = 145;
                            if (fid == 279) roted = 148;
                            if (fid == 280) roted = 151;
                            if (fid == 338) roted = 138;
                            if (fid == 339) roted = 134;
                            if (fid == 340) roted = 154;
                            if (fid == 1041) roted = 204;
                            if (fid == 1042) roted = 208;
                            if (fid == 1043) roted = 212;
                            if (fid == 447) roted = 180;
                            if (fid == 448) roted = 184;
                            if (fid == 449) roted = 188;
                            if (fid == 450) roted = 192;
                            if (fid == 451) roted = 196;
                            if (fid == 452) roted = 200;
                            if (fid == 1075) roted = 265;
                            if (fid == 1076) roted = 269;
                            if (fid == 1077) roted = 273;
                            if (fid == 1078) roted = 277;
                            if (fid == 471) roted = 281;
                            if (fid == 475) roted = 284;
                            if (fid == 476) roted = 287;
                            if (fid == 477) roted = 290;
                            if (fid == 497) roted = 313;
                            if (fid == 492) roted = 320;
                            if (fid == 493) roted = 324;
                            if (fid == 494) roted = 328;
                            if (fid == 499) roted = 332;
                            if (fid == 1502) roted = 337;
                            if (fid == 1507) roted = 344;
                            if (fid == 1506) roted = 349;
                            break;
                        case 3:
                            if (fid == 338) roted = 139;
                            if (fid == 339) roted = 135;
                            if (fid == 340) roted = 155;
                            if (fid == 1041) roted = 205;
                            if (fid == 1042) roted = 209;
                            if (fid == 1043) roted = 213;
                            if (fid == 447) roted = 181;
                            if (fid == 448) roted = 185;
                            if (fid == 449) roted = 189;
                            if (fid == 450) roted = 193;
                            if (fid == 451) roted = 197;
                            if (fid == 452) roted = 201;
                            if (fid == 1075) roted = 266;
                            if (fid == 1076) roted = 270;
                            if (fid == 1077) roted = 274;
                            if (fid == 1078) roted = 278;
                            if (fid == 497) roted = 314;
                            if (fid == 492) roted = 321;
                            if (fid == 493) roted = 325;
                            if (fid == 494) roted = 329;
                            if (fid == 499) roted = 333;
                            if (fid == 1502) roted = 336;
                            if (fid == 1507) roted = 345;
                            if (fid == 1506) roted = 350;
                            break;
                        case 4:
                            if (fid == 497) roted = 315;
                            if (fid == 1507) roted = 346;
                            if (fid == 1506) roted = 351;
                            break;
                        case 5:
                            if (fid == 497) roted = 316;
                            break;
                        default:
                            roted = 15;
                            break;
                    }
                    
                    Bitmap bmp2 = MainForm.miscBMD.Clone(new Rectangle(roted * 16, 0, 16, 16), MainForm.miscBMD.PixelFormat);
                    return bmp2;
                }
                //Medieval
                else if (fid == 440 || fid == 275 || fid == 329 || fid == 273 || fid == 328 || fid == 327)
                {
                    int roted = 15;
                    switch (coins)
                    {
                        case 0:
                            if (fid == 440) roted = 168;
                            if (fid == 275) roted = 116;
                            if (fid == 329) roted = 128;
                            if (fid == 273) roted = 124;
                            if (fid == 328) roted = 156;
                            if (fid == 327) roted = 120;
                            break;
                        case 1:
                            if (fid == 440) roted = 169;
                            if (fid == 275) roted = 117;
                            if (fid == 329) roted = 129;
                            if (fid == 273) roted = 125;
                            if (fid == 328) roted = 157;
                            if (fid == 327) roted = 121;
                            break;
                        case 2:
                            if (fid == 440) roted = 170;
                            if (fid == 275) roted = 118;
                            if (fid == 329) roted = 130;
                            if (fid == 273) roted = 126;
                            if (fid == 328) roted = 158;
                            if (fid == 327) roted = 122;
                            break;
                        case 3:
                            if (fid == 440) roted = 171;
                            if (fid == 275) roted = 119;
                            if (fid == 329) roted = 131;
                            if (fid == 273) roted = 127;
                            if (fid == 328) roted = 159;
                            if (fid == 327) roted = 123;
                            break;
                        case 4:
                            if (fid == 440) roted = 172;
                            break;
                        case 5:
                            if (fid == 440) roted = 173;
                            break;
                        default:
                            roted = 15;
                            break;
                    }
                    Bitmap bmp2 = MainForm.miscBMD.Clone(new Rectangle(roted * 16, 0, 16, 16), MainForm.miscBMD.PixelFormat);
                    return bmp2;
                }
                else if (fid >= 375 && fid <= 380 || fid == 438 || fid == 439)
                {
                    int roted = 15;
                    switch (coins)
                    {
                        case 0:
                            if (fid == 375) { roted = 34; }
                            else if (fid == 376) { roted = 38; }
                            else if (fid == 377) { roted = 40; }
                            else if (fid == 378) { roted = 44; }
                            else if (fid == 379) { roted = 46; }
                            else if (fid == 380) { roted = 50; }
                            else if (fid == 438) roted = 160;
                            else if (fid == 439) roted = 164;
                            else { roted = 15; }
                            break;
                        case 1:
                            if (fid == 375) { roted = 35; }
                            else if (fid == 376) { roted = 39; }
                            else if (fid == 377) { roted = 41; }
                            else if (fid == 378) { roted = 45; }
                            else if (fid == 379) { roted = 47; }
                            else if (fid == 380) { roted = 51; }
                            else if (fid == 438) roted = 161;
                            else if (fid == 439) roted = 165;

                            else { roted = 15; }
                            break;
                        case 2:
                            if (fid == 375) { roted = 36; }
                            else if (fid == 438) roted = 162;
                            else if (fid == 376) { roted = 38; }
                            else if (fid == 377) { roted = 42; }
                            else if (fid == 378) { roted = 44; }
                            else if (fid == 379) { roted = 48; }
                            else if (fid == 380) { roted = 50; }
                            else if (fid == 439) roted = 164;
                            else { roted = 15; }
                            break;
                        case 3:
                            if (fid == 375) { roted = 37; }
                            else if (fid == 438) roted = 163;
                            else if (fid == 376) { roted = 39; }
                            else if (fid == 377) { roted = 43; }
                            else if (fid == 378) { roted = 45; }
                            else if (fid == 379) { roted = 49; }
                            else if (fid == 380) { roted = 51; }
                            else if (fid == 439) roted = 165;
                            else { roted = 15; }
                            break;

                        default:
                            roted = 15;
                            break;
                    }
                    Bitmap bmp2 = MainForm.miscBMD.Clone(new Rectangle(roted * 16, 0, 16, 16), MainForm.miscBMD.PixelFormat);
                    return bmp2;
                }
                //New Year 2015
                else if (fid == 464 || fid == 465)
                {
                    int roted = 15;
                    switch (coins)
                    {
                        case 0:
                            if (fid == 464) roted = 243;
                            if (fid == 465) roted = 247;
                            break;
                        case 1:
                            if (fid == 464) roted = 244;
                            if (fid == 465) roted = 248;
                            break;
                        case 2:
                            if (fid == 464) roted = 245;
                            if (fid == 465) roted = 249;
                            break;
                        case 3:
                            if (fid == 464) roted = 246;
                            if (fid == 465) roted = 250;
                            break;
                        case 4:
                            if (fid == 464) roted = 353;
                            if (fid == 465) roted = 354;
                            break;

                        default:
                            roted = 15;
                            break;
                    }
                    Bitmap bmp2 = MainForm.miscBMD.Clone(new Rectangle(roted * 16, 0, 16, 16), MainForm.miscBMD.PixelFormat);
                    return bmp2;
                }
                    //Summer 2016
                else if (fid == 481 || fid == 482 || fid == 483)
                {
                    int roted = 15;
                    switch (coins)
                    {
                        case 0:
                            if (fid == 481) roted = 291;
                            if (fid == 482) roted = 297;
                            if (fid == 483) roted = 303;
                            break;
                        case 1:
                            if (fid == 481) roted = 292;
                            if (fid == 482) roted = 298;
                            if (fid == 483) roted = 304;
                            break;
                        case 2:
                            if (fid == 481) roted = 293;
                            if (fid == 482) roted = 299;
                            if (fid == 483) roted = 305;
                            break;
                        case 3:
                            if (fid == 481) roted = 294;
                            if (fid == 482) roted = 300;
                            if (fid == 483) roted = 306;
                            break;
                        case 4:
                            if (fid == 481) roted = 295;
                            if (fid == 482) roted = 301;
                            break;
                        case 5:
                            if (fid == 481) roted = 296;
                            if (fid == 482) roted = 302;
                            break;
                        default:
                            roted = 15;
                            break;
                    }
                    Bitmap bmp2 = MainForm.miscBMD.Clone(new Rectangle(roted * 16, 0, 16, 16), MainForm.miscBMD.PixelFormat);
                    return bmp2;
                }
                else
                {
                    return null;
                }
            }
            else if (fid == 361)
            {
                var roted = 15;
                switch (coins)
                {
                    case 0:
                        roted = 23;
                        break;
                    case 1:
                        roted = 24;
                        break;
                    case 2:
                        roted = 25;
                        break;
                    case 3:
                        roted = 26;
                        break;
                    default:
                        roted = 23;
                        break;
                }
                Bitmap bmp2 = MainForm.miscBMD.Clone(new Rectangle(roted * 16, 0, 16, 16), MainForm.miscBMD.PixelFormat);
                return bmp2;
            }
            else if (fid >= 417 && fid <= 420 || fid == 453 || fid == 461)
            {
                var roted = 15;
                switch (coins)
                {
                    case 0:
                        if (fid == 417) roted = 81;
                        if (fid == 418) roted = 82;
                        if (fid == 419) roted = 83;
                        if (fid == 420) roted = 84;
                        if (fid == 453) roted = 177;
                        if (fid == 461) roted = 252;
                        break;
                    case 1:
                        if (fid == 417) roted = 74;
                        if (fid == 418) roted = 75;
                        if (fid == 419) roted = 76;
                        if (fid == 420) roted = 77;
                        if (fid == 453) roted = 176;
                        if (fid == 461) roted = 253;
                        break;
                    case 2:
                        if (fid == 461) roted = 254;
                        break;
                }
                Bitmap bmp2 = MainForm.miscBMD.Clone(new Rectangle(roted * 16, 0, 16, 16), MainForm.miscBMD.PixelFormat);
                return bmp2;
            }
            else if (fid >= 421 && fid <= 423 || fid == 1027 || fid == 1028)
            {
                var roted = 15;
                switch (coins)
                {
                    case 0:
                        if (fid == 417) { roted = 81; }
                        else if (fid == 421) roted = 85;
                        else if (fid == 422) { roted = 86; }
                        else if (fid == 423) roted = 80;
                        else if (fid == 1028) roted = 100;
                        else if (fid == 1027) roted = 93;
                        break;
                    case 1:
                        if (fid == 417) { roted = 74; }
                        else if (fid == 421) roted = 78;
                        else if (fid == 422) { roted = 79; }
                        else if (fid == 423) roted = 87;
                        else if (fid == 1028) roted = 101;
                        else if (fid == 1027) roted = 94;
                        break;
                    case 2:
                        if (fid == 422) { roted = 79; }
                        else if (fid == 421) roted = 78;
                        else if (fid == 423) roted = 88;
                        else if (fid == 1028) roted = 102;
                        else if (fid == 1027) roted = 95;
                        break;
                    case 3:
                        if (fid == 422) { roted = 79; }
                        else if (fid == 421) roted = 78;
                        else if (fid == 423) roted = 89;
                        else if (fid == 1028) roted = 103;
                        else if (fid == 1027) roted = 96;
                        break;
                    case 4:
                        if (fid == 422) { roted = 79; }
                        else if (fid == 421) roted = 78;
                        else if (fid == 423) roted = 90;
                        else if (fid == 1028) roted = 104;
                        else if (fid == 1027) roted = 97;
                        break;
                    case 5:
                        if (fid == 422) { roted = 79; }
                        else if (fid == 421) roted = 78;
                        else if (fid == 423) roted = 91;
                        else if (fid == 1028) roted = 105;
                        else if (fid == 1027) roted = 98;
                        break;
                    case 6:
                        if (fid == 422) { roted = 79; }
                        else if (fid == 421) roted = 78;
                        else if (fid == 423) roted = 92;
                        else if (fid == 1028) roted = 106;
                        else if (fid == 1027) roted = 99;
                        break;
                    default:
                        if (fid == 422) { roted = 79; }
                        else if (fid == 421) roted = 78;
                        break;
                }
                Bitmap bmp2 = MainForm.miscBMD.Clone(new Rectangle(roted * 16, 0, 16, 16), MainForm.miscBMD.PixelFormat);
                return bmp2;
            }
            //One-Way blocks
            else if (fid >= 1001 && fid <= 1004 || fid == 1052 || fid == 1053 || fid == 1054 || fid == 1055 || fid == 1055 || fid == 1056 || fid == 1092)
            {
                var rote = 15;
                switch (coins)
                {
                    case 0:
                        if (fid == 1001) { rote = 54; }
                        if (fid == 1003) { rote = 58; }
                        if (fid == 1002) { rote = 62; }
                        if (fid == 1004) { rote = 66; }
                        if (fid == 1052) rote = 223;
                        if (fid == 1053) rote = 227;
                        if (fid == 1054) rote = 231;
                        if (fid == 1055) rote = 235;
                        if (fid == 1056) rote = 239;
                        if (fid == 1092) rote = 307;
                        
                        break;
                    case 1:
                        if (fid == 1001) { rote = 55; }
                        if (fid == 1003) { rote = 59; }
                        if (fid == 1002) { rote = 63; }
                        if (fid == 1004) { rote = 67; }
                        if (fid == 1052) rote = 224;
                        if (fid == 1053) rote = 228;
                        if (fid == 1054) rote = 232;
                        if (fid == 1055) rote = 236;
                        if (fid == 1056) rote = 240;
                        if (fid == 1092) rote = 308;
                        break;
                    case 2:
                        if (fid == 1001) { rote = 56; }
                        if (fid == 1003) { rote = 60; }
                        if (fid == 1002) { rote = 64; }
                        if (fid == 1004) { rote = 68; }
                        if (fid == 1052) rote = 225;
                        if (fid == 1053) rote = 229;
                        if (fid == 1054) rote = 233;
                        if (fid == 1055) rote = 237;
                        if (fid == 1056) rote = 241;
                        if (fid == 1092) rote = 309;
                        break;
                    case 3:
                        if (fid == 1001) { rote = 57; }
                        if (fid == 1003) { rote = 61; }
                        if (fid == 1002) { rote = 65; }
                        if (fid == 1004) { rote = 69; }
                        if (fid == 1052) rote = 226;
                        if (fid == 1053) rote = 230;
                        if (fid == 1054) rote = 234;
                        if (fid == 1055) rote = 238;
                        if (fid == 1056) rote = 242;
                        if (fid == 1092) rote = 310;
                        break;
                }
                Bitmap bmp2 = MainForm.miscBMD.Clone(new Rectangle(rote * 16, 0, 16, 16), MainForm.miscBMD.PixelFormat);
                return bmp2;
            }
            else if (fid == 242 || fid == 381)
            {
                var rote = 15;
                switch (coins)
                {
                    case 0:
                        if (fid == 242) rote = 108;
                        if (fid == 381) rote = 112;
                        break;
                    case 1:
                        if (fid == 242) rote = 109;
                        if (fid == 381) rote = 113;
                        break;
                    case 2:
                        if (fid == 242) rote = 110;
                        if (fid == 381) rote = 114;
                        break;
                    case 3:
                        if (fid == 242) rote = 111;
                        if (fid == 381) rote = 115;
                        break;
                }
                Bitmap bmp2 = MainForm.miscBMD.Clone(new Rectangle(rote * 16, 0, 16, 16), MainForm.miscBMD.PixelFormat);
                return bmp2;
            }
            else 
            {
                return null;
            }
        }
        public static Image SetImageOpacity(Image image, float opacity)
        {
            try
            {
                //create a Bitmap the size of the image provided  
                Bitmap bmp = new Bitmap(image.Width, image.Height);

                //create a graphics object from the image  
                using (Graphics gfx = Graphics.FromImage(bmp))
                {

                    //create a color matrix object  
                    ColorMatrix matrix = new ColorMatrix();

                    //set the opacity  
                    matrix.Matrix33 = opacity;

                    //create image attributes  
                    ImageAttributes attributes = new ImageAttributes();

                    //set the color(opacity) of the image  
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    //now draw the image  
                    gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
                return bmp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }
    }

}