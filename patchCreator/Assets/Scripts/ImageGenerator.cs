using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking.Match;
using Graphics = System.Drawing.Graphics;

namespace Assets.Scripts {
    public class ImageGenerator 
    {
        public static int PaperSize = 512;

        private static bool IsValidChar(char c)
        {
            return ' ' < c && c < '~';
        }

        private static string ReadText(StreamReader sr)
        {
            while (!IsValidChar(Convert.ToChar(sr.Peek()))) sr.Read();
            var result = "";

            while (IsValidChar(Convert.ToChar(sr.Peek())))
            {
                result += Convert.ToChar(sr.Read());
            }

            return result;
        }

        static int ReadNumber(StreamReader sr)
        {
            string temp = ReadText(sr);
            return Convert.ToInt32(temp);
        }

        static float ReadFloat(StreamReader sr)
        {
            return Convert.ToSingle(ReadText(sr));
        }

        static float getAngle(float x1, float y1, float x2, float y2) {
            float cos = (x1 * x2 + y1 * y2) / ((float)Math.Sqrt(x1 * x1 + y1 * y1) * (float)Math.Sqrt(x2 * x2 + y2 * y2));
            return (float)Math.Acos(cos);
        }




        [Serializable]
        public class Face
        {
            public int Num;
            public List<Point> Points = new List<Point>();    //
            public List<PointF> Uvfront = new List<PointF>();  //
            public List<PointF> Uvback = new List<PointF>();   //
            public int Dir;               //
            public int ColorFront;        //正面颜色
            public int ColorBack;         //反面颜色
            public int MaterialFront = 0;     //-1 表示颜色 0表示默认 1~n表示贴图编号
            public int MaterialBack = 0;      // 
            public string ImageFront = "";
            public string ImageBack = "";

        }

        static void ReadFromOldFile(string filePath, ref int pointsSize, ref int patchSize, ref int materialSize,
            ArrayList list, ArrayList points, ArrayList materials
            ) {
            FileStream fs = new FileStream(filePath, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            pointsSize = ReadNumber(sr);
            patchSize = ReadNumber(sr);
            materialSize = ReadNumber(sr);
            for (int i = 0; i < pointsSize; i++)
            {
                ReadFloat(sr);
                ReadFloat(sr);
                ReadFloat(sr);
            }
        
            for (int i = 0; i < pointsSize; i++)
            {
                Point p = new Point();
                p.X = (int)(ReadFloat(sr) * PaperSize);
                p.Y = PaperSize - (int)(ReadFloat(sr) * PaperSize);
                points.Add(p);
            }

            for (int i = 0; i < materialSize; i++)
            {
                materials.Add(ReadText(sr));
            }
            for (int i = 0; i < patchSize; i++)
            {
                Face f = new Face();
                f.Num = ReadNumber(sr);
                f.Points = new List<Point>();
                for (int j = 0; j < f.Num; j++)
                {
                    f.Points.Add((Point) points[ReadNumber(sr)]);
                }
                f.Dir = ReadNumber(sr);
                f.ColorFront = ReadNumber(sr);
                f.ColorBack = ReadNumber(sr);
                f.MaterialFront = ReadNumber(sr);
                f.Uvfront = new List<PointF>();
                if (f.MaterialFront != -1)
                {
                    for (int j = 0; j < f.Num; j++)
                    {
                        PointF p = new PointF();
                        p.X = ReadFloat(sr);
                        p.Y = 1 - ReadFloat(sr);
                        f.Uvfront.Add(p);
                    }
                }
                f.MaterialBack = ReadNumber(sr);
                f.Uvback = new List<PointF>();
                if (f.MaterialBack != -1)
                {
                    for (int j = 0; j < f.Num; j++)
                    {
                        PointF p = new PointF();
                        p.X = ReadFloat(sr);
                        p.Y = 1 - ReadFloat(sr);
                        f.Uvback.Add(p);
                    }
                }
                list.Add(f);
            }
        }


        private static void Draw(System.Drawing.Graphics pic, List<Point> points, int c, int material, string imageNames, List<PointF> uv) {
            if (material == -1)
            {
                
                System.Drawing.Color color = System.Drawing.Color.FromArgb(255, (c >> 16) & 255, (c >> 8) & 255, c & 255);
                SolidBrush brush = new SolidBrush(color);
                pic.FillPolygon(brush, points.ToArray());
            }
            else if (material >= 0)
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(imageNames);
                float w = image.Width;
                float h = image.Height;
                TextureBrush brush = new TextureBrush(image) {WrapMode = System.Drawing.Drawing2D.WrapMode.Clamp};

                float src_x1 = ((PointF)uv[0]).X * w;
                float src_y1 = ((PointF)uv[0]).Y * h;
                float src_x2 = ((PointF)uv[1]).X * w;
                float src_y2 = ((PointF)uv[1]).Y * h;
                float dst_x1 = ((Point)points[0]).X;
                float dst_y1 = ((Point)points[0]).Y;
                float dst_x2 = ((Point)points[1]).X;
                float dst_y2 = ((Point)points[1]).Y;
                float src_px = src_x2 - src_x1;
                float src_py = src_y2 - src_y1;
                float dst_px = dst_x2 - dst_x1;
                float dst_py = dst_y2 - dst_y1;
                float src_l = (float)Math.Sqrt(src_px * src_px + src_py * src_py);
                float dst_l = (float)Math.Sqrt(dst_px * dst_px + dst_py * dst_py);
                float cos_angle = (src_px * dst_px + src_py * dst_py) / src_l / dst_l;
                float sin_angle = (src_px * dst_py - src_py * dst_px) / src_l / dst_l;
                float scale = (float)Math.Sqrt((dst_px * dst_px + dst_py * dst_py) / (src_px * src_px + src_py * src_py));

                brush.TranslateTransform(dst_x1, dst_y1);
                brush.ScaleTransform(scale, scale);
                if (sin_angle > 0)
                    brush.RotateTransform(-(float)(180.0 * Math.Acos(cos_angle) / Math.PI));
                else
                    brush.RotateTransform(-(float)(180.0 * Math.Acos(cos_angle) / Math.PI));
                brush.TranslateTransform(-src_x1, -src_y1);
                pic.FillPolygon(brush, points.ToArray());
            }
        }
        /*
        public static Bitmap Generate(List<Face> patches, bool isFront, int paperSize) {

            int patchSize = patches.Count;

            if (isFront) {
                Bitmap bm = new Bitmap(paperSize, paperSize);
                Graphics front = System.Drawing.Graphics.FromImage(bm);
                for (int i = 0; i < patchSize; i++) {
                    Draw(front, patches[i].Points, patches[i].ColorFront, patches[i].MaterialFront, patches[i].ImageFront, patches[i].Uvfront);
                }
                front.DrawImage(bm, 0, 0, paperSize, paperSize);
                front.Dispose();
                return bm;
                // bm.Save(Application.dataPath + "/front.jpg");
                // front.Dispose();
            }
            else {
                Bitmap bm = new Bitmap(paperSize, paperSize);
                Graphics back = System.Drawing.Graphics.FromImage(bm);
                for (int i = 0; i < patchSize; i++)
                {
                    Debug.Log(patches[i].Uvback.Count);
                    Draw(back, patches[i].Points, patches[i].ColorBack, patches[i].MaterialBack, patches[i].ImageBack, patches[i].Uvback);
                }
                back.DrawImage(bm, 0, 0, paperSize, paperSize);
                back.Dispose();
                return bm;
                // bm.Save(Application.dataPath + "/back.jpg");
                // back.Dispose();
            }
        }*/


        public static Bitmap Generate(List<Face> patches, bool isFront, int paperSize)
        {
            int patchSize = patches.Count;
            System.Drawing.Color black = System.Drawing.Color.FromArgb(255, 0, 0, 0);
            System.Drawing.Pen pen = new System.Drawing.Pen(black);
            pen.Width = 2;
            if (isFront)
            {
                Bitmap bm = new Bitmap(paperSize, paperSize);
                Graphics front = System.Drawing.Graphics.FromImage(bm);
                System.Drawing.Color white = System.Drawing.Color.FromArgb(255, 255, 255, 255);
                front.FillRectangle(new SolidBrush(white), 0, 0, paperSize, paperSize);
                for (int i = 0; i < patchSize; i++) {

                    var tt = new List<Point>(patches[i].Points.ToArray()); //TODO 都不知道为什么要转一下,自己直接尝试出来的
                    tt.Reverse();
                    //patches[i].Points.CopyTo(tt);
                    Draw(front, tt, patches[i].ColorFront, patches[i].MaterialFront, patches[i].ImageFront, patches[i].Uvfront);
                }

                for (int i = 0; i < patchSize; i++)
                {
                    if (patches[i].MaterialFront != -2)
                        front.DrawPolygon(pen, patches[i].Points.ToArray());
                }
                front.DrawRectangle(pen, 0, 0, paperSize, paperSize);

                front.DrawImage(bm, 0, 0, paperSize, paperSize);
                front.Dispose();
                return bm;
                // bm.Save(Application.dataPath + "/front.jpg");
                // front.Dispose();
            }
            else {
                Bitmap bm = new Bitmap(paperSize, paperSize);
                Graphics back = System.Drawing.Graphics.FromImage(bm);
                System.Drawing.Color white = System.Drawing.Color.FromArgb(255, 255, 255, 255);
                back.FillRectangle(new SolidBrush(white), 0, 0, paperSize, paperSize);
                for (int i = 0; i < patchSize; i++)
                {
                    //Debug.Log(patches[i].Uvback.Count);
                    var tt = new List<Point>(patches[i].Points.ToArray()); //TODO 都不知道为什么要转一下,自己直接尝试出来的
                    tt.Reverse();
                    //var tt = new List<PointF>(patches[i].Uvback.ToArray()); //TODO 都不知道为什么要转一下
                    //tt.Reverse();

                    Draw(back, tt , patches[i].ColorBack, patches[i].MaterialBack, patches[i].ImageBack, patches[i].Uvback);
                }

                for (int i = 0; i < patchSize; i++)
                {
                    if (patches[i].MaterialBack != -2)
                        back.DrawPolygon(pen, patches[i].Points.ToArray());
                }
                back.DrawRectangle(pen, 0, 0, paperSize, paperSize);

                back.DrawImage(bm, 0, 0, paperSize, paperSize);
                back.Dispose();
                return bm;
                // bm.Save(Application.dataPath + "/back.jpg");
                // back.Dispose();
            }
        }





        static public void ImageGenerateFromOldFile(string filePath)
        {
            int pointsSize = 0;
            int patchSize = 0;
            int materialSize = 0;

            ArrayList patches = new ArrayList();
            ArrayList points = new ArrayList();
            ArrayList materials = new ArrayList();  //是文件数组

            Debug.Log(materials.Count);





            ReadFromOldFile(filePath, ref pointsSize, ref patchSize, ref materialSize, patches, points, materials);



            foreach (Face face in patches)
            {
                if (face.MaterialFront >= 0)
                    face.ImageFront = materials[face.MaterialFront] as string;
                if (face.MaterialBack >= 0)
                    face.ImageBack = materials[face.MaterialBack] as string;
            }

            Bitmap bmFront = Generate(patches.Cast<Face>().ToList(), true,
                512);
            bmFront.Save(Application.dataPath + "/Data/test/front.jpg");
            
            

            Bitmap bmBack = Generate(patches.Cast<Face>().ToList(), false,
               512);
            bmBack.Save(Application.dataPath + "/Data/test/back.jpg");
            
            // bmFront.Save(Application.dataPath + "data/test/back.jpg");

        }

        // Use this for initialization
        
        void Start()
        {
            //ImageGenerateFromOldFile(Application.dataPath + "/duizhe.zzs");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
