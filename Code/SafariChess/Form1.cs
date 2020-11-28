using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//AI Logic 1202

namespace SafariChess
{
    public partial class Form1 : Form
    {
        int animasi = 0;
        int pilx1 = 0, pily1 = 0;
        int pilx2 = 0, pily2 = 0;
        int x1 = 0, y1 = 0;
        int x2 = 0, y2 = 0; 

        int dx = 0;
        int mode = 1; 

        string giliran = "Player 1";
        int counter = -1;
        Button[,] arrbtn = new Button[9, 7];
        Label[,] arrlabel = new Label[9, 7];
        Point[] clickedloc = new Point[2];
        Board current_board;

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = giliran + "'s Turn";

            //buat board baru
            Board b = new Board();

            //set pawn pawn musuh
            //row 1
            b.map[0, 0].hewan = new Animal("Lio", 7, "Player 2", SafariChess.Properties.Resources.Dou_Shou_Qi_Lion_194528);
            b.map[0, 6].hewan = new Animal("Tig", 6, "Player 2", SafariChess.Properties.Resources.Dou_Shou_Qi_Tiger_194529);

            //row 2
            b.map[1, 1].hewan = new Animal("Dog", 3, "Player 2", SafariChess.Properties.Resources.Dou_Shou_Qi_Dog_194532);
            b.map[1, 5].hewan = new Animal("Cat", 2, "Player 2", SafariChess.Properties.Resources.Dou_Shou_Qi_Cat_194533);

            //row 3
            b.map[2, 0].hewan = new Animal("Mou", 1, "Player 2", SafariChess.Properties.Resources.Dou_Shou_Qi_Rat_194534);
            b.map[2, 2].hewan = new Animal("Jag", 5, "Player 2", SafariChess.Properties.Resources.Dou_Shou_Qi_Leopard_194530);
            b.map[2, 4].hewan = new Animal("Wol", 4, "Player 2", SafariChess.Properties.Resources.Dou_Shou_Qi_Wolf_194531);
            b.map[2, 6].hewan = new Animal("Ele", 8, "Player 2", SafariChess.Properties.Resources.Dou_Shou_Qi_Elephant_194527);

            //set petak air, trap, goal - dipindah di class #Mitchell

            //set pawn pawn player
            //row 6
            b.map[6, 0].hewan = new Animal("Ele", 8, "Player 1", SafariChess.Properties.Resources.Dou_Shou_Qi_Elephant_194527);
            b.map[6, 2].hewan = new Animal("Wol", 4, "Player 1", SafariChess.Properties.Resources.Dou_Shou_Qi_Wolf_194531);
            b.map[6, 4].hewan = new Animal("Jag", 5, "Player 1", SafariChess.Properties.Resources.Dou_Shou_Qi_Leopard_194530);
            b.map[6, 6].hewan = new Animal("Mou", 1, "Player 1", SafariChess.Properties.Resources.Dou_Shou_Qi_Rat_194534);

            //row 7
            b.map[7, 1].hewan = new Animal("Cat", 2, "Player 1", SafariChess.Properties.Resources.Dou_Shou_Qi_Cat_194533);
            b.map[7, 5].hewan = new Animal("Dog", 3, "Player 1", SafariChess.Properties.Resources.Dou_Shou_Qi_Dog_194532);

            //row 8
            b.map[8, 0].hewan = new Animal("Tig", 6, "Player 1", SafariChess.Properties.Resources.Dou_Shou_Qi_Tiger_194529);
            b.map[8, 6].hewan = new Animal("Lio", 7, "Player 1", SafariChess.Properties.Resources.Dou_Shou_Qi_Lion_194528);

            current_board = b;

            //untuk draw
            clickedloc[0] = new Point(-1, -1);
            clickedloc[1] = new Point(-1, -1);
        }
        private Board copyBoard(Board b)
        {
            Board copyB = new Board();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    copyB.map[i, j].isGoal = b.map[i, j].isGoal;
                    copyB.map[i, j].isLand = b.map[i, j].isLand;
                    copyB.map[i, j].isTrap = b.map[i, j].isTrap;
                    if(b.map[i,j].hewan != null)
                    {
                        copyB.map[i, j].hewan = b.map[i, j].hewan;
                    }
                }
            }

            return copyB;
        }

        //MINMAX
        private double minmax(Board b, int ply, bool isMaximizing, int depthNow, double alpha, double beta)
        {
            double score = evaluation(b);
            if(score == 1000 || score == -1000 || depthNow == ply)
            {
                return score;
            }
            else
            {
                if (isMaximizing)
                {
                    double best = -100000;

                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 7; j++)
                        {
                            if(b.map[i,j].hewan != null)
                            {
                                if(b.map[i,j].hewan.belongsTo == "Player 2")
                                {
                                    int desx = -1, desy = -1;

                                    if(AI_CheckDown(b.map[i,j].hewan,b,j,i,ref desx,ref desy,"Player 2"))
                                    {
                                        Board newState = copyBoard(b);
                                        newState.map[desy, desx].hewan = newState.map[i, j].hewan;
                                        newState.map[i, j].hewan = null;
                                        double value = minmax(newState, ply, !isMaximizing, depthNow + 1, alpha, beta);
                                        best = Math.Max(best, value);
                                        alpha = Math.Max(alpha, best);
                                        if (beta <= alpha)
                                            break;
                                    }
                                    if(AI_CheckUp(b.map[i,j].hewan,b,j,i,ref desx, ref desy,"Player 2"))
                                    {
                                        Board newState = copyBoard(b); ;
                                        newState.map[desy, desx].hewan = newState.map[i, j].hewan;
                                        newState.map[i, j].hewan = null;
                                        double value = minmax(newState, ply, !isMaximizing, depthNow + 1, alpha, beta);
                                        best = Math.Max(best, value);
                                        alpha = Math.Max(alpha, best);
                                        if (beta <= alpha)
                                            break;
                                    }
                                    if (AI_CheckLeft(b.map[i, j].hewan, b, j, i, ref desx, ref desy, "Player 2"))
                                    {
                                        Board newState = copyBoard(b); ;
                                        newState.map[desy, desx].hewan = newState.map[i, j].hewan;
                                        newState.map[i, j].hewan = null;
                                        double value = minmax(newState, ply, !isMaximizing, depthNow + 1, alpha, beta);
                                        best = Math.Max(best, value);
                                        alpha = Math.Max(alpha, best);
                                        if (beta <= alpha)
                                            break;
                                    }
                                    if (AI_CheckRight(b.map[i, j].hewan, b, j, i, ref desx, ref desy, "Player 2"))
                                    {
                                        Board newState = copyBoard(b); ;
                                        newState.map[desy, desx].hewan = newState.map[i, j].hewan;
                                        newState.map[i, j].hewan = null;
                                        double value = minmax(newState, ply, !isMaximizing, depthNow + 1, alpha, beta);
                                        best = Math.Max(best, value);
                                        alpha = Math.Max(alpha, best);
                                        if (beta <= alpha)
                                            break;
                                    }
                                }
                            }
                        }
                    }

                    return best;
                }
                else
                {
                    double best = 100000;

                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 7; j++)
                        {
                            if (b.map[i, j].hewan != null)
                            {
                                if (b.map[i, j].hewan.belongsTo == "Player 1")
                                {
                                    int desx = -1, desy = -1;

                                    if (AI_CheckDown(b.map[i, j].hewan, b, j, i, ref desx, ref desy, "Player 1"))
                                    {
                                        Board newState = copyBoard(b);
                                        newState.map[desy, desx].hewan = newState.map[i, j].hewan;
                                        newState.map[i, j].hewan = null;
                                        double value = minmax(newState, ply, !isMaximizing, depthNow + 1, alpha, beta);
                                        best = Math.Min(best, value);
                                        beta = Math.Min(beta, best);
                                        if (beta <= alpha)
                                            break;
                                    }
                                    if (AI_CheckUp(b.map[i, j].hewan, b, j, i, ref desx, ref desy, "Player 1"))
                                    {
                                        Board newState = copyBoard(b);
                                        newState.map[desy, desx].hewan = newState.map[i, j].hewan;
                                        newState.map[i, j].hewan = null;
                                        double value = minmax(newState, ply, !isMaximizing, depthNow + 1, alpha, beta);
                                        best = Math.Min(best, value);
                                        beta = Math.Min(beta, best);
                                        if (beta <= alpha)
                                            break;
                                    }
                                    if (AI_CheckLeft(b.map[i, j].hewan, b, j, i, ref desx, ref desy, "Player 1"))
                                    {
                                        Board newState = copyBoard(b);
                                        newState.map[desy, desx].hewan = newState.map[i, j].hewan;
                                        newState.map[i, j].hewan = null;
                                        double value = minmax(newState, ply, !isMaximizing, depthNow + 1, alpha, beta);
                                        best = Math.Min(best, value);
                                        beta = Math.Min(beta, best);
                                        if (beta <= alpha)
                                            break;
                                    }
                                    if (AI_CheckRight(b.map[i, j].hewan, b, j, i, ref desx, ref desy, "Player 1"))
                                    {
                                        Board newState = copyBoard(b);
                                        newState.map[desy, desx].hewan = newState.map[i, j].hewan;
                                        newState.map[i, j].hewan = null;
                                        double value = minmax(newState, ply, !isMaximizing, depthNow + 1, alpha, beta);
                                        best = Math.Min(best, value);
                                        beta = Math.Min(beta, best);
                                        if (beta <= alpha)
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    return best;
                }
            }
        }

        private double findBestMove(Board b,ref int x,ref int y, ref int desx, ref int desy, int ply)
        {
            double bestVal = -100000;

            //All Possible Move
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if(b.map[i,j].hewan != null)
                    {
                        if(b.map[i,j].hewan.belongsTo == "Player 2")
                        {
                            //MessageBox.Show(i+"-"+j);
                            int goX = -1, goY = -1;
                            if(AI_CheckDown(b.map[i,j].hewan,b,j,i,ref goX, ref goY, "Player 2"))
                            {
                                // sudah berhasil clone
                                Board tempState = copyBoard(b);
                                tempState.map[goY, goX].hewan = tempState.map[i, j].hewan;
                                tempState.map[i, j].hewan = null;
                                double moveVal = minmax(tempState, ply, false, 0, -100000, 100000);
                                if(moveVal > bestVal)
                                {
                                    x = j;
                                    y = i;
                                    desx = goX;
                                    desy = goY;
                                    bestVal = moveVal;
                                }
                            }
                            if (AI_CheckUp(b.map[i, j].hewan, b, j, i, ref goX, ref goY, "Player 2"))
                            {
                                Board tempState = copyBoard(b);
                                tempState.map[goY, goX].hewan = tempState.map[i, j].hewan;
                                tempState.map[i, j].hewan = null;
                                double moveVal = minmax(tempState, ply, false, 0, -1000000, 1000000);

                                if (moveVal > bestVal)
                                {
                                    x = j;
                                    y = i;
                                    desx = goX;
                                    desy = goY;
                                    bestVal = moveVal;
                                }
                            }
                            if (AI_CheckLeft(b.map[i, j].hewan, b, j, i, ref goX, ref goY, "Player 2"))
                            {
                                Board tempState = copyBoard(b);
                                tempState.map[goY, goX].hewan = tempState.map[i, j].hewan;
                                tempState.map[i, j].hewan = null;
                                double moveVal = minmax(tempState, ply, false, 0, -1000000, 1000000);

                                if (moveVal > bestVal)
                                {
                                    x = j;
                                    y = i;
                                    desx = goX;
                                    desy = goY;
                                    bestVal = moveVal;
                                }
                            }
                            if (AI_CheckRight(b.map[i, j].hewan, b, j, i, ref goX, ref goY, "Player 2"))
                            {
                                Board tempState = copyBoard(b);
                                tempState.map[goY, goX].hewan = tempState.map[i, j].hewan;
                                tempState.map[i, j].hewan = null;
                                double moveVal = minmax(tempState, ply, false, 0, -1000000, 1000000);

                                if (moveVal > bestVal)
                                {
                                    x = j;
                                    y = i;
                                    desx = goX;
                                    desy = goY;
                                    bestVal = moveVal;
                                }
                            }
                        }
                    }
                }
            }
            MessageBox.Show(bestVal + "");
            return bestVal;
        }
        public void AIMove(String difficult)
        {
            int ply = 0;
            if(difficult == "easy")
            {
                ply = 4;
            }else if(difficult == "medium")
            {
                ply = 5;
            }
            else if(difficult == "hard")
            {
                ply = 6;
            }
            
            int MoveX = -1;
            int MoveY = -1;
            int MoveDesX = -1;
            int moveDesY = -1;

            double bestMove = findBestMove(current_board,ref MoveX,ref MoveY,ref MoveDesX,ref moveDesY, ply);
            MessageBox.Show("Best Move : ["+MoveX+"-"+MoveY+"] To ["+MoveDesX+"-"+moveDesY+"] with score "+bestMove);
            //Gerak Function
            pilx1 = MoveX;
            pily1 = MoveY;
            pilx2 = MoveDesX;
            pily2 = moveDesY;
            x1 = (MoveX+1)*44;
            y1 = (MoveY+1)*44;
            x2 = (MoveDesX+1)*44;
            y2 = (moveDesY+1)*44;
            animasi = 1;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            bool up = false;
            bool down = false;
            bool left = false;
            bool right = false;
            bool across_up = false;
            bool across_down = false;
            bool across_left = false;
            bool across_right = false;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    int x = (j + 1) * 44;
                    int y = (i + 1) * 44;
                    if (current_board.map[i, j].isLand)
                    {
                        if ((i + j) % 2 == 1) { e.Graphics.FillRectangle(Brushes.Green, x, y, 43, 43); }
                        else { e.Graphics.FillRectangle(Brushes.GreenYellow, x, y, 43, 43); }
                    }
                    else if (!current_board.map[i, j].isLand)
                    { e.Graphics.FillRectangle(Brushes.LightBlue, x, y, 43, 43); }

                    if (current_board.map[i, j].isTrap)
                    {
                        e.Graphics.FillEllipse(Brushes.DarkGoldenrod, x, y, 42, 42);
                        e.Graphics.FillEllipse(Brushes.GreenYellow, x + 5, y + 5, 32, 32);
                    }

                    if (current_board.map[i, j].isGoal)
                    {
                        e.Graphics.FillRectangle(Brushes.Red, x, y, 43, 43);
                        e.Graphics.DrawImage(Image.FromFile("star.png"), x + 5, y + 5, 34, 32);
                    }
                }
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    int x = (j + 1) * 44;
                    int y = (i + 1) * 44;
                    if (current_board.map[i, j].hewan != null)
                    {
                        //nanti ganti image disini
                        //abc
                        if(current_board.map[i, j].hewan.belongsTo == giliran)
                        {
                            int gerakanimasi = 0; 
                            if(i == pily1 && j == pilx1)
                            {
                                if (animasi == 1)
                                {
                                    gerakanimasi = 1; 
                                }
                            }

                            if(gerakanimasi == 0)
                            {
                                e.Graphics.DrawImage(current_board.map[i, j].hewan.img, x - dx, y - dx, 43 + 2 * dx, 43 + 2 * dx);
                            }
                            else
                            {
                                e.Graphics.DrawImage(current_board.map[i, j].hewan.img, x1 - dx, y1 - dx, 43 + 2 * dx, 43 + 2 * dx);
                            }
                        }
                        else
                        {
                            e.Graphics.DrawImage(current_board.map[i, j].hewan.img, x, y, 43, 43);
                        }
                    }
                    
                    //hint
                    if (clickedloc[0] != new Point(-1, -1))
                    {
                        if (clickedloc[0].X == j && clickedloc[0].Y == i)
                        {
                            Animal animal = current_board.map[i, j].hewan;
                            //up
                            if (i > 0)
                            {
                                Tile upside = current_board.map[i - 1, j];
                                if (upside.isLand)
                                {
                                    if (upside.hewan == null)
                                    {
                                        up = true;
                                    }
                                    else if (animal.belongsTo != upside.hewan.belongsTo)
                                    {
                                        if (animal.grade == 1 && upside.hewan.grade == 8) // tikus makan gajah
                                        {
                                            up = true;
                                        }
                                        else if (animal.grade >= upside.hewan.grade && (animal.grade != 8 && upside.hewan.grade != 1)) // gajah kalah dgn tikus
                                        {
                                            up = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (animal.grade == 1) // if mouse
                                    {
                                        if (upside.hewan == null)
                                        {
                                            up = true;
                                        }
                                    }
                                    else if (animal.grade == 6 || animal.grade == 7) //if tiger or lion
                                    {
                                        Tile across = current_board.map[i - 4, j];
                                        if (current_board.map[3, j].hewan == null && current_board.map[4, j].hewan == null && current_board.map[5, j].hewan == null)
                                        {
                                            if (across.hewan == null)
                                            {
                                                across_up = true;
                                            }
                                            else if (animal.grade >= across.hewan.grade && animal.belongsTo != upside.hewan.belongsTo)
                                            {
                                                across_up = true;
                                            }
                                        }
                                    }
                                }
                            }

                            //down
                            if (i < 8) 
                            {
                                Tile downside = current_board.map[i + 1, j];
                                if (downside.isLand)
                                {
                                    if (downside.hewan == null)
                                    {
                                        down = true;
                                    }
                                    else if (animal.belongsTo != downside.hewan.belongsTo)
                                    {
                                        if (animal.grade == 1 && downside.hewan.grade == 8) // tikus makan gajah
                                        {
                                            down = true;
                                        }
                                        else if (animal.grade >= downside.hewan.grade && (animal.grade != 8 && downside.hewan.grade != 1)) // gajah kalah dgn tikus
                                        {
                                            down = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (animal.grade == 1) // if mouse
                                    {
                                        if (downside.hewan == null)
                                        {
                                            down = true;
                                        }
                                    }
                                    else if (animal.grade == 6 || animal.grade == 7) //if tiger or lion
                                    {
                                        Tile across = current_board.map[i + 4, j];
                                        if (current_board.map[3, j].hewan == null && current_board.map[4, j].hewan == null && current_board.map[5, j].hewan == null)
                                        {
                                            if (across.hewan == null)
                                            {
                                                across_down = true;
                                            }
                                            else if (animal.grade >= across.hewan.grade && animal.belongsTo != across.hewan.belongsTo)
                                            {
                                                across_down = true;
                                            }
                                        }
                                    }
                                }
                            }

                            //left
                            if (j > 0) 
                            {
                                Tile leftside = current_board.map[i, j - 1];
                                if (leftside.isLand)
                                {
                                    if (leftside.hewan == null)
                                    {
                                        left = true;
                                    }
                                    else if (animal.belongsTo != leftside.hewan.belongsTo)
                                    {
                                        if (animal.grade == 1 && leftside.hewan.grade == 8) // tikus makan gajah
                                        {
                                            left = true;
                                        }
                                        else if (animal.grade >= leftside.hewan.grade && (animal.grade != 8 && leftside.hewan.grade != 1)) // gajah kalah dgn tikus
                                        {
                                            left = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (animal.grade == 1) // if mouse
                                    {
                                        if (leftside.hewan == null)
                                        {
                                            left = true;
                                        }
                                    }
                                    else if (animal.grade == 6 || animal.grade == 7) //if tiger or lion
                                    {
                                        Tile across = current_board.map[i, j - 3];
                                        if (current_board.map[i, j - 2].hewan == null && leftside.hewan == null )
                                        {
                                            if (across.hewan == null)
                                            {
                                                across_left = true;
                                            }
                                            else if (animal.grade >= across.hewan.grade)
                                            {
                                                across_left = true;
                                            }
                                        }
                                    }
                                }
                            }

                            //right
                            if (j < 6)
                            {
                                Tile rightside = current_board.map[i, j + 1];
                                if (rightside.isLand)
                                {
                                    if (rightside.hewan == null)
                                    {
                                        right = true;
                                    }
                                    else if (animal.belongsTo != rightside.hewan.belongsTo)
                                    {
                                        if (animal.grade == 1 && rightside.hewan.grade == 8) // tikus makan gajah
                                        {
                                            right = true;
                                        }
                                        else if (animal.grade >= rightside.hewan.grade && (animal.grade != 8 && rightside.hewan.grade != 1)) // gajah kalah dgn tikus
                                        {
                                            right = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (animal.grade == 1) // if mouse
                                    {
                                        if (rightside.hewan == null)
                                        {
                                            right = true;
                                        }
                                    }
                                    else if (animal.grade == 6 || animal.grade == 7) //if tiger or lion
                                    {
                                        Tile across = current_board.map[i, j + 3];
                                        if (current_board.map[i, j + 2].hewan == null && rightside.hewan == null)
                                        {
                                            if (across.hewan == null)
                                            {
                                                across_right = true;
                                            }
                                            else if (animal.grade >= across.hewan.grade)
                                            {
                                                across_right = true;
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }

            if (across_up)
            {
                e.Graphics.FillEllipse(Brushes.White, (clickedloc[0].X + 1) * 44 + 16, (clickedloc[0].Y - 3) * 44 + 16, 10, 10);
            }
            else if (up)
            {
                e.Graphics.FillEllipse(Brushes.White, (clickedloc[0].X + 1) * 44 + 16, (clickedloc[0].Y) * 44 + 16, 10, 10);
            }

            if (across_down)
            {
                e.Graphics.FillEllipse(Brushes.White, (clickedloc[0].X + 1) * 44 + 16, (clickedloc[0].Y + 5) * 44 + 16, 10, 10);
            }
            else if (down)
            {
                e.Graphics.FillEllipse(Brushes.White, (clickedloc[0].X + 1) * 44 + 16, (clickedloc[0].Y + 2) * 44 + 16, 10, 10);
            }


            if (across_left)
            {
                e.Graphics.FillEllipse(Brushes.White, (clickedloc[0].X - 2) * 44 + 16, (clickedloc[0].Y + 1) * 44 + 16, 10, 10);
            }
            else if (left)
            {
                e.Graphics.FillEllipse(Brushes.White, (clickedloc[0].X) * 44 + 16, (clickedloc[0].Y + 1) * 44 + 16, 10, 10);
            }

            if (across_right)
            {
                e.Graphics.FillEllipse(Brushes.White, (clickedloc[0].X + 4) * 44 + 16, (clickedloc[0].Y + 1) * 44 + 16, 10, 10);
            }
            else if (right)
            {
                e.Graphics.FillEllipse(Brushes.White, (clickedloc[0].X + 2) * 44 + 16, (clickedloc[0].Y + 1) * 44 + 16, 10, 10);
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    Rectangle rectangle = new Rectangle((j + 1) * 44, (i + 1) * 44, 44, 44);
                    if (counter == -1 && rectangle.Contains(e.Location) && current_board.map[i, j].hewan != null && 
                        current_board.map[i, j].hewan.belongsTo == giliran)
                    {
                        counter += 1;
                        clickedloc[counter] = new Point((rectangle.Location.X / 44) - 1, (rectangle.Location.Y / 44) - 1);
                        pily1 = clickedloc[counter].Y;
                        pilx1 = clickedloc[counter].X;
                        x1 = (pilx1 + 1) * 44;
                        y1 = (pily1 + 1) * 44;
                        //MessageBox.Show("posisi awal = "+ clickedloc[counter].ToString());
                        break;
                    }
                    else if (counter == 0 && rectangle.Contains(e.Location))
                    {
                        Point temp = new Point((rectangle.Location.X / 44) - 1, (rectangle.Location.Y / 44) - 1);
                        pily2 = temp.Y;
                        pilx2 = temp.X;

                        if(pily2 == pily1 && pilx2 == pilx1)
                        {
                            counter = -1;
                            clickedloc = new Point[2];
                            clickedloc[0] = new Point(-1, -1);
                            clickedloc[1] = new Point(-1, -1);
                        }
                        else if(movePawnEditan(pilx1, pily1, pilx2, pily2, giliran)) {
                            counter += 1;
                            clickedloc[counter] = new Point((rectangle.Location.X / 44) - 1, (rectangle.Location.Y / 44) - 1);
                            animasi = 1;
                            pily2 = clickedloc[counter].Y;
                            pilx2 = clickedloc[counter].X;
                            x2 = (pilx2 + 1) * 44;
                            y2 = (pily2 + 1) * 44;
                            //MessageBox.Show(clickedloc[counter].ToString());
                            break;
                        }
                    }
                }
            }
        }

        public void gantiGiliran()
        {
            if (giliran == "Player")
            {
                giliran = "AI";
                //AIMove();
            }
            else if (giliran == "AI") 
            {
                giliran = "Player";
            }
            else if (giliran == "Player 1")
            {
                giliran = "Player 2";
                AIMove("easy");
            }
            else if (giliran == "Player 2")
            {
                giliran = "Player 1";
            }
        }

        private bool plusmove(int xawal, int yawal, int xtuju, int ytuju)
        {
            if ((xtuju == xawal - 1 && ytuju == yawal) || (xtuju == xawal && ytuju == yawal - 1) || (xtuju == xawal + 1 && ytuju == yawal) || (xtuju == xawal && ytuju == yawal + 1))
            {
                return true;
            }
            return false;
        }

        //Function KEVIN
        public bool movePawnEditan(int xawal, int yawal, int xtuju, int ytuju, string movingPlayer)
        {
            bool boleh = false; 

            //pergerakan antar petak
            if (current_board.map[yawal, xawal].hewan != null && 
                current_board.map[yawal, xawal].hewan.belongsTo == movingPlayer)
            {
                if (current_board.map[ytuju, xtuju].hewan != null)
                {
                    //sama sama di darat, baru boleh saling memakan
                    if (current_board.map[yawal, xawal].isLand && current_board.map[ytuju, xtuju].isLand)
                    {
                        //makan pion lain
                        if (current_board.map[yawal, xawal].hewan.grade == 1 && plusmove(xawal, yawal, xtuju, ytuju))
                        {
                            if (!(current_board.map[ytuju, xtuju].hewan.grade == 8 && current_board.map[ytuju, xtuju].hewan.belongsTo != movingPlayer))
                            {
                                return false;
                            }
                        }
                        else if (current_board.map[yawal, xawal].hewan.grade == 7 || current_board.map[yawal, xawal].hewan.grade == 6)
                        {
                            //----PERGERAKAN TIGER & LION-------
                            //lion dan tiger dapat memakan dengan melompati air
                            //hitung jarak lompatan
                            int jaraky = ytuju - yawal;
                            int jarakx = xtuju - xawal;
                            if (Math.Abs(jarakx) > 1)
                            {
                                //jika jarak lbh dr 1, hanya valid ketika melewati air
                                bool allwater = true;
                                if (jarakx < -1)
                                {
                                    //jarak x lbh kecil dr -1, berarti geser ke kiri
                                    for (int i = -1; i > jarakx; i--)
                                    {
                                        if (current_board.map[yawal, xawal + i].isLand || current_board.map[yawal, xawal + i].hewan != null)
                                        {
                                            //jika ada 1 saja yg bersifat land, maka tdk valid
                                            allwater = false;
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = 1; i < jarakx; i++)
                                    {
                                        //jarak x lbh besar dr 1, berarti geser ke kanan
                                        if (current_board.map[yawal, xawal + i].isLand || current_board.map[yawal, xawal + i].hewan != null)
                                        {
                                            //jika ada 1 saja yg bersifat land, maka tdk valid
                                            allwater = false;
                                        }
                                    }
                                }
                                if (!allwater)
                                {
                                    //ada yg tidak air, tidak valid move nya
                                    return false;
                                }
                            }
                            else if (Math.Abs(jaraky) > 1)
                            {
                                //sama dengan jarakx, hanya ini membandingkan y
                                bool allwater = true;
                                if (jaraky < -1)
                                {
                                    for (int i = -1; i > jaraky; i--)
                                    {
                                        if (current_board.map[yawal + i, xawal].isLand || current_board.map[yawal + i, xawal].hewan != null)
                                        {
                                            allwater = false;
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = 1; i < jaraky; i++)
                                    {
                                        if (current_board.map[yawal + i, xawal].isLand || current_board.map[yawal + i, xawal].hewan != null)
                                        {
                                            allwater = false;
                                        }
                                    }
                                }
                                if (!allwater)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                //jika jarak x maupun y tidak lbh dr 1, maka hanya bisa move 4 arah mata angin saja
                                if (!plusmove(xawal, yawal, xtuju, ytuju))
                                {
                                    return false;
                                }
                            }
                            if (!(current_board.map[ytuju, xtuju].hewan.grade < current_board.map[yawal, xawal].hewan.grade && current_board.map[ytuju, xtuju].hewan.belongsTo != movingPlayer))
                            {
                                //pion msuh lbh kuat, tdk valid
                                return false;
                            }
                        }
                        else if (current_board.map[yawal, xawal].hewan.grade == 8 && plusmove(xawal, yawal, xtuju, ytuju))
                        {
                            if (!(current_board.map[ytuju, xtuju].hewan.grade != 1 && current_board.map[ytuju, xtuju].hewan.belongsTo != movingPlayer))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (!(current_board.map[yawal, xawal].hewan.grade > current_board.map[ytuju, xtuju].hewan.grade && current_board.map[ytuju, xtuju].hewan.belongsTo != movingPlayer && plusmove(xawal, yawal, xtuju, ytuju)))
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    //pindah tile saja
                    if (current_board.map[ytuju, xtuju].isGoal && plusmove(xawal, yawal, xtuju, ytuju))
                    {
                        //masuk goal
                        if (movingPlayer == "Player 2" && ytuju == 0)
                        {
                            return false;
                        }
                        else if (movingPlayer == "Player 2" && ytuju == 8)
                        {
                            //menang player 2(AI)
                            MessageBox.Show("Player 2 Win!");
                        }

                        if (movingPlayer == "Player 1" && ytuju == 8)
                        {
                            return false;
                        }
                        else if (movingPlayer == "Player 1" && ytuju == 0)
                        {
                            //menang player
                            MessageBox.Show("Player 1 Win!");
                        }
                    }
                    else if (current_board.map[ytuju, xtuju].isTrap && plusmove(xawal, yawal, xtuju, ytuju))
                    {
                        //masuk trap
                        //current_board.map[ytuju, xtuju].hewan = current_board.map[yawal, xawal].hewan;
                        //current_board.map[ytuju, xtuju].hewan.grade = 0;
                        //current_board.map[yawal, xawal].hewan = null;
                    }
                    else
                    {
                        if (!current_board.map[ytuju, xtuju].isLand && plusmove(xawal, yawal, xtuju, ytuju))
                        {
                            if (current_board.map[yawal, xawal].hewan.grade != 1)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            //sama dengan PERGERAKAN TIGER & LION diatas, hanya saja tidak membamdingkan kekuatan pion, dikarenakan tidak memakan pion lain, hanya perpindah tempat
                            if (current_board.map[yawal, xawal].hewan.grade == 7 || current_board.map[yawal, xawal].hewan.grade == 6)
                            {
                                int jaraky = ytuju - yawal;
                                int jarakx = xtuju - xawal;
                                if (Math.Abs(jarakx) > 1)
                                {
                                    bool allwater = true;
                                    if (jarakx < -1)
                                    {
                                        for (int i = -1; i > jarakx; i--)
                                        {
                                            if (current_board.map[yawal, xawal + i].isLand || current_board.map[yawal, xawal + i].hewan != null)
                                            {
                                                allwater = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 1; i < jarakx; i++)
                                        {
                                            if (current_board.map[yawal, xawal + i].isLand || current_board.map[yawal, xawal + i].hewan != null)
                                            {
                                                allwater = false;
                                            }
                                        }
                                    }
                                    if (!allwater)
                                    {
                                        return false;
                                    }
                                }
                                else if (Math.Abs(jaraky) > 1)
                                {
                                    bool allwater = true;
                                    if (jaraky < -1)
                                    {
                                        for (int i = -1; i > jaraky; i--)
                                        {
                                            if (current_board.map[yawal + i, xawal].isLand || current_board.map[yawal + i, xawal].hewan != null)
                                            {
                                                allwater = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 1; i < jaraky; i++)
                                        {
                                            if (current_board.map[yawal + i, xawal].isLand || current_board.map[yawal + i, xawal].hewan != null)
                                            {
                                                allwater = false;
                                            }
                                        }
                                    }
                                    if (!allwater)
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (!plusmove(xawal, yawal, xtuju, ytuju))
                                    {
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                if (!plusmove(xawal, yawal, xtuju, ytuju))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            //gagal move pawn, giliran masih di player sebelumnya
            return false;
        }

        //FUNCTION FUNCTION AI - MING
        //PENGECEKAN POSSIBLE MOVE
        private bool AI_CheckUp(Animal param,Board b, int x, int y,ref int desx,ref int desy, String player)
        {
            if(y - 1 < 0)
            {
                return false;
            }
            else if(b.map[y-1, x].hewan == null)
            {
                if (b.map[y - 1, x].isLand)
                {
                    //jika land
                    desx = x;
                    desy = y - 1;
                    return true;
                }
                else
                {
                    if(param.grade == 1)
                    {
                        //jika air tapi tikus
                        desx = x;
                        desy = y - 1;
                        return true;
                    }else if(param.grade == 6 || param.grade == 7)
                    {
                        //jika air tapi singa/harimau
                        if(y-4  >= 0)
                        {
                            if (b.map[y - 4, x].hewan == null)
                            {
                                desx = x;
                                desy = y - 4;
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if(b.map[y - 1,x].hewan.belongsTo == player)
                {
                    return false;
                }else if(b.map[y - 1, x].isLand)
                {
                    if (param.grade == 1)
                    {
                        if (b.map[y - 1, x].hewan.grade == 8)
                        {
                            desx = x;
                            desy = y - 1;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (param.grade == 8 && b.map[y - 1, x].hewan.grade == 1)
                    {
                        return false;
                    }
                    else
                    {
                        if (param.grade > b.map[y - 1, x].hewan.grade)
                        {
                            desx = x;
                            desy = y - 1;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        private bool AI_CheckDown(Animal param, Board b, int x, int y, ref int desx, ref int desy, String player)
        {
            if (y + 1 > 8)
            {
                return false;
            }
            else if (b.map[y + 1, x].hewan == null)
            {
                if (b.map[y + 1, x].isLand)
                {
                    //jika land
                    desx = x;
                    desy = y + 1;
                    return true;
                }
                else
                {
                    if (param.grade == 1)
                    {
                        //jika air tapi tikus
                        desx = x;
                        desy = y + 1;
                        return true;
                    }
                    else if (param.grade == 6 || param.grade == 7)
                    {
                        if(y+4 < 9)
                        {
                            if (b.map[y + 4, x].hewan == null)
                            {
                                //jika air tapi singa/harimau
                                desx = x;
                                desy = y + 4;
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (b.map[y + 1, x].hewan.belongsTo == player)
                {
                    return false;
                }else if(b.map[y + 1, x].isLand)
                {
                    if (param.grade == 1)
                    {
                        if (b.map[y + 1, x].hewan.grade == 8)
                        {
                            desx = x;
                            desy = y + 1;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (param.grade == 8 && b.map[y + 1, x].hewan.grade == 1)
                    {
                        return false;
                    }
                    else
                    {
                        if (param.grade > b.map[y + 1, x].hewan.grade)
                        {
                            desx = x;
                            desy = y + 1;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
                
            }
        }
        private bool AI_CheckLeft(Animal param, Board b, int x, int y, ref int desx, ref int desy, String player)
        {
            if (x - 1 < 0)
            {
                return false;
            }
            else if (b.map[y, x-1].hewan == null)
            {
                if (b.map[y, x - 1].isLand)
                {
                    //jika land
                    desx = x - 1;
                    desy = y;
                    return true;
                }
                else
                {
                    if (param.grade == 1)
                    {
                        //jika air tapi tikus
                        desx = x - 1;
                        desy = y;
                        return true;
                    }
                    else if (param.grade == 6 || param.grade == 7)
                    {
                        if(x-3 >= 0)
                        {
                            if (b.map[y, x - 3].hewan == null)
                            {
                                //jika air tapi singa/harimau
                                desx = x - 3;
                                desy = y;
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }                                               
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (b.map[y, x - 1].hewan.belongsTo == player)
                {
                    return false;
                }else if(b.map[y,x - 1].isLand)
                {
                    if (param.grade == 1)
                    {
                        if (b.map[y, x - 1].hewan.grade == 8)
                        {
                            desx = x - 1;
                            desy = y;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (param.grade == 8 && b.map[y, x - 1].hewan.grade == 1)
                    {
                        return false;
                    }
                    else
                    {
                        if (param.grade > b.map[y, x - 1].hewan.grade)
                        {
                            desx = x - 1;
                            desy = y;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        private bool AI_CheckRight(Animal param, Board b, int x, int y, ref int desx, ref int desy, String player)
        {
            if (x + 1 > 6)
            {
                return false;
            }
            else if (b.map[y, x + 1].hewan == null)
            {
                if (b.map[y, x + 1].isLand)
                {
                    //jika land
                    desx = x + 1;
                    desy = y;
                    return true;
                }
                else
                {
                    if (param.grade == 1)
                    {
                        //jika air tapi tikus
                        desx = x + 1;
                        desy = y;
                        return true;
                    }
                    else if (param.grade == 6 || param.grade == 7)
                    {
                        if(x+3 < 7)
                        {
                            if (b.map[y, x + 3].hewan == null)
                            {
                                //jika air tapi singa/harimau
                                desx = x + 3;
                                desy = y;
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (b.map[y, x + 1].hewan.belongsTo == player)
                {
                    return false;
                }else if(b.map[y, x + 1].isLand)
                {
                    if (param.grade == 1)
                    {
                        if (b.map[y, x + 1].hewan.grade == 8)
                        {
                            desx = x + 1;
                            desy = y;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (param.grade == 8 && b.map[y, x + 1].hewan.grade == 1)
                    {
                        return false;
                    }
                    else
                    {
                        if (param.grade > b.map[y, x + 1].hewan.grade)
                        {
                            desx = x + 1;
                            desy = y;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
                
            }
        }

        //Distance Function
        private double euclidean(int x, int y, int desx, int desy)
        {
            int dX = Math.Abs(desx - x);
            int dY = Math.Abs(desy - y);
            double distance = Math.Sqrt(dX * dX + dY * dY);

            //max cost = 12
            return distance;
        }

        private double backtrackPath(int x, int y, int desx, int desy, Animal animal, String player)
        {
            double distance = 0;
            int[,] sol = new int[9, 7];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    sol[i, j] = 0;
                }
            }

            if (backtrackGoal(x, y, desx, desy, sol, animal, player))
            {
                int total_path = 0;
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        total_path += sol[i, j];
                    }
                }
                distance = total_path;
            }
            else
            {
                distance = 64;
            }

            //MessageBox.Show(distance+"");
            return distance;
        }

        private bool backtrackGoal(int x, int y, int desx, int desy, int[,] sol, Animal animal, String player)
        {
            //String solution = animal.name+" : \n";
            //for (int i = 0; i < 9; i++)
            //{
            //    for (int j = 0; j < 7; j++)
            //    {
            //        solution += sol[i, j].ToString();
            //    }
            //    solution += "\n";
            //}
            //MessageBox.Show(solution);
            sol[y, x] = 1;
            if(x == desx && y == desy)
            {
                return true;
            }
            else
            {
                int nextx = -1, nexty = -1;
                if (AI_CheckLeft(animal, current_board, x, y, ref nextx, ref nexty, player) && sol[nexty, nextx] != 1)
                {
                    if(backtrackGoal(nextx, nexty, desx, desy, sol, animal, player))
                    {
                        return true;
                    }
                }
                if (AI_CheckRight(animal, current_board, x, y, ref nextx, ref nexty, player) && sol[nexty, nextx] != 1)
                {
                    if(backtrackGoal(nextx, nexty, desx, desy, sol, animal, player))
                    {
                        return true;
                    }
                }
                if (AI_CheckUp(animal, current_board, x, y, ref nextx, ref nexty, player) && sol[nexty, nextx] != 1)
                {
                    if(backtrackGoal(nextx, nexty, desx, desy, sol, animal, player))
                    {

                        return true;
                    }
                }
                if (AI_CheckDown(animal, current_board, x, y, ref nextx, ref nexty, player) && sol[nexty, nextx] != 1)
                {
                    if(backtrackGoal(nextx, nexty, desx, desy, sol, animal, player))
                    {
                        return true;
                    }
                }
                sol[y, x] = 0;
                return false;
            }
        }
        //LOGIC 0 -- WIN COND
        private double winScore(String player, Board b)
        {
            int score = 0;

            if(player == "Player 2")
            {
                if(b.map[0,3].hewan != null)
                {
                    if(b.map[0,3].hewan.belongsTo == "Player 1")
                    {
                        score = 1000;
                    }
                }
            }
            else
            {
                if (b.map[8, 3].hewan != null)
                {
                    if (b.map[8, 3].hewan.belongsTo == "Player 2")
                    {
                        score = 1000;
                    }
                }
            }

            return score;
        }


        //LOGIC 1 -- Jarak bidak ke base (540)
        private double heuristicBase(int x, int y)
        {
            double hasil = 640;
            hasil -= euclidean(x, y, 3, 8)*45;

            return hasil;
        }

        public void Suara(string url)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();

            player.SoundLocation = url;
            player.Play();
        }
        

        //LOGIC 2 -- Total antar bidak yang bisa dimakan (640)
        private double heuristicEat(Animal param, int x, int y, Board b, String playerMusuh, String player)
        {
            double last = -1;

            for(int i = 0; i<9; i++)
            {
                for(int j = 0; j<7; j++)
                {
                    if(b.map[i,j].hewan != null)
                    {
                        if (b.map[i, j].hewan.belongsTo == playerMusuh)
                        {
                            if (param.grade == 1)
                            {
                                if (b.map[i, j].hewan != null)
                                {
                                    if (b.map[i, j].hewan.grade == 8)
                                    {
                                        double hasil = 64 - backtrackPath(x, y, j, i,param, player) * 2;
                                        if (hasil > last)
                                        {
                                            last = hasil;
                                        }
                                    }

                                }
                            }
                            else
                            {
                                if (b.map[i, j].hewan != null)
                                {
                                    if (b.map[i, j].hewan.grade < param.grade)
                                    {
                                        double hasil = 64 - backtrackPath(x, y, j, i,param, player) * 2;
                                        if (hasil > last)
                                        {
                                            last = hasil;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                }
            }
            return last*10;
        }

        //LOGIC 3 -- Sisa Bidak Player
        private double countAnimal(String player, Board b)
        {
            double total = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if(b.map[i,j].hewan != null)
                    {
                        if(b.map[i,j].hewan.belongsTo == player)
                        {
                            total += b.map[i,j].hewan.grade * 50;
                        }
                    }
                }
            }
            return total;
        }

        private double evaluation(Board b)
        {
            double bestAI = 0;
            double bestPlayer = 0;

            //AI
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if(b.map[i,j].hewan != null)
                    {
                        if(b.map[i,j].hewan.belongsTo == "Player 2")
                        {
                            double scoreBase = heuristicBase(i, j);
                            //double scoreEat = heuristicEat(b.map[i, j].hewan, j, i, b, "Player 1", "Player 2");
                            double scoreWin = winScore("Player 2", b);
                            if(bestAI < scoreWin)
                            {
                                bestAI = scoreWin;
                            }
                            if(bestAI < scoreBase)
                            {
                                bestAI = scoreBase;
                            }
                            //if(bestAI < scoreEat)
                            //{
                            //    bestAI = scoreEat;
                            //}
                        }
                    }
                }
            }
            
            //Player
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (b.map[i, j].hewan != null)
                    {
                        if (b.map[i, j].hewan.belongsTo == "Player 1")
                        {
                            double scoreBase = heuristicBase(i, j);
                            //double scoreEat = heuristicEat(b.map[i, j].hewan, j,i, b, "Player 2", "Player 1");
                            double scoreWin = winScore("Player 1", b);
                            if (bestPlayer < scoreWin)
                            {
                                bestPlayer = scoreWin;
                            }
                            if (bestPlayer < scoreBase)
                            {
                                bestPlayer = scoreBase;
                            }
                            //if (bestPlayer < scoreEat)
                            //{
                            //    bestPlayer = scoreEat;
                            //}
                        }
                    }
                }
            }

            if(bestAI == 1000)
            {
                return 1000;
            }else if(bestPlayer == 1000)
            {
                return -1000;
            }
            else
            {
                bestAI += countAnimal("Player 2",b);
                bestPlayer += countAnimal("Player 1",b);
                return bestAI - bestPlayer;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            if(mode == 1)
            {
                dx += 1; 
                if(dx == 10) { mode = 2; }
            }
            else if(mode == 2)
            {
                dx -= 1;
                if (dx == 0) { mode = 1; }
            }

            // animasi aktif 
            if (animasi == 1)
            {
                if (x1 != x2)
                {
                    if (x1 > x2) { x1 -= 2; } else { x1 += 2; }
                    if (x1 == x2) {
                        animasi = 0;
                        Tile t = current_board.map[pily1, pilx1];
                        current_board.map[pily2, pilx2].hewan = t.hewan;
                        current_board.map[pily1, pilx1].hewan = null; 
                        clickedloc[0] = new Point(-1, -1);
                        counter = -1;
                        gantiGiliran(); 
                    }
                }
                else if (y1 != y2)
                {
                    if (y1 > y2) {  y1 -= 2; } else { y1 += 2; }
                    if (y1 == y2) {
                        animasi = 0;
                        Tile t = current_board.map[pily1, pilx1];
                        current_board.map[pily2, pilx2].hewan = t.hewan;
                        current_board.map[pily1, pilx1].hewan = null;
                        clickedloc[0] = new Point(-1, -1);
                        counter = -1;
                        gantiGiliran();
                    }
                }
            }

            this.Invalidate(); 
        }
    }
}
