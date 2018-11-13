using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;

namespace Breakout
{
    public partial class Form1 : Form
    {
        Vector ballPos;
        Vector ballSpeed;
        int ballRadius;
        Rectangle paddlePos;
        List<Rectangle> blockPos;
        Timer timer = new Timer();

        public Form1()
        {
            InitializeComponent();

            // パドルの初期位置とサイズ
            this.paddlePos = new Rectangle(100, this.Height - 50, 100, 5);
            this.blockPos = new List<Rectangle>();
            // ブロックをリストに格納
            for(int x = 0; x <= this.Height; x += 100)
            {
                for(int y = 50; y <= 200; y += 40)
                {
                    this.blockPos.Add(new Rectangle(25 + x, y, 80, 25));
                }
            }
        }

        double DotProduct(Vector a, Vector b)
        {
            // 内積計算
            return a.X * b.X + a.Y * b.Y;
        }

        bool PaddleHitCircle(Vector p1, Vector p2, Vector center, float radius)
        {
            // パドルの方向ベクトル
            Vector paddleDir = (p2 - p1);
            // パドルの法線
            Vector n = new Vector(paddleDir.Y, -paddleDir.X);
            n.Normalize();

            Vector dir1 = center - p1;
            Vector dir2 = center - p2;

            double dist = Math.Abs(DotProduct(dir1, n));
            double a1 = DotProduct(dir1, paddleDir);
            double a2 = DotProduct(dir2, paddleDir);

            return (a1 * a2 < 0 && dist < radius) ? true : false;
        }

        int BlockHitCircle(Rectangle block, Vector ball)
        {
            if (PaddleHitCircle(new Vector(block.Left, block.Top),
                new Vector(block.Right, block.Top), ball, ballRadius))
            {
                return 1;
            }
            else if (PaddleHitCircle(new Vector(block.Left, block.Bottom),
               new Vector(block.Right, block.Bottom), ball, ballRadius))
            {
                return 2;
            }
            else if (PaddleHitCircle(new Vector(block.Right, block.Top),
               new Vector(block.Right, block.Bottom), ball, ballRadius))
            {
                return 3;
            }
            else if (PaddleHitCircle(new Vector(block.Left, block.Top),
                new Vector(block.Left, block.Bottom), ball, ballRadius))
            {
                return 4;
            }
            else
            {
                return -1;
            }
        }

        private void Update(object sender, EventArgs e)
        {
            // ボールの移動
            ballPos += ballSpeed;

            // 左右の壁でのバウンド
            if (ballPos.X + ballRadius > this.Bounds.Width - ballRadius || ballPos.X - ballRadius < 0)
            {
                ballSpeed.X *= -1;
            }

            // 上の壁でバウンド
            if (ballPos.Y - ballRadius < 50)
            {
                ballSpeed.Y *= -1;
            }

            // ボールが下に落ちたとき
            if (ballPos.Y == this.Height)
            {
                MessageBox.Show("げーむおーばー" + "\r\n" + "あなたのとくてんは：" + textBoxScore.Text);
                this.Dispose();
                Application.Restart();
            }

            // ブロックが無くなったとき
            if(blockPos.Count == 0)
            {
                timer.Stop();
                MessageBox.Show("げーむくりあ！" + "\r\n" + textBoxScore.Text);
                this.Dispose();
                Application.Restart();
            }

            // パドルのあたり判定
            if (PaddleHitCircle(new Vector(this.paddlePos.Left, this.paddlePos.Top),
                             new Vector(this.paddlePos.Right, this.paddlePos.Top),
                             ballPos, ballRadius))
            {
                ballSpeed.Y *= -1;
            }

            // ブロックとのあたり判定
            for(int i = 0; i < this.blockPos.Count; i++)
            {
                int collision = BlockHitCircle(blockPos[i], ballPos);
                if (collision == 1 || collision == 2)
                {
                    ballSpeed.Y *= -1;
                    this.blockPos.Remove(blockPos[i]);
                    textBoxScore.Text = (int.Parse(textBoxScore.Text) + 10).ToString() ;
                }
                else if (collision == 3 || collision == 4)
                {
                    ballSpeed.X *= -1;
                    this.blockPos.Remove(blockPos[i]);
                    textBoxScore.Text = (int.Parse(textBoxScore.Text) + 10).ToString();
                }

            }

            // 再描画
            Invalidate();
        }

        private void Draw(object sender, PaintEventArgs e)
        {
            SolidBrush pinkBrush = new SolidBrush(Color.HotPink);
            SolidBrush grayBrush = new SolidBrush(Color.DimGray);
            SolidBrush blueBrush = new SolidBrush(Color.LightBlue);


            float px = (float)this.ballPos.X - ballRadius;
            float py = (float)this.ballPos.Y - ballRadius;

            e.Graphics.FillEllipse(pinkBrush, px, py, this.ballRadius * 2, this.ballRadius * 2);
            e.Graphics.FillRectangle(grayBrush, paddlePos);

            for(int i = 0; i < blockPos.Count; i++)
            {
                e.Graphics.FillRectangle(blueBrush, blockPos[i]);
            }
        }

        private void PaddleMove(object sender, KeyPressEventArgs e)
        {
            // Aキーが押された時
            if (e.KeyChar == 'a')
            {
                this.paddlePos.X -= 20;
            }
            // Sキーが押された時
            if (e.KeyChar == 's')
            {
                this.paddlePos.X += 20;
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            // ボールの初期位置を設定
            this.ballPos = new Vector(200, 240);
            // ボールの半径を設定
            this.ballRadius = 10;

            // かんたんモード
            if (radioButtonEasy.Checked == true)
            {
                this.ballSpeed = new Vector(-2, -4);
            }
            // ふつうモード
            else if (radioButtonNormal.Checked == true)
            {
                this.ballSpeed = new Vector(-4, -6);
            }
            // むずかしいモード
            else if (radioButtonHard.Checked == true)
            {
                this.ballSpeed = new Vector(-6, -10);
            }
            // 30ミリ秒ごとに画面を再描画
            timer.Interval = 30;
            timer.Tick += new EventHandler(Update);
            timer.Start();
        }
    }
}
