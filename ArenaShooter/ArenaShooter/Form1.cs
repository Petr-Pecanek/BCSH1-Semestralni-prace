using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

using ArenaShooter.Data;
using ArenaShooter.Entities;
using ArenaShooter.GameDialogs;

namespace ArenaShooter
{
    public partial class Form1 : Form
    {
        private const int GameTickIntervalMs = 20;
        private const int WeaponCooldownFrames = 10;
        private const float RepelForceMultiplier = 0.15f;
        private const float EnemyRepelDistance = 40f;
        private const float WallRepelDistance = 50f;
        private const int SpawnMargin = 50;

        private Player _player;
        private Image _playerImg;
        private Image _enemyImg;
        private bool _isMouseDown = false;
        private Point _mousePos;

        private List<Entity> _allEntities = new List<Entity>();
        HashSet<Keys> _pressedKeys = new HashSet<Keys>();
        private List<Obstacle> _obstacles = new List<Obstacle>();

        private Random _random = new Random();
        private Font _statsFont = new Font("Arial", 10, FontStyle.Bold);

        private string _currentDifficulty = "Easy";
        private int _spawnInterval = 40;
        private int _spawnTimer;
        private int _fireCooldown = 0;
        private int _pointsPerKill = 5;
        private int _currentScore;

        private GameData _gameData = new GameData();
        private DifficultyStats _currentStats;

        private TextureBrush _bgBrush;
        private Image[] _easyAssets;
        private Image[] _mediumAssets;
        private Image[] _hardAssets;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Text = "Arena Shooter: Zombie Apocalypse";

            this.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) _isMouseDown = true; };
            this.MouseUp += (s, e) => { if (e.Button == MouseButtons.Left) _isMouseDown = false; };
            this.MouseMove += (s, e) => { _mousePos = e.Location; };

            LoadAllAssets();

            _currentDifficulty = DifficultyDialog.Show();
            if (_currentDifficulty == null)
            {
                Environment.Exit(0);
                return;
            }

            _gameData = FileManager.Load();
            ApplyDifficultySettings();
            InitializeGame();
        }

        private void LoadAllAssets()
        {
            try
            {
                _playerImg = Image.FromFile("Assets/soldier.png");
                _enemyImg = Image.FromFile("Assets/zombie.png");

                _easyAssets = new Image[] {
                Image.FromFile("Assets/easy_asset1.png"),
                Image.FromFile("Assets/easy_asset2.png"),
                Image.FromFile("Assets/easy_asset3.png")
                };

                _mediumAssets = new Image[]
                {
                    Image.FromFile("Assets/medium_asset1.png"),
                    Image.FromFile("Assets/medium_asset2.png"),
                    Image.FromFile("Assets/medium_asset3.png")
                };

                _hardAssets = new Image[]
                {
                    Image.FromFile("Assets/hard_asset1.png"),
                    Image.FromFile("Assets/hard_asset2.png"),
                    Image.FromFile("Assets/hard_asset3.png")
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba pøi naèítání assetù: " + ex.Message);
            }
        }

        private void ApplyDifficultySettings()
        {
            this.WindowState = FormWindowState.Maximized;
            _currentStats = _gameData.Levels[_currentDifficulty];

            this.BackColor = Color.White;
            Image bgImg = null;

            switch (_currentDifficulty)
            {
                case "Easy":
                    _spawnInterval = 40;
                    _pointsPerKill = 5;
                    this.BackColor = Color.ForestGreen;
                    try {
                        bgImg = Image.FromFile("Assets/easy_bg.png"); 
                    } catch (Exception ex)
                    {
                        MessageBox.Show("Nepodaøilo se naèíst pozadí mapy: " + ex.Message);
                    }
                    break;
                case "Medium":
                    _spawnInterval = 25;
                    _pointsPerKill = 8;
                    this.BackColor = Color.DarkGray;
                    try
                    {
                        bgImg = Image.FromFile("Assets/medium_bg.png");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Nepodaøilo se naèíst pozadí mapy: " + ex.Message);
                    }
                    break;
                case "Hard":
                    _spawnInterval = 15;
                    _pointsPerKill = 10;
                    this.BackColor = Color.DimGray;
                    try { 
                        bgImg = Image.FromFile("Assets/hard_bg.png"); 
                    } catch (Exception ex)
                    {
                        MessageBox.Show("Nepodaøilo se naèíst pozadí mapy: " + ex.Message);
                    }
                    break;
            }

            if (bgImg != null)
            {
                _bgBrush = new TextureBrush(bgImg);
            }
        }

        private void InitializeGame()
        {
            Cursor.Hide();
            _pressedKeys.Clear();
            _allEntities.Clear();
            _isMouseDown = false;
            _currentScore = 0;
            _spawnTimer = 0;

            _player = new Player(0, 0, _playerImg);
            var screenBounds = Screen.PrimaryScreen.Bounds;
            _player.X = (screenBounds.Width / 2) - (_player.Width / 2);
            _player.Y = (screenBounds.Height / 2) - (_player.Height / 2);

            _allEntities.Add(_player);
            SetupMap();

            timer1.Interval = GameTickIntervalMs;
            timer1.Start();
        }

        private void SetupMap()
        {
            _obstacles.Clear();
            _allEntities.RemoveAll(e => e is Obstacle);

            Image[] currentAssets = null;
            int gridSpacing = 0;

            switch (_currentDifficulty)
            {
                case "Easy":
                    currentAssets = _easyAssets;
                    gridSpacing = 200;
                    break;
                case "Medium":
                    currentAssets = _mediumAssets;
                    gridSpacing = 200;
                    break;
                case "Hard":
                    currentAssets = _hardAssets;
                    gridSpacing = 200;
                    break;
            }

            GenerateMapAssets(currentAssets, gridSpacing);

            foreach (var wall in _obstacles)
            {
                _allEntities.Add(wall);
            }
        }

        private void GenerateMapAssets(Image[] mapAssets, int gridStep)
        {
            var screen = Screen.PrimaryScreen.Bounds;

            if (mapAssets == null || mapAssets.Length == 0) return;

            for (int x = 100; x < screen.Width - 100; x += gridStep)
            {
                for (int y = 100; y < screen.Height - 100; y += gridStep)
                {
                    if (Math.Abs(x - screen.Width / 2) < gridStep && Math.Abs(y - screen.Height / 2) < gridStep)
                        continue;

                    int offsetX = _random.Next(-40, 40);
                    int offsetY = _random.Next(-40, 40);

                    Image selectedAsset = mapAssets[_random.Next(mapAssets.Length)];

                    if (selectedAsset == null) continue;

                    _obstacles.Add(new Obstacle(
                        x + offsetX,
                        y + offsetY,
                        selectedAsset.Width,
                        selectedAsset.Height,
                        selectedAsset));
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_player == null) return;

            HandleEnemySpawning();

            if (_isMouseDown)
            {
                HandleManualShooting();
            }
            else
            {
                if (_fireCooldown < 10) _fireCooldown++;
            }

            UpdateEntities();
            ResolveCollisionsAndCleanup();

            this.Invalidate();
        }

        private void HandleEnemySpawning()
        {
            _spawnTimer++;
            if (_spawnTimer >= _spawnInterval)
            {
                SpawnEnemy();
                _spawnTimer = 0;
            }
        }

        private void SpawnEnemy()
        {
            int side = _random.Next(4);
            int x = 0, y = 0;

            switch (side)
            {
                case 0:
                    x = _random.Next(0, this.ClientSize.Width);
                    y = -SpawnMargin;
                    break;
                case 1:
                    x = _random.Next(0, this.ClientSize.Width);
                    y = this.ClientSize.Height + SpawnMargin;
                    break;
                case 2:
                    x = -SpawnMargin;
                    y = _random.Next(0, this.ClientSize.Height);
                    break;
                case 3:
                    x = this.ClientSize.Width + SpawnMargin;
                    y = _random.Next(0, this.ClientSize.Height);
                    break;
            }

            _allEntities.Add(new Enemy(x, y, _player, _enemyImg));
        }

        private void HandleManualShooting()
        {
            _fireCooldown++;
            if (_fireCooldown >= WeaponCooldownFrames)
            {
                _allEntities.Add(new Bullet(
                    _player.X + (_player.Width / 2),
                    _player.Y + (_player.Height / 2),
                    _mousePos.X,
                    _mousePos.Y));

                _fireCooldown = 0;
            }
        }

        private void UpdateEntities()
        {
            Point localMouse = this.PointToClient(Cursor.Position);
            _player.UpdateRotation(localMouse);

            float playerOldX = _player.X;
            float playerOldY = _player.Y;

            foreach (var entity in _allEntities)
            {
                entity.Update(_pressedKeys);
            }

            var enemies = _allEntities.OfType<Enemy>().ToList();

            for (int i = 0; i < enemies.Count; i++)
            {
                foreach (var wall in _obstacles)
                {
                    ApplyRepelForce(enemies[i], wall, WallRepelDistance);
                }

                for (int j = i + 1; j < enemies.Count; j++)
                {
                    ApplyRepelForce(enemies[i], enemies[j], EnemyRepelDistance);
                    ApplyRepelForce(enemies[j], enemies[i], EnemyRepelDistance);
                }
            }

            foreach (var entity in _allEntities)
            {
                if (entity is Enemy enemy)
                {
                    HandleEntityWallCollision(enemy, enemy.X, enemy.Y);
                }
            }

            HandleEntityWallCollision(_player, playerOldX, playerOldY);
            KeepPlayerInScreenBounds();
        }

        private void ApplyRepelForce(Entity toMove, Entity awayFrom, float repelDistance)
        {
            float dx = (toMove.X + toMove.Width / 2) - (awayFrom.X + awayFrom.Width / 2);
            float dy = (toMove.Y + toMove.Height / 2) - (awayFrom.Y + awayFrom.Height / 2);
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);

            if (dist < repelDistance && dist > 0)
            {
                toMove.X += (dx / dist) * (repelDistance - dist) * RepelForceMultiplier;
                toMove.Y += (dy / dist) * (repelDistance - dist) * RepelForceMultiplier;
            }
        }

        private void HandleEntityWallCollision(Entity entity, float oldX, float oldY)
        {
            foreach (var wall in _obstacles)
            {
                if (entity.Bounds.IntersectsWith(wall.Bounds))
                {
                    float tempX = entity.X;
                    entity.X = oldX;

                    if (entity.Bounds.IntersectsWith(wall.Bounds))
                    {
                        entity.X = tempX;
                        entity.Y = oldY;

                        if (entity.Bounds.IntersectsWith(wall.Bounds))
                        {
                            entity.X = oldX;
                        }
                    }
                    break;
                }
            }
        }

        private void KeepPlayerInScreenBounds()
        {
            if (_player.X < 0) _player.X = 0;
            if (_player.Y < 0) _player.Y = 0;

            if (_player.X + _player.Width > this.ClientSize.Width)
            {
                _player.X = this.ClientSize.Width - _player.Width;
            }
            if (_player.Y + _player.Height > this.ClientSize.Height)
            {
                _player.Y = this.ClientSize.Height - _player.Height;
            }
        }

        private void ResolveCollisionsAndCleanup()
        {
            List<Entity> toRemove = new List<Entity>();
            var bullets = _allEntities.OfType<Bullet>().ToList();
            var enemies = _allEntities.OfType<Enemy>().ToList();

            CheckPlayerDeath(enemies);
            HandleBulletHits(bullets, enemies, toRemove);
            HandleBulletWallCollisions(bullets, toRemove);

            foreach (var item in toRemove.Distinct())
            {
                _allEntities.Remove(item);
            }
        }

        private void CheckPlayerDeath(List<Enemy> enemies)
        {
            foreach (var enemy in enemies)
            {
                if (_player.Bounds.IntersectsWith(enemy.Bounds))
                {
                    GameOver();
                    return;
                }
            }
        }

        private void HandleBulletHits(List<Bullet> bullets, List<Enemy> enemies, List<Entity> toRemove)
        {
            foreach (var enemy in enemies)
            {
                if (toRemove.Contains(enemy)) continue;

                foreach (var bullet in bullets)
                {
                    if (bullet.Bounds.IntersectsWith(enemy.Bounds))
                    {
                        toRemove.Add(enemy);
                        toRemove.Add(bullet);
                        _currentScore += _pointsPerKill;
                        break;
                    }
                }
            }
        }

        private void HandleBulletWallCollisions(List<Bullet> bullets, List<Entity> toRemove)
        {
            foreach (var bullet in bullets)
            {
                if (toRemove.Contains(bullet)) continue;

                foreach (var wall in _obstacles)
                {
                    if (bullet.Bounds.IntersectsWith(wall.Bounds))
                    {
                        toRemove.Add(bullet);
                        break;
                    }
                }

                if (IsOutOfBounds(bullet))
                {
                    toRemove.Add(bullet);
                }
            }
        }

        private bool IsOutOfBounds(Bullet b)
        {
            return b.X < -100 || b.X > this.ClientSize.Width + 100 || b.Y < -100 || b.Y > this.ClientSize.Height + 100;
        }

        private void GameOver()
        {
            timer1.Stop();
            Cursor.Show();

            _currentStats.LastScore = _currentScore;
            if (_currentScore > _currentStats.HighScore)
            {
                _currentStats.HighScore = _currentScore;
            }

            FileManager.Save(_gameData);

            bool wantsRestart = GameOverDialog.Show(_currentScore, _currentStats.HighScore, _currentDifficulty);
            if (wantsRestart)
            {
                InitializeGame();
            }
            else
            {
                Application.Exit();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentDifficulty) || _gameData == null) return;

            base.OnPaint(e);

            if (_bgBrush != null)
            {
                e.Graphics.FillRectangle(_bgBrush, this.ClientRectangle);
            }
            else
            {
                using (Brush b = new SolidBrush(this.BackColor))
                {
                    e.Graphics.FillRectangle(b, this.ClientRectangle);
                }
            }

            foreach (var entity in _allEntities)
            {
                entity.Draw(e.Graphics);
            }

            float x = 20;
            e.Graphics.DrawString($"Difficulty: {_currentDifficulty}", _statsFont, Brushes.White, x, 20);
            e.Graphics.DrawString($"Score: {_currentScore}", _statsFont, Brushes.White, x, 40);
            e.Graphics.DrawString($"Last: {_currentStats.LastScore}", _statsFont, Brushes.Gray, x, 60);
            e.Graphics.DrawString($"Best: {_currentStats.HighScore}", _statsFont, Brushes.Gold, x, 80);

            if (!timer1.Enabled) return;

            using (Pen sightPen = new Pen(Color.Red, 2))
            {
                e.Graphics.DrawLine(sightPen, _mousePos.X - 10, _mousePos.Y, _mousePos.X + 10, _mousePos.Y);
                e.Graphics.DrawLine(sightPen, _mousePos.X, _mousePos.Y - 10, _mousePos.X, _mousePos.Y + 10);
                e.Graphics.DrawEllipse(sightPen, _mousePos.X - 5, _mousePos.Y - 5, 10, 10);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            _pressedKeys.Add(e.KeyCode);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            _pressedKeys.Remove(e.KeyCode);
        }
    }
}
