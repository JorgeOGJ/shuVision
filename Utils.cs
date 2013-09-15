using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace vcJOGJ
{
	/// <summary>
	/// Clase con diversos metodos utilitarios
	/// </summary>
	public class Utils
	{
		
		#region Imagen-Arreglos
		/// <summary>
		/// Convierte una imagen rgb a un arreglo de 3 dimensiones
		/// </summary>
		/// <param name="bmp">Imagen bmp en rgb</param>
		/// <returns>arreglo de 3 dimensiones con los valores de la imagen </returns>
		public   static float [,,]ToArray( Bitmap bmp)
		{
			int w=bmp.Width;
			int h=bmp.Height;
			float[,,] array=new float[w,h,3];
			BitmapData bData = bmp.LockBits(new Rectangle(0, 0,w,h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - w*3;

				for(int y=0;y<h;++y)
				{
					for(int x=0; x < w; ++x )
					{

						array[x,y,2]=*(p+0);
						array[x,y,1]=*(p+1);
						array[x,y,0]=*(p+2);
						p += 3;
					}
					p += nOffset;
				}
			}

			bmp.UnlockBits(bData);
			return array;
		}
		
		public static float[,]ToMap(Bitmap bmp)
		{
			int w=bmp.Width;
			int h=bmp.Height;
			float[,] array=new float[w,h];
			BitmapData bData = bmp.LockBits(new Rectangle(0, 0,w,h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - w*3;

				for(int y=0;y<h;++y)
				{
					for(int x=0; x < w; ++x )
					{

						array[x,y]=(float)((*(p+0)*.11)+(*(p+1)*.59)+(*(p+2)*.30));

						p += 3;
					}
					p += nOffset;
				}
			}

			bmp.UnlockBits(bData);
			return array;
			
		}
		public static int[,]ToIntMap(Bitmap bmp)
		{
			int w=bmp.Width;
			int h=bmp.Height;
			int[,] array=new int[w,h];
			BitmapData bData = bmp.LockBits(new Rectangle(0, 0,w,h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - w*3;

				for(int y=0;y<h;++y)
				{
					for(int x=0; x < w; ++x )
					{

						array[x,y]=(int)(((*(p+0))+(*(p+1))+(*(p+2)))/3);

						p += 3;
					}
					p += nOffset;
				}
			}

			bmp.UnlockBits(bData);
			return array;
			
		}
		/// <summary>
		/// Convierte el arreglo a una imagen
		/// </summary>
		/// <param name="data"></param>
		/// <returns>Imagen convertida apartir de un arreglo</returns>
		public   static Bitmap FromArray(float[,,] data)
		{
			
			int w=data.GetUpperBound(0);
			int h=data.GetUpperBound(1);
			Bitmap bmp=new Bitmap(w,h);
			BitmapData bData = bmp.LockBits(new Rectangle(0, 0,w,h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - w*3;

				for(int y=0;y<h;++y)
				{
					for(int x=0; x < w; ++x )
					{

						*(p+0)=(byte)data[x,y,2];
						*(p+1)=(byte)data[x,y,1];
						*(p+2)=(byte)data[x,y,0];
						p += 3;
					}
					p += nOffset;
				}
			}

			bmp.UnlockBits(bData);
			return bmp;
		}
		
		/// <summary>
		/// Convierte una arreglo de 3 dimensiones a una imagen sin hacer una copia de ella
		/// </summary>
		/// <param name="bmp">Imagen de salida</param>
		/// <param name="data">Arreglo conteniendo las intensidades para la imagen</param>
		public static void ToBitmap(Bitmap bmp,float[,,] data)
		{
			int w=bmp.Width;
			int h=bmp.Height;
			BitmapData bData = bmp.LockBits(new Rectangle(0, 0,w,h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - w*3;

				for(int y=0;y<h;++y)
				{
					for(int x=0; x < w; ++x )
					{

						*(p+0)=(byte)data[x,y,2];
						*(p+1)=(byte)data[x,y,1];
						*(p+2)=(byte)data[x,y,0];
						p += 3;
					}
					p += nOffset;
				}
			}

			bmp.UnlockBits(bData);
		}
		
		public static  Bitmap Resize(Bitmap Image, int Width, int Height)
		{
			
			Bitmap NewBitmap = new Bitmap(Width, Height);
			using (Graphics NewGraphics = Graphics.FromImage(NewBitmap))
			{
				
				NewGraphics.CompositingQuality = CompositingQuality.HighSpeed;
				NewGraphics.SmoothingMode = SmoothingMode.HighSpeed;
				NewGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				NewGraphics.DrawImage(Image, new System.Drawing.Rectangle(0, 0, Width, Height));
			}
			
			return NewBitmap;
		}

		public static  Bitmap DrawMap(float [,]map)
		{
			float[] minmax=MinMax(map);
			int w=map.GetUpperBound(0);
			int h=map.GetUpperBound(1);
			Bitmap bmp=new Bitmap(w,h);
			BitmapData bData = bmp.LockBits(new Rectangle(0, 0,w,h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - w*3;

				for(int y=0;y<h;++y)
				{
					for(int x=0; x < w; ++x )
					{

						*(p+0)=*(p+1)=*(p+2)=(byte)Scale(minmax[0],minmax[1],0,255,map[x,y]);
						p += 3;
					}
					p += nOffset;
				}
			}

			bmp.UnlockBits(bData);
			return bmp;
		}
		public static  Bitmap DrawMap(float [,]map,int min,int max)
		{
			
			int w=map.GetUpperBound(0);
			int h=map.GetUpperBound(1);
			Bitmap bmp=new Bitmap(w,h);
			BitmapData bData = bmp.LockBits(new Rectangle(0, 0,w,h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - w*3;

				for(int y=0;y<h;++y)
				{
					for(int x=0; x < w; ++x )
					{

						*(p+0)=*(p+1)=*(p+2)=(byte)Scale(min,max,0,255,map[x,y]);
						p += 3;
					}
					p += nOffset;
				}
			}

			bmp.UnlockBits(bData);
			return bmp;
		}
		public static  Bitmap DrawMap(byte [,]map)
		{
			byte[] minmax=MinMax(map);
			int w=map.GetUpperBound(0);
			int h=map.GetUpperBound(1);
			Bitmap bmp=new Bitmap(w,h);
			BitmapData bData = bmp.LockBits(new Rectangle(0, 0,w,h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - w*3;

				for(int y=0;y<h;++y)
				{
					for(int x=0; x < w; ++x )
					{

						*(p+0)=*(p+1)=*(p+2)=(byte)Scale(minmax[0],minmax[1],0,255,map[x,y]);
						p += 3;
					}
					p += nOffset;
				}
			}

			bmp.UnlockBits(bData);
			return bmp;
		}
		public static  Bitmap DrawMap(short [,]map)
		{
			short[] minmax=MinMax(map);
			int w=map.GetUpperBound(0);
			int h=map.GetUpperBound(1);
			Bitmap bmp=new Bitmap(w,h);
			BitmapData bData = bmp.LockBits(new Rectangle(0, 0,w,h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - w*3;

				for(int y=0;y<h;++y)
				{
					for(int x=0; x < w; ++x )
					{

						*(p+0)=*(p+1)=*(p+2)=(byte)Scale(minmax[0],minmax[1],0,255,map[x,y]);
						p += 3;
					}
					p += nOffset;
				}
			}

			bmp.UnlockBits(bData);
			return bmp;
		}
		public static  Bitmap DrawMap(int [,]map,int min=0,int max=255)
		{
			
			int w=map.GetUpperBound(0);
			int h=map.GetUpperBound(1);
			Bitmap bmp=new Bitmap(w,h);
			BitmapData bData = bmp.LockBits(new Rectangle(0, 0,w,h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - w*3;

				for(int y=0;y<h;++y)
				{
					for(int x=0; x < w; ++x )
					{

						*(p+0)=*(p+1)=*(p+2)=(byte)Scale(min,max,0,255,map[x,y]);
						p += 3;
					}
					p += nOffset;
				}
			}

			bmp.UnlockBits(bData);
			return bmp;
		}
		#endregion
		
		#region Kernel Gaussiano
		/// <summary>
		/// Crea un kernel Gaussiano 2D de un tamaño 5*sigma
		/// </summary>
		/// <param name="sigma">Valor de sigma para la funcion</param>
		/// <returns>kernel de tamaño 5sigma</returns>
		public static float[,] Kernel2D(float sigma)
		{
			int size=((int)(sigma*3)|1);
			
			Sigma=sigma;
			sqrSigma=Sigma*Sigma;
			int r = size >> 1;
			float[,] kernel = new float[size, size];

			for ( int y = -r, i = 0; i < size; y++, i++ )
			{
				for ( int x = -r, j = 0; j < size; x++, j++ )
				{
					kernel[i, j] = F2( x, y );
					
				}
			}
			return kernel;
		}
		
		/// <summary>
		/// Crea un kernel de 1 dimension vertical
		/// </summary>
		/// <param name="sigma">Valor de sigma</param>
		/// <returns>kernel vertical de tamaño 5sigma</returns>
		public static float[,] Kernel2DV(float sigma)
		{
			int size=((int)(sigma*3)|1);
			Console.WriteLine(size.ToString());
			Sigma=sigma;
			sqrSigma=Sigma*Sigma;
			int r = size >> 1;
			float[,] kernel = new float[1, size];

			for ( int  i = 0; i < 1; i++ )
			{
				for ( int x = -r, j = 0; j < size; x++, j++ )
				{
					kernel[i, j] = F( x);
					
				}
			}
			return kernel;
		}
		
		/// <summary>
		/// Crea un kernel de 1 dimension horizontal
		/// </summary>
		/// <param name="sigma">Valor de sigma</param>
		/// <returns>kernel horizontal de tamaño 5sigma</returns>
		public static float[,] Kernel2DH(float sigma)
		{
			int size=((int)(sigma*5)|1);
			Console.WriteLine(size.ToString());
			Sigma=sigma;
			sqrSigma=Sigma*Sigma;
			int r = size >> 1;
			float[,] kernel = new float[size, 1];

			for ( int y = -r, i = 0; i < size; y++, i++ )
			{
				for ( int j = 0; j < 1; j++ )
				{
					kernel[i, j] = F( y);
					
				}
			}
			return kernel;
		}
		
		private static float F(int x)
		{
			return (float)(Math.Exp( x * x / ( -2 * sqrSigma ) ) / ( Math.Sqrt( 2 * Math.PI ) * Sigma ));
		}
		private  static float F2(int x,int y)
		{
			return (float)( Math.Exp( ( x * x + y * y ) / ( -2 * sqrSigma ) ) / ( 2 * Math.PI * sqrSigma ));
			
		}
		private static float Sigma=1.4f;
		private static float sqrSigma=(float)(1.4*1.4);
		#endregion
		
		#region Matematicas
		public static int[] MinMax(int [,]map)
		{
			int min=int.MaxValue;
			int max=int.MinValue;
			int w=map.GetLength(0);
			int h=map.GetLength(1);
			Parallel.For(0,w,delegate(int i)
			             {
			             	for(int j=0;j<h;j++)
			             	{
			             		if(map[i,j]>max)
			             		{
			             			max=map[i,j];
			             		}
			             		if(map[i,j]<min)
			             		{
			             			min=map[i,j];
			             		}
			             	}
			             });
			int []m=new int[2];
			m[0]=min;
			m[1]=max;
			return m;
		}
		public static byte[] MinMax(byte[,]map)
		{
			byte min=255;
			byte max=0;
			int w=map.GetLength(0);
			int h=map.GetLength(1);
			Parallel.For(0,w,delegate(int i)
			             {
			             	for(int j=0;j<h;j++)
			             	{
			             		if(map[i,j]>max)
			             		{
			             			max=map[i,j];
			             		}
			             		if(map[i,j]<min)
			             		{
			             			min=map[i,j];
			             		}
			             	}
			             });
			byte []m=new byte[2];
			m[0]=min;
			m[1]=max;
			return m;
		}
		public static short[] MinMax(short[,]map)
		{
			short min=short.MaxValue;
			short max=short.MinValue;
			int w=map.GetLength(0);
			int h=map.GetLength(1);
			Parallel.For(0,w,delegate(int i)
			             {
			             	for(int j=0;j<h;j++)
			             	{
			             		if(map[i,j]>max)
			             		{
			             			max=map[i,j];
			             		}
			             		if(map[i,j]<min)
			             		{
			             			min=map[i,j];
			             		}
			             	}
			             });
			short []m=new short[2];
			m[0]=min;
			m[1]=max;
			return m;
		}
		public static float[] MinMax(float[,] map)
		{
			float min=float.MaxValue;
			float max=float.MinValue;
			int w=map.GetLength(0);
			int h=map.GetLength(1);
			Parallel.For(0,w,delegate(int i)
			             {
			             	for(int j=0;j<h;j++)
			             	{
			             		if(map[i,j]>max)
			             		{
			             			max=map[i,j];
			             		}
			             		if(map[i,j]<min)
			             		{
			             			min=map[i,j];
			             		}
			             	}
			             });
			float []m=new float[2];
			m[0]=min;
			m[1]=max;
			return m;
		}
		public static float Distance(float x,float y)
		{
			return (float)(Math.Abs(x)+Math.Abs(y));
		}
		public static float[,,] Abs(float[,,] x)
		{
			float[,,] s=new float[x.GetLength(0),x.GetLength(1),3];
			int w=x.GetLength(0);
			int h=x.GetLength(1);
			Parallel.For(0,w,delegate(int i)
			             {
			             	for(int j=0;j<h;j++)
			             	{
			             		s[i,j,0]=Math.Abs(x[i,j,0]);
			             		s[i,j,1]=Math.Abs(x[i,j,1]);
			             		s[i,j,2]=Math.Abs(x[i,j,2]);
			             	}
			             });
			return s;
		}
		public static double Mean(Bitmap b)
		{
			BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int w=b.Width;
			int h=b.Height;
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;
			double sum=0;
			int count=0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - b.Width*3;

				Parallel.For(0,h,delegate(int y)
				             {
				             	for(int x=0; x < w; ++x )
				             	{

				             		sum+=*(p+0);
				             		p += 3;
				             		count++;
				             	}
				             	p += nOffset;
				             });
			}

			b.UnlockBits(bData);
			return sum/=count;
		}		
		public static int[] Ecualizar( int[] histogra, int nuPixel )
		{
			int[] eh = new int[256];
			float coef = 255.0f / nuPixel;

			float prev = histogra[0] * coef;
			eh[0] = (byte) prev;

			for(int i=1;i<256;i++)
			{
				prev += histogra[i] * coef;
				eh[i] =   (int)prev;
			}

			return eh;
		}
		public static void Ajustar2(float[,,] im)
		{
			
			int w=im.GetUpperBound(0);
			int h=im.GetUpperBound(1);
			
			
			for(int i=0;i<w;i++)
			{
				for(int j=0;j<h;j++)
				{
					if(float.IsNaN(im[i,j,0]))im[i,j,0]=0;
					im[i,j,0]=im[i,j,1]=im[i,j,2]=(((1+im[i,j,0])*255)/2);
				}
			}
			
		}
		public static void Ajustar(float[,,] im)
		{
			int w=im.GetUpperBound(0);
			int h=im.GetUpperBound(1);
			float max=float.MinValue;
			float min=float.MaxValue;
			
			for(int i=0;i<w;i++)
			{
				for(int j=0;j<h;j++)
				{
					if(im[i,j,0]>max)
					{
						max=im[i,j,0];
					}
					if(im[i,j,0]<min)
					{
						min=im[i,j,0];
					}
				}
			}
			
			
			for(int i=0;i<w;i++)
			{
				for(int j=0;j<h;j++)
				{
					if(float.IsNaN(im[i,j,0]))im[i,j,0]=0;
					im[i,j,0]=im[i,j,1]=im[i,j,2]=(((Math.Abs(min)+im[i,j,0])*255)/Math.Abs(min-max));
				}
			};
		}
		public static void AjustarF(float[,] map)
		{
			int w=map.GetLength(0);
			int h=map.GetLength(1);
			float max=float.MinValue;
			float min=float.MaxValue;
			
			for(int i=0;i<w;i++)
			{
				for(int j=0;j<h;j++)
				{
					if(map[i,j]>max)
					{
						max=map[i,j];
					}
					if(map[i,j]<min)
					{
						min=map[i,j];
					}
				}
			}
			
			for(int i=0;i<w;i++)
			{
				for(int j=0;j<h;j++)
				{
					
					map[i,j]=(((Math.Abs(min)+map[i,j])*255)/Math.Abs(min-max));
				}
			}
			
		}
		public static double Scale(double fromMin, double fromMax, double toMin, double toMax, double x)
		{
			if (fromMax - fromMin == 0) return 0;
			return (toMax - toMin) * (x - fromMin) / (fromMax - fromMin) + toMin;
		}
		public static float[,] GenMap(float [,,] im)
		{
			int w=im.GetUpperBound(0);
			int h=im.GetUpperBound(1);
			float [,]map=new float[w,h];
			float max=float.MinValue;
			
			for(int i=0;i<w;i++)
			{
				for(int j=0;j<h;j++)
				{
					if(im[i,j,0]>max)
					{
						max=im[i,j,0];
					}
				}
			}
			
			for(int i=0;i<w;i++)
			{
				for(int j=0;j<h;j++)
				{
					
					map[i,j]=((im[i,j,0])/max);
				}
			}
			
			return map;
		}
		public static float Pow(float a,float b)
		{
			if (b==0) return 1;
			if (a==0) return 0;
			if (b%2==0) {
				return Pow(a*a, b/2);
			} else if (b%2==1) {
				return a*Pow(a*a,b/2);
			}
			return 0;
		}
		#endregion
		
		#region Guardar
		public static void Save(float [,,]im,string name)
		{
			StreamWriter w=new StreamWriter(name);
			int max=im.GetUpperBound(0)+1;
			int min=im.GetUpperBound(1)+1;
			for(int i=0;i<max;i++)
			{
				for(int j=0;j<min;j++)
				{
					w.Write(im[i,j,0]+"\t");
				}
				w.WriteLine();
			}
			w.Close();
		}
		public static void Save(float [,]im,string name)
		{
			StreamWriter w=new StreamWriter(name);
			int max=im.GetLength(0);
			int min=im.GetLength(1);

			for(int i=0;i<max;i++)
			{
				for(int j=0;j<min;j++)
				{
					w.Write(im[i,j]+"\t");
				}
				w.WriteLine();
			}
			w.Close();
		}
		public static void Save(double [,]im)
		{
			StreamWriter w=new StreamWriter("./pixels");
			int max=im.GetUpperBound(0)+1;
			int min=im.GetUpperBound(1)+1;
			w.WriteLine(max);
			w.WriteLine(min);
			for(int i=0;i<max;i++)
			{
				for(int j=0;j<min;j++)
				{
					w.Write(im[i,j]+"\t");
				}
				w.WriteLine();
			}
			w.Close();
		}
		#endregion
		
		public static System.Drawing.Point[] Find(float sim,float[,]map)
		{
			ArrayList puntos=new ArrayList();
			for(int x=0;x<map.GetLength(0);x++)
			{
				for(int y=0;y<map.GetLength(1);y++)
				{
					if(map[x,y]>=sim)
					{
						
						puntos.Add(new System.Drawing.Point(x,y));
						
					}
				}
			}
			System.Drawing.Point []ps=new System.Drawing.Point[puntos.Count];
			for(int i=0;i<puntos.Count;i++)
			{
				ps[i]=(System.Drawing.Point)puntos[i];
			}
			
			return ps;
		}
		
		public static int[,] SubArray(int  [,]src,int startRow, int endRow, int startColumn, int endColumn)
		{
			 int rows = src.GetLength(0);
            int cols = src.GetLength(1);

            if ((startRow > endRow) || (startColumn > endColumn) || (startRow < 0) ||
                (startRow >= rows) || (endRow < 0) || (endRow >= rows) ||
                (startColumn < 0) || (startColumn >= cols) || (endColumn < 0) ||
                (endColumn >= cols))
            {
                throw new ArgumentException("Argument out of range.");
            }

            int[,] X = new int[endRow - startRow + 1, endColumn - startColumn + 1];
            for (int i = startRow; i <= endRow; i++)
            {
                for (int j = startColumn; j <= endColumn; j++)
                {
                    X[i - startRow, j - startColumn] = src[i, j];
                }
            }

            return X;
		}
	}
	
	
}
