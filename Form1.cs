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
            canvas=new Bitmap(pictureBox1.Width,pictureBox1.Height);
            g=Graphics.FromImage(canvas);
            pictureBox1.Image=canvas;
            game = new Field(g);
            game.canvas=pictureBox1;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
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
                MessageBox.Show("Hounds won! Hounds have chased down the fox!");
                game.Init();
            }
            else if(game.FoxWon())
                 {
                     MessageBox.Show("Fox won! Fox escaped!");
                     game.Init();
                 }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            game.Init();
            game.DrawField();
        }
    }
}
