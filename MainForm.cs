/*
 * Creado por SharpDevelop.
 * Usuario: JorgeOG
 * Fecha: 05/06/2012
 * Hora: 08:48 p.m.
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using AForge.Controls;
using AForge.Video;
using AForge.Video.DirectShow;

namespace vcJOGJ
{
	
	/// <summary>
	/// Formulario principal de la version 3.5 de la app para Visión por Computadora /2012
	/// </summary>
	public partial class MainForm : Form
	{
		
		public MainForm()
		{

			InitializeComponent();
			this.AllowDrop=true;
			
			histogram1.Color=Color.Red;
			histogram2.Color=Color.Green;
			histogram3.Color=Color.Blue;
			histogram1.BackColor=Color.Gainsboro;
			histogram2.BackColor=Color.Gainsboro;
			histogram3.BackColor=Color.Gainsboro;
			histogram4.BackColor=Color.Gainsboro;
			histogram5.BackColor=Color.Gainsboro;
			videoSourcePlayer1.Hide();
			comboBox1.Text="1";
			tabPage1.Text="Main";
			tabPage2.Text="Custom";
			tabControl1.SelectTab(tabPage1);
			label14.Text="Offset";
			label15.Text="Factor";
			textBox12.Text="0";
			textBox13.Text="1";
			sliderControl1.PositionChanged+= new SliderControl.PositionChangedHandler(sliderControl1_PositionChanged);
			filtrosToolStripMenuItem1.Enabled=false;
			
		}

		#region Metodos para la GUI
		protected override void OnDragEnter(DragEventArgs drgevent)
		{
			base.OnDragEnter(drgevent);
			drgevent.Effect= DragDropEffects.Move;
		}
		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			base.OnDragDrop(drgevent);
			int x=PointToClient(new Point(drgevent.X,drgevent.Y)).X;
			int y=PointToClient(new Point(drgevent.X,drgevent.Y)).Y;
			Rectangle r1=new Rectangle(x,y,5,5);
			Rectangle r2=new Rectangle(pictureBox1.Location.X,pictureBox1.Location.Y,pictureBox1.Width,pictureBox1.Height);
			if(r1.IntersectsWith(r2))
			{
				string[]files=(string[])drgevent.Data.GetData(DataFormats.FileDrop);
				switch(Media(files[0]))
				{
						case 0:{
							init=new Bitmap(System.Drawing.Image.FromFile(files[0]));
							bmp=init.Clone() as Bitmap;
							pictureBox1.Image=System.Drawing.Image.FromFile(files[0]);
							histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
							histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
							histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
							histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
							histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
							break;
						}
					case 1:
						{
							FileVideoSource fileSource = new FileVideoSource(files[0]);
							OpenVideoSource( fileSource );
							pictureBox1.Hide();
							videoSourcePlayer1.Show();
							videoSourcePlayer1.BringToFront();
							isVideo=true;
							break;
						}
						default:{MessageBox.Show("No es ni video ni imagen :( ");break;}
				}
				
			}
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			//worker.Join(100);
			CloseCurrentVideoSource();
			
		}
		private void sliderControl1_PositionChanged(object sender, float position)
		{
			
			if(!isVideo)
			{
				umbral=(int)(((1+position)*255)/2);
				label16.Text=umbral.ToString();
				bmp=init.Clone() as Bitmap;
				if(!IsMoravec)
				{
					filtros.Thresholding(bmp,umbral);
					pictureBox1.Image=bmp;
					histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
					histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
					histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
					histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
					histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
				}
				else
				{
					filtros.Moravec(bmp,umbral);
					pictureBox1.Image=bmp;
					histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
					histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
					histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
					histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
					histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
				}
			}
			else
			{
				if(!IsMoravec)
				{
					umbral=(int)(((1+position)*255)/2);
					label16.Text=umbral.ToString();
					k=14;
					seq.Add(13);
				}
				else
				{
					umbral=(int)(((1+position)*255)/2);
					label16.Text=umbral.ToString();
					k=15;
					seq.Add(8);
				}
			}
		}
		private delegate void SetValuesCallback( AForge.Controls.Histogram  control, double[] values);
		private void SetValues(AForge.Controls.Histogram  control, double[] values )
		{
			try
			{
				
				if ( control.InvokeRequired )
				{
					SetValuesCallback d = new SetValuesCallback( SetValues );
					Invoke( d, new object[] { control, values} );

				}
				else
				{
					if(!control.IsDisposed)
						control.Values=values;
					
				}
			}catch(Exception e){Console.WriteLine(e);}
		}
		private void CalcHisto()
		{
			try{
				while(!stop)
				{
					
					SetValues(histogram1,filtros.Histograma((video),0));
					SetValues(histogram2,filtros.Histograma((video),1));
					SetValues(histogram3,filtros.Histograma((video),2));
					SetValues(histogram4,filtros.Histograma((video),3));
					SetValues(histogram5,filtros.Histograma((video),4));
				}
			}catch(Exception e){Console.WriteLine(e);}
			
		}
		private void EnableVideo(bool flag)
		{
			if(flag)
			{
				
			}
			else
			{
				
				pictureBox1.Show();
				
				videoSourcePlayer1.Hide();
				videoSourcePlayer1.SendToBack();
				label11.Hide();
				flag=false;
				isVideo=false;
			}
		}
		private void OpenVideoSource( IVideoSource source )
		{
			filtrosToolStripMenuItem1.Enabled=true;
			if(asyncV!=null && asyncV.IsRunning)CloseCurrentVideoSource( );
			try{
				asyncV=new AsyncVideoSource(source,true);
				asyncV.NewFrame+=new NewFrameEventHandler(asyncV_NewFrame);
				asyncV.Start();
				videoSourcePlayer1.VideoSource = asyncV;
				video=new Bitmap(videoSourcePlayer1.Size.Width,videoSourcePlayer1.Size.Height);
				stopWatch = null;
				timer1.Start();
				stop=false;
				worker = new Thread( new ThreadStart( CalcHisto) );
				worker.Start( );
			}catch(Exception e){Console.WriteLine(e);}
			
		}
		private void CloseCurrentVideoSource( )
		{
			try{
				if(videoSourcePlayer1.VideoSource!=null)
				{
					k=0;
					
					isVideo=false;
					stop=true;
					asyncV.Stop();
					asyncV.WaitForStop();
					
					//asyncV=null;
					video=null;
					worker.Join(100);
					worker=null;
					seq.Clear();
				}
			}catch(Exception e){Console.WriteLine(e);}
		}
		private void Timer1Tick(object sender, EventArgs e)
		{
			
			if(asyncV.IsRunning)
			{
				if ( asyncV.NestedVideoSource != null )
				{
					
					int framesReceived = asyncV.FramesProcessed;

					if ( stopWatch == null )
					{
						stopWatch = new Stopwatch( );
						stopWatch.Start( );
					}
					else
					{
						stopWatch.Stop( );

						float fps = 1000.0f * framesReceived / stopWatch.ElapsedMilliseconds;
						label11.Text = fps.ToString( "F2" ) + " fps";
						
						stopWatch.Reset( );
						stopWatch.Start( );
					}
					
				}
			}
		}
		private void AbrirImagenToolStripMenuItem1Click(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				
				OpenFileDialog fd=new OpenFileDialog();
				if(fd.ShowDialog()==DialogResult.OK)
				{
					init=new Bitmap(fd.FileName);
					bmp=init.Clone() as Bitmap;
					pictureBox1.Image=bmp;
					histogram1.Values= filtros.Histograma(bmp,0);
					histogram2.Values= filtros.Histograma(bmp,1);
					histogram3.Values= filtros.Histograma(bmp,2);
					histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
					histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
				}
			}
			else
			{
				EnableVideo(false);
				CloseCurrentVideoSource();
				OpenFileDialog fd=new OpenFileDialog();
				if(fd.ShowDialog()==DialogResult.OK)
				{
					init=new Bitmap(fd.FileName);
					bmp=init.Clone() as Bitmap;
					pictureBox1.Image=bmp;
					histogram1.Values= filtros.Histograma(bmp,0);
					histogram2.Values= filtros.Histograma(bmp,1);
					histogram3.Values= filtros.Histograma(bmp,2);
					histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
					histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
				}
			}
			
		}
		private void CargarImagenOriginalToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				bmp=init.Clone() as Bitmap;
				pictureBox1.Image=init;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
			}
			else
			{
				k=0;
				seq.Clear();
			}
		}
		private void AbrirCamaraToolStripMenuItemClick(object sender, EventArgs e)
		{
			
			
			pictureBox1.Hide();
			label11.Show();
			videoSourcePlayer1.Show();
			videoSourcePlayer1.BringToFront();
			VideoCaptureDeviceForm form = new VideoCaptureDeviceForm( );

			if ( form.ShowDialog( this ) == DialogResult.OK )
			{
				VideoCaptureDevice videoSource = form.VideoDevice;
				OpenVideoSource( videoSource );
				isVideo=true;
				
			}
			
			
		}
		private void asyncV_NewFrame(object sender, NewFrameEventArgs eventArgs)
		{
			c=FromName();
			int min,max,minr,maxr,maxg,ming,maxb,minb;
			minr=ming=minb=min=(int)numericUpDown3.Value;
			maxg=maxb=maxr=max=(int)numericUpDown4.Value;
			float sigma=(float)numericUpDown2.Value;
			int low=(byte)numericUpDown5.Value;
			int high=(byte)numericUpDown6.Value;
			float sigma2=(float)numericUpDown7.Value;
			float K=(float)numericUpDown8.Value;
			float umbral2=(float)numericUpDown9.Value;
			
			if(seq.Count==0)
			{
				switch(k)
				{
						case 1:{filtros.Gris(eventArgs.Frame);break;}
						case 2:{filtros.Negativo(eventArgs.Frame);break;}
						case 3:{filtros.Gris(eventArgs.Frame);filtros.EcualizarHistograma(eventArgs.Frame);break;}
						case 4:{filtros.EcualizarHistograma(eventArgs.Frame);break;}
						case 5:{filtros.Gris(eventArgs.Frame);filtros.ExpandirHistograma(eventArgs.Frame,min,max);break;}
						case 6:{filtros.ExpandirHistograma(eventArgs.Frame,minr,maxr,ming,maxg,minb,maxb);break;}
						case 8:{filtros.FastConvolution(eventArgs.Frame,filtro,offset);break;}
						case 9:{filtros.Thresholding(eventArgs.Frame);break;}
						case 10:{filtros.Gris(eventArgs.Frame);filtros.Multinivel(eventArgs.Frame,level);break;}
						case 11:{filtros.Regiones(eventArgs.Frame,level,c,over);break;}
						case 12:{filtros.Multinivel(eventArgs.Frame,level);break;}
						case 13:{filtros.Gris(eventArgs.Frame);filtros.Regiones(eventArgs.Frame,level,c,over);break;}
						case 14:{filtros.Thresholding(eventArgs.Frame,umbral);break;}
						case 15:{filtros.Moravec(eventArgs.Frame,umbral);break;}
						case 16:{filtros.Canny(eventArgs.Frame,sigma,(byte)low,(byte)high);break;}
						case 17:{filtros.HoughT(eventArgs.Frame);break;}
						case 18:{filtros.Harris(eventArgs.Frame,K,sigma2,umbral2);break;}
						case 19:{COLOR.CIELAB.ExtraerL(eventArgs.Frame,color,true);break;}
						case 20:{COLOR.CIELAB.Extraera(eventArgs.Frame,color,true);break;}
						case 21:{COLOR.CIELAB.Extraerb(eventArgs.Frame,color,true);break;}
						case 22:{Texturas.CalcSDH(eventArgs.Frame);System.Threading.Tasks.Parallel.Invoke(new Action(SetText));break;}
						case 30:{filtros.FaceDetect(eventArgs.Frame);break;}
						case 31:{FFT.FT(eventArgs.Frame);break;}
				}
				
			}
			else
			{
				for(int i=0;i<seq.Count;i++)
				{
					filtros.Secuencia(eventArgs.Frame,(int)seq[i],filtro,level,offset,umbral,min,max,sigma,low,high,sigma2,K,umbral2,c,true,over);
				}
			}
			
			try{
				video=eventArgs.Frame.Clone() as Bitmap;
			}catch(Exception e){Console.WriteLine(e);}
			
		}
		private void SetText()
		{
			meanText.Text=Texturas.media.ToString();
			energiaText.Text=Texturas.energia.ToString();
			entroText.Text=Texturas.entropia.ToString();
			homoText.Text=Texturas.homogeneidad.ToString();
			conText.Text=Texturas.contraste.ToString();
		}
		private void GuardarImagenToolStripMenuItem1Click(object sender, EventArgs e)
		{
			if(bmp==null)AbrirImagenToolStripMenuItem1Click(sender,e);
			else
			{
				SaveFileDialog sv=new SaveFileDialog();
				sv.Filter="bmp|*.bmp";
				
				if(sv.ShowDialog()== DialogResult.OK)
				{
					pictureBox1.Image.Save(sv.FileName,System.Drawing.Imaging.ImageFormat.Bmp);
				}
			}
			
		}
		private void AbrirVideoToolStripMenuItemClick(object sender, EventArgs e)
		{
			
			
			OpenFileDialog fd=new OpenFileDialog();
			if ( fd.ShowDialog( ) == DialogResult.OK )
			{
				
				FileVideoSource fileSource = new FileVideoSource(fd.FileName );
				OpenVideoSource( fileSource );
				pictureBox1.Hide();
				
				videoSourcePlayer1.Show();
				videoSourcePlayer1.BringToFront();
				isVideo=true;
			}
		}
		private void GetMatrix()
		{
			switch(dim)
			{
				case 1:
					{
						filtro[0,0]=(float)(1.0/10.0);
						filtro[0,1]=(float)(1.0/10.0);
						filtro[0,2]=(float)(1.0/10.0);
						filtro[1,0]=(float)(1.0/10.0);
						filtro[1,1]=(float)(1.0/5.0);
						filtro[1,2]=(float)(1.0/10.0);
						filtro[2,0]=(float)(1.0/10.0);
						filtro[2,1]=(float)(1.0/10.0);
						filtro[2,2]=(float)(1.0/10.0);
						break;
					}
				case 2:
					{
						filtro[0,0]=float.Parse(textBox47.Text);
						filtro[0,1]=float.Parse(textBox46.Text);
						filtro[1,0]=float.Parse(textBox44.Text);
						filtro[1,1]=float.Parse(textBox43.Text);
						break;
					}
				case 3:
					{
						filtro[0,0]=float.Parse(textBox3.Text);
						filtro[0,1]=float.Parse(textBox4.Text);
						filtro[0,2]=float.Parse(textBox5.Text);
						filtro[1,0]=float.Parse(textBox6.Text);
						filtro[1,1]=float.Parse(textBox7.Text);
						filtro[1,2]=float.Parse(textBox8.Text);
						filtro[2,0]=float.Parse(textBox9.Text);
						filtro[2,1]=float.Parse(textBox10.Text);
						filtro[2,2]=float.Parse(textBox11.Text);
						break;
					}
					
			}
		}
		private void Button1Click(object sender, EventArgs e)
		{
			
			dim=3;
			filtro=new float[dim,dim];
			GetMatrix();
			tabControl1.SelectTab(tabPage1);
			filtros.Convolution(bmp, filtro,int.Parse(textBox12.Text));
			pictureBox1.Image=bmp;
			
			histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
			histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
			histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
			histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
			histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
		}
		private void Button3Click(object sender, EventArgs e)
		{
			dim=2;
			filtro=new float[dim,dim];
			GetMatrix();
			tabControl1.SelectTab(tabPage1);
			filtros.Convolution(bmp, filtro,int.Parse(textBox12.Text));
			pictureBox1.Image=bmp;
			
			histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
			histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
			histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
			histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
			histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
		}
		private void BlurToolStripMenuItemClick(object sender, EventArgs e)
		{
			textBox3.Text="1/10";
			textBox4.Text="1/10";
			textBox5.Text="1/10";
			textBox6.Text="1/10";
			textBox7.Text="1/5";
			textBox8.Text="1/10";
			textBox9.Text="1/10";
			textBox10.Text="1/10";
			textBox11.Text="1/10";
			dim=1;
			filtro=new float[3,3];
			GetMatrix();
			tabControl1.SelectTab(tabPage1);
			filtros.Convolution(bmp, filtro,int.Parse(textBox12.Text));
			pictureBox1.Image=bmp;
			
			histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
			histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
			histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
			histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
			histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
		}
		private void FiltrosToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				
				tabControl1.SelectTab(tabPage2);
				
			}
			else
			{
				k=8;
			}
		}
		private void NumericUpDown1ValueChanged(object sender, EventArgs e)
		{
			if(isVideo)
			{
				if(checkBox1.Checked)
				{
					if(IsGray)
					{
						level=(int)numericUpDown1.Value;
						k=13;
						seq.Add(12);
					}
					else
					{
						level=(int)numericUpDown1.Value;
						k=11;
						seq.Add(10);
					}
				}
				else
				{
					if(IsGray)
					{
						level=(int)numericUpDown1.Value;
						k=10;
						seq.Add(9);
					}
					else
					{
						level=(int)numericUpDown1.Value;
						k=12;
						seq.Add(11);
					}
					
				}
			}
			else
			{
				if(!checkBox1.Checked)
				{
					if(!IsGray)
					{
						bmp=init.Clone() as Bitmap;
						filtros.Multinivel(bmp,(int)numericUpDown1.Value);
						pictureBox1.Image=bmp;
						histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
						histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
						histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
						histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
						histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
					}
					else
					{
						bmp=init.Clone() as Bitmap;
						filtros.Gris(bmp);
						filtros.Multinivel(bmp,(int)numericUpDown1.Value);
						pictureBox1.Image=bmp;
						histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
						histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
						histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
						histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
						histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
					}
				}
				else
				{
					if(!IsGray)
					{
						bmp=init.Clone() as Bitmap;
						filtros.Regiones(bmp,(int)numericUpDown1.Value,FromName());
						pictureBox1.Image=bmp;
						histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
						histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
						histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
						histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
						histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
					}
					else
					{
						bmp=init.Clone() as Bitmap;
						filtros.Gris(bmp);
						filtros.Regiones(bmp,(int)numericUpDown1.Value,FromName());
						pictureBox1.Image=bmp;
						histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
						histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
						histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
						histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
						histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
					}
				}
			}
		}
		private void NumericUpDown3ValueChanged(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				bmp=init.Clone() as Bitmap;
				filtros.ExpandirHistograma(bmp,(int)numericUpDown3.Value,(int)numericUpDown4.Value);
				pictureBox1.Image=bmp;
				
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
				
			}
			
		}
		private void NumericUpDown4ValueChanged(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				bmp=init.Clone() as Bitmap;
				filtros.ExpandirHistograma(bmp,(int)numericUpDown3.Value,(int)numericUpDown4.Value);
				pictureBox1.Image=bmp;
				
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
				
			}
			
		}
		private void NumericUpDown2ValueChanged(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				bmp=init.Clone() as Bitmap;
				filtros.Canny(bmp,(float)numericUpDown2.Value,(byte)numericUpDown5.Value,(byte)numericUpDown6.Value);
				pictureBox1.Image=bmp;

				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
			}
			
		}
		private void NumericUpDown5ValueChanged(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				bmp=init.Clone() as Bitmap;
				filtros.Canny(bmp,(float)numericUpDown2.Value,(byte)numericUpDown5.Value,(byte)numericUpDown6.Value);
				pictureBox1.Image=bmp;

				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
				
			}
			
		}
		private void NumericUpDown6ValueChanged(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				bmp=init.Clone() as Bitmap;
				filtros.Canny(bmp,(float)numericUpDown2.Value,(byte)numericUpDown5.Value,(byte)numericUpDown6.Value);
				pictureBox1.Image=bmp;

				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
				
			}
			
		}
		void NumericUpDown7ValueChanged(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				
				bmp=init.Clone() as Bitmap;
				filtros.Harris(bmp,(float)numericUpDown8.Value,(float)numericUpDown7.Value,(float)numericUpDown9.Value);
				pictureBox1.Image=bmp;

				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
				
			}
		}
		
		void NumericUpDown8ValueChanged(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				bmp=init.Clone() as Bitmap;
				filtros.Harris(bmp,(float)numericUpDown8.Value,(float)numericUpDown7.Value,(float)numericUpDown9.Value);				pictureBox1.Image=bmp;
				pictureBox1.Image=bmp;

				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
				
			}
		}
		
		void NumericUpDown9ValueChanged(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				bmp=init.Clone() as Bitmap;
				filtros.Harris(bmp,(float)numericUpDown8.Value,(float)numericUpDown7.Value,(float)numericUpDown9.Value);
				pictureBox1.Image=bmp;

				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
				
			}
			
		}
		#endregion
		
		#region Variables
		private Bitmap init;
		private Bitmap bmp;
		private bool isVideo=false;
		private Bitmap temp;
		private Bitmap salida;
		private int k=0;
		private object dummy=new object();
		private AsyncVideoSource asyncV;
		private Stopwatch stopWatch = null;
		private Thread worker=null;
		private  Bitmap video;
		private volatile bool stop=true;
		private int umbral=127;
		private int level=2;
		private float [,]filtro;
		private int dim=3;
		private bool IsGray=false;
		private bool IsMoravec=false;
		private Imagen filtros=new Imagen();
		private int offset=0;
		private ArrayList seq=new ArrayList();
		#endregion
		
		#region Metodos donde se aplican los algoritmos
		private void EscalaDeGrisesToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				filtros.Gris(bmp);
				pictureBox1.Image=bmp;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
			}
			else
			{
				k=1;
				seq.Add(1);
			}
			
		}
		private void NegativoToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				filtros.Negativo(bmp);
				pictureBox1.Image=bmp;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
			}
			else
			{
				k=2;
				seq.Add(2);
			}
			
		}
		private void ExpandirToolStripMenuItemClick(object sender, EventArgs e)
		{
			int min=(int)numericUpDown3.Value;
			int max=(int)numericUpDown4.Value;
			filtros.ExpandirHistograma(bmp,min,max);
			pictureBox1.Image=bmp;
			histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
			histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
			histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
			histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
			histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
		}
		private void GrisToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				filtros.Gris(bmp);
				filtros.EcualizarHistograma(bmp);
				pictureBox1.Image=bmp;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
			}
			else
			{
				k=3;
				seq.Add(3);
			}
		}
		private void ColorToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				filtros.EcualizarHistograma(bmp);
				pictureBox1.Image=bmp;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
			}
			else
			{
				k=4;
				seq.Add(4);
			}
		}
		private void GrisToolStripMenuItem1Click(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				filtros.Gris(bmp);
				filtros.ExpandirHistograma(bmp,(int)numericUpDown3.Value,(int)numericUpDown4.Value);
				pictureBox1.Image=bmp;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
			}
			else
			{
				k=5;
				seq.Add(5);
			}
		}
		private void ColorToolStripMenuItem1Click(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				int rmin;
				int rmax;
				int gmin;
				int gmax;
				int bmax;
				int bmin;
				gmin=bmin=rmin=(int)numericUpDown3.Value;
				rmax=bmax=gmax=(int)numericUpDown4.Value;
				
				filtros.ExpandirHistograma(bmp,rmin,rmax,gmin,gmax,bmin,bmax);
				pictureBox1.Image=bmp;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
			}
			else
			{
				k=6;
				seq.Add(6);
			}
		}
		private void TemplateMatchingToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				OpenFileDialog fd=new OpenFileDialog();
				if(fd.ShowDialog()==DialogResult.OK)
				{
					temp=new Bitmap(fd.FileName);
					pictureBox2.Image=temp;
				}
				salida=new Bitmap(bmp.Width-temp.Width+1,bmp.Height-temp.Height+1);
				Application.DoEvents();
				int cc=int.Parse(this.comboBox1.Text);
				
				filtros.Correlation(ref salida,bmp,temp,cc);
				
				bmp=salida;
				pictureBox1.Image=salida;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
			}
		}
		private void UsandoMascaraToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				
				double [,]filtro={{1,1},
					{-1,-1,}};
				double[,]f={{1,1,1,1,1},{1,1,1,1,1},{1,1,1,1,1},{1,1,1,1,1},{1,1,1,1,1}};
				Bitmap zsalida=new Bitmap(bmp.Width+filtro.GetUpperBound(0)-1,bmp.Height+filtro.GetUpperBound(1)-1);
				
				filtros.Correlation(ref zsalida,bmp,ref  f);
				
				pictureBox1.Image=zsalida;
				bmp=zsalida;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
			}
			
		}
		void SobelNToolStripMenuItem1Click(object sender, EventArgs e)
		{
			filtro=new float[,]{{-1,-2,-1},
				{0,0,0},
				{1,2,1}};
			k=8;
			offset=127;
			seq.Add(7);
		}
		void SobelWToolStripMenuItem1Click(object sender, EventArgs e)
		{
			filtro=new float[,]{{-1,0,1},
				{-2,0,2},
				{-1,0,1}};
			k=8;
			offset=127;
			seq.Add(7);
		}
		void SobelNWToolStripMenuItem1Click(object sender, EventArgs e)
		{
			filtro=new float[,]{{-2,-2,0},
				{-2,0,2},
				{0,2,2}};
			k=8;
			offset=127;
			seq.Add(7);
		}
		void LAP1ToolStripMenuItem1Click(object sender, EventArgs e)
		{
			filtro=new float[,]{{-1,-1,-1},
				{0,0,0},
				{1,1,1}};
			k=8;
			offset=127;
			seq.Add(7);
		}
		void Laplaciano2ToolStripMenuItemClick(object sender, EventArgs e)
		{
			filtro=new float[,]{{-1,0,1},
				{-1,0,1},
				{-1,0,1}};
			k=8;
			offset=127;
			seq.Add(7);
		}
		void Laplaciano3ToolStripMenuItemClick(object sender, EventArgs e)
		{
			filtro=new float[,]{{-2,-1,0},
				{-1,0,1},
				{0,1,2}};
			k=8;
			offset=127;
			seq.Add(7);
		}
		void SharpenToolStripMenuItemClick(object sender, EventArgs e)
		{
			filtro=new float[,]{{0,-1,0},
				{-1,5,-1},
				{0,-1,0}};
			k=8;
			offset=0;
			seq.Add(7);
		}
		void BlurToolStripMenuItem1Click(object sender, EventArgs e)
		{
			filtro=Utils.Kernel2D(1.4f);
			
			k=8;
			offset=45;
			seq.Add(7);
		}
		void RobertsToolStripMenuItemClick(object sender, EventArgs e)
		{
			textBox47.Text="1";
			textBox46.Text="1";
			textBox44.Text="-1";
			textBox43.Text="-1";
		}
		void LAP1ToolStripMenuItemClick(object sender, EventArgs e)
		{
			textBox3.Text="0";
			textBox4.Text="1";
			textBox5.Text="0";
			textBox6.Text="1";
			textBox7.Text="-4";
			textBox8.Text="1";
			textBox9.Text="0";
			textBox10.Text="1";
			textBox11.Text="0";
			
		}
		void LAP2ToolStripMenuItemClick(object sender, EventArgs e)
		{
			textBox3.Text="-1";
			textBox4.Text="-1";
			textBox5.Text="-1";
			textBox6.Text="-1";
			textBox7.Text="8";
			textBox8.Text="-1";
			textBox9.Text="-1";
			textBox10.Text="-1";
			textBox11.Text="-1";
		}
		void LAP3ToolStripMenuItemClick(object sender, EventArgs e)
		{
			textBox3.Text="-1";
			textBox4.Text="-1";
			textBox5.Text="-1";
			textBox6.Text="-1";
			textBox7.Text="9";
			textBox8.Text="-1";
			textBox9.Text="-1";
			textBox10.Text="-1";
			textBox11.Text="-1";
		}
		void LAP4ToolStripMenuItemClick(object sender, EventArgs e)
		{
			textBox3.Text="1";
			textBox4.Text="-2";
			textBox5.Text="1";
			textBox6.Text="-2";
			textBox7.Text="4";
			textBox8.Text="-2";
			textBox9.Text="1";
			textBox10.Text="-2";
			textBox11.Text="1";
			
		}
		void SobelNToolStripMenuItemClick(object sender, EventArgs e)
		{
			textBox3.Text="-1";
			textBox4.Text="-2";
			textBox5.Text="-1";
			textBox6.Text="0";
			textBox7.Text="0";
			textBox8.Text="0";
			textBox9.Text="1";
			textBox10.Text="2";
			textBox11.Text="1";
			
		}
		void SobelWToolStripMenuItemClick(object sender, EventArgs e)
		{
			textBox3.Text="-1";
			textBox4.Text="0";
			textBox5.Text="1";
			textBox6.Text="-2";
			textBox7.Text="0";
			textBox8.Text="2";
			textBox9.Text="-1";
			textBox10.Text="0";
			textBox11.Text="1";
		}
		void SobelNWToolStripMenuItemClick(object sender, EventArgs e)
		{
			textBox3.Text="-2";
			textBox4.Text="-2";
			textBox5.Text="0";
			textBox6.Text="-2";
			textBox7.Text="0";
			textBox8.Text="2";
			textBox9.Text="0";
			textBox10.Text="2";
			textBox11.Text="2";
			
		}
		void PrewittNToolStripMenuItemClick(object sender, EventArgs e)
		{
			textBox3.Text="-1";
			textBox4.Text="-1";
			textBox5.Text="-1";
			textBox6.Text="0";
			textBox7.Text="0";
			textBox8.Text="0";
			textBox9.Text="1";
			textBox10.Text="1";
			textBox11.Text="1";
		}
		void PrewittWToolStripMenuItemClick(object sender, EventArgs e)
		{
			textBox3.Text="-1";
			textBox4.Text="0";
			textBox5.Text="1";
			textBox6.Text="-1";
			textBox7.Text="0";
			textBox8.Text="1";
			textBox9.Text="-1";
			textBox10.Text="0";
			textBox11.Text="1";
		}
		void PrewittNWToolStripMenuItemClick(object sender, EventArgs e)
		{
			textBox3.Text="-2";
			textBox4.Text="-1";
			textBox5.Text="0";
			textBox6.Text="-1";
			textBox7.Text="0";
			textBox8.Text="1";
			textBox9.Text="0";
			textBox10.Text="1";
			textBox11.Text="2";
		}
		void CrucePorCerosToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				Bitmap salida=filtros.CrucePorCeros(bmp);
				pictureBox1.Image=salida;
				bmp=salida;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
			}
		}
		void SimpleToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				IsMoravec=false;
				filtros.Thresholding(bmp);
				pictureBox1.Image=bmp;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
			}
			else
			{
				IsMoravec=false;
				k=9;
				seq.Add(8);
			}
		}
		void EscalaDeGrisesToolStripMenuItem1Click(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				filtros.Gris(bmp);
				filtros.Multinivel(bmp,4);
				pictureBox1.Image=bmp;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
				IsGray=true;
			}
			else
			{
				k=10;
				IsGray=true;
				seq.Add(9);
			}
		}
		void ColorToolStripMenuItem2Click(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				
				filtros.Multinivel(bmp,4);
				pictureBox1.Image=bmp;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
				IsGray=false;
			}
			else
			{
				k=12;
				IsGray=false;
				seq.Add(11);
			}
		}
		void EscalaDeGrisesToolStripMenuItem2Click(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				filtros.Gris(bmp);
				filtros.Regiones(bmp,4,FromName(),over);
				pictureBox1.Image=bmp;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
			}
			else
			{
				k=13;
				seq.Add(12);
			}
		}
		void ColorToolStripMenuItem3Click(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				
				filtros.Regiones(bmp,4,FromName(),over);
				pictureBox1.Image=bmp;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
			}
			else
			{
				k=11;
				seq.Add(10);
			}
		}
		void HoughToolStripMenuItemClick(object sender, EventArgs e)
		{
			
		}
		void MoravecToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				IsMoravec=true;
				filtros.Moravec(bmp,umbral);
				pictureBox1.Image=bmp;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
			}
			else
			{
				IsMoravec=true;
				k=15;
				seq.Add(13);
			}
		}
		void CannyToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				filtros.Canny(bmp,(float)numericUpDown2.Value,(byte)numericUpDown5.Value,(byte)numericUpDown6.Value);
				pictureBox1.Image=bmp;

				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
			}
			else
			{
				k=16;
				seq.Add(14);
			}
		}
		void HarrisToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				filtros.Harris(bmp,(float)numericUpDown8.Value,(float)numericUpDown7.Value,(float)numericUpDown9.Value);
				pictureBox1.Image=bmp;

				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
				
			}
			else
			{
				k=18;
				seq.Add(16);
			}
			
		}
		#endregion
		private int Media(string s)
		{
			if(s.Contains(".mp4") || s.Contains(".avi") || s.Contains(".3gp") || s.Contains(".flv") || s.Contains(".mpg"))
			{
				return 1;
			}
			else if(s.Contains(".bmp") || s.Contains(".png") || s.Contains(".jpg") || s.Contains(".jpeg") || s.Contains(".tif") ||s.Contains(".pgm"))
			{
				return 0;
			}
			else{return -1;}
		}
		
		
		
		void ExtraerCanalLToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				COLOR.CIELAB.ExtraerL(bmp,color);
				pictureBox1.Image=bmp;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
				
			}
			else
			{
				k=19;
			}
			
		}
		
		bool color =true;
		
		void ExtraerCanalAToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				COLOR.CIELAB.Extraera(bmp,color);
				pictureBox1.Image=bmp;

				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
				
			}
			else{
				k=20;}
		}
		
		void ExtraerCanalBToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				COLOR.CIELAB.Extraerb(bmp,color);
				pictureBox1.Image=bmp;

				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
				
			}
			else{
				k=21;}
		}
		
		void PseudoColorToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!color)
			{
				color=true;
				pseudoColorToolStripMenuItem.Text="Pseudocolor(Activado)";
			}
			else
			{
				color=false;
				pseudoColorToolStripMenuItem.Text="Pseudocolor(Desactivado)";
			}
		}
		
		void ExtraerTodosToolStripMenuItemClick(object sender, EventArgs e)
		{
			
			if(!isVideo)
			{
				ImgShow l=new ImgShow(COLOR.CIELAB.GetLImage(bmp),"L");
				ImgShow a=new ImgShow(COLOR.CIELAB.GetaImage(bmp),"a");
				ImgShow b=new ImgShow(COLOR.CIELAB.GetbImage(bmp),"b");
			}
		}
		
		void OverlayToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				OpenFileDialog fd=new OpenFileDialog();
				if(fd.ShowDialog()==DialogResult.OK)
				{
					Bitmap b2=new Bitmap(fd.FileName);
					filtros.Overlay(bmp,b2);
					pictureBox1.Image=bmp;
				}
			}
		}
		private Color FromName()
		{
			Color c=Color.Green;
			switch(toolStripComboBox1.SelectedIndex)
			{
					case 0:{c=Color.Red;break;}
					case 1:{c=Color.Blue;break;}
					case 3:{c=Color.Yellow;break;}
					case 2:{c=Color.Green;break;}
					case 5:{c=Color.Orange;break;}
					case 4:{c=Color.Purple;break;}
					case 6:{c=Color.Violet;break;}
					case 8:{c=Color.White;break;}
					case 7:{c=Color.Gray;break;}
			}
			return c;
		}
		private Color c;
		private bool over=true;
		void SobrepuestoToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!over)
			{
				over=true;
				sobrepuestoToolStripMenuItem.Text="Overlay(Activo)";
			}
			else
			{
				over=false;
				sobrepuestoToolStripMenuItem.Text="Overlay(Desactivado)";
			}
		}
		
		void TransformadaToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				filtros.HoughT(bmp);
				pictureBox1.Image=bmp;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
			}
			else
			{
				k=17;
				seq.Add(15);
			}
		}
		
		void LineasToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				filtros.HoughL(bmp);
				pictureBox1.Image=bmp;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
			}
			else
			{
				k=23;
				seq.Add(21);
			}
		}
		
		void PropiedadesToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				Texturas.CalcSDH(bmp);
				meanText.Text=Texturas.media.ToString();
				energiaText.Text=Texturas.energia.ToString();
				entroText.Text=Texturas.entropia.ToString();
				homoText.Text=Texturas.homogeneidad.ToString();
				conText.Text=Texturas.contraste.ToString();
			}
			else
			{
				k=22;
				seq.Add(21);
			}
		}
		
		void ImagenesDePropiedadesToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				new ImgShow(Texturas.IH(bmp,20),"Homegeneidad");
				Application.DoEvents();
				new ImgShow(Texturas.IE(bmp,20),"Energia");
				Application.DoEvents();
				new ImgShow(Texturas.IEY(bmp,20),"Entropia");
				Application.DoEvents();
				new ImgShow(Texturas.IC(bmp,20),"Contraste");
				Application.DoEvents();
				new ImgShow(Texturas.IM(bmp,20),"Media");
				Application.DoEvents();
			}
		}
		
		void ClasificadorToolStripMenuItemClick(object sender, EventArgs e)
		{
			PTextures clasificador=new PTextures();
			clasificador.Show();
			
		}
		
		
		
		void ColorizarToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				COLOR.RGB.Colorizar(bmp);
				pictureBox1.Image=bmp;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
			}
			
		}
		
		void FaceDetectToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo){
				filtros.FaceDetect(bmp);
				pictureBox1.Image=bmp;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);}
			else{k=30;}
		}
		
		void TransformadaFourierToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(!isVideo)
			{
				FFT.FT(bmp);
				pictureBox1.Image=bmp;
				histogram1.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),0);
				histogram2.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),1);
				histogram3.Values= filtros.Histograma(new Bitmap(pictureBox1.Image),2);
				histogram4.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),3);
				histogram5.Values=filtros.Histograma(new Bitmap(pictureBox1.Image),4);
			}
			else{k=31;}
		}
	}
}
