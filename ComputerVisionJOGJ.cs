/*
 * Super clase para implementar los algoritmos vistos en clase de vision
 * tanto para imagenes como para video
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading.Tasks;


namespace vcJOGJ
{
	
	public class Imagen
	{
		public Imagen()
		{
			
		}
		#region Metodos Publicos que contienen los algoritmos para el procesamiento de Imagenes
		
		#region Negativo,Gris
		
		public void Negativo(Bitmap b)
		{
			BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int w=b.Width;
			int h=b.Height;
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - b.Width*3;

				for(int y=0;y<h;y++)
				{
					for(int x=0; x < w; ++x )
					{

						*(p+0)=(byte)(255-(*(p+0)));
						*(p+1)=(byte)(255-(*(p+1)));
						*(p+2)=(byte)(255-(*(p+2)));
						p += 3;
					}
					p += nOffset;
				}
			}

			b.UnlockBits(bData);
		}
		public void Gris(Bitmap b)
		{
			BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int w=b.Width;
			int h=b.Height;
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - b.Width*3;

				for(int y=0;y<h;y++)
				{
					for(int x=0; x < w; ++x )
					{

						*(p+0)=(*(p+1))=(*(p+2))=(byte)((*(p+0)*.11)+(*(p+1)*.59)+(*(p+2)*.30));
						
						p += 3;
					}
					p += nOffset;
				}
			}

			b.UnlockBits(bData);
		}
		#endregion
		
		#region Correlacion
		public void Correlation(ref Bitmap salida,Bitmap b,Bitmap temp,int type)
		{
			if(type==1)
			{
				
				WaitForm wf=new WaitForm();
				pg=wf.progressBar1;
				
				wf.Show();
				
				Bitmap src=b.Clone() as Bitmap;
				Bitmap tb=temp.Clone() as Bitmap;
				Gris(b);
				Gris(tb);
				float[,,] s=Utils.ToArray(salida);
				float[,,] t=Utils.ToArray(tb);
				float[,,] bi=Utils.ToArray(b);
				
				int w=b.Width;
				int h=b.Height;
				int wt=tb.Width;
				int ht=tb.Height;
				int z=0;
				float val=0;
				float c1=0;
				float c2=0;
				int Width=salida.Width;
				int Height=salida.Height;
				for(int i=0;i<Width;i++)
				{
					for (int j = 0; j < Height; j++)
					{
						
						for(int k=0;k<wt;k++)
						{
							for(int l=0;l<ht;l++)
							{
								
								
								val+=bi[i+k,j+l,0]*t[k,l,0];
								c1+=(float)Math.Pow(bi[(i+k),(j+l),0],2);
								c2+=(float)Math.Pow(t[k,l,0],2);
								
								
							}

						}
						z++;
						int p=(int)(Math.Abs((z*100)/(((Width-wt)*(Height-ht)))));
						if(p>100)p=100;
						pg.Value=p;
						val=((val/(c1*c2)));
						
						s[i,j,0]=s[i,j,1]=s[i,j,2]= val;
						val=0.0F;
						c1=0.0F;
						c2=0.0F;
					}
					
					
				}
				
				Utils.Ajustar(s);
				Utils.FromArray(s).Save("./MapaCorrelacion1.bmp");
				//salida=Utils.FromArray(s);
				float[,]map=Utils.GenMap(s);
				System.Drawing.Point []puntos=Utils.Find(0.95F,map);
				for(int i=0;i<puntos.Length;i++)
				{
					using(Graphics g=Graphics.FromImage(src))
					{
						Pen p=new Pen(Color.Red);
						Rectangle r=new Rectangle(puntos[i].X,puntos[i].Y,temp.Width,temp.Height);
						g.DrawRectangle(p,r);
					}
				}
				salida=src;
				
				wf.Close();

				
			}
			else
			{
				
				WaitForm wf=new WaitForm();
				pg=wf.progressBar1;
				
				wf.Show();
				Bitmap src=b.Clone() as Bitmap;
				Bitmap tb=temp.Clone() as Bitmap;
				Gris(b);
				Gris(tb);
				
				float[,,] s=Utils.ToArray(salida);
				float[,,] t=Utils.ToArray(tb);
				float[,,] bi=Utils.ToArray(b);
				double mB=Utils.Mean(b);
				double mT=Utils.Mean(tb);
				int w=b.Width;
				int h=b.Height;
				int wt=temp.Width;
				int ht=temp.Height;
				int z=0;
				float val=0;
				float c1=0;
				float c2=0;

				int Width=salida.Width;
				int Height=salida.Height;
				for(int i=0;i<Width;i++)
				{
					for (int j = 0; j <Height; j++)
					{
						
						for(int k=0;k<wt;k++)
						{
							for(int l=0;l<ht;l++)
							{
								
								
								val+=(float)((bi[i+k,j+l,0]-mB)*(t[k,l,0]-mT));
								c1+=(float)Math.Pow((bi[(i+k),(j+l),0]-mB),2);
								c2+=(float)Math.Pow((t[k,l,0]-mT),2);
								
								
							}

						}
						z++;
						int p=(int)(Math.Abs((z*100)/(((Width-wt)*(Height-ht)))));
						if(p>100)p=100;
						pg.Value=p;
						
						val=(float)((val/Math.Sqrt((c1*c2))));
						
						s[i,j,0]=s[i,j,1]=s[i,j,2]= val;
						val=0.0F;
						c1=0.0F;
						c2=0.0F;
					}
					
					
				}
				
				Utils.Ajustar2(s);
				Utils.FromArray(s).Save("./MapaCorrelacion2.bmp");
				//salida=Utils.FromArray(s);
				float[,]map=Utils.GenMap(s);
				System.Drawing.Point []puntos=Utils.Find(0.95F,map);
				for(int i=0;i<puntos.Length;i++)
				{
					using(Graphics g=Graphics.FromImage(src))
					{
						Pen p=new Pen(Color.Red);
						Rectangle r=new Rectangle(puntos[i].X,puntos[i].Y,temp.Width,temp.Height);
						g.DrawRectangle(p,r);
					}
				}
				salida=src;
				
				wf.Close();
			}
			
		}
		/*public void Correlation(ref double [,]m,double[,]f,double[,]g)
		{
			
			int w=m.GetUpperBound(0)+1;
			int h=m.GetUpperBound(1)+1;
			int wt=g.GetUpperBound(0)+1;
			int ht=g.GetUpperBound(1)+1;
			int w2=f.GetUpperBound(0);
			int h2=f.GetUpperBound(1);

			double val=0;
			Parallel.For(0,w,delegate(int i)
			{
				
				for(int j=0;j<h;j++)
				{
					if(j>h2)break;
					val=0;
					for(int k=0;k<wt;k++)
					{
						
						for(int l=0;l<ht;l++)
						{
							
							if(((i+k)>w2) || ((j+l)>h2))continue;
							val+=(f[i+k,j+l]*g[k,l]);
							
						}

					}
					m[i,j] = val;
					
				}
			             });
			Save(m);
			
		}*/
		public void Correlation(ref Bitmap salida,Bitmap b,ref double [,]filtro)
		{
			Gris(salida);
			Gris(b);
			int wt=filtro.GetUpperBound(0)+1;
			int ht=filtro.GetUpperBound(1)+1;
			int w=b.Width;
			int h=b.Height;
			float[,,] s=Utils.ToArray(salida);
			float[,,] bs=Utils.ToArray(b);
			double val=0.0F;
			float mean=(float)(Math.Pow(1,-1));
			for(int i=0;i<w-wt;i++)
			{
				for(int j=0;j<h-ht;j++)
				{
					for(int k=0;k<wt;k++)
					{
						for(int l=0;l<ht;l++)
						{
							val+=((bs[i+k,j+l,0]*filtro[k,l]*mean));
						}
					}
					
					s[i,j,0]=s[i,j,1]=s[i,j,2]=(float)val;
					val=0.0F;
				}
			}
			//	Save(s);
			Utils.Ajustar(s);
			
		}
		#endregion
		
		#region Convolucion
		public void Convolution(Bitmap input,float [,]filter,int offset)
		{
			
			int width=input.Width;
			int height=input.Height;
			int wt=filter.GetLength(0);
			int ht=filter.GetLength(1);
			int xMiddle = filter.GetLength(0) >>1;
			int yMiddle = filter.GetLength(1) >>1;
			BitmapData data=input.LockBits(new Rectangle(0,0,width,height),ImageLockMode.ReadWrite,PixelFormat.Format24bppRgb);
			Bitmap copy=input.Clone() as Bitmap;
			BitmapData data2=copy.LockBits(new Rectangle(0,0,width,height),ImageLockMode.ReadWrite,PixelFormat.Format24bppRgb);
			paso = data.Stride;
			imageData=data2.Scan0;
			int Width=input.Width;
			int Height=input.Height;
			IntPtr Scan0=data.Scan0;
			unsafe
			{
				
				Parallel.For(0,width,delegate(int x)
				             {
				             	for (int y = 0; y < height; y++)
				             	{
				             		float r = 0.0f;
				             		float g = 0.0f;
				             		float b = 0.0f;

				             		for (int xFilter = 0; xFilter < wt; xFilter++)
				             		{
				             			for (int yFilter = 0; yFilter < ht; yFilter++)
				             			{
				             				int x0 = x - xMiddle + xFilter;
				             				int y0 = y - yMiddle + yFilter;

				             				if (x0 >= 0 && x0 < Width &&
				             				    y0 >= 0 && y0 < Height)
				             				{
				             					byte *clr= GetPixel(x0, y0);

				             					r += *(clr+2) * filter[xFilter, yFilter];
				             					g += *(clr+1) * filter[xFilter, yFilter];
				             					b += *(clr+0) * filter[xFilter, yFilter];
				             				}
				             			}
				             		}
				             		

				             		if (r > 255)
				             			r = 255;
				             		if (g > 255)
				             			g = 255;
				             		if (b > 255)
				             			b = 255;

				             		if (r < 0)
				             			r = 0;
				             		if (g < 0)
				             			g = 0;
				             		if (b < 0)
				             			b = 0;
				             		SetPixel(Scan0,x,y,(byte)r,(byte)g,(byte)b);
				             	}
				             	
				             });
			}
			input.UnlockBits(data);
			copy.UnlockBits(data2);
		}
		private float[,,] Convolution(Bitmap bmp,float[,]filter)
		{
			int width=bmp.Width;
			int height=bmp.Height;
			int wt=filter.GetLength(0);
			int ht=filter.GetLength(1);
			int xMiddle = filter.GetLength(0) >> 2;
			int yMiddle = filter.GetLength(1) >> 2;

			float[,,] bf=Utils.ToArray(bmp);
			float[,,] output = new float[width,height,3];


			Parallel.For(0,width,delegate(int x)
			             {
			             	for (int y = 0; y < height; y++)
			             	{
			             		float r = 0.0F;
			             		float g = 0.0F;
			             		float b = 0.0F;

			             		for (int xFilter = 0; xFilter < wt; xFilter++)
			             		{
			             			for (int yFilter = 0; yFilter < ht; yFilter++)
			             			{
			             				int x0 = x - xMiddle + xFilter;
			             				int y0 = y - yMiddle + yFilter;

			             				if (x0 >= 0 && x0 < width &&
			             				    y0 >= 0 && y0 < height)
			             				{
			             					
			             					r +=  (bf[x0,y0,0] * filter[xFilter, yFilter]);
			             					g +=  (bf[x0,y0,1]* filter[xFilter, yFilter]);
			             					b +=   (bf[x0,y0,2] * filter[xFilter, yFilter]);
			             				}
			             			}
			             		}
			             		output[x,y,0]=r;
			             		output[x,y,1]=g;
			             		output[x,y,2]=b;

			             	}
			             });

			return output;
		}
		private float[,] FC(Bitmap b,float[,] filter)
		{
			int width=b.Width;
			int height=b.Height;
			int wt=filter.GetLength(0);
			int ht=filter.GetLength(1);
			int xMiddle = filter.GetLength(0) >> 2;
			int yMiddle = filter.GetLength(1) >> 2;

			float[,] output = new float[width,height];

			BitmapData data=b.LockBits(new Rectangle(0,0,width,height),ImageLockMode.ReadWrite,PixelFormat.Format24bppRgb);
			imageData=data.Scan0;
			paso=data.Stride;
			unsafe
			{
				Parallel.For(0,width,delegate(int x)
				             {
				             	for (int y = 0; y < height; y++)
				             	{
				             		float r = 0.0F;
				             		
				             		for (int xFilter = 0; xFilter < wt; xFilter++)
				             		{
				             			for (int yFilter = 0; yFilter < ht; yFilter++)
				             			{
				             				int x0 = x - xMiddle + xFilter;
				             				int y0 = y - yMiddle + yFilter;

				             				if (x0 >= 0 && x0 < width &&
				             				    y0 >= 0 && y0 < height)
				             				{
				             					
				             					r +=  (GetPixel(x0,y0)[0] * filter[xFilter, yFilter]);
				             					
				             				}
				             			}
				             		}
				             		output[x,y]=r;
				             		

				             	}
				             });
			}
			b.UnlockBits(data);
			return output;
		}
		private float[,] FC(float[,] b,float[,] filter)
		{
			int width=b.GetLength(0);
			int height=b.GetLength(1);
			int wt=filter.GetLength(0);
			int ht=filter.GetLength(1);
			int xMiddle = filter.GetLength(0) >> 2;
			int yMiddle = filter.GetLength(1) >> 2;

			float[,] output = new float[width,height];

			Parallel.For(0,width,delegate(int x)
			             {
			             	for (int y = 0; y < height; y++)
			             	{
			             		float r = 0.0F;
			             		
			             		for (int xFilter = 0; xFilter < wt; xFilter++)
			             		{
			             			for (int yFilter = 0; yFilter < ht; yFilter++)
			             			{
			             				int x0 = x - xMiddle + xFilter;
			             				int y0 = y - yMiddle + yFilter;

			             				if (x0 >= 0 && x0 < width &&
			             				    y0 >= 0 && y0 < height)
			             				{
			             					
			             					r +=  (b[x0,y0]* filter[xFilter, yFilter]);
			             					
			             				}
			             			}
			             		}
			             		output[x,y]=r;
			             		

			             	}
			             });
			return output;
		}
		public void FastConvolution(Bitmap b,float [,]filter,int offset)
		{
			float TopLeft, TopMid, TopRight;
			float MidLeft, Pixel , MidRight ;
			float BottomLeft, BottomMid, BottomRight;
			float Factor=1;
			float Offset=(float)offset;
			TopLeft=filter[0,0];
			TopMid=filter[0,1];
			TopRight=filter[0,2];
			MidLeft=filter[1,0];
			Pixel=filter[1,1];
			MidRight=filter[1,2];
			BottomLeft=filter[2,0];
			BottomMid=filter[2,1];
			BottomRight=filter[2,2];
			Bitmap bSrc = (Bitmap)b.Clone();
			BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			int stride2 = stride * 2;
			System.IntPtr Scan0 = bmData.Scan0;
			System.IntPtr SrcScan0 = bmSrc.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;
				byte * pSrc = (byte *)(void *)SrcScan0;

				int nOffset = stride + 6 - b.Width*3;
				int nWidth = b.Width - 2;
				int nHeight = b.Height - 2;

				double nPixel;

				for(int y=0;y<nHeight;y++)
				{
					for(int x=0; x < nWidth; ++x )
					{
						nPixel = ( ( ( (pSrc[2] * TopLeft) + (pSrc[5] * TopMid) + (pSrc[8] * TopRight) +
						              (pSrc[2 + stride] * MidLeft) + (pSrc[5 + stride] * Pixel) + (pSrc[8 + stride] * MidRight) +
						              (pSrc[2 + stride2] * BottomLeft) + (pSrc[5 + stride2] * BottomMid) + (pSrc[8 + stride2] * BottomRight)) / Factor) + Offset);

						if (nPixel < 0) nPixel = 0;
						if (nPixel > 255) nPixel = 255;

						p[5 + stride]= (byte)nPixel;

						nPixel = ( ( ( (pSrc[1] * TopLeft) + (pSrc[4] * TopMid) + (pSrc[7] * TopRight) +
						              (pSrc[1 + stride] * MidLeft) + (pSrc[4 + stride] * Pixel) + (pSrc[7 + stride] * MidRight) +
						              (pSrc[1 + stride2] * BottomLeft) + (pSrc[4 + stride2] * BottomMid) + (pSrc[7 + stride2] * BottomRight)) / Factor) + Offset);

						if (nPixel < 0) nPixel = 0;
						if (nPixel > 255) nPixel = 255;
						
						p[4 + stride] = (byte)nPixel;

						nPixel = ( ( ( (pSrc[0] * TopLeft) + (pSrc[3] * TopMid) + (pSrc[6] * TopRight) +
						              (pSrc[0 + stride] * MidLeft) + (pSrc[3 + stride] * Pixel) + (pSrc[6 + stride] * MidRight) +
						              (pSrc[0 + stride2] * BottomLeft) + (pSrc[3 + stride2] * BottomMid) + (pSrc[6 + stride2] * BottomRight)) / Factor) + Offset);

						if (nPixel < 0) nPixel = 0;
						if (nPixel > 255) nPixel = 255;
						
						p[3 + stride] = (byte)nPixel;

						p += 3;
						pSrc += 3;
					}

					p += nOffset;
					pSrc += nOffset;
				}
			}
			
			b.UnlockBits(bmData);
			bSrc.UnlockBits(bmSrc);
		}
		#endregion
		
		#region Operaciones sobre Histograma
		public void EcualizarHistograma(Bitmap b)
		{
			
			int []rh=new int[256];
			int []gh=new int[256];
			int []bh=new int[256];
			
			
			BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int w=b.Width;
			int h=b.Height;
			int pixels=w*h;
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;
			
			unsafe
			{
				
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - b.Width*3;

				for(int y=0;y<h;y++)
				{
					for(int x=0; x < w; ++x )
					{

						bh[*(p+0)]++;
						gh[*(p+1)]++;
						rh[*(p+2)]++;
						p += 3;
					}
					p += nOffset;
				}
				
				rh=Utils.Ecualizar(rh,pixels);
				gh=Utils.Ecualizar(gh,pixels);
				bh=Utils.Ecualizar(bh,pixels);
				
				p = (byte *)(void *)Scan0;
				for(int y=0;y<h;y++)
				{
					for(int x=0; x < w; ++x )
					{

						*(p+0)=(byte)bh[*(p+0)];
						*(p+1)=(byte)gh[*(p+1)];
						*(p+2)=(byte)rh[*(p+2)];
						p += 3;
					}
					p += nOffset;
				}
			}
			
			b.UnlockBits(bData);
			
		}
		public void ExpandirHistograma(Bitmap b,int min, int max)
		{
			
			float minx = float.MaxValue;
			float maxx = float.MinValue;

			float coeff=0;
			
			BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int w=b.Width;
			int h=b.Height;
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - b.Width*3;

				Parallel.For(0,h,delegate(int y)
				             {
				             	for(int x=0; x < w; ++x )
				             	{

				             		if(minx>(int)(*(p+2)))
				             		{
				             			minx=(int)(*(p+2));
				             		}
				             		if(maxx<(int)(*(p+2)))
				             		{
				             			maxx=(int)(*(p+2));
				             		}
				             		p += 3;
				             	}
				             	p += nOffset;
				             });
			}

			coeff=(float)((max-min)/(maxx-minx));
			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - b.Width*3;

				for(int y=0;y<h;y++)
				{
					for(int x=0; x < w; ++x )
					{

						*(p+0)=(*(p+1))=(*(p+2))=(byte)(( (p[2] - minx) *coeff )+min);
						p += 3;
					}
					p += nOffset;
				}
			}

			b.UnlockBits(bData);
		}
		public void ExpandirHistograma(Bitmap b,int rmin,int rmax,int gmin,int gmax,int bmin, int bmax)
		{
			float minr = 255;
			float maxr = 0;
			float ming = 255;
			float maxg = 0;
			float minb = 255;
			float maxb = 0;
			
			float coeffr=0;
			float coeffg=0;
			float coeffb=0;
			
			BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int w=b.Width;
			int h=b.Height;
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - b.Width*3;

				Parallel.For(0,h,delegate(int y)
				             {
				             	for(int x=0; x < w; ++x )
				             	{

				             		if(minr>(int)(*(p+2)))
				             		{
				             			minr=(int)(*(p+2));
				             		}
				             		if(maxr<(int)(*(p+2)))
				             		{
				             			maxr=(int)(*(p+2));
				             		}
				             		if(ming>(int)(*(p+1)))
				             		{
				             			ming=(int)(*(p+1));
				             		}
				             		if(maxg<(int)(*(p+1)))
				             		{
				             			maxg=(int)(*(p+1));
				             		}
				             		if(minb>(int)(*(p+0)))
				             		{
				             			minb=(int)(*(p+0));
				             		}
				             		if(maxb<(int)(*(p+0)))
				             		{
				             			maxb=(int)(*(p+0));
				             		}
				             		p += 3;
				             	}
				             	p += nOffset;
				             });
			}

			coeffr=(float)((rmax-rmin)/(maxr-minr));
			coeffg=(float)((gmax-gmin)/(maxg-ming));
			coeffb=(float)((bmax-bmin)/(maxb-minb));
			
			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - b.Width*3;

				Parallel.For(0,h,delegate(int y)
				             {
				             	for(int x=0; x < w; ++x )
				             	{

				             		*(p+0)=(byte)(( (p[0] - minb) *coeffb )+bmin);
				             		*(p+1)=(byte)(( (p[1] - ming) *coeffg )+gmin);
				             		*(p+2)=(byte)(( (p[2] - minr) *coeffr )+rmin);
				             		p += 3;
				             	}
				             	p += nOffset;
				             });
			}

			b.UnlockBits(bData);
		}
		public double[] Histograma(Bitmap bmp,int channel)
		{
			double []histo=new double[256];
			double []rh=new double[256];
			double []gh=new double[256];
			double []bh=new double[256];
			
			Bitmap b=bmp.Clone() as Bitmap;
			BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;
			int w=b.Width;
			int h=b.Height;
			unsafe
			{
				
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - b.Width*3;

				Parallel.For(0,h,delegate(int y)
				             {
				             	for(int x=0; x < w; ++x )
				             	{

				             		bh[*(p+0)]++;
				             		gh[*(p+1)]++;
				             		rh[*(p+2)]++;
				             		
				             		p += 3;
				             	}
				             	p += nOffset;
				             });
				
				
			}
			
			b.UnlockBits(bData);

			switch(channel)
			{
				case 0:
					{
						histo=rh;
						break;
					}
					
				case 1:
					{
						histo=gh;
						break;
					}
					
				case 2:
					{
						histo=bh;
						break;
					}
				case 3:
					{
						histo=CumulativeHistogram(b);
						break;
					}
				case 4:
					{
						histo=GreyHistogram(b);
						break;
					}
			}
			return histo;

		}
		#endregion
		
		#region Umbralizaciones
		public void Thresholding(Bitmap b,int umbral)
		{
			Gris(b);

			BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int w=b.Width;
			int h=b.Height;
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - b.Width*3;

				for(int y=0;y<h;y++)
				{
					for(int x=0; x < w; ++x )
					{

						if(*(p+0)>umbral)
						{
							*(p+0)=(*(p+1))=(*(p+2))=(byte)255;
						}
						else
						{
							*(p+0)=(*(p+1))=(*(p+2))=(byte)0;
						}
						
						p += 3;
					}
					p += nOffset;
				}
			}

			b.UnlockBits(bData);
			
		}
		public void Thresholding(Bitmap b)
		{
			Gris(b);
			double umbral=Utils.Mean(b);
			BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int w=b.Width;
			int h=b.Height;
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - b.Width*3;

				for(int y=0;y<h;y++)
				{
					for(int x=0; x < w; ++x )
					{

						if(*(p+0)>umbral)
						{
							*(p+0)=(*(p+1))=(*(p+2))=(byte)255;
						}
						else
						{
							*(p+0)=(*(p+1))=(*(p+2))=(byte)0;
						}
						
						p += 3;
					}
					p += nOffset;
				}
			}

			b.UnlockBits(bData);
			
		}
		public void Multinivel(Bitmap b,int levels)
		{
			BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int w=b.Width;
			int h=b.Height;
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;
			byte []map=GenMapLevels(levels);
			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - b.Width*3;

				for(int y=0;y<h;y++)
				{
					for(int x=0; x < w; ++x )
					{

						*(p+0)=map[*(p+0)];
						*(p+1)=map[*(p+1)];
						*(p+2)=map[*(p+2)];
						p += 3;
					}
					p += nOffset;
				}
			}

			b.UnlockBits(bData);
		}
		public void Regiones(Bitmap b,int regiones,Color c,bool over=false)
		{
			
			Multinivel(b,regiones);
			Bitmap copy=b.Clone() as Bitmap;
			if(over)ClearImage(b);
			BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			BitmapData bData2 = copy.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int w=b.Width;
			int h=b.Height;
			paso = bData.Stride;
			IntPtr Scan0 = bData.Scan0;
			imageData=bData2.Scan0;
			unsafe
			{
				
				Parallel.For(0,h-1,delegate(int y)
				             {
				             	for(int x=0; x < w-1; ++x )
				             	{

				             		if(!ComparePixels(GetPixel(x,y),GetPixel(x,y+1))||!ComparePixels(GetPixel(x,y),GetPixel(x+1,y)))
				             		{
				             			SetPixel(Scan0,x,y,c.R,c.G,c.B);
				             		}
				             		
				             	}

				             });
			}

			b.UnlockBits(bData);
			copy.UnlockBits(bData2);
		}

		#endregion
		
		public void FaceDetect(Bitmap bs)
		{
			BitmapData bData = bs.LockBits(new Rectangle(0, 0, bs.Width, bs.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int w=bs.Width;
			int h=bs.Height;
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;
			byte max,min;
			max=byte.MinValue;
			min=byte.MaxValue;
			
			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - bs.Width*3;

				for(int y=0;y<h;y++)
				{
					for(int x=0; x < w; ++x )
					{
						byte r=*(p+2);
						byte g=*(p+1);
						byte b=*(p+0);
						if (r > max) max = r; if (g > max) max = g;
						if (b > max) max = b; if (r < min) min = r;
						if (g < min) min = g; if (b < min) min = b;

						if ((r > 95 && g > 40 && b > 20) && (Math.Abs(r - g) > 15) && ((r > g) && (r > b)) && ((max - min) > 15) && (r != 0 && g != 0 && b != 0))
						{
							
						}
						else
						{
							
							*(p+0)=*(p+1)=*(p+2)=0;
							
						}

						p += 3;
					}
					p += nOffset;
				}
			}

			bs.UnlockBits(bData);
		}
		public void Overlay(Bitmap b,Bitmap a)
		{
			
			BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			BitmapData bData2 = a.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int w=b.Width;
			int h=b.Height;
			int stride=bData.Stride;
			IntPtr Scan0 = bData.Scan0;
			IntPtr Scan02= bData2.Scan0;
			unsafe
			{
				
				byte * p = (byte *)(void *)Scan0;
				byte * p2= (byte *)(void *)Scan02;
				int nOffset = stride - b.Width*3;

				for(int y=0;y<h;y++)
				{
					for(int x=0; x < w; ++x )
					{

						
						*(p+0)=(byte)(*(p+0)|*(p2+0));
						*(p+1)=(byte)(*(p+1)|*(p2+1));
						*(p+2)=(byte)(*(p+2)|*(p2+2));
						
						p += 3;
						p2 += 3;
					}
					p += nOffset;
					p2 +=nOffset;
				}
			}

			b.UnlockBits(bData);
			a.UnlockBits(bData2);
		}
		
		#region Deteccion de Rectas,Bordes,Esquinas
		public Bitmap CrucePorCeros(Bitmap b)
		{
			Gris(b);
			Bitmap salida=new Bitmap(b.Width,b.Height);
			Gris(salida);
			float[,] laplaciano=
			{{-1,-1,-1},
				{-1,8,-1},
				{-1,-1,-1}};
			float[,,] s=Convolution(b,laplaciano);
			float[,,] final=CheckVecinos(s);
			salida=Utils.FromArray(final);
			return salida;
		}
		public void HoughL(Bitmap b)
		{
			Thresholding(b);
			GenLUT();
			BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int w=b.Width;
			int h=b.Height;
			int halfWidth   = w / 2;
			int halfHeight  = h / 2;

			int offset = bData.Stride - bData.Width*3;

			int halfHoughWidth = (int) Math.Sqrt( halfWidth * halfWidth + halfHeight * halfHeight );
			int houghWidth = halfHoughWidth * 2;
			int houghHeight=180;
			HoughMap = new short[houghHeight, houghWidth];

			int startX = -halfWidth;
			int startY = -halfHeight;
			int stopX  = w  - halfWidth ;
			int stopY  = h - halfHeight;
			unsafe
			{
				byte* src = (byte*) bData.Scan0;

				
				for ( int y = startY; y <stopY; y++ )
				{
					
					for ( int x = startX; x < stopX; x++, src+=3)
					{
						if ( *(src+0) != 0  && *(src+1) != 0  && *(src+2) != 0 )
						{
							
							for ( int theta = 0; theta < houghHeight; theta++ )
							{
								int radius = (int)( CosMap[theta] * (x) - SinMap[theta] * (y) )+halfHoughWidth;

								if ( ( radius < 0 ) || ( radius >= houghWidth ) )
									continue;

								HoughMap[theta, radius]++;
							}
						}
					}
					src += offset;
				}
			}
			b.UnlockBits(bData);
			
			BitmapData bData2 = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			HoughLine []H=GetLines(0.90F);
			System.Windows.Forms.MessageBox.Show(H.Length.ToString());
			foreach(HoughLine line in H)
			{
				int    r = line.Radius;
				double t = line.Theta;

				// se verifica que la linea esta en una parte mas abajo de la imagen
				if ( r < 0 )
				{
					t += 180;
					r = -r;
				}

				// convertimos los grados a radianes
				t = ( t / 180 ) * Math.PI;

				//transformamos coordenadas obteniendo como centro el centro
				//mismo de la imagen
				int w2 = w /2;
				int h2 = h / 2;

				float x0 = 0.0F, x1 = 0.0F, y0 = 0.0F, y1 = 0.0F;

				if ( line.Theta != 0 )
				{
					// linea no vertical
					x0 = -w2; // entonces es el punto mas a la izquierda
					x1 = w2;  // entonces es el punto mas a la derecha

					// calculamos los valores de y, transformando coordenadas
					y0 = (float)(( -Math.Cos( t ) * x0 + r ) / Math.Sin( t ));
					y1 = (float)(( -Math.Cos( t ) * x1 + r ) / Math.Sin( t ));
				}
				else
				{
					// para lineas verticales
					x0 = line.Radius;
					x1 = line.Radius;

					y0 = h2;
					y1 = -h2;
				}
				Drawing.DrawLine(bData2,new PointF(x0,y0),new PointF(x1,y1),Color.Green);
			}
			b.UnlockBits(bData2);
		}
		public void HoughT(Bitmap b)
		{
			Thresholding(b);
			GenLUT();
			BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int w=b.Width;
			int h=b.Height;
			int halfWidth   = w / 2;
			int halfHeight  = h / 2;

			int offset = bData.Stride - bData.Width*3;

			int halfHoughWidth = (int) Math.Sqrt( halfWidth * halfWidth + halfHeight * halfHeight );
			int houghWidth = halfHoughWidth * 2;
			int houghHeight=180;
			HoughMap = new short[houghHeight, houghWidth];

			int startX = -halfWidth;
			int startY = -halfHeight;
			int stopX  = w  - halfWidth ;
			int stopY  = h - halfHeight;
			unsafe
			{
				byte* src = (byte*) bData.Scan0;

				
				Parallel.For(startY,stopY,delegate(int y)
				             {
				             	
				             	for ( int x = startX; x < stopX; x++, src+=3)
				             	{
				             		if ( *(src+0) != 0  && *(src+1) != 0  && *(src+2) != 0 )
				             		{
				             			
				             			for ( int theta = 0; theta < houghHeight; theta++ )
				             			{
				             				int radius = (int)( CosMap[theta] * (x) - SinMap[theta] * (y) )+halfHoughWidth;

				             				if ( ( radius < 0 ) || ( radius >= houghWidth ) )
				             					continue;

				             				HoughMap[theta, radius]++;
				             			}
				             		}
				             	}
				             	src += offset;
				             });
			}
			b.UnlockBits(bData);
			short[] minmax=Utils.MinMax(HoughMap);
			
			ClearImage(b);
			
			float n=h/10.0f;
			
			using(Graphics g=Graphics.FromImage(b))
			{
				g.DrawImage(Utils.Resize(Utils.DrawMap(HoughMap),w,180),0.0f,n);
			}
			
		}
		public void Canny(Bitmap b,float sigma,byte LowThreshold,byte HighThreshold)
		{
			int w=b.Width;
			int h=b.Height;
			float [,]gx=new float[w,h];
			float [,]gy=new float[w,h];
			float [,]map=new float[w,h];
			byte  []angles=new byte[w*h];
			int k=0;
			double toAngle = 180.0 / Math.PI;
			float[,]Kernel=Utils.Kernel2DV(sigma);
			Convolution(b,Kernel,0);
			Thresholding(b);
			BitmapData data=b.LockBits(new Rectangle(0,0,b.Width,b.Height),ImageLockMode.ReadWrite,PixelFormat.Format24bppRgb);
			paso=data.Stride;
			imageData=data.Scan0;
			unsafe{
				Parallel.For(0,w-1,delegate(int x)
				             {
				             	for(int y=0;y<h-1;y++,k++)
				             	{
				             		byte a=GetPixel(x,y+1)[0];
				             		byte t=GetPixel(x,y)[0];
				             		byte c=GetPixel(x+1,y+1)[0];
				             		byte d=GetPixel(x+1,y)[0];
				             		gx[x,y]=((a-t)+(c-d));
				             		gy[x,y]=((t-d)+(a-c));
				             		map[x,y]=Utils.Distance(gx[x,y],gy[x,y]);
				             		double val=Math.Atan2(gy[x,y],gx[x,y])*toAngle;
				             		if ( val < 22.5 )
				             		{
				             			val = 0;
				             		}
				             		else if ( val < 67.5 )
				             		{
				             			val = 45;
				             		}
				             		else if (val < 112.5 )
				             		{
				             			val = 90;
				             		}
				             		else if ( val < 157.5 )
				             		{
				             			val = 135;
				             		}
				             		else
				             		{
				             			val = 0;
				             		}
				             		
				             		angles[k]=(byte)val;
				             	}

				             });
			}
			b.UnlockBits(data);
			byte[,]nomax=NoMax(map,angles);
			byte[,]hy=Hysteresis(nomax,LowThreshold,HighThreshold);
			BitmapData data2=b.LockBits(new Rectangle(0,0,b.Width,b.Height),ImageLockMode.ReadWrite,PixelFormat.Format24bppRgb);
			paso=data2.Stride;
			
			for(int x=0;x<w; x++)
			{
				for(int y=0;y<h;y++)
				{
					SetPixel(data2.Scan0,x,y,hy[x,y],hy[x,y],hy[x,y]);
				}
			}
			b.UnlockBits(data2);
		}
		public void Moravec(Bitmap b,int umbral)
		{
			
			int w=b.Width;
			int h=b.Height;
			BitmapData data=b.LockBits(new Rectangle(0,0,w,h),ImageLockMode.ReadWrite,PixelFormat.Format24bppRgb);
			imageData=data.Scan0;
			paso=data.Stride;

			float color=0.0f;
			float[,]map=new float[w,h];
			
			unsafe
			{
				Parallel.For(1,w-2,delegate(int i)
				             {
				             	
				             	for(int j=1;j<h-2;j++)
				             	{
				             		
				             		for(int k=-1;k<2;k++)
				             		{
				             			for(int l=-1;l<2;l++)
				             			{
				             				
				             				color+=GetPixel(i+k,j+l)[0]-GetPixel(i,j)[0];
				             				
				             			}
				             			
				             		}
				             		map[i,j]=((1.0f/8.0f)*color);
				             		color=0.0f;
				             	}
				             });

			}
			b.UnlockBits(data);
			float t=(umbral*Utils.MinMax(map)[1])/255;
			System.Drawing.Point[] moravec=Utils.Find(t,map);
			BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			for(int i=0;i<moravec.Length;i++)
			{
				Drawing.DrawRectangles(bData,new Rectangle(moravec[i].X,moravec[i].Y,3,3),Color.Red);
			}
			b.UnlockBits(bData);
			
			
			
		}
		public void Harris(Bitmap b,float k,float sigma,float umbral)
		{

			int width=b.Width;
			int height=b.Height;
			
			//kernel para encontrar derivadas en x y y
			float[,] filtrox={{-1, 0, 1}};
			float[,] filtroy={{-1},{ 0},{ 1}};
			
			float [,]gxy=new float[width,height];
			float [,]HarrisMap=new float[width,height];
			float[,]tempx=new float[width,height];
			float[,]tempy=new float[width,height];
			float[,]tempxy=new float[width,height];
			
			//kernel Gaussiano de tamño 3sigma
			float[,] Kernel=Utils.Kernel2D(sigma);
			
			int wt=Kernel.GetLength(0);
			int ht=Kernel.GetLength(1);
			int xMiddle = Kernel.GetLength(0) >> 2;
			int yMiddle = Kernel.GetLength(1) >> 2;
			float [,]img=Utils.ToMap(b);
			
			//Convolucion para encontrar derivadas
			float [,]gx=FC(img,filtrox);
			float [,]gy=FC(img,filtroy);
			

			//Cuadrado de las derivadas
			Parallel.For(0,width,delegate(int x)
			             {
			             	for(int y=0;y<height;y++)
			             	{
			             		gxy[x,y]=gx[x,y]*gy[x,y];
			             		gy[x,y]=gy[x,y]*gy[x,y];
			             		gx[x,y]=gx[x,y]*gx[x,y];

			             	}
			             });
			
			
			Array.Copy(gx,tempx,gx.Length);
			Array.Copy(gy,tempy,gy.Length);
			Array.Copy(gxy,tempxy,gxy.Length);
			
			//Rapida convulucion con un kernel gaussiano
			Parallel.For(0,width,delegate(int x)
			             {
			             	for (int y = 0; y < height; y++)
			             	{
			             		float r = 0.0F;
			             		float g = 0.0F;
			             		float z = 0.0F;
			             		
			             		for (int xFilter = 0; xFilter < wt; xFilter++)
			             		{
			             			for (int yFilter = 0; yFilter < ht; yFilter++)
			             			{
			             				int x0 = x - xMiddle + xFilter;
			             				int y0 = y - yMiddle + yFilter;

			             				if (x0 >= 0 && x0 < width &&
			             				    y0 >= 0 && y0 < height)
			             				{
			             					
			             					r +=  (tempx[x0,y0]* Kernel[xFilter, yFilter]);
			             					g +=  (tempy[x0,y0]* Kernel[xFilter, yFilter]);
			             					z +=  (tempxy[x0,y0]* Kernel[xFilter, yFilter]);
			             				}
			             			}
			             		}
			             		gx[x,y]=r;
			             		gy[x,y]=g;
			             		gxy[x,y]=z;
			             		

			             	}
			             });
			
			//Calculo de la funcion R
			for(int x=0;x<width;x++)
			{
				for(int y=0;y<height;y++)
				{
					float A=gx[x,y];
					float B=gy[x,y];
					float C=gxy[x,y];
					
					float R= (A * B - C * C) - (k * ((A + B) * (A + B)));
					if(R>umbral)HarrisMap[x,y]=R;
				}
			}
			List<PointF> corners=new List<PointF>();

			//Supresion de no maximos en una venta de inspeccion de 3x3, para verificar vecinos
			int maxY=height-3;
			for(int y=3;y<maxY;y++)
			{
				for (int x = 3, maxX = width - 3; x < maxX; x++)
				{
					float val = HarrisMap[x, y];

					for (int i = -3; (val != 0) && (i <= 3); i++)
					{

						for (int j = -3; j <= 3; j++)
						{
							if (HarrisMap[x + i, y + j] > val)
							{
								val = 0;
								break;
							}
						}
					}

					if (val != 0)
					{
						
						corners.Add(new PointF(x,y));
					}
				}
			}
			
			
			BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			for(int i=0;i<corners.Count;i++)
			{
				Drawing.DrawRectangles(bData,new Rectangle((int)corners[i].X,(int)corners[i].Y,3,3),Color.White);
			}
			b.UnlockBits(bData);
		}

		#endregion
		
		#region Bulleye
		public void BullEye(Bitmap b)
		{
			
			//Gris(b);
			int cx=b.Width/2;
			int cy=b.Height/2;
			
			Brush br=new LinearGradientBrush(new Rectangle(0,0,b.Width,b.Height),Color.White,Color.Black,LinearGradientMode.BackwardDiagonal);
			using(Graphics g=Graphics.FromImage(b))
			{
				g.FillEllipse(br,new Rectangle(cx-100,cy-100,200,200));
				
			}
		}
		#endregion

		public void Secuencia(Bitmap b,int id,float [,]filtro,int level,int offset,int umbral,int min,int max,float sigma,int low,int high,float sigma2,float K,float umbral2,Color c,bool flag1,bool flag2)
		{
			switch(id)
			{
					case 1:{new Imagen().Gris(b);break;}
					case 2:{new Imagen().Negativo(b);break;}
					case 3:{new Imagen().Gris(b);new Imagen().EcualizarHistograma(b);break;}
					case 4:{new Imagen().EcualizarHistograma(b);break;}
					case 5:{new Imagen().Gris(b);new Imagen().ExpandirHistograma(b,min,max);break;}
					case 6:{new Imagen().ExpandirHistograma(b,min,max,min,max,min,max);break;}
					case 7:{new Imagen().FastConvolution(b,filtro,offset);break;}
					case 8:{new Imagen().Thresholding(b,umbral);break;}
					case 9:{new Imagen().Gris(b);new Imagen().Multinivel(b,level);break;}
					case 10:{new Imagen().Regiones(b,level,c,flag2);break;}
					case 11:{new Imagen().Multinivel(b,level);break;}
					case 12:{new Imagen().Gris(b);new Imagen().Regiones(b,level,c,flag2);break;}
					case 13:{new Imagen().Moravec(b,umbral);break;}
					case 14:{new Imagen().Canny(b,sigma,(byte)low,(byte)high);break;}
					case 15:{new Imagen().HoughT(b);break;}
					case 16:{new Imagen().Harris(b,K,sigma2,umbral2);break;}
					/*case 17:{COLOR.CIELAB.ExtraerL(b,flag,true);break;}
					case 18:{COLOR.CIELAB.Extraera(b,flag,true);break;break;}
					case 19:{COLOR.CIELAB.Extraerb(b,flag,true);break;break;}*/
			}
		}
		#endregion
		
		#region Metodos Utilitarios e internos de esta clase
		
		#region Metodos Canny
		private float[,] Gradiente(float[,] gx,float[,] gy)
		{
			int w=gx.GetLength(0);
			int h=gx.GetLength(1);
			
			float[,]map=new float[w,h];
			for(int x=0;x<w;x++)
			{
				for(int y=0;y<h;y++)
				{
					
					map[x,y]=Utils.Distance(gx[x,y],gy[x,y]);
				}
			}
			return map;

		}
		private byte[] Orients(float[,]gx,float[,]gy)
		{
			int w=gx.GetLength(0);
			int h=gx.GetLength(1);
			int k=0;
			double toAngle = 180.0 / Math.PI;
			byte[] o=new byte[w*h];
			for(int i=0;i<w;i++)
			{
				for(int j=0;j<h;j++)
				{
					if(gx[i,j]==0){ o[k] = ( gy[i,j] == 0 ) ?(byte) 0 :(byte) 90;}
					else
					{
						double div = (double) (gy[i,j] / gx[i,j]);
						double orientation=0;
						
						if ( div < 0 )
						{
							orientation = 180 -  Math.Atan( -div ) * toAngle;
						}
						
						else
						{
							
							orientation =  Math.Atan( div ) * toAngle;
						}

						
						if ( orientation < 22.5 )
						{
							orientation = 0;
						}
						else if ( orientation < 67.5 )
						{
							orientation = 45;
						}
						else if ( orientation < 112.5 )
						{
							orientation = 90;
						}
						else if ( orientation < 157.5 )
						{
							orientation = 135;
						}
						else
						{
							orientation = 0;
						}
						
						o[k]=(byte)orientation;
					}
					k++;
				}
			}
			return o;
		}
		private byte[,] NoMax(float[,]gradients,byte[]orients)
		{
			float max=float.NegativeInfinity;
			foreach(float f in gradients)
			{
				if(f>max)
				{
					max=f;
				}
			}
			int w=gradients.GetLength(0);
			int h=gradients.GetLength(1);
			byte[,]nomax=new byte[w,h];
			float lp=0.0f;
			float rp=0.0f;
			int k=0;
			Parallel.For(1,h-1,delegate(int y)
			             {
			             	
			             	for ( int x = 1; x < w-1; x++, k++)
			             	{
			             		
			             		switch ( orients[k] )
			             		{
			             			case 0:
			             				lp  = gradients[x - 1, y];
			             				rp = gradients[x + 1, y];
			             				break;
			             			case 45:
			             				lp  = gradients[x - 1, y + 1];
			             				rp = gradients[x + 1, y - 1];
			             				break;
			             			case 90:
			             				lp  = gradients[x, y + 1];
			             				rp = gradients[x, y - 1];
			             				break;
			             			case 135:
			             				lp  = gradients[x + 1, y + 1];
			             				rp = gradients[x - 1, y - 1];
			             				break;
			             		}
			             		
			             		if ( ( gradients[x, y] < lp ) || ( gradients[x, y] < rp ) )
			             		{
			             			nomax[x,y] = 0;
			             		}
			             		else
			             		{
			             			nomax[x,y] = (byte) ( (gradients[x, y]*255) / max );
			             		}
			             	}
			             	
			             });
			return nomax;
		}
		private byte[,] Hysteresis(byte [,]map,byte LT,byte HT)
		{
			
			int w=map.GetLength(0);
			int h=map.GetLength(1);
			byte[,]hy=new byte[w,h];
			
			Parallel.For(0,w,delegate(int i)
			             {
			             	for(int j=0;j<h;j++)
			             	{
			             		hy[i,j]=255;
			             	}
			             });
			
			
			Parallel.For(1,w-1,delegate(int x)
			             {
			             	for(int y=1;y<h-1;y++)
			             	{
			             		hy[0,y]=0;
			             		hy[x,0]=0;
			             		hy[w-1,y]=0;
			             		hy[x,h-1]=0;
			             		if(map[x,y]<HT)
			             		{
			             			if(map[x,y]<LT)
			             			{
			             				hy[x,y]=0;
			             				
			             			}
			             			else
			             			{
			             				if(
			             					(map[x+1,y]<HT) &&
			             					(map[x,y+1]<HT)
			             				)
			             				{
			             					hy[x,y]=0;
			             					
			             				}
			             				
			             				
			             			}
			             		}
			             		
			             	}
			             });
			return hy;
		}
		#endregion
		
		#region Hough
		
		public struct HoughLine
		{
			public   double  Theta;

			public  short	Radius;

			public  short	Intensity;
			public  double  RelativeIntensity;
			public HoughLine( double theta, short radius, short intensity, double relativeIntensity )
			{
				Theta = theta;
				Radius = radius;
				Intensity = intensity;
				RelativeIntensity = relativeIntensity;
			}
			
		}
		private void GenLUT()
		{
			
			double theta = Math.PI / 180;

			SinMap=new double[180];
			CosMap=new double[180];
			
			for ( int i = 0; i < 180; i++ )
			{
				SinMap[i] = Math.Sin( i * theta);
				CosMap[i] = Math.Cos( i * theta);
			}
			
			
		}
		private HoughLine[] GetLines(float umbral)
		{
			short maxMapIntensity = 0;
			for ( int i = 0; i < HoughMap.GetLength(0); i++ )
			{
				for ( int j = 0; j < HoughMap.GetLength(1); j++ )
				{
					if ( HoughMap[i, j] > maxMapIntensity )
					{
						maxMapIntensity = HoughMap[i, j];
					}
				}
			}
			int		maxTheta = HoughMap.GetLength( 1 );
			int		maxRadius = HoughMap.GetLength( 0 );

			short	intensity;
			bool	foundGreater;

			int     halfHoughWidth = maxRadius >> 1;

			lines.Clear( );

			for ( int theta = 0; theta < maxTheta; theta++ )
			{

				for ( int radius = 0; radius < maxRadius; radius++ )
				{
					
					intensity = HoughMap[radius,radius];

					if ( intensity < 10 )
						continue;

					foundGreater = false;

					for ( int tt = theta - 4, ttMax = theta + 4; tt < ttMax; tt++ )
					{
						if ( foundGreater == true )
							break;

						int cycledTheta = tt;
						int cycledRadius = radius;

						if ( cycledTheta < 0 )
						{
							cycledTheta = maxTheta + cycledTheta;
							cycledRadius = maxRadius - cycledRadius;
						}
						if ( cycledTheta >= maxTheta )
						{
							cycledTheta -= maxTheta;
							cycledRadius = maxRadius - cycledRadius;
						}

						for ( int tr = cycledRadius - 4, trMax = cycledRadius + 4; tr < trMax; tr++ )
						{
							if ( tr < 0 )
								continue;
							if ( tr >= maxRadius )
								break;

							if ( HoughMap[tr,cycledTheta] > intensity )
							{
								foundGreater = true;
								break;
							}
						}
					}
					if ( !foundGreater )
					{
						
						lines.Add( new HoughLine( (double) theta, (short) ( radius - halfHoughWidth ), intensity, (double) intensity / maxMapIntensity ) );
					}
				}
			}
			//lines.Sort();
			int count = 0, n = lines.Count;

			while ( ( count < n ) && ( ( (HoughLine) lines[count] ).RelativeIntensity >= umbral ) )
				count++;
			
			n = Math.Min( count, lines.Count );

			HoughLine[] dst = new HoughLine[n];
			lines.CopyTo( 0, dst, 0, n );
			return dst;
		}
		
		#endregion
		
		#region Metodos Utilitarios
		
		private void ClearImage(Bitmap b)
		{
			
			BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int w=b.Width;
			int h=b.Height;
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - b.Width*3;

				for(int y=0;y<h;y++)
				{
					for(int x=0; x < w; ++x )
					{

						
						p[0]=p[1]=p[2]=0;
						p += 3;
					}
					p += nOffset;
				}
			}

			b.UnlockBits(bData);
		}
		private unsafe bool ComparePixels(byte *P,byte*P2)
		{
			if((*(P+0)!=*(P2+0))||(*(P+1)!=*(P2+1))||(*(P+2)!=*(P2+2)))
			{
				return false;
				
			}
			return true;
		}
		private unsafe byte* GetPixel(int x,int y)
		{
			return  (byte*) imageData.ToPointer( ) + y * paso + x * 3;
		}
		private void SetPixel( IntPtr data,int x, int y, byte r, byte g, byte b)
		{
			
			unsafe
			{
				
				byte* ptr = (byte*) data.ToPointer( ) + y * paso + x * 3;
				
				*(ptr+2) = r;
				*(ptr+1) = g;
				*(ptr+0) = b;
				
			}
			
		}
		
		
		private float[,,] CheckVecinos(float[,,]im)
		{
			
			float[,,] salida=new float[im.GetLength(0),im.GetLength(1),3];
			int w=im.GetLength(0);
			int h=im.GetLength(1);
			
			Parallel.For(0,w-1,delegate(int i)
			             {
			             	for(int j = 0; j < (h-1); j++)
			             	{
			             		if((im[i,j,0]*im[i+1,j,0])<0)
			             		{
			             			
			             			salida[i,j,0]=salida[i,j,1]=salida[i,j,2]=255.0F;
			             			
			             		}
			             		
			             		else
			             		{
			             			salida[i,j,0]=salida[i,j,1]=salida[i,j,2]=0.0F;
			             		}

			             	}
			             });
			
			return salida;
		}
		
		private double[] GreyHistogram(Bitmap b)
		{
			Gris(b);
			int w=b.Width;
			int h=b.Height;
			double []gh=new double[256];
			
			BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;
			
			unsafe
			{
				
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - b.Width*3;

				Parallel.For(0,h,delegate(int y)
				             {
				             	for(int x=0; x < w; ++x )
				             	{
				             		gh[*(p+2)]++;
				             		p += 3;
				             	}
				             	p += nOffset;
				             });

			}
			
			b.UnlockBits(bData);
			return gh;
		}
		
		private double[] CumulativeHistogram(Bitmap b)
		{
			Gris(b);
			double []h=Histograma(b,0);
			int pixels=b.Width*b.Height;
			for(int i=0;i<256;i++)
			{
				h[i]/=pixels;
			}
			double []a=new double[256];
			a[0]=h[0];
			for(int k=1;k<256;k++)
			{
				a[k]=a[k-1]+h[k];
			}
			return a;
		}
		private byte[] GenMapLevels(int levels)
		{
			int val=256/levels;
			int umbral=val/2;
			byte[] map = new byte[256];

			Parallel.For(0,256,delegate(int i)
			             {
			             	map[i] = (byte) Math.Min( 255, ( i / val ) * val + umbral );
			             });
			return map;

		}
		

		
		#endregion
		
		#region Variables
		private int paso;
		private ArrayList lines=new ArrayList();
		private IntPtr imageData;
		private double []SinMap;
		private double []CosMap;
		private short [,]HoughMap;
		private System.Windows.Forms.ProgressBar pg;
		#endregion
		#endregion
	}
	public class FFT
	{
		
		private static int[,] Image;
		private static int Width;
		private static int Height;
		private static int nx,ny;
		private static COMPLEX[,] CImage;
		private static COMPLEX[,] Data;
		private static COMPLEX[,] DataS;
		
		
		private static void Foward()
		{
			int i,j;
			CImage =new COMPLEX [Width,Height];
			Data = new COMPLEX[Width, Height];
			
			for (i=0;i<Width;i++)
			{
				for (j = 0; j <Height; j++)
				{
					CImage[i, j].real =(double) Image[i,j];
					CImage[i, j].imag = 0;
				}
			}
			Data= FFT2D( CImage, nx, ny, 1);
		}
		private static void FFTShift()
		{
			int i, j;
			DataS = new COMPLEX[nx, ny];

			for(i=0;i<=(nx/2)-1;i++)
			{
				for (j = 0; j <= (ny / 2) - 1; j++)
				{
					DataS[i + (nx / 2), j + (ny / 2)] = Data[i, j];
					DataS[i, j] = Data[i + (nx / 2), j + (ny / 2)];
					DataS[i + (nx / 2), j] = Data[i , j + (ny / 2)];
					DataS[i, j + (nx / 2)] = Data[i + (nx / 2), j ];
				}
			}

		}
		public static void FT(Bitmap b)
		{
			using(Bitmap t=Utils.Resize(b,256,256))
			{
				int w=b.Width;
				int h=b.Height;
				Image=Utils.ToIntMap(t);
				Width=t.Width;
				Height=t.Height;
				nx=Width;
				ny=Height;
				Foward();
				FFTShift();
				int i, j;
				float max;
				float [,] FFTLog = new float [nx,ny];
				float [,] FourierMagnitude = new float[nx, ny];
				int [,] FFTNormalized = new int[nx, ny];

				for(i=0;i<=Width-1;i++)
				{
					for (j = 0; j <= Height-1; j++)
					{
						FourierMagnitude[i, j] =DataS[i, j].Magnitude();
						FFTLog[i, j] = (float)Math.Log(1 + FourierMagnitude[i, j]);
						
					}
				}
				max = FFTLog[0, 0];
				for(i=0;i<=Width-1;i++)
					for (j = 0; j <= Height-1; j++)
				{
					if (FFTLog[i, j] > max)
						max = FFTLog[i, j];
				}
				for(i=0;i<=Width-1;i++)
					for (j = 0; j <= Height-1; j++)
				{
					FFTLog[i, j] = FFTLog[i, j] / max;
				}
				for(i=0;i<=Width-1;i++)
					for (j = 0; j <= Height-1; j++)
				{
					FFTNormalized [i,j]=(int)(1000*FFTLog[i,j]);
				}
				ClearImage(b);

				using(Graphics g=Graphics.FromImage(b))
				{
					
					g.DrawImage(Utils.Resize(Utils.DrawMap(FFTNormalized),w,h),0.0f,0.0F);
				}
			}
		}
		private static COMPLEX [,] FFT2D(COMPLEX[,] c, int nx, int ny, int dir)
		{
			int i,j;
			int m;
			double []real;
			double []imag;
			COMPLEX [,] output = c;
			real = new double[nx] ;
			imag = new double[nx];
			
			for (j=0;j<ny;j++)
			{
				for (i=0;i<nx;i++)
				{
					real[i] = c[i,j].real;
					imag[i] = c[i,j].imag;
				}
				
				m = (int)Math.Log((double)nx, 2);
				FFT1D(dir,m,ref real,ref imag);

				for (i=0;i<nx;i++)
				{
					
					output[i, j].real = real[i];
					output[i, j].imag = imag[i];
				}
			}
			
			real = new double[ny];
			imag = new double[ny];
			
			for (i=0;i<nx;i++)
			{
				for (j=0;j<ny;j++)
				{
					
					real[j] = output[i, j].real;
					imag[j] = output[i, j].imag;
				}
				m = (int)Math.Log((double)ny, 2);
				FFT1D(dir,m,ref real,ref imag);
				for (j=0;j<ny;j++)
				{
					
					output[i, j].real = real[j];
					output[i, j].imag = imag[j];
				}
			}
			
			return(output);
		}
		private static void FFT1D(int dir, int m, ref double[] x, ref double[] y )
		{
			long nn, i, i1, j, k, i2, l, l1, l2;
			double c1, c2, tx, ty, t1, t2, u1, u2, z;
			
			nn = 1;
			for (i = 0; i < m; i++)
				nn *= 2;
			
			i2 = nn >> 1;
			j = 0;
			for (i = 0; i < nn - 1; i++)
			{
				if (i < j)
				{
					tx = x[i];
					ty = y[i];
					x[i] = x[j];
					y[i] = y[j];
					x[j] = tx;
					y[j] = ty;
				}
				k = i2;
				while (k <= j)
				{
					j -= k;
					k >>= 1;
				}
				j += k;
			}
			
			c1 = -1.0;
			c2 = 0.0;
			l2 = 1;
			for (l = 0; l < m; l++)
			{
				l1 = l2;
				l2 <<= 1;
				u1 = 1.0;
				u2 = 0.0;
				for (j = 0; j < l1; j++)
				{
					for (i = j; i < nn; i += l2)
					{
						i1 = i + l1;
						t1 = u1 * x[i1] - u2 * y[i1];
						t2 = u1 * y[i1] + u2 * x[i1];
						x[i1] = x[i] - t1;
						y[i1] = y[i] - t2;
						x[i] += t1;
						y[i] += t2;
					}
					z = u1 * c1 - u2 * c2;
					u2 = u1 * c2 + u2 * c1;
					u1 = z;
				}
				c2 = Math.Sqrt((1.0 - c1) / 2.0);
				if (dir == 1)
					c2 = -c2;
				c1 = Math.Sqrt((1.0 + c1) / 2.0);
			}
			if (dir == 1)
			{
				for (i = 0; i < nn; i++)
				{
					x[i] /= (double)nn;
					y[i] /= (double)nn;
					
				}
			}

		}
		private static void ClearImage(Bitmap b)
		{
			
			BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int w=b.Width;
			int h=b.Height;
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - b.Width*3;

				for(int y=0;y<h;y++)
				{
					for(int x=0; x < w; ++x )
					{

						
						p[0]=p[1]=p[2]=0;
						p += 3;
					}
					p += nOffset;
				}
			}

			b.UnlockBits(bData);
		}
	}
	struct COMPLEX
	{
		public double real, imag;
		public COMPLEX(double x, double y)
		{
			real = x;
			imag = y;
		}
		public float Magnitude()
		{
			return ((float)Math.Sqrt(real * real + imag * imag));
		}
		public float Phase()
		{
			return ((float)Math.Atan(imag / real));
		}
	}
}
