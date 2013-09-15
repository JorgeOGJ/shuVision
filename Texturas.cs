/*
 Clase usada para el procesamiento de Texturas
 conteniendo los metodos para crear Histogramas de sumas y diferencias
 */
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.Collections.Generic;

namespace vcJOGJ
{
	
	public  class Texturas
	{
		private static float []sh;
		private  static  float []dh;
		public static float energia;
		public static  float entropia;
		public static float contraste;
		public static float media;
		public static float homogeneidad;

		/// <summary>
		/// Calcula el SDH para r{1,2} y theta {0,45,90,135} para una imagen de entrada y sus propiedades de textura
		/// </summary>
		/// <param name="b">Imagen de textura</param>
		public static void CalcSDH(Bitmap b)
		{
			
			int[,]Map=Utils.ToIntMap(b);
			int level=GetLevel(Map);
			sh=sha(Map,level);
			dh=dha(Map,level);
			
			energia=energy(sh,dh);
			entropia=entropy(sh,dh);
			contraste=contrast(dh);
			media=average(sh);
			homogeneidad=homogenity(dh);
		}
		
		public static void CalcSDH(int[,] Map)
		{
			
			
			int level=GetLevel(Map);
			sh=sha(Map,level);
			dh=dha(Map,level);
			
			energia=energy(sh,dh);
			entropia=entropy(sh,dh);
			contraste=contrast(dh);
			media=average(sh);
			homogeneidad=homogenity(dh);
		}
		
		
		private static int GetLevel(int[,]Map)
		{
			int l=0;
			int w=Map.GetLength(0);
			int h=Map.GetLength(1);
			

			List<int>levels=new List<int>();
			

			for(int y=0;y<h;y++)
			{
				for(int x=0; x < w; ++x )
				{
					if(l<Map[x,y])
					{
						l=Map[x,y];
					}
					
				}
				
			}
			
			
			return l;
		}
		/// <summary>
		/// calcula el histograma de sumas acumalado para r{1,2} y thetha{0,45,90,135}
		/// </summary>
		/// <param name="a">la imagen de entrada pasada como una matriz de datos
		/// </param>
		/// <param name="level">el numero de niveles de grises en la imagen</param>
		/// <returns>un arreglo con la sh acumulada</returns>
		private static float[] sha(int[,]a,int level)
		{
			int L=(2*level)+3;
			float[] s;
			float []sh1;
			float []sh2;
			float []sh3;
			float []sh4;
			float []sh5;
			float []sh6;
			float []sh7;
			float []sh8;
			
			s=new float[L];
			sh1=new float[L];
			sh2=new float[L];
			sh3=new float[L];
			sh4=new float[L];
			sh5=new float[L];
			sh6=new float[L];
			sh7=new float[L];
			sh8=new float[L];
			
			
			
			sh1=calcsh(1,0,a,L);
			sh2=calcsh(1,45,a,L);
			sh3=calcsh(1,90,a,L);
			sh4=calcsh(1,135,a,L);
			sh5=calcsh(2,0,a,L);
			sh6=calcsh(2,45,a,L);
			sh7=calcsh(2,90,a,L);
			sh8=calcsh(2,135,a,L);
			
			for(int i=0;i<s.Length;i++)
			{
				s[i]=((sh1[i]+sh2[i]+sh3[i]+sh4[i]+sh5[i]+sh6[i]+sh7[i]+sh8[i]));
			}
			float sum=0.0f;
			for(int i=0;i<s.Length;i++)
			{
				sum+=s[i];
			}
			for(int i=0;i<s.Length;i++)
			{
				s[i]/=sum;
			}
			return s;
			
		}
		/// <summary>
		/// calcula el histograma de diferencias acumulado para r{1,2} y thetha{0,45,90,135}
		/// </summary>
		/// <param name="a">la imagen de entrada pasada como una matriz de datos</param>
		/// <param name="level">el numero de niveles de grises en la imagen</param>
		/// <returns>un arreglo con la dh acumulada</returns>
		private static float[] dha(int[,]a,int level)
		{
			int L=	(level)+1;
			float[] s;
			float []sh1;
			float []sh2;
			float []sh3;
			float []sh4;
			float []sh5;
			float []sh6;
			float []sh7;
			float []sh8;
			s=new float[L];
			sh1=new float[L];
			sh2=new float[L];
			sh3=new float[L];
			sh4=new float[L];
			sh5=new float[L];
			sh6=new float[L];
			sh7=new float[L];
			sh8=new float[L];
			sh1=calcdh(1,0,a,L);
			sh2=calcdh(1,45,a,L);
			sh3=calcdh(1,90,a,L);
			sh4=calcdh(1,135,a,L);
			sh5=calcdh(2,0,a,L);
			sh6=calcdh(2,45,a,L);
			sh7=calcdh(2,90,a,L);
			sh8=calcdh(2,135,a,L);
			
			for(int i=0;i<s.Length;i++)
			{
				s[i]=(sh1[i]+sh2[i]+sh3[i]+sh4[i]+sh5[i]+sh6[i]+sh7[i]+sh8[i]);
				
			}
			float sum=0.0f;
			for(int i=0;i<s.Length;i++)
			{
				sum+=s[i];
			}
			for(int i=0;i<s.Length;i++)
			{
				s[i]/=sum;
			}
			return s;
			
		}
		
		private static float[] calcsh(int r,int thetha,int [,]a,int level)
		{
			float []s=new float[level];
			int w=a.GetLength(0);
			int h=a.GetLength(1);
			
			switch(thetha)
			{
				case 0:
					{
						
						int sum;
						for(int i=0;i<w-r;i++)
						{
							for(int j=0;j<h;j++)
							{
								
								if(!(i+r>w))
								{
									sum=a[i,j]+a[i+r,j];
									
									s[sum]++;sum=0;
								}
							}
							
						}
						
						break;
					}
				case 45:
					{
						
						int sum;
						for(int i=0;i<w-r;i++)
						{
							for(int j=0;j<h;j++)
							{
								if((j-r<0) )
								{
									continue;
								}
								if(i+r>w)
								{
									break;
								}
								sum=a[i,j]+a[i+r,j-r];
								
								s[sum]++;sum=0;
								
							}
							
						}
						
						break;
					}
				case 90:
					{
						
						int sum;
						for(int i=0;i<w;i++)
						{
							for(int j=0;j<h;j++)
							{
								
								if(!(j-r<0))
								{
									sum=a[i,j]+a[i,j-r];
									
									
									s[sum]++;sum=0;
								}
							}
							
						}
						
						
						break;
					}
				case 135:
					{
						
						int sum;
						for(int i=r;i<w;i++)
						{
							for(int j=r;j<h;j++)
							{
								
								if(!(i-r<0) || !(j-r<0))
								{
									sum=a[i,j]+a[i-r,j-r];
									
									s[sum]++;sum=0;
								}
							}
							
						}
						
						
						break;
						
					}
					
			}
			int count=0;
			for(int l=0;l<s.Length;l++)
			{
				count+=(int)s[l];
				
			}
			Console.WriteLine(count.ToString());
			for(int l=0;l<s.Length;l++)
			{
				s[l]/=count;
				
			}
			
			
			return s;
		}
		
		private static float[] calcdh(int r,int thetha,int [,]a,int level)
		{
			
			int w=a.GetLength(0);
			int h=a.GetLength(1);
			float[]d=new float[level];
			
			switch(thetha)
			{
				case 0:
					{
						
						int diff;
						
						for(int i=0;i<w-r;i++)
						{
							for(int j=0;j<h;j++)
							{
								
								if(!(i+r>w))
								{
									diff=a[i+r,j]-a[i,j];
									d[(int)(Utils.Scale(-level,level,0,level,diff))]++;
									diff=0;
								}
							}
							
						}
						
						break;
					}
				case 45:
					{
						
						int diff;
						
						for(int i=0;i<w-r;i++)
						{
							for(int j=0;j<h;j++)
							{
								
								if(i+r>w)
								{
									break;
									
								}
								if(j-r<0)
									
								{
									continue;
								}
								diff=a[i+r,j-r]-a[i,j];
								d[(int)(Utils.Scale(-level,level,0,level,diff))]++;
								diff=0;
							}
							
						}
						
						break;
					}
				case 90:
					{
						
						int diff;
						
						for(int i=0;i<w;i++)
						{
							for(int j=0;j<h;j++)
							{
								
								if(!(j-r<0))
								{
									diff=a[i,j-r]-a[i,j];
									d[(int)(Utils.Scale(-level,level,0,level,diff))]++;
									diff=0;
								}
							}
							
						}
						
						break;
					}
				case 135:
					{
						int diff;
						
						for(int i=r;i<w;i++)
						{
							for(int j=r;j<h;j++)
							{
								if(!(i-r<0)||!(j-r<0))
								{
									diff=a[i-r,j-r]-a[i,j];
									d[(int)(Utils.Scale(-level,level,0,level,diff))]++;
									diff=0;
								}
								
								
							}
							
						}
						
						break;
					}
					
			}
			int sum=0;
			for(int k=0;k<d.Length;k++)
			{
				sum+=(int)d[k];
			}
			Console.WriteLine(sum.ToString());
			for(int k=0;k<d.Length;k++)
			{
				d[k]/=sum;
			}
			
			return d;
		}
		

		#region Propiedades de Textura
		private static float homogenity(float []dh)
		{
			float sum=0.0f;
			for(int j=0;j<dh.Length;j++)
			{
				
				sum+=(float)((1.0f/(1.0f+(j*j)))*dh[j]);
			}
			
			return sum;
		}
		private static float energy(float []sh,float []dh)
		{
			float sums=0.0f;
			float sumd=0.0f;
			
			for(int i=0;i<sh.Length;i++)
			{
				sums+=(float)(sh[i]*sh[i]);
				
			}
			for(int i=0;i<dh.Length;i++)
			{
				sumd+=(float)(dh[i]*dh[i]);
			}
			return sums*sumd;
			
		}
		private static float entropy(float []sh,float []dh)
		{
			float sums=0.0F;
			float sumd=0.0F;
			
			float tmp=0.0f;
			for(int i=0;i<sh.Length;i++)
			{
				tmp=(float)(sh[i]*Math.Log(sh[i]));
				if(float.IsNaN(tmp))
				{
					tmp=0;
					
				}
				sums+=tmp;
				
			}
			tmp=0.0f;
			for(int j=0;j<dh.Length;j++)
			{
				tmp=(float)(dh[j]*Math.Log(dh[j]));
				if(float.IsNaN(tmp))
				{
					tmp=0;
					
				}
				sumd+=tmp;
				
			}
			
			return -sums-sumd;
			
			
		}
		private static float average(float []sh)
		{
			float sum=0.0F;
			for(int j=0;j<sh.Length;j++)
			{
				sum+=j*sh[j];
			}
			return sum/2;
			
		}
		private static float contrast(float []dh)
		{
			float sum=0.0F;
			for(int j=0;j<dh.Length;j++)
			{
				sum+=(float)(j*j)*dh[j];
			}
			return sum;
			
		}
		#endregion
		
		#region Imagenes de Propiedades de Textura
		public static Bitmap IH(Bitmap b,int ws)
		{
			
			int [,]Map=Utils.ToIntMap(b);
			int w=Map.GetLength(0);
			int h=Map.GetLength(1);
			float [,]I=new float[w,h];
			for(int x=0;x<w-ws;x++)
			{
				for(int y=0;y<h-ws;y++)
				{
					
					int x2=x+ws;
					int y2=y+ws;
					if(x2>w)
					{
						break;
					}
					if(y2>h)
					{
						break;
					}
					int[,]temp=Utils.SubArray(Map,x,x+ws,y,y+ws);
					float []dhc=dha(temp,GetLevel(temp));
					
					I[x,y]=homogenity(dhc);
					
					
				}
			}
			return Utils.DrawMap(I);
		}
		public static Bitmap IE(Bitmap b,int ws)
		{
			int [,]Map=Utils.ToIntMap(b);
			int w=Map.GetLength(0);
			int h=Map.GetLength(1);
			float [,]I=new float[w,h];
			for(int x=0;x<w-ws;x++)
			{
				for(int y=0;y<h-ws;y++)
				{
					
					int x2=x+ws;
					int y2=y+ws;
					if(x2>w)
					{
						break;
					}
					if(y2>h)
					{
						break;
					}
					int[,]temp=Utils.SubArray(Map,x,x+ws,y,y+ws);
					float []dhc=dha(temp,GetLevel(temp));
					float []shc=sha(temp,GetLevel(temp));
					I[x,y]=energy(shc,dhc);
					
					
					
				}
			}
			return Utils.DrawMap(I);
		}
		public static Bitmap IEY(Bitmap b,int ws)
		{
			int [,]Map=Utils.ToIntMap(b);
			int w=Map.GetLength(0);
			int h=Map.GetLength(1);
			float [,]I=new float[w,h];
			for(int x=0;x<w-ws;x++)
			{
				for(int y=0;y<h-ws;y++)
				{
					
					int x2=x+ws;
					int y2=y+ws;
					if(x2>w)
					{
						break;
					}
					if(y2>h)
					{
						break;
					}
					int[,]temp=Utils.SubArray(Map,x,x+ws,y,y+ws);
					float []dhc=dha(temp,GetLevel(temp));
					float []shc=sha(temp,GetLevel(temp));
					I[x,y]=entropy(shc,dhc);
					
					
				}
			}
			return Utils.DrawMap(I);
		}
		public static Bitmap IC(Bitmap b,int ws)
		{
			int [,]Map=Utils.ToIntMap(b);
			int w=Map.GetLength(0);
			int h=Map.GetLength(1);
			float [,]I=new float[w,h];
			for(int x=0;x<w-ws;x++)
			{
				for(int y=0;y<h-ws;y++)
				{
					
					int x2=x+ws;
					int y2=y+ws;
					if(x2>w)
					{
						break;
					}
					if(y2>h)
					{
						break;
					}
					int[,]temp=Utils.SubArray(Map,x,x+ws,y,y+ws);
					float []dhc=dha(temp,GetLevel(temp));
					
					I[x,y]=contrast(dhc);
					
					
				}
			}
			return Utils.DrawMap(I);
		}
		public static Bitmap IM(Bitmap b,int ws)
		{
			int [,]Map=Utils.ToIntMap(b);
			int w=Map.GetLength(0);
			int h=Map.GetLength(1);
			float [,]I=new float[w,h];
			for(int x=0;x<w-ws;x++)
			{
				for(int y=0;y<h-ws;y++)
				{
					
					int x2=x+ws;
					int y2=y+ws;
					if(x2>w)
					{
						break;
					}
					if(y2>h)
					{
						break;
					}
					int[,]temp=Utils.SubArray(Map,x,x+ws,y,y+ws);
					
					float []shc=sha(temp,GetLevel(temp));
					I[x,y]=average(shc);
					
					
				}
			}
			return Utils.DrawMap(I);
		}
		#endregion
		
	}
}
