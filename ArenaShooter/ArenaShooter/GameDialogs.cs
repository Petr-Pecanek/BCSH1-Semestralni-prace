using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaShooter
{
    public static class GameDialogs
    {
        public static string ShowDifficultySelection()
        {
            string selected = "Easy";

            using (Form form = new Form())
            {
                form.Text = "Arena Shooter: Zombie Apocalypse";
                form.Size = new Size(600, 450);
                form.StartPosition = FormStartPosition.CenterScreen;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.ControlBox = true;
                form.BackColor = Color.FromArgb(45, 45, 48);

                Label lbl = new Label()
                {
                    Text = "CHOOSE DIFFICULTY",
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(400, 30),
                    Location = new Point((form.ClientSize.Width - 400) / 2, 50)
                };

                FlowLayoutPanel buttonPanel = new FlowLayoutPanel()
                {
                    Size = new Size(250, 250),
                    Location = new Point((form.ClientSize.Width - 250) / 2, 120),
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false
                };

                void addBtn(string text, string val, Color color)
                {
                    Button btn = new Button()
                    {
                        Text = text,
                        Size = new Size(240, 60),
                        Margin = new Padding(0, 0, 0, 15),
                        BackColor = color,
                        ForeColor = Color.White,
                        Font = new Font("Arial", 10, FontStyle.Bold),
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand
                    };
                    btn.FlatAppearance.BorderSize = 0;
                    btn.Click += (s, e) => { selected = val; form.DialogResult = DialogResult.OK; };
                    buttonPanel.Controls.Add(btn);
                }

                addBtn("EASY (Forest)", "Easy", Color.FromArgb(76, 175, 80));
                addBtn("MEDIUM (City)", "Medium", Color.FromArgb(121, 121, 121));
                addBtn("HARD (Graveyard)", "Hard", Color.FromArgb(211, 47, 47));

                form.Controls.Add(lbl);
                form.Controls.Add(buttonPanel);

                return (form.ShowDialog() == DialogResult.OK) ? selected : null;
            }
        }

        public static bool ShowGameOver(int score, int high, string diff)
        {
            bool restart = false;

            using (Form form = new Form())
            {
                form.Text = "Arena Shooter: Zombie Apocalypse";
                form.Size = new Size(600, 450);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.ControlBox = true;
                form.BackColor = Color.FromArgb(45, 45, 48);

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
                    Size = new Size(240, 60),
                    Location = new Point((form.ClientSize.Width - 240) / 2, 280),
                    BackColor = Color.FromArgb(76, 175, 80),
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