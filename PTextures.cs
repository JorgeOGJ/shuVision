using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

namespace vcJOGJ
{
	/// <summary>
	///GUI usada para el reconocimiento de patrones de texturas y posteriormente clasificar una imagen
	/// </summary>
	public partial class PTextures : Form
	{
		// diccionarios usados para guardar el Set de Entramiento
		private Dictionary<string, double[]> TxtSet;
		//Diccionario usado para guardar las distancias  de la imagen a
		// clasificar con respecto a las texturas
		private Dictionary<string ,double>TxtDB;
		
		private Bitmap bmp;

		private double []vimg;
		private double[] dst;
		private Bitmap[] setBmp;
		private string [] setStr;
		private int ks=0;
		
		public PTextures()
		{
			
			InitializeComponent();
			GenerarSet();
		
		}

		void GenerarSet()
		{
			string []text=Directory.GetFiles("./texturas");
			
			
			TxtSet =new Dictionary<string, double[]>(text.Length);
			TxtDB=new Dictionary<string,double>(text.Length);
			setStr=new string[text.Length];
			setBmp=new Bitmap[text.Length];
			int k=0;
			foreach(string s in text)
			{
				
				double []d=new double[6];
				Bitmap tmp=new Bitmap(Bitmap.FromFile(s));
				setBmp[k]=new Bitmap(tmp);
				setStr[k]=s;
				string name=Path.GetFileNameWithoutExtension(s);
				
				
				Texturas.CalcSDH(new Bitmap(Bitmap.FromFile(s)));
	
				d[0]=Texturas.homogeneidad;
				d[1]=Texturas.contraste;
				d[2]=Texturas.energia;
				d[3]=Texturas.entropia;
				d[4]=Texturas.media;
				d[5]=Utils.Mean(tmp);
				TxtSet.Add(name,d);
				k++;
			}
			this.ks=k;
			textBox1.AppendText("Set de Clasificacion  generado correctamente");
		
		}
		
		void ImagenToolStripMenuItemClick(object sender, EventArgs e)
		{
			OpenFileDialog fd=new OpenFileDialog();
			if(fd.ShowDialog()== DialogResult.OK)
			{
				pictureBox1.Image=new Bitmap(Bitmap.FromFile(fd.FileName));
				bmp=new Bitmap(Bitmap.FromFile(fd.FileName));
			}
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			classify(bmp);
		}
		double euclidian(double []text,double []img)
		{
			double d = 0.0, u;

            for (int i = 0; i < text.Length; i++)
            {
                u = text[i] - img[i];
                d += u * u;
            }

			return Math.Sqrt(d);
		}
		void sort(ref double[]dist)
		{
			double temp=0;
			for(int i=0;i<dist.Length;i++)
			{
				for(int j=1;j<dist.Length;j++)
				{
					if(dist[i]>dist[j])
					{
						temp=dist[i];
						dist[i]=dist[j];
						dist[j]=temp;
					}
					
				}
			}
		}
		//calcula las propiedades de la imagen de entrada
		void getImg(Bitmap tmp)
		{
			
			    vimg=new double[6];
			    Texturas.CalcSDH(tmp);
	
				vimg[0]=Texturas.homogeneidad;
				vimg[1]=Texturas.contraste;
				vimg[2]=Texturas.energia;
				vimg[3]=Texturas.entropia;
				vimg[4]=Texturas.media;
				vimg[5]=Utils.Mean(tmp);
		
				
		}
		void classify(Bitmap bmp)
		{
			getImg(bmp);
			dst=new double[TxtSet.Count];
			int k=0;
			foreach(KeyValuePair<string , double[]> s in TxtSet)
			{
				double []d=s.Value;
				double dist=euclidian(d,vimg);
				dst[k]=dist;
				TxtDB.Add(s.Key,dist);
				k++;
			}
			sort(ref dst);
			textBox1.AppendText("\nClasificado!");
			pictureBox2.Image=getD();
			TxtDB.Clear();
			
		}
		
		Bitmap getD()
		{
		
			int k=0;
			for(int i=0;i<TxtDB.Count;i++)
			{
				string s=Path.GetFileNameWithoutExtension(setStr[i]);
				if(dst[0]==TxtDB[s])
				{
					k=i;
					textBox1.AppendText(string.Format("como :{0}\n",s));
					label2.Text=s;
					break;
				}
				else{continue;}
			}
			return setBmp[k];
		}
		
	
	}
}
