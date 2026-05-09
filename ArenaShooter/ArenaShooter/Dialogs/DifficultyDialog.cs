using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaShooter.GameDialogs
{
    public static class DifficultyDialog
    {
        private const int DialogWidth = 600;
        private const int DialogHeight = 450;
        private const int BtnWidth = 240;
        private const int BtnHeight = 60;

        private static readonly Color BgColor = Color.FromArgb(45, 45, 48);
        private static readonly Color EasyColor = Color.FromArgb(76, 175, 80);
        private static readonly Color MediumColor = Color.FromArgb(121, 121, 121);
        private static readonly Color HardColor = Color.FromArgb(211, 47, 47);

        public static string Show()
        {
            string selected = "Easy";

            using (Form form = new Form())
            {
                form.Text = "Arena Shooter: Zombie Apocalypse";
                form.Size = new Size(DialogWidth, DialogHeight);
                form.StartPosition = FormStartPosition.CenterScreen;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.ControlBox = true;
                form.BackColor = BgColor;

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
                    Size = new Size(BtnWidth + 10, 250),
                    Location = new Point((form.ClientSize.Width - (BtnWidth + 10)) / 2, 120),
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false
                };

                void addBtn(string text, string val, Color color)
                {
                    Button btn = new Button()
                    {
                        Text = text,
                        Size = new Size(BtnWidth, BtnHeight),
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

                addBtn("EASY (Forest)", "Easy", EasyColor);
                addBtn("MEDIUM (City)", "Medium", MediumColor);
                addBtn("HARD (Graveyard)", "Hard", HardColor);

                form.Controls.Add(lbl);
                form.Controls.Add(buttonPanel);

                return (form.ShowDialog() == DialogResult.OK) ? selected : null;
            }
        }
    }
}