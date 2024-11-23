namespace Fox_n_Hounds_checkers
{
    public partial class Form1 : Form
    {
        Bitmap canvas;
        Field game;
        Graphics g;
        public Form1()
        {
            InitializeComponent();
            canvas = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(canvas);
            pictureBox1.Image = canvas;
            game = new Field(g);
            game.canvas = pictureBox1;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {

                if (!game.IsSelected())
                {
                    game.SelectClicked(e);
                }
                else
                {
                    game.MoveToBox(e);
                }
                if (game.FoxLost())
                {
                    game.DrawField();
                    MessageBox.Show("Hounds won! Hounds have chased down the fox!");
                    game.Init();
                }
                else if (game.FoxWon())
                {
                    game.DrawField();
                    MessageBox.Show("Fox won! Fox escaped!");
                    game.Init();
                }
                game.DrawField();
                if (checkBox1.Checked && (!game.FoxTurn))
                {
                    HoundsAI();
                }


            }
            catch (Exception ex)
            {
                Enabled = true;
                MessageBox.Show(ex.Message);
            }
            if (game.FoxTurn)
            {
                label2.Text = "Fox goes";
            }
            else
            {
                label2.Text = "Hounds go";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            game.Init();
            game.FoxTurn = true;
            game.DrawField();
            if (game.FoxTurn)
            {
                label2.Text = "Fox goes";
            }
            else
            {
                label2.Text = "Hounds go";
            }
        }
        private void HoundsAI()
        {

            Enabled = false;
            game.HoundsAITurn();
            game.DrawField();
            Enabled = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked && (!game.FoxTurn))
            {
                HoundsAI();
            }
        }
    }
}
