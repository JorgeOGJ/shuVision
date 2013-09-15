using System;
using System.Drawing;
using System.Windows.Forms;

namespace vcJOGJ
{
	/// <summary>
	/// Muestra una imagen en modo modal dentro de una ventana nueva
	/// </summary>
	public partial class ImgShow : Form
	{
		public ImgShow(Bitmap b,string name)
		{
			
			InitializeComponent();
			
		
				this.Text=name;
				this.pictureBox1.Image=b;
				this.Show();
				
			
		}
		
		void GuardarImagenToolStripMenuItemClick(object sender, EventArgs e)
		{
			SaveFileDialog sv=new SaveFileDialog();
			sv.Filter="bmp|*.bmp";
				
				if(sv.ShowDialog()== DialogResult.OK)
				{
					pictureBox1.Image.Save(sv.FileName,System.Drawing.Imaging.ImageFormat.Bmp);
				}
			
		}
	}
}
