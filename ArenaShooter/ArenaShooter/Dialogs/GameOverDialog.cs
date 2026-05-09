using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaShooter.GameDialogs
{
    public static class GameOverDialog
    {
        private const int DialogWidth = 600;
        private const int DialogHeight = 450;
        private const int BtnWidth = 240;
        private const int BtnHeight = 60;

        private static readonly Color BgColor = Color.FromArgb(45, 45, 48);
        private static readonly Color EasyColor = Color.FromArgb(76, 175, 80);

        public static bool Show(int score, int high, string diff)
        {
            bool restart = false;

            using (Form form = new Form())
            {
                form.Text = "Arena Shooter: Zombie Apocalypse";
                form.Size = new Size(DialogWidth, DialogHeight);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.ControlBox = true;
                form.BackColor = BgColor;

                Label lblHeader = new Label()
                {
                    Text = "ZOMBIES ATE YOUR BRAIN!",
                    Font = new Font("Arial", 14, FontStyle.Bold),
                    ForeColor = Color.IndianRed,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(500, 40),
                    Location = new Point((form.ClientSize.Width - 500) / 2, 40)
                };

                Label lblStats = new Label()
                {
                    Text = $"Difficulty: {diff}\n\n" +
                           $"Best Score: {high}\n\n" +
                           $"Current Score: {score}",
                    Font = new Font("Arial", 11, FontStyle.Bold),
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(400, 150),
                    Location = new Point((form.ClientSize.Width - 400) / 2, 100)
                };

                Button btnRestart = new Button()
                {
                    Text = "PLAY AGAIN",
                    Size = new Size(BtnWidth, BtnHeight),
                    Location = new Point((form.ClientSize.Width - BtnWidth) / 2, 280),
                    BackColor = EasyColor,
                    ForeColor = Color.White,
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnRestart.FlatAppearance.BorderSize = 0;
                btnRestart.Click += (s, e) => { restart = true; form.DialogResult = DialogResult.OK; };

                form.Controls.Add(lblHeader);
                form.Controls.Add(lblStats);
                form.Controls.Add(btnRestart);

                form.ShowDialog();
                return restart;
            }
        }
    }
}