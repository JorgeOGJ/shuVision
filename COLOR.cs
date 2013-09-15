/*
 * Clase para Pasar entre espacios de color
 */
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
namespace vcJOGJ
{
	
	public class COLOR
	{
		public class CIELAB
		{
			
			public static float[] FromRGB(byte R,byte G, byte B)
			{
				float[] Lab=new float[3];
				float rn=R/255.0F;
				float gn=G/255.0F;
				float bn=B/255.0F;
				
				float X,Y,Z;
				
				
				float Xn=0.9505F,Yn=1.0F,Zn=1.0890F;
				

				
				X=(float)(rn*0.4124 + gn*0.3576 + bn*0.1805);
				Y=	(float)(rn*0.2126 + gn*0.7152 + bn*0.0722);
				Z=	(float)(rn*0.0193 + gn*0.1192 + bn*0.9505);
				

				Lab[0]=(float)(116*F(Y/Yn)-16);
				Lab[1]=(float)(500*(F(X/Xn)-F(Y/Yn)));
				Lab[2]=(float)(200*(F(Y/Yn)-F(Z/Zn)));

				
				return Lab;
				
			}
			private static float F(float t)
			{
				return ((t > 0.008856)?(float) Math.Pow(t, (1.0/3.0)) :(float) (7.787*t + 16.0/116.0));
			}
			public static Bitmap GetLImage(Bitmap b)
			{
				BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
				int w=b.Width;
				int h=b.Height;
				float [,]map=new float[w,h];
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

							
							map[x,y]=FromRGB(*(p+2),*(p+1),*(p+0))[0];
							p += 3;
						}
						p += nOffset;
					}
				}

				b.UnlockBits(bData);
				return Utils.DrawMap(map);
			}
			public static Bitmap GetaImage(Bitmap b)
			{
				BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
				int w=b.Width;
				int h=b.Height;
				int stride = bData.Stride;
				IntPtr Scan0 = bData.Scan0;
				float [,]map=new float[w,h];
				unsafe
				{
					byte * p = (byte *)(void *)Scan0;

					int nOffset = stride - b.Width*3;

					for(int y=0;y<h;y++)
					{
						for(int x=0; x < w; ++x )
						{

							map[x,y]=FromRGB(*(p+2),*(p+1),*(p+0))[1];
							p += 3;
						}
						p += nOffset;
					}
				}

				b.UnlockBits(bData);

				return Utils.DrawMap(map);
			}
			public static Bitmap GetbImage(Bitmap b)
			{
				BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
				int w=b.Width;
				int h=b.Height;
				int stride = bData.Stride;
				IntPtr Scan0 = bData.Scan0;
				float [,]map=new float[w,h];
				unsafe
				{
					byte * p = (byte *)(void *)Scan0;

					int nOffset = stride - b.Width*3;

					for(int y=0;y<h;y++)
					{
						for(int x=0; x < w; ++x )
						{

							map[x,y]=FromRGB(*(p+2),*(p+1),*(p+0))[2];
							p += 3;
						}
						p += nOffset;
					}
				}

				b.UnlockBits(bData);

				return Utils.DrawMap(map);
			}
			
			public static void ExtraerL(Bitmap b,bool color,bool fast=false)
			{
				if(color)
				{
					BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
					int w=b.Width;
					int h=b.Height;
					if((w>800 || h>800))fast=true;
					int stride = bData.Stride;
					IntPtr Scan0 = bData.Scan0;

					unsafe
					{
						byte * p = (byte *)(void *)Scan0;

						int nOffset = stride - b.Width*3;
						
						for(int x=0;x<w;x++)
						{
							for(int y=0;y<h;y++)
							{
								*(p+2)=(byte)Utils.Scale(0,100,0,255,FromRGB(*(p+2),*(p+1),*(p+0))[0]);
								p += 3;
							}
							p += nOffset;
						}
						
					}

					b.UnlockBits(bData);
				}
				else
				{
					BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
					int w=b.Width;
					int h=b.Height;
					
					int stride = bData.Stride;
					IntPtr Scan0 = bData.Scan0;

					if((w>800 || h>800))fast=true;
					unsafe
					{
						byte * p = (byte *)(void *)Scan0;

						int nOffset = stride - b.Width*3;

						
						for(int x=0;x<w;x++)
						{
							for(int y=0;y<h;y++)
							{
								*(p+0)=*(p+1)=*(p+2)=(byte)Utils.Scale(0,100,0,255,FromRGB(*(p+2),*(p+1),*(p+0))[0]);
								p += 3;
							}
							p += nOffset;
						}
						
						
					}

					b.UnlockBits(bData);
				}
			}
			public static void Extraera(Bitmap b,bool color,bool fast=false)
			{
				if(color)
				{
					BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
					int w=b.Width;
					int h=b.Height;
					
					if((w>800 || h>800))fast=true;
					int stride = bData.Stride;
					IntPtr Scan0 = bData.Scan0;

					unsafe
					{
						byte * p = (byte *)(void *)Scan0;

						int nOffset = stride - b.Width*3;

						if(fast)
						{
							Parallel.For(0,h,y=>
							             {
							             	for(int x=0; x < w; ++x )
							             	{

							             		
							             		*(p+1)=(byte)Utils.Scale(-100,100,0,255,FromRGB(*(p+2),*(p+1),*(p+0))[1]);
							             		p += 3;
							             	}
							             	p += nOffset;
							             });
						}
						else
						{
							for(int y=0;y<h;y++)
							{
								for(int x=0; x < w; ++x )
								{

									
									*(p+1)=(byte)Utils.Scale(-100,100,0,255,FromRGB(*(p+2),*(p+1),*(p+0))[1]);
									p += 3;
								}
								p += nOffset;
							}
						}
					}

					b.UnlockBits(bData);
				}
				else
				{
					BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
					int w=b.Width;
					int h=b.Height;
					
					int stride = bData.Stride;
					IntPtr Scan0 = bData.Scan0;

					if((w>800 || h>800))fast=true;
					unsafe
					{
						byte * p = (byte *)(void *)Scan0;

						int nOffset = stride - b.Width*3;

						if(fast)
						{
							Parallel.For(0,h,y=>
							             {
							             	for(int x=0; x < w; ++x )
							             	{

							             		
							             		*(p+0)=*(p+1)=*(p+2)=(byte)Utils.Scale(-100,100,0,255,FromRGB(*(p+2),*(p+1),*(p+0))[1]);
							             		p += 3;
							             	}
							             	p += nOffset;
							             });
						}
						else
						{
							for(int y=0;y<h;y++)
							{
								for(int x=0; x < w; ++x )
								{

									
									*(p+0)=*(p+1)=*(p+2)=(byte)Utils.Scale(-100,100,0,255,FromRGB(*(p+2),*(p+1),*(p+0))[1]);
									p += 3;
								}
								p += nOffset;
							}
						}
					}

					b.UnlockBits(bData);
				}
			}
			public static void Extraerb(Bitmap b,bool color,bool fast=false)
			{
				if(color)
				{
					BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
					int w=b.Width;
					int h=b.Height;
					if((w>800 || h>800))fast=true;
					int stride = bData.Stride;
					IntPtr Scan0 = bData.Scan0;

					unsafe
					{
						byte * p = (byte *)(void *)Scan0;

						int nOffset = stride - b.Width*3;

						if(fast)
						{
							Parallel.For(0,h,y=>
							             {
							             	for(int x=0; x < w; ++x )
							             	{

							             		
							             		*(p+0)=(byte)Utils.Scale(-90,90,0,255,FromRGB(*(p+2),*(p+1),*(p+0))[2]);
							             		p += 3;
							             	}
							             	p += nOffset;
							             });
						}
						else
						{
							for(int y=0;y<h;y++)
							{
								for(int x=0; x < w; ++x )
								{

									
									*(p+0)=(byte)Utils.Scale(-90,90,0,255,FromRGB(*(p+2),*(p+1),*(p+0))[2]);
									p += 3;
								}
								p += nOffset;
							}
						}
					}

					b.UnlockBits(bData);
				}
				else
				{
					BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
					int w=b.Width;
					int h=b.Height;
					if((w>800 || h>800))fast=true;
					int stride = bData.Stride;
					IntPtr Scan0 = bData.Scan0;

					unsafe
					{
						byte * p = (byte *)(void *)Scan0;

						int nOffset = stride - b.Width*3;

						if(fast)
						{
							Parallel.For(0,h,y=>
							             {
							             	for(int x=0; x < w; ++x )
							             	{

							             		
							             		*(p+0)=*(p+1)=*(p+2)=(byte)Utils.Scale(-90,90,0,255,FromRGB(*(p+2),*(p+1),*(p+0))[2]);
							             		p += 3;
							             	}
							             	p += nOffset;
							             });
						}
						else
						{
							for(int y=0;y<h;y++)
							{
								for(int x=0; x < w; ++x )
								{

									
									*(p+0)=*(p+1)=*(p+2)=(byte)Utils.Scale(-90,90,0,255,FromRGB(*(p+2),*(p+1),*(p+0))[2]);
									p += 3;
								}
								p += nOffset;
							}
						}
					}

					b.UnlockBits(bData);
				}
			}
			
		}
		
		
		public class RGB
		{
			private Color FromByte(byte r,byte g, byte b)
			{
				return Color.FromArgb(r,g,b);
			}
			
			public static void Colorizar(Bitmap b)
			{
				Color []Mapa={Color.Black,Color.Red,Color.Orange,
					Color.Yellow,Color.Green,
					Color.Blue,Color.Purple,Color.Violet,
					Color.White};
				
				
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
							Color c=Mapa[(int)Utils.Scale(0,255,0,Mapa.Length-1,*(p+0))];
							SetPixel(stride,Scan0,x,y,c);
							
							p += 3;
						}
						p += nOffset;
					}
				}

				b.UnlockBits(bData);
			}
			
			private static void SetPixel(int paso, IntPtr data,int x, int y,Color c)
			{
				
				unsafe
				{
					
					byte* ptr = (byte*) data.ToPointer( ) + y * paso + x * 3;
					
					*(ptr+2) = c.R;
					*(ptr+1) = c.G;
					*(ptr+0) = c.B;
					
				}
				
			}
		}
	}
}
